using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour
{
    private SpriteRenderer rend;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        rend.color = new Color(255, 255, 255, .4f);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        rend.color = new Color(255, 255, 255, 1f);
    }
}
