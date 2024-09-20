using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchEnteryCell : MonoBehaviour
{
    //public string playerCiv { get; set; }
    public string playerUserName { get; set; }
    public string playerEloBadge { get; set; }
    public int playerEloRating { get; set; }
    //public string oppCiv { get; set; }

    public string oppUserName { get; set; }
    public string oppEloBadge { get; set; }
    public int oppEloRating { get; set; }

    [SerializeField]
    private Image playerCivImg;
    [SerializeField]
    private TMP_Text playerUserNameTxt;
    [SerializeField]
    private TMP_Text playerEloBadgeTxt;
    [SerializeField]
    private TMP_Text playerEloRatingTxt;

    [SerializeField]
    private Image oppCivImg;
    [SerializeField]
    private TMP_Text oppUserNameTxt;
    [SerializeField]
    private TMP_Text oppEloBadgeTxt;
    [SerializeField]
    private TMP_Text oppEloRatingTxt;

    public void UpdateUI(Sprite playerSpr, Sprite oppSpr)
    {
        playerCivImg.sprite = playerSpr;
        playerUserNameTxt.text = playerUserName;
        playerEloBadgeTxt.text = playerEloBadge;
        playerEloRatingTxt.text = playerEloRating.ToString();

        oppCivImg.sprite = oppSpr;
        oppUserNameTxt.text = oppUserName;
        oppEloBadgeTxt.text = oppEloBadge;
        oppEloRatingTxt.text = oppEloRating.ToString();
}

}
