using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Saving;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class TerrainMaster : MonoBehaviour
{
    Dictionary<int, LandChunk[]> levels = new Dictionary<int, LandChunk[]>();
    [SerializeField] LandEnemieAI landEnemieAI;

    public GameObject unitCountPrefab;

   [HideInInspector] public List<LandChunk> spawnableLand = new List<LandChunk>();
   [HideInInspector] public List<LandChunk> hostileLand = new List<LandChunk>();
   [HideInInspector] public List<LandChunk> playerLand = new List<LandChunk>();
   [HideInInspector] public List<LandChunk> landTiles = new List<LandChunk>();

    public Difficulty difficulty;

    public int levelChoosen = 0;

    private Transform playersIsland;
    public bool gameRunning = false;

    private bool stopHint;

    public static TerrainMaster Instance;

    private void Awake()
    {
        Instance = this;
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

        if (GameStateGO.GameState.newGame)
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
#if UNITY_EDITOR
            chunk.name = $"Level:{levelCounter}_LandID:{keyCount}";
#endif
            chunk.terrainMaster = this;
            chunk.landEnemieAI = landEnemieAI;
            var text = Instantiate(unitCountPrefab, chunk.transform); //Add text to land
            chunk.unitCountText = text.GetComponent<TextMesh>();
            chunk.unitCountText.color = Color.yellow;
            
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
            var mapData = GameStateGO.GameState.worldData.Where(t => t.landID.Contains(chunk.gameObject.name));
            chunk.inCombat = mapData.First().inCombat;
            chunk.unitClaimed = mapData.First().unitClaimed;
            chunk.unitPower = mapData.First().unitPower;

            if (chunk.unitClaimed == MapData.UnitClaimed.Unclaimed)
            {
                chunk.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, .5f);
            } 
            else if (chunk.unitClaimed == MapData.UnitClaimed.Player)
            {
                chunk.GetComponent<SpriteRenderer>().color = Color.blue;
                chunk.GetComponentInChildren<TextMesh>().text = $"Power: {chunk.unitPower}";
            }
            else
            {
                chunk.GetComponent<SpriteRenderer>().color = Color.red;
                chunk.GetComponentInChildren<TextMesh>().text = $"Power: {chunk.unitPower}";
            }
        }
    }

    public void FindLandTiles()
    {
        var randomPlayerSpawnPoint = Random.Range(0, spawnableLand.Count);
        spawnableLand[randomPlayerSpawnPoint].GetComponent<SpriteRenderer>().color = Color.blue;
        spawnableLand[randomPlayerSpawnPoint].unitClaimed = MapData.UnitClaimed.Player;
        playersIsland = spawnableLand[randomPlayerSpawnPoint].transform;
        playerLand.Add(spawnableLand[randomPlayerSpawnPoint]);
        spawnableLand[randomPlayerSpawnPoint].unitPower = 1;
        spawnableLand[randomPlayerSpawnPoint].GetComponentInChildren<TextMesh>().text =
            $"Power: {spawnableLand[randomPlayerSpawnPoint].unitPower}";

        //Makes terrain we don't use gray
        foreach (LandChunk chunk in levels[levelChoosen])
        {
            if (chunk.unitClaimed == MapData.UnitClaimed.Unclaimed)
            {
                chunk.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, .5f);
            }
        }
    }

    public IEnumerator HelpPlayerTimer()
    {
        UiManager.Instance.gameWindow.SetActive(true);
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
        int landClaimCounter = 0, maxLandClaim = Random.Range(1, (int)difficulty * 2);

        foreach (var land in spawnableLand)
        {
            if (landClaimCounter >= maxLandClaim) break; 
            
            if (land.unitClaimed == MapData.UnitClaimed.Unclaimed)
            {
                switch (difficulty)
                {
                    case Difficulty.easy:
                        land.unitPower = Random.Range(1,3);
                        break;
                    case Difficulty.medium:
                        land.unitPower = Random.Range(3,5);
                        break;
                    case Difficulty.hard:
                        land.unitPower = Random.Range(5,10);
                        break;
                    case Difficulty.insane:
                        land.unitPower = Random.Range(10,20);
                        break;
                }
                
                land.GetComponent<SpriteRenderer>().color = Color.red;
                land.unitClaimed = MapData.UnitClaimed.Hostile;
                land.GetComponentInChildren<TextMesh>().text = $"Power: {land.unitPower}";
                hostileLand.Add(land);
                landEnemieAI.UpdateStats(land.unitPower);
                landClaimCounter++;
            }
        }
        
        gameRunning = true;

        GameStateGO.GameState.newGame = true;
        GameStateGO.GameState.SaveWorldData(landTiles, "", "");
    }

    private void UpdateWarStrenght()
    {
        UiManager.Instance.enemySlider.value = hostileLand.Count / 10f;
        UiManager.Instance.playerSlider.value = playerLand.Count / 10f;
    }
}

public enum Difficulty { easy, medium, hard, insane };