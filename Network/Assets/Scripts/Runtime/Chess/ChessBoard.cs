using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// This script is attached to the chessboard Model and handles the generation of tiles and selecting those tiles
/// </summary>
public class ChessBoard : MonoBehaviour
{
    [Header("Prefabs & Materials")]
    [SerializeField] private Material tileMat;
    [Space]
    [Tooltip("The order of the gameobjects matter. The order is the same as the 'ChessPieceType' enum")]
    [SerializeField] private GameObject[] chessPrefabs;
    [SerializeField] private Material[] teamMaterial;

    [Header("Variables")]
    [SerializeField] private Vector2Int tileCount = new Vector2Int(8,8);
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float deathSize = 0.3f;
    [SerializeField] private float defeatedPawnMargin = 0.3f;
    [SerializeField] private float dragOffset = 1f;
    [SerializeField] private float yOffset = 0.2f;

    private ChessPiece[,] _chessPieces;
    private ChessPiece _currentlyDragging;
    private List<ChessPiece> _defeatedWhite = new List<ChessPiece>();
    private List<ChessPiece> _defeatedBlack = new List<ChessPiece>();
    private List<Vector2Int> _availableMoves = new List<Vector2Int>();

    private GameObject[,] _tiles;
    private Camera _cam;
    private Vector2Int _currentHover;


    private void Awake()
    {
        _cam = Camera.main;
        GernerateGrid(tileSize, tileCount);

        SpawnAllPieces();
        PositionAllPieces();
    }

    private void Update()
    {
        RaycastHit info;
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight")))
        {
            //get the index of the tile hit
            Vector2Int hitPos = ReturnTileIndex(info.collider.gameObject);
            
            //if we are not hovering any tile
            if(_currentHover == -Vector2Int.one)
            {
                _currentHover = hitPos;
                _tiles[hitPos.x, hitPos.y].layer = LayerMask.NameToLayer("Hover");
            }

            //if we already hovered a tile, change previous
            if (_currentHover != hitPos)
            {
                _tiles[_currentHover.x, _currentHover.y].layer = (ContainsValidMove(ref _availableMoves, _currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                _currentHover = hitPos;
                _tiles[hitPos.x, hitPos.y].layer = LayerMask.NameToLayer("Hover");
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (_chessPieces[hitPos.x, hitPos.y] != null)
                {
                    //is it out turn?
                    if (true)   //TODO change for turn based chess
                    {
                        _currentlyDragging = _chessPieces[hitPos.x, hitPos.y];

                        //get list of where you can move towards
                        _availableMoves = _currentlyDragging.GetAvailableMoves(ref _chessPieces, tileCount.x, tileCount.y);

                        HighlightTiles();
                    }
                }
            }

            if (_currentlyDragging != null && Input.GetMouseButtonUp(0))
            {
                Vector2Int previousPosition = new Vector2Int(_currentlyDragging.currentX, _currentlyDragging.currentY);

                bool validMove = MoveTo(_currentlyDragging, hitPos.x, hitPos.y);
                if (!validMove)
                {
                    _currentlyDragging.SetPosition( GetTileCenter(previousPosition.x, previousPosition.y));
                }                
                
                _currentlyDragging = null;
                RemoveHighlightTiles();
            }
        }
        else
        {
            if(_currentHover != -Vector2Int.one)
            {
                _tiles[_currentHover.x, _currentHover.y].layer = (ContainsValidMove(ref _availableMoves, _currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                _currentHover = -Vector2Int.one;
            }

            if(_currentlyDragging && Input.GetMouseButtonUp(0))
            {
                _currentlyDragging.SetPosition(GetTileCenter(_currentlyDragging.currentX, _currentlyDragging.currentY));
                _currentlyDragging = null;
                RemoveHighlightTiles();
            }
        }

        if (_currentlyDragging)
        {
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up);
            float dist = 0.0f;
            if(horizontalPlane.Raycast(ray, out dist))
            {
                _currentlyDragging.SetPosition(ray.GetPoint(dist) + Vector3.up * dragOffset);
            }
        }
    }

    //generate board
    private void GernerateGrid(float tileSize, Vector2Int tileCount)
    {
        _tiles = new GameObject[this.tileCount.x, this.tileCount.y];
        for (int x = 0; x < this.tileCount.x; x++)
        { 
            for (int y = 0; y < this.tileCount.y; y++)
            {
                _tiles[x, y] = GenerateSingleTile(tileSize, x, y);
            }
        }
    }
    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format($"X:({x}), Y: ({y})"));
        tileObject.transform.parent = transform;    //generates the tiles as children of this object*

        //omdat een quad prefab meegeven saai is*
        Mesh mesh  = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMat;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, 0, y * tileSize);
        vertices[1] = new Vector3(x * tileSize, 0, (y + 1) * tileSize);
        vertices[2] = new Vector3((x + 1) * tileSize, 0, y * tileSize);
        vertices[3] = new Vector3((x + 1) * tileSize, 0, (y + 1) * tileSize);

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tris;

        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }


    #region Spawn Pieces
    private void SpawnAllPieces()
    {
        _chessPieces = new ChessPiece[tileCount.x, tileCount.y];

        int whiteTeam = 0;
        int blackTeam = 1;

        //White Team
        _chessPieces[0, 0] = SpawnOnePiece(ChessPieceType.ROOK, whiteTeam);
        _chessPieces[1, 0] = SpawnOnePiece(ChessPieceType.KNIGHT, whiteTeam);
        _chessPieces[2, 0] = SpawnOnePiece(ChessPieceType.BISHOP, whiteTeam);
        _chessPieces[3, 0] = SpawnOnePiece(ChessPieceType.QUEEN, whiteTeam);
        _chessPieces[4, 0] = SpawnOnePiece(ChessPieceType.KING, whiteTeam);
        _chessPieces[5, 0] = SpawnOnePiece(ChessPieceType.BISHOP, whiteTeam);
        _chessPieces[6, 0] = SpawnOnePiece(ChessPieceType.KNIGHT, whiteTeam);
        _chessPieces[7, 0] = SpawnOnePiece(ChessPieceType.ROOK, whiteTeam);

        for (int i = 0; i < tileCount.x; i++)
        {
            //_chessPieces[i, 1] = SpawnOnePiece(ChessPieceType.PAWN, whiteTeam);
        }

        //Black Team
        _chessPieces[0, 7] = SpawnOnePiece(ChessPieceType.ROOK, blackTeam);
        _chessPieces[1, 7] = SpawnOnePiece(ChessPieceType.KNIGHT, blackTeam);
        _chessPieces[2, 7] = SpawnOnePiece(ChessPieceType.BISHOP, blackTeam);
        _chessPieces[4, 7] = SpawnOnePiece(ChessPieceType.KING, blackTeam);
        _chessPieces[3, 7] = SpawnOnePiece(ChessPieceType.QUEEN, blackTeam);
        _chessPieces[5, 7] = SpawnOnePiece(ChessPieceType.BISHOP, blackTeam);
        _chessPieces[6, 7] = SpawnOnePiece(ChessPieceType.KNIGHT, blackTeam);
        _chessPieces[7, 7] = SpawnOnePiece(ChessPieceType.ROOK, blackTeam);

        for (int i = 0; i < tileCount.x; i++)
        {
            //_chessPieces[i, 6] = SpawnOnePiece(ChessPieceType.PAWN, blackTeam);
        }

    }
    private ChessPiece SpawnOnePiece(ChessPieceType type, int team)
    {
        ChessPiece piece = Instantiate(chessPrefabs[(int)type - 1], transform).GetComponent<ChessPiece>();

        piece.type = type;
        piece.team = team;
        piece.GetComponentInChildren<MeshRenderer>().material = teamMaterial[team];

        return piece;
    }
    #endregion

    #region Positioning
    private void PositionAllPieces()
    {
        for (int x = 0; x < tileCount.x; x++)
        {
            for (int y = 0; y < tileCount.y; y++)
            {
                if (_chessPieces[x,y] != null)
                {
                    PositionOnePiece(new Vector2Int(x,y), true);
                }
            }
        }
    }

    private void PositionOnePiece(Vector2Int gridPos, bool force = false)
    {
        _chessPieces[gridPos.x, gridPos.y].currentX = gridPos.x;
        _chessPieces[gridPos.x, gridPos.y].currentY = gridPos.y;
        _chessPieces[gridPos.x, gridPos.y].SetPosition(GetTileCenter(gridPos.x, gridPos.y));
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, 0, y * tileSize) + new Vector3(tileSize/2, 0, tileSize/2);
    }
    #endregion


    #region Highlight Tiles
    private void HighlightTiles()
    {
        for (int i = 0; i < _availableMoves.Count; i++)
        {
            _tiles[_availableMoves[i].x, _availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
        }
    }

    private void RemoveHighlightTiles()
    {
        for (int i = 0; i < _availableMoves.Count; i++)
        {
            _tiles[_availableMoves[i].x, _availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
        }

        _availableMoves.Clear();
    }
    #endregion


    #region Operations
    private Vector2Int ReturnTileIndex(GameObject hitInfo) 
    { 
        for (int x = 0; x < tileCount.x; x++)
        {
            for(int y = 0; y < tileCount.y; y++)
            {

                if (_tiles[x,y] == hitInfo)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return -Vector2Int.one; // invalid, should never happen
    }

    private bool MoveTo(ChessPiece piece, int x, int y)
    {
        //if the move is not in the available moves it cannot be moved to
        if (!ContainsValidMove(ref _availableMoves, new Vector2(x, y))) return false;

        Vector2Int previousPos = new Vector2Int(piece.currentX, piece.currentY);

        //check for piece on the target pos
        if (_chessPieces[x,y] != null)
        {
            ChessPiece otherPiece = _chessPieces[x,y];

            //other piece is of the same team
            if(piece.team == otherPiece.team)
            {
                return false;
            }

            if(otherPiece.team == 0)
            {
                _defeatedWhite.Add(otherPiece);
                otherPiece.SetScale(Vector3.one * deathSize);
                otherPiece.SetPosition(new Vector3(8 * tileSize, 0, -1 * tileSize) 
                    + new Vector3(tileSize / 2, yOffset, tileSize / 2)
                    + (Vector3.forward * defeatedPawnMargin) * _defeatedWhite.Count);
            }
            else
            {
                _defeatedBlack.Add(otherPiece);
                otherPiece.SetScale(Vector3.one * deathSize);
                otherPiece.SetPosition(new Vector3(-1 * tileSize, 0, 8 * tileSize)
                    + new Vector3(tileSize / 2, yOffset, tileSize / 2)
                    + (Vector3.back * defeatedPawnMargin) * _defeatedBlack.Count);
            }

            
        }

        _chessPieces[x,y] = piece;
        _chessPieces[previousPos.x, previousPos.y] = null;

        PositionOnePiece(new Vector2Int(x,y), true);

        return true;
    }

    private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2 pos)
    {
        for (int i = 0; i < moves.Count; i++)
        {
            if (moves[i].x == pos.x && moves[i].y == pos.y)
            {
                return true;
            }
        }
        return false;
    }
    #endregion
}