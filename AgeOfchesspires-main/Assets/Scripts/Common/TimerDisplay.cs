using System;
using System.Collections;
using TMPro;
using UnityEngine;  
  
public class TimerDisplay : MonoBehaviour
{
    private int countDownStartValue;
    public TextMeshProUGUI timerUI;
    private Coroutine timerCort;

  

    public void StartTimer(int time)
    {
        timerUI.text = "";
        countDownStartValue = time;// PlayerPrefs.GetInt("ActionTime", 0);
        timerCort = StartCoroutine(countDownTimer(countDownStartValue));
        timerUI.text = countDownStartValue + "sec";
    }

    IEnumerator countDownTimer(int time)
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            Debug.Log("Count Down::" + time);
            Debug.Log("Timer::" + timerUI.text);

            //TimeSpan spanTime = TimeSpan.FromSeconds(countDownStartValue);

            time -= 1;
            timerUI.text = "0:"+time;
            if(time <= 0)
            {
                StopCoroutine(timerCort);
                break;
            }

        }

        SendTurn();
        yield return null;
    }

    private void SendTurn()
    {
        FindObjectOfType<MultiplayerGamePlay>().PassTurn();
        
    }
}