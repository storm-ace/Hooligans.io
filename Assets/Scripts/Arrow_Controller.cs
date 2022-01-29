using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_Controller : MonoBehaviour
{
    public Transform[] points;
    private Arrow line;

    private void Start()
    {
        line = GetComponent<Arrow>();
        line.SetUpLine(points);
    }
}
