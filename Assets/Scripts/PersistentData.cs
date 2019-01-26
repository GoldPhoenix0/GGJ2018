using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public int NumberOfPlayers = 2;
    public int MaxNumberOfPlayers = 4;

    public int BoardXSize = 5;
    public int BoardYSize = 10;

    public static PersistentData _instance;


    public static PersistentData instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject tmpObj = new GameObject();
                tmpObj.name = "PersistentObj";
                _instance = tmpObj.gameObject.AddComponent<PersistentData>();
            }
            return _instance;
        }
    }



    // Use this for initialization
    void Start () {
        if (_instance != null && this != PersistentData.instance  )
            Destroy(this);
        else
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
