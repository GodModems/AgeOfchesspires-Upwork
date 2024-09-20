using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject soundStateOn;
    [SerializeField]
    private GameObject soundStateOff;
    [SerializeField]
    private GameObject musicStateOn;
    [SerializeField]
    private GameObject musicStateOff;

    [SerializeField]
    private GameObject popup;

    private int option = 0;

    // Start is called before the first frame update

    void Start()
    {


      

       

        UpdateUI();


    }

    public GameObject GetSettingsPopup(int option)
    {
        this.option = option;
        return popup;
    }

    void OnEnable()
    {
        LeanTween.scale(popup, new Vector3(1, 1, 1), 1.5f).setEaseInOutBounce();

    }

    public void UpdateUI()
    {
        int musicEnabled = PlayerPrefs.GetInt("isMusicOn", 1);
        if (musicEnabled == 1)
        {
            musicStateOn.SetActive(true);
            musicStateOff.SetActive(false);
        }
        else
        {
            musicStateOn.SetActive(false);
            musicStateOff.SetActive(true);

        }

        int soundEnbaled = PlayerPrefs.GetInt("isSoundOn", 1);
        if (soundEnbaled == 1)
        {
            soundStateOn.SetActive(true);
            soundStateOff.SetActive(false);
        }
        else
        {
            soundStateOn.SetActive(false);
            soundStateOff.SetActive(true);

        }

    }

    public void OnMusicToggle()
    {
        SoundManager.Instance.PlayEffectFor(GameConstants.SoundEffect.Button_Click);
        int musicEnabled = PlayerPrefs.GetInt("isMusicOn", 1);
        bool music = false;
        if (musicEnabled == 1) { 
            musicStateOn.SetActive(false);
            musicStateOff.SetActive(true);
            LeanTween.scale(musicStateOff, new Vector3(1.5f, 1.5f, 1.1f), 0.5f).setEaseInOutBounce();
            music = false;
            SoundManager.Instance.SetMusic(music);
        }
        else
        {
            musicStateOn.SetActive(true);
            musicStateOff.SetActive(false);
            LeanTween.scale(musicStateOn, new Vector3(1.5f, 1.5f, 1.1f), 0.5f).setEaseInOutBounce();
            music = true;
            SoundManager.Instance.SetMusic(music);

        }

        LeanTween.delayedCall(0.25f, 
            ()=>LeanTween.scale(musicStateOn, new Vector3(1, 1, 1), 0.5f).setEaseInOutBounce()
            ) ;
        LeanTween.delayedCall(0.25f,
           () => LeanTween.scale(musicStateOff, new Vector3(1, 1, 1), 0.5f).setEaseInOutBounce()
           );

        PlayerPrefs.SetInt("isMusicOn", music ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OnSoundToggle()
    {
        SoundManager.Instance.PlayEffectFor(GameConstants.SoundEffect.Button_Click);
        int soundEnbaled = PlayerPrefs.GetInt("isSoundOn", 1);
        bool sound = false;
        if (soundEnbaled==1)
        {
            soundStateOn.SetActive(false);
            soundStateOff.SetActive(true);
            LeanTween.scale(soundStateOff, new Vector3(1.5f, 1.5f, 1.1f), 0.5f).setEaseInOutBounce();
            sound = false;
            SoundManager.Instance.SetSound(sound);
        }
        else
        {
            soundStateOn.SetActive(true);
            soundStateOff.SetActive(false);
            LeanTween.scale(soundStateOn, new Vector3(1.5f, 1.5f, 1.1f), 0.5f).setEaseInOutBounce();
            sound = true;
            SoundManager.Instance.SetSound(sound);

        }

        LeanTween.delayedCall(0.25f,
           () => LeanTween.scale(soundStateOn, new Vector3(1, 1, 1), 0.5f).setEaseInOutBounce()
           );
        LeanTween.delayedCall(0.25f,
          () => LeanTween.scale(soundStateOff, new Vector3(1, 1, 1), 0.5f).setEaseInOutBounce()
          );

        PlayerPrefs.SetInt("isSoundOn", sound ? 1 : 0);
        PlayerPrefs.Save();


    }

    public void OnBackPressed() {
        SoundManager.Instance.PlayEffectFor(GameConstants.SoundEffect.Button_Click);



        gameObject.SetActive(false);
    }

     void OnDisable()
    {
        LeanTween.scale(popup, new Vector3(0f, 0f, 1), 1.5f).setEaseInOutBounce();

    }

}
