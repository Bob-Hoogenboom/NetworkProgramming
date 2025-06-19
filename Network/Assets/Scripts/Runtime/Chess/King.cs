using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The King can move to any space around itself
/// </summary>
public class King : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        //Right row
        if(currentX + 1 < tileCountX)
        {
            //Right
            if (board[currentX + 1, currentY] == null)
            {
                moves.Add(new Vector2Int(currentX + 1, currentY));
            }
            else if(board[currentX + 1, currentY].team != team)
            {
                moves.Add(new Vector2Int(currentX + 1, currentY));
            }

            //Top Right
            if(currentY +1 < tileCountY)
            {
                if (board[currentX + 1, currentY + 1] == null)
                {
                    moves.Add(new Vector2Int(currentX + 1, currentY + 1));
                }
                else if (board[currentX + 1, currentY + 1].team != team)
                {
                    moves.Add(new Vector2Int(currentX + 1, currentY + 1));
                }
            }

            //Bottom Right
            if (currentY - 1 >= 0)
            {
                if (board[currentX + 1, currentY - 1] == null)
                {
                    moves.Add(new Vector2Int(currentX + 1, currentY - 1));
                }
                else if (board[currentX + 1, currentY - 1].team != team)
                {
                    moves.Add(new Vector2Int(currentX + 1, currentY - 1));
                }
            }
        }


        //Left row
        if (currentX - 1 >= 0)
        {
            //Left
            if (board[currentX - 1, currentY] == null)
            {
                moves.Add(new Vector2Int(currentX - 1, currentY));
            }
            else if (board[currentX - 1, currentY].team != team)
            {
                moves.Add(new Vector2Int(currentX - 1, currentY));
            }

            //Top Left
            if (currentY + 1 < tileCountY)
            {
                if (board[currentX - 1, currentY + 1] == null)
                {
                    moves.Add(new Vector2Int(currentX - 1, currentY + 1));
                }
                else if (board[currentX - 1, currentY + 1].team != team)
                {
                    moves.Add(new Vector2Int(currentX - 1, currentY + 1));
                }
            }

            //Bottom Left
            if (currentY - 1 >= 0)
            {
                if (board[currentX - 1, currentY - 1] == null)
                {
                    moves.Add(new Vector2Int(currentX - 1, currentY - 1));
                }
                else if (board[currentX - 1, currentY - 1].team != team)
                {
                    moves.Add(new Vector2Int(currentX - 1, currentY - 1));
                }
            }
        }

        //Up
        if(currentY + 1 < tileCountY)
        {
            if (board[currentX, currentY +1] == null || board[currentX, currentY +1].team != team)
            {
                moves.Add(new Vector2Int(currentX, currentY + 1));
            }
        }

        //Down
        if (currentY - 1 >= 0)
        {
            if (board[currentX, currentY - 1] == null || board[currentX, currentY - 1].team != team)
            {
                moves.Add(new Vector2Int(currentX, currentY - 1));
            }
        }

        return moves;

    }
}
