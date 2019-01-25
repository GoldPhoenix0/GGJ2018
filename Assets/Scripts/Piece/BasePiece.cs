using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePiece : MonoBehaviour
{
    public struct PieceDimension
    {
        public bool[,] PieceDimensions;
        public Vector2Int PivotPoint;

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
    public PieceDimension PieceInfo;
    public RotationDirection CurrentRotation;

    //protected Vector2Int[,] RelativeDirections;

    private void Awake()
    {
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
                            rotatedDirection.PieceDimensions[newRow, newColumn] = PieceDimensions[i, k];

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
                            rotatedDirection.PieceDimensions[newRow, newColumn] = PieceDimensions[i, k];
                            
                            // Add in the new pivotPoint into this location
                            if (normalDimensions.PivotPoint.x == i && normalDimensions.PivotPoint.y == k)
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
}
