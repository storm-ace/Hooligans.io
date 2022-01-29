using System.Collections;
using System.Collections.Generic;
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
        var soldier = collision.GetComponent<SoldierUnit>();

        if (soldier != null && soldier.gameObject.tag != gameObject.tag)
        {
            if (soldierUnit.unitsDetected.Contains(soldier.gameObject) == false) soldierUnit.unitsDetected.Add(collision.gameObject);
        }
        else if (soldierUnit.player && collision.GetComponent<AI>())
        {
            soldierUnit.commander = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var soldier = collision.GetComponent<SoldierUnit>();

        if (soldier != null && soldier.gameObject.tag != gameObject.tag)
        {
            if (soldierUnit.unitsDetected.Contains(soldier.gameObject) == true) soldierUnit.unitsDetected.Remove(collision.gameObject);
        }
        else if (collision.CompareTag("Player"))
        {
            soldierUnit.commander = null;
        }
    }
}
