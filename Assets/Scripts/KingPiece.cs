using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class KingPiece : SimpleMovePiece
{
    public override int GetMoveDistance()
    {
        return 1;
    }

    public override bool IsAcceptable(int relativeRow, int relativeCol)
    {
        return true;
    }

    public override void OnMoved()
    {
        base.OnMoved();
    }

    public override string GetShortName()
    {
        return "KI";
    }
}
