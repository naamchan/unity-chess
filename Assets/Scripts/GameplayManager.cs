using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Text;

public enum Side { White = 0, Black = 1 };

public enum ChessPieceType
{
    Pawn = 0,
    Rook = 1,
    Knight = 2,
    Bishop = 3,
    Queen = 4,
    King = 5,
}

[Serializable]
public class PieceLocation
{
    [SerializeField]
    public GameObject piecePrefab;

    [SerializeField]
    public ChessPieceType type;

    [SerializeField]
    public Side side;

    [SerializeField]
    public int row;

    [SerializeField]
    public int col;
}

[Serializable]
public class PieceTemplate
{
    [SerializeField]
    public ChessPieceType type;

    [SerializeField]
    public Side side;

    [SerializeField]
    public ChessPiece piecePrefab;
}

public class GameplayManager : MonoBehaviour
{
    [SerializeField]
    private ChessTile tilePrefab;

    [SerializeField]
    private Material[] tileMaterials;

    [SerializeField]
    private List<PieceLocation> startPieces;

    [SerializeField]
    private List<PieceTemplate> pieceTemplates;

    [SerializeField]
    private new Camera camera;

    [SerializeField]
    private Material movableTileMaterial;

    [SerializeField]
    private Canvas promoteCanvas;

    public static GameplayManager Instance { get { return instance; } }

    private static GameplayManager instance;

    public const int BoardWidth = 8;
    public const int BoardHeight = 8;
    private readonly Vector3 tileOffset = new Vector3(-3.5f, 0, -3.5f);

    private const int GameplayRaycastableLayer = 8;

    private List<List<ChessTile>> boards = new List<List<ChessTile>>();
    private List<List<ChessPiece>> pieces = new List<List<ChessPiece>>();

    private List<ChessTile> moveTargetTiles = new List<ChessTile>();
    private Side currentSide = Side.White;
    private ChessPiece currentPiece = null;

    Dictionary<Side, Dictionary<ChessPieceType, ChessPiece>> piecePrefabDatabase = new Dictionary<Side, Dictionary<ChessPieceType, ChessPiece>>();

    public ChessPiece GetPieceAt(int row, int column)
    {
        if (IsInBound(row, column))
            return pieces[row][column];
        return null;
    }

    private ChessTile GetTileAt(int row, int column)
    {
        if (IsInBound(row, column))
            return boards[row][column];
        return null;
    }

    public Side GetMySide()
    {
        return currentSide;
    }

    public bool IsOccupied(int row, int column)
    {
        return GetPieceAt(row, column) != null;
    }

    private void Awake()
    {
        //manage itself
        instance = this;

        foreach( var piece in pieceTemplates )
        {
            if (!piecePrefabDatabase.ContainsKey(piece.side))
                piecePrefabDatabase.Add(piece.side, new Dictionary<ChessPieceType, ChessPiece>());
            piecePrefabDatabase[piece.side].Add(piece.type, piece.piecePrefab);
        }
    }

    private void Start()
    {
        GenerateTile();
        GeneratePiece();
    }

    private void Update()
    {
        // TODO: Make this support both drag and click
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = camera.ScreenPointToRay(mousePosition);

            RaycastHit rayCastHit;
            if (Physics.Raycast(ray, out rayCastHit, 100f, 1 << GameplayRaycastableLayer))
            {
                OnRayCasted(rayCastHit);
            }
        }
    }

    private void FixedUpdate()
    {
        transform.position = transform.position + new Vector3(1f, 0, 0);
    }

    private void OnRayCasted(RaycastHit rayCastHit)
    {
        ChessTile chessTile = rayCastHit.collider.GetComponent<ChessTile>();
        ChessPiece chessPiece = rayCastHit.collider.GetComponent<ChessPiece>();
        if (chessTile != null)
        {
            if (pieces[chessTile.row][chessTile.col] != null)
                chessPiece = pieces[chessTile.row][chessTile.col];
        }
        else if( chessPiece != null)
        {
            chessTile = GetTileAt(chessPiece.Row, chessPiece.Column);
        }

        if (currentPiece == null)
        {
            if (chessPiece != null && chessPiece.Side == GetMySide())
            {
                Select(chessPiece);
            }
        }
        else
        {
            if (chessPiece == null)
            {
                if (isInPossibleMovement(chessTile))
                    Move(chessTile);
            }
            else
            {
                if (chessPiece == currentPiece)
                    Deselect();
                else if (chessPiece.Side == currentSide)
                {
                    Deselect();
                    Select(chessPiece);
                }
                else if (isInPossibleMovement(chessTile))
                    Move(chessTile);
            }
        }
    }

    private void Move(ChessTile chessTile)
    {
        if (pieces[chessTile.row][chessTile.col] != null)
        {
            RemovePiece(chessTile.row, chessTile.col);
        }

        currentPiece.OnMoved();
        MovePieceTo(currentPiece, chessTile.row, chessTile.col);

        currentSide = currentSide == Side.White ? Side.Black : Side.White;

        if (!(currentPiece.GetPieceType() == ChessPieceType.Pawn && (currentPiece.Row == 0 || currentPiece.Row == BoardHeight - 1)))
        {
            Deselect();
        }
        else
        {
            promoteCanvas.gameObject.SetActive(true);
        }
    }

    private void RemovePiece(int row, int col)
    {
        Destroy(pieces[row][col].gameObject);
        pieces[row][col] = null;
    }

    private void Select(ChessPiece chessPiece)
    {
        currentPiece = chessPiece;

        List<BoardPosition> possibleMovement = chessPiece.GetPossibleMove();
        for (int index = 0; index < possibleMovement.Count; index++)
        {
            ChessTile tile = boards[possibleMovement[index].Row][possibleMovement[index].Column];

            moveTargetTiles.Add(tile);
            tile.SetTemporaryMaterial(movableTileMaterial);
        }
    }

    private void Deselect()
    {
        currentPiece = null;

        foreach (ChessTile tile in moveTargetTiles)
        {
            tile.ResetMaterial();
        }
        moveTargetTiles.Clear();
    }

    private bool isInPossibleMovement(ChessTile chessTile)
    {
        foreach (var position in currentPiece.GetPossibleMove())
        {
            if (chessTile.row == position.Row && chessTile.col == position.Column)
                return true;
        }
        return false;
    }

    private void GeneratePiece()
    {
        for (int row = 0; row < BoardHeight; ++row)
        {
            pieces.Add(new List<ChessPiece>());

            for (int col = 0; col < BoardWidth; ++col)
            {
                pieces[row].Add(null);
            }
        }

        foreach (var startPiece in startPieces)
        {
            Spawn(piecePrefabDatabase[startPiece.side][startPiece.type], startPiece.row, startPiece.col);
        }
    }

    private void Spawn( ChessPiece prefab, int row, int col )
    {
        ChessPiece chessPiece = Instantiate(prefab);
        if (chessPiece != null)
        {
            chessPiece.Row = row;
            chessPiece.Column = col;
        }
        chessPiece.transform.position = GetPositionFromRowCol(row, col);

        pieces[row][col] = chessPiece;
    }

    public void Promote(ChessPiece piece, ChessPieceType type)
    {
        //Input validation
        if (piece.GetPieceType() != ChessPieceType.Pawn)
            return;

        if (!(type == ChessPieceType.Bishop || type == ChessPieceType.Queen || type == ChessPieceType.Knight || type == ChessPieceType.Rook))
            return;

        // type => GameObject
        ChessPiece promotedPiece = Instantiate(piecePrefabDatabase[piece.Side][type]);
        RemovePiece(piece.Row, piece.Column);
        MovePieceTo(promotedPiece, piece.Row, piece.Column);

        Deselect();
        Destroy(piece.gameObject);
        promoteCanvas.gameObject.SetActive(false);
    }

    private void MovePieceTo(ChessPiece piece, int row, int column)
    {
        ChessTile chessTile = boards[row][column];
        currentPiece.transform.position = chessTile.transform.position;

        pieces[currentPiece.Row][currentPiece.Column] = null;
        pieces[row][column] = currentPiece;

        currentPiece.Row = row;
        currentPiece.Column = column;

        PrintBoard();
    }

    private void GenerateTile()
    {
        for (int row = 0; row < BoardHeight; row++)
        {
            boards.Add(new List<ChessTile>());

            for (int col = 0; col < BoardWidth; col++)
            {
                ChessTile tile = Instantiate(tilePrefab);
                tile.col = col;
                tile.row = row;
                tile.transform.position = GetPositionFromRowCol(row, col);
                tile.SetPermanentMaterial(tileMaterials[(row + col) % 2]);
                tile.name = "Tile_" + row + "_" + col;

                boards[row].Add(tile);
            }
        }
    }

    private Vector3 GetPositionFromRowCol(int row, int col)
    {
        return new Vector3(col, 0, row) + tileOffset;
    }

    public bool IsInBound(int row, int col)
    {
        return row >= 0 && row < BoardHeight && col >= 0 && col < BoardWidth;
    }

    public ChessPiece GetPromotedPiece()
    {
        return currentPiece;
    }

    private void PrintBoard()
    {
        StringBuilder sb = new StringBuilder();
        
        foreach( var row in (pieces as IEnumerable<List<ChessPiece>>).Reverse())
        {
            foreach( var piece in row )
            {
                if (piece == null)
                    sb.Append("NU ");
                else
                    sb.Append(piece.GetShortName()).Append(" ");
            }
            sb.AppendLine();
        }

        Debug.Log(sb.ToString());
    }
}
