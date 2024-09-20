using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Alert : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI messageText;
    [SerializeField]
    private Button okBtn;
    [SerializeField] 
    private Button cancelBtn;
    [SerializeField]
    private TextMeshProUGUI okText;
    [SerializeField]
    private TextMeshProUGUI cancelText;

    private int _alertType;
    private int _alertTag;
    private string _alertMessage;

    [SerializeField]
    private GameObject bg;

   

    void OnDisable()
    {

    }

    public void SetAlertTypeOrTag(GameConstants.AlertType type, int tag)
    {
        _alertType = (int)type;
        _alertTag = tag;
    }

    public void SetMessage(string msg)
    {
        _alertMessage = msg;
    }

    public void UpdateUI() {
        messageText.text = _alertMessage;
        if(_alertType == (int)GameConstants.AlertType.AlertType_OkOnly)
        {
            cancelBtn.gameObject.SetActive(false);
        }
        else if (_alertType == (int)GameConstants.AlertType.AlertType_CancelOnly)
        {
            okBtn.gameObject.SetActive(false);  
        }


    }

    public void OkBtnCallback() 
    {
        SoundManager.Instance.PlayEffectFor(GameConstants.SoundEffect.Button_Click);

        if (_alertTag == (int)GameConstants.AlertButton.Back_Game_tag) {

            Time.timeScale = 1f;

            FindObjectOfType<MultiplayerGamePlay>()._gameState = MultiplayerGameState.OPP_WIN;
            GameObject.FindObjectOfType<MultiplayerGamePlay>().ShowGameOverTemp(true, true);
            gameObject.SetActive(false);

            //EloRatingSystem eloRatingSystem = FindObjectOfType<EloRatingSystem>();
            //eloRatingSystem.CalculateEloRating(30, 0, currMyCiv, currOppCiv);

            //SceneManager.LoadScene("Main", LoadSceneMode.Single);

            //FindObjectOfType<UIManager>().showModelsPanel();

        }
    }

    public void CancelBtnCallback()
    {
        SoundManager.Instance.PlayEffectFor(GameConstants.SoundEffect.Button_Click);

        if (_alertTag == (int)GameConstants.AlertButton.Back_Game_tag)
        {
            gameObject.SetActive(false);
        }
    }
}
