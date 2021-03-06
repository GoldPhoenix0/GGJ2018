﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour {

    public enum gameState
    {
        Paused,
        Init,
        PlayerXPlace,
        PlayerXFinish,
        ScoreRound,
        EndRound,
        RestartGame

    }
    [SerializeField]
    private GameScoreManager GSM;

    [SerializeField]
    private float ScoringSpeed = 1.0f;

    //private float RestartGameDelay = 5.0f;

    public gameState currentState = gameState.Init;
    private int startPlayer = 0;
    private int CurrentPlayer = 0;
    private float _timer;

    private BasePiece[] PlayerPlacements;
    private bool[] PieceCollides;

    // Use this for initialization
    void Start()
    {
        PlayerPlacements = new BasePiece[PersistentData.instance.NumberOfPlayers];
        PieceCollides = new bool[PersistentData.instance.NumberOfPlayers];
        CurrentPlayer = 0;
        startPlayer = 0;
        InitRound();
    }

    private void InitRound()
    {
        for (int i = 0; i < PersistentData.instance.NumberOfPlayers; i++)
        {
            PlayerPlacements[i] = null;
            PieceCollides[i] = false;
        }

        GSM.DeSelectPlayer(CurrentPlayer);
        CurrentPlayer = startPlayer;

        BoardManager.instance.GetNextPiece();
        BoardManager.instance.UpdatePlayerColor(CurrentPlayer);

        currentState = gameState.PlayerXPlace;

        GSM.SelectPlayer(CurrentPlayer);
    }
	
    public void CommitPlacement( )
    {
        if (currentState != gameState.PlayerXPlace)
            return;

        if (!BoardManager.instance.CanPlaceCurrentPiece())
            return;

        PlayerPlacements[CurrentPlayer] = BoardManager.instance.CurrentSelectedPiece;

        EndPlayerTurn();

        // prepare for next player
        GSM.DeSelectPlayer(CurrentPlayer);
        AdvanceCurrentPlayer();
        GSM.SelectPlayer(CurrentPlayer);
        // see if we've gotten back to the first player of this round
        if(CurrentPlayer == startPlayer)    // We've looped through all the players
        {

            GSM.DeSelectPlayer(CurrentPlayer);
            currentState = gameState.ScoreRound;
            _timer = 0;
            ScoreCurrentRound();
        }
        else // Next players turn
        {
            currentState = gameState.PlayerXPlace;
            BoardManager.instance.UpdatePlayerColor(CurrentPlayer);
            BoardManager.instance.GetNextPiece();
            //BoardManager.instance.UpdatePiece(0);
        }

    }

    private void EndPlayerTurn()
    {
        currentState = gameState.PlayerXFinish;

        // TODO Get the BasePiece to handle how it hides itself incase just making the whole game object inactive screws with accessing data on it.
        BoardManager.instance.PlayUnknownAudioOnPiece(BoardManager.instance.CurrentSelectedPiece);
        BoardManager.instance.CurrentSelectedPiece = null;
        PlayerPlacements[CurrentPlayer].gameObject.SetActive(false);

    }

    private void ScoreCurrentRound()
    {
        _timer = 0;

        for(int i = 0 ; i < PersistentData.instance.NumberOfPlayers; i++)
        {
            for(int j = i+1; j < PersistentData.instance.NumberOfPlayers; j++)
            {
                if( PlayerPlacements[i].DoesOtherPieceCollide( PlayerPlacements[j] )   )
                {
                    PieceCollides[i] = true;
                    PieceCollides[j] = true;
                    break;
                }
            }
        }

    }

    private void EndRound()
    {
        // prep for next round
        // Every round the start player moves on for the next round

        startPlayer++;
        if (startPlayer >= PersistentData.instance.NumberOfPlayers)
            startPlayer = 0;



        for(int i = 0; i < PersistentData.instance.NumberOfPlayers; i++)
        {

            if(PieceCollides[i])
            {
                Destroy(PlayerPlacements[i].gameObject);
                PlayerPlacements[i] = null;
            }
            else
            {
                GSM.PlayerScoreDelta(i, PlayerPlacements[i].PiecePoints);
                BoardManager.instance.PlacePiece(PlayerPlacements[i]);
            }
        }

        if(BoardManager.instance.IsGameOver())
        {
            OnGameOverStart();
            return;
        }

        InitRound();

    }

    public void OnGameOverStart()
    {
        // Show Game Ending Flair Here!
        currentState = gameState.RestartGame;
        _timer = 0;
        BoardManager.instance.CurrentSelectedPiece = null;
        GSM.ShowResultsScreen();
    }

    private void RevealPlacedPiece()
    {
        PlayerPlacements[CurrentPlayer].ColorPiece(PersistentData.instance.PlayerColors[CurrentPlayer], true);
        PlayerPlacements[CurrentPlayer].gameObject.SetActive(true);

    }

    private void AdvanceCurrentPlayer()
    {
        CurrentPlayer++;

        // loop around
        if (CurrentPlayer >= PersistentData.instance.NumberOfPlayers)
            CurrentPlayer = 0;

    }

    private int PreviousPlayer()
    {
        int PrevPlayer = CurrentPlayer;

        PrevPlayer--;
        if (PrevPlayer < 0)
            PrevPlayer = PersistentData.instance.NumberOfPlayers - 1;

        return PrevPlayer;
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    // Update is called once per frame
    void Update () {
        if(currentState == gameState.ScoreRound)
        {
            _timer += Time.deltaTime;

            if(_timer >= ScoringSpeed)
            {
                _timer = 0;

                GSM.DeSelectPlayer(PreviousPlayer());
                GSM.SelectPlayer(CurrentPlayer);
                RevealPlacedPiece();
                if(PieceCollides[CurrentPlayer])
                {
                    // Play collision sound!
                    BoardManager.instance.OnItemsCrashed(PlayerPlacements[CurrentPlayer]);
                }
                else
                {
                    // Possible Particles here as well
                    SFXManager.instance.PlayAudioClip(PlayerPlacements[CurrentPlayer].PlacementAudio);
                }

                AdvanceCurrentPlayer();
                if(CurrentPlayer == startPlayer)
                {
                    _timer = 0;
                    currentState = gameState.EndRound;
                    
                }
            }
        }
        else if(currentState == gameState.EndRound)
        {
            _timer += Time.deltaTime;

            if (_timer >= ScoringSpeed)
            {
                GSM.DeSelectPlayer(PreviousPlayer());
                EndRound();
            }
        }
        else if(currentState == gameState.RestartGame)
        {
            //_timer += Time.deltaTime;

            //if (_timer > RestartGameDelay)
            //    RestartGame();
        }
	}
}
