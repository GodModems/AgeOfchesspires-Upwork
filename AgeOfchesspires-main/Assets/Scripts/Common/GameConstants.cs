using UnityEngine;

public class GameConstants
{
  

    public const int HIGHT_LIGHT_TAG = 200;


    public enum PlayerTags
    {
        
        MENU_BUTTON = 501,
        USER_ICON = 502,
        COMPUTER_ICON = 503,
        VS_ICON = 504,
        MENU_HOME = 505,
        MENU_RULE = 506,
        MENU_SETTING = 507,
        MENU_ABOUT = 508,
        USER_CHIP_TAG = 600,
        COMP_CHIP_TAG = 700,
        MENU_VIDEO = 701,
        MENU_QUIT = 702,
        MENU_RESUME = 703
    }

    public enum AlertType
    {
        AlertType_OkAndCancel = 0,
        AlertType_OkOnly,
        AlertType_CancelOnly,
        AlertType_None,
    };
    public enum AlertButton
    {
        Back_Game_tag = 0,
    };

    public enum SoundEffect
    {
        Button_Click = 0,
        Dice_Click,
        Explosion,
        Move_Play,
        Projectile_Kill
    };

    public enum GameSidePanelBtns
    {
        Home = 0,
        Rules,
        Settings,
        AboutGame,
        GameVdo,
        Resume,
        QuitGame
    };

    public enum GamePlayMode
    {
        OneVOne      
    }

    public enum MultiplayerGamePlayMode
    {
        Host = 4,
        Client = 5,
        AutoHostAndClient = 6
    }

    public enum Civilisation
    {
        Teutons = 0,
        Vikings,
        Britons,
        French,
        Burgendy,
        Spanish,
        Egyptian,
        Chinese,
        Huns,
        Haitians
    }

    public static GameObject FindChildWithTag(GameObject parent, string tag)
    {
        GameObject child = null;
        foreach (Transform transform in parent.transform)
        {
            if (transform.CompareTag(tag))
            {
                child = transform.gameObject;
                break;
            }
        }
        return child;
    }
}