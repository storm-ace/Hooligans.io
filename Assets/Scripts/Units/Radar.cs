using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;

public class Radar : MonoBehaviour
{
    private SoldierUnit soldierUnit;

    private void Awake()
    {
        soldierUnit = GetComponentInParent<SoldierUnit>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //No hit detected
        if (!collision.gameObject) return;
        //Not hitting self
        if (collision.gameObject == soldierUnit.gameObject) return;
        //Not hitting teammates
        if (collision.gameObject.CompareTag(soldierUnit.tag)) return;

        var aiCommander = collision.GetComponent<AI>();
        var playerCommander = collision.GetComponent<Player>();

        if (aiCommander && soldierUnit.player)
        {
            soldierUnit.commander = aiCommander.gameObject;
        }
        else if (playerCommander && soldierUnit.IsAI)
        {
            soldierUnit.commander = playerCommander.gameObject;   
        }

        var detectedSoldier = collision.GetComponent<SoldierUnit>();
        if (!detectedSoldier) return;
        
        if (!soldierUnit.unitsDetected.Contains(detectedSoldier.gameObject))
        {
            soldierUnit.unitsDetected.Add(detectedSoldier.gameObject);
        }
        else if (soldierUnit.unitsDetected.Contains(detectedSoldier.gameObject))
        {
            soldierUnit.unitsDetected.Remove(detectedSoldier.gameObject);
        }
        else if (aiCommander && soldierUnit.player)
        {
            soldierUnit.commander = null;
        }
        else if (playerCommander && soldierUnit.IsAI)
        {
            soldierUnit.commander = null;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var soldier = collision.GetComponent<SoldierUnit>();

        if (soldier != null && !soldier.gameObject.CompareTag(gameObject.tag))
        {
            if (soldierUnit.unitsDetected.Contains(soldier.gameObject) == true) soldierUnit.unitsDetected.Remove(collision.gameObject);
        }
        else if (collision.CompareTag("Player"))
        {
            soldierUnit.commander = null;
        }
    }
}
