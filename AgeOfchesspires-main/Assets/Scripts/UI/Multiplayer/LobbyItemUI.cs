using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyItemUI : MonoBehaviour {

    public TextMeshProUGUI username;
    public TextMeshProUGUI waitingTxt;
    public Image playerDp;
    public Image ready;

    private RoomPlayer _player;

    public void SetPlayer(RoomPlayer player) {
        _player = player;
        waitingTxt.text = "Waiting...";
        //playerDp.sprite = FindObjectOfType<PlayfabManager>().GetSelectedDp();
    }

    private void Update() {
        if (_player.Object != null && _player.Object.IsValid)
        {
            username.text = _player.Username.Value;
            ready.gameObject.SetActive(_player.IsReady);
            if(_player.IsReady )
            {
                waitingTxt.text = "Ready";
            }
        }
    }
}