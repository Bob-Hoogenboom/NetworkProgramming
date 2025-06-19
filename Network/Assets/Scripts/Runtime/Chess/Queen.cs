using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Queen is simply a combination of Rook and Bishop
/// The Queen can move diagonally, horizontally and vertically
/// </summary>
public class Queen : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        #region Rook Directions
        //Down
        for (int i = currentY - 1; i >= 0; i--)
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
        for (int i = currentX + 1; i < tileCountX; i++)
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
        #endregion

        #region Bishop Directions
        //Top Right
        for (int x = currentX + 1, y = currentY + 1; x < tileCountX && y < tileCountY; x++, y++)
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
        #endregion

        return moves;

    }
}