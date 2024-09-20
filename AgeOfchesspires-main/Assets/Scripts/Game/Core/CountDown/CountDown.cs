using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI countdowns;
    [SerializeField]
    private Image loadProgress;

    float progress = 0;
    //private Coroutine timerCort;
    private int count;

    /*private void Reset()
    {
        count = 0;
        timerCort = null;
    }*/

    private void OnEnable()
    {
        count = 0;
        if (PlayerPrefs.GetInt("isFirstRun", 0) == 0)
        {
            //PlayerPrefs.SetInt("isFirstRun", 1);
            //PlayerPrefs.Save();
            //StartCountDown();
        }
        else
        {
            ShowDiceRoll();
        }
    }


    public void StartCountDown()
    {
        //timerCort = 
            StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        count = 3;
        while (count > -1)
        {
            countdowns.text = count.ToString();
            yield return new WaitForSeconds(1);
            count--;
            progress+=0.34f;
            loadProgress.fillAmount = progress;

            SoundManager.Instance.PlayEffectFor(GameConstants.SoundEffect.Button_Click);

            Debug.Log("count" + count);
        }

        ShowDiceRoll();

    }

    public void ShowDiceRoll()
    {
        

        FindObjectOfType<UIManager>().ShowDiceRollPanel();
        gameObject.SetActive(false);

    }

    public void SkipDiceRoll()
    {
        gameObject.SetActive(false);

        int playerVal = Random.Range(1, 7);
        int opponentVal = Random.Range(1, 7);

        GameObject.FindWithTag("GameController").GetComponent<GameController>().UpdateDiceValues(playerVal, opponentVal);

    }

}
