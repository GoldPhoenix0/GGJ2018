using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScoreManager : MonoBehaviour
{

    [SerializeField]
    private GameObject scorePanel;
    [SerializeField]
    private GameObject scoreDisplayPrefab;

    private List<PlayerScore> Scores;

    // Use this for initialization
    void Awake()
    {
        Scores = new List<PlayerScore>();

        for (int i = 0; i < PersistentData.instance.NumberOfPlayers; i++)
        {
            GameObject tmpGO;
            PlayerScore tmpPS;
            tmpGO = Instantiate(scoreDisplayPrefab, scorePanel.transform);

            tmpPS = tmpGO.GetComponent<PlayerScore>();
            Scores.Add(tmpPS);

            Scores[i].PlayerNumber = i;
            Scores[i].SetScorePosition();
            Scores[i].PlayerScoreValue = 0;
        }
    }

    public void SelectPlayer(int i)
    {
        Scores[i].SetHighlight(true);
    }

    public void DeSelectPlayer(int i)
    {
        Scores[i].SetHighlight(false);
    }

    /// <summary>
    /// Change the players score
    /// </summary>
    /// <param name="PlayerNumber">0 for first player</param>
    /// <param name="scoreChange">The value to add to the players current score</param>
    public void PlayerScoreDelta(int PlayerNumber, int scoreChange)
    {
        Scores[PlayerNumber].PlayerScoreValue = Scores[PlayerNumber].PlayerScoreValue + scoreChange;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
