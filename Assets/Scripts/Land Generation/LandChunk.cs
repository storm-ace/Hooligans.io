using System;
using System.Collections;
using Flag;
using Saves;
using UI;
using UnityEngine;

public class LandChunk : MonoBehaviour
{
    public TerrainMaster terrainMaster;

    [Header("A hostile unit will spawn on the oppositeSpawnPoint")] public GameObject oppositeSpawnPoint;
    [Header("Based on amount of collision points we scale the (real country size)")] public float unitMovementSpeed;
    public GameData.MapData.UnitClaimed unitClaimed;
    public int unitPower;
    public TextMesh unitCountText;
    public bool inCombat;
    public LandEnemieAI landEnemieAI;
    public bool unitAvailable;
    public SpriteRenderer flagIcon;
    public GameObject flagIconPrefab;

    private void Start()
    {
        var flag = Instantiate(flagIconPrefab, gameObject.transform);
        flagIcon = flag.GetComponent<SpriteRenderer>();
        flagIcon.sortingOrder = 20;
        //flagIcon.sprite = SaveSystem.instance.currentFlag;
        
        CalculateCountrySize();
    }

    private void Update()
    {
        if (unitAvailable)
        {
            flagIcon.enabled = true;
            unitAvailable = true;
        }
        else
        {
            flagIcon.enabled = false;
            unitAvailable = false;
        }
    }

    public void CalculateCountrySize()
    {
       unitMovementSpeed = GetComponent<PolygonCollider2D>().GetTotalPointCount();

        if (unitMovementSpeed < 50)
        {
            unitMovementSpeed = .6f;
        }
        else if (unitMovementSpeed > 50 && unitMovementSpeed < 100)
        {
            unitMovementSpeed = .4f;
        }
        else unitMovementSpeed = .1f;
    }

    public void IncomingUnits(UnitAgent unitAgent)
    {
        //Land belongs to nobody
        if (unitClaimed == GameData.MapData.UnitClaimed.Unclaimed)
        {
            switch (unitAgent.unitType)
            {
                case UnitType.Player:
                    unitClaimed = GameData.MapData.UnitClaimed.Player;
                    GetComponent<SpriteRenderer>().color = Color.blue;
                    unitPower = 1;
                    //unitCountText.text = $"Power: {unitPower}";
                    terrainMaster.playerLand.Add(this);
                    unitAvailable = true;
                    break;
                case UnitType.Enemy:
                    unitClaimed = GameData.MapData.UnitClaimed.Hostile;
                    GetComponent<SpriteRenderer>().color = Color.red;
                    unitPower = 1;
                    //unitCountText.text = $"Power: {unitPower}";
                    terrainMaster.hostileLand.Add(this);
                    landEnemieAI.Strenght++;
                    StartCoroutine(ResetCombatStatus());
                    break;
            }
            
            Destroy(unitAgent.gameObject);

            SaveSystem.instance.UpdateLandData();
            SaveSystem.instance.SaveData();
        }

        //Land belongs to enemy
        if (unitClaimed == GameData.MapData.UnitClaimed.Hostile)
        {
            SaveSystem.instance.gameData.enemyUnitStrenght = unitPower;
            UiManager.Instance.OpenCombatWindow(unitAgent, this, unitPower);
        }
        
        //Land belongs to player
        if (unitClaimed == GameData.MapData.UnitClaimed.Player)
        {
            unitAvailable = true;
            Destroy(unitAgent.gameObject);
        }
        
        SaveSystem.instance.SaveData();
    }

    private IEnumerator ResetCombatStatus()
    {
        float timer = 10f;
        while (timer > 0)
        {
            timer--;
            yield return new WaitForSeconds(1f);
        }
        inCombat = false;
    }
}
