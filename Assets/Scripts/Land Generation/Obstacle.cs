using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Movement.Movement>() != null)
        {
            collision.GetComponent<Movement.Movement>().overrideModifiers = true;
            collision.GetComponent<Movement.Movement>().speed = 1;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Movement.Movement>() != null)
        {
            collision.GetComponent<Movement.Movement>().overrideModifiers = false;
        }
    }
}
