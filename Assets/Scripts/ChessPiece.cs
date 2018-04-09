using System;
using System.Collections.Generic;
using UnityEngine;

public struct BoardPosition
{
    public int Row;
    public int Column;

    public BoardPosition(int row, int column)
    {
        Row = row;
        Column = column;
    }
}

public abstract class ChessPiece : MonoBehaviour
{
    public int Row;
    public int Column;
    public Side Side { get { return side; } }

    [SerializeField]
    private Side side;

    [SerializeField]
    private ChessPieceType type;

    public abstract List<BoardPosition> GetPossibleMove();
    public abstract string GetShortName();

    public bool IsMovableToDestination(int relativeRow, int relativeCol)
    {
        ChessPiece pieceAtDestination = GameplayManager.Instance.GetPieceAt(Row + relativeRow, Column + relativeCol);
        return (pieceAtDestination == null || pieceAtDestination.Side != GameplayManager.Instance.GetMySide());
    }

    public virtual void OnMoved()
    {
        Debug.Log(name + " " + Row + ":" + Column + " Moved");
    }

    public ChessPieceType GetPieceType()
    {
        return type;
    }
}

public abstract class SimpleMovePiece : ChessPiece
{
    private BoardPosition[] deltaBoard = new BoardPosition[]
    {
        new BoardPosition(0, -1),
        new BoardPosition(0, 1),
        new BoardPosition(1, 0),
        new BoardPosition(-1, 0),
        new BoardPosition(-1, -1),
        new BoardPosition(1, 1),
        new BoardPosition(1, -1),
        new BoardPosition(-1, 1),
    };

    public override List<BoardPosition> GetPossibleMove()
    {
        List<BoardPosition> possibleMoves = new List<BoardPosition>();

        foreach (var deltaPosition in deltaBoard)
        {
            for (int i = 1; i <= GetMoveDistance(); ++i)
            {
                int numRow = i * deltaPosition.Row;
                int numCol = i * deltaPosition.Column;

                if (!IsAcceptable(numRow, numCol) || !IsMovableToDestination(numRow, numCol))
                {
                    break;
                }

                if (GameplayManager.Instance.IsInBound(Row + numRow, Column + numCol))
                {
                    possibleMoves.Add(new BoardPosition(Row + numRow, Column + numCol));
                }
            }
        }

        return possibleMoves;
    }

    public abstract int GetMoveDistance();
    public abstract bool IsAcceptable(int relativeRow, int relativeCol);
}