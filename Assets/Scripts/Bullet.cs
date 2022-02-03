using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject target;

    private UnitManager _unitManager;
    private float dmg;

    public void Instantiate(GameObject target, float dmg, UnitManager unitManager)
    {
        this.target = target;
        this.dmg = dmg;
        _unitManager = unitManager;
        Invoke("AliveTimer", 30f);
    }

    private void AliveTimer()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if (target != null)
        {
            transform.LookAt(target.transform.position);
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * 10f);
            if (Vector2.Distance(transform.position, target.transform.position) < 1)
            {
                var soldier = target.GetComponent<SoldierUnit>();
                var commanderAI = target.GetComponent<AI>();
                var commanderPlayer = target.GetComponent<Player>();

                if (soldier)
                {
                    soldier.Health -= dmg;
                    if (soldier.ai) _unitManager.player.Hits++;
                    //unitManager.player.
                    //print($"HIT dealing: HP: {soldier.Health} - {dmg}");
                }

                if (commanderAI)
                {
                    commanderAI.Health -= dmg;
                    _unitManager.player.Hits++;
                    //print($"HIT dealing: HP: {commander.Health} - {dmg}");
                }

                if (commanderPlayer)
                {
                    commanderPlayer.Health -= dmg;
                }
                
                
                Destroy(gameObject);
            }
        }
    }
}
