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
    
    private NetworkObject selectedCharacter;
    [SerializeField] private string roomName;

    [AssetsOnly]
    [SerializeField] private NetworkObject[] playerObjects;

    [SerializeField] private List<Vector3> spawnPoints;
    [ShowInInspector] [ReadOnly] private Dictionary<PlayerRef, NetworkObject> spawnedPlayers;

    private void Awake()
    {
        spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();
    }
    
    
    private NetworkRunner _runner;

    #region DebugUi
    private enum GuiState
    {
        CharacterSelect,
        NetworkSelect,
        Running
    }

    private GuiState currentState;
    private void OnGUI()
    {
        if (_runner == null)
        {
            

            if (currentState == GuiState.CharacterSelect)
            {
                //Draw buttons for each character
                for (int i = 0; i < playerObjects.Length; i++)
                {
                    if (GUI.Button(new Rect(Screen.width/2 - 100, Screen.height/2 - 60 + i * 45, 200, 40), playerObjects[i].name))
                    {
                        selectedCharacter = playerObjects[i];
                        currentState = GuiState.NetworkSelect;
                    }
                }
            }

            if (currentState == GuiState.NetworkSelect)
            {
                if (GUI.Button(new Rect(Screen.width/2 - 100, Screen.height/2 - 40, 200, 40), "Host"))
                {
                    StartGame(GameMode.Host);
                    currentState = GuiState.Running;
                }

                if (GUI.Button(new Rect(Screen.width/2 - 100, Screen.height/2 + 5, 200, 40), "Join"))
                {
                    StartGame(GameMode.Client);
                    currentState = GuiState.Running;
                }
            }

        }
    }
    #endregion
    
    
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

        var playerObj = runner.Spawn(selectedCharacter, spawnPoint, Quaternion.identity, player);
        
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
        data.MousePosition = Input.mousePosition;
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
