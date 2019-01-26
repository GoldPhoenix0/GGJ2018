﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePiece : MonoBehaviour
{
    [System.Serializable]
    public struct PieceDimension
    {
        [System.Serializable]
        public class Row
        {
            public bool[] RowBools;
        }

        public Row[] Columns;


        public bool[,] PieceDimensions;
        public Vector2Int PivotPoint;

        public void Init()
        {
            int xLength = 0, yLength = 0;

            if(Columns != null)
            {
                yLength = Columns.Length;

                for (int i = 0; i < Columns.Length; i++)
                {
                    if (Columns[i] == null)
                        continue;

                    bool[] RowValues = Columns[i].RowBools;

                    if (RowValues == null)
                        continue;

                    xLength = Mathf.Max(xLength, RowValues.Length);
                }
            }

            PieceDimensions = new bool[xLength, yLength];

            for (int y = 0; y < PieceDimensions.GetLength(0); y++)
            {
                if (Columns == null || y >= Columns.Length || Columns[y] == null)
                    continue;

                bool[] rowBools = Columns[y].RowBools;

                for (int x = 0; x < PieceDimensions.GetLength(1); x++)
                {
                    bool setValue = false;

                    if (rowBools != null && x < rowBools.Length)
                        setValue = rowBools[x];

                    PieceDimensions[y, x] = setValue;
                }
            }
        }

        public Vector2Int GetRelativeIndex(int x, int y)
        {
            return new Vector2Int(PivotPoint.x - x, PivotPoint.y - y);
        }
    }

    public enum RotationDirection
    {
        Normal,
        Rotate90,
        Rotate180,
        Rotate270,
        Count
    }

    //public bool[,] PieceDimensions;
    //public Vector2Int PivotCoordinates;
    public PieceHighlight PieceHighlightPrefab;
    public Transform PieceHighlightHolder;

    public PieceDimension PieceInfo;
    public RotationDirection CurrentRotation;
    public PieceHighlight[] CurrentHighlights { get; protected set; }
    //protected Vector2Int[,] RelativeDirections;

    private void Awake()
    {
        PieceInfo.Init();

        InitHighlights();

        //Debug.Log("Done");

        //RelativeDirections = new Vector2Int[PieceDimensions.GetLength(0), PieceDimensions.GetLength(1)];

        //// Calculate the Dimensions here
        //for (int i = 0; i < PieceDimensions.GetLength(0); i++)
        //{
        //    for (int k = 0; k < PieceDimensions.GetLength(1); k++)
        //    {
        //        RelativeDirections[i, k] = new Vector2Int(i - PivotCoordinates.x, k - PivotCoordinates.y);
        //    }
        //}
    }

    protected void InitHighlights()
    {
        PieceDimension.Row[] columns = PieceInfo.Columns;

        if (columns == null)
            return;

        float width = 1f;
        List<PieceHighlight> attachedHighlights = new List<PieceHighlight>();
        for (int y = 0; y < columns.Length; y++)
        {
            bool[] rowItems = columns[y].RowBools;

            if (rowItems == null)
                continue;

            for (int x = 0; x < rowItems.Length; x++)
            {
                if (!rowItems[x])
                    continue;

                Vector2Int drawOffset = PieceInfo.GetRelativeIndex(x, y);
                Vector3 cubePosition = transform.localPosition;
                cubePosition += new Vector3(drawOffset.x * width, 0f, drawOffset.y);

                PieceHighlight thisHighlight = PieceHighlightHolder.InstantiateChild<PieceHighlight>(PieceHighlightPrefab.gameObject);
                thisHighlight.transform.localPosition = cubePosition;
            }
        }

        CurrentHighlights = attachedHighlights.ToArray();
    }

    public List<Vector2Int> GetRelativeLocationFromPoint(int x, int y)
    {
        return GetRelativeLocationFromPoint(x, y, CurrentRotation);
    }

    public List<Vector2Int> GetRelativeLocationFromPoint(int x, int y, RotationDirection direction)
    {
        List<Vector2Int> relativeLocations = new List<Vector2Int>();

        PieceDimension rotatedPieceLocations = GetRotatedPieceDimension(PieceInfo, direction);

        for (int i = 0; i < rotatedPieceLocations.PieceDimensions.GetLength(0); i++)
        {
            for (int k = 0; k < rotatedPieceLocations.PieceDimensions.GetLength(1); k++)
            {
                if (!rotatedPieceLocations.PieceDimensions[i, k])
                    continue;

                Vector2Int offsetLocation = rotatedPieceLocations.GetRelativeIndex(i, k);
                relativeLocations.Add(new Vector2Int(x + offsetLocation.x, y + offsetLocation.y));
            }
        }

        return relativeLocations;
    }

    public PieceDimension GetRotatedPieceDimension(PieceDimension normalDimensions, RotationDirection direction)
    {
        PieceDimension rotatedDirection = new PieceDimension();
        bool[,] PieceDimensions = normalDimensions.PieceDimensions;


        switch(direction)
        {
            case RotationDirection.Normal:
            case RotationDirection.Rotate180:
                {
                    rotatedDirection.PieceDimensions = new bool[PieceDimensions.GetLength(0), PieceDimensions.GetLength(1)];
                    int newRow = 0;
                    int newColumn = 0;

                    for (int i = 0; i < PieceDimensions.GetLength(0); i++)
                    {
                        newColumn = 0;
                        for (int k = 0; k < PieceDimensions.GetLength(1); k++)
                        {
                            int checkIndex = direction == RotationDirection.Rotate180 ? PieceDimensions.GetLength(1) - k - 1 : k;
                            rotatedDirection.PieceDimensions[newRow, newColumn] = PieceDimensions[i, checkIndex];

                            // Add in the new pivotPoint into this location
                            if (normalDimensions.PivotPoint.x == i && normalDimensions.PivotPoint.y == k)
                                rotatedDirection.PivotPoint = new Vector2Int(newRow, newColumn);

                            newColumn++;
                        }
                        newRow++;
                    }
                    break;
                }
            case RotationDirection.Rotate90:
            case RotationDirection.Rotate270:
                {
                    rotatedDirection.PieceDimensions = new bool[PieceDimensions.GetLength(1), PieceDimensions.GetLength(0)];
                    int newRow = 0;
                    int newColumn = 0;

                    for (int i = 0; i < PieceDimensions.GetLength(1); i++)
                    {
                        newColumn = 0;
                        for (int k = 0; k < PieceDimensions.GetLength(0); k++)
                        {
                            int checkIndex = direction == RotationDirection.Rotate270 ? PieceDimensions.GetLength(0) - k - 1 : k;
                            rotatedDirection.PieceDimensions[newRow, newColumn] = PieceDimensions[i, checkIndex];
                            
                            // Add in the new pivotPoint into this location
                            if (normalDimensions.PivotPoint.x == i && normalDimensions.PivotPoint.y == checkIndex)
                                rotatedDirection.PivotPoint = new Vector2Int(newRow, newColumn);

                            newColumn++;
                        }
                        newRow++;
                    }
                }
                break;

        }

        return rotatedDirection;
    }

    //public static bool[,] RotateMatrixCounterClockwise(bool[,] oldMatrix)
    //{
    //    bool[,] newMatrix = new bool[oldMatrix.GetLength(1), oldMatrix.GetLength(0)];
    //    int newColumn, newRow = 0;
    //    for (int oldColumn = oldMatrix.GetLength(1) - 1; oldColumn >= 0; oldColumn--)
    //    {
    //        newColumn = 0;
    //        for (int oldRow = 0; oldRow < oldMatrix.GetLength(0); oldRow++)
    //        {
    //            newMatrix[newRow, newColumn] = oldMatrix[oldRow, oldColumn];
    //            newColumn++;
    //        }
    //        newRow++;
    //    }
    //    return newMatrix;
    //}

    void OnDrawGizmosSelected()
    {
        PieceDimension.Row[] columns = PieceInfo.Columns;

        if (columns == null)
            return;

        float width = 1f;

        for (int y = 0; y < columns.Length; y++)
        {
            bool[] rowItems = columns[y].RowBools;

            if (rowItems == null)
                continue;

            for (int x = 0; x < rowItems.Length; x++)
            {
                if (!rowItems[x])
                    continue;

                Vector2Int drawOffset = PieceInfo.GetRelativeIndex(x, y);
                Vector3 cubePosition = transform.localPosition;
                cubePosition += new Vector3(drawOffset.x * width, 0f, drawOffset.y);

                Gizmos.DrawWireCube(cubePosition, new Vector3(width, width, width));
            }
        }

    }
}
