using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharHandler : MonoBehaviour
{
    private Vector2 winSize;

    public GameObject buttonPrefab;
    public GameObject buttonPrefabSmall;

    public GameObject colorCase;
    public GameObject colorCase2;

    public Image AirForceB;
    public Image AirForceW;
    public Image ArmyB;
    public Image ArmyW;
    public Image CoastGuardB;
    public Image CoastGuardW;
    public Image MarineB;
    public Image MarineW;
    public Image NavyB;
    public Image NavyW;
    public Image PresidentB;
    public Image PresidentW;
    public Image WitchB;
    public Image WitchW;

    //Orig
    public Image AirForceBOrig;
    public Image AirForceWOrig;
    public Image ArmyBOrig;
    public Image ArmyWOrig;
    public Image CoastGuardBOrig;
    public Image CoastGuardWOrig;
    public Image MarineBOrig;
    public Image MarineWOrig;
    public Image NavyBOrig;
    public Image NavyWOrig;
    public Image PresidentBOrig;
    public Image PresidentWOrig;
    public Sprite horseMenSprW;
    public Sprite horseMenSprB;
    public Sprite witchSprW;
    public Sprite witchSprB;

    [SerializeField]
    private List<Button> dummyButtonArray;

    public float aspectRatiio;

    private void Start()
    {
        winSize = new Vector2(Screen.width, Screen.height);
        dummyButtonArray = new List<Button>();


        aspectRatiio = winSize.x / winSize.y;
    }


    public static Scene Scene()
    {
        return SceneManager.GetActiveScene();
    }

    public Transform GetPosInRectFromCharPoint(Button butt, Char_Point pos) //Vector2
    {
        // Implement your logic here



        Transform bt = butt.transform.Find("bt");
        Transform wt = butt.transform.Find("wt");
        Transform bwt = butt.transform.Find("bwt");

        Transform posInRect = bwt;


       
        /*if (pos.colour == colour.WHITE)
        {

            posInRect = wt;
        }
        else if (pos.colour == colour.BLACK)
        {
           
            posInRect = bt;
        }
        else
        {
            
        }*/

        return posInRect;


    }

    private List<Button> combinedListW = new List<Button>();
    private List<Button> combinedListB = new List<Button>();
    private List<Button> allArrayOne = null;
    public List<Button> PutPlayersToBoard(List<Button> allBoardRect, List<Character> players, bool isBlack, bool isUser)
    {
        // Implement your logic here
        allArrayOne = new List<Button>();
        Button butt;
        string colour = isBlack ? "Black" : "White";

        for (int i = 0; i < players.Count; i++)
        {
            Char_Point playerPosition = players[i].getPosition();
            butt = allBoardRect[playerPosition.index()];

            GameObject gameObject = null;
            //if (players[i].getName() == CharacterDef.PRESIDENT || players[i].getName() == CharacterDef.ARMY)
            //{
                 gameObject = Instantiate(buttonPrefab);

            //}
            //else
            //{
                //gameObject = Instantiate(buttonPrefabSmall);
            //}

            Button charShow = gameObject.GetComponent<Button>();

            charShow.GetComponent<GridInfo>().characterDef = players[i].getName();
            charShow.GetComponent<GridInfo>().x = playerPosition.x;
            charShow.GetComponent<GridInfo>().y = playerPosition.y;
            charShow.GetComponent<GridInfo>().colour = playerPosition.colour;


            Image image = SelectImage(players[i].getImageName(), colour);
            charShow.GetComponent<Image>().sprite = image.sprite;

            if (isUser)
            {
                gameObject.GetComponent<DragAndDropHandler>().enabled = true;

                int myCiv = GameObject.FindObjectOfType<MultiplayerGamePlay>().currMyCiv;
                if (charShow.GetComponent<GridInfo>().characterDef == CharacterDef.COASTGUARD &&
                    myCiv == (int)GameConstants.Civilisation.Huns)
                {
                    if(colour.Equals("White"))
                    {
                        charShow.GetComponent<Image>().sprite = horseMenSprW;
                    }
                    else
                    {
                        charShow.GetComponent<Image>().sprite = horseMenSprB;
                    }
                }
                else
                if (charShow.GetComponent<GridInfo>().characterDef == CharacterDef.NAVY &&
                    myCiv == (int)GameConstants.Civilisation.Haitians)
                {
                    if (colour.Equals("White"))
                    {
                        charShow.GetComponent<Image>().sprite = witchSprW;
                    }
                    else
                    {
                        charShow.GetComponent<Image>().sprite = witchSprB;
                    }
                }


            }
            else
            {
                int oppCiv = GameObject.FindObjectOfType<MultiplayerGamePlay>().currOppCiv;
                if (charShow.GetComponent<GridInfo>().characterDef == CharacterDef.COASTGUARD &&
                    oppCiv == (int)GameConstants.Civilisation.Huns)
                {
                    if (colour.Equals("White"))
                    {
                        charShow.GetComponent<Image>().sprite = horseMenSprW;
                    }
                    else
                    {
                        charShow.GetComponent<Image>().sprite = horseMenSprB;
                    }
                }
                else
                if (charShow.GetComponent<GridInfo>().characterDef == CharacterDef.NAVY &&
                    oppCiv == (int)GameConstants.Civilisation.Haitians)
                {
                    if (colour.Equals("White"))
                    {
                        charShow.GetComponent<Image>().sprite = witchSprW;
                    }
                    else
                    {
                        charShow.GetComponent<Image>().sprite = witchSprB;
                    }
                }
            }


            charShow.tag = i.ToString();

            charShow.GetComponent<GridInfo>().imageName = players[i].getImageName();

            RectTransform buttonRect = charShow.GetComponent<RectTransform>();

            float val = butt.transform.localRotation.eulerAngles.z;///TTTTTT

            buttonRect.transform.parent = butt.transform;


            buttonRect.transform.localPosition = GetPosInRectFromCharPoint(butt, playerPosition).localPosition;

//            if(aspectRatiio == 1.6f)
//            {
                buttonRect.transform.localScale = Vector2.one;//new Vector3(aspectRatiio,aspectRatiio,aspectRatiio);
                                                              //            }

            allArrayOne.Add(charShow);

            if(isBlack)
            {
                combinedListB.Add(charShow);
            }
            else
            {
                combinedListW.Add(charShow);
            }
            
        }

        return allArrayOne;
    }

    private Image SelectImage(string imageName, string color)
    {
        Image image = null;

        if(imageName.Contains("Airforce"))
        {
            if(color.Equals("Black"))
            {
                image = AirForceB;
            }
            else
            {
                image = AirForceW;
            }
        }
        else if (imageName.Contains("Marine"))
        {
            if (color.Equals("Black"))
            {
                image = MarineB;
            }
            else
            {
                image = MarineW;
            }
        }
        else if (imageName.Contains("Army"))
        {
            if (color.Equals("Black"))
            {
                image = ArmyB;
            }
            else
            {
                image = ArmyW;
            }
        }
        else if (imageName.Contains("Coast-Guard"))
        {
            if (color.Equals("Black"))
            {
                image = CoastGuardB;
            }
            else
            {
                image = CoastGuardW;
            }
        }
        else if (imageName.Contains("Navy"))
        {
            if (color.Equals("Black"))
            {
                image = NavyB;
            }
            else
            {
                image = NavyW;
            }
        }
        else if (imageName.Contains("President"))
        {
            if (color.Equals("Black"))
            {
                image = PresidentB;
            }
            else
            {
                image = PresidentW;
            }
        }
        else if (imageName.Contains("Witch"))
        {
            if (color.Equals("Black"))
            {
                image = WitchB;
            }
            else
            {
                image = WitchW;
            }
        }
        image.type = Image.Type.Simple;
        Debug.Log("CharHandler::SelectImage::"+imageName);
        return image;
    }

    private Image SelectImageOrig(string imageName, string color)
    {
        Image image = null;

        if (imageName.Contains("Airforce"))
        {
            if (color.Equals("Black"))
            {
                image = AirForceBOrig;
            }
            else
            {
                image = AirForceWOrig;
            }
        }
        else if (imageName.Contains("Marine"))
        {
            if (color.Equals("Black"))
            {
                image = MarineBOrig;
            }
            else
            {
                image = MarineWOrig;
            }
        }
        else if (imageName.Contains("Army"))
        {
            if (color.Equals("Black"))
            {
                image = ArmyBOrig;
            }
            else
            {
                image = ArmyWOrig;
            }
        }
        else if (imageName.Contains("Coast-Guard"))
        {
            if (color.Equals("Black"))
            {
                image = CoastGuardBOrig;
            }
            else
            {
                image = CoastGuardWOrig;
            }
        }
        else if (imageName.Contains("Navy"))
        {
            if (color.Equals("Black"))
            {
                image = NavyBOrig;
            }
            else
            {
                image = NavyWOrig;
            }
        }
        else if (imageName.Contains("President"))
        {
            if (color.Equals("Black"))
            {
                image = PresidentBOrig;
            }
            else
            {
                image = PresidentWOrig;
            }
        }
        image.type = Image.Type.Simple;
        Debug.Log("CharHandler::SelectImage::" + imageName);
        return image;
    }

    public void ColourEffectPlaceWhereToGo(List<Char_Point> posSelection, List<Button> allBoards)
    {
        StartCoroutine(colorHigLightDelayed(posSelection, allBoards));
    }


    IEnumerator colorHigLightDelayed(List<Char_Point> posSelection, List<Button> allBoards)
    {
        //RemoveHighLightColour();

        yield return new WaitForSeconds(0.05f);
        
        foreach (Char_Point pos in posSelection)
        {
            int posIndex = pos.index();
            foreach (var rect in allBoards)
            {

                Transform b = rect.transform.Find("b");
                Transform w = rect.transform.Find("w");
                Transform bw = rect.transform.Find("bw");

                if (rect.tag == posIndex.ToString())
                {
                    GameObject colourBG = null;
                    if (pos.colour == colour.BOTH)
                    {
                       

                        bw.gameObject.SetActive(true);



                       
                    }
                    else
                    {
                      
                        if (pos.colour == colour.WHITE)
                        {
                            w.gameObject.SetActive(true);
                        }
                        else
                        {
                            b.gameObject.SetActive(true);
                        }




                       
                    }

                   

                    dummyButtonArray.Add(rect);
                }
            }

            yield return new WaitForSeconds(0.05f);
        }

        yield return null;

    }

    public void RemoveHighLightColour()
    {

        // Implement your logic here
        for (int i = 0; i < dummyButtonArray.Count; i++)
        {
            Button butt = dummyButtonArray[i];

            Transform b = butt.transform.Find("b");
            Transform w = butt.transform.Find("w");
            Transform bw = butt.transform.Find("bw");

            b.gameObject.SetActive(false);
            w.gameObject.SetActive(false);
            bw.gameObject.SetActive(false);


           
        }
        dummyButtonArray.Clear();

    }

    public void ChangeSkin(bool isUserPlayerBlack, bool isToggled)
    {
        List<Button> combinedList;
        List<Button> combinedListOpp;
        if (isUserPlayerBlack)
        {
            combinedList = combinedListB;
            combinedListOpp = combinedListW;
        }
        else
        {
            combinedList = combinedListW;
            combinedListOpp = combinedListB;

        }
        
        foreach (Button button in combinedList)
        {
            string imageName = button.GetComponent<GridInfo>().imageName;
            string color = isUserPlayerBlack ?
                "Black" : "White";

            Image image = null;
            if (isToggled)
            {
                image = SelectImage(imageName, color);
            }
            else
            {
                image = SelectImageOrig(imageName, color);
            }
            
            button.GetComponent<Image>().sprite = image.sprite;
            button.GetComponent<GridInfo>().imageName = imageName;
        }

        foreach (Button button in combinedListOpp)
        {
            string imageName = button.GetComponent<GridInfo>().imageName;
            string color = isUserPlayerBlack ?
                "White" : "Black";

            Image image = null;
            if (isToggled)
            {
                image = SelectImage(imageName, color);
            }
            else
            {
                image = SelectImageOrig(imageName, color);
            }

            button.GetComponent<Image>().sprite = image.sprite;
            button.GetComponent<GridInfo>().imageName = imageName;
        }
    }
}
