using System.Collections;
using System.Collections.Generic;
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

    public void Initialize(LandChunk landToAttack, LandChunk landOrigin)
    {
        this.landToAttack = landToAttack;
        this.landOrigin = landOrigin;
    }

    private void FixedUpdate()
    {
        float step = movementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(new Vector3(transform.position.x, transform.position.y, -.5f),
            landToAttack.transform.position, step);
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
                //Destroy(gameObject);
            }
        }
    }

    public void Retreat()
    {
        landToAttack = landOrigin;
    }
}

public enum UnitType { Player, Enemy }