using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersionDisplay : MonoBehaviour
{
    [SerializeField] private BuildInfo buildInfo;
    
    // Start is called before the first frame update
    private void Start()
    {
        var text = GetComponent<TextMeshProUGUI>();
        text.text = $"Version: {buildInfo.versionNumber}\n\rBuild {buildInfo.buildNumber}";
    }
}
