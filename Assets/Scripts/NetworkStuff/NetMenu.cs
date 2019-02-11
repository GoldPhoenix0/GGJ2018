using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetMenu : MonoBehaviour
{
	[SerializeField]
	private Text IPAddressField;

	[SerializeField]
	private NetworkManager NM;
	
    // Start is called before the first frame update
    void Start()
    {
		NM = FindObjectOfType<NetworkManager> ();	// As apparently when scenes change, this can be lost or something

		PersistentData.instance.MenuSceneName = SceneManager.GetActiveScene ().name;
		NM.offlineScene = SceneManager.GetActiveScene().name;
    }



	public void HostGame ()
	{
		NetStateManager nsm;

		NM.networkPort = 7778;	// TODO Use the thingy to properly find a game over the network/allow the user to set a port so there won't be clashes.
		PersistentData.instance.LastIPAddress = NM.networkAddress;

		nsm = NM.playerPrefab.GetComponent<NetStateManager> ();
		nsm.boardX = PersistentData.instance.BoardXSize;
		nsm.boardY = PersistentData.instance.BoardYSize;
		nsm.NumOfPlayers = PersistentData.instance.NumberOfPlayers;

		NM.StartHost ();
	}

	public void JoinGame ()
	{
		NM.networkAddress = IPAddressField.text;
		NM.networkPort = 7778;
		PersistentData.instance.LastIPAddress = NM.networkAddress;
		NM.StartClient ();
	}

}
