using System;
using Saves;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        public Text coins;
        
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
            if (Instance == null) Instance = this;

            Application.targetFrameRate = 30;
            ColorUtility.TryParseHtmlString("#39971F", out onColor);
            ColorUtility.TryParseHtmlString("#AE2021", out offColor);
            cam = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.F1))  Application.OpenURL(Application.persistentDataPath + "/SaveData.io");
        }

        private void OnApplicationQuit()
        {
            SaveSystem.instance.SaveData();
        }

        private void FixedUpdate()
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            if (gameRunning && Input.GetMouseButtonDown(0))
            {
                helpPlayerPanel.SetActive(false);
            }

            coins.text = $"{SaveSystem.instance.gameData.coins}";
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
            if (SaveSystem.instance.gameData.continueGame)
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
            Time.timeScale = 1;
            var terrainMaster = GetComponent<TerrainMaster>();
            
            difficultyWindow.SetActive(false);
            gameWindow.SetActive(true);
            menu.SetActive(false);
            
            terrainMaster.PopulateLevelsList();
            gameRunning = true;
            
            if (!SaveSystem.instance.gameData.continueGame) StartCoroutine(terrainMaster.HelpPlayerTimer());
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

        public void OpenCombatWindow(UnitAgent attackingUnit, LandChunk landUnderAttack, int enemyPower)
        {
            combatWindow.SetActive(true);
            Time.timeScale = 0;

            if (attackingUnit.unitType == UnitType.Player)
            {
                //Flee button active
                CombatWindow.RenderFleeBtn(true, attackingUnit, landUnderAttack, enemyPower);
            }
            else
            {
                //Flee button not active
                CombatWindow.RenderFleeBtn(false, attackingUnit, landUnderAttack, enemyPower);
            }
        }

        public void ResetGameData()
        {
            SaveSystem.instance.ResetSave();
            CloseSettingsPanel();
        }

        public void ReturnToMenu()
        {
            SaveSystem.instance.gameData.returnToMenu = true;
            SceneManager.LoadSceneAsync("Menu");
        }
    }

}