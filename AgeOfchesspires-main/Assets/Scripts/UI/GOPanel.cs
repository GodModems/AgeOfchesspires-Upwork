using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GOPanel : MonoBehaviour
{
    [SerializeField] private  GameObject buttonPlayAgain;
    [SerializeField] private TextMeshProUGUI playerScoreTxt;
    [SerializeField] private TextMeshProUGUI machineScoreTxt;
    [SerializeField] private TextMeshProUGUI wonTitleTxt;

    public GameObject UndoBtn;
    public GameObject MovesPanelGO;

    private string wonTitle;
    private string playerScores;
    private string opponentScores;

    // Start is called before the first frame update
    public void SetWonTitle(string wonTitle)
    {
        this.wonTitle = wonTitle;
    }
    public void SetPlayerScores(string playerScores)
    {
        this.playerScores = playerScores;
    }
    public void SetOpponentScores(string opponentScores)
    {
        this.opponentScores = opponentScores;
    }

    public void UpdateUI()
    {
        playerScoreTxt.text = playerScores;
        wonTitleTxt.text = wonTitle;    
        machineScoreTxt.text =opponentScores;
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void PlayAgainMultiplayer()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

}
