using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PawnPiece : ChessPiece
{
    [SerializeField]
    private bool IsMoved = false;

    [SerializeField]
    private bool IsMoveUp;

    public override List<BoardPosition> GetPossibleMove()
    {
        List<BoardPosition> possibleMoves = new List<BoardPosition>();

        // 1 step
        if (!GameplayManager.Instance.IsOccupied(Row + GetMoveDistance(1), Column))
            possibleMoves.Add(new BoardPosition(Row + GetMoveDistance(1), Column));

        // 2 step
        if (!IsMoved && !GameplayManager.Instance.IsOccupied(Row + GetMoveDistance(2), Column))
        {
            possibleMoves.Add(new BoardPosition(Row + GetMoveDistance(2), Column));
        }

        //Capture
        if( GameplayManager.Instance.GetPieceAt( Row+ GetMoveDistance(1), Column - 1) != null )
        {
            possibleMoves.Add(new BoardPosition(Row + GetMoveDistance(1), Column - 1));
        }

        if (GameplayManager.Instance.GetPieceAt(Row + GetMoveDistance(1), Column + 1) != null)
        {
            possibleMoves.Add(new BoardPosition(Row + GetMoveDistance(1), Column + 1));
        }

        return possibleMoves;
    }

    private int GetMoveDirectionMultiplier()
    {
        return IsMoveUp ? 1 : -1;
    }

    private int GetMoveDistance(int distance)
    {
        return IsMoveUp ? distance : -distance;
    }

    public override void OnMoved()
    {
        base.OnMoved();

        IsMoved = true;
    }

    public override string GetShortName()
    {
        return "PA";
    }
}
