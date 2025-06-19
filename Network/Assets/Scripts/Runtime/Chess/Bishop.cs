using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Bishop can move diagonally in all four directions
/// The Bishop can NOT jump over enemy pieces
/// </summary>
public class Bishop : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        //Top Right
        for (int x = currentX + 1, y = currentY + 1; x < tileCountX && y < tileCountY; x++, y++)
        {
            if (board[x,y] == null)
            {
                moves.Add(new Vector2Int(x, y));
            }

            else
            {
                if (board[x, y].team != team)
                {
                    moves.Add(new Vector2Int(x, y));
                }
            }
        }

        //Top Left
        for (int x = currentX - 1, y = currentY + 1; x >= 0 && y < tileCountY; x--, y++)
        {
            if (board[x, y] == null)
            {
                moves.Add(new Vector2Int(x, y));
            }

            else
            {
                if (board[x, y].team != team)
                {
                    moves.Add(new Vector2Int(x, y));
                }
            }
        }

        //Bottom Right
        for (int x = currentX + 1, y = currentY - 1; x < tileCountX && y >= 0; x++, y--)
        {
            if (board[x, y] == null)
            {
                moves.Add(new Vector2Int(x, y));
            }

            else
            {
                if (board[x, y].team != team)
                {
                    moves.Add(new Vector2Int(x, y));
                }
            }
        }

        //Bottom Left
        for (int x = currentX - 1, y = currentY - 1; x >= 0 && y >= 0; x--, y--)
        {
            if (board[x, y] == null)
            {
                moves.Add(new Vector2Int(x, y));
            }

            else
            {
                if (board[x, y].team != team)
                {
                    moves.Add(new Vector2Int(x, y));
                }
            }
        }

        return moves;
    }
}
