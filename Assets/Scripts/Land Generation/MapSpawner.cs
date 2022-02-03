using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    public GameObject coin;
    public GameObject gameAI;

    public float coinRespawnTime = 5;

    public GameObject[] treesPrefabs;
    public GameObject[] obstaclesPrefabs;

    public List<GameObject> activeCoins = new List<GameObject>();
    public List<GameObject> idleCoins = new List<GameObject>();
    private List<GameObject> obstacles = new List<GameObject>();

    public bool treesSpawn, obstaclesSpawn;

    [SerializeField] Renderer rend;

    [SerializeField] int radius;
    int spawnRatio;

    private void Awake()
    {
        Application.targetFrameRate = 30;
    }

    private void Start()
    {
        radius = (int)rend.bounds.extents.magnitude - 20;
        spawnRatio = (int)rend.bounds.extents.magnitude;

        if (treesSpawn) GenerateTrees();
        if (obstaclesSpawn) GenerateObstacles();

        Transform coins = GameObject.FindGameObjectWithTag("Coins").transform;
        
        for (int i = 0; i < spawnRatio * 5; i++)
        {
            int x = Random.Range((int)-radius, (int)radius);
            int y = Random.Range((int)-radius, (int)radius);

            Vector3 target = transform.position + new Vector3(x, y, 0);
            target.z = 0;

            var coinSpawn = Instantiate(coin, target, Quaternion.identity, coins);
            coinSpawn.GetComponent<Coins>().mapSpawner = this;
            this.activeCoins.Add(coinSpawn);
        }

        SpawnEnemyAI();

        InvokeRepeating("CoinPool", coinRespawnTime, coinRespawnTime);
    }

    private void SpawnEnemyAI()
    {
        int x = Random.Range(-radius, radius);
        int y = Random.Range(-radius, radius);

        Vector3 target = transform.position + new Vector3(x, y, 0);
        target.z = 0;

        Instantiate(gameAI, target, Quaternion.identity);
    }

    private void GenerateObstacles()
    {
        Transform obstacle = GameObject.FindGameObjectWithTag("Obstacle").transform;

        for (int i = 0; i < spawnRatio; i++)
        {
            int x = Random.Range(-radius, radius);
            int y = Random.Range(-radius, radius);

            Vector3 target = transform.position + new Vector3(x, y, 0);
            target.z = 0;

            var obstacleSpawn = Instantiate(obstaclesPrefabs[0], target, Quaternion.identity, obstacle);
            obstacles.Add(obstacleSpawn);
        }
    }

    private void CoinPool()
    {
        //(this will reduce memory cache and lag)
        if (idleCoins.Count > 0)
        {
            int x = Random.Range(-radius, radius);
            int y = Random.Range(-radius, radius);

            Vector3 target = transform.position + new Vector3(x, y, 0);
            target.z = 0;

            idleCoins[0].transform.position = target;
            idleCoins[0].SetActive(true);
            activeCoins.Add(idleCoins[0]);
            idleCoins.Remove(idleCoins[0]);
        }
    }

    private void GenerateTrees()
    {
        for (int i = 0; i < spawnRatio; i++)
        {
            int x = Random.Range(-radius, radius);
            int y = Random.Range(-radius, radius);

            Vector3 target = transform.position + new Vector3(x, y, 0);
            target.z = 0;

            var tree = Random.Range(0, treesPrefabs.Length);
            Instantiate(treesPrefabs[tree], target, Quaternion.identity, GameObject.FindGameObjectWithTag("Nature").transform);
        }
    }
}
