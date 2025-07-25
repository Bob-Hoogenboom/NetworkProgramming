using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Rook can move Horizontally and Vertically on the board
/// The Rook can NOT jump over enemy pieces
/// </summary>
public class Rook : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        //Down
        for (int i = currentY- 1; i >= 0; i--)
        {
            if (board[currentX, i] == null)
            {
                moves.Add(new Vector2Int(currentX, i));
            }
            if (board[currentX, i] != null)
            {
                if (board[currentX, i].team != team)
                {
                    moves.Add(new Vector2Int(currentX, i));
                    break;
                }
            }
        }

        //Up
        for (int i = currentY + 1; i < tileCountY; i++)
        {
            if (board[currentX, i] == null)
            {
                moves.Add(new Vector2Int(currentX, i));
            }
            if (board[currentX, i] != null)
            {
                if (board[currentX, i].team != team)
                {
                    moves.Add(new Vector2Int(currentX, i));
                    break;
                }
            }
        }

        //Left
        for (int i = currentX - 1; i >= 0; i--)
        {
            if (board[i, currentY] == null)
            {
                moves.Add(new Vector2Int(i, currentY));
            }
            if (board[i, currentY] != null)
            {
                if (board[i, currentY].team != team)
                {
                    moves.Add(new Vector2Int(i, currentY));
                    break;
                }
            }
        }

        //Right
        for (int i = currentX + 1; i < tileCountX ; i++)
        {
            if (board[i, currentY] == null)
            {
                moves.Add(new Vector2Int(i, currentY));
            }
            if (board[i, currentY] != null)
            {
                if (board[i, currentY].team != team)
                {
                    moves.Add(new Vector2Int(i, currentY));
                    break;
                }
            }
        }

        return moves;
    }
}
