using Flag;
using UnityEngine;

public class LandEnemieAI : MonoBehaviour
{
    public TerrainMaster terrainMaster;

    [SerializeField] private GameObject hostilePrefab;

    //AI will commit attacks even if the unit count is low if the urge is high enough (can be trigger if the ai is losing)
    [SerializeField] private float attackUrge;
    //Strenght will effect how often ai will try to attack or defend
    [SerializeField] private float strenght;
    //AI will try to fallback when great loses has being hit
    [SerializeField] private float fallbackUrge;
    //AI will try to push player again when there is to much near friendly chunks aggrestion
    public float attackPlayerUrge;
    [SerializeField] private float timeBetweenStages = 10;

    public float Strenght { get { return strenght; } set { strenght = value; } }

    public string state;

    // private void Awake()
    // {
    //     StartCoroutine(ScanForWeakLands());
    // }

    private void LateUpdate()
    {
        attackUrge += .1f * terrainMaster.playerLand.Count * Time.deltaTime;
        attackPlayerUrge += .001f * Time.deltaTime;

        //CommitAction();
    }

    // private IEnumerator ScanForWeakLands()
    // {
    //     while (true)
    //     {
    //         List<LandChunk> cloneTerrain = new List<LandChunk>(terrainMaster.hostileLand);
    //         foreach (LandChunk chunk in cloneTerrain)
    //         {
    //             if (chunk.unitPower < 10) fallbackUrge += .15f;
    //             yield return new WaitForSeconds(.2f);
    //         }
    //
    //         yield return null;
    //     }
    // }

    public void UpdateStats(float strenght)
    {
        this.strenght += strenght;
    }

    private void CommitAction()
    {
        if (timeBetweenStages > 0) timeBetweenStages -= Time.deltaTime;
        if (state != string.Empty && timeBetweenStages <= 0) return; //Waits to commit other action after current action has passed
        LandChunk landToAttack = null;

        // if (fallbackUrge > 1)
        // {
        //     float lowestStrenght = 25;
        //     float highestStrenght = 0;
        //     LandChunk weakestLand = null;
        //     LandChunk strongestLand = null;
        //
        //     foreach (LandChunk chunk in terrainMaster.hostileLand)
        //     {
        //         if (chunk.unitPower > highestStrenght)
        //         {
        //             highestStrenght = chunk.unitPower;
        //             strongestLand = chunk;
        //             print("Highest " + highestStrenght + " " + chunk.name);
        //         }
        //
        //         if (chunk.unitPower <= 25)
        //         {
        //             if (chunk.unitPower < lowestStrenght && chunk != strongestLand)
        //             {
        //                 lowestStrenght = chunk.unitPower;
        //                 weakestLand = chunk;
        //                 print("Lowest " + lowestStrenght + " " + chunk.name);
        //             }
        //         }
        //     }
        //
        //     if (weakestLand == null)
        //     {
        //         fallbackUrge = 0;
        //         return;
        //     }
        //
        //     state = $"{strongestLand.name} is supporting {weakestLand}";
        //     DefendLand(strongestLand, weakestLand);
        //     fallbackUrge = 0;
        // }
        // else if (attackPlayerUrge > 1)
        
        if (attackPlayerUrge > 1)
        {
            LandChunk landOrigin = terrainMaster.hostileLand[Random.Range(0, terrainMaster.hostileLand.Count)];
            foreach (LandChunk chunk in terrainMaster.playerLand)
            {
                if (landOrigin.unitPower > chunk.unitPower)
                {
                    landToAttack = chunk;
                    state = "Attack Player";
                    AttackPlayer(landOrigin, landToAttack);
                    attackPlayerUrge = 0;
                    attackUrge = 0;
                    break;
                }
            }
        }
        else if (attackUrge > 1)
        {
            LandChunk landOrigin = terrainMaster.hostileLand[Random.Range(0, terrainMaster.hostileLand.Count)];
            foreach (LandChunk chunk in terrainMaster.landTiles)
            {
                if (landOrigin.unitPower > chunk.unitPower && chunk.unitClaimed == GameData.MapData.UnitClaimed.Unclaimed && chunk.inCombat == false)
                {
                    landToAttack = chunk;
                    state = "Attack empty land";
                    AttackLand(landOrigin, landToAttack);
                    attackUrge = 0;
                    chunk.inCombat = true;
                    break;
                }
            }
        }
    }

    private void AttackLand(LandChunk landOrigin, LandChunk landToAttack)
    {
        SpawnUnit(landOrigin, landToAttack);
    }

    private void SpawnUnit(LandChunk landOrigin, LandChunk landToAttack)
    {
        Vector3 spawnPosition = new Vector3(landOrigin.transform.position.x + Random.Range(-.3f, .3f),
                    landOrigin.transform.position.y + Random.Range(-.3f, .3f), -0.5f);
        var unit = Instantiate(hostilePrefab, spawnPosition, Quaternion.identity);
        var unitAgent = unit.GetComponent<UnitAgent>();
        unitAgent.Initialize(landToAttack, landOrigin);
        //landOrigin.unitPower--;
        landOrigin.unitCountText.text = $"Power: {landOrigin.unitPower}";

        state = string.Empty;
        timeBetweenStages = 10;
    }

    private void AttackPlayer(LandChunk landOrigin, LandChunk landToAttack)
    {
        SpawnUnit(landOrigin, landToAttack);
    }

    private void DefendLand(LandChunk landOrigin, LandChunk landToDefend)
    {
        SpawnUnit(landOrigin, landToDefend);
    }
}
