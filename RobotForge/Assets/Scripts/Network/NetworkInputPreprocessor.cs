using System.Collections;
using System.Collections.Generic;
using Fusion;
using Network;
using UnityEngine;

public class NetworkInputPreprocessor : NetworkBehaviour
{
    private NetworkInputManager inputManager;
    public override void Spawned()
    {
        inputManager = GetComponent<NetworkInputManager>();
        inputManager.InitializeManually();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            inputManager.ProcessButtons(data);
        }
    }
}
