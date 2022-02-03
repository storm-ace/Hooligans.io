using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour
{
    public int aliveTime = 10;

    private void Start()
    {
        Invoke("Kill", aliveTime);
    }

    private void Kill() => Destroy(gameObject);

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.10f * Time.deltaTime);
    }
}
