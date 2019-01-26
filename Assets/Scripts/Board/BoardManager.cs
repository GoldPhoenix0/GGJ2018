using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public BasePiece[] PiecePrefabs;
    public BoardGridLocation BoardGridLocationPrefab;
    public Transform BoardGridHolder;


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
        GenerateBoard(PersistentData.instance.BoardXSize, PersistentData.instance.BoardYSize);

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



    /// <summary>
    /// 
    /// </summary>
    /// <param name="x">X location of the Pivot Point</param>
    /// <param name="y">Y location of the Pivot Point</param>
    /// <param name="piece"></param>
    public void PlacePiece(int x, int y, BasePiece piece)
    {
        //Vector2Int[] Positions
    }
}
