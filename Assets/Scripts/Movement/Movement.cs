using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed;
    public bool overrideModifiers;

    [SerializeField] Transform playerSprite;

    private Camera camera;

    private void Awake()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        var dir = Input.mousePosition - camera.WorldToScreenPoint(playerSprite.position);
        var speedDetection = Vector3.Distance(dir, transform.position);
        var tempSpeed = speed;

        SpeedByMousePosition(speedDetection, tempSpeed, overrideModifiers);

        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        playerSprite.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Vector3 target = camera.ScreenToWorldPoint(Input.mousePosition);
        target.z = transform.position.z;

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    private void SpeedByMousePosition(float speedDetection, float tempSpeed, bool overrideModifiers = false)
    {
        if (overrideModifiers) return;

        if (speedDetection <= 200) speed = Mathf.Lerp(tempSpeed, 3, Time.deltaTime * 10);
        if (speedDetection <= 50) speed = Mathf.Lerp(tempSpeed, 0, Time.deltaTime * 10);
        if (speedDetection >= 200) speed = Mathf.Lerp(tempSpeed, 5, Time.deltaTime * 10);
        if (speed <= .1f) speed = 0;
    }
}
