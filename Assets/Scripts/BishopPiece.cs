using UnityEngine;
using System.Collections;

public class BishopPiece : SimpleMovePiece
{
    public override int GetMoveDistance()
    {
        return 7;
    }

    public override bool IsAcceptable(int relativeRow, int relativeCol)
    {
        return Mathf.Abs(relativeRow) == Mathf.Abs(relativeCol);
    }

    public override string GetShortName()
    {
        return "BI";
    }
}
