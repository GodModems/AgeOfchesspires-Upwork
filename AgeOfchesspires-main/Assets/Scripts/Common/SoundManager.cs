using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource musicSource;
    [SerializeField]
    private AudioSource btnClickSrc;
    [SerializeField]
    private AudioSource diceClickSrc; 
    [SerializeField]
    private AudioSource explosionClickSrc;
    [SerializeField]
    private AudioSource moveClickSrc;
    [SerializeField]
    private AudioSource projClickSrc;

    public static SoundManager Instance { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }


    }

    public void Start()
    {
        int sound = PlayerPrefs.GetInt("isSoundOn", 1);
        int music = 0;// PlayerPrefs.GetInt("isMusicOn", 1);
        SetSound(sound == 0 ? false : true);
        SetMusic(music == 0 ? false : true);
    }

    public void SetMusic(bool flag) {
        if (flag)
        {
            musicSource.volume = 1;
        }
        else
        {
            musicSource.volume = 0;
        }
    }

    public void SetSound(bool flag)
    {
        if (flag)
        {
            btnClickSrc.volume = 1;
            diceClickSrc.volume = 1;
            explosionClickSrc.volume = 1;
            moveClickSrc.volume = 1;
            projClickSrc.volume = 1;
        }
        else
        {
            btnClickSrc.volume = 0;
            diceClickSrc.volume = 0;
            explosionClickSrc.volume = 0;
            moveClickSrc.volume = 0;
            projClickSrc.volume = 0;
        }
    }

    public void PlayEffectFor(GameConstants.SoundEffect soundEffect)
    {
        switch(soundEffect)
        {
            case GameConstants.SoundEffect.Button_Click:
                btnClickSrc.Play();
                break;
            case GameConstants.SoundEffect.Move_Play:
                moveClickSrc.Play();
                break;
            case GameConstants.SoundEffect.Projectile_Kill:
                projClickSrc.Play();
                break;
            case GameConstants.SoundEffect.Dice_Click:
                diceClickSrc.Play();
                break;
            case GameConstants.SoundEffect.Explosion:
                explosionClickSrc.Play();
                break;
        }
    }

}
