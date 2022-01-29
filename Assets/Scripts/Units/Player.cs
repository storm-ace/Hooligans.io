using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject warningPanel, deadIcon, playerSprite;
    [SerializeField] Text warningText;

    private UnitManager unitManager;

    public List<Text> buyTextList = new List<Text>();
    public GameObject world;

    [SerializeField] private float coins = 0;
    public float Coins
    {
        get { return coins; } 
        set { coins = value; }
    }
    
    [SerializeField] private float health = 100;
    public float Health {
        get { return health; }
        set { health = value; }
    }

    public Text coinText;

    bool warning = false;

    Coroutine warningCoroutine;

    private GameObject target, selected;

    float worldSize;

    private void Awake()
    {
        unitManager = GameObject.FindObjectOfType<UnitManager>();
    }

    private void Start()
    {
        worldSize = world.GetComponent<Renderer>().bounds.extents.magnitude - 10;
    }

    private void Update()
    {
        for (int i = 0; i < buyTextList.Count; i++)
        {
            int.TryParse(buyTextList[i].text.TrimEnd('x'), out int result);
            if (coins >= result) buyTextList[i].color = Color.white; else buyTextList[i].color = Color.red;
        }

        if (transform.position.x > worldSize || transform.position.x < -worldSize || transform.position.y > worldSize || transform.position.y < -worldSize)
        {
            if (warning) return;

            warning = true;
            warningCoroutine = StartCoroutine(LeaveZoneWarning());
        }
        else if (warning)
        {
            StopCoroutine(warningCoroutine);
            warning = false;
            warningPanel.SetActive(false);
        }

        LockTarget();
    }

    public GameObject markTarget()
    {
        selected.SetActive(true);
        return gameObject;
    }

    private void LockTarget()
    {
        RaycastHit2D hitResult = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.up, .1f);
        if (hitResult.collider && hitResult.collider.GetComponent<AI>())
        {
            target = hitResult.collider.GetComponent<AI>().markTarget();
        }

        if (target)
        {
            foreach (var unit in unitManager.playerUnits)
            {
                unit.GetComponent<SoldierUnit>().ScanForHostiles();
            }
        }
    }

    public void UpdateCoins()
    {
        coinText.text = coins.ToString() + "x";
    }

    IEnumerator LeaveZoneWarning()
    {
        float lenght = 3f;
        warningPanel.SetActive(true);

        while (lenght >= 0)
        {
            warningText.text = $"Return to the battlefield!\n {lenght}";
            lenght--;
            yield return new WaitForSeconds(1f);
        }

        GetComponent<Movement>().enabled = false;
        warningPanel.SetActive(false);
        playerSprite.SetActive(false);
        deadIcon.SetActive(true);

        foreach (var unit in unitManager.playerUnits)
        {
            unit.GetComponentInChildren<SpriteRenderer>().color = new Color(255, 255, 255, .25f);
        }

        unitManager.ghostUnits = unitManager.playerUnits.ToList();
        unitManager.playerUnits.Clear();
    }
}
