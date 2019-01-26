using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerScore : MonoBehaviour {

    [SerializeField]
    private Text playerScoreLabel;
    [SerializeField]
    private Text playerScoreField;
    [SerializeField]
    private Image ScoreBackground;
    [SerializeField]
    private Sprite SelectedBackGround;
    [SerializeField]
    private Sprite DefaultSprite;




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
            if(_PlayerNumber >= 0 && _PlayerNumber < PersistentData.instance.PlayerColors.Length)
                ScoreBackground.color = PersistentData.instance.PlayerColors[_PlayerNumber];
        }
    }

    public void SetHighlight(bool isHighlighted)
    {
        if (isHighlighted)
            ScoreBackground.sprite = SelectedBackGround;
        else
            ScoreBackground.sprite = DefaultSprite;


    }


    // Use this for initialization
    void Awake () {
        DefaultSprite = ScoreBackground.sprite;
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

}
