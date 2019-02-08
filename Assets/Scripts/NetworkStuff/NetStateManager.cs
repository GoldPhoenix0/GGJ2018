using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class NetStateManager : NetworkBehaviour
{

    public enum gameState {
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

	private NetUI netManager;	// As it's handling more than UI now.

	[SerializeField]
	private float ScoringSpeed = 1.0f;

	[SerializeField]
	private float RestartGameDelay = 5.0f;

	public gameState currentState = gameState.Init;

	[SyncVar]
	public int PlayerID=0;

	private float _timer;

	// Use this for initialization
	void Start ()
	{

		GSM = FindObjectOfType<GameScoreManager> ();
		netManager = FindObjectOfType<NetUI> ();

		// Only call this for the local player netStateManager otherwise it will overwrite other peoples player ID's on the server
		if (isLocalPlayer) {
			CmdAddPlayer ();
		}

		netManager.registerPlayer (this);


		InitRound ();
	}

	[Command]
	private void CmdAddPlayer ()
	{
		// TODO check and handle if -1 is returned aka no room for this new player.
		PlayerID = netManager.GetNextPlayerID ();
	}

	private void InitRound ()
	{
		for (int i = 0; i < PersistentData.instance.NumberOfPlayers; i++) {
			netManager.PlayerPlacements [i] = null;
		}

		netManager.ResetForNewRound ();

		if (isLocalPlayer) {
			BoardManager.instance.GetNextPiece ();
			BoardManager.instance.UpdatePlayerColor (PlayerID);
			GSM.SelectPlayer (PlayerID);
		}
		currentState = gameState.PlayerXPlace;

	}

	public void CommitPlacement ()
	{
		// Don't make other player pieces commit.
		if (isLocalPlayer != true)
			return;

		if (currentState != gameState.PlayerXPlace)
			return;

		if (!BoardManager.instance.CanPlaceCurrentPiece ())
			return;

		netManager.PlayerPlacements [PlayerID] = BoardManager.instance.CurrentSelectedPiece;

		EndPlayerTurn ();

		// prepare for next player
		GSM.DeSelectPlayer (PlayerID);

		// Let Server know player has commited to their placement
		CmdCommitPiece ( netManager.PlayerPlacements [PlayerID].PieceIndex,netManager.PlayerPlacements [PlayerID].PieceLocation, netManager.PlayerPlacements[PlayerID].CurrentRotation );
	}


	[Command]
	private void CmdCommitPiece (int pIndex, Vector3 pLoc, BasePiece.RotationDirection pRot )
	{
		Debug.Log (pIndex + "-" + pLoc + "\n" + pRot);
		RpcPiecePlacement (pIndex, pLoc, pRot);

		if (netManager.TrackCommit (PlayerID)) {
			RpcAllPiecesPlaced ();
		}
	}

	[ClientRpc]
	public void RpcPiecePlacement (int pIndex, Vector3 pLoc, BasePiece.RotationDirection pRot)
	{
		if (!isLocalPlayer) {
			// create this piece
			netManager.PlaceOtherPlayerPiece (PlayerID, pIndex, pLoc, pRot);
		}
	}

	[ClientRpc]
	private void RpcAllPiecesPlaced ()
	{	
		
		ScoreCurrentRound ();
	}


	private void EndPlayerTurn ()
	{
		// Stop this player from doing anything
		currentState = gameState.PlayerXFinish;

		BoardManager.instance.PlayUnknownAudioOnPiece (BoardManager.instance.CurrentSelectedPiece);
		BoardManager.instance.CurrentSelectedPiece = null;
		netManager.PlayerPlacements [PlayerID].gameObject.SetActive (false);

	}

	private void ScoreCurrentRound ()
	{
	// Only one of the NetStateManager scripts needs to do this.  Probably should have split the state stuff away from the network stuff
		_timer = 0;
		netManager.SetAllGameStates (gameState.ScoreRound);

		for (int i = 0; i < PersistentData.instance.NumberOfPlayers; i++) {
			for (int j = i + 1; j < PersistentData.instance.NumberOfPlayers; j++) {
				if (netManager.PlayerPlacements [i].DoesOtherPieceCollide (netManager.PlayerPlacements [j])) {
					netManager.PieceCollides [i] = true;
					netManager.PieceCollides [j] = true;
					break;
				}
			}
		}
	}

	private void EndRound ()
	{
		// prep for next round


		for (int i = 0; i < PersistentData.instance.NumberOfPlayers; i++) {

			if (netManager.PieceCollides [i]) {
				Destroy (netManager.PlayerPlacements [i].gameObject);
				netManager.PlayerPlacements [i] = null;
			} else {
				GSM.PlayerScoreDelta (i, netManager.PlayerPlacements [i].PiecePoints);
				BoardManager.instance.PlacePiece (netManager.PlayerPlacements [i]);
			}
		}

		if (BoardManager.instance.IsGameOver ()) {
			netManager.SetAllGameStates (gameState.RestartGame);
			netManager.OnGameOverStart ();
			return;
		}

		InitRound ();

	}


	private void RevealPlacedPiece (int IDofPlayer)
	{
		netManager.PlayerPlacements [IDofPlayer].ColorPiece (PersistentData.instance.PlayerColors [IDofPlayer], true);
		netManager.PlayerPlacements [IDofPlayer].gameObject.SetActive (true);

	}


	private IEnumerator RevealPlayerPieces ()
	{
		float revealTimer = 0;

		for (int i = 0; i < PersistentData.instance.NumberOfPlayers; i++) {
			while (revealTimer <= ScoringSpeed) {
				revealTimer += Time.deltaTime;
				yield return null;
			}
			RevealPlacedPiece (i);
			if (netManager.PieceCollides [i]) {
				// Play collision sound!
				BoardManager.instance.OnItemsCrashed (netManager.PlayerPlacements [i]);
			} else {
				// Possible Particles here as well
				SFXManager.instance.PlayAudioClip (netManager.PlayerPlacements [i].PlacementAudio);
			}
		}

	}


	// Update is called once per frame
	void Update ()
	{
		if (isLocalPlayer) {
			// If all players have commited their pieces, start the RevealPlayerPieces coroutine?
			if (currentState == gameState.ScoreRound) {

				StartCoroutine (RevealPlayerPieces ());
				_timer = 0;
				netManager.SetAllGameStates(gameState.EndRound);

			} else if (currentState == gameState.EndRound) {
				_timer += Time.deltaTime;

				if (_timer >= ScoringSpeed * PersistentData.instance.NumberOfPlayers) {
					EndRound ();
				}
			}
		}
	}
}
