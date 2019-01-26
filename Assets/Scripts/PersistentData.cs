using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public int NumberOfPlayers = 3;
    public int MaxNumberOfPlayers = 7;

    public int BoardXSize = 5;
    public int BoardYSize = 10;

    public Color32[] PlayerColors = new Color32[10]
    {
        new Color32(230, 25, 75, 255), // Red
        new Color32(245,130, 49, 255), // Orange
        new Color32(255,225, 25, 255), // Yellow
        new Color32(188,246, 12, 255), // Lime
        new Color32( 60,180, 75, 255), // Green
        new Color32( 70,240,240, 255), // Cyan
        new Color32( 67, 99,216, 255), // Blue
        new Color32(145, 30,180, 255), // Purple
        new Color32(240, 50,230, 255), // Magenta
        new Color32(128,128,128, 255)  // Grey
    };

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
