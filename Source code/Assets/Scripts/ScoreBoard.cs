using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreBoard : MonoBehaviour {

    public Text scoreText;
    int score1, score2, score3, score4, score5, score6, score7, score8;
   // GameObject[] players = GameObject.FindGameObjectsWithTag("Player");


    // Use this for initialization
    void Start () {
        score1 = 0;
        score2 = 0;
        score3 = 0;
        score4 = 0;
        score5 = 0;
        score6 = 0;
        score7 = 0;
        score8 = 0;
        UpdateScore();
    }
	
	// Update is called once per frame
	void Update () {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        getScore(players);
        UpdateScore();


    }


    void UpdateScore ()
    {
        scoreText.text = "Player1: " + score1 + "p\n" +
            "Player2: " + score2 + "p\n" +
            "Player3: " + score3 + "p\n" +
            "Player4: " + score4 + "p\n" +
            "Player5: " + score5 + "p\n" +
            "Player6: " + score6 + "p\n" +
            "Player7: " + score7 + "p\n" +
            "Player8: " + score8 + "p";

    }

    void getScore(GameObject[] players)
    {
        if (players.Length < 1)
            return;
        score1 = players[0].GetComponent<GameMechanics>().score;
        if (players.Length < 2)
            return;
        score2 = players[1].GetComponent<GameMechanics>().score;
        if (players.Length < 3)
            return;
        score3 = players[2].GetComponent<GameMechanics>().score;
        if (players.Length < 4)
            return;
        score4 = players[3].GetComponent<GameMechanics>().score;
        if (players.Length < 5)
            return;
        score5 = players[4].GetComponent<GameMechanics>().score;
        if (players.Length < 6)
            return;
        score6 = players[5].GetComponent<GameMechanics>().score;
        if (players.Length < 7)
            return;
        score7 = players[6].GetComponent<GameMechanics>().score;
        if (players.Length < 8)
            return;
        score8 = players[7].GetComponent<GameMechanics>().score;
   
    }


    
}
