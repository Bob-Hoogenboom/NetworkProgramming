using UnityEngine;

/// <summary>
/// This script is attached to the chessboard Model and handles the generation of tiles and selecting those tiles
/// </summary>
public class ChessBoard : MonoBehaviour
{
    [Header("Graphics")]
    [SerializeField] private Material tileMat;

    [Header("Variables")]
    [SerializeField] private Vector2Int tileCount = new Vector2Int(8,8);
    [SerializeField] private float tileSize = 1.0f;

    private GameObject[,] _tiles;
    private Camera _currentCam;
    private Vector2Int currentHover;


    private void Awake()
    {
        GernerateGrid(tileSize, tileCount);
    }

    private void Update()
    {
        if (!_currentCam)
        {
            _currentCam = Camera.current;
            return;
        }

        RaycastHit info;
        Ray ray = _currentCam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile")))
        {
            //get the index of the tile hit
            Vector2Int hitPos = ReturnTileIndex(info.collider.gameObject);
            
            //if we are not hovering any tile
            if(currentHover == -Vector2Int.one)
            {
                currentHover = hitPos;
                _tiles[hitPos.x, hitPos.y].layer = LayerMask.NameToLayer("Hover");
            }

            //if we already hovered a tile, change previous
            if (currentHover != hitPos)
            {
                _tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tiles");
                currentHover = hitPos;
                _tiles[hitPos.x, hitPos.y].layer = LayerMask.NameToLayer("Hover");
            }
        }
        else
        {

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
}