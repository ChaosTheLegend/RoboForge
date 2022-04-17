using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugResolutionUi : MonoBehaviour
{
    private int currentResolution;

    [SerializeField]
    private List<Vector2Int> resolutions;
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0,80,200,20), "change resolution"))
        {
            currentResolution++;
            currentResolution %= resolutions.Count;
            var newRes = resolutions[currentResolution];
            Screen.SetResolution(newRes.x, newRes.y, FullScreenMode.Windowed);
        }
    
    }
}
