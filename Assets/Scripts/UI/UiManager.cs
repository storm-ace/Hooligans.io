using System.Collections;
using System.Collections.Generic;
using Saving;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class UiManager : MonoBehaviour
    {
        public static UiManager Instance;

        public GameObject settingsWindow, difficultyWindow, statusBar, menuButtons, flagWindow, combatWindow;
        public GameObject helpPlayerPanel, gameWindow;
        public AudioSource audioSource;
        public AudioClip clickBack;
        public GameObject menu;
        public GameObject drawings;
        public CombatWindow CombatWindow;
        
        private Camera cam;

        #region Settings

        [SerializeField] RectTransform musicToggle, audioToggle;
        [SerializeField] Texture greenBtnTexture, redBtnTexture;
        [SerializeField] Outline musicOutline, audioOutline;
        private bool musicValue, audioValue;
        private Color onColor, offColor;

        #endregion

        #region WarStrenght

        public Slider enemySlider;
        public Slider playerSlider;

        #endregion

        private bool gameRunning = false;

        private void Awake()
        {
            Instance = this;

            Application.targetFrameRate = 30;
            ColorUtility.TryParseHtmlString("#39971F", out onColor);
            ColorUtility.TryParseHtmlString("#AE2021", out offColor);
            cam = Camera.main;
        }

        private void FixedUpdate()
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            if (gameRunning && Input.GetMouseButtonDown(0))
            {
                helpPlayerPanel.SetActive(false);
            }

        }

        public void CloseSettingsPanel()
        {
            audioSource.PlayOneShot(clickBack);
            settingsWindow.SetActive(false);
            statusBar.SetActive(true);
            menuButtons.SetActive(true);
        }

        public void OpenSettingsPanel()
        {
            settingsWindow.SetActive(true);
            statusBar.SetActive(false);
            menuButtons.SetActive(false);
        }

        public void CloseDifficultyWindow()
        {
            audioSource.PlayOneShot(clickBack);
            difficultyWindow.SetActive(false);
            statusBar.SetActive(true);
            menuButtons.SetActive(true);
        }

        public void PlayGame()
        {
            if (GameStateGO.GameState.newGame)
            {
                CloseMenuUI(false);
                StartGame();
            }
            else
            {
                CloseMenuUI();
            }
        }

        private void CloseMenuUI(bool openDifficultyWindow = true)
        {
            difficultyWindow.SetActive(openDifficultyWindow);
            statusBar.SetActive(false);
            menuButtons.SetActive(false);
        }

        public void OpenFlagWindow()
        {
            flagWindow.SetActive(true);
            statusBar.SetActive(false);
            menuButtons.SetActive(false);
            cam.enabled = false;
            drawings.SetActive(true);
        }

        public void CloseFlagWindow()
        {
            flagWindow.SetActive(false);
            statusBar.SetActive(true);
            menuButtons.SetActive(true);
            cam.enabled = true;
            drawings.SetActive(false);
        }

        public void ToggleMusic()
        {
            musicValue = !musicValue;

            if (musicValue)
            {
                musicToggle.pivot = new Vector2(1, .5f);
                musicToggle.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 1000);
                musicOutline.effectColor = onColor;
                musicToggle.GetComponent<RawImage>().texture = greenBtnTexture;
                musicToggle.GetComponentInChildren<Text>().text = "On";
            }
            else
            {
                musicToggle.pivot = new Vector2(0, .5f);
                musicToggle.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 1000);
                musicOutline.effectColor = offColor;
                musicToggle.GetComponent<RawImage>().texture = redBtnTexture;
                musicToggle.GetComponentInChildren<Text>().text = "Off";
            }
        }

        public void ToggleSound()
        {
            audioValue = !audioValue;

            if (audioValue)
            {
                audioToggle.pivot = new Vector2(1, .5f);
                audioToggle.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 1000);
                audioOutline.effectColor = onColor;
                audioToggle.GetComponent<RawImage>().texture = greenBtnTexture;
                audioToggle.GetComponentInChildren<Text>().text = "On";
            }
            else
            {
                audioToggle.pivot = new Vector2(0, .5f);
                audioToggle.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 1000);
                audioOutline.effectColor = offColor;
                audioToggle.GetComponent<RawImage>().texture = redBtnTexture;
                audioToggle.GetComponentInChildren<Text>().text = "Off";
            }
        }

        public void StartGame()
        {
            difficultyWindow.SetActive(false);
            var terrainMaster = GetComponent<TerrainMaster>();
            terrainMaster.PopulateLevelsList();
            StartCoroutine(terrainMaster.HelpPlayerTimer());
            menu.SetActive(false);
            gameRunning = true;
        }

        public void GamemodeEasy()
        {
            GetComponent<TerrainMaster>().SpawnHostilesOnTheMap(Difficulty.easy);
        }

        public void GamemodeMedium()
        {
            GetComponent<TerrainMaster>().SpawnHostilesOnTheMap(Difficulty.medium);
        }

        public void GamemodeHard()
        {
            GetComponent<TerrainMaster>().SpawnHostilesOnTheMap(Difficulty.hard);
        }

        public void OpenCombatWindow(UnitAgent attackingUnit)
        {
            combatWindow.SetActive(true);
            Time.timeScale = 0;

            if (attackingUnit.unitType == UnitType.Player)
            {
                //Flee button active
                CombatWindow.RenderFleeBtn(true, attackingUnit);
            }
            else
            {
                //Flee button not active
                CombatWindow.RenderFleeBtn(false, attackingUnit);
            }
        }
    }

}