using UnityEngine;

public class UnitAgent : MonoBehaviour
{
    public float movementSpeed = 1;
    public UnitType unitType;
    public Color unitcolor = Color.red;
    public CircleCollider2D circleCollider;
    public float unitPower = 4;
    public float unitCombatRange = 4;
    public LandChunk landToAttack, landOrigin;
    public SpriteRenderer flag;
    
    private bool flee;

    public void Initialize(LandChunk landToAttack, LandChunk landOrigin)
    {
        this.landToAttack = landToAttack;
        this.landOrigin = landOrigin;
        // flag.sprite = GameStateGO.GameState.flag;
    }

    private void FixedUpdate()
    {
        float step = movementSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, landToAttack.transform.position, step);
        
        var distance = Vector2.Distance(transform.position, landToAttack.transform.position);

        if (flee && distance < .4)
        {
            landOrigin.unitAvailable = true;
            Destroy(gameObject); 
        }
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        var land = col.gameObject.GetComponent<LandChunk>();

        if (land)
        {
            movementSpeed = land.unitMovementSpeed;
            if (land == landToAttack)
            {
                land.IncomingUnits(this);
            }
        }
    }

    public void Retreat()
    {
        landToAttack = landOrigin;
        flee = true;
    }
}

public enum UnitType { Player, Enemy }