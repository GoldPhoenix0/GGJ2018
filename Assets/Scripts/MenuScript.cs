using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MenuScript : MonoBehaviour {

    [SerializeField]
    private string GameSceneName = "GameScene";

    [SerializeField]
    private Text NumberOfPlayersUI;


    public void ChangeNumberOfPlayers(int deltaNOP)
    {
        PersistentData.instance.NumberOfPlayers += deltaNOP;

        if (PersistentData.instance.NumberOfPlayers > PersistentData.instance.MaxNumberOfPlayers)
            PersistentData.instance.NumberOfPlayers = 2;

        if (PersistentData.instance.NumberOfPlayers == 1)
            PersistentData.instance.NumberOfPlayers = PersistentData.instance.MaxNumberOfPlayers;

        // update UI
        NumberOfPlayersUI.text = PersistentData.instance.NumberOfPlayers.ToString();
    }

    private void Start ()
	{
	}

	public void StartGame()
    {

        SceneManager.LoadScene(GameSceneName);

    }




    public void QuitGame()
    {
        Application.Quit();
    }



    


}
