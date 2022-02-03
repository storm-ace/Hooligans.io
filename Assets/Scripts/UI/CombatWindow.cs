using System;
using System.ComponentModel;
using Camera_controls;
using Saves;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class CombatWindow : MonoBehaviour
    {
        public Button attackBtn, fleeBtn;
        public Text playerPowerText, enemyPowerText, winRewardText;

        private UnitAgent _unitAgent;
        private LandChunk _landUnderAttack;

        private PanZoom _panZoom;

        private void Awake()
        {
            _panZoom = FindObjectOfType<PanZoom>();
        }

        public void RenderFleeBtn(bool canFlee, UnitAgent attackingUnit, LandChunk landUnderAttack, int enemyPower)
        {
            fleeBtn.interactable = canFlee;
            _unitAgent = attackingUnit;
            _landUnderAttack = landUnderAttack;
            enemyPowerText.text = $"Strength: {enemyPower}X";
            winRewardText.text = $"Win Reward: {10 * landUnderAttack.unitPower + (int)TerrainMaster.Instance.difficulty}";
            playerPowerText.text = $"Strength: {_panZoom.selectedLand.unitPower}X";
        }

        [Description("Saves game instance and loads the combat world")]
        public void LoadLevel()
        {
            Time.timeScale = 1;
            SaveSystem.instance.UpdateCombatData(_landUnderAttack.name);
            SaveSystem.instance.SaveData();
            SceneManager.LoadSceneAsync("World");
        }

        public void Flee()
        {
            Time.timeScale = 1;
            gameObject.SetActive(false);
            _unitAgent.Retreat(); 
        }
    }
}
