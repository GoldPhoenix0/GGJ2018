using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class NetStateManager : NetworkBehaviour
{

    public enum gameState {
		WaitingForPlayers,
		Init,
		PlayerXPlace,
		PlayerXFinish,
		ScoreRound,
		Scoring,
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

	public gameState currentState = gameState.WaitingForPlayers;

	[SyncVar]
	public int PlayerID=-1;

	[SyncVar]
	public int boardX;
	[SyncVar]
	public int boardY;
	[SyncVar]
	public int NumOfPlayers;

	private float _timer;

	private void Awake ()
	{
		currentState = gameState.WaitingForPlayers;
	}

	// Use this for initialization
	void Start ()
	{
		GSM = FindObjectOfType<GameScoreManager> ();
		netManager = FindObjectOfType<NetUI> ();

		// Only call this for the local player netStateManager otherwise it will overwrite other peoples player ID's on the server
		if (isLocalPlayer) {
			// Idea is that syncvar's will make sure this class has the board size and number of players from the server
			// Though depending how things work, it might be the server instantiating the netStateManager object from its prefab, not the clients version of the prefab, so these values would already be correct
			// so wouldn't actually need to be syncvars.  But either way, it should mean the client can now save these values into the persistentData object
			// so that the game can then be properly set up
			PersistentData.instance.BoardXSize = boardX;
			PersistentData.instance.BoardYSize = boardY;
			PersistentData.instance.NumberOfPlayers = NumOfPlayers;
			netManager.InitArrays ();	// Needs to be done early on so other init's and such don't try and refer to an uninitialised array/list.
			FindObjectOfType<BoardManager> ().Init ();
			FindObjectOfType<SetupOrthoCamera> ().SetupView ();
			GSM.InitGSM ();

			CmdAddPlayer ();

			// Save whether it's a client or the "host" so we know the right way of disconnecting at the end of the game.
			if (isServer) {
				netManager.isClient = false;
			} else {
				netManager.isClient = true;
			}
		}
		currentState = gameState.WaitingForPlayers;
		netManager.registerPlayer (this);

	}


	/*
	public override void OnStartClient ()
	{
		Debug.Log ("OnStartClient()");

	}
*/

	[Command]
	private void CmdAddPlayer ()
	{
		// TODO check and handle if -1 is returned aka no room for this new player.
		PlayerID = netManager.GetNextPlayerID ();
		this.gameObject.name = "Player" + PlayerID;
		if (PlayerID == (PersistentData.instance.NumberOfPlayers - 1)) {
			RpcAllPlayersInGame ();
		}
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

		// Let Server know player has commited to their placement
		CmdCommitPiece ( netManager.PlayerPlacements [PlayerID].PieceIndex,netManager.PlayerPlacements [PlayerID].PieceLocation, netManager.PlayerPlacements[PlayerID].CurrentRotation );
	}


	[Command]
	private void CmdCommitPiece (int pIndex, Vector3 pLoc, BasePiece.RotationDirection pRot )
	{
		RpcPiecePlacement (pIndex, pLoc, pRot);

		if (netManager.TrackCommit (PlayerID)) {
			RpcAllPiecesPlaced ();
		}
	}


	[ClientRpc]
	public void RpcAllPlayersInGame ()
	{
		netManager.SetAllGameStates( gameState.Init);
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
					// Easier to just mark both pieces now even though it will double up as otherwise we have to check for edge cases as 
					// if we just mark i's, then the piece at the end of the list will never get marked and if
					// we just mark the i's, the first piece in the list will never get marked
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
		currentState = gameState.EndRound;
		_timer = 0;
	}


	// Update is called once per frame
	void Update ()
	{
		if (isLocalPlayer) {
			// Checking if PlayerID is not -1 to ensure the syncVar has had time to reach the client before we do initialisation
			// Only really matters when first starting the game and for the last person to join, as it's only then that this
			// tends to happen on the clients before the syncVar containing the playerID is properly set
			if (currentState == gameState.Init && PlayerID != -1) {
				InitRound ();
				netManager.SetAllGameStates (gameState.PlayerXPlace);
			}
			else
			// If all players have commited their pieces, start the RevealPlayerPieces coroutine?
			if (currentState == gameState.ScoreRound) {

				StartCoroutine (RevealPlayerPieces ());
				_timer = 0;
				netManager.SetAllGameStates(gameState.Scoring);

			} else if (currentState == gameState.EndRound) {
				EndRound ();
			}
		}
	}
}
