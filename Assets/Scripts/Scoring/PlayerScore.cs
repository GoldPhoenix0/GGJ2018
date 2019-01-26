using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerScore : MonoBehaviour {

    [SerializeField]
    private Text playerScoreLabel;
    [SerializeField]
    private Text playerScoreField;

    private int _PlayerNumber;

    private int _PlayerScoreValue;

    /// <summary>
    /// Get/Set players current score, will update UI
    /// </summary>
    public int PlayerScoreValue
    {
        get
        {
            return _PlayerScoreValue;
        }
        set
        {
            _PlayerScoreValue = value;
            playerScoreField.text = _PlayerScoreValue.ToString();
        }
    }

    public int PlayerNumber
    {
        get
        {
            return _PlayerNumber;
        }
        set
        {
            _PlayerNumber = value;
            playerScoreLabel.text = "Player " + (_PlayerNumber + 1).ToString();  // Since player numbers start from 0 but humans don't
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
    public void SetScorePosition()
    {
        RectTransform myRT;

        myRT = (RectTransform)this.gameObject.transform;
        Vector2 vals;
        vals = myRT.anchorMin;
        vals.x = PlayerNumber * (1.0f / PersistentData.instance.NumberOfPlayers);
        vals.y = 0;
        myRT.anchorMin = vals;

        vals = myRT.anchorMax;
        vals.x = (PlayerNumber + 1) * (1.0f / PersistentData.instance.NumberOfPlayers);
        vals.y = 1;
        myRT.anchorMax = vals;

    }

	// Update is called once per frame
	void Update () {
		
	}
}
