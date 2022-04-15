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

    private NetworkInputData _lastData;
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = RecordInputs();
        CheckButtonStates(data, _lastData);
        _lastData = data;
        
        data.ProcessedButtons = false;
        data.ProcessedMovement = false;
        data.ProcessedSecondaryMovement = false;
        data.ProcessedShootAxis = false;
        data.ProcessedSecondaryShootAxis = false;

        input.Set(data);
    }

    private NetworkInputData CheckButtonStates(NetworkInputData newData, NetworkInputData oldData)
    {
        var output = new NetworkInputData();
        output.Jump = AddStates(newData.Jump, oldData.Jump);
        output.Run = AddStates(newData.Run, oldData.Run);
        output.Dash = AddStates(newData.Dash, oldData.Dash);
        output.Crouch = AddStates(newData.Crouch, oldData.Crouch);
        output.Interact = AddStates(newData.Interact, oldData.Interact);
        output.Pause = AddStates(newData.Pause, oldData.Pause);
        output.Reload = AddStates(newData.Reload, oldData.Reload);
        output.Shoot = AddStates(newData.Shoot, oldData.Shoot);
        output.SecondaryShoot = AddStates(newData.SecondaryShoot, oldData.SecondaryShoot);
        output.SwitchCharacter = AddStates(newData.SwitchCharacter, oldData.SwitchCharacter);
        output.SwitchWeapon = AddStates(newData.SwitchWeapon, oldData.SwitchWeapon);
        output.TimeControl = AddStates(newData.TimeControl, oldData.TimeControl);

        output.Horizontal = newData.Horizontal;
        output.Vertical = newData.Vertical;
        output.SecondaryHorizontal = newData.SecondaryHorizontal;
        output.SecondaryVertical = newData.SecondaryVertical;
        output.ShootAxis = newData.ShootAxis;
        output.SecondaryShootAxis = newData.SecondaryShootAxis;

        return output;
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
        data.Horizontal = Input.GetAxis(prefix + "Horizontal");
        data.Vertical = Input.GetAxis(prefix + "Vertical");
        data.SecondaryHorizontal = Input.GetAxis(prefix + "SecondaryHorizontal");
        data.SecondaryVertical = Input.GetAxis(prefix + "SecondaryVertical");
        data.ShootAxis = Input.GetAxis(prefix + "ShootAxis");
        data.SecondaryShootAxis = Input.GetAxis(prefix + "SecondaryShootAxis");
        
        return data;
    }

    
    private MMInput.ButtonStates AddStates(MMInput.ButtonStates newState, MMInput.ButtonStates oldState = MMInput.ButtonStates.Off)
    {
        return (oldState, newState) switch
        {
            (MMInput.ButtonStates.Off, MMInput.ButtonStates.Off) => MMInput.ButtonStates.Off,
            (MMInput.ButtonStates.ButtonDown, MMInput.ButtonStates.Off) => MMInput.ButtonStates.ButtonDown,
            (MMInput.ButtonStates.ButtonPressed, MMInput.ButtonStates.Off) => MMInput.ButtonStates.ButtonPressed,
            (MMInput.ButtonStates.ButtonUp, MMInput.ButtonStates.Off) => MMInput.ButtonStates.ButtonUp,
            
            (MMInput.ButtonStates.Off, MMInput.ButtonStates.ButtonDown) => MMInput.ButtonStates.Off,
            (MMInput.ButtonStates.ButtonDown, MMInput.ButtonStates.ButtonDown) => MMInput.ButtonStates.ButtonPressed,
            (MMInput.ButtonStates.ButtonPressed, MMInput.ButtonStates.ButtonDown) => MMInput.ButtonStates.ButtonPressed,
            (MMInput.ButtonStates.ButtonUp, MMInput.ButtonStates.ButtonDown) => MMInput.ButtonStates.ButtonUp,
            
            (MMInput.ButtonStates.Off, MMInput.ButtonStates.ButtonPressed) => MMInput.ButtonStates.Off,
            (MMInput.ButtonStates.ButtonDown, MMInput.ButtonStates.ButtonPressed) => MMInput.ButtonStates.ButtonPressed,
            (MMInput.ButtonStates.ButtonPressed, MMInput.ButtonStates.ButtonPressed) => MMInput.ButtonStates.ButtonPressed,
            (MMInput.ButtonStates.ButtonUp, MMInput.ButtonStates.ButtonPressed) => MMInput.ButtonStates.ButtonUp,

            (MMInput.ButtonStates.Off, MMInput.ButtonStates.ButtonUp) => MMInput.ButtonStates.Off,
            (MMInput.ButtonStates.ButtonDown, MMInput.ButtonStates.ButtonUp) => MMInput.ButtonStates.ButtonDown,
            (MMInput.ButtonStates.ButtonPressed, MMInput.ButtonStates.ButtonUp) => MMInput.ButtonStates.ButtonPressed,
            (MMInput.ButtonStates.ButtonUp, MMInput.ButtonStates.ButtonUp) => MMInput.ButtonStates.ButtonUp,
            
            _ => MMInput.ButtonStates.Off
        };
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
