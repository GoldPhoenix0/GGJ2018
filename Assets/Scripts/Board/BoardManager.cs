using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public BasePiece[] BasePieces;

    public BoardGridLocation[,] BoardGridLocations;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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
