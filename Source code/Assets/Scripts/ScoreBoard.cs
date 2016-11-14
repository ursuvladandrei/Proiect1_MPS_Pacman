using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreBoard : MonoBehaviour
{

    public Text scoreText;
    int[] scores = new int[8];

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < 8; i++)
        {
            scores[i] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        getScore(players);
        UpdateScore(players);

    }

    void UpdateScore(GameObject[] players)
    {
        scoreText.text = "";
        for (int i = 0; i < players.Length; i++)
        {
            scoreText.text += "Player" + (i + 1) + ": " + scores[i] + "p\n";
        }
    }

    void getScore(GameObject[] players)
    {

        for (int i = 0; i < players.Length; i++)
        {
            scores[i] = players[i].GetComponent<GameMechanics>().score;
        }

    }

}
