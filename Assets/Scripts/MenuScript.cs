using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MenuScript : MonoBehaviour {

    [SerializeField]
    private string GameSceneName = "GameScene";


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
