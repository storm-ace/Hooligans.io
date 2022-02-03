using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class SmokeController : MonoBehaviour
{
    public int smokeTimer = 50;

    public GameObject smokePrefab;
    public Text cooldownTimerText;
    
    private float smokeCurrentTime;
    private Transform player;

    private void Start()
    {
        smokeCurrentTime = smokeTimer;
        player = FindObjectOfType<Player>().transform;
    }

    private void Update()
    {
        if (smokeCurrentTime > 1)
        {
            smokeCurrentTime -= 1f * Time.deltaTime;
            cooldownTimerText.text = Mathf.RoundToInt(smokeCurrentTime) + "S";
        }
        else if (smokeCurrentTime > 0)
        {
            smokeCurrentTime -= 1f * Time.deltaTime;
            cooldownTimerText.text = $"{smokeCurrentTime:0.##}MS";
        }
        else
        {
            cooldownTimerText.color = new Color(255, 255, 255, Mathf.Lerp(cooldownTimerText.color.a, 0, 1f * Time.deltaTime));
            smokeCurrentTime = 0;
        }
    }

    public void DeploySmoke()
    {
        if (smokeCurrentTime <= 0)
        {
            Instantiate(smokePrefab, player.position, quaternion.identity);
            smokeCurrentTime = smokeTimer;
            cooldownTimerText.color = new Color(255, 255, 255, 1);
        }
    }
}
