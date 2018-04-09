using UnityEngine;
using System.Collections;

public class RookPiece : SimpleMovePiece
{
    public override int GetMoveDistance()
    {
        return 7;
    }

    public override bool IsAcceptable(int relativeRow, int relativeCol)
    {
        return (relativeRow == 0 || relativeCol == 0);
    }

    public override void OnMoved()
    {
        base.OnMoved();
    }

    public override string GetShortName()
    {
        return "RO";
    }
}
