using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KnightPiece : ChessPiece
{
    public override List<BoardPosition> GetPossibleMove()
    {
        List<BoardPosition> possibleMoves = new List<BoardPosition>();

        for(int numCol = -2; numCol <= 2; numCol++)
        {
            if (numCol == 0)
                continue;

            int absRow = 3 - Mathf.Abs(numCol);

            if(GameplayManager.Instance.IsInBound(Row + absRow, Column + numCol ))
            {
                if(IsMovableToDestination(absRow, numCol))
                    possibleMoves.Add(new BoardPosition(Row - absRow, Column + numCol));
            }

            if (GameplayManager.Instance.IsInBound(Row - absRow, Column + numCol))
            {
                if (IsMovableToDestination(-absRow, numCol))
                    possibleMoves.Add(new BoardPosition(Row - absRow, Column + numCol));
            }
        }

        return possibleMoves;
    }

    public override string GetShortName()
    {
        return "KN";
    }
}


