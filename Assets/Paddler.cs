using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddler : MonoBehaviour
{
    public GameObject paddle;
    public float speed = 10;
    public float catchAngle = 30;
    public float releaseAngle = -30;
    public string actionKeyName = "space";
    bool actionQueued;
    float? currentTime;

    public bool isInDrivePhase => currentTime.HasValue && currentTime.Value * speed < (float)Math.PI;

    // Start is called before the first frame update
    void Start()
    {
        paddle.transform.parent = transform;
        paddle.transform.localRotation = paddleQuat(0);
    }

    float paddleAngle(float t) => (-(float)Math.Cos(t * speed) + 1) / 2 * (releaseAngle - catchAngle) + catchAngle;
    Quaternion paddleQuat(float t) => Quaternion.AngleAxis(paddleAngle(t), new Vector3(0, 0, 1));

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(actionKeyName))
        {
            actionQueued = true;
        }
        else if (Input.GetKeyUp(actionKeyName))
        {
            actionQueued = false;
        }

        if (actionQueued)
        {
            if (!currentTime.HasValue)
            {
                currentTime = 0;
                actionQueued = false;
            }
        }

        if (currentTime.HasValue)
        {
            currentTime += Time.deltaTime;
            if (currentTime * speed >= 2 * (float)Math.PI)
            {
                currentTime = null;
            }
        }

        if (currentTime.HasValue)
        {
            paddle.transform.localRotation = paddleQuat(currentTime.Value);
        }
        else
        {
            paddle.transform.localRotation = paddleQuat(0);
        }
    }
}
