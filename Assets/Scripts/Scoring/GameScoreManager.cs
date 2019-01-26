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
    void Start()
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

    // Update is called once per frame
    void Update()
    {

    }
}
