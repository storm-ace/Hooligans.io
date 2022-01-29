using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    public GameObject scoreboardStatsPrefab;
    
    private List<ScoreStats> ScoreStatsList = new List<ScoreStats>();

    private void Start()
    {
        var playerStats = Instantiate(scoreboardStatsPrefab, transform.GetChild(0));
        var enemyStats  = Instantiate(scoreboardStatsPrefab, transform.GetChild(0));

        var playerData = new ScoreStats("Player");
        playerData.usernameText  = playerStats.transform.GetChild(0).GetComponent<Text>();
        playerData.userScoreText = playerStats.transform.GetChild(1).GetComponent<Text>();
        ScoreStatsList.Add(playerData);
        
        var enemyData = new ScoreStats("Enemy");
        enemyData.usernameText  =  enemyStats.transform.GetChild(0).GetComponent<Text>();
        enemyData.userScoreText = enemyStats.transform.GetChild(1).GetComponent<Text>();
        ScoreStatsList.Add(enemyData);
    }

    private void Update()
    {
        for (int i = 0; i < ScoreStatsList.Count; i++)
        {
            ScoreStatsList[i].UpdateStats();
        }
    }
}

public class ScoreStats
{
    public string name;
    public int value;
    public Text usernameText, userScoreText;
    public Player player;
    public AI AI;
    
    public ScoreStats(string name, int value = 0)
    {
        this.name = name;
        this.value = value;
    }

    public void UpdateStats()
    {
        usernameText.text = name;
        userScoreText.text = value.ToString();
    }
}