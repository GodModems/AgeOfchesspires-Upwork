using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
	public GameObject textPrefab;
	public Transform parent;
	public Button readyUp;
	public Button customizeButton;
	public Button leaveButton;
	public TMP_Text waitTxt;

    public TMP_Text modeTxt;
    public TMP_Text gocTxt;
    public TMP_Text timerTxt;
    public TMP_Text roomCodeTxt;

    public GameObject civsSelectObj;
    public GameObject civsContent;

    private static readonly Dictionary<RoomPlayer, LobbyItemUI> ListItems = new Dictionary<RoomPlayer, LobbyItemUI>();
    private static bool IsSubscribed;

    public GameObject playBtn;
    public GameObject timerBtn;

    private void OnEnable()
    {
        timerBtn.SetActive(false);
        readyUp.GetComponent<TimerQM>().ResetTimer();
        //EnableCivs();
        
    }

    private void Awake()
	{
		Setup();
        RoomPlayer.PlayerChanged += (player) =>
		{
			var isLeader = RoomPlayer.Local.IsLeader;
            if (customizeButton)
            {
                customizeButton.interactable = !RoomPlayer.Local.IsReady;
            }
		};



	}

    

    public void Setup()
	{
		if (IsSubscribed) return;

		RoomPlayer.PlayerJoined += AddPlayer;
		RoomPlayer.PlayerLeft += RemovePlayer;

		RoomPlayer.PlayerChanged += EnsureAllPlayersReady;

		readyUp.onClick.AddListener(ReadyUpListener);

        

		IsSubscribed = true;
    }

    private void SendCiv()
    {
        int myCiv = UIManager.Instance._myCivilization;
        Debug.Log("Sending Civ::"+myCiv);
        RoomPlayer.Local.RPC_SendCiv(myCiv);
    }

    private void OnDestroy()
	{
		if (!IsSubscribed) return;

		RoomPlayer.PlayerJoined -= AddPlayer;
		RoomPlayer.PlayerLeft -= RemovePlayer;

		readyUp.onClick.RemoveListener(ReadyUpListener);

		IsSubscribed = false;

		
    }

	private void AddPlayer(RoomPlayer player)
	{
        if (ListItems.ContainsKey(player))
        {
            var toRemove = ListItems[player];
            Destroy(toRemove.gameObject);

            ListItems.Remove(player);
        }

        GameObject lobbyObject = Instantiate(textPrefab, parent);
        var obj = lobbyObject.GetComponent<LobbyItemUI>();
        obj.SetPlayer(player);

        LeanTween.scale(lobbyObject, new Vector3(1, 1, 1), 0.75f).setEaseInSine();

        ListItems.Add(player, obj);


        Debug.Log("Lobby Updated with New Player::"+player.name);

		waitTxt.gameObject.SetActive(false);
		//readyUp.interactable = true;
		customizeButton.interactable = true;
		leaveButton.interactable = true;
        playBtn.GetComponent<Button>().interactable = true;

        RoomPlayer.gpMode = 0;

        string gmode = "1v1";
        if (RoomPlayer.gpMode == 0)
        {
            gmode = "1v1";
        }
        if (RoomPlayer.gpMode == 1)
        {
            gmode = "1v2";
        }
        if (RoomPlayer.gpMode == 2)
        {
            gmode = "2v2";
        }
        modeTxt.text = "GameMode:" + gmode;
        gocTxt.text = "Type:" + (RoomPlayer.isGOC == 1 ? "Game Of Chance" : "Standard");
        timerTxt.text = RoomPlayer.actionTime > 0 ? "Timer:" + RoomPlayer.actionTime.ToString() : "Timer:None";
        roomCodeTxt.text = (GameMode)FindObjectOfType<UIManager>().currentMultiplayerGameMode == GameMode.Host ?
            ServerInfo.LobbyName : ClientInfo.LobbyName;

        civsSelectObj.SetActive(true);
        playBtn.SetActive(true);

    }

    private void RemovePlayer(RoomPlayer player)
	{
        if (!ListItems.ContainsKey(player))
            return;

        var obj = ListItems[player];
        if (obj != null)
        {
            Destroy(obj.gameObject);
            ListItems.Remove(player);
        }
    }

	public void OnDestruction()
	{
	}

	private void ReadyUpListener()
	{
		var local = RoomPlayer.Local;
        if (local && local.Object && local.Object.IsValid)
        {
            local.RPC_ChangeReadyState(!local.IsReady);

            if (local.IsReady)
            {
                Debug.Log("I am ready, resume Timer");
                readyUp.interactable = true;
                readyUp.GetComponent<TimerQM>().ResumeTimer();
                DisableCivs();
            }
            else
            {
                Debug.Log("I am not ready, pause Timer");

                readyUp.interactable = true;
                readyUp.GetComponent<TimerQM>().PauseTimer();
                EnableCivs();
            }
        }
	}

    public void EnableCivs()
    {
        int count = civsContent.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform child = civsContent.transform.GetChild(i);
            child.GetComponent<Button>().interactable = true;
        }
        
    }

    public void DisableCivs()
    {
        int count = civsContent.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform child = civsContent.transform.GetChild(i);
            child.GetComponent<Button>().interactable = false;
        }
    }

    public void OnPlay()
    {
        playBtn.SetActive(false);
        timerBtn.SetActive(true);
        RoomPlayer.Local.IsReady = false;

        EnableReadyButton();

    }

    private Button currentBtn;
    public void SetSelection(Button button)
    {
        if(currentBtn != null) 
        {
            currentBtn.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        }
        button.GetComponent<Image>().color = new Color(0,255,0,255);
        currentBtn = button;
    }

    public void EnableReadyButton()
    {
       ReadyUpListener();
    }
   
    private void EnsureAllPlayersReady(RoomPlayer lobbyPlayer)
	{
        //if (!RoomPlayer.Local.IsLeader) 
        //	return;


        if (IsAllReady())
		{
            SendCiv();
            UIManager.Instance.ShowLoading();



            //load multiplayer game scene here
            //SoundManager.Instance.PlayEffectFor(GameConstants.SoundEffect.Button_Click);
            //SceneManager.LoadScene("GameMultiplayer", LoadSceneMode.Single);

            UIManager.Instance.OnPlayMultiplayerGameSelected();

        }
	}

    /*IEnumerator StartGame()
    {
        yield return new WaitForSeconds(3);

        //load multiplayer game scene here
        SoundManager.Instance.PlayEffectFor(GameConstants.SoundEffect.Button_Click);
        SceneManager.LoadScene("GameMultiplayer", LoadSceneMode.Single);


        yield return null;
    }*/

    private static bool IsAllReady() => RoomPlayer.GetPlayers().Count== PlayerPrefs.GetInt("S_MaxUsers", 2) && RoomPlayer.GetPlayers().All(player => player.IsReady);
}