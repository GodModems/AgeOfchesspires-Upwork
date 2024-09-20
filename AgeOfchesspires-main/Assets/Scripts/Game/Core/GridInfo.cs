using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using static GameConstants;

public class GridInfo : MonoBehaviour/*, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler*/
{
    public CharacterDef characterDef;
    public int x;
    public int y;
    public colour colour;
    public bool isEmpty = true;
    public int isAlive = 1;
    public string imageName = "";
    //public bool isFirstmoveDone = false;
    public int isPromoted = 0;
    public int isPromoted1 = 0;
    private bool isBurgundy = false;
    //promotion related
    public GameObject promotionPanel6th;
    public GameObject promotionPanel8th;

    [SerializeField]
    private List<GameObject> promoIcons;

    public void ShowPromotionOption(int myCiv, bool isBurgundy, bool is6th)
    {
        this.isBurgundy = isBurgundy;
        if (isBurgundy && is6th)
        {
            promotionPanel6th.SetActive(true);
        }
        else
        {
            promotionPanel8th.SetActive(true);
        }

        UpdatePromotionUI(myCiv);
    }

    public void UpdatePromotionUI(int civ)
    {
        ResetPromoIcons();
            if (civ == (int)Civilisation.Teutons)
            {
            promoIcons[4].SetActive(false);
            promoIcons[5].SetActive(false);

        }
            else if (civ == (int)Civilisation.Vikings)
            {
            promoIcons[4].SetActive(false);
            promoIcons[5].SetActive(false);
        }
            else if (civ == (int)Civilisation.Britons)
            {
            promoIcons[4].SetActive(false);
            promoIcons[5].SetActive(false);
        }
            if (civ == (int)Civilisation.French)
            {
            promoIcons[4].SetActive(false);
            promoIcons[5].SetActive(false);
            promoIcons[3].SetActive(false);
        }
            else if (civ == (int)Civilisation.Burgendy)
            {
            promoIcons[4].SetActive(false);
            promoIcons[5].SetActive(false);
        }
            else if (civ == (int)Civilisation.Spanish)
            {
            promoIcons[3].SetActive(false);
            promoIcons[4].SetActive(false);
            promoIcons[5].SetActive(false);
        }
            else if (civ == (int)Civilisation.Egyptian)
            {
            promoIcons[0].SetActive(false);
            promoIcons[3].SetActive(false);
            promoIcons[5].SetActive(false);
            promoIcons[4].SetActive(false);
        }
            else if (civ == (int)Civilisation.Chinese)
            {
            promoIcons[4].SetActive(false);
            promoIcons[5].SetActive(false);
        }
            else if (civ == (int)Civilisation.Huns)
            {
            promoIcons[0].SetActive(false);
            promoIcons[2].SetActive(false);
            promoIcons[3].SetActive(false);
            promoIcons[4].SetActive(false);
            promoIcons[5].SetActive(false);
        }
            else if (civ == (int)Civilisation.Haitians)
            {
            promoIcons[0].SetActive(false);
            promoIcons[4].SetActive(false);
        }
    }

    public void ResetPromoIcons()
    {
        foreach(GameObject gameObject in promoIcons)
        {
            gameObject.SetActive(true);
        }
    }

    public void HidePromotionOption(int option)
    {
        if (promotionPanel6th.activeSelf)
        {
            isPromoted = option;
            Hidepromo6th();
        }
        else
        {
            isPromoted1 = option;
            Hidepromo8th();
        }
    }

    public void Hidepromo6th()
    {
        promotionPanel6th.SetActive(false);


    }
    public void Hidepromo8th()
    {
        promotionPanel8th.SetActive(false);

    }

    public void OnPromotion(int option)
    {
        MultiplayerGamePlay multiplayerGP = FindObjectOfType<MultiplayerGamePlay>();

        int tag = int.Parse(gameObject.tag);
        //Character player = 
        multiplayerGP.playerHandler.getUserPlayers()[tag].SetPromoted(true);
        multiplayerGP.playerHandler.getUserPlayers()[tag].SetPromotedOption(option);
        //player.SetPromoted(true);
        //player.SetPromotedOption(option);
        updateSprite(option);
        HidePromotionOption(option);


       /* int cx = 7 - x;
        int cy = 7 - y;
        int cColour = (int)colour;
        uint localId = RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw;
        RoomPlayer.Local.RPC_SetMovement(
            localId, cx, cy, cColour, -1, -1, -1, false, false, option);*/
    }

    public void updateSprite(int option, bool isMutation=false)
    {
        MultiplayerGamePlay multiplayerGP = FindObjectOfType<MultiplayerGamePlay>();

        Image img = GetComponent<Button>().image;

        if (option == 3)//Knight
        {
            if (RoomPlayer.Local.IsLeader)
            {
                img.sprite = multiplayerGP.charHandler.NavyW.sprite;
            }
            else
            {
                img.sprite = multiplayerGP.charHandler.NavyB.sprite;
            }
        }
        else if (option == 4)//Tower
        {
            if (RoomPlayer.Local.IsLeader)
            {
                img.sprite = multiplayerGP.charHandler.MarineW.sprite;
            }
            else
            {
                img.sprite = multiplayerGP.charHandler.MarineB.sprite;
            }
        }
        else if (option == 2)//elephant
        {
            if (RoomPlayer.Local.IsLeader)
            {
                img.sprite = multiplayerGP.charHandler.AirForceW.sprite;
            }
            else
            {
                img.sprite = multiplayerGP.charHandler.AirForceB.sprite;
            }
        }
        else if (option == 1)//Wazir
        {
            if (RoomPlayer.Local.IsLeader)
            {
                img.sprite = multiplayerGP.charHandler.ArmyW.sprite;
            }
            else
            {
                img.sprite = multiplayerGP.charHandler.ArmyB.sprite;
            }
        }
        else if (option == 9)//Horsemen
        {
            if (RoomPlayer.Local.IsLeader)
            {
                img.sprite = multiplayerGP.charHandler.horseMenSprW;
            }
            else
            {
                img.sprite = multiplayerGP.charHandler.horseMenSprB;
            }
        }
        else if (option == 10)//Witch
        {
            if (RoomPlayer.Local.IsLeader)
            {
                img.sprite = multiplayerGP.charHandler.WitchW.sprite;
            }
            else
            {
                img.sprite = multiplayerGP.charHandler.WitchB.sprite;
            }
        }
        else if (option == 5)//pawn
        {
            if (RoomPlayer.Local.IsLeader)
            {
                img.sprite = multiplayerGP.charHandler.CoastGuardW.sprite;
            }
            else
            {
                img.sprite = multiplayerGP.charHandler.CoastGuardB.sprite;
            }
        }

        
            int cx = 7 - x;
            int cy = 7 - y;
            int cColour = (int)colour;
            uint localId = RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw;
            RoomPlayer.Local.RPC_SetConversion(
                localId, cx, cy, cColour, option, isMutation);
        
    }

    /*public void OnPointerDown(PointerEventData eventData)
    {
        FindObjectOfType<MultiplayerGamePlay>().handleClickToUserPlayers(GetComponent<Button>());
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }*/
}
