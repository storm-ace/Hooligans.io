using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Flag;
using Saves;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainMaster : MonoBehaviour
{
    Dictionary<int, LandChunk[]> levels = new Dictionary<int, LandChunk[]>();
    [SerializeField] LandEnemieAI landEnemieAI;

    public GameObject unitCountPrefab;

   public List<LandChunk> spawnableLand = new List<LandChunk>();
   public List<LandChunk> hostileLand = new List<LandChunk>();
   public List<LandChunk> playerLand = new List<LandChunk>();
   public List<LandChunk> landTiles = new List<LandChunk>();

   public Difficulty difficulty;

    public int levelChoosen = 0;

    private Transform playersIsland;
    public bool gameRunning = false;

    private bool stopHint;

    public GameObject saveManagerPrefab;

    public static TerrainMaster Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }

        if (SaveSystem.instance == null)
        {
            Instantiate(saveManagerPrefab);
        }
    }

    private void Start()
    {
        if (SaveSystem.instance.gameData.continueGame && !SaveSystem.instance.gameData.returnToMenu)
        {
            UpdateCombatStatus(true);
        }
        else if (SaveSystem.instance.gameData.continueGame && SaveSystem.instance.gameData.returnToMenu)
        {
            UpdateCombatStatus(false);
        }
    }

    private void UpdateCombatStatus(bool playGame)
    {
        print(SaveSystem.instance.gameData.fightWon);
        if (SaveSystem.instance.gameData.fightWon)
        {
            var id = SaveSystem.instance.gameData.attackingLand;
            var searchID = SaveSystem.instance.gameData.worldData.FindIndex(c => c.landID.Contains(id));
            var newLandData = new GameData.MapData()
            {
                landID = id,
                unitClaimed = GameData.MapData.UnitClaimed.Player,
                unitPower = 1,
                inCombat = false,
                unitAvailable = true
            };

            SaveSystem.instance.gameData.worldData[searchID] = newLandData;
            SaveSystem.instance.gameData.fightWon = false;
        }

        if (playGame) UiManager.Instance.PlayGame();
        else
        {
            SaveSystem.instance.gameData.returnToMenu = false;
        }
    }

    private void FixedUpdate()
    {
        if (gameRunning)
        {
            UpdateWarStrenght();
            if (Input.GetMouseButtonDown(0))
            {
                StopCoroutine(HelpPlayerTimer());
                stopHint = true;
            }
        }
    }

    public void PopulateLevelsList()
    {
        var levelsFound = GameObject.FindGameObjectsWithTag("Level");
        var levelCounter = 0;

        foreach (var level in levelsFound)
        {
            var chunks = level.GetComponentsInChildren<LandChunk>();
            levels.Add(levelCounter, chunks);
            levelCounter++;
        }

        if (SaveSystem.instance.gameData.continueGame)
        {
            CreateLandTiles();
            PopulatedLandFromFile(levelsFound);
        }
        else
        {
            CreateLandTiles();
            FindLandTiles();
        }
    }

    private void CreateLandTiles(bool addSpawnInfoToList = true, int levelCounter = 0)
    {
        int keyCount = 0;

        foreach (var chunk in levels[levelCounter])
        {
            chunk.name = $"Level:{levelCounter}_LandID:{keyCount}";
            chunk.terrainMaster = this;
            chunk.landEnemieAI = landEnemieAI;
            //var text = Instantiate(unitCountPrefab, chunk.transform); //Add text to land
            //chunk.unitCountText = text.GetComponent<TextMesh>();
            //chunk.unitCountText.color = Color.yellow;
            
            if (chunk.oppositeSpawnPoint && addSpawnInfoToList)
            {
                spawnableLand.Add(chunk);
            }
            
            landTiles.Add(chunk);

            keyCount++;
        }
    }
    
    private void PopulatedLandFromFile(GameObject[] level)
    {
        print("Load saving game");
        foreach (var chunk in landTiles)
        {
            var mapData = SaveSystem.instance.gameData.worldData.Where(t => t.landID.Contains(chunk.gameObject.name));
            chunk.unitClaimed = mapData.First().unitClaimed;
            chunk.unitPower = mapData.First().unitPower;
            chunk.inCombat = mapData.First().inCombat;
            chunk.unitAvailable = mapData.First().unitAvailable;

            if (chunk.unitClaimed == GameData.MapData.UnitClaimed.Unclaimed)
            {
                chunk.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, .5f);
            } 
            else if (chunk.unitClaimed == GameData.MapData.UnitClaimed.Player)
            {
                chunk.GetComponent<SpriteRenderer>().color = Color.blue;
                playerLand.Add(chunk);
                //chunk.GetComponentInChildren<TextMesh>().text = $"Power: {chunk.unitPower}";
            }
            else
            {
                chunk.GetComponent<SpriteRenderer>().color = Color.red;
                //chunk.GetComponentInChildren<TextMesh>().text = $"Power: {chunk.unitPower}";
            }
        }
    }

    public void FindLandTiles()
    {
        var randomPlayerSpawnPoint = Random.Range(0, spawnableLand.Count);
        spawnableLand[randomPlayerSpawnPoint].GetComponent<SpriteRenderer>().color = Color.blue;
        spawnableLand[randomPlayerSpawnPoint].unitClaimed = GameData.MapData.UnitClaimed.Player;
        playersIsland = spawnableLand[randomPlayerSpawnPoint].transform;
        playerLand.Add(spawnableLand[randomPlayerSpawnPoint]);
        spawnableLand[randomPlayerSpawnPoint].unitPower = 1;
        spawnableLand[randomPlayerSpawnPoint].unitAvailable = true;
        /*spawnableLand[randomPlayerSpawnPoint].GetComponentInChildren<TextMesh>().text =
            $"Power: {spawnableLand[randomPlayerSpawnPoint].unitPower}";*/

        //Makes terrain we don't use gray
        foreach (LandChunk chunk in levels[levelChoosen])
        {
            if (chunk.unitClaimed == GameData.MapData.UnitClaimed.Unclaimed)
            {
                chunk.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, .5f);
            }
        }
    }

    public IEnumerator HelpPlayerTimer()
    {
        float delay = 6f;
        yield return new WaitForSeconds(delay);
        //Prevents the hint from printing if true
        if (!stopHint)
            UiManager.Instance.helpPlayerPanel.SetActive(true);
    }

    public void SpawnHostilesOnTheMap(Difficulty difficulty)
    {
        print($"difficulty: {difficulty}");
        this.difficulty = difficulty;
        int landClaimCounter = 0, maxLandClaim = Random.Range(3, 10 * (int)difficulty);

        while (landClaimCounter < maxLandClaim)
        {
            foreach (var land in landTiles)
            {
                if (landClaimCounter > maxLandClaim) break;
                
                if (land.unitClaimed == GameData.MapData.UnitClaimed.Unclaimed)
                {
                    switch (difficulty)
                    {
                        case Difficulty.easy:
                            land.unitPower = Random.Range(1, 3);
                            break;
                        case Difficulty.medium:
                            land.unitPower = Random.Range(3, 5);
                            break;
                        case Difficulty.hard:
                            land.unitPower = Random.Range(5, 10);
                            break;
                        case Difficulty.insane:
                            land.unitPower = Random.Range(10, 20);
                            break;
                    }

                    land.GetComponent<SpriteRenderer>().color = Color.red;
                    land.unitClaimed = GameData.MapData.UnitClaimed.Hostile;
                    //land.GetComponentInChildren<TextMesh>().text = $"Power: {land.unitPower}";
                    hostileLand.Add(land);
                    landEnemieAI.UpdateStats(land.unitPower);
                    landClaimCounter++;
                }
            }
        }

        gameRunning = true;

        SaveSystem.instance.gameData.continueGame = true;
        SaveSystem.instance.SaveData();
    }

    public void BuyUnits()
    {
        if (SaveSystem.instance.gameData.coins >= 100 && playerLand[0] && !playerLand[0].unitAvailable)
        {
            playerLand[0].unitAvailable = true;
            SaveSystem.instance.gameData.coins -= 100;
            SaveSystem.instance.SaveData();
        }
    }

    private void UpdateWarStrenght()
    {
        UiManager.Instance.enemySlider.value = hostileLand.Count / 10f;
        UiManager.Instance.playerSlider.value = playerLand.Count / 10f;
    }
}

public enum Difficulty { easy, medium, hard, insane };