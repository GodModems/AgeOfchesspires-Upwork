using Fusion;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPlayer : NetworkBehaviour
{
    public enum EGameState
    {
        Lobby,
        GameCutscene,
        GameReady
    }

    public static readonly List<RoomPlayer> Players = new List<RoomPlayer>();

    public static Action<RoomPlayer> PlayerJoined;
    public static Action<RoomPlayer> PlayerLeft;
    public static Action<RoomPlayer> PlayerChanged;
    public static Action<RoomPlayer> PlayerGOCSet;
    public static Action<RoomPlayer> PlayerRollCompleted;
    public static Action<RoomPlayer> PlayerPosSet;
    public static Action<RoomPlayer> PlayerShowingArmySelect;


    public static RoomPlayer Local;
    public static MultiplayerGamePlay multiplayerGamePlay;
    public int gocPosOpp;
    public int charPosOpp;

    [Networked(OnChanged = nameof(OnStateChanged))] public NetworkBool IsReady { get; set; }
    [Networked(OnChanged = nameof(OnStateChanged))] public NetworkString<_32> Username { get; set; }
    private static void OnStateChanged(Changed<RoomPlayer> changed) => PlayerChanged?.Invoke(changed.Behaviour);
    //Game Of Chance Ready
    [Networked(OnChanged = nameof(OnGOCChanged))] public NetworkBool IsGOCSetup { get; set; }
    private static void OnGOCChanged(Changed<RoomPlayer> changed) => PlayerGOCSet?.Invoke(changed.Behaviour);
    //Dice roll Ready
    [Networked(OnChanged = nameof(OnIsRollChanged))] public NetworkBool IsRollCompleted { get; set; }
    private static void OnIsRollChanged(Changed<RoomPlayer> changed) => PlayerRollCompleted?.Invoke(changed.Behaviour);
    //Position Selection Ready
    [Networked(OnChanged = nameof(OnPosChanged))] public NetworkBool IsSelecPosSetup { get; set; }
    private static void OnPosChanged(Changed<RoomPlayer> changed) => PlayerPosSet?.Invoke(changed.Behaviour);

    [Networked(OnChanged = nameof(OnShowingArmySelection))] public NetworkBool IsShowingArmySelection { get; set; }
    private static void OnShowingArmySelection(Changed<RoomPlayer> changed) => PlayerShowingArmySelect?.Invoke(changed.Behaviour);

    [Networked] public NetworkBool HasFinished { get; set; }
    [Networked] public EGameState GameState { get; set; }

    public string myTimerText { get; set; }

    public int playSessionCount { get; set; }

    public bool IsLeader => Object != null && Object.IsValid && Object.HasStateAuthority;

    [SerializeField]
    private GameObject nameText;
    [SerializeField]
    private GameObject pointsText;
    [SerializeField]
    private GameObject turnIndic;
    [SerializeField]
    private GameObject timeIndic;
    [SerializeField]
    private GameObject playerObj;
    [SerializeField]
    private GameObject passTurnBtn;

    [SerializeField]
    private TimerController timerController;

    public Image playerDp;

    public static int isGOC { get; set; }
    public static int actionTime { get; set; }
    public static int gpMode { get; set; }
    public static string userName { get; set; }

    public static int currCivOpp { get; set; }

    private static int currentOppELO { get; set; }

    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasInputAuthority)
        {
            Local = this;


            Local.Username = PlayerPrefs.GetString("localUserName");
            nameText.GetComponent<TextMeshProUGUI>().text = Local.Username.ToString();
            //playerDp.sprite = FindObjectOfType<PlayfabManager>().GetSelectedDp();
            playerDp.transform.localScale = new Vector3(1,1,1);
            Debug.Log($"Player {Local.Username} Spawned++");
            PlayerChanged?.Invoke(this);
            
            RPC_SetPlayerStats(Local.Username);
        }


        Players.Add(this);
        PlayerJoined?.Invoke(this);


        DontDestroyOnLoad(gameObject);
    }

   

    public int GetCurrentOppELO()
    {
        return currentOppELO;
    }

    

    public TimerController GetTimer()
    {
        return timerController;
    }

    public static List<RoomPlayer> GetPlayers()
    {
        return Players;
    }

    public static RoomPlayer GetPlayerAt(int index)
    {
        return Players[index];
    }

    public GameObject GetPlayerObj()
    {
        return playerObj;
    }

    public void DeactivateTurnIndic()
    {
        turnIndic.SetActive(false);
        passTurnBtn.SetActive(false);

    }

    public void DeactivateTimeIndic()
    {
        timeIndic.SetActive(false);
        passTurnBtn.SetActive(false);

    }

    public void ActivateTurnIndic()
    {
        turnIndic.SetActive(true);
        passTurnBtn.SetActive(true);
    }

    public void ActivateTimeIndic()
    {
        timeIndic.SetActive(true);
        passTurnBtn.SetActive(false);
        timeIndic.GetComponent<TimerDisplay>().StartTimer(actionTime);
    }

    public void PassTurn()
    {
        FindObjectOfType<MultiplayerGamePlay>().PassTurn();
    }

    public void SetNameText(string name)
    {
        nameText.GetComponent< TextMeshProUGUI >().text = name;
    }

    public void SetPointsText(string points)
    {
        pointsText.GetComponent<TextMeshProUGUI>().text = points;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RPC_SetPlayerStats(NetworkString<_32> username)
    {
        Debug.Log($"RPC-Set Player Stat {username}");
        Username = username;
        
    }


    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_ChangeReadyState(NetworkBool state)
    {
        Debug.Log($"Setting {Object.Name} ready state to {state}");
        IsReady = state;
    }

    //GamePlayData
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_ChangeGOCSetup(NetworkBool state)
    {
        Debug.Log($"Setting {Object.Name} GOC state to {state}");
        IsGOCSetup = state;
    }

    

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All, InvokeLocal = false)]
    public void RPC_ArmySetup(uint id, int armySelected)
    {
        Debug.Log("Opp Selected army::"+ armySelected);

        int myArmy = armySelected;
        if (!FindObjectOfType<MultiplayerGamePlay>().isMyTeam(id))
        {
            myArmy = (armySelected == 0 ? 1 : 0);
        }
       
        PlayerPrefs.SetInt("PlayerArmySelected", myArmy);
        PlayerPrefs.Save();
        GameObject.FindWithTag("GameController").GetComponent<GameController>()._playerSelectedArmy = myArmy;

        int currentGPMode = PlayerPrefs.GetInt("gameplayMode", 0);
        if (currentGPMode == 0)
        {
            //UIManager.Instance.CloseDiceRollPanel();
            //UIManager.Instance.ShowCharSelection();
        }
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All, InvokeLocal = false)]
    public void RPC_UpdateTurnCount()
    {
        playSessionCount += 1;
        FindObjectOfType<MultiplayerGamePlay>().SyncTurnCounter();
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All, InvokeLocal = false)]
    public void RPC_ChangePosSetup(int OppPos, NetworkBool state)
    {
        Debug.Log($"Setting {Object.Name} POS state to {state}");
        Debug.Log("POS::RPC Opp Char Selection Pos:" + OppPos);
        //PlayerPrefs.SetInt("switchCharOpp", OppPos);
        //PlayerPrefs.Save();
        FindObjectOfType<MultiplayerGamePlay>().playerHandler.SyncCharPosOpp(OppPos);

        IsSelecPosSetup = state;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All, InvokeLocal = false)]
    public void RPC_SetMovement(uint localId, int cx, int cy, int ccolour, int px, int py, int pcolour, bool isChip, bool isRescue, int isPromoted)
    {
        Debug.Log(Local.Username+"'s::Opponent Moved::" + px + "," + py + ",and colour:" + pcolour+"isChip:"+isChip+"isRescue:"+isRescue);
        FindObjectOfType<MultiplayerGamePlay>().SyncMoveToTheBoard(localId, new Char_Point(cx, cy, (colour)ccolour), new Char_Point(px, py, (colour)pcolour), isChip, isRescue, isPromoted);

    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All, InvokeLocal = false)]
    public void RPC_SetConversion(uint localId, int cx, int cy, int ccolour, int promoOption, bool isMutation)
    {
        FindObjectOfType<MultiplayerGamePlay>().SyncTransform(localId, new Char_Point(cx, cy, (colour)ccolour), promoOption, isMutation);
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All, InvokeLocal = false)]
    public void RPC_SyncPostMutation(int index)
    {
        FindObjectOfType<MultiplayerGamePlay>().SyncPostMutation(index);
    }

   [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All, InvokeLocal = false)]
    public void RPC_PassTurn(uint candidateId)
    {
       
        FindObjectOfType<MultiplayerGamePlay>().GetTurn(candidateId);
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All, InvokeLocal = false)]
    public void RPC_SendCiv(int civ)
    {
        Debug.Log("RPC Received Opp Civ::"+civ);
        currCivOpp = civ;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All, InvokeLocal = false)]
    public void RPC_SendElo(int elo)
    {
        Debug.Log("RPC Received Opp Elo::" + elo);
        currentOppELO = elo;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All, InvokeLocal = false)]
    public void RPC_SendPoints(float points)
    {
        
        pointsText.GetComponent< TextMeshProUGUI >().text = points.ToString();
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All, InvokeLocal = false)]
    public void RPC_SyncGOCOpp(int r)
    {
        gocPosOpp = r;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All, InvokeLocal = false)]
    public void RPC_ShowCheck(bool isInCheck)
    {
        FindObjectOfType<MultiplayerGamePlay>().SyncShowCheck(isInCheck);
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All, InvokeLocal = false)]
    public void RPC_SendWon(uint playerId, bool isWon, bool isLeaving)
    {
        FindObjectOfType<MultiplayerGamePlay>().SyncGameOver(playerId, isWon, isLeaving);
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All, InvokeLocal = false)]
    public void RPC_SyncCaptured(uint localId, int cx, int cy, int cColour, bool isForceAdd, bool isChip = false)
    {
        FindObjectOfType<MultiplayerGamePlay>().SyncCaptured(localId, cx, cy, cColour, isForceAdd, isChip);
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All, InvokeLocal = false)]
    public void RPC_SyncDied(uint localId, bool isOpp, int cx, int cy, int cColour,  bool isChip = false)

    {
        FindObjectOfType<MultiplayerGamePlay>().SyncKilled(localId,isOpp, cx,cy, cColour, isChip);
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All, InvokeLocal = false)]
    public void RPC_SetRemoval(uint localId, int cx, int cy, int ccolour, bool isChip, bool isMine=false)
    {
        Debug.Log("Sending Sync Removal::isMine::"+isMine);
        FindObjectOfType<MultiplayerGamePlay>().SynSetRemoval(localId, cx, cy, ccolour, isChip, isMine);

    }

    
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All, InvokeLocal = false)]
    public void RPC_SetReturnToBoard(uint localId, int cx, int cy, int ccolour, CharacterDef characterDef)
    {
        FindObjectOfType<MultiplayerGamePlay>().SynSetReturnToBoard(localId, cx, cy, ccolour, characterDef);

    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All, InvokeLocal = false)]
    public void RPC_SendMoveStep(string step)
    {
        FindObjectOfType<MultiplayerGamePlay>().UpdateMovingStepOpp(step);

    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All, InvokeLocal = false)]
    public void RPC_DisparRescue()
    {
        FindObjectOfType<MultiplayerGamePlay>().SynDisappearResceText();
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All, InvokeLocal = false)]
    public void RPC_DisplayRescue()
    {
        FindObjectOfType<MultiplayerGamePlay>().SyncDisplayRescueText();
    }

    private void OnDisable()
    {
        // OnDestroy does not get called for pooled objects
        PlayerLeft?.Invoke(this);
        Players.Remove(this);
    }

    public static void RemovePlayer(NetworkRunner runner, PlayerRef p)
    {
        var roomPlayer = GetPlayers().FirstOrDefault(x => x.Object.InputAuthority == p);
        if (roomPlayer != null)
        {
            GetPlayers().Remove(roomPlayer);
            runner.Despawn(roomPlayer.Object);
        }
    }

    public static void InformDisconnection(PlayerRef p)
    {
        var roomPlayer = GetPlayers().FirstOrDefault(x => x.Object.InputAuthority == p);
        if (roomPlayer != null)
        {
            Debug.Log("You are disconnected!!");
        }

       
    }

   
}
