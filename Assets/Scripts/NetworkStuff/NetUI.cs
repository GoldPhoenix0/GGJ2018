using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetUI : MonoBehaviour
{
	public GameScoreManager GSM;
	public List<NetStateManager> AllPlayers;
	public int [] Connected;
	public bool [] CommittedPiece;
	public BasePiece [] PlayerPlacements;
	public bool[] PieceCollides;
	public bool isClient;


    // Start is called before the first frame update
    void Awake()
    {

    }

	public void InitArrays ()
	{
		AllPlayers = new List<NetStateManager> ();
		Connected = new int [PersistentData.instance.NumberOfPlayers];
		CommittedPiece = new bool [PersistentData.instance.NumberOfPlayers];
		PlayerPlacements = new BasePiece [PersistentData.instance.NumberOfPlayers];
		PieceCollides = new bool [PersistentData.instance.NumberOfPlayers];

		for (int i = 0; i < Connected.Length; i++) {
			Connected [i] = -1; // -1 for not connected
			CommittedPiece [i] = false;
		}
	}

	public int GetNextPlayerID ()
	{
		for (int i = 0; i < Connected.Length; i++) {
			if (Connected [i] == -1) {
				Connected [i] = i;
				return i;
			}
		}
		return -1;	// No spare spots for a new player.
	}

	public void registerPlayer (NetStateManager nsm)
	{
		AllPlayers.Add (nsm);
	}

	/// <summary>
	/// Tracks the commit.  Returns true if all players have commited, false if there's any player that hasn't.
	/// </summary>
	/// <returns><c>true</c>, if all players have committed<c>false</c> otherwise.</returns>
	/// <param name="playerID">Player identifier.</param>
	public bool TrackCommit (int playerID)
	{
		CommittedPiece [playerID] = true;

		for (int i = 0; i < CommittedPiece.Length; i++) {
			if (CommittedPiece [i] == false)
				return false;
		}
		return true;
	}

	// Calls commitPlacement on all players and lets them decide if they're the localPlayer or not.
	public void CommitPlayerPiece ()
	{
		foreach (NetStateManager nsm in AllPlayers) {
			if (nsm != null) {
				nsm.CommitPlacement ();
			}
		}
	}

	public void SetAllGameStates (NetStateManager.gameState gs)
	{
		foreach (NetStateManager nsm in AllPlayers) {
			if (nsm != null) {
				nsm.currentState = gs;
			}
		}
	}

	public void PlaceOtherPlayerPiece (int PlayerID, int pIndex, Vector3 pLoc, BasePiece.RotationDirection pRot)
	{
		BasePiece tmpObj;

		tmpObj = BoardManager.instance.GeneratePieceAt (pIndex, pLoc, pRot);

		PlayerPlacements [PlayerID] = tmpObj;

		tmpObj.gameObject.SetActive (false);	// Hide piece for now
	}

	public void ResetForNewRound ()
	{
		for (int i = 0; i < CommittedPiece.Length; i++)
		{
			CommittedPiece [i] = false;
			PieceCollides [i] = false;
		}
	}

	public void RestartGame ()
	{
		if (isClient)
			FindObjectOfType<NetworkManager> ().StopClient ();
		else
			FindObjectOfType<NetworkManager> ().StopHost ();

			
		//UnityEngine.SceneManagement.SceneManager.LoadScene (PersistentData.instance.MenuSceneName);
	}

	public void OnGameOverStart ()
	{
		// Show Game Ending Flair Here!
		BoardManager.instance.CurrentSelectedPiece = null;
		GSM.ShowResultsScreen ();
	}

}
