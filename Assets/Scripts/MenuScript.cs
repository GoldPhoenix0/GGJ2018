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

    [SerializeField]
    private Text GridXUI;
    [SerializeField]
    private Text GridYUI;

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

    public void ChangeGridXSize(int deltaNOP)
    {
        PersistentData.instance.BoardXSize += deltaNOP;

        if (PersistentData.instance.BoardXSize < PersistentData.instance.MinBoardSize)
            PersistentData.instance.BoardXSize = PersistentData.instance.MaxBoardSize;

        if (PersistentData.instance.BoardXSize > PersistentData.instance.MaxBoardSize)
            PersistentData.instance.BoardXSize = PersistentData.instance.MinBoardSize;

        GridXUI.text = PersistentData.instance.BoardXSize.ToString();
    }

    public void ChangeGridYSize(int deltaNOP)
    {
        PersistentData.instance.BoardYSize += deltaNOP;

        if (PersistentData.instance.BoardYSize < PersistentData.instance.MinBoardSize)
            PersistentData.instance.BoardYSize = PersistentData.instance.MaxBoardSize;

        if (PersistentData.instance.BoardYSize > PersistentData.instance.MaxBoardSize)
            PersistentData.instance.BoardYSize = PersistentData.instance.MinBoardSize;

        GridYUI.text = PersistentData.instance.BoardYSize.ToString();
    }

    private void Start ()
	{
        ChangeNumberOfPlayers(0);
        ChangeGridXSize(0);
        ChangeGridYSize(0);
		PersistentData.instance.MenuSceneName = SceneManager.GetActiveScene ().name;
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
