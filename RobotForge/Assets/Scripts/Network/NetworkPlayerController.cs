using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Network
{
    public class NetworkPlayerController : NetworkBehaviour
    {
        private NetworkCharacterControllerPrototype _cc;

        private void Awake()
        {
            _cc = GetComponent<NetworkCharacterControllerPrototype>();
        }

        public override void FixedUpdateNetwork()
        {
        }
    }
}

