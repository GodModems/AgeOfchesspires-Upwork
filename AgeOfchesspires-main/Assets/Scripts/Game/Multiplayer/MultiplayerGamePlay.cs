using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameConstants;
using static RoomPlayer;

public enum MultiplayerPlayState { USER_MOVE, OPP_MOVE, WAIT_DICE, USER_WAIT_PROMOTE, OPP_WAIT_PROMOTE, OPP_FRND_MOVE, OPP_FRND_WAIT_PROMOTE };

public class MultiplayerGamePlay : NetworkBehaviour
{
    //const int AVATAR_COM_X = -326;
    //const int AVATAR_COM_Y = 370;
    //const int AVATAR_PLAYER_X = -326;
    //const int AVATAR_PLAYER_Y = -390;

    public MultiplayerGameState _gameState = MultiplayerGameState.ONGOING ;
    public MultiplayerPlayState _state;

    public TextMeshProUGUI rescueTxt;
    public TextMeshProUGUI checkTxt;

    //public TextMeshProUGUI playerPoints;
    //public TextMeshProUGUI oppPoints;
    //public TextMeshProUGUI opp2Points;
    //public TextMeshProUGUI opp3PlayerTeamPoints;

    public Transform moveContents;
    public Transform moveContentsGO;
    public GameObject moveContentPlayerIcon;
    public GameObject moveContentComputerIcon;
    public GameObject moveContentPlayerIconGO;
    public GameObject moveContentComputerIconGO;
    public GameObject moveEnteryPrefab;

    public GameObject actualBoardWhite;

    public Sprite board1;
    public Sprite actualBoardWhiteSpr;
    public Sprite actualBoardBlack;
    public Sprite mainBoardRotated;

    public Sprite board1Orig;
    public Sprite actualBoardWhiteOrig;
    public Sprite actualBoardBlackOrig;
    public Sprite mainBoardRotatedOrig;



    public bool isPlayerStartFirst = false;
    public Transform PlayerOneTrans;
    public Transform PlayerTwoTrans;
    public Transform OneV2Transform;
    public Transform TwoV2Transform;

    [SerializeField] List<Button> oppCharacters = new List<Button>();
    [SerializeField] List<Button> userCharacters = new List<Button>();
    [SerializeField] List<Button> allBoardRect = new List<Button>();
    [SerializeField] List<Button> userChips = new List<Button>();
    [SerializeField] List<Button> oppChips = new List<Button>();

    [SerializeField] List<Button> powByUSer = new List<Button>();
    [SerializeField] List<Button> powByOpp = new List<Button>();
    [SerializeField] List<Button> killedByUSer = new List<Button>();
    [SerializeField] List<Button> killedByOpp = new List<Button>();

    public Transform PlayerPowTransform;
    public Transform PlayerDieTransform;
    public Transform OppPowTransform;
    public Transform OppDieTransform;


    public CharHandler charHandler;
    public MultiplayerPlayerHandler playerHandler;
    public ButtonsHandler buttonsHandler;
    private bool isUserPlayerBlack;
    private Button buttSelected;
    private List<Char_Point> posSelectionToMove= new List<Char_Point>();
    bool just_promote_userBot = false;

    bool just_promote_comp = false;
    bool isInCheck = false;
    private HashSet<colour> colour_rescue1 = new HashSet<colour>();

    private Vector2 winSize;
    private float aspectRatio;

    // Start is called before the first frame update

    public GameObject Undobtn;
    public GameObject MovesPanel;
    public GameObject GameOverImg;
    public GameObject ShowResultBtn;

    public GameObject titleObj;
    public GameObject titleObj1;

    public TMP_Text goReasonTxt;

    //[SerializeField] GameObject playerTurnIndc;
    //[SerializeField] GameObject oppTurnIndc;
    //[SerializeField] GameObject opp2TurnIndc;
    //[SerializeField] GameObject opp3PlayerTeamTurnIndc;

    //[SerializeField] GameObject playerTimeIndc;
    //[SerializeField] GameObject oppTimeIndc;
    //[SerializeField] GameObject opp2TimeIndc;
    //[SerializeField] GameObject opp3PlayerTeamTimeIndc;

    [SerializeField] Image skinImage;


    int gameMode = 0;//1v1 (player VS bot)
    int isGameOfChance;
    int actionTime = 0;//Action time None
    bool isBottomPlace = true;
    bool isRotated = false;

    public int currMyCiv = -1;
    public int currOppCiv = -1;
    public int myPawnCount = -1;
    public int myKnightCount = -1;

    [SerializeField]
    private Image MovesDpPlayer1;
    [SerializeField]
    private Image MovesDpOpp1;
    [SerializeField]
    private Image MovesGODpPlayer1;
    [SerializeField]
    private Image MovesGODpOpp1;

    [SerializeField]
    private GameObject passTurnBtn;
    [SerializeField]
    private TMP_Text turnText;

    //Civ Names
    [SerializeField]
    private TMP_Text MyCivName;
    [SerializeField]
    private TMP_Text OppCivName;

    [SerializeField]
    private Sprite newBoardNormal;
    [SerializeField]
    private Sprite newBoardNormalRot;

    public void OnArmySelection(int army)
    {
        SoundManager.Instance.PlayEffectFor(GameConstants.SoundEffect.Button_Click);
        GameObject.FindWithTag("GameController").GetComponent<GameController>().SetPlayerArmySelected(army);

        if (UIManager.Instance.IsAIMode() == 0)
        {
            uint localId = RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw;
            RoomPlayer.Local.RPC_ArmySetup(localId, army);
        }
    }

   

    public void SetupGameplay()
    {

        SetupPrefsAndCleanup();



        if (RoomPlayer.Local.IsLeader)
        {
            OnArmySelection(1);
            PlayerPrefs.SetInt("switchChar", 1);
            PlayerPrefs.Save();

            isPlayerStartFirst = true;

            RoomPlayer.Local.GetTimer().ResetTimeRemaining();
            RoomPlayer.Local.GetTimer().StartTimer();

            //Need to pause other player timer
            RoomPlayer.GetPlayerAt(1).GetTimer().ResetTimeRemaining();
            RoomPlayer.GetPlayerAt(1).GetTimer().PauseTimer();

            actualBoardWhite.GetComponent<Image>().sprite = newBoardNormal;

        }
        else
        {
            RoomPlayer.Local.GetTimer().PauseTimer();
            RoomPlayer.Local.GetTimer().ResetTimeRemaining();
           

            RoomPlayer.GetPlayerAt(0).GetTimer().ResetTimeRemaining();
            RoomPlayer.GetPlayerAt(0).GetTimer().StartTimer();

            actualBoardWhite.GetComponent<Image>().sprite = newBoardNormalRot;

        }

        Image fullboard = GetComponent<Image>();

        if(!RoomPlayer.Local.IsLeader)
        {
            uint leaderID = RoomPlayer.GetPlayerAt(0).GetComponent<NetworkObject>().Id.Raw;
            if(!isMyTeam(leaderID))
            {
                fullboard.sprite = mainBoardRotated;
                isRotated = true;
            }   
            

        }

        float actualBw = 864;
        
       

        actionTime = 0;

        int gameMode = 0; // RoomPlayer.gpMode; //PlayerPrefs.GetInt("gameMode", 0);
        if (gameMode == 1)
        {
            MovesPanel.SetActive(true);
           
        }
        else
        if(gameMode == 2)
        {
            MovesPanel.SetActive(false);
           
        }

        ResetTurnIndics();
        ResetTimeIndics();

        if (gameMode == 0)
        {
            if (isPlayerStartFirst)
            {
                Debug.Log("I won the toss!!!222222");
                _state = MultiplayerPlayState.USER_MOVE;
                //playerHandler.turnStateSeqnce.Push(MultiplayerTurnState.PLAYER_TURN);



                if (actionTime == 0)
                {
                    RoomPlayer.Local.ActivateTurnIndic();
                }
                else
                {
                    RoomPlayer.Local.ActivateTimeIndic();

                }


            }
            else
            {
                Debug.Log("I lost the toss!!!2222222");

                _state = MultiplayerPlayState.OPP_MOVE;//
                //playerHandler.turnStateSeqnce.Push(MultiplayerTurnState.OPP1_TURN);


                if (actionTime == 0)
                {
                    RoomPlayer.Local.DeactivateTurnIndic();

                }
                else
                {
                    RoomPlayer.Local.DeactivateTimeIndic();
                }
            }
        }
        else
        {
            if (RoomPlayer.Local.IsLeader)
            {
                Debug.Log("I will start as a room leader!!!");
                _state = MultiplayerPlayState.USER_MOVE;
                //playerHandler.turnStateSeqnce.Push(MultiplayerTurnState.PLAYER_TURN);


                if (actionTime == 0)
                {
                    RoomPlayer.Local.ActivateTurnIndic();
                }
                else
                {
                    RoomPlayer.Local.ActivateTimeIndic();

                }


            }
            else
            {
                
                _state = MultiplayerPlayState.OPP_MOVE;//


                if (actionTime == 0)
                {
                    RoomPlayer.Local.DeactivateTurnIndic();

                }
                else
                {
                    RoomPlayer.Local.DeactivateTimeIndic();
                }
            }
        }

        int armySelected = GameObject.FindWithTag("GameController").GetComponent<GameController>()._playerSelectedArmy;
        Debug.Log("Army Selected::" + armySelected);
        isUserPlayerBlack = (armySelected == 0 ? true : false);

        //I will revert colour of the board here as we need to sow same board as selected
        //by host
        bool isBoardColorBlack = isUserPlayerBlack;
        if (!RoomPlayer.Local.IsLeader)
        {
            isBoardColorBlack = !isUserPlayerBlack;
        }
        ////////////////////////

        if (isBoardColorBlack)
        {
            actualBoardWhite.GetComponent<Image>().sprite = actualBoardBlack;
            Debug.Log("Army Selected::" + "Black");
        }


        titleObj.SetActive(true);
        titleObj1.SetActive(false);
        

        aspectRatio = charHandler.aspectRatiio;

        //playerHandler.initChips();

        if (isUserPlayerBlack)
        {
            Debug.Log("My color is black");
            
        }
        else
        {
            Debug.Log("My color is white");
            
        }

            playerHandler.initBoardData(isUserPlayerBlack);

        
        
        
        GameObject pActualBoard = actualBoardWhite;
        allBoardRect = buttonsHandler.createTilesOnLayers(pActualBoard);

        isBottomPlace = true;//its always true because every player is at bottom by default
        
        
       
        

        /* foreach (var p in allBoardRect)
         {
             //p.onClick.AddListener(() => { HandleClickToTheBoard(p); });due to drag drop
         }*/

         userCharacters = charHandler.PutPlayersToBoard(allBoardRect, playerHandler.getUserPlayers(), isUserPlayerBlack, true);
        
        //put chars as default if it is not game of chance
        ///*if (isGameOfChance == 0)
        //{
            oppCharacters = charHandler.PutPlayersToBoard(allBoardRect, playerHandler.getOppPlayers(), !isUserPlayerBlack, false);
        //}*/
         
       
        /*foreach (var p in userCharacters)
        {
            //p.onClick.AddListener(() => { handleClickToUserPlayers(p); });due to drag drop
        }*/


        /*foreach (var p in oppCharacters)
        {
            //p.onClick.AddListener(() => { handleClickToOpponent(p); });due to drag drop
        }*/



        
        buttonsHandler.SetEnableDice(0);


        
        
        _gameState = MultiplayerGameState.ONGOING;


        //transform.localScale = new Vector2(transform.localScale.x - 0.45f, transform.localScale.y - 0.45f);

        //in case of game of chance only
        if (isGameOfChance == 1)
        {
            RoomPlayer.PlayerGOCSet += EnsureAllPlayersGOCReady;


            var local = RoomPlayer.Local;
            if (local && local.Object && local.Object.IsValid)
            {
                local.RPC_ChangeGOCSetup(!local.IsGOCSetup);
            }
        }

        initMultiplayer();

       ;
    }

    public List<Button> GetUserCharBtns()
    {
        return userCharacters;
    }
    public List<Button> GetOppCharBtns()
    {
        return oppCharacters;
    }

    public void UpdateDp()
    {
        moveContentPlayerIcon.GetComponent<Image>().sprite
            = RoomPlayer.GetPlayerAt(0).playerDp.sprite;
        moveContentComputerIcon.GetComponent<Image>().sprite
            = RoomPlayer.GetPlayerAt(1).playerDp.sprite;
        moveContentPlayerIconGO.GetComponent<Image>().sprite
                = RoomPlayer.GetPlayerAt(0).playerDp.sprite;
        moveContentComputerIconGO.GetComponent<Image>().sprite
            = RoomPlayer.GetPlayerAt(1).playerDp.sprite;

        MovesDpPlayer1.sprite = RoomPlayer.GetPlayerAt(0).playerDp.sprite;

        MovesDpOpp1.sprite = RoomPlayer.GetPlayerAt(1).playerDp.sprite;

    }

  

    public void initMultiplayer()
    {
        Debug.Log("Init Multiplayer::GP Mode::"+gameMode);
        if (gameMode == 0)
        {
            Debug.Log("Players[0]" + RoomPlayer.GetPlayerAt(0).Username);
            Debug.Log("Players[1]" + RoomPlayer.GetPlayerAt(1).Username);
            RoomPlayer.GetPlayerAt(0).SetNameText(RoomPlayer.GetPlayerAt(0).Username.ToString());
            RoomPlayer.GetPlayerAt(1).SetNameText(RoomPlayer.GetPlayerAt(1).Username.ToString());

            RoomPlayer.Local.GetPlayerObj().transform.parent = PlayerOneTrans;
            RoomPlayer.Local.GetPlayerObj().transform.localPosition = new Vector3(-80,0,0);

            if (RoomPlayer.Local.IsLeader)
            {
                RoomPlayer.GetPlayerAt(1).GetPlayerObj().transform.parent = PlayerTwoTrans;
                RoomPlayer.GetPlayerAt(1).GetPlayerObj().transform.localPosition = new Vector3(-80, 0, 0);
            }
            else
            {
                RoomPlayer.GetPlayerAt(0).GetPlayerObj().transform.parent = PlayerTwoTrans;
                RoomPlayer.GetPlayerAt(0).GetPlayerObj().transform.localPosition = new Vector3(-80, 0, 0);
            }

            //RoomPlayer.GetPlayerAt(0).transform.GetChild(0).transform.localScale = Vector2.one;
            //RoomPlayer.GetPlayerAt(1).transform.GetChild(0).transform.localScale = Vector2.one;

            int curPlayerElo = FindObjectOfType<EloRatingSystem>().GetUpdatedPlayerElo();
            RoomPlayer.Local.RPC_SendElo(curPlayerElo);

        }
        
    }

    

   

  

    private void EnsureAllPlayersGOCReady(RoomPlayer lobbyPlayer)
    {
        //if (!RoomPlayer.Local.IsLeader) 
        //	return;

        if (IsAllGOCReady())
        {
            //load multiplayer game scene here
            SoundManager.Instance.PlayEffectFor(GameConstants.SoundEffect.Button_Click);

            playerHandler.initBoardDataOppGOC(lobbyPlayer);
            SyncOppCharactersGOC();
        }
    }

    private static bool IsAllGOCReady() => RoomPlayer.GetPlayers().Count == PlayerPrefs.GetInt("S_MaxUsers", 2) && RoomPlayer.GetPlayers().All(player => player.IsGOCSetup);

    public void SyncOppCharactersGOC()
    {
        if (playerHandler.getOppPlayers().Count > 0)
        { 
            oppCharacters = charHandler.PutPlayersToBoard(allBoardRect, playerHandler.getOppPlayers(), !isUserPlayerBlack, false);
            if (oppCharacters.Count > 0)
            {
                foreach (var p in oppCharacters)
                {
                    p.onClick.AddListener(() => { handleClickToOpponent(p); });
                }
            }
        }
    }

   

  

    public void OnRestartSameGame()
    {
        //SetupPrefsAndCleanup();
        SetupGameplay();
    }

    public void SetupPrefsAndCleanup()
    {
        playerHandler.cleanupForNewScene();

        UIManager.Instance.InitGameUI();

        currMyCiv = UIManager.Instance._myCivilization;
        currOppCiv = RoomPlayer.currCivOpp;

        Debug.Log("My Civilization::"+currMyCiv);
        Debug.Log("Opp Civilization"+currOppCiv);

        MyCivName.text = GetCivName(currMyCiv);
        OppCivName.text = GetCivName(currOppCiv);

        if (currOppCiv == (int)GameConstants.Civilisation.Teutons)//teuton
        {
            myPawnCanAttackPawn = false;
        }
        gameMode = RoomPlayer.gpMode;//PlayerPrefs.GetInt("gameMode", 0);
        Debug.Log("GameStartedWith GameMode=" + gameMode);
        actionTime = RoomPlayer.actionTime;//PlayerPrefs.GetInt("ActionTime", 0);
        if (actionTime == -1) actionTime = 0;
        Debug.Log("GameStartedWith ActionTime=" + actionTime);
        isGameOfChance = RoomPlayer.isGOC; //PlayerPrefs.GetInt("isChance", 0);
        Debug.Log("GameStartedWith isGOC=" + isGameOfChance);

        RoomPlayer.GetPlayerAt(0).GetTimer().PauseTimer();
        RoomPlayer.GetPlayerAt(1).GetTimer().PauseTimer();

        UpdateGameDPs();

    }

    [SerializeField]
    private List<Sprite> CivDps;

    public void UpdateGameDPs()
    {
        if (RoomPlayer.Local.IsLeader)
        {
            RoomPlayer.GetPlayerAt(0).playerDp.sprite = CivDps[currMyCiv];
            RoomPlayer.GetPlayerAt(1).playerDp.sprite = CivDps[currCivOpp];
        }
        else
        {
            RoomPlayer.GetPlayerAt(0).playerDp.sprite = CivDps[currCivOpp];
            RoomPlayer.GetPlayerAt(1).playerDp.sprite = CivDps[currMyCiv];
        }
        
    }

    public string GetCivName(int civ)
    {
        string name = "None";
        if (civ == (int)Civilisation.Teutons)
        {
            name = "Teutons";
        }
        else if (civ == (int)Civilisation.Vikings)
        {
            name = "Vikings";
        }
        else if (civ == (int)Civilisation.Britons)
        {
            name = "Britons";
        }
        if (civ == (int)Civilisation.French)
        {
            name = "French";
        }
        else if (civ == (int)Civilisation.Burgendy)
        {
            name = "Burgundy";
        }
        else if (civ == (int)Civilisation.Spanish)
        {
            name = "Spanish";
        }
        else if (civ == (int)Civilisation.Egyptian)
        {
            name = "Egyptian";
        }
        else if (civ == (int)Civilisation.Chinese)
        {
            name = "Chinese";
        }
        else if (civ == (int)Civilisation.Huns)
        {
            name = "Huns";
        }
        else if (civ == (int)Civilisation.Haitians)
        {
            name = "Haitians";
        }

        return name;
    }

    public void ResetTurnIndics()
    {

        RoomPlayer.Local.DeactivateTurnIndic();

       
    }

    public void ResetTimeIndics()
    {

        RoomPlayer.Local.DeactivateTimeIndic();


       
    }

    private bool isPawnSelected = false;
    public void handleClickToUserPlayers(Button p)
    {
        Debug.Log("Handler Click To User Player::");
        if (_gameState == MultiplayerGameState.ONGOING)
        {
            if (_state == MultiplayerPlayState.USER_MOVE || _state == MultiplayerPlayState.USER_WAIT_PROMOTE)
            {
                buttonsHandler.SetEnableDice(0);
                buttSelected = p;

                if (currOppCiv == (int)Civilisation.Teutons)//teutons
                {
                    int index = int.Parse(p.tag);
                    List<Character> userList = playerHandler.getUserPlayers();
                    Character player = userList[index];
                    if (player.getName() == CharacterDef.COASTGUARD)
                    {
                        isPawnSelected = true;
                    }
                    else
                    {
                        isPawnSelected= false;
                    }

                    Debug.Log("Is Pawn Selected=" + isPawnSelected);
                }

                int user = 1;
                if (!isBottomPlace) user = 0;

                getAvailableMoveAndColourThem(buttSelected, user); //1 user

               
            }
            
        }
    }

    public List<Char_Point> GetMoveSelectionList()
    {
        return posSelectionToMove;
    }

    public void getAvailableMoveAndColourThem(Button buttSelect, int whichOne)
    {

        Debug.Log("getAvailableMoveAndColourThem::" + buttSelect.GetComponent<GridInfo>().x + "," + buttSelect.GetComponent<GridInfo>().y);

        charHandler.RemoveHighLightColour();
        posSelectionToMove.Clear();

        int index = int.Parse(buttSelect.tag);

        Debug.Log("Index::"+index);

        if (_state == MultiplayerPlayState.OPP_WAIT_PROMOTE || _state == MultiplayerPlayState.USER_WAIT_PROMOTE)
            posSelectionToMove = playerHandler.selectPositionToPromote(index, whichOne);
        else
            posSelectionToMove = playerHandler.selectWhereToGo(index, whichOne);

        charHandler.ColourEffectPlaceWhereToGo(posSelectionToMove, allBoardRect);

       

    }

    private bool GetIsPawnTarget(Transform transform)
    {

        


        foreach (Transform gameObjectT in transform)
        {
            string name = gameObjectT.name;

            Debug.Log("GetIsPawnTarget::Child:::" + name);

            //gameobject has other child for transform's highlights etc positions purpose so we only need actual child
            //i.e character or chip

            if (gameObjectT.gameObject.activeSelf)
            {
                if (!name.Equals("b") && !name.Equals("w") && !name.Equals("bw") &&
                    !name.Equals("bt") && !name.Equals("wt") && !name.Equals("bwt") && !name.Equals("blast"))
                {
                    int index = int.Parse(gameObjectT.tag);
                    List<Character> userList = playerHandler.getUserPlayers();
                    Character player = userList[index];

                    if (player.getName() == CharacterDef.COASTGUARD)
                    {
                        return true;

                    }
                    else
                    {
                        Debug.Log("@@@Pawn is Not Found in Parent@@@");
                    }
                }
            }
        }

        return false;
    }

    public bool HandleClickToTheBoard(Button p)
    {
        Debug.Log("MAA::Handler Click To the Board::");
        Debug.Log("State::"+_state);
        Debug.Log("GamePlayState::"+_gameState);

        if ((_state == MultiplayerPlayState.USER_MOVE || _state == MultiplayerPlayState.USER_WAIT_PROMOTE) && buttSelected && _gameState == MultiplayerGameState.ONGOING)
        {
            if(currOppCiv == (int)GameConstants.Civilisation.Teutons)
            {
                bool isTargetPawn = GetIsPawnTarget(p.transform);
                if(isTargetPawn && isPawnSelected && RoomPlayer.Local.playSessionCount < 11)
                {
                    Debug.Log("@@@Cant attack Opp Pawn till 10th turn as he is teuton@@@");
                    isPawnSelected = false;
                    buttSelected.GetComponent<DragAndDropHandler>().ResetPlace();
                    buttSelected.GetComponent<DragAndDropHandler>().ResetHighligts();

                    return false;
                }
            }

            // ... do
            Button moveToButton = p;
            bool clickToMove = false;

            int user = 1;
            int chipTag = (int)GameConstants.PlayerTags.USER_CHIP_TAG;
            if (!isBottomPlace)
            {
                user = 0;
                chipTag = (int)GameConstants.PlayerTags.COMP_CHIP_TAG;
            }

            Debug.Log("Button Selected::" + buttSelected);


            foreach (Char_Point pos in posSelectionToMove)
            {
                int indx = pos.index();
                if (int.Parse(moveToButton.tag) == indx && buttSelected &&
                    int.Parse(buttSelected.tag) < chipTag)
                {
                    clickToMove = true;
                }
            }

            Debug.Log("Click To Move::" + clickToMove);


            if (_state == MultiplayerPlayState.USER_MOVE && clickToMove)
            {
                
                if (moveButtSelectToNewPos(moveToButton, buttSelected, user))
                {
                    if (_state != MultiplayerPlayState.USER_WAIT_PROMOTE)
                    {
                        UpdateStatusAfterMove();

                        StartCoroutine(PassTurnTimed(0.25f));
                        //PassTurn();


                    }
                    else
                    {
                        displayRescue_text(1);
                    }
                }
            }
            else if (_state == MultiplayerPlayState.USER_WAIT_PROMOTE && clickToMove)
            {
                disappearRescue_text();


                moveButtSelectToNewPos(moveToButton, buttSelected, user);
                
                UpdateStatusAfterMove();
                StartCoroutine(PassTurnTimed(0.25f));
                //PassTurn();
            }

        }

        buttSelected = null;
        return true;
        //_state = MultiplayerPlayState.OPP_MOVE;

    }

    public Button GetButtonToSync(uint id, Char_Point gridInfo, bool isChip=false, bool isRescue=false, bool isMine=false)
    {

        Button buttonToSync = null;

        if (!isChip)
        {
            if (!isRescue)
            {
                foreach (Button parentBtn in allBoardRect)
                {
                    Transform parent = parentBtn.transform;
                    foreach (Transform transform1 in parent)
                    {
                        string name = transform1.name;
                        if (!name.Equals("b") && !name.Equals("w") && !name.Equals("bw") &&
                            !name.Equals("bt") && !name.Equals("wt") && !name.Equals("bwt") && !name.Equals("blast"))
                        {
                            GridInfo gridInfo1 = transform1.GetComponent<GridInfo>();
                            if (gridInfo1 != null)
                            {
                                if (gridInfo.x == gridInfo1.x && gridInfo.y == gridInfo1.y && gridInfo.colour == gridInfo1.colour)
                                {
                                    Transform btnTrans = transform1;
                                    buttonToSync = btnTrans.GetComponent<Button>();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {

                //need to sync button from captured

                Transform powTransform = PlayerPowTransform;

                if (RoomPlayer.gpMode != 0)
                {
                    if (isMyTeam(id))
                    {
                        Debug.Log("My Team, Rescue Get button to sync");
                        powTransform = OppPowTransform;
                    }
                }



                foreach (Transform parent in powTransform)
                {

                    foreach (Transform charac in parent)
                    {
                        GridInfo gridInfo1 = charac.GetComponent<GridInfo>();
                        if (gridInfo1 != null)
                        {
                            if (gridInfo.x == gridInfo1.x && gridInfo.y == gridInfo1.y && gridInfo.colour == gridInfo1.colour)
                            {
                                buttonToSync = charac.GetComponent<Button>();
                                break;
                            }
                        }
                    }
                }
            }
        }
        else
        {

            List<Button> chipList = oppChips;
            if(isMine)//the case when opponent captures this user's chip and needs to be synced here
            {
                Debug.Log("The case when opponent captures this user's chip and needs to be synced here");
                chipList = userChips;
            }

            if (RoomPlayer.gpMode != 0)
            {
                if (isMyTeam(id))
                {
                    chipList = userChips;
                }
            }

            Debug.Log(gridInfo.x + ":Sync Opp Chip should be at:" + gridInfo.y);
            foreach (Button chip in chipList)
            {
                
                    GridInfo gridInfo1 = chip.GetComponent<GridInfo>();
                    if (gridInfo1 != null)
                    {
                        if (gridInfo.x == gridInfo1.x && gridInfo.y == gridInfo1.y && gridInfo.colour == gridInfo1.colour)
                        {
                            Debug.Log("Chip Found!");
                            buttonToSync = chip;
                            break;
                        }
                    }
                
            }
        }

        //Debug.Log("Opponent's Button to sync::" + buttonToSync.GetComponent<GridInfo>().x + "," + buttonToSync.GetComponent<GridInfo>().y);
        return buttonToSync;
        
    }

    public void SyncColorHighlight(uint id, List<Char_Point> posList)
    {
        if (!isMyTeam(id))
        {
            foreach (Char_Point item in posList)
            {
                item.x = 7 - item.x;
                item.y = 7 - item.y;

                item.colour = item.colour == colour.WHITE ? colour.BLACK : colour.WHITE;
            }

        }

        charHandler.ColourEffectPlaceWhereToGo(posList, allBoardRect);

    }

    //Button moveToSync;
    private List<Char_Point> localColorHiglight = new List<Char_Point>();

    public void SyncMoveToTheBoard(uint id, Char_Point gridInfoBtn, Char_Point gridInfo, bool isChip, bool isRescue, int isPromoted = -1)
    {


        //need to map x,y, colour and chardef
        //to findout which character would that be at opponent end
        //once found that button then assign it to movTo

        int whichOne = 0;
        if (RoomPlayer.gpMode != 0)
        {
            if (isMyTeam(id))
            {
                Debug.Log("Sync move to my team:Orig grid Info Btn");
                Debug.Log("X=" + gridInfoBtn.x + ",Y=" + gridInfoBtn.y);

                gridInfoBtn.x = 7 - gridInfoBtn.x;
                gridInfoBtn.y = 7 - gridInfoBtn.y;

                gridInfo.x = 7 - gridInfo.x;
                gridInfo.y = 7 - gridInfo.y;

                whichOne = 1;

                Debug.Log("Sync move to my team:My team grid Info Btn");
                Debug.Log("X=" + gridInfoBtn.x + ",Y=" + gridInfoBtn.y);
            }
        }

        Button buttonToSync = GetButtonToSync(id, gridInfoBtn, isChip, isRescue);

        /*if (isPromoted >= 0)
        {
            SyncPromoted(buttonToSync, isPromoted);
        }
        else
        {*/

            
            if (buttonToSync != null)
            {
                //charHandler.RemoveHighLightColour();

                RectTransform buttonRect = buttonToSync.GetComponent<RectTransform>();


                buttonRect.transform.parent = null;

                Button moveToSync = allBoardRect.ElementAt<Button>(gridInfo.index());

                //Char_Point charPColor = new Char_Point(gridInfoColor.x, gridInfoColor.y, gridInfoColor.colour);

                //localColorHiglight.Add(charPColor);
                //charHandler.ColourEffectPlaceWhereToGo(localColorHiglight, allBoardRect);

                Debug.Log(gridInfo.x + ":Move sync Opp with:" + gridInfo.y);

                buttonRect.transform.parent = moveToSync.transform;

                buttonRect.transform.localPosition = charHandler.GetPosInRectFromCharPoint(moveToSync, gridInfo).localPosition;
                buttonRect.transform.localScale = Vector2.one;

                ////////////////////asdf
                buttonRect.GetComponent<GridInfo>().x = gridInfo.x;
                buttonRect.GetComponent<GridInfo>().y = gridInfo.y;

                //Debug.Log("MA::move to new position::"+ int.Parse(buttSelected.tag));
                if (int.Parse(buttonRect.tag) >= (int)GameConstants.PlayerTags.COMP_CHIP_TAG)
                {
                    buttonRect.localRotation = Quaternion.Euler(0f, 0f, 0);

                }



                charHandler.RemoveHighLightColour();

                if (isChip)
                {
                    playerHandler.updatePositionForChip(gridInfo, whichOne, int.Parse(buttonToSync.tag), true);
                }
                else
                {
                    playerHandler.updatePositionForPlayer(gridInfo, whichOne, int.Parse(buttonToSync.tag), true, false, isPromoted);
                }
            }
        //}
    }

    public void SyncPromoted(Button buttonToSync, int option)
    {
        
            Debug.Log("SyncMove Is Promoted True");

            


        if (option == 3)//Knight
        {
            if (RoomPlayer.Local.IsLeader)
            {
                buttonToSync.image.sprite = charHandler.NavyB.sprite;
            }
            else
            {
                buttonToSync.image.sprite = charHandler.NavyW.sprite;
            }
        }
        else if (option == 4)//Tower
        {
            if (RoomPlayer.Local.IsLeader)
            {
                buttonToSync.image.sprite = charHandler.MarineB.sprite;
            }
            else
            {
                buttonToSync.image.sprite = charHandler.MarineW.sprite;
            }
        }
        else if (option == 2)//elephant
        {
            if (RoomPlayer.Local.IsLeader)
            {
                buttonToSync.image.sprite = charHandler.AirForceB.sprite;
            }
            else
            {
                buttonToSync.image.sprite = charHandler.AirForceW.sprite;
            }
        }
        else if (option == 1)//Wazir
        {
            if (RoomPlayer.Local.IsLeader)
            {
                buttonToSync.image.sprite = charHandler.ArmyB.sprite;
            }
            else
            {
                buttonToSync.image.sprite = charHandler.ArmyW.sprite;
            }
        }
        else if (option == 9)//Horsemen
        {
            if (RoomPlayer.Local.IsLeader)
            {
                buttonToSync.image.sprite = charHandler.horseMenSprB;
            }
            else
            {
                buttonToSync.image.sprite = charHandler.horseMenSprW;
            }
        }
        else if (option == 10)//Witch
        {
            if (RoomPlayer.Local.IsLeader)
            {
                buttonToSync.image.sprite = charHandler.WitchB.sprite;
            }
            else
            {
                buttonToSync.image.sprite = charHandler.WitchW.sprite;
            }
        }
        else if (option == 5)//Pawn
        {
            if (RoomPlayer.Local.IsLeader)
            {
                buttonToSync.image.sprite = charHandler.CoastGuardB.sprite;
            }
            else
            {
                buttonToSync.image.sprite = charHandler.CoastGuardW.sprite;
            }
        }
    }

    public void SyncTransform(uint id, Char_Point gridInfoBtn, int option, bool isMutation)
    {
        Button buttonToSync = GetButtonToSync(id, gridInfoBtn, false, false);
        SyncPromoted(buttonToSync, option);

        if(isMutation)
        {
            //btn drag drop disable
            buttonToSync.GetComponent<DragAndDropHandler>().enabled = false;
        }
    }

    public void SyncPostMutation(int index)
    {
        //changed owner
        //remove from player list
        //add to opponent list
        //Character oppChar = playerHandler.getUserPlayers()[index];
        //oppChar.setOwner(Owner.COMPUTER);
        //playerHandler.getUserPlayers().Remove(oppChar);
        //playerHandler.getOppPlayers().Add(oppChar);
    }

    /*private void HandleClickToUserChips(Button p)
    {
        Debug.Log("Handler Click To User Chip::");

        if (_gameState == MultiplayerGameState.ONGOING *//*&& isInCheck == false*//*)
        {
            if (_state == MultiplayerPlayState.USER_MOVE)
            {
                Debug.Log("touch to user Chip");
                buttSelected = p;

                int user = 1;
                if (!isBottomPlace) user = 0;

                int index;
                if(isBottomPlace)
                {
                    index = int.Parse(buttSelected.tag) - (int)GameConstants.PlayerTags.USER_CHIP_TAG;
                }
                else
                {
                    index = int.Parse(buttSelected.tag) - (int)GameConstants.PlayerTags.COMP_CHIP_TAG;
                }

                getAvailableMoveAndColourThem(buttSelected, user); //1 user 
                buttonsHandler.SetEnableDice(playerHandler.getRightDice(playerHandler.getUserChips()[index]));

                
            }
            

        }
        
    }*/

    IEnumerator PassTurnTimed(float time)
    {
        yield return new WaitForSeconds(time);
        PassTurn();

        
    }


    private int turnFlag = 0;
    public void PassTurn()
    {

        RoomPlayer.Local.playSessionCount += 1;
        RoomPlayer.Local.RPC_UpdateTurnCount();

        _state = MultiplayerPlayState.OPP_MOVE;
        ResetTurnIndics();
        ResetTimeIndics();

        RoomPlayer.Local.GetTimer().PauseTimer();
        //Need to Resume other player timer
        if (RoomPlayer.Local.IsLeader)
        {
            RoomPlayer.GetPlayerAt(1).GetTimer().ResumeTimer();
        }
        else
        {
            RoomPlayer.GetPlayerAt(0).GetTimer().ResumeTimer();
        }

        uint localId = RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw;

        uint candidateId = 0;
        if (gameMode == 0)
        {
            Debug.Log("Pass Turn1:gpMode:"+gameMode);
            if (RoomPlayer.Local.IsLeader)
            {
                candidateId = 3;

            }
            else
            {
                candidateId = 2;

            }
        }
        else if (gameMode == 1)
        {
            Debug.Log("Pass Turn2:gpMode:" + gameMode);
            if (RoomPlayer.Local.IsLeader)
            {
                Debug.Log("Pass Turn3:turnFlag:" + turnFlag);
                if (turnFlag == 0)
                {
                    candidateId = 3;
                    turnFlag = 1;
                }
                else
                {
                    candidateId = 4;
                    turnFlag = 0;
                }
            }
            else
            {
                candidateId = 2;
            }
        }
        else
        {
            if (RoomPlayer.Local.IsLeader)
            {
                
                candidateId = 3;
                
            }
            else
            {
                Debug.Log("Pass Turn4:localID:" + localId);

                if (localId.Equals(3))
                {
                    candidateId = 4;
                }
                else if (localId.Equals(4))
                {
                    candidateId = 5;
                }
                else
                {
                    candidateId = 2;
                }
            }
        }

        

        Debug.Log("Pass Turn5:CandidID:" + candidateId);

        //MA::for skipping rescue
        if (passTurnBtn.activeSelf)
        {
            disappearRescue_text();
            UpdateStatusAfterMove();
        }

        if (!isMyLastMove)
        {

            RoomPlayer.Local.GetTimer().PauseTimer();
            //Need to Resume other player timer
            if (RoomPlayer.Local.IsLeader)
            {
                RoomPlayer.GetPlayerAt(1).GetTimer().ResumeTimer();
            }
            else
            {
                RoomPlayer.GetPlayerAt(0).GetTimer().ResumeTimer();
            }
            RoomPlayer.Local.RPC_PassTurn(candidateId);
        }
        else
        {
            ShowResultBtn.SetActive(true);
            winTxt = "Opponent Won!";
            
        }

    }

    private bool myPawnCanAttackPawn = true;
    public void SyncTurnCounter()
    {
        turnText.text = "Turn = " + RoomPlayer.Local.playSessionCount.ToString();

        if (currOppCiv == 0)
        { 
            int turnCount = RoomPlayer.Local.playSessionCount;
            if (turnCount > 10 && !myPawnCanAttackPawn)
            {
                myPawnCanAttackPawn = true;
                Debug.Log("My Pawn Can Attack Opp Pawn Now");
            }
        }
    }

    public void GetTurn(uint candidateId)
    {
       /* RoomPlayer.Local.playSessionCount += 1;
        RoomPlayer.Local.RPC_UpdateTurnCount();*/

        /*if (RoomPlayer.Local.playSessionCount % 2 == 0 && RoomPlayer.Local.playSessionCount != 0)
        {
            Debug.Log("Turn Completed!!!!!!");
            turnText.text = "Turn = " + RoomPlayer.Local.playSessionCount.ToString();
            RoomPlayer.Local.RPC_UpdateTurnCount();
        }*/

        uint localId = RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw;

        if (localId.Equals(candidateId))
        {

            RoomPlayer.Local.GetTimer().ResumeTimer();

            //Need to pause other player timer
            if (RoomPlayer.Local.IsLeader)
            {
                RoomPlayer.GetPlayerAt(1).GetTimer().PauseTimer();
            }
            else
            {
                RoomPlayer.GetPlayerAt(0).GetTimer().PauseTimer();
            }


            _state = MultiplayerPlayState.USER_MOVE;
            if (actionTime == 0)
            {
                RoomPlayer.Local.ActivateTurnIndic();
            }
            else
            {
                RoomPlayer.Local.ActivateTimeIndic();
            }
        }
    }

    

    public Button getButtonByTag(List<Button> bttList, int tag)
    {
        foreach (Button butt in bttList)
        {
            //MA
            if (butt != null)
            {
                if (int.Parse(butt.tag) == tag)
                    return butt;
            }
            
        }


        return null;
    }

    public void handleClickToOpponent(Button pSender)
    {
        Debug.Log("MAA::Handler Click To Opponent::");

        if (_gameState == MultiplayerGameState.ONGOING)
        {
            Button touchPlayer = pSender;
            handleClickToTheBoard(touchPlayer.transform.parent);
        }
    }

    public void handleClickToTheBoard(Transform parent)
    {

        if ( (_state == MultiplayerPlayState.USER_MOVE || _state == MultiplayerPlayState.USER_WAIT_PROMOTE) && buttSelected && _gameState == MultiplayerGameState.ONGOING)
        {
            Debug.Log("Current Opp Civ="+currOppCiv);

            if (currOppCiv == 0)
            {
                bool isTargetPawn = GetIsPawnTarget(parent);
                Debug.Log("Is target Pawn=" + isTargetPawn);
                Debug.Log("Turn count=" + RoomPlayer.Local.playSessionCount);
                Debug.Log("Is Pawn Selected=" + isPawnSelected);
                if (isTargetPawn && isPawnSelected && !myPawnCanAttackPawn)
                {
                    isPawnSelected = false;
                    buttSelected.GetComponent<DragAndDropHandler>().ResetPlace();
                    buttSelected.GetComponent<DragAndDropHandler>().ResetHighligts();
                    return;
                }
            }

            // ... do
            Button moveToButton = parent.gameObject.GetComponent<Button>();
            bool clickToMove = false;

            int user = 1;
            int chipTag = (int)GameConstants.PlayerTags.USER_CHIP_TAG;
            if (!isBottomPlace)
            {
                user = 0;
                chipTag = (int)GameConstants.PlayerTags.COMP_CHIP_TAG;
            }

            foreach (var pos in posSelectionToMove)
            {
                int indx = pos.index();
                if (moveToButton.tag.Equals(indx.ToString()) && buttSelected &&
                    int.Parse(buttSelected.tag) < chipTag)
                {
                    clickToMove = true;
                }
            }

            if (_state == MultiplayerPlayState.USER_MOVE && clickToMove)
            {
                if (moveButtSelectToNewPos(moveToButton, buttSelected, user))
                {
                    if (_state != MultiplayerPlayState.USER_WAIT_PROMOTE)
                    {
                        //PassTurn();

                        
                        StartCoroutine(PassTurnTimed(actionTime));
                        UpdateStatusAfterMove();

                    }
                    else
                    {
                        displayRescue_text(user);
                    }
                }
            }
            else if (_state == MultiplayerPlayState.USER_WAIT_PROMOTE && clickToMove)
            {
                disappearRescue_text();
                moveButtSelectToNewPos(moveToButton, buttSelected, user);

                //PassTurn();

                
                StartCoroutine(PassTurnTimed(actionTime));
                UpdateStatusAfterMove();

            }

        }
       

    }

    

    private void disappearRescue_text()
    {
        rescueTxt.gameObject.SetActive(false);
        passTurnBtn.SetActive(false);

        
        RoomPlayer.Local.RPC_DisparRescue();
        
        
    }

    public void SynDisappearResceText()
    {
        rescueTxt.gameObject.SetActive(false);
    }

    private void displayRescue_text(int whichOne)
    {
        //if (whichOne > 0)
        //{
            rescueTxt.text = "User Can Rescue one piece";
        //}

        passTurnBtn.SetActive(false);

        /*else
        {
            rescueTxt.text = "Opponent will Rescue one piece";
        }*/
        

        rescueTxt.gameObject.SetActive(true);

        RoomPlayer.Local.RPC_DisplayRescue();
    }

    public void SyncDisplayRescueText()
    {
        rescueTxt.text = "Opponent will Rescue one piece";
        rescueTxt.gameObject.SetActive(true);
    }

   
    public void UpdateStatusAfterMove()
    {

        UpdateMovingStep(playerHandler.getMovingStep());
        updatePowAndDieList();

        Debug.Log("MA::UpdateStatusAfterMove::"+gameMode);

        if (gameMode == 0)
        {
            UpdatePoint(playerHandler.getPoint(1), playerHandler.getPoint(0));
        }
        /*else if (gameMode == 1)
        {
            UpdatePoint(playerHandler.getPoint(1), playerHandler.getPoint(0), playerHandler.getPoint(2));
        }
        else if (gameMode == 2)
        {
            Debug.Log("MA::Update Points::" + gameMode);

            UpdatePoint(playerHandler.getPoint(1), playerHandler.getPoint(0), playerHandler.getPoint(2), playerHandler.getPoint(3));
        }*/



        //updatePowAndDieList();
        if (currMyCiv == (int)Civilisation.French)
        {
            isInCheck = false;
            ShowCheck(isInCheck);

            //buttonsHandler.SetEnableDice(0);
            _gameState = MultiplayerGameState.ONGOING;
        }
        else
        {
            isInCheck = false;///*playerHandler.isPresidentInCheck(0) ||*/ playerHandler.isPresidentInCheck(1);
            ShowCheck(isInCheck);

            //buttonsHandler.SetEnableDice(0);
            _gameState = playerHandler.isGameOver();
        }

        StartCoroutine(ShowGameOverCort(true));
        
    }

    public void ShowCheck(bool isInCheck)
    {
        checkTxt.gameObject.SetActive(isInCheck);
        if (isInCheck)
        {
            checkTxt.text = "Check!";
            //ShowGameOverTemp();//MA::for testing only
        }
        else
        {
            checkTxt.text = "";
        }

        if(RoomPlayer.Local)
        {
            RoomPlayer.Local.RPC_ShowCheck(isInCheck);
        }
        
    }

    public void SyncShowCheck(bool isInCheck)
    {
        checkTxt.gameObject.SetActive(isInCheck);
        if (isInCheck)
            checkTxt.text = "Check!";
        else
            checkTxt.text = "";
    }

    private bool isLeaving = false;
    public void ShowGameOverTemp(bool isOppWon = false, bool isLeft = false)
    {
        isLeaving = isLeft;
        if (isOppWon)
        {
            _gameState = MultiplayerGameState.OPP_WIN;
        }
        else
        {
            _gameState = MultiplayerGameState.USER_WIN;

        }

        StartCoroutine(ShowGameOverCort());
    }

    private string winTxt = "";
    IEnumerator ShowGameOverCort(bool isSanitChk = false)
    {
        Debug.Log("Showing Game Over!!!");

        EloRatingSystem eloRatingSystem =  FindObjectOfType<EloRatingSystem>();
        bool isAutoMatch = UIManager.Instance.autoMatchMade;

        if (_gameState == MultiplayerGameState.USER_WIN)
        {

            //GameOverImg.SetActive(true);
            yield return new WaitForSeconds(1);
            ShowResultBtn.SetActive(true);
            
            winTxt = "You Won!";
            if(isLeaving)
            {
                goReasonTxt.text = "Opponent Left";
            }

            uint localId = RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw;

            if (isAutoMatch)
            {
                eloRatingSystem.CalculateEloRating(30, 1, currMyCiv, currOppCiv, false);
            }

            RoomPlayer.Local.RPC_SendWon(localId, true, isLeaving);
            isLeaving = false;
            RoomPlayer.Local.RPC_PassTurn(localId);
        }
        else if (_gameState == MultiplayerGameState.OPP_WIN)
        {
            yield return new WaitForSeconds(1);
            ShowResultBtn.SetActive(true);
            winTxt = "You Lost!";
            if (isLeaving)
            {
                goReasonTxt.text = "You Left";
            }
            uint localId = RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw;

            if (isAutoMatch)
            {
                eloRatingSystem.CalculateEloRating(30, 0, currMyCiv, currOppCiv, false);
            }

            RoomPlayer.Local.RPC_SendWon(localId, false, isLeaving);
            isLeaving = false;
            RoomPlayer.Local.RPC_PassTurn(localId);
        }
        else
        {
            if (isAutoMatch)
            {
                if (!isSanitChk)//we dont want to calcilate elo on everytime game state check
                {
                    eloRatingSystem.CalculateEloRating(30, 0.5f, currMyCiv, currOppCiv, true);
                }
            }
        }

    }

    public void OnGameOverResult()
    {
        
            //GameOverImg.SetActive(true);

            //checkTxt.text = "";
            ShowCheck(false);

            //GameOverImg.SetActive(false);

            ShowGameOverUI(winTxt);

            
        
    }

    public void ShowGameOverUI(string pwinText)
    {
        winTxt = pwinText;
        /*if (gameMode == 1)
        {
            float totalPointsT2 = playerHandler.getPoint(0) + playerHandler.getPoint(2);
            UIManager.Instance.ShowGameOver(gameMode, pwinText, playerHandler.getPoint(1).ToString(), totalPointsT2.ToString());
            HideMoves();
        }*/
        /*if (gameMode == 2)
        {*/
            float totalPointsT1 = playerHandler.getPoint(1) + playerHandler.getPoint(3);
            float totalPointsT2 = playerHandler.getPoint(0) + playerHandler.getPoint(2);
            UIManager.Instance.ShowGameOver(gameMode, pwinText, totalPointsT1.ToString(), totalPointsT2.ToString());
            HideMoves();

        /*}
        else
        {

            UIManager.Instance.ShowGameOver(gameMode, pwinText, playerHandler.getPoint(1).ToString(), playerHandler.getPoint(0).ToString());
        }*/
    }

    private bool isMyLastMove = false;
  
    public void SyncGameOver(uint playerId, bool isWon, bool isLeaving)
    {
        //GameOverImg.SetActive(true);


        if (!isMyLastMove)
        {
            isMyLastMove = true;
            /*if (isWon)
            {
                winTxt = "Opponent Won!";
                if (isLeaving)
                {
                    goReasonTxt.gameObject.SetActive(true);
                    goReasonTxt.text = "You Left";
                }
            }
            else
            {
                winTxt = "You Won!";
                if (isLeaving)
                {
                    goReasonTxt.gameObject.SetActive(true);
                    goReasonTxt.text = "Opponent Left";
                }
            }*/
        }
        /*else
        {
            ShowResultBtn.SetActive(true);
            

            *//*if (isMyTeam(playerId))
            {
                if(isWon)
                    winTxt = "You Won!";
                else
                    winTxt = "Opponent Won!";
            }
            else
            {*//*
            if (isWon)
            {
                winTxt = "Opponent Won!";
                if(isLeaving)
                {
                    goReasonTxt.gameObject.SetActive(true);
                    goReasonTxt.text = "You Left";
                }
            }
            else
            {
                winTxt = "You Won!";
                if (isLeaving)
                {
                    goReasonTxt.gameObject.SetActive(true);
                    goReasonTxt.text = "Opponent Left";
                }
            }
            //}
        }*/

        if (isWon)
        {
            winTxt = "Opponent Won!";
            if (isLeaving)
            {
                goReasonTxt.gameObject.SetActive(true);
                goReasonTxt.text = "You Left";
            }
        }
        else
        {
            winTxt = "You Won!";
            if (isLeaving)
            {
                goReasonTxt.gameObject.SetActive(true);
                goReasonTxt.text = "Opponent Left";
            }
        }
        ShowResultBtn.SetActive(true);

        //ShowGameOverUI(winTxt);

        /*if (gameMode == 1)
        {
            float totalPointsT2 = playerHandler.getPoint(0) + playerHandler.getPoint(2);
            UIManager.Instance.ShowGameOver(gameMode, winTxt, playerHandler.getPoint(1).ToString(), totalPointsT2.ToString());
            HideMoves();

        }
        else if (gameMode == 2)
        {
            float totalPointsT1 = playerHandler.getPoint(1) + playerHandler.getPoint(3);
            float totalPointsT2 = playerHandler.getPoint(0) + playerHandler.getPoint(2);
            UIManager.Instance.ShowGameOver(gameMode, winTxt, totalPointsT1.ToString(), totalPointsT2.ToString());
            HideMoves();

        }
        else
        {

            UIManager.Instance.ShowGameOver(gameMode, winTxt, playerHandler.getPoint(1).ToString(), playerHandler.getPoint(0).ToString());

        }*/
    }

    public void HideMoves()
    {
        MovesPanel.SetActive(false);
    }

    public void ShowMoves()
    {
        MovesPanel.SetActive(true);
    }

    public void updatePowAndDieList(bool isSync = false, bool isForceAdd = false)
    {

        //UpdatePlayerPowAndDieList(isSync, isForceAdd);
        UpdateOpponentPowAndDieList(isSync, isForceAdd);
    }

    public Transform GetOppPowTransform()
    {
        return OppPowTransform;
    }

    public Transform GetUserPowTransform()
    {
        return PlayerPowTransform;
    }

    /*
        public int GetPlayerCapturIndex()
        {
            return capute_Index1;
        }*/

    //int capute_Index1 = 0;
    //int kill_Index1 = 0;
    public void UpdatePlayerPowAndDieList(bool isSync = false, bool isForceAdd = false)
    {
        List<Character> userList = playerHandler.getUserPlayers();


        //int capute_Index1 = 0;
        //int kill_Index1 = 0;

        for (int i = 0; i < userList.Count(); i++)
        {
            Debug.Log("@@@IsaliveStatus:"+ userList[i].isAlive());
            Debug.Log("@@@IsCaptured:" + userList[i].GetIsCaptured());

            if (userList[i].isAlive() == State.CAPTURE && !userList[i].GetIsCaptured())
            {
                Button btt = getButtonByTag(userCharacters, i);
                if (btt != null)
                {
                    Debug.Log("@@user's updating pow and die list::"+ userList[i].getName());
                    //Debug.Log("btt Name:"+btt.name);
                    //Debug.Log("btt Name:" + btt.GetComponent<GridInfo>().characterDef);

                    /*if (!isForceAdd)
                    {
                        Debug.Log("Not Force Add:1");
                        
                        btt.*//*GetComponent<RectTransform>().*//*transform.SetParent(PlayerPowTransform*//*.GetChild(capute_Index1).GetComponent<RectTransform>().transform*//*);
                        btt.transform.localPosition = Vector2.zero;

                        

                    }
                    else
                    {*/
                    Debug.Log("Force Add:2");
                        btt.transform.SetParent(OppPowTransform/*.GetChild(capute_Index1).GetComponent<RectTransform>().transform*/);
                        btt.transform.localPosition = Vector2.zero;
                    //}

                    DragAndDropHandler dragAndDropHandler = btt.GetComponent<DragAndDropHandler>();
                    if (dragAndDropHandler)
                    {
                        dragAndDropHandler.enabled = false;
                    }

                    CharacterDef characterDef = btt.GetComponent<GridInfo>().characterDef;
                    if (characterDef == CharacterDef.PRESIDENT || characterDef == CharacterDef.ARMY)
                    {
                        //btt.transform.localScale = new Vector3(0.5f, 0.5f, 1);

                    }

                    /*if(isComp2Active)
                    {
                        userList[i].setIsOutByComp2(true);
                    }*/

                    if (!isSync)
                    {
                        Debug.Log("Now Sync@@Capture::captured Char:tag=" + btt.tag);
                        int cx = 7 - btt.GetComponent<GridInfo>().x;
                        int cy = 7 - btt.GetComponent<GridInfo>().y;
                        int cColour = (int)btt.GetComponent<GridInfo>().colour;
                        uint localId = RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw;

                        RoomPlayer.Local.RPC_SyncCaptured(localId, cx, cy, (int)cColour, isForceAdd, false);
                        playerHandler.updatePoint();

                    }
                    //capute_Index1++;
                    userList[i].SetIsCaptured(true);

                }
            }
            /*else if (userList[i].isAlive() == State.DIED)
            {

                Button btt = getButtonByTag(userCharacters, i);
                if (btt != null)
                {
                    if (OppDieTransform.GetChild(kill_Index1).GetComponent<RectTransform>().transform.childCount > 0)
                    {
                        kill_Index1 += 1;
                    }

                    btt.GetComponent<RectTransform>().transform.SetParent(OppDieTransform.GetChild(kill_Index1).GetComponent<RectTransform>().transform);
                    btt.transform.localPosition = Vector2.zero;

                    CharacterDef characterDef = btt.GetComponent<GridInfo>().characterDef;
                    if (characterDef == CharacterDef.PRESIDENT || characterDef == CharacterDef.ARMY)
                    {
                        btt.transform.localScale = new Vector3(0.5f, 0.5f, 1);

                    }

                    *//*if (isComp2Active)
                    {
                        userList[i].setIsOutByComp2(true);
                    }*//*

                    if (!isSync)
                    {
                        Debug.Log("@@Capture::I have killed Opp Char:tag=" + btt.tag);
                        int cx = 7 - btt.GetComponent<GridInfo>().x;
                        int cy = 7 - btt.GetComponent<GridInfo>().y;
                        int cColour = (int)btt.GetComponent<GridInfo>().colour;
                        uint localId = RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw;

                        RoomPlayer.Local.RPC_SyncDied(localId, false, cx, cy, (int)cColour);
                        playerHandler.updatePoint();

                        kill_Index1++;
                    }

                }

            }*/
        }
    }

   

    /*public void UpdatePowList()
    {
        int capute_Index = 0;
        List<Character> compList = playerHandler.getOppPlayers();
        for (int i = 0; i < compList.Count(); i++)
        {
            if (compList[i].isAlive() == State.CAPTURE)
            {
                Button btt = getButtonByTag(oppCharacters, i);
                if (btt != null)
                {
                    btt.transform.localRotation = Quaternion.identity;


                    btt.transform.SetParent(PlayerPowTransform.GetChild(capute_Index).GetComponent<RectTransform>().transform);
                    btt.transform.localPosition = Vector2.zero;

                    CharacterDef characterDef = btt.GetComponent<GridInfo>().characterDef;
                    if (characterDef == CharacterDef.PRESIDENT || characterDef == CharacterDef.ARMY)
                    {
                        btt.transform.localScale = new Vector3(0.5f, 0.5f, 1);

                    }

                    Debug.Log("Capture::I have captured Opp Char:tag=" + btt.tag);
                    int cx = 7 - btt.GetComponent<GridInfo>().x;
                    int cy = 7 - btt.GetComponent<GridInfo>().y;
                    int cColour = (int)btt.GetComponent<GridInfo>().colour;
                    uint localId = RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw;

                    RoomPlayer.Local.RPC_SyncCaptured(localId, cx, cy, (int)cColour, capute_Index, false);

                    capute_Index++;
                }
            }
        }
    }*/

    /*public void UpdateDieList()
    {
        int kill_Index = 0;
        List<Character> compList = playerHandler.getOppPlayers();

        if (compList[i].isAlive() == State.DIED)
        {
            Button btt = getButtonByTag(oppCharacters, i);

            if (btt != null)
            {
                btt.transform.localRotation = Quaternion.identity;


                btt.GetComponent<RectTransform>().transform.SetParent(OppDieTransform.GetChild(kill_Index).GetComponent<RectTransform>().transform);
                btt.transform.localPosition = Vector2.zero;


                CharacterDef characterDef = btt.GetComponent<GridInfo>().characterDef;
                if (characterDef == CharacterDef.PRESIDENT || characterDef == CharacterDef.ARMY)
                {
                    btt.transform.localScale = new Vector3(0.5f, 0.5f, 1);

                }

                Debug.Log("Capture::I have killed Opp Char:tag=" + btt.tag);
                int cx = 7 - btt.GetComponent<GridInfo>().x;
                int cy = 7 - btt.GetComponent<GridInfo>().y;
                int cColour = (int)btt.GetComponent<GridInfo>().colour;
                uint localId = RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw;

                RoomPlayer.Local.RPC_SyncDied(localId, cx, cy, (int)cColour, kill_Index);

                kill_Index++;
            }
        }
    }*/

    //public int GetOppCaptureIndex()
    ///{
    //    return capute_Index2;
    //}

    //int capute_Index2 = 0;
    //int kill_Index2 = 0;
    public void UpdateOpponentPowAndDieList(bool isSync = false, bool isForceAdd = false)
    {
        //int capute_Index2 = 0;
       // int kill_Index2 = 0;
        List<Character> compList = playerHandler.getOppPlayers();
        for (int i = 0; i < compList.Count(); i++)
        {
            if (compList[i].isAlive() == State.CAPTURE && !compList[i].GetIsCaptured())
            {
                Button btt = getButtonByTag(oppCharacters, i);
                if (btt != null)
                {
                    btt.transform.localRotation = Quaternion.identity;
                    Debug.Log("@@Opp's updating pow and die list::" + compList[i].getName());

                    //if (!isForceAdd)
                    //{

                    
                        btt.transform.SetParent(PlayerPowTransform/*.GetChild(capute_Index2).GetComponent<RectTransform>().transform*/);
                        btt.transform.localPosition = Vector2.zero;

                    btt.GetComponent<DragAndDropHandler>().enabled = false;
                    DragAndDropHandler dragAndDropHandler = btt.GetComponent<DragAndDropHandler>();
                    if (dragAndDropHandler)
                    {
                        dragAndDropHandler.enabled = false;
                    }

                    //}
                    //else
                    //{
                    //    btt/*.GetComponent<RectTransform>()*/.transform.SetParent(OppPowTransform/*.GetChild(capute_Index2).GetComponent<RectTransform>().transform*/);
                    //    btt.transform.localPosition = Vector2.zero;
                    //}



                    CharacterDef characterDef = btt.GetComponent<GridInfo>().characterDef;
                    if (characterDef == CharacterDef.PRESIDENT || characterDef == CharacterDef.ARMY)
                    {
                        //btt.transform.localScale = new Vector3(0.5f, 0.5f, 1);

                    }

                    if (!isSync)
                    {
                        Debug.Log("@@Capture::I have captured Opp Char:tag=" + btt.tag);
                        int cx = 7 - btt.GetComponent<GridInfo>().x;
                        int cy = 7 - btt.GetComponent<GridInfo>().y;
                        int cColour = (int)btt.GetComponent<GridInfo>().colour;
                        uint localId = RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw;

                        RoomPlayer.Local.RPC_SyncCaptured(localId, cx, cy, (int)cColour, isForceAdd, false);

                    }
                    //capute_Index2++;
                    compList[i].SetIsCaptured(true);
                }
            }
            /*else if (compList[i].isAlive() == State.DIED)
            {
                Button btt = getButtonByTag(oppCharacters, i);
                if (btt != null)
                {
                    btt.transform.localRotation = Quaternion.identity;

                    

                    btt.GetComponent<RectTransform>().transform.SetParent(PlayerDieTransform.GetChild(kill_Index2).GetComponent<RectTransform>().transform);
                    btt.transform.localPosition = Vector2.zero;


                    CharacterDef characterDef = btt.GetComponent<GridInfo>().characterDef;
                    if (characterDef == CharacterDef.PRESIDENT || characterDef == CharacterDef.ARMY)
                    {
                        btt.transform.localScale = new Vector3(0.5f, 0.5f, 1);

                    }
                    if (!isSync)
                    {
                        Debug.Log("@@Capture::I have killed Opp Char:tag=" + btt.tag);
                        int cx = 7 - btt.GetComponent<GridInfo>().x;
                        int cy = 7 - btt.GetComponent<GridInfo>().y;
                        int cColour = (int)btt.GetComponent<GridInfo>().colour;
                        uint localId = RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw;

                        RoomPlayer.Local.RPC_SyncDied(localId, true, cx, cy, (int)cColour);

                    }
                    kill_Index2++;

                }
            }*/
        }
    }

    static int oppcapute_Index = 0;
    public void SyncCaptured(uint id, int cx, int cy, int cColour, bool isForceAdd, bool isChip=false)
    {
        Transform powTransform = OppPowTransform;
        Debug.Log("SyncCaptured--");
        if (isForceAdd)
        {
            Debug.Log("SyncCaptured--2");
            powTransform = PlayerPowTransform;
        }


       /* if (RoomPlayer.gpMode != 0)
        {
            if (isMyTeam(id))
            {
                Debug.Log("Sync captured to my team");

                cx = 7 - cx;
                cy = 7 - cy;
                powTransform = PlayerPowTransform;
            }
        }*/

        Button btt = GetButtonToSync(id, new Char_Point(cx, cy, (colour)cColour), isChip); 
        if (btt != null)
        {

            btt/*.GetComponent<RectTransform>()*/.transform.SetParent(powTransform/*.GetChild(oppcapute_Index).GetComponent<RectTransform>().transform*/);
            btt.transform.localPosition = Vector2.zero;

            btt.GetComponent<GridInfo>().isAlive = (int)State.CAPTURE;
            btt.gameObject.SetActive(true);

            Debug.Log("Capture::My Captured Char:tag=" + btt.tag);
            DragAndDropHandler dragAndDropHandler = btt.GetComponent<DragAndDropHandler>();
            if (dragAndDropHandler)
            {
                dragAndDropHandler.enabled = false;
            }
            /*List<Character> oppChars = playerHandler.getOppPlayers();
            for (int i = 0; i < .Count(); i++)
            {
                if(oppChars[i].
            }*/

            CharacterDef characterDef = btt.GetComponent<GridInfo>().characterDef;
            if (characterDef == CharacterDef.PRESIDENT || characterDef == CharacterDef.ARMY)
            {
                btt.transform.localScale = new Vector3(0.5f, 0.5f, 1);

            }

            if (currMyCiv == (int)(GameConstants.Civilisation.French))
            {
                if (characterDef == CharacterDef.COASTGUARD)
                {

                    myPawnCount -= 1;

                    Debug.Log("Sync Pawn Captured!!" + myPawnCount);


                    if (myPawnCount <= 0)
                    {
                        _gameState = MultiplayerGameState.OPP_WIN;
                        StartCoroutine(ShowGameOverCort());
                    }
                }
            }
            else if (currMyCiv == (int)(GameConstants.Civilisation.Spanish))
            {
                if (characterDef == CharacterDef.NAVY)
                {

                    myKnightCount -= 1;

                    Debug.Log("Sync Knight Captured!!" + myKnightCount);




                    if (myKnightCount <= 0)
                    {
                        _gameState = MultiplayerGameState.OPP_WIN;
                        StartCoroutine(ShowGameOverCort());
                    }
                }
            }

            oppcapute_Index += 1;
        }

        playerHandler.updatePoint();


    }

    static int opp_kill_Index = 0;
    public void SyncKilled(uint id, bool isOpp, int cx, int cy, int cColour, bool isChip = false)
    {
        Transform dieTransform = PlayerDieTransform;
        if(isOpp)
        {
            dieTransform = OppDieTransform;
        }

        if (RoomPlayer.gpMode != 0)
        {
            if (isMyTeam(id))
            {
                Debug.Log("Sync killed to my team");
                cx = 7 - cx;
                cy = 7 - cy;
                dieTransform = OppDieTransform;
                if (isOpp)
                {
                    dieTransform = PlayerDieTransform;
                }
            }
        }

        Button btt = GetButtonToSync(id, new Char_Point(cx, cy, (colour)cColour), isChip);
        if (btt != null)
        {

            

            btt.GetComponent<RectTransform>().transform.SetParent(dieTransform.GetChild(opp_kill_Index).GetComponent<RectTransform>().transform);
            btt.transform.localPosition = Vector2.zero;

            btt.GetComponent<GridInfo>().isAlive = (int)State.DIED ;
            btt.gameObject.SetActive(true);
            Debug.Log("Capture::My Killed Char:tag=" + btt.tag);


            CharacterDef characterDef = btt.GetComponent<GridInfo>().characterDef;
            if (characterDef == CharacterDef.PRESIDENT || characterDef == CharacterDef.ARMY)
            {
                btt.transform.localScale = new Vector3(0.5f, 0.5f, 1);

            }

            opp_kill_Index += 1;
        }
        else
        {
            Debug.Log("Could not Find Button To Sync!!");
        }

        playerHandler.updatePoint();

        //updatePowAndDieList(true);
    }

    /*private void cleanUpPowDieButton()
    {
        foreach (Transform p in PlayerPowTransform)
        {
            RemoveAllFromButton(p);
        }
        foreach (Transform p in OppPowTransform)
        {
            RemoveAllFromButton(p);
        }
        foreach (Transform p in PlayerDieTransform)
        {
            RemoveAllFromButton(p);
        }

        foreach (Transform p in OppDieTransform)
        {
            RemoveAllFromButton(p);
        }
    }*/

    /*private void RemoveAllFromButton(Transform trans)
    {
        foreach (Transform child in trans)
        {
            child.gameObject.SetActive(false);
            //Destroy(child.gameObject);
        }
    }*/

    public bool moveButtSelectToNewPos(Button moveTo, Button buttSelect, int whichOne, bool isChip=false)
    {
        foreach (Char_Point movePos in posSelectionToMove)
        {
            int indx = movePos.index();
            if (int.Parse(moveTo.tag) == indx && buttSelect)
            {
                updatePosStatus moveStt = playerHandler.updatePosition(movePos, whichOne, int.Parse(buttSelect.tag));
                if (!moveStt.isValid)
                {
                    buttSelect.GetComponent<DragAndDropHandler>().ResetHighligts();
                    buttSelect.GetComponent<DragAndDropHandler>().ResetPlace();
                    return false;
                }

                Debug.Log("Move Pos Status Valid");

                removePlayersByTags(moveStt.removePlayers, moveTo, whichOne);

                /*int chipTag = (int)GameConstants.PlayerTags.USER_CHIP_TAG;
                if (whichOne == 0)
                {
                    chipTag = (int)GameConstants.PlayerTags.COMP_CHIP_TAG;
                }*/

                /*if (int.Parse(buttSelected.tag) >= chipTag && moveStt.removePlayers.Count() > 0)
                {
                    StartCoroutine(ShowExplosion(moveTo));

                    SoundManager.Instance.PlayEffectFor(SoundEffect.Projectile_Kill);

                    int cx, cy, ccolour;
                    uint localId;
                    if (playerHandler.needPermanentRemove(int.Parse(buttSelected.tag)))
                    {
                        //buttSelected.transform.parent = null ;
                        //here i should remove it as well

                        cx = 7 - buttSelected.GetComponent<GridInfo>().x;
                        cy = 7 - buttSelected.GetComponent<GridInfo>().y;
                        ccolour = (int)buttSelected.GetComponent<GridInfo>().colour;
                        localId = RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw;
                        RoomPlayer.Local.RPC_SetRemoval(localId, cx, cy, ccolour, true);

                        buttSelected.gameObject.SetActive(false);


                        //Destroy(buttSelected.gameObject);
                    }

                    cx = 7 - buttSelected.GetComponent<GridInfo>().x;
                    cy = 7 - buttSelected.GetComponent<GridInfo>().y;
                    ccolour = (int)buttSelected.GetComponent<GridInfo>().colour;
                    localId = RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw;
                    RoomPlayer.Local.RPC_SetRemoval(localId, cx, cy, ccolour, true);
                    buttSelected.gameObject.SetActive(false);




                }
                else
                {*/
                    if (GameObject.FindObjectOfType<MultiplayerGamePlay>().currMyCiv
                        == (int)GameConstants.Civilisation.Britons && moveStt.removePlayers.Count() > 0 &&
                        buttSelect.GetComponent<GridInfo>().characterDef == CharacterDef.COASTGUARD)
                    {
                        buttSelect.GetComponent<DragAndDropHandler>().ResetPlace();
                    }
                    else
                    {
                        movePlayerToNewPos(buttSelect, movePos, isChip);
                    }
                    if (moveStt.removePlayers.Count() > 0)
                        SoundManager.Instance.PlayEffectFor(SoundEffect.Projectile_Kill);
                    else
                        SoundManager.Instance.PlayEffectFor(SoundEffect.Move_Play);

                    foreach (Character p in playerHandler.getCastleMove())
                    {
                        Button marine = null;
                        Button moveToBtt;
                        indexPlayer tag = playerHandler.getindexByCharacter(p);


                        marine = getButtonByTag(userCharacters, tag.index);

                        movePlayerToNewPos(marine, p.getPosition(), isChip);
                    }

                    if (_state == MultiplayerPlayState.USER_MOVE || _state == MultiplayerPlayState.OPP_MOVE)
                    {
                        //colour_rescue1.Clear();//MA
                        colour_rescue1 = playerHandler.colourToRescueCapturePlayer();
                        if (colour_rescue1.Count() > 0)
                        {
                            if (_state == MultiplayerPlayState.USER_MOVE)
                                _state = MultiplayerPlayState.USER_WAIT_PROMOTE;
                            //else if (_state == MultiplayerPlayState.OPP_MOVE)
                            //_state = MultiplayerPlayState.OPP_WAIT_PROMOTE;
                        }
                        
                    }
                    else
                    {
                        playerHandler.cleanUpResuePostion();
                    }

                //}
                charHandler.RemoveHighLightColour();
                break;
            }
        }
        return true;
    }

    

    private IEnumerator ShowExplosion(Button moveTo)
    {
        moveTo.transform.Find("blast").gameObject.SetActive(true);
        SoundManager.Instance.PlayEffectFor(SoundEffect.Explosion);

        yield return new WaitForSeconds(2f);
        moveTo.transform.Find("blast").gameObject.SetActive(false);

    }

    private void movePlayerToNewPos(Button player, Char_Point movTo, bool isChip=false)
    {
        Button newParent = allBoardRect.ElementAt<Button>(movTo.index());
        
        
        RectTransform buttonRect = player.GetComponent<RectTransform>();

        //uint localId = RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw;
        //RoomPlayer.Local.RPC_ColorTheSelected(localId, posSelectionToMove);

        buttonRect.transform.parent = null;

        buttonRect.transform.parent = newParent.transform;

       buttonRect.transform.localPosition = charHandler.GetPosInRectFromCharPoint(newParent, movTo).localPosition;
        buttonRect.transform.localScale = Vector2.one;

       

        //Debug.Log("MA::move to new position::"+ int.Parse(buttSelected.tag));
        if (int.Parse(buttonRect.tag) >= (int)GameConstants.PlayerTags.USER_CHIP_TAG)
        {
            buttonRect.localRotation = Quaternion.Euler(0f, 0f, 0);

        }

        Debug.Log(movTo.x+":Move me to pos" + movTo.y);

        int px = 7 - movTo.x;
        int py = 7 - movTo.y;
        int pColour = (int)movTo.colour;

        int cx = 7 - player.GetComponent<GridInfo>().x;
        int cy = 7 - player.GetComponent<GridInfo>().y;
        int cColour = (int)player.GetComponent<GridInfo>().colour;
        uint localId = RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw;

        bool isRescue = false;
        if(_state == MultiplayerPlayState.USER_WAIT_PROMOTE)
        {
            isRescue = true;
        }


        //For Burgendy
        if (currMyCiv == (int)Civilisation.Burgendy)
        {
            if (movTo.y > 4 && movTo.y < 6 && player.GetComponent<GridInfo>().characterDef == CharacterDef.COASTGUARD)
            {
                Debug.Log("->Move Coast Guard To 6 Is Promoted True");

                /*player.GetComponent<GridInfo>().isPromoted = true;
                if (RoomPlayer.Local.IsLeader)
                {
                    player.image.sprite = charHandler.NavyW.sprite;
                }
                else
                {
                    player.image.sprite = charHandler.NavyB.sprite;
                }*/

                if (player.GetComponent<GridInfo>().isPromoted == 0)
                {
                    player.GetComponent<GridInfo>().ShowPromotionOption(currMyCiv, true, true);
                }
            }
            else if (movTo.y > 6 && movTo.y < 8 && player.GetComponent<GridInfo>().characterDef == CharacterDef.COASTGUARD)
            {
                if (player.GetComponent<GridInfo>().isPromoted1 == 0)
                {
                    player.GetComponent<GridInfo>().ShowPromotionOption(currMyCiv, true, false);
                }
            }
        }
        else
        {
            if (movTo.y > 6 && movTo.y < 8 && player.GetComponent<GridInfo>().characterDef == CharacterDef.COASTGUARD)
            {
                if (player.GetComponent<GridInfo>().isPromoted1 == 0)
                {
                    player.GetComponent<GridInfo>().ShowPromotionOption(currMyCiv, false, false);
                }
            }

           

            
        }

        

        ////////////////asdf
        player.GetComponent<GridInfo>().x = movTo.x;
        player.GetComponent<GridInfo>().y = movTo.y;

        RoomPlayer.Local.RPC_SetMovement(
            localId, cx, cy, cColour, px, py, pColour, isChip, isRescue, -1);

    }

   

    private void removePlayersByTags(List<int> tags, Button moveTo, int whichOne)
    {
        

        foreach (int tag in tags)
        {
            Debug.Log(" REMOVE PLAYERS NY TAGS::"+tag);

            if (tag >= (int)GameConstants.PlayerTags.COMP_CHIP_TAG)
            {
                Debug.Log("Its a Chip::111");
                foreach (Button p in oppChips)
                {
                    try
                    {
                        if (int.Parse(p.tag) == tag)
                        {

                            Debug.Log("Tag Matched::111");

                            if (!playerHandler.needPermanentRemove(tag) && whichOne == 0) //computer
                            {
                                Debug.Log("Returing to board 1");
                            }
                            else
                            {
                                Debug.Log(" REMOVE CHIP ::111::" + p.tag);


                                int cx = 7 - p.GetComponent<GridInfo>().x;
                                int cy = 7 - p.GetComponent<GridInfo>().y;
                                int ccolour = (int)p.GetComponent<GridInfo>().colour;
                                uint localId = RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw;

                                RoomPlayer.Local.RPC_SetRemoval(localId, cx, cy, ccolour, true, true);
                                p.gameObject.SetActive(false);


                            }

                            break;
                        }
                    }
                    catch(Exception e) { }
    
                }
            }
            else if (tag >= (int)GameConstants.PlayerTags.USER_CHIP_TAG)
            {
                Debug.Log("Its a Chip::222");

                foreach (Button p in userChips)
                {
                    try
                    {
                        if (int.Parse(p.tag) == tag)
                        {

                            Debug.Log("Tag is matched::222");

                            if (!playerHandler.needPermanentRemove(tag) && whichOne > 0)
                            {
                                Debug.Log("Returing to board 2");

                            }
                            else
                            {
                                Debug.Log(" REMOVE CHIP ::222::" + p.tag);

                                int cx = 7 - p.GetComponent<GridInfo>().x;
                                int cy = 7 - p.GetComponent<GridInfo>().y;
                                int ccolour = (int)p.GetComponent<GridInfo>().colour;
                                uint localId = RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw;

                                RoomPlayer.Local.RPC_SetRemoval(localId, cx, cy, ccolour, true, true);

                                p.gameObject.SetActive(false);

                                //Destroy(p);
                            }

                        break;
                    }
                    }
                    catch (Exception e) { }

                }
            }
            else
            {

                Debug.Log("ITS a character");

                foreach (Transform gameObjectT in moveTo.transform)
                {
                    string name = gameObjectT.name;

                    Debug.Log("DESTROYING:::"+name);

                    //gameobject has other child for transform's highlights etc positions purpose so we only need actual child
                    //i.e character or chip

                    if (!name.Equals("b") && !name.Equals("w") && !name.Equals("bw") &&
                        !name.Equals("bt") && !name.Equals("wt") && !name.Equals("bwt") && !name.Equals("blast"))
                    { 
                        if (int.Parse(gameObjectT.gameObject.tag) == tag)
                        {
                            Debug.Log(" REMOVE PLAYERS ::444::" + tag);
                            
                        }
                    }

                }
            }

        }
    }

    public void SynSetReturnToBoard(uint id, int cx, int cy, int ccolour, CharacterDef characterDef)
    {
        if (RoomPlayer.gpMode != 0)
        {
            if (isMyTeam(id))
            {
                Debug.Log("Sync removal to my team");

                cx = 7 - cx;
                cy = 7 - cy;
            }
        }

        Button btt1 = GetButtonToSync(id, new Char_Point(cx, cy, (colour)ccolour), true);




        // Return the chip to the board
        float rotate = 0;

        btt1.transform.parent = null;


        //chip.getPosition();
        Char_Point pos = new Char_Point();
        pos.x = btt1.GetComponent<GridInfo>().x;
        pos.y = btt1.GetComponent<GridInfo>().y;
        pos.colour = btt1.GetComponent<GridInfo>().colour;
        //chip.ResetInitPositionTo(pos);






    }

    public void SynSetRemoval(uint id, int cx, int cy, int ccolour, bool isChip, bool isMine = false)
    {
        Debug.Log("Receiving and syncing  Removal::isMine::" + isMine);

        if (RoomPlayer.gpMode != 0)
        {
            if (isMyTeam(id))
            {
                Debug.Log("Sync removal to my team");

                cx = 7 - cx;
                cy = 7 - cy;
            }
        }

        Button btt1 = GetButtonToSync(id, new Char_Point(cx, cy, (colour)ccolour), isChip, false, isMine);

        Debug.Log("MA::Sync Set Chip Removal Opp::"+cx+","+cy+","+ccolour+","+isChip);
        /*Button btt1 = getButtonByTag(oppCharacters, int.Parse(tag));
        if (btt1 != null)
        {*/

        btt1.gameObject.SetActive(false);
        //}



    }

    public bool isMyTeam(uint id)
    {
        bool isMyTeam = false;
        uint localId = RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw;
        if (RoomPlayer.gpMode == 1)
        {
            if ((localId == 3 && id == 4) || (id == 3 && localId == 4))
            {
                isMyTeam = true;
            }


        }
        else
        {
            Debug.Log("gpMode:"+RoomPlayer.gpMode+",Is my team::id::" + id + ",local Id::" + localId);
            if ((localId == 2 && id == 4) || (id == 2 && localId == 4))
            {
                    isMyTeam = true;
            }
            else
            if ((localId == 3 && id == 5) || (id == 3 && localId == 5))
            {
                    isMyTeam = true;
            }
        }

        return isMyTeam;
    }

    public bool GetIsRotated()
    {
        return isRotated;
    }

    public void UpdatePoint(float u_point, float comp_point, float comp2_point=0, float comp3_point = 0)
    {
        Debug.Log("UpdatePoint::" + u_point+"::"+comp_point);

        if (gameMode == 0)
        {
            //playerPoints.text = u_point.ToString();
            //oppPoints.text = comp_point.ToString();

            RoomPlayer.Local.SetPointsText(u_point.ToString());
            RoomPlayer.Local.RPC_SendPoints(u_point);

        }
        
    }

    int i = 0;
    GameObject gameObjectmove = null;
    GameObject gameObjectmove2 = null;
    GameObject gameObjectmove3 = null;

    int moveCount = 1;

    void UpdateMovingStep(string txt)
    {
        Debug.Log("UpdateMovingStep::" + txt);

        //int gameMode = PlayerPrefs.GetInt("gameMode", 0);
        if (gameMode == 0)
        {
            //if (i == 0)
            //{
                gameObjectmove = new GameObject();
                gameObjectmove = Instantiate(moveEnteryPrefab);
                gameObjectmove.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = txt;
                gameObjectmove.transform.SetParent(moveContents);
                gameObjectmove.transform.localScale = Vector3.one;

                gameObjectmove2 = new GameObject();
                gameObjectmove2 = Instantiate(moveEnteryPrefab);
                gameObjectmove2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = txt;
                gameObjectmove2.transform.SetParent(moveContentsGO);
                gameObjectmove2.transform.localScale = Vector3.one;

            moveCount++;

            RoomPlayer.Local.RPC_SendMoveStep(txt);

              
        }
        else if (gameMode == 1)
        {
           
        }
    }

    public void UpdateMovingStepOpp(string txt)
    {
        gameObjectmove = new GameObject();

        gameObjectmove = Instantiate(moveEnteryPrefab);
        
        gameObjectmove.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = txt;
        gameObjectmove.transform.SetParent(moveContents);
        gameObjectmove.transform.localScale = Vector3.one;


        gameObjectmove2 = new GameObject();
        gameObjectmove2 = Instantiate(moveEnteryPrefab);
        
        gameObjectmove2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = txt;
        gameObjectmove2.transform.SetParent(moveContentsGO);
        gameObjectmove2.transform.localScale = Vector3.one;


    }

 

    
}
