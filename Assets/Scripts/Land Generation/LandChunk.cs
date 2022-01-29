using System;
using System.Collections;
using System.Collections.Generic;
using Saving;
using UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Saving;

public class LandChunk : MonoBehaviour
{
    public TerrainMaster terrainMaster;

    [Header("A hostile unit will spawn on the oppositeSpawnPoint")] public GameObject oppositeSpawnPoint;
    [Header("Based on amount of collision points we scale the (real country size)")] public float unitMovementSpeed;
    public MapData.UnitClaimed unitClaimed;
    public int unitPower;
    public TextMesh unitCountText;
    public bool inCombat;
    public LandEnemieAI landEnemieAI;

    private void Start()
    {
        CalculateCountrySize();
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
        if (this.unitClaimed == MapData.UnitClaimed.Unclaimed)
        {
            if (unitAgent.unitType == UnitType.Player)
            {
                this.unitClaimed = MapData.UnitClaimed.Player;
                GetComponent<SpriteRenderer>().color = Color.blue;
                unitPower = 1;
                unitCountText.text = $"Power: {unitPower}";
                terrainMaster.playerLand.Add(this);
            }
            else if (unitAgent.unitType == UnitType.Enemy)
            {
                this.unitClaimed = MapData.UnitClaimed.Hostile;
                GetComponent<SpriteRenderer>().color = Color.red;
                unitPower = 1;
                unitCountText.text = $"Power: {unitPower}";
                terrainMaster.hostileLand.Add(this);
                landEnemieAI.Strenght++;
                StartCoroutine(ResetCombatStatus());
            }
        }

        //Land belongs to enemy
        if (this.unitClaimed == MapData.UnitClaimed.Hostile)
        {
            if (unitAgent.unitType == UnitType.Player)
            {
                UiManager.Instance.OpenCombatWindow(unitAgent);
            }
            else
            {
                UiManager.Instance.OpenCombatWindow(unitAgent);
            }
        }
        
        GameStateGO.GameState.SaveWorldData(terrainMaster.landTiles);
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
