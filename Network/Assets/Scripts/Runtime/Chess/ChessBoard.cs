using System.Collections.Generic;
using TMPro;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Vector2Int tileCount = new Vector2Int(8, 8);
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

    private Camera _cam;
    private GameObject[,] _tiles;
    private Vector2Int _currentHover;

    private bool _isWhiteTurn;
    
    [Header("User Interface")]
    [SerializeField] private GameObject resultWindow;
    [SerializeField] private TMP_Text whiteScoreTXT;
    [SerializeField] private TMP_Text blackScoreTXT;
    [SerializeField] private GameObject rematchIndicator;
    [SerializeField] private GameObject leaveIndicator;
    [SerializeField] private Button rematchBTN;
    [Space]
    [SerializeField] private int whiteScore = 0;
    [SerializeField] private int blackScore = 0;


    [Header("Multiplayer")]
    private int _playerCount = -1;
    private int _currentTeam = -1;

    private bool _localGame = true;
    private bool[] _playerRematch = new bool[2];




    private void Start()
    {
        _cam = Camera.main;
        _isWhiteTurn = true;
        resultWindow.SetActive(false);

        GernerateGrid(tileSize, tileCount);

        SpawnAllPieces();
        PositionAllPieces();

        RegisterEvent();
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
                    if ((_chessPieces[hitPos.x, hitPos.y].team == 0 && _isWhiteTurn && _currentTeam == 0) || (_chessPieces[hitPos.x, hitPos.y].team == 1 && !_isWhiteTurn && _currentTeam == 1))   
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
               
                //if the move is not in the available moves it cannot be moved to
                if (ContainsValidMove(ref _availableMoves, new Vector2(hitPos.x, hitPos.y)))
                {
                    MoveTo(previousPosition.x, previousPosition.y, hitPos.x, hitPos.y);

                    //Net Implementation
                    NetMakeMove mm = new NetMakeMove();
                    mm.originalX = previousPosition.x;
                    mm.originalY = previousPosition.y;
                    mm.destinationX = hitPos.x;
                    mm.destinationY = hitPos.y;
                    mm.teamId = _currentTeam;

                    Client.instance.SendToServer(mm);
                }
                else
                {
                    _currentlyDragging.SetPosition( GetTileCenter(previousPosition.x, previousPosition.y));
                    _currentlyDragging = null;
                    RemoveHighlightTiles();
                }
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

    #region Generate Board
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
    #endregion


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
            _chessPieces[i, 1] = SpawnOnePiece(ChessPieceType.PAWN, whiteTeam);
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
            _chessPieces[i, 6] = SpawnOnePiece(ChessPieceType.PAWN, blackTeam);
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


    #region Game Functions
    public void CheckMate(int winner)
    {
        // Do any internal logic here, e.g., stop game, etc.

        // Fire the event to notify others
        DisplayWinning(winner);
    }

    public void GameReset()
    {
        //UI
        rematchBTN.interactable = true;

        rematchIndicator.SetActive(false);
        leaveIndicator.SetActive(false);
        resultWindow.SetActive(false);

        //references
        _currentlyDragging = null;

        _playerRematch[0] = _playerRematch[1] = false;

        for (int x = 0; x < tileCount.x; x++)
        {
            for (int y = 0; y < tileCount.y; y++)
            {
                if (_chessPieces[x,y] != null)
                {
                    Destroy(_chessPieces[x, y].gameObject);
                }
                _chessPieces[x, y] = null;
            }
        }

        for (int i = 0; i < _defeatedWhite.Count; i++)
        {
            Destroy(_defeatedWhite[i].gameObject);
        }

        for (int i = 0; i < _defeatedBlack.Count; i++)
        {
            Destroy(_defeatedBlack[i].gameObject);
        }

        _defeatedWhite.Clear();
        _defeatedBlack.Clear();

        SpawnAllPieces();
        PositionAllPieces();
        _isWhiteTurn = true; //white always starts
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
        return -Vector2Int.one; //invalid, should never happen
    }

    private void MoveTo(int originalX, int originalY, int x, int y)
    {
        ChessPiece piece = _chessPieces[originalX, originalY];
        Vector2Int previousPos = new Vector2Int(originalX, originalY);

        //check for piece on the target pos
        if (_chessPieces[x,y] != null)
        {
            ChessPiece otherPiece = _chessPieces[x,y];

            //other piece is of the same team
            if(piece.team == otherPiece.team)
            {
                return;
            }

            if(otherPiece.team == 0)
            {
                if (otherPiece.type == ChessPieceType.KING) CheckMate(1);    

                _defeatedWhite.Add(otherPiece);
                otherPiece.SetScale(Vector3.one * deathSize);
                otherPiece.SetPosition(new Vector3(8 * tileSize, 0, -1 * tileSize) 
                    + new Vector3(tileSize / 2, yOffset, tileSize / 2)
                    + (Vector3.forward * defeatedPawnMargin) * _defeatedWhite.Count);
            }
            else
            {
                if (otherPiece.type == ChessPieceType.KING) CheckMate(0);
                
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

        _isWhiteTurn = !_isWhiteTurn; //Toggle boolean to swap turns
        if (_localGame)
        {
            _currentTeam = (_currentTeam == 0) ? 1 : 0;
        }

        //TODO specialMoves*

        if (_currentlyDragging)
        {
            _currentlyDragging = null;
        }
        RemoveHighlightTiles();

        //TODO specialMoves Checkmate*
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

    # region Events
    private void RegisterEvent()
    {
        NetUtility.S_WELCOME += OnWelcomeServer;
        NetUtility.S_MAKE_MOVE += OnMakeMoveServer;
        NetUtility.S_REMATCH += OnRematchServer;

        NetUtility.C_WELCOME += OnWelcomeClient;
        NetUtility.C_START_GAME += OnStartGameClient;
        NetUtility.C_MAKE_MOVE += OnMakeMoveClient;
        NetUtility.C_REMATCH += OnRematchClient;

        GameUI.instance.setLocalGame += OnSetLocalGame;
    }

    private void UnregisterEvents()
    {
        NetUtility.S_WELCOME -= OnWelcomeServer;
        NetUtility.S_MAKE_MOVE -= OnMakeMoveServer;
        NetUtility.S_REMATCH -= OnRematchServer;

        NetUtility.C_WELCOME -= OnWelcomeClient;
        NetUtility.C_START_GAME -= OnStartGameClient;
        NetUtility.C_MAKE_MOVE -= OnMakeMoveClient;
        NetUtility.C_REMATCH -= OnRematchClient;

        GameUI.instance.setLocalGame -= OnSetLocalGame;
    }

    //Server
    private void OnWelcomeServer(NetMessage msg, NetworkConnection cnn)
    {
        //client has connected, assign a team and return a message
        NetWelcome nw = msg as NetWelcome;

        //assign team
        nw.assignedTeam = ++_playerCount; //the Host initiates a server and also joins as a client*

        //return back to client
        Server.instance.SendToClient(cnn, nw);

        //if black team joins #initial value -1 (+1 = 0 white team ), (+1 = 1 black team)
        if(_playerCount == 1)
        {
            Server.instance.Broadcast(new NetStartGame());
        }
    }

    private void OnMakeMoveServer(NetMessage msg, NetworkConnection cnn)
    {
        //recieve, broadcast is back
        NetMakeMove mm = msg as NetMakeMove;

        Server.instance.Broadcast(mm);
    }

    private void OnRematchServer(NetMessage msg, NetworkConnection cnn)
    {
        Server.instance.Broadcast(msg);
    }

    //Client
    private void OnWelcomeClient(NetMessage msg)
    {
        //recieve connection message
        NetWelcome nw = msg as NetWelcome;

        //assign team
        _currentTeam = nw.assignedTeam;

        if(_localGame && _currentTeam == 0)
        {
            Server.instance.Broadcast(new NetStartGame());
        }

        Debug.Log($"My assigned team is {nw.assignedTeam}");
    }

    private void OnStartGameClient(NetMessage msg)
    {
        //Change the camera
        if (_localGame)
        {
            GameUI.instance.ChangeCamera(CameraType.LOCAL_GAME);
        }
        else
        {
            GameUI.instance.ChangeCamera((_currentTeam == 0) ? CameraType.WHITE_TEAM : CameraType.BLACK_TEAM);
        }
    }

    private void OnMakeMoveClient(NetMessage msg)
    {
        NetMakeMove mm = msg as NetMakeMove;
        
        Debug.Log($"MM : {mm.teamId} : {mm.originalX} ,{mm.originalY} -> {mm.destinationX} ,{mm.destinationY}");

        if (mm.teamId != _currentTeam)
        {
            //TODO change for special move*
            ChessPiece target = _chessPieces[mm.originalX, mm.originalY];

            _availableMoves = target.GetAvailableMoves(ref _chessPieces, tileCount.x, tileCount.y);
            //specialMove = target.GetSpecialMoves(ref _chessPieces, ref moveList, ref _availableMoves);

            MoveTo(mm.originalX, mm.originalY, mm.destinationX, mm.destinationY);
        }
    }

    private void OnRematchClient(NetMessage msg)
    {
        //recieved connection rematch
        NetRematch rm = msg as NetRematch;

        _playerRematch[rm.teamId] = rm.wantRematch == 1;

        //activate UI
        if(rm.teamId != _currentTeam)
        {
            if(rm.wantRematch == 1)
            {
                rematchIndicator.SetActive(true);
            }
            else
            {
                leaveIndicator.SetActive(true);
                rematchBTN.interactable = false;
            }

        }

        if (_playerRematch[0] && _playerRematch[1])
        {
            GameReset();
        }

    }

    //local
    private void OnSetLocalGame(bool value)
    {
        _playerCount = -1;
        _currentTeam = -1;

        _localGame = value;
    }
    #endregion

    #region User Interface
    private void DisplayWinning(int winner)
    {
        resultWindow.SetActive(true);

        //make a nice window for displaying a win
        //TODO make it better because this is not networked so its better to just say which team won
        if (winner == 0)
        {
            whiteScore++;

            whiteScoreTXT.text = $"White Won This Round!\nScore: {whiteScore}";
            blackScoreTXT.text = $"Black Score:\n{blackScore}";

            //if you are white display that YOU have won

            Debug.Log("White wins! White Score: " + whiteScore);
        }
        else if (winner == 1)
        {
            blackScore++;

            whiteScoreTXT.text = $"White Score:\n{whiteScore}";
            blackScoreTXT.text = $"Black Won This Round!\nScore: {blackScore}";

            //if you are black display that YOU have won

            Debug.Log("Black wins! Black Score: " + blackScore);
        }

    }

    public void OnRematchBTN()
    {
        if (_localGame)
        {
            NetRematch wrm = new NetRematch();
            wrm.teamId = 0;
            wrm.wantRematch = 1;

            Client.instance.SendToServer(wrm);


            NetRematch brm = new NetRematch();
            brm.teamId = 1;
            brm.wantRematch = 1;

            Client.instance.SendToServer(brm);
        }
        else
        {
            NetRematch rm = new NetRematch();
            rm.teamId = _currentTeam;
            rm.wantRematch = 1;

            Client.instance.SendToServer(rm);
        }
    }

    public void OnMenuBTN()
    {
        NetRematch rm = new NetRematch();
        rm.teamId = _currentTeam;
        rm.wantRematch = 0;

        Client.instance.SendToServer(rm);

        GameReset();
        GameUI.instance.OnLeaveFromGameMenu();

        Invoke("ShutDownRelay", 1.0f);

        _playerCount = -1;
        _currentTeam = -1;
    }

    private void ShutDownRelay()
    {
        Client.instance.Shutdown();
        Server.instance.Shutdown();
    }
    #endregion
}