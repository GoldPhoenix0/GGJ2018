using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public BasePiece[] PiecePrefabs;
    public BoardGridLocation BoardGridLocationPrefab;
    public Transform BoardGridHolder;
    public Transform PieceHolder;

    public BasePiece tempPiece;
    public BasePiece.RotationDirection checkRotation;

    public BoardGridLocation[,] BoardGridLocations { get; protected set; }

    private void Start()
    {
        Init();
    }

    public Coroutine Init()
    {
        return StartCoroutine(InitAsync());
    }

    public IEnumerator InitAsync()
    {
        GenerateBoard();

        tempPiece = PieceHolder.InstantiateChild<BasePiece>(PiecePrefabs[0].gameObject);

        yield break;
    }
	

    public void GenerateBoard(int gridX = 10, int gridY = 10)
    {
        BoardGridLocations = new BoardGridLocation[gridX, gridY];

        for (int x = 0; x < BoardGridLocations.GetLength(0); x++)
        {
            for (int y = 0; y < BoardGridLocations.GetLength(1); y++)
            {
                BoardGridLocation thisLocation = BoardGridHolder.InstantiateChild<BoardGridLocation>(BoardGridLocationPrefab.gameObject, x + "_" + y + BoardGridLocationPrefab.name);
                float width = thisLocation.Width;
                thisLocation.transform.localPosition = new Vector3(width * x,0f,  width * y);
                thisLocation.X = x;
                thisLocation.Y = y;
                BoardGridLocations[x, y] = thisLocation;
                thisLocation.SetMaterial(x + y);
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                BoardGridLocation hoveredTransform = hit.transform.GetComponent<BoardGridLocation>();

                if (hoveredTransform == null)
                    return;

                Debug.Log("hit object at " + hoveredTransform.X + "," + hoveredTransform.Y);

                tempPiece.transform.localPosition = hoveredTransform.transform.localPosition;
            }
        }
        if(Input.GetMouseButtonDown(1))
        {
            ChangeRotation(-1);
        }
    }

    public void ChangeRotation(int dir)
    {
        int nextRotationIndex = ((int)checkRotation + dir + (int)BasePiece.RotationDirection.Count) % (int)BasePiece.RotationDirection.Count;
        checkRotation = (BasePiece.RotationDirection)nextRotationIndex;

        //if (tempPiece != null)
        //   tempPiece.RotatePiece(checkRotation);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x">X location of the Pivot Point</param>
    /// <param name="y">Y location of the Pivot Point</param>
    /// <param name="piece"></param>
    public bool PlacePiece(int x, int y, BasePiece piece, BasePiece.RotationDirection rotationDir)
    {
        if (piece == null)
            return false;

        BoardGridLocation selectedLocation = GetValidLocationAtPosition(x, y);

        if (selectedLocation == null)
            return false;

        List<Vector2Int> piecePositions = piece.GetRelativeLocationFromPoint(selectedLocation.X, selectedLocation.Y, rotationDir);

        bool isValid = true;
        List<BoardGridLocation> foundBoardLocations = new List<BoardGridLocation>(piecePositions.Count);

        for (int i = 0; i < piecePositions.Count; i++)
        {
            Vector2Int checkLocationPosition = piecePositions[i];

            BoardGridLocation checkLocation = GetLocationAtPosition(checkLocationPosition);

            if(checkLocation == null)
            {
                isValid = false;
                continue;
            }

            if (checkLocation.InUse)
                isValid = false;

            foundBoardLocations.Add(checkLocation);
        }


        return isValid;
    }

    public BoardGridLocation GetValidLocationAtPosition(Vector2Int checkPos)
    {
        return GetValidLocationAtPosition(checkPos.x, checkPos.y);
    }

    public BoardGridLocation GetValidLocationAtPosition(int x, int y)
    {
        if (BoardGridLocations == null || BoardGridLocations.GetLength(0) <= 0 || BoardGridLocations.GetLength(1) <= 0)
            return null;

        int closestXLocation = Mathf.Clamp(x, 0, BoardGridLocations.GetLength(0) - 1);
        int closestYLocation = Mathf.Clamp(y, 0, BoardGridLocations.GetLength(1) - 1);

        return BoardGridLocations[closestXLocation, closestYLocation];
    }

    public BoardGridLocation GetLocationAtPosition(Vector2Int checkPos)
    {
        return GetLocationAtPosition(checkPos.x, checkPos.y);
    }

    public BoardGridLocation GetLocationAtPosition(int x, int y)
    {
        if (BoardGridLocations == null || x >= BoardGridLocations.GetLength(0) || y >= BoardGridLocations.GetLength(1))
            return null;

        return BoardGridLocations[x, y];
    }
}
