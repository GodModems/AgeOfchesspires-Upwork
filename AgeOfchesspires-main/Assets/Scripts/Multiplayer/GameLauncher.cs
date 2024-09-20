using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using Photon.Realtime;
using Photon.Voice;
//using Photon.Voice.Fusion;
using Photon.Voice.Unity;
using Photon.Voice.Unity.UtilityScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.Unicode;

public enum ConnectionStatus
{
	Disconnected,
	Connecting,
	Failed,
	Connected
}

public class GameLauncher : MonoBehaviour, INetworkRunnerCallbacks
{
	[SerializeField]
	private RoomPlayer _roomPlayerPrefab;

	//private FusionVoiceClient _voiceClient;

	[SerializeField]
	private GameObject _runnerVoicePrefab;

	//[SerializeField]
	//private GameObject _speakerPrefab;

	private List<SessionInfo> _sessions = new List<SessionInfo>();

	public static ConnectionStatus ConnectionStatus = ConnectionStatus.Disconnected;

	public GameMode _gameMode;
	private NetworkRunner _runner;

	private int _gamePlayMode;
	private int _gamePlayType;


	private void Start()
	{
		Application.runInBackground = true;
		Application.targetFrameRate = Screen.currentResolution.refreshRate;
		QualitySettings.vSyncCount = 1;


		DontDestroyOnLoad(gameObject);

	}

	//public void SetCreateLobby(){
	//	_gameMode = GameMode.Host;
	//}
	//public void SetJoinLobby()
	//{
	//	_gameMode = GameMode.Client;
	//}

	public void SetGameplayMode1v1()
	{ 
		_gamePlayMode = 1;
		UIManager.Instance.OnMode1v1Selected();

    }
    public void SetGameplayMode1v2() => _gamePlayMode = 2;
    public void SetGameplayMode2v2() => _gamePlayMode = 3;
    public void SetGamePlayTypeStandard() => _gamePlayType = 1;
    public void SetGamePlayTypeChance() => _gamePlayType = 2;

    //private bool isQuick=false;

	public MatchHistoryManager matchHistoryManager;
	public bool sessionJoined = false;

    public async void JoinSessionLobby()
	{
        Debug.Log("OnJoinSessionLobby");
        UIManager.Instance.ShowLoading();
        SetConnectionStatus(ConnectionStatus.Connecting);

        if (_runner != null)
            LeaveSession();

        //GameObject go = new GameObject("Session");
        //DontDestroyOnLoad(go);

        GameObject go = Instantiate(_runnerVoicePrefab);
        DontDestroyOnLoad(go);
        _runner = go.GetComponent<NetworkRunner>(); //go.AddComponent<NetworkRunner>();
        _runner.AddCallbacks(this);


        await JoinSessionLobbyTask(_runner);

    }


    private async Task JoinSessionLobbyTask(NetworkRunner runner)
    {
        
        var result = await runner.JoinSessionLobby(SessionLobby.ClientServer);

        if (result.Ok)
        {
			// all good
			sessionJoined = true;
			Debug.Log("MA::Session joined!!:"+sessionJoined);
        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }

        UIManager.Instance.HideLoading();

    }

	public NetworkRunner GetCurrentRunner()
	{
		return _runner;
	}

    public void CreateALobby()
    {
        Debug.Log("OnJoinOrCreateLobby");

        SetConnectionStatus(ConnectionStatus.Connecting);

        if (_runner != null)
            LeaveSession();

        /* GameObject go = new GameObject("Session");
         DontDestroyOnLoad(go);
		_runner = go.AddComponent<NetworkRunner>();
		        _runner.AddCallbacks(this);

		*/

        GameObject go = Instantiate(_runnerVoicePrefab);
        DontDestroyOnLoad(go);
        _runner = go.GetComponent<NetworkRunner>(); //go.AddComponent<NetworkRunner>();
        _runner.AddCallbacks(this);

        var customProps = new Dictionary<string, SessionProperty>();

        customProps["actionTime"] = PlayerPrefs.GetInt("ActionTime", -1);
        customProps["gameplayMode"] = PlayerPrefs.GetInt("gameplayMode", 0);

        Debug.Log("isGOC===" + PlayerPrefs.GetInt("isChance", 0));

        customProps["isGOC"] = PlayerPrefs.GetInt("isChance", 0);

		//customProps["roomName"] = ServerInfo.LobbyName;

        Debug.Log("LobbyNameServer===" + ServerInfo.LobbyName);
        Debug.Log("LobbyNameClient===" + ClientInfo.LobbyName);

		_gameMode = GameMode.Host;//(GameMode)FindObjectOfType<UIManager>().currentMultiplayerGameMode;

		bool isRoomPriv = true;// PlayerPrefs.GetInt("privateLobby", 1) == 1;
        if (isRoomPriv)
		{
			customProps["isPriv"] = 1;
			customProps["RoomCode"] = UIManager.Instance.roomCodeVal;

            Debug.Log("Create Game With Code::" + customProps["RoomCode"]);

        }
        else
		{
            customProps["isPriv"] = 0;
        }

        Debug.Log("GameMode===" + _gameMode);

        Debug.Log($"Created gameobject {go.name} - starting game");
		_runner.StartGame(new StartGameArgs
		{
			GameMode = _gameMode,
			//SessionName = _gameMode == GameMode.Host ? ServerInfo.LobbyName : ClientInfo.LobbyName,
			SessionProperties = customProps,
			PlayerCount = PlayerPrefs.GetInt("S_MaxUsers", 2)
		}) ;

    }

    public void JoinALobby(string roomId, int actTime, int goc, int gpMode, bool isPriv, string roomCode)
    {
        Debug.Log("Game Launcher JoinALobby");

        SetConnectionStatus(ConnectionStatus.Connecting);

        if (_runner != null)
            LeaveSession();

        /* GameObject go = new GameObject("Session");
         DontDestroyOnLoad(go);

         _runner = go.AddComponent<NetworkRunner>();
                _runner.AddCallbacks(this);
        */

        GameObject go = Instantiate(_runnerVoicePrefab);
        DontDestroyOnLoad(go);
        _runner = go.GetComponent<NetworkRunner>(); //go.AddComponent<NetworkRunner>();
        _runner.AddCallbacks(this);


        FindObjectOfType<UIManager>().SetIsAIMode(0);


        var customProps = new Dictionary<string, SessionProperty>();

		customProps["actionTime"] = actTime; 
        PlayerPrefs.SetInt("ActionTime", actTime);

		customProps["gameplayMode"] = 0;// gpMode; 
        PlayerPrefs.SetInt("gameplayMode", gpMode);

        Debug.Log("isGOC===" + PlayerPrefs.GetInt("isChance", 0));



        customProps["isGOC"] = goc;//
		PlayerPrefs.SetInt("isChance", 0);
		PlayerPrefs.Save();

		customProps["isPriv"] = isPriv?1:0;
        
		customProps["RoomCode"] = roomCode;

        //Debug.Log("Join Game With Code::" + customProps["RoomCode"]);

        //_gameMode = (GameMode)FindObjectOfType<UIManager>().currentGameMode;

        //Debug.Log("GameMode===" + _gameMode);


        Debug.Log($"Created gameobject {go.name} - starting game");
        _runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Client,
            SessionName = roomId,
            SessionProperties = customProps,
            PlayerCount = PlayerPrefs.GetInt("S_MaxUsers", 2)
        });


    }

    public void QuickJoinLobby()
	{
        Debug.Log("OnJoinOrCreateLobby");

        SetConnectionStatus(ConnectionStatus.Connecting);

        if (_runner != null)
            LeaveSession();

        GameObject go = Instantiate(_runnerVoicePrefab);
        DontDestroyOnLoad(go);
        _runner = go.GetComponent<NetworkRunner>(); //go.AddComponent<NetworkRunner>();
        _runner.AddCallbacks(this);

       /* GameObject go = new GameObject("Session");
        DontDestroyOnLoad(go);
        _runner = go.AddComponent<NetworkRunner>();
        _runner.AddCallbacks(this);
        Debug.Log($"Created gameobject {go.name} - starting game");*/

        FindObjectOfType<UIManager>().currentMultiplayerGameMode = (int)GameMode.AutoHostOrClient;
        FindObjectOfType<UIManager>().SetIsAIMode(0);

        

        

        var customProps = new Dictionary<string, SessionProperty>();
        customProps["isPriv"] = 0;
        customProps["gameplayMode"] = 0;

        _runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionProperties = customProps,
            PlayerCount = PlayerPrefs.GetInt("S_MaxUsers", 2),
        });
    }

    private void SetConnectionStatus(ConnectionStatus status)
	{
		Debug.Log($"Setting connection status to {status}");

        ConnectionStatus = status;
		
		if (!Application.isPlaying)
            return;

       

		if (status == ConnectionStatus.Disconnected || status == ConnectionStatus.Failed)
		{
			//if (!_isRefresh)
			//{
				UIManager.Instance._disconnectUI.gameObject.SetActive(true);
				UIManager.Instance._disconnectUI.ShowMessage("Player Left Room", "Other player Left The Game");
			//}
        }
		else if(status == ConnectionStatus.Connected)
		{
            sessionJoined = true;
        }
	}
	
	private static bool _isRefresh = false;
    public void LeaveSession(bool isRefresh = false)
	{
        _isRefresh = isRefresh;

        if (_runner != null)
		{
			if (_runner.IsCloudReady)
			{
				UIManager.Instance.ShowLoading();
			}
			_runner.Shutdown();

		}
		else
		{
			SetConnectionStatus(ConnectionStatus.Disconnected);
		}

		
		
		
	}

	/*public GameObject GetVoiceObject()
	{
		return voiceChatObj;
    }*/
	
	public List<SessionInfo> GetSessions()
	{
		return _sessions;
	}

	public void OnConnectedToServer(NetworkRunner runner)
	{
		Debug.Log("Connected to server");
		SetConnectionStatus(ConnectionStatus.Connected);

        Debug.Log("MA::Connected to server!!!");
		
    }
    public void OnDisconnectedFromServer(NetworkRunner runner)
	{
		Debug.Log("Disconnected from server");
		LeaveSession();
		SetConnectionStatus(ConnectionStatus.Disconnected);
        sessionJoined = false;
    }
	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
		if (runner.CurrentScene > 0)
		{
			Debug.LogWarning($"Refused connection requested by {request.RemoteAddress}");
			request.Refuse();
		}
		else
		{
			request.Accept();

            /*var customProps = runner.SessionInfo.Properties;
            string userName = customProps["Username"];
            Debug.Log("Opp Username=" + userName);*/
        }
	}
	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
		

		Debug.Log($"Connect failed {reason}");
		LeaveSession();
		SetConnectionStatus(ConnectionStatus.Failed);
		(string status, string message) = ConnectFailedReasonToHuman(reason);

		UIManager.Instance._disconnectUI.gameObject.SetActive(true);
        UIManager.Instance._disconnectUI.ShowMessage(status,message);
	}
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		Debug.Log($"Player {player} Joined!");

        if (runner.IsServer)
		{
            RoomPlayer roomPlayer = runner.Spawn(_roomPlayerPrefab, Vector3.zero, Quaternion.identity, player);			
			roomPlayer.GameState = RoomPlayer.EGameState.Lobby;

            /*_voiceClient = _runner.gameObject.AddComponent<FusionVoiceClient>();
            _voiceClient.PrimaryRecorder = _runner.gameObject.GetComponentInChildren<Recorder>();
            _voiceClient.SpeakerPrefab = _speakerPrefab;*/
        }


        Debug.Log($"A session with props::");
		var customProps = runner.SessionInfo.Properties;
		if (customProps.ContainsKey("actionTime"))
		{			
			int act = customProps["actionTime"];
			RoomPlayer.actionTime = act;
			Debug.Log("act===" + act);	
		}

		if (customProps.ContainsKey("gameplayMode"))
		{
			int gpm = customProps["gameplayMode"];
			RoomPlayer.gpMode = gpm;
			Debug.Log("gpm===" + gpm);		
		}

		if (customProps.ContainsKey("isGOC"))
		{
			int goc = customProps["isGOC"];
			RoomPlayer.isGOC = goc;
			Debug.Log("isGOC===" + goc);			
		}

        if(customProps.ContainsKey("roomName"))
		{
			/*string roomName = customProps["roomName"];
            GameObject voiceChatObj = Instantiate(_voicePrefab, Vector3.zero, Quaternion.identity);
			voiceChatObj.tag = "VoiceObj";
            voiceChatObj.GetComponent<ConnectAndJoin>().RoomName = roomName;
			DontDestroyOnLoad(voiceChatObj);*/

        }

       

        //RoomPlayer.userName = PlayerPrefs.GetString("localUserName");
        //Debug.Log("Username=" + RoomPlayer.userName);

        //string userName = customProps["Username"];
        //Debug.Log("Opp Username=" + userName); 




        SetConnectionStatus(ConnectionStatus.Connected);
	}

	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		Debug.Log($"{player.PlayerId} disconnected.");

		RoomPlayer.RemovePlayer(runner, player);


		SetConnectionStatus(ConnectionStatus.Disconnected);
	}
	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
		

        Debug.Log($"OnShutdown {shutdownReason}");

        (string status, string message) = ShutdownReasonToHuman(shutdownReason);
        SetConnectionStatus(ConnectionStatus.Disconnected);

        RoomPlayer.GetPlayers().Clear();

		if(_runner)
			Destroy(_runner.gameObject);

        UIManager.Instance.HideLoading();
        _runner = null;

		sessionJoined = false;
        Debug.Log("MA::OnShutdown : Session joined!!:" + sessionJoined);


        if (_isRefresh)
		{
			_isRefresh = false;
			JoinSessionLobby();
		}
		else
		{
            //UIManager.Instance._disconnectUI.gameObject.SetActive(true);
            //UIManager.Instance._disconnectUI.ShowMessage( status, message);

            UIManager.Instance.OnLobbyRoom();
        }

    }
	public void OnInput(NetworkRunner runner, NetworkInput input) { }
	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionInfoList)
    {
        Debug.Log($"Session List Updated with {sessionInfoList.Count} session(s)");

		_sessions.Clear();
        _sessions = sessionInfoList;

		RoomUI roomUI = FindObjectOfType<RoomUI>(true);
		if(roomUI != null )
        {
			roomUI.UpdateSessionUI();
        }

        /*foreach( SessionInfo sessionInfo in sessionInfoList )
		{
            Debug.Log($"A session with props::");
            var customProps = sessionInfo.Properties;

            int act = customProps["actionTime"];
            int gpm = customProps["gameplayMode"];
            int goc = customProps["isGOC"];
            Debug.Log("act===" + act);
            Debug.Log("gpm===" + gpm);
            Debug.Log("isGOC===" + goc);
        }*/



    }

    public void UpdateSessionCallback()
    {
		RoomUI roomUI = FindObjectOfType<RoomUI>(true);
		//if (roomUI != null)
		//{ 
			roomUI.UpdateSessionUI();
		//}
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
	public void OnSceneLoadDone(NetworkRunner runner) { }
	public void OnSceneLoadStart(NetworkRunner runner) { }

	private static (string, string) ShutdownReasonToHuman(ShutdownReason reason)
	{
		switch (reason)
		{
			case ShutdownReason.Ok:
				return ("Closed", "The game is closed by the player");
			case ShutdownReason.Error:
				return ("Error", "Shutdown was caused by some internal error");
			case ShutdownReason.IncompatibleConfiguration:
				return ("Incompatible Config", "Mismatching type between client Server Mode and Shared Mode");
			case ShutdownReason.ServerInRoom:
				return ("Room name in use", "There's a room with that name! Please try a different name or wait a while.");
			case ShutdownReason.DisconnectedByPluginLogic:
				return ("Disconnected By Host", "You were kicked, the room may have been closed");
			case ShutdownReason.GameClosed:
				return ("Game Closed", "The session cannot be joined, the game is closed");
			case ShutdownReason.GameNotFound:
				return ("Game Not Found", "This room does not exist");
			case ShutdownReason.MaxCcuReached:
				return ("Max Players", "The Max CCU has been reached, please try again later");
			case ShutdownReason.InvalidRegion:
				return ("Invalid Region", "The currently selected region is invalid");
			case ShutdownReason.GameIdAlreadyExists:
				return ("ID already exists", "A room with this name has already been created");
			case ShutdownReason.GameIsFull:
				return ("Game is full", "This lobby is full!");
			case ShutdownReason.InvalidAuthentication:
				return ("Invalid Authentication", "The Authentication values are invalid");
			case ShutdownReason.CustomAuthenticationFailed:
				return ("Authentication Failed", "Custom authentication has failed");
			case ShutdownReason.AuthenticationTicketExpired:
				return ("Authentication Expired", "The authentication ticket has expired");
			case ShutdownReason.PhotonCloudTimeout:
				return ("Cloud Timeout", "Connection with the Photon Cloud has timed out");
			default:
				Debug.LogWarning($"Unknown ShutdownReason {reason}");
				return ("Unknown Shutdown Reason", $"{(int)reason}");
		}
	}

	private static (string,string) ConnectFailedReasonToHuman(NetConnectFailedReason reason)
	{
		switch (reason)
		{
			case NetConnectFailedReason.Timeout:
				return ("Timed Out", "");
			case NetConnectFailedReason.ServerRefused:
				return ("Connection Refused", "The lobby may be currently in-game");
			case NetConnectFailedReason.ServerFull:
				return ("Server Full", "");
			default:
				Debug.LogWarning($"Unknown NetConnectFailedReason {reason}");
				return ("Unknown Connection Failure", $"{(int)reason}");
		}
	}

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

   
}