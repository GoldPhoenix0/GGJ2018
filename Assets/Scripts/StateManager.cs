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
        EndRound

    }
    [SerializeField]
    private GameScoreManager GSM;

    [SerializeField]
    private float ScoringSpeed = 1.0f;

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

        CurrentPlayer = startPlayer;

        BoardManager.instance.GetNextPiece();

        currentState = gameState.PlayerXPlace;
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
        AdvanceCurrentPlayer();
        // see if we've gotten back to the first player of this round
        if(CurrentPlayer == startPlayer)    // We've looped through all the players
        {
            currentState = gameState.ScoreRound;
            ScoreCurrentRound();
        }
        else // Next players turn
        {
            currentState = gameState.PlayerXPlace;
            BoardManager.instance.UpdatePiece(0);
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


        InitRound();

    }

    private void RevealPlacedPiece()
    {
        PlayerPlacements[CurrentPlayer].gameObject.SetActive(true);
    }

    private void AdvanceCurrentPlayer()
    {
        CurrentPlayer++;

        // loop around
        if (CurrentPlayer >= PersistentData.instance.NumberOfPlayers)
            CurrentPlayer = 0;
    }

	// Update is called once per frame
	void Update () {
        if(currentState == gameState.ScoreRound)
        {
            _timer += Time.deltaTime;

            if(_timer >= ScoringSpeed)
            {
                _timer = 0;
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

            if(_timer >= ScoringSpeed)
                EndRound();
        }
	}
}