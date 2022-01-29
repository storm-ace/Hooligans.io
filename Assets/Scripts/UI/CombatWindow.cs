using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Saving;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class CombatWindow : MonoBehaviour
    {
        public Button attackBtn, fleeBtn;

        private UnitAgent unitAgent;

        public void RenderFleeBtn(bool canFlee, UnitAgent attackingUnit)
        {
            fleeBtn.interactable = canFlee;
        }

        [Description("Saves game instance and loads the combat world")]
        public void LoadLevel()
        {
            Time.timeScale = 1;
            GameStateGO.GameState.SaveWorldData(TerrainMaster.Instance.landTiles, "", "");
            SceneManager.LoadSceneAsync("World");
        }

        public void Flee()
        {
            unitAgent.Retreat(); 
        }
    }
}
