using Fusion;
using System.Collections;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public int _playerSelectedArmy=0;
    private int _oppoSelectedArmy;
    private int _playerSelectedChar;
    private int _oppoSelectedChar;

    private Sprite _boardSprite;

    [SerializeField]
    public GameObject counterDownObj;

    

    [SerializeField]
    public MultiplayerGamePlay multiplayerGamePlay;

    public bool isTossWon = false;

    public void OnEnable()
    {
        RoomPlayer.PlayerShowingArmySelect += EnsureAllPlayersNotArmySelected;
    }

    public void OnDisable()
    {
        RoomPlayer.PlayerShowingArmySelect -= EnsureAllPlayersNotArmySelected;
    }

    public void EnsureAllPlayersNotArmySelected(RoomPlayer lobbyPlayer)
    {
        if (IsAllArmySelected())
        {
            
        }
    }
    private static bool IsAllArmySelected()
    {
        int maxPlayersReq = PlayerPrefs.GetInt("S_MaxUsers", 2);

        return RoomPlayer.GetPlayers().Count == maxPlayersReq && RoomPlayer.GetPlayers().All(player => player.IsShowingArmySelection);
    }
    // Start is called before the first frame update
    void Start()
    {
        int sound = PlayerPrefs.GetInt("isSoundOn", 1);
        int music = PlayerPrefs.GetInt("isMusicOn", 1);
        SoundManager.Instance.SetSound(sound == 0 ? false : true);
        SoundManager.Instance.SetMusic(music == 0 ? false : true);

        _boardSprite = GetComponent<Sprite>();
        UIManager.Instance.InitGameUI();


        if (UIManager.Instance.IsAIMode()==1)
        {

            //commented for new chess
            //Debug.Log("Showing Countdown1");
            //ShowCountDownTimer();
        }
        else
        {
            SetUpBoard();

            //commented for new chess
            //Debug.Log("Showing Countdown2");
            //ShowCountDownTimer();

        }

        //commented for new chess
        //PlayerPrefs.SetInt("switchChar", 1);
        //PlayerPrefs.Save();

    }

    public void ShowCountDownTimer()
    {
        if (!counterDownObj)
        {
            counterDownObj = FindObjectOfType<CountDown>().gameObject;
        }
        counterDownObj.SetActive(true);

        if (PlayerPrefs.GetInt("isFirstRun", 0) == 0)
        {
            PlayerPrefs.SetInt("isFirstRun", 1);
            PlayerPrefs.Save();
            FindObjectOfType<CountDown>().StartCountDown();
        }
    }
    

    public void UpdateDiceValues(int player, int opponent)
    {

        Debug.Log(player+"GameController::"+opponent);
        UIManager.Instance.CloseDiceRollPanel();

        //TODO: show army selection and character selection

        if (UIManager.Instance.IsAIMode() == 1)
        {
            if (player > opponent)
            {
                Debug.Log("I Won the toss, Showing army Selection111111");

                isTossWon = true;
                PlayerPrefs.SetInt("playerWonToss", 1);
                PlayerPrefs.Save();

                //player starts first
                UIManager.Instance.ShowArmySelection();
            }
            else
            {
                Debug.Log("I Lost the toss, Showing waiting11111");

                isTossWon = false;
                PlayerPrefs.SetInt("playerWonToss", 0);
                PlayerPrefs.Save();

                ShowCharSelection();
            }
        }
        else
        {

            if (RoomPlayer.Local.IsLeader)
            {
                Debug.Log("I Won the toss, Showing army Selection111111");

                isTossWon = true;
                PlayerPrefs.SetInt("playerWonToss", 1);
                PlayerPrefs.Save();

                //player starts first
                UIManager.Instance.ShowArmySelection();
            }
            else
            {
                Debug.Log("I Lost the toss, Showing waiting11111");

                isTossWon = false;
                PlayerPrefs.SetInt("playerWonToss", 0);
                PlayerPrefs.Save();

                if (UIManager.Instance.IsAIMode() == 1)
                {
                    ShowCharSelection();
                }
                else
                {
                    UIManager.Instance.ShowWaitPanel();
                }
            }
        }
    }

    public void ShowCharSelection()
    {
        PlayerPrefs.SetInt("playerWonToss", 0);
        PlayerPrefs.Save();
        //opponenet starts first
        UIManager.Instance.ShowCharSelection();
    }

    public void SetPlayerArmySelected(int army) { 
        _playerSelectedArmy = army;

        PlayerPrefs.SetInt("PlayerArmySelected", _playerSelectedArmy);
        PlayerPrefs.Save();

        //UIManager.Instance.CloseArmySelection();
        //UIManager.Instance.ShowCharSelection();


    }

    

    public int GetPlayerSelectedArmy()
    {
        int playerArmy = PlayerPrefs.GetInt("PlayerArmySelected", _playerSelectedArmy);
        return playerArmy;
    }

    

    public void SetPlayerSelectedChar(int character)
    {
        Debug.Log("POS::Set Local Selected Char::" + character);
        _playerSelectedChar = character;
        if (UIManager.Instance.IsAIMode() == 1)
        {
            PlayerPrefs.SetInt("switchChar", _playerSelectedChar);
            PlayerPrefs.Save();
        }
        else
        {
            PlayerPrefs.SetInt("switchChar"+RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw, _playerSelectedChar);
            PlayerPrefs.Save();
        }
    }

    public void StopCountdownTimer()
    {
        counterDownObj.SetActive(false);
    }

    public int GetPlayerSelectedCharacter()
    {
        return _playerSelectedChar;
    }

    private void UpdateGameBoard()
    { 
        if(_playerSelectedArmy == 0)
        {
            StartCoroutine(RotateTo360());
        }
        else
        {
            StartCoroutine(RotateTo0());

        }
    }

    IEnumerator  RotateTo360()
    {
        yield return new WaitForSeconds(0.5f);
        transform.localRotation = Quaternion.Euler(0, 0, 360);
    }

    IEnumerator  RotateTo0()
    {
        yield return new WaitForSeconds(0.5f);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void SetUpBoard()
    {
         multiplayerGamePlay.SetupGameplay();
        
    }

   
}
