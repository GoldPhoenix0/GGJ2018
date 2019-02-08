using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class GameScoreManager : MonoBehaviour
{

    [SerializeField]
    private GameObject scorePanel;
    [SerializeField]
    private GameObject scoreDisplayPrefab;

    [SerializeField]
    private GameObject FinalResultsGroup;
    [SerializeField]
    private Image ResultsBackground;
    [SerializeField]
    private Text ResultsHeading;
    [SerializeField]
    private Text ResultsList;

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

    public void ShowResultsScreen()
    {
        FinalResultsGroup.SetActive(true);

        Dictionary<int, int> finalScores = new Dictionary<int, int>(Scores.Count);

        for (int i = 0; i < Scores.Count; i++)
        {
            PlayerScore checkScore = Scores[i];
            finalScores.Add(checkScore.PlayerNumber, checkScore.PlayerScoreValue);
        }

        var orderedList = finalScores.OrderByDescending(x => x.Value);

        int resultLine = 0;
        int highScore = int.MinValue;
        string resultsText = "";
        string winningPlayerNames = "";

        foreach(KeyValuePair<int, int> order in orderedList)
        {
            int playerNumber = order.Key + 1;

            if(resultLine == 0)
            {
                highScore = order.Value;

                ResultsBackground.color = PersistentData.instance.PlayerColors[order.Key];
            }
            else
            {
                resultsText += "\n";
            }

            // Add all players to the heading
            if(highScore == order.Value)
            {
                if (resultLine > 0)
                    winningPlayerNames += " & ";

                winningPlayerNames += HelperFunctions.ConvertNumberToText(playerNumber);
            }

            resultsText += "Player " + playerNumber.ToString() + " - " + order.Value.ToString();

            resultLine++;
        }

        ResultsHeading.text = "Player " + winningPlayerNames + " Wins!";
        ResultsList.text = resultsText;

    }
}
