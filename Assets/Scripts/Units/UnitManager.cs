using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitManager : MonoBehaviour
{
    public GameObject playerGO, unitSpawnLocation;
    public List<GameObject> playerUnits = new List<GameObject>();
    public List<GameObject> enemyUnits = new List<GameObject>();
    public List<GameObject> ghostUnits = new List<GameObject>();
    public GameObject[] unitPrefabs;

    public int playerStrenght;
    public int enemyStrenght;

    Camera cam;
    Player player;
    Transform unitsTransform;

    private KeyCode[] keyCodes =
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
    };

    private string[] unitIDs =
    {
        "Button Man",
        "Button ManRifle",
        "Button Soldier",
        "Button SoldierRifle",
        "Button SoldierMachine",
        "Button HitMan",
        "Button Tank",
        "Button TankSuper"
    };

    private void Awake()
    {
        cam = Camera.main;
        player = playerGO.GetComponent<Player>();
        unitsTransform = GameObject.FindGameObjectWithTag("Units").transform;
    }

    public void SpawnUnit(string unitRequest = "")
    {
        string buttonInfo = unitRequest != String.Empty ? unitRequest : EventSystem.current.currentSelectedGameObject.name;

        int unitCost;
        int unitIndex;
        int unitStrenght;

        switch (buttonInfo)
        {
            case "Button Man":
                unitCost = 3;
                unitIndex = 0;
                unitStrenght = 1;
                break;
            case "Button ManRifle":
                unitCost = 5;
                unitIndex = 1;
                unitStrenght = 3;
                break;
            case "Button Soldier":
                unitCost = 10;
                unitIndex = 2;
                unitStrenght = 6;
                break;
            case "Button SoldierRifle":
                unitCost = 16;
                unitIndex = 3;
                unitStrenght = 14;
                break;
            case "Button SoldierMachine":
                unitCost = 20;
                unitIndex = 4;
                unitStrenght = 10;
                break;
            case "Button HitMan":
                unitCost = 35;
                unitIndex = 5;
                unitStrenght = 15;
                break;
            case "Button Tank":
                unitCost = 50;
                unitIndex = 6;
                unitStrenght = 20;
                break;
            case "Button TankSuper":
                unitCost = 100;
                unitIndex = 7;
                unitStrenght = 26;
                break;
            default:
                unitCost = 0;
                unitIndex = -1;
                unitStrenght = 0;
                break;
        }

        if (player.Coins < unitCost) return;
        if (playerUnits.Count > 63) return;

        var unit = Instantiate(unitPrefabs[unitIndex], unitsTransform);
        unit.tag = "PlayerUnit";
        for (int i = 0; i < unit.transform.childCount; i++)
        {
            unit.transform.GetChild(i).tag = unit.tag;
        }
        unit.GetComponent<SoldierUnit>().player = player;
        unit.GetComponent<SoldierUnit>().unitManager = this;
        playerUnits.Add(unit);
        playerStrenght += unitStrenght;

        player.Coins -= unitCost;
        player.UpdateCoins();
    }

    public float SpawnUnit(string unitRequest, int coins, AI ai)
    {
        int unitCost;
        int unitIndex;
        int unitStrenght;

        switch (unitRequest)
        {
            case "Button Man":
                unitCost = 3;
                unitIndex = 0;
                unitStrenght = 1;
                break;
            case "Button ManRifle":
                unitCost = 5;
                unitIndex = 1;
                unitStrenght = 3;
                break;
            case "Button Soldier":
                unitCost = 10;
                unitIndex = 2;
                unitStrenght = 6;
                break;
            case "Button SoldierRifle":
                unitCost = 16;
                unitIndex = 3;
                unitStrenght = 14;
                break;
            case "Button SoldierMachine":
                unitCost = 20;
                unitIndex = 4;
                unitStrenght = 10;
                break;
            case "Button HitMan":
                unitCost = 35;
                unitIndex = 5;
                unitStrenght = 15;
                break;
            case "Button Tank":
                unitCost = 50;
                unitIndex = 6;
                unitStrenght = 20;
                break;
            case "Button TankSuper":
                unitCost = 100;
                unitIndex = 7;
                unitStrenght = 26;
                break;
            default:
                unitCost = 0;
                unitIndex = -1;
                unitStrenght = 0;
                break;
        }

        if (coins > unitCost)
        {
            coins -= unitCost;
            var unit = Instantiate(unitPrefabs[unitIndex]);
            unit.tag = "EnemyUnit";
            for (int i = 0; i < unit.transform.childCount; i++)
            {
                unit.transform.GetChild(i).tag = unit.tag;
            }
            unit.GetComponent<SoldierUnit>().ai = ai;
            unit.GetComponent<SoldierUnit>().IsAI = true;
            unit.GetComponent<SoldierUnit>().unitManager = this;
            unit.tag = "EnemyUnit";
            enemyUnits.Add(unit);
            enemyStrenght += unitStrenght;
        }

        return coins;
    }

    private void Update()
    {
        int keyID = 0;
        
        foreach (KeyCode keyCode in keyCodes)
        {
            if (Input.GetKeyDown(keyCode))
            {
                SpawnUnit(unitIDs[keyID]);
                break;
            }

            keyID++;
        }
        
        switch (playerUnits.Count)
        {
            case 10:
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 8, Time.deltaTime);
                break;
            case 24:
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 10, Time.deltaTime);
                break;
            case 30:
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 14, Time.deltaTime);
                break;
            case 50:
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 19, Time.deltaTime);
                break;
            case 60:
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 20, Time.deltaTime);
                break;
        }

        List<Vector3> targetPositionList = GetPositionListAround(unitSpawnLocation.transform.position, 
            new float[] { 3, 5, 7, 9 },
            new int[] { 5, 15, 18, 20 });

        int targetPositionListIndex = 0;

        for (int i = 0; i < playerUnits.Count; i++)
        {
            playerUnits[i].transform.position = targetPositionList[targetPositionListIndex];
            playerUnits[i].transform.rotation = player.transform.GetChild(1).rotation;
            targetPositionListIndex = (targetPositionListIndex + 1) % targetPositionList.Count;
        }
    }

    public static List<Vector3> GetPositionListAround(Vector3 startPostition, float[] ringDistanceArray, int[] ringPostitionCountArray)
    {
        List<Vector3> positionList = new List<Vector3>();
        positionList.Add(startPostition);
        for (int i = 0; i < ringDistanceArray.Length; i++)
        {
            positionList.AddRange(GetPositionListAround(startPostition, ringDistanceArray[i], ringPostitionCountArray[i]));
        }
        return positionList;
    }

    public static List<Vector3> GetPositionListAround(Vector3 startPosition, float distance, int positionCount)
    {
        List<Vector3> positionList = new List<Vector3>();
        for (int i = 0; i < positionCount; i++)
        {
            float angle = i * (360f / positionCount);
            Vector3 dir = ApplyRotationToVector(new Vector3(1, 0), angle);
            Vector3 position = startPosition + dir * distance;
            positionList.Add(position);
        }
        return positionList;
    }

    public static Vector3 ApplyRotationToVector(Vector3 vec, float angle)
    {
        return Quaternion.Euler(0, 0, angle) * vec;
    }
}
