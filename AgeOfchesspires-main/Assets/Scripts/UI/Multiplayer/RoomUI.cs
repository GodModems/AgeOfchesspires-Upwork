using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
	public GameObject roomEnteryPrefab;
	public Transform parent;

    [SerializeField]
    private GameObject noRoomLabel;
    [SerializeField]
    private GameObject refreshButton;
    [SerializeField]
    private GameObject refreshButtonR;
    [SerializeField]
    private GameObject createButton;
    [SerializeField]
    private GameObject playAiButton;
    [SerializeField]
    private GameObject quickJoinButton;
    [SerializeField]
    public Image dpImg;
    

    private GameLauncher gameLauncher;
    private UIManager uiManager;
    private int gamePlayMode = 0;

    [SerializeField]
    public Image myCivIcon;

    [SerializeField]
    private TMP_Text eloText;

    [SerializeField]
    private List<Sprite> civIconsList; 

    private void OnEnable()
    {
        //ClearExistingSessions();
        ShowRoomRefreshUI();
        noRoomLabel.GetComponent<TMP_Text>().text = "Loading...";
        //dpImg.sprite = FindObjectOfType<PlayfabManager>().GetSelectedDp();
        UpdateSessionUI();

    }

    private void Awake()
    {
		gameLauncher = FindObjectOfType<GameLauncher>();
        uiManager = FindObjectOfType<UIManager>();
        //ClearExistingSessions() ;
        noRoomLabel.GetComponent<TMP_Text>().text = "Loading...";

        
        //dpImg.sprite = FindObjectOfType<PlayfabManager>().GetSelectedDp();
    }

    

    private void Start()
    {
        //DisableButtons();
        uiManager.ShowLoading();
        noRoomLabel.GetComponent<TMP_Text>().text = "Loading...";
        //dpImg.sprite = FindObjectOfType<PlayfabManager>().GetSelectedDp();

        FindObjectOfType<GameLauncher>().LeaveSession(true);

        updateCivImage(PlayerPrefs.GetInt("myCivIcon",0));

        NetworkRunner networkRunner = gameLauncher.GetCurrentRunner();
        if (networkRunner != null)
        {
            if (!networkRunner.IsCloudReady)
            {
                createButton.GetComponent<Button>().interactable = false;
                quickJoinButton.GetComponent<Button>().interactable = false;
                noRoomLabel.GetComponent<TMP_Text>().text = "Network Time Out.";

            }
            else
            {
                createButton.GetComponent<Button>().interactable = true;
                quickJoinButton.GetComponent<Button>().interactable = true;
                noRoomLabel.GetComponent<TMP_Text>().text = "Loading...";

            }
        }

        EloRatingSystem eloRatingSystem = FindObjectOfType<EloRatingSystem>();
        int eloRating = eloRatingSystem.GetUpdatedPlayerElo();
        string eloBadg = eloRatingSystem.GetBadgeForRating(eloRating);
        eloText.text = eloRatingSystem.GetUpdatedPlayerElo().ToString()+"("+ eloBadg+")";
        
    }

    public void ShowRoomRefreshUI()
    {
        noRoomLabel.gameObject.SetActive(true);
        noRoomLabel.GetComponent<TMP_Text>().text = "No Room Avaialble.";
        //refreshButton.SetActive(true);

    }

    public void HideRoomRefreshUI()
    {
        noRoomLabel.SetActive(false);
        //refreshButton.SetActive(false);
    }

    public void DisableButtons()
    {
        refreshButtonR.GetComponent<Button>().interactable = false;
        createButton.GetComponent<Button>().interactable = false;
        playAiButton.GetComponent<Button>().interactable = false;
        quickJoinButton.GetComponent<Button>().interactable = false;

    }

    public void EnableButtons()
    {
        refreshButtonR.GetComponent<Button>().interactable = true;
        createButton.GetComponent<Button>().interactable = true;
        playAiButton.GetComponent<Button>().interactable = true;
        quickJoinButton.GetComponent<Button>().interactable = true;
    }

    public void ClearExistingSessions()
	{
        foreach (Transform transform in parent)
        {
            Destroy(transform.gameObject);
        }
    }

    public void RefreshSession()
    {
        gameLauncher.LeaveSession(true);
    }

    public void UpdateSessionUI()
	{
		List<SessionInfo> sessions = gameLauncher.GetSessions();
        ClearExistingSessions();
        if(sessions.Count>0)
        {
            HideRoomRefreshUI();
        }
        else
        {
            ShowRoomRefreshUI();
        }

        foreach (SessionInfo sessionInfo in sessions)
        {
			CreateRoomEntry(sessionInfo);
        }

        EnableButtons();
        uiManager.HideLoading();

    }

    public void updateCivImage(int civ)
    {
        myCivIcon.sprite = civIconsList[civ];
    }

    public void CreateRoomEntry(SessionInfo sessionInfo)
    {
        if (sessionInfo.IsVisible)
        {
            var customProps = sessionInfo.Properties;

            int gpMode = customProps["gameplayMode"];
            gamePlayMode = PlayerPrefs.GetInt("gameplayMode", 0);

            Debug.Log("My selected gameplay mode:" + gamePlayMode);
            Debug.Log("Creating room entry with gameplay mode:"+gpMode);

            if (gpMode == gamePlayMode)
            {
                int act = -1;// customProps["actionTime"];
                if (act == -1) { act = 0; }
                int goc = 0;// customProps["isGOC"];
                bool isPriv = customProps["isPriv"] == 1;

                string rName = "";// customProps["roomName"];
                
                GameObject roomEntryOb = Instantiate(roomEnteryPrefab, parent);
                RoomEntry roomEntry = roomEntryOb.GetComponent<RoomEntry>();

                LeanTween.scale(roomEntryOb, new Vector3(1, 1, 1), 0.5f).setEaseInSine();


                string roomCode = "";
                if (isPriv)
                {
                    roomCode = customProps["RoomCode"];
                    Debug.Log("Creating Room Entry With Room Code::" + roomCode);
                }

                roomEntry.SetRoomEntry(rName, sessionInfo.Name, sessionInfo.PlayerCount + "/" + sessionInfo.MaxPlayers, isPriv, act, goc, gpMode, roomCode);

                if (!sessionInfo.IsOpen || sessionInfo.PlayerCount >= sessionInfo.MaxPlayers)
                {
                    roomEntry.DisableJoinBtn();
                }
                else
                {
                    roomEntry.EnableJoinBtn();
                }
            }
        }
    }
}