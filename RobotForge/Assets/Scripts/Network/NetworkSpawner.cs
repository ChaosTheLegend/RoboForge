using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Network;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [AssetsOnly]
    [SerializeField] private NetworkObject playerObject;
    [SerializeField] private string roomName;
    [SerializeField] private List<Vector3> spawnPoints;
    [ShowInInspector] [ReadOnly] private Dictionary<PlayerRef, NetworkObject> spawnedPlayers;

    private void Awake()
    {
        spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();
    }
    
    
    private NetworkRunner _runner;

    private void OnGUI()
    {
        if (_runner == null)
        {
            if (GUI.Button(new Rect(0,0,200,40), "Host"))
            {
                StartGame(GameMode.Host);
            }
            if (GUI.Button(new Rect(0,40,200,40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }
    }
    async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = roomName,
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneObjectProvider = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }
    
    

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        var spawnPoint = spawnPoints[spawnedPlayers.Count];

        var playerObj = runner.Spawn(playerObject, spawnPoint, Quaternion.identity, player);
        
        spawnedPlayers.Add(player, playerObj);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (spawnedPlayers.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            spawnedPlayers.Remove(player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
        var data = RecordInputs();
        
        data.ProcessedButtons = false;
        data.ProcessedMovement = false;
        data.ProcessedSecondaryMovement = false;
        
        input.Set(data);
    }

    
    private NetworkInputData RecordInputs()
    {
        var data = new NetworkInputData();
        //used to easily access axies
        var prefix = "Player1_";
        
        //Buttons
        data.Jump = RecordButton(prefix + "Jump");
        data.Run = RecordButton(prefix + "Run");
        data.Interact = RecordButton(prefix + "Interact");
        data.Dash = RecordButton(prefix + "Dash");
        data.Crouch = RecordButton(prefix + "Crouch");
        data.Shoot = RecordButton(prefix + "Shoot");
        data.SecondaryShoot = RecordButton(prefix + "SecondaryShoot");
        data.Reload = RecordButton(prefix + "Reload");
        data.SwitchWeapon = RecordButton(prefix + "SwitchWeapon");
        data.Pause = RecordButton(prefix + "Pause");
        data.TimeControl = RecordButton(prefix + "TimeControl");
        data.SwitchCharacter = RecordButton(prefix + "SwitchCharacter");

        
        //Axies
        data.PrimaryMovement = new Vector2(Input.GetAxis(prefix + "Horizontal"), Input.GetAxis(prefix + "Vertical")).normalized;
        data.SecondaryMovement = new Vector2(Input.GetAxis(prefix + "SecondaryHorizontal"), Input.GetAxis(prefix + "SecondaryVertical")).normalized;
        return data;
    }
    private MMInput.ButtonStates RecordButton(string axisName)
    {
        if (Input.GetButton(axisName))
        {
            return MMInput.ButtonStates.ButtonPressed;
        }
        if (Input.GetButtonDown(axisName))
        {
            return MMInput.ButtonStates.ButtonDown;
        }
        if (Input.GetButtonUp(axisName))
        {
            return MMInput.ButtonStates.ButtonUp;
        }

        return MMInput.ButtonStates.Off;
    }
    
    
    
    

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    { }

    public void OnConnectedToServer(NetworkRunner runner)
    { }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    { }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    { }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    { }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    { }

    public void OnSceneLoadDone(NetworkRunner runner)
    { }

    public void OnSceneLoadStart(NetworkRunner runner)
    { }
}
