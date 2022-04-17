using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerSpeedometer : NetworkBehaviour
{
    private List<Vector3> previousPositions = new List<Vector3>();


    [Networked] private float averageSpeed { get; set; }
    [Networked] private float maxSpeed { get; set; }
    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority) return;
        
        //save current position in list
        previousPositions.Add(transform.position);
        
        //if list is too long, remove oldest position
        if (previousPositions.Count > 10)
        {
            previousPositions.RemoveAt(0);
        }
        
        //calculate average speed
        averageSpeed = 0;
        for (int i = 0; i < previousPositions.Count - 1; i++)
        {
            averageSpeed += Vector3.Distance(previousPositions[i], previousPositions[i + 1]);
        }
        averageSpeed /= previousPositions.Count;
        //save max speed
        maxSpeed = Mathf.Max(maxSpeed, averageSpeed);
        
    }

    private void OnGUI()
    {
        //Draw speedometer at the bottom right corner
        GUI.Label(new Rect(Screen.width - 200, Screen.height - 50, 200, 25), "Speed: " + averageSpeed + " m/s");
        GUI.Label(new Rect(Screen.width - 200, Screen.height - 25, 200, 25), "Max Speed: " + maxSpeed + " m/s");
    }
}
