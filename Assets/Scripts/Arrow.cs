using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Transform[] points;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void SetUpLine(Transform[] points)
    {
        lineRenderer.positionCount = points.Length;
        this.points = points;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < points.Length; i++)
        {
            lineRenderer.SetPosition(i, points[i].position);
        }
    }
}
