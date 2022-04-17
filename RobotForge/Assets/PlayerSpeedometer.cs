using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerSpeedometer : NetworkBehaviour
{
    private List<Vector3> previousPositions = new List<Vector3>();


    private enum ScreenPosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
    
    
    
    [Networked] private float averageSpeed { get; set; }
    [Networked] private float maxSpeed { get; set; }
    
    [SerializeField] private int maxSamples = 10;
    [SerializeField] private  ScreenPosition speedometerPosition;
    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority) return;
        
        //save current position in list
        previousPositions.Add(transform.position);
        
        //if list is too long, remove oldest position
        if (previousPositions.Count > maxSamples)
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
        averageSpeed /= Time.fixedDeltaTime;
       
        //save max speed
        maxSpeed = Mathf.Max(maxSpeed, averageSpeed);
        
    }

    private void OnGUI()
    {
        //Draw speedometer based on the spedometer position
        switch (speedometerPosition)
        {
            case ScreenPosition.TopLeft:
                DrawSpeedometer(new Rect(10, 10, 120, 100), averageSpeed, maxSpeed);
                break;
            case ScreenPosition.TopRight:
                DrawSpeedometer(new Rect(Screen.width - 130, 10, 120, 100), averageSpeed, maxSpeed);
                break;
            case ScreenPosition.BottomLeft:
                DrawSpeedometer(new Rect(10, Screen.height - 110, 120, 100), averageSpeed, maxSpeed);
                break;
            case ScreenPosition.BottomRight:
                DrawSpeedometer(new Rect(Screen.width - 130, Screen.height - 110, 120, 100), averageSpeed, maxSpeed);
                break;
        }
    }

    private void DrawSpeedometer(Rect p0, float p1, float p2)
    {
        //Draw speed and max speed with 1 decimal place
        GUI.Label(p0,  "Average speed: "+$"{p1:0.0}" + " m/s");
        GUI.Label(new Rect(p0.x, p0.y + 20, 100, 100), "Max speed: "+$"{p2:0.0}" + " m/s");
    }
}
