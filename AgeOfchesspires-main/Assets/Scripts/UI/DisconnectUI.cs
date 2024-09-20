using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisconnectUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI statusText;
    [SerializeField]
    private TextMeshProUGUI msgText;

    // Start is called before the first frame update
    void Start()
    {
        msgText.text = "";
    }

    public void ShowMessage(string status, string message)
    {
        statusText.text = status;
        msgText.text = message;
    }

    public void OnBackBtn()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            UIManager.Instance.OnPlayButton();

        }
        else
        {
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }
    }

}
