using System.Collections.Generic;
using UnityEngine;

public enum ChessPieceType
{
    NONE = 0,
    PAWN = 1,
    ROOK = 2,
    KNIGHT = 3,
    BISHOP = 4,
    QUEEN = 5,
    KING = 6
}

public class ChessPiece : MonoBehaviour
{
    public float dragSpeed = 10f;

    public int team; // white team = 0 and black team  = 1
    public int currentX;
    public int currentY;
    public ChessPieceType type;

    //used for lerp animations
    private Vector3 _desiredPosition;
    private Vector3 _desiredScale = Vector3.one;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, _desiredPosition, Time.deltaTime * dragSpeed);
        transform.localScale = Vector3.Lerp(transform.localScale, _desiredScale, Time.deltaTime * dragSpeed);
    }

    public virtual List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        r.Add(new Vector2Int(3, 3));
        r.Add(new Vector2Int(4, 3));
        r.Add(new Vector2Int(3, 4));
        r.Add(new Vector2Int(4, 4));

        return r;
    }

    public virtual void SetPosition(Vector3 pos, bool force = false)
    {
        _desiredPosition = pos;
        if (force)
        {
            transform.position = _desiredPosition;
        }
    }

    public virtual void SetScale(Vector3 scale, bool force = false)
    {
        _desiredScale = scale;
        if (force)
        {
            transform.localScale = _desiredScale;
        }
    }
}
 