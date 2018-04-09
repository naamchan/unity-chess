using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QueenPiece : SimpleMovePiece
{
    public override int GetMoveDistance()
    {
        return 7;
    }

    public override bool IsAcceptable(int relativeRow, int relativeCol)
    {
        return (relativeRow == 0 || relativeCol == 0) || Mathf.Abs(relativeRow) == Mathf.Abs(relativeCol);
    }

    public override string GetShortName()
    {
        return "QU";
    }
}