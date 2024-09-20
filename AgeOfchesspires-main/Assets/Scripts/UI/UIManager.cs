
using Fusion;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject SettingsPanel;
    [SerializeField]
    private GameObject mainPanel;
    [SerializeField]
    private GameObject AlertPanel;
    [SerializeField]
    private GameObject ModesPanel;
    [SerializeField]
    private GameObject LobbyPanel;
    [SerializeField]
    private GameObject GOPanel;
    [SerializeField]
    private GameObject SetupGamePanel;
    [SerializeField]
    private GameObject GameRoomPanel;
    [SerializeField]
    private GameObject DiceRollingPanel;
    [SerializeField]
    private GameObject ArmySelectionPanel;
    [SerializeField]
    private GameObject CharSelectionPanel;
    [SerializeField]
    private GameObject ButtonPanel;
    [SerializeField]
    private GameObject LoginPanel;
    [SerializeField]
    private GameObject WaitPanel;
    [SerializeField]
    private GameObject RegistrationPanel;
    [SerializeField]
    private GameObject ProfilePanel;
    [SerializeField]
    private GameObject ResetPanel;
    [SerializeField]
    private GameObject LoadingPanel;
    [SerializeField]
    private GameObject Dice6Button;
    [SerializeField]
    private GameObject Dice8Button;
    [SerializeField]
    private Sprite Dice6BlackImage;
    [SerializeField]
    private Sprite Dice8BlackImage;
    [SerializeField]
    private Sprite Dice6WhiteImage;
    [SerializeField]
    private Sprite Dice8WhiteImage;
    public DisconnectUI _disconnectUI;

    [SerializeField]
    private GameObject gameBoard;
   
    private GameObject playButton;
   
    private GameObject settingButton;
   
    private GameObject infoButton;

    [SerializeField]
    private TMP_Text roomNameLbl;
    [SerializeField]
    private TMP_InputField roomName;

    [SerializeField]
    private TMP_InputField roomCode;
    [SerializeField]
    private GameObject roomCodeParent;

    [SerializeField]
    private TMP_InputField userName;

    [SerializeField]
    private TMP_InputField password;

    [SerializeField]
    private TMP_Text setupScreenTitle;

    [SerializeField]
    private GameObject roomAccess;

    [SerializeField]
    private Toggle gameMode1v1;

    [SerializeField]
    private Toggle gameMode1v2;

    [SerializeField]
    private Toggle gameMode2v2;


    [SerializeField]
    private Toggle actionTimeNone;

    [SerializeField]
    private Toggle actionTime30;

    [SerializeField]
    private Toggle actionTime60;

    [SerializeField]
    private Toggle actionTime90;

    [SerializeField]
    private Toggle gameTypeStand;

    [SerializeField]
    private Toggle gameTypeChance;

    [SerializeField]
    private Toggle lobbyTypePrivate;

    [SerializeField]
    private Toggle lobbyTypePublic;

    [SerializeField]
    private GameObject multiplayerConfirmBtn;

    [SerializeField]
    private GameObject aiGameConfirmBtn;

    [SerializeField]
    private GameObject matchConfirmBtn;
    [SerializeField]
    private GameObject validNickTxt;


    public string roomNameVal;
    public string roomCodeVal;
    public string userNameVal;

    public int currentActionTime;

    public bool autoMatchMade = true;

    public static UIManager Instance { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        //defaults
        PlayerPrefs.SetString("localUserName", "");
        PlayerPrefs.Save();
        /* PlayerPrefs.SetInt("ActionTime", -1);
         PlayerPrefs.Save();*/
        PlayerPrefs.SetInt("gameplayMode", 0);
        PlayerPrefs.Save();
        /*PlayerPrefs.SetInt("isChance", 0);
        PlayerPrefs.Save();*/
        PlayerPrefs.SetInt("privateLobby", 0);
        PlayerPrefs.Save();
        
        /*PlayerPrefs.SetInt("playerWonToss", 0);
        PlayerPrefs.Save();*/

        #if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
#else
            Debug.unityLogger.logEnabled = false;
#endif

        OnMode1v1Selected();
    }

    public int currentMultiplayerGameMode = 6;//auto host or client

    public GameObject homeUI1;
    public GameObject homeUI2;
    public GameObject homeUI3;

    public GameObject lobbyUI1;
    public GameObject lobbyUI3;
    public GameObject lobbyUI4;

    public void OnHomeUI()
    {
        homeUI1.SetActive(true);
        homeUI2.GetComponent<Button>().interactable = false;
        homeUI3.SetActive(true);


        lobbyUI1.SetActive(false); 
        lobbyUI3.GetComponent<Button>().interactable = true;
        lobbyUI4.SetActive(false);

    }
    public void OnLobbyUI()
    {
        homeUI1.SetActive(false);
        homeUI2.GetComponent<Button>().interactable = true;
        homeUI3.SetActive(false);
        lobbyUI1.SetActive(true);
        lobbyUI3.GetComponent<Button>().interactable = false;
        lobbyUI4.SetActive(true);

    }

    public void SetCurrentGameMode(int gameMode)
    {
        currentMultiplayerGameMode = gameMode;
    }

    public void OnRoomValueChanged()
    {
        roomNameVal = roomName.text;
        Debug.Log("RoomName Given="+roomNameVal+",GameMode:"+currentMultiplayerGameMode);

        

        if (currentMultiplayerGameMode == 5)
        {
            ClientInfo.LobbyName = roomNameVal;
        }
        else if (currentMultiplayerGameMode == 4)
        {
            ServerInfo.LobbyName = roomNameVal;
        }
    }

    public void OnRoomCodeValueChange()
    {
        roomCodeVal = roomCode.text;
    }

    public void OnUserNameValueChanged()
    {
        validNickTxt.SetActive(false);
        userNameVal = userName.text;
    }

    public void ShowLoading()
    {
        if(LoadingPanel != null)
        {
            LoadingPanel.SetActive(true);
        }
    }

    public void HideLoading()
    {
        if (LoadingPanel != null)
        {
            LoadingPanel.SetActive(false);
        }
    }

    public int IsAIMode()
    {
        int isAI = PlayerPrefs.GetInt("isAIMode", 1);
        return isAI;
    }


    public void SetIsAIMode(int isAI)
    {
        PlayerPrefs.SetInt("isAIMode", isAI);
        PlayerPrefs.Save();
    }

    void OnEnable()
    {
        playButton = GameObject.Find("PlayBtn");
        settingButton = GameObject.Find("SettingBtn");
        infoButton = GameObject.Find("InfoBtn");

       

        if (playButton)
            LeanTween.scale(playButton, new Vector3(1, 1, 1), 1.5f).setEaseInOutBounce();

        if (settingButton)
            LeanTween.scale(settingButton, new Vector3(1, 1, 1), 1.0f).setEaseInOutBounce();

        if (infoButton)
            LeanTween.scale(infoButton, new Vector3(1, 1, 1), 1.0f).setEaseInOutBounce();

        

    }

    public void ShowWaitPanel()
    {
        
            WaitPanel.SetActive(true);
    }

    public void HideWaitPanel()
    {
            WaitPanel.SetActive(false);
    }

    public void OnPlayButton()
    {
        SoundManager.Instance.PlayEffectFor(GameConstants.SoundEffect.Button_Click);
        //SceneManager.LoadScene("Game", LoadSceneMode.Single);
        
        SetIsAIMode(0);

        ResetAllUI();
        ModesPanel.SetActive(true);
    }

    public void OnSetupGame()
    {
        ResetAllUI();
        SetupGamePanel.SetActive(true);
    }

    public void OnGameRoomPanel()
    {
        autoMatchMade = false;
        ResetAllUI();
        GameRoomPanel.SetActive(true);
    }

    public void OnMode1v1Selected()
    {
        PlayerPrefs.SetInt("gameplayMode", 0);
        PlayerPrefs.Save();

        PlayerPrefs.SetInt("S_MaxUsers", 2);
        PlayerPrefs.Save();

        ResetAllUI();
        int loggedIn = 1;// PlayerPrefs.GetInt("loggedIn");
        if (loggedIn == 0)
        {

            Debug.Log("Player is not logged in");
            LoginPanel.SetActive(true);
        }
        else
        {
            Debug.Log("Player is logged in");

            if(LobbyPanel)
            {
                LobbyPanel.SetActive(true);
            }


            GameLauncher multGameLauncher = FindObjectOfType<GameLauncher>();
            if (!multGameLauncher.sessionJoined)
            {
                multGameLauncher.JoinSessionLobby();
            }
        }


        //gameMode1v1.isOn = true;
       
    }
 

    public int _myCivilization = 0;

    public void SetCurrentCivilization(int civ)
    {
        Debug.Log("My Current Civ::"+civ);
        _myCivilization = civ;

        Debug.Log("My Current Civ Set As::" + _myCivilization);

        PlayerPrefs.SetInt("myCivIcon", civ);
        PlayerPrefs.Save();
    }

    
    public void OnLoginSuccess(string username)
    {
        ResetAllUI();
        HideRegistrationPanel();
        LobbyPanel.SetActive(true);
        Debug.Log("UserName:" + username);
        PlayerPrefs.SetString("localUserName", username);
        PlayerPrefs.Save();

        Debug.Log("Seeting Player logged in now");
        PlayerPrefs.SetInt("loggedIn", 1);
        PlayerPrefs.Save();

        FindObjectOfType<GameLauncher>().JoinSessionLobby();
    }

    public void OnSetupMultiplayerSelected(bool isCreate =true)
    {
        SoundManager.Instance.PlayEffectFor(GameConstants.SoundEffect.Button_Click);
        ResetAllUI();
        SetupGamePanel.SetActive(true);

        ClientInfo.LobbyName = "";
        ServerInfo.LobbyName = "";

        roomNameLbl.gameObject.SetActive(true);
        roomName.gameObject.SetActive(true);

        if (isCreate)
        {
            setupScreenTitle.text = "Create Game";
            multiplayerConfirmBtn.SetActive(true);
        }
        else
        {
            setupScreenTitle.text = "Select Game";
            multiplayerConfirmBtn.SetActive(false);
            matchConfirmBtn.SetActive(true);//join btns
            
        }
        
        aiGameConfirmBtn.SetActive(false);
    }

    public void OnSetupAIGameSelected()
    {
        SetIsAIMode(1);
        SoundManager.Instance.PlayEffectFor(GameConstants.SoundEffect.Button_Click);
        ResetAllUI();
        
        SetupGamePanel.SetActive(true);

        roomNameLbl.gameObject.SetActive(false);
        roomName.gameObject.SetActive(false);
        roomAccess.SetActive(false);

        multiplayerConfirmBtn.SetActive(false);
        aiGameConfirmBtn.SetActive(true);
    }

    [SerializeField]
    private GameObject mainCivPanel;
    [SerializeField]
    private GameObject quickJoinBtn;
    [SerializeField]
    private GameObject civSelectionPanel;

    public void showCivSelectionPanel()
    {
        mainCivPanel.SetActive(false);
        quickJoinBtn.SetActive(false);
        civSelectionPanel.SetActive(true);
    }

    public void hideCivSelectionPanel()
    {
        mainCivPanel.SetActive(true);
        quickJoinBtn.SetActive(true);
        civSelectionPanel.SetActive(false);
    }

    public void OnSetupGameConfirm()
    {
        //if (isAIMode)
        //{
        FindObjectOfType<GameLauncher>().LeaveSession();

        OnPlayAIGameSelected();
        /*}
        else
        {
            OnPlayMultiplayerGameSelected();
        }*/
    }



    IEnumerator StartGameCort()
    {
        yield return new WaitForSeconds(3);

        //load multiplayer game scene here
        SoundManager.Instance.PlayEffectFor(GameConstants.SoundEffect.Button_Click);
        SceneManager.LoadScene("GameMultiplayer", LoadSceneMode.Single);


        yield return null;
    }

    public void OnPlayAIGameSelected()
    {
        SoundManager.Instance.PlayEffectFor(GameConstants.SoundEffect.Button_Click);
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void OnPlayMultiplayerGameSelected()
    {
        StartCoroutine(StartGameCort());
    }

    public void OnSceneFullReload()
    {
        PlayerPrefs.SetInt("isFirstRun", 0);
        PlayerPrefs.Save();
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        Resources.UnloadUnusedAssets();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void OnSettingsButton(int option = 0)
    {
        SoundManager.Instance.PlayEffectFor(GameConstants.SoundEffect.Button_Click);

        //ResetAllUI();
        SettingsPanel.SetActive(true);
        SettingsPanel.GetComponent<SettingsPanel>().GetSettingsPopup(option).transform.localScale
           = new Vector3(1,1,1);
        //LeanTween.scale(SettingsPanel.GetComponent<SettingsPanel>().GetSettingsPopup(), new Vector3(1, 1, 1), 1.5f).setEaseInOutBounce();
    }
  

 

    public void OnButtonScreen() {
        SoundManager.Instance.PlayEffectFor(GameConstants.SoundEffect.Button_Click);

        
        ButtonPanel.SetActive(true);

        LeanTween.moveLocal(ButtonPanel.transform.Find("root").gameObject, new UnityEngine.Vector3(655f, 0, 0), 1f).setEaseInOutBounce().setOnComplete(()=>PauseGame());
        

    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void CloseButtonScreen()
    {
        SoundManager.Instance.PlayEffectFor(GameConstants.SoundEffect.Button_Click);

        LeanTween.moveLocal(ButtonPanel.transform.Find("root").gameObject, new Vector3(5,0,0), 1f).setEaseInOutBounce();

        ButtonPanel.SetActive(false);

        Time.timeScale = 1f;

    }

    public void ResetAllUI() {

        if (mainPanel)
            mainPanel.SetActive(false);
        if (SettingsPanel)
            SettingsPanel.SetActive(false);
        if (AlertPanel) 
            AlertPanel.SetActive(false);
        if (ModesPanel) 
            ModesPanel.SetActive(false);
        if (SetupGamePanel)
            SetupGamePanel.SetActive(false);
        if (LobbyPanel)
            LobbyPanel.SetActive(false);
        if (GameRoomPanel)
            GameRoomPanel.SetActive(false);
        if (LoginPanel)
            LoginPanel.SetActive(false);
        if (_disconnectUI)
            _disconnectUI.gameObject.SetActive(false);
    }

    public void CloseSettingsPanel()
    {
        SettingsPanel.SetActive(false);
    }

    public void
       ShowGameOver(int gameMode, string wonTitle, string playerScores, string opponentScores)
    {

        GOPanel goPanel = GOPanel.GetComponent<GOPanel>();
        goPanel.SetWonTitle(wonTitle);

        if(gameMode == 1)
        {
            goPanel.SetPlayerScores("You:" + playerScores);
            goPanel.SetOpponentScores("Team2 Scores:" + opponentScores);
            goPanel.UndoBtn.SetActive(false);
            goPanel.MovesPanelGO.SetActive(false);

        }



        /*else if(gameMode == 2)
        {
            goPanel.SetPlayerScores("Your Team Scores:" + playerScores);
            goPanel.SetOpponentScores("Team2 Scores:" + opponentScores);
            goPanel.UndoBtn.SetActive(false);
            goPanel.MovesPanelGO.SetActive(false);

        }
        else
        {
            goPanel.SetPlayerScores("You:" + playerScores);
            string oppText = "Opponent:";//War MAchine
            goPanel.UndoBtn.SetActive(true);

            if (IsAIMode()==0)
            {
                oppText = "Opponent:";
                goPanel.UndoBtn.SetActive(false);
            }

            goPanel.SetOpponentScores(oppText + opponentScores);
            goPanel.MovesPanelGO.SetActive(true);

        }*/

        goPanel.UpdateUI();
        GOPanel.SetActive(true);

    }

    public void showModelsPanel()
    {
        mainPanel.SetActive(false);
        ModesPanel.SetActive(true);
    }

    public void HideGameOver()
    {
        GOPanel.SetActive(false);
    }

    public void 
        ShowAlert(string msg, GameConstants.AlertType alertType, int tag)
    {
        Alert alertPanel = AlertPanel.GetComponent<Alert>();
        alertPanel.SetMessage(msg);
        alertPanel.SetAlertTypeOrTag(alertType, tag);
        alertPanel.UpdateUI();
        AlertPanel.SetActive(true);
    }

    public void OnPlayAgain()
    {
        SceneManager.LoadScene("Game");
    }

    public void ShowDiceRollPanel()
    {
        if(!DiceRollingPanel)
        {
            DiceRollingPanel = GameObject.Find("DiceRollingPanel");
        }
        DiceRollingPanel.SetActive(true);
    }

    public void CloseDiceRollPanel()
    {
        if (!DiceRollingPanel)
        {
            DiceRollingPanel = GameObject.Find("DiceRollingPanel");
        }
        DiceRollingPanel.SetActive(false);
    }

    public void ShowArmySelection()
    {
        ArmySelectionPanel.SetActive(true);
    }
    public void CloseArmySelection()
    {
        ArmySelectionPanel.SetActive(false);
    }
    public void ShowCharSelection()
    {
        if(IsAIMode() == 0)
        {
            HideWaitPanel();
            CloseDiceRollPanel();

        }

        CharSelectionPanel.SetActive(true);
    }
    public void CloseCharSelection()
    {
        CharSelectionPanel.SetActive(false);
    }

    public void ShowLoginPanel()
    {
        ResetAllUI();
        LoginPanel.SetActive(true);
        userName.text = "";
    }
    public void UpdateDiceButtons(int option) {
        if (option == 0)
        {
            Dice6Button.GetComponent<Image>().sprite = Dice6BlackImage;
            Dice8Button.GetComponent<Image>().sprite = Dice8BlackImage;
        }
        else
        {
            Dice6Button.GetComponent<Image>().sprite = Dice6WhiteImage;
            Dice8Button.GetComponent<Image>().sprite = Dice8WhiteImage;
        }
    }



    public void OnDice6Clicked() 
    {
        SoundManager.Instance.PlayEffectFor(GameConstants.SoundEffect.Dice_Click);
    }

    public void OnDice8Clicked()
    {
        SoundManager.Instance.PlayEffectFor(GameConstants.SoundEffect.Dice_Click);

    }

   

    public void SideGameMenuButtonCallbacks(int buttonId) {

        CloseButtonScreen();

        switch((GameConstants.GameSidePanelBtns)buttonId)
        {
            case GameConstants.GameSidePanelBtns.Home:

                
                OnBackButtonClick(GameConstants.AlertButton.Back_Game_tag);

                break; 
            
            case GameConstants.GameSidePanelBtns.Settings:
                OnSettingsButton();
                break;  
            
            
            case GameConstants.GameSidePanelBtns.Resume:

                ResumeGame();
                CloseButtonScreen();
                

                break;
            case GameConstants.GameSidePanelBtns.QuitGame:
                OnBackButtonClick(GameConstants.AlertButton.Back_Game_tag);
                break;
            default:
                break;
        }
    }

    public void OnBackButtonClick(GameConstants.AlertButton alertButtonType ) {
        SoundManager.Instance.PlayEffectFor(GameConstants.SoundEffect.Button_Click);

        switch (alertButtonType)
        {
            case (int)GameConstants.AlertButton.Back_Game_tag:
                ShowAlert("Are you sure to quit?", GameConstants.AlertType.AlertType_OkAndCancel, 0);
                break;
            default: break;
        }
    }



    public void OnLobbyTypePrivate()
    {
        if (lobbyTypePrivate.isOn)
        {
            PlayerPrefs.SetInt("privateLobby", 1);
            PlayerPrefs.Save();
        }

        roomCodeParent.SetActive(true);
    }

    public void OnLobbyTypePublic()
    {
        if (lobbyTypePublic.isOn)
        {

            PlayerPrefs.SetInt("privateLobby", 0);
            PlayerPrefs.Save();
        }

        roomCodeParent.SetActive(false);
    }



    public void OnSetupRoomBackSelected()
    {
        ResetAllUI();
        SetIsAIMode (0);
        LobbyPanel.SetActive(true);

    }

    public void OnLobbyRoomBackSelected()
    {
        FindObjectOfType<GameLauncher>().LeaveSession();
        OnLobbyRoom();

    }

    public void OnLobbyRoom()
    {
        ResetAllUI();
        SetIsAIMode(0);

        if (ModesPanel)
            ModesPanel.SetActive(true);
        else if (LobbyPanel)
            LobbyPanel.SetActive(true);
    }

    public void OnPlayerRegistration()
    {
        RegistrationPanel.SetActive(true);
    }

    public void HideRegistrationPanel()
    {
        RegistrationPanel.SetActive(false);
    }



    void OnDestroy()
    {
        PlayerPrefs.SetInt("isFirstRun", 0);
        PlayerPrefs.Save();
    }

    public void InitGameUI()
    {
        DiceRollingPanel = GameObject.Find("DiceRollingPanel");
        gameBoard = GameObject.Find("GameBoard");
        GOPanel = GameObject.Find("GameOver");
        ArmySelectionPanel = GameObject.Find("ArmySelectionPanel");
        CharSelectionPanel = GameObject.Find("CharSelectionPanel");
        ButtonPanel = GameObject.Find("SideButtonsPanel");    
        Dice6Button = GameObject.Find("Dice6Button");
        Dice8Button = GameObject.Find("Dice8Button");
        SettingsPanel = GameObject.Find("Settings");
        
        
    }

    private void OnApplicationQuit()
    {
        Debug.Log("MA::On App Quit!");
        PlayerPrefs.SetInt("loggedIn", 0);
        PlayerPrefs.Save();
    }

}
