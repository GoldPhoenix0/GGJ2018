using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;

    public BasePiece[] PiecePrefabs;
    public BoardGridLocation BoardGridLocationPrefab;
    public Transform BoardGridHolder;
    public Transform PieceHolder;

    public BasePiece CurrentSelectedPiece;
    public BasePiece.RotationDirection CurrentPieceRotation;
    protected int LastPieceIndex = 0;

    public AudioClip[] UnknownPlacementAudioClips;
    public AudioClip CrashAudioClip;
    public AudioClip RotateAudioClip;
    public AudioClip SelectLocationAudioClip;

    public BoardGridLocation[,] BoardGridLocations { get; protected set; }

    protected float timeBeforeScrollWheel = 0.25f;
    protected float currentScrollWheelDuration = 0f;
    protected BoardGridLocation previousTouchedBoardGrid;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);

        instance = this;
    }

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

    private void Update()
    {
        // Do nothing while we don't have a piece
        if (CurrentSelectedPiece == null)
            return;

        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                BoardGridLocation hoveredTransform = hit.transform.GetComponent<BoardGridLocation>();

                if (hoveredTransform == null)
                    return;

                //Debug.Log("hit object at " + hoveredTransform.X + "," + hoveredTransform.Y);

                CurrentSelectedPiece.transform.localPosition = hoveredTransform.transform.localPosition;
                CurrentSelectedPiece.UpdateHits();

                if(hoveredTransform != previousTouchedBoardGrid)
                    SFXManager.instance.PlayAudioClip(SelectLocationAudioClip);

                previousTouchedBoardGrid = hoveredTransform;
            }
        }
        if(Input.GetMouseButtonDown(1))
        {
            ChangeRotation(1);
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            PlacePiece(CurrentSelectedPiece);
        }


        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");

        if(currentScrollWheelDuration <= 0f && Mathf.Abs(mouseWheel) > 0.01f)
        {
            CyclePiece((int)Mathf.Sign(mouseWheel));
            currentScrollWheelDuration = timeBeforeScrollWheel;
        }

        currentScrollWheelDuration -= Time.deltaTime;
    }

    public bool CanPlaceCurrentPiece()
    {
        return CanPlacePiece(CurrentSelectedPiece);
    }

    public bool CanPlacePiece(BasePiece checkPiece)
    {
        if (checkPiece == null)
            return false;

        bool isValid = true;

        PieceHighlight[] highlightArray = checkPiece.CurrentHighlights;

        for (int i = 0; i < highlightArray.Length; i++)
        {
            if(highlightArray[i] == null || !highlightArray[i].IsGridPositionValid() || highlightArray[i].FoundGridPosition == null)
                isValid = false;
        }

        return isValid;
    }

    public void PlacePiece(BasePiece checkPiece)
    {
        if (!CanPlacePiece(checkPiece))
            return;

        PieceHighlight[] highlightArray = checkPiece.CurrentHighlights;

        for (int i = 0; i < highlightArray.Length; i++)
        {
            // we aren't valid... we should stop now!
            if (highlightArray[i] == null)
                return;

            highlightArray[i].HideHighlight();

            BoardGridLocation foundBoard = highlightArray[i].FoundGridPosition;

            foundBoard.InUse = true;

            //SFXManager.instance.PlayAudioClip(checkPiece.PlacementAudio);
        }
    }

    public void GetNextPiece()
    {
        CurrentSelectedPiece = null;
        CurrentPieceRotation = BasePiece.RotationDirection.Normal;

        UpdatePiece(0);
    }

    public void CyclePiece(int dir)
    {
        UpdatePiece(LastPieceIndex + dir);
    }

    public void UpdatePiece(int index)
    {
        LastPieceIndex = (PiecePrefabs.Length + index) % PiecePrefabs.Length;

        BasePiece piece = PiecePrefabs[LastPieceIndex];

        Vector3 newPosition = Vector3.zero;
        if (CurrentSelectedPiece != null)
        {
            newPosition = CurrentSelectedPiece.transform.localPosition;
            Destroy(CurrentSelectedPiece.gameObject);
        }

        CurrentSelectedPiece = PieceHolder.InstantiateChild<BasePiece>(piece.gameObject);
        CurrentSelectedPiece.transform.localPosition = newPosition;
        CurrentSelectedPiece.RotatePiece(CurrentPieceRotation);
    }

    public void ChangeRotation(int dir)
    {
        int nextRotationIndex = ((int)CurrentPieceRotation + dir + (int)BasePiece.RotationDirection.Count) % (int)BasePiece.RotationDirection.Count;
        CurrentPieceRotation = (BasePiece.RotationDirection)nextRotationIndex;

        if (CurrentSelectedPiece != null)
        {
            CurrentSelectedPiece.RotatePiece(CurrentPieceRotation);
            SFXManager.instance.PlayAudioClip(RotateAudioClip);
        }
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

    public bool IsGameOver()
    {
        int numPlayers = PersistentData.instance.NumberOfPlayers;

        int numTilesRemaining = 0;
        for (int x = 0; x < BoardGridLocations.GetLength(0); x++)
        {
            for (int y = 0; y < BoardGridLocations.GetLength(1); y++)
            {
                BoardGridLocation checkLocation = BoardGridLocations[x, y];

                if (checkLocation == null || checkLocation.InUse)
                    continue;

                numTilesRemaining++;
            }
        }

        return numTilesRemaining < numPlayers;
    }

    public void OnItemsCrashed(BasePiece pieceCrashed)
    {
        SFXManager.instance.PlayAudioClip(CrashAudioClip);

        if (pieceCrashed == null)
            return;

        //Play particles here
    }

    public void PlayUnknownAudioOnPiece(BasePiece piece)
    {
        if (piece == null)
            return;

        int pointSize = piece.PiecePoints;

        if (pointSize >= 6)
            PlayUnknownAudio(3);
        else if (pointSize >= 3)
            PlayUnknownAudio(1);
        else
            PlayUnknownAudio(0);
    }

    public void PlayUnknownAudio(int sizeIndex)
    {
        if (UnknownPlacementAudioClips == null)
            return;

        sizeIndex = Mathf.Clamp(sizeIndex, 0, UnknownPlacementAudioClips.Length - 1);
        SFXManager.instance.PlayAudioClip(UnknownPlacementAudioClips[sizeIndex]);
    }
}
