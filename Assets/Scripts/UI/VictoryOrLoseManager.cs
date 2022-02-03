using System;
using System.Collections.Generic;
using Saves;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class VictoryOrLoseManager : MonoBehaviour
    {
        public Text winConditionText;

        public List<Text> combatTextInfo;

        private Player _player;

        private void Awake()
        {
            _player = FindObjectOfType<Player>();
        }

        public void WinStatus(bool lose)
        {
            Time.timeScale = 0;
            transform.GetChild(0).gameObject.SetActive(true);
            
            if (lose)
            {
                winConditionText.text = "Lost";
                winConditionText.color = Color.red;
            }

            SetCombatInfo();
        }

        private void SetCombatInfo()
        {
            combatTextInfo[0].text += $" {_player.Hits}";
            combatTextInfo[1].text += $" {_player.Kills}";
            combatTextInfo[2].text += $" {_player.TotalCoins}";
            combatTextInfo[3].text += $" {_player.Units}";
            combatTextInfo[4].text += $" {_player.Deaths}";
        }

        public void Continue()
        {
            // print(_player.Dead ? "You lost!" : "You won!");
            SaveSystem.instance.gameData.fightWon = !_player.Dead;
            SceneManager.LoadSceneAsync("Menu");
        }
    }
}