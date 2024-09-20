using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomEntry : MonoBehaviour
{
    private string roomName;
    private string roomId;
    private string playerCount;
    private bool isPriv;
    private int timer;
    private int isGOC;
    private string roomCode;
    private int gpMode;

    [SerializeField]
    private TextMeshProUGUI roomNameTxt;
    [SerializeField]
    private TextMeshProUGUI playerCountTxt;
    [SerializeField]
    private TextMeshProUGUI isPublicTxt;
    [SerializeField]
    private TextMeshProUGUI timerTxt;
    [SerializeField]
    private TextMeshProUGUI isGOCTxt;
    [SerializeField]
    private TMP_InputField roomCodeInput;
    [SerializeField]
    private Button joinBtn;
    [SerializeField]
    private TMP_Text errorTxt;

    public void SetRoomEntry(string roomName, string roomId, string playerCount, bool isPriv,int timer ,int isGOC, int gpMode, string roomCode="")
    {
        Debug.Log("Set Room Entry With Room Code::" + roomCode + ", and Room Id::" + roomId);

        this.roomId = roomId;
        this.roomName = roomName;
        roomNameTxt.text = roomName;
        this.playerCount = playerCount;
        playerCountTxt.text = playerCount;
        this.isPriv = isPriv;
        isPublicTxt.text = this.isPriv?"Private":"Public";
        this.timer = timer;
        timerTxt.text = timer.ToString();
        this.isGOC = isGOC;
        isGOCTxt.text = this.isGOC == 1? "Game Of Chance":"Standard";
        this.gpMode = gpMode;

        if(this.isPriv)
        {
            roomCodeInput.gameObject.SetActive(true);
            this.roomCode = roomCode;
        }
        else
        {
            roomCodeInput.gameObject.SetActive(false);
        }
    }

    public void OnJoinLobbyBtn()
    {
        Debug.Log("On Join Lobby With Room Code::" + roomCode+", and Room Id::"+roomId);

        bool isValid = true;
        if (this.isPriv) 
        {
            string roomCode = roomCodeInput.text;
            if(!roomCode.Equals(this.roomCode))
            {
                isValid = false;
                errorTxt.gameObject.SetActive(true);
            }
        }

        if(isValid)
        {
            errorTxt.gameObject.SetActive(false);
            FindObjectOfType<GameLauncher>().JoinALobby(roomId, timer, isGOC, gpMode, isPriv, roomCode);
            UIManager.Instance.OnGameRoomPanel();

        }

    }

    public void EnableJoinBtn()
    {
        joinBtn.interactable = true;
    }

    public void DisableJoinBtn()
    {
        joinBtn.interactable = false;
    }
}
