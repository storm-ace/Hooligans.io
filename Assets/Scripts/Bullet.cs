using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject target;
    private float dmg;

    public void Instantiate(GameObject target, float dmg)
    {
        this.target = target;
        this.dmg = dmg;
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
                var commander = target.GetComponent<AI>();

                if (soldier) soldier.Health -= dmg;
                if (commander) commander.Health -= dmg;
                
                Destroy(gameObject);
            }
        }
    }
}
