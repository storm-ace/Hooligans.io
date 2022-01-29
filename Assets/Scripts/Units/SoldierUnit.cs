using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierUnit : MonoBehaviour
{
    [HideInInspector] public Player player;
    [HideInInspector] public AI ai;
    [HideInInspector] public UnitManager unitManager;

    [SerializeField] Transform detectionLayer;

    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private float health = 10;
    public float Health { get { return health; } set { health = value; } }

    [SerializeField] private float range = 5;
    public float Range { get { return range; } set { range = value; } }
    [SerializeField] private float dmg;
    public float Dmg { get { return dmg; } set { dmg = value; } }
    [SerializeField] private float fps;
    public float FPS { get { return fps; } set { fps = value; } }
    private float rpmDefault;

    private bool isAI;
    public bool IsAI { get { return isAI; } set { isAI = value; } }

    public List<GameObject> unitsDetected = new List<GameObject>();
    public GameObject commander;

    private bool shootCommander;

    private GameObject target;

    private void Start()
    {
        rpmDefault = fps;
        fps = 0;
        detectionLayer.GetComponent<CircleCollider2D>().radius = range;
    }

    private void Update()
    {
        if (health <= 0)
        {
            if (isAI)
            {
                unitManager.enemyUnits.Remove(gameObject);
            }
            else unitManager.playerUnits.Remove(gameObject);

            Destroy(gameObject);
        }

        FireOnUnit();
    }

    public void ScanForHostiles()
    {
        if (isAI) return;

        target = null;

        foreach (GameObject unit in unitsDetected)
        {
            if (Vector3.Distance(unit.transform.position, transform.position) < range)
            {
                target = unit.gameObject;
                break;
            }
        }
        
        LockOnCommander();
    }

    /// <summary>
    /// <param name="commander"></param>
    /// Fires on commander after all units are KIA
    /// </summary>
    private void LockOnCommander()
    {
        if (unitsDetected.Count == 0)
        {
            target = commander;
            shootCommander = true;
        }
        else
        {
            shootCommander = false;
        }
    }

    private void FireOnUnit()
    {
        //Shots between
        if (fps > 0)
        {
            fps -= Time.deltaTime;
        }
        else //FIRE!
        {
            if (target == null) return;
            var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().Instantiate(target, dmg);
            fps = rpmDefault;
        }
    }
}
