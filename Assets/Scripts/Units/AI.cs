using System;
using System.Collections.Generic;
using Saves;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class AI : MonoBehaviour
{
    [SerializeField] Transform mesh;
    [SerializeField] GameObject unitSpawnLocation, SelectCircle;

    public int unitStrength;

    public TextMesh healthText;

    private MapSpawner _mapSpawner;
    private static UnitManager unitManager;

    private readonly float speed = 5;
    [SerializeField] private GameObject targetCoin = null;
    public GameObject TargetCoin  { get { return targetCoin; } set { targetCoin = value; } }

    [SerializeField] private int coins;

    public int Coins
    {
        get { return coins; } 
        set { coins = value; }
    }

    [SerializeField] private float health = 100;
    public float Health {
        get { return health; }
        set { health = value; }
    }

    private bool dead;
    public bool Dead
    {
        get { return dead; }
    }
    
    public string action;
    
    private float spawnUnitTimer = 2f;

    private void Awake()
    {
        _mapSpawner = FindObjectOfType<MapSpawner>();
        unitManager = FindObjectOfType<UnitManager>();
    }

    private void Start()
    {
        unitStrength = SaveSystem.instance.gameData.enemyUnitStrenght;
    }

    private void Update()
    {
        LookForCoins();
        MoveToCoin();
        MoveUnits();
        SpawnUnit();
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        healthText.text = string.Format($"{health}%", "0");
        health = Mathf.Clamp(health, 0, 100);
        
        if (health <= 0 && !dead)
        {
            FindObjectOfType<VictoryOrLoseManager>().WinStatus(false);
        }
    }

    private void MoveUnits()
    {
        List<Vector3> targetPositionList = UnitManager.GetPositionListAround(unitSpawnLocation.transform.position,
                    new float[] { 3, 5, 7, 9 },
                    new int[] { 5, 15, 18, 20 });

        int targetPositionListIndex = 0;

        for (int i = 0; i < unitManager.enemyUnits.Count; i++)
        {
            unitManager.enemyUnits[i].transform.position = targetPositionList[targetPositionListIndex];
            unitManager.enemyUnits[i].transform.rotation = transform.GetChild(0).rotation;
            targetPositionListIndex = (targetPositionListIndex + 1) % targetPositionList.Count;
        }
    }

    private void LookForCoins()
    {
        if (targetCoin != null) return;

        action = "Looking for coins";
        Transform closestCoin = _mapSpawner.activeCoins[0].transform;

        foreach (var coin in _mapSpawner.activeCoins)
        {
            if (Vector3.Distance(transform.position, coin.transform.position) < 
                    Vector3.Distance(transform.position, closestCoin.position))
            {
                closestCoin = coin.transform;
            }
        }

        targetCoin = closestCoin.gameObject;
    }

    public GameObject markTarget()
    {
        SelectCircle.SetActive(true);
        return gameObject;
    }

    private void MoveToCoin()
    {
        //Check if the coin is still valid
        if (targetCoin == null)
        {
            targetCoin = null;
            return;
        }
        if (!targetCoin.activeInHierarchy)
        {
            targetCoin = null;
            return;
        }

        action = "Move to coin";
        transform.position = Vector3.MoveTowards(transform.position, targetCoin.transform.position, Time.deltaTime * speed);

        var dir = targetCoin.transform.position - transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        mesh.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void SpawnUnit()
    {
        if (spawnUnitTimer > 0)
        {
            spawnUnitTimer -= Time.deltaTime;
            return;
        }
        else
        {
            spawnUnitTimer = 2f;
        }

        string[] weakUnits = { "Button Man", "Button ManRifle", "Button Soldier" };
        string[] normalUnits = { "Button SoldierRifle", "Button SoldierMachine", "Button HitMan" };
        string[] strongUnits = { "Button Tank", "Button TankSuper" };

        action = "Spawn a unit";
        if (unitManager.playerStrenght < 30)
        {
            unitManager.SpawnUnit(weakUnits[Random.Range(0, weakUnits.Length)], coins, this);
        }
        else if (unitManager.playerStrenght > 30 && unitManager.playerStrenght < 100)
        {
            unitManager.SpawnUnit(normalUnits[Random.Range(0, normalUnits.Length)], coins, this);
        }
        else if (unitManager.playerStrenght > 100)
        {
            unitManager.SpawnUnit(strongUnits[Random.Range(0, strongUnits.Length)], coins, this);
        }
    }
}
