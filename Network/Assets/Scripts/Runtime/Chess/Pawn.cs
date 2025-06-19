using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Pawn has 3 moves,
/// 1. 1st space in front of the pawn. towards the opponent
/// 2. 2nd space in front of the pawn, towards the opponent
/// 3. A pawn can slay an opponent pawn one space diagonaly
/// </summary>
public class Pawn : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        int dir = (team == 0) ? 1 : -1;

        //Move 1.
        if (board[currentX, currentY + dir] == null)
        {
            moves.Add(new Vector2Int(currentX, currentY + dir));
        }

        //Move 2.
        if (board[currentX, currentY + dir] == null)
        {
            //White team
            if(team == 0 && currentY == 1 && board[currentX, currentY + (dir * 2)] == null)
            {
                moves.Add(new Vector2Int(currentX, currentY + (dir * 2)));
            }
            //Black team
            if (team == 1 && currentY == 6 && board[currentX, currentY + (dir * 2)] == null)
            {
                moves.Add(new Vector2Int(currentX, currentY + (dir * 2)));
            }
        }

        //Move 3.
        if(currentX != tileCountX - 1)
        {
            if (board[currentX + 1, currentY + dir] != null && board[currentX + 1, currentY + dir].team != team)
            {
                moves.Add(new Vector2Int(currentX + 1, currentY + dir));
            }
        }

        if (currentX != 0)
        {
            if (board[currentX - 1, currentY + dir] != null && board[currentX - 1, currentY + dir].team != team)
            {
                moves.Add(new Vector2Int(currentX - 1, currentY + dir));
            }
        }


        return moves;
    }
}