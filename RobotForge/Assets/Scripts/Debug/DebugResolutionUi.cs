using System.Collections.Generic;
using Fusion;
using UnityEngine;
using LogType = UnityEngine.LogType;

public class DebugResolutionUi : MonoBehaviour
{
    private int currentResolution;

    [SerializeField]
    private List<Vector2Int> resolutions;

    private  bool isOpen;

    static string myLog = "";
    private string output;
    private string stack;

    void OnEnable()
    {
        Application.logMessageReceived += Log;
        
    }
     
    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    private void OnGUI()
    {
        var screenWidth = Screen.width;
        var screenHeight = Screen.height;
        
        
        
        //make a button that toggles debug ui
        if (GUI.Button(new Rect(screenWidth - 200, 0, 200, 20), "Open Debug UI"))
        {
            isOpen = !isOpen;
        }
        if(!isOpen) return;
        
        
        if (GUI.Button(new Rect(screenWidth - 200,20,200,20), "change resolution"))
        {
            currentResolution++;
            currentResolution %= resolutions.Count;
            var newRes = resolutions[currentResolution];
            Screen.SetResolution(newRes.x, newRes.y, FullScreenMode.Windowed);
        }
        
        //Draw games fps
        GUI.Label(new Rect(screenWidth - 200,  screenHeight - 20, 200, 20), "FPS: " + (1f / Time.deltaTime).ToString("0.0"));
        
        //Draw a field that displays all the console logs
        GUI.TextArea(new Rect(screenWidth - 200, 40, 200, 200), myLog);
        
    }

    private void Log(string logString, string stackTrace, LogType type)
    {
        output = logString;
        stack = stackTrace;
        myLog = output + "\n" + myLog;
        if (myLog.Length > 5000)
        {
            myLog = myLog.Substring(0, 4000);
        }
    }
}
