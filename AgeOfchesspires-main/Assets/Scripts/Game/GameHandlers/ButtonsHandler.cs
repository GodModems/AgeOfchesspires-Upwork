using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsHandler : MonoBehaviour
{
    

    

    public Image refBG;
    public Image black4;
    public Image white4;

    public List<Button> allArray = new List<Button>();


    public Transform PlayerPowTransform;
    public Transform PlayerDieTransform;

    public Transform CompPowTransform;
    public Transform CompDieTransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    public List<Button> CreateListofPOWPlayer(int whichOne)
    {
        List<Button> powButtons = new List<Button>();
        
        
        Transform refTransform = whichOne >0 ? PlayerPowTransform : CompPowTransform;
        
       

        for (int i = 0; i < 30; i++)
        {
            Button buttonBg = new GameObject("ButtonBg").AddComponent<Button>();
            buttonBg.image = buttonBg.gameObject.AddComponent<Image>();
            buttonBg.image.sprite = refBG.sprite;

            RectTransform buttonRect = buttonBg.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(50, 50);
            buttonRect.transform.parent = refTransform;
            buttonRect.transform.position = Vector2.zero;

            buttonRect.transform.localScale = Vector2.one;

            powButtons.Add(buttonBg);

        }

        return powButtons;
        
    }

    public List<Button> createListOfKilledByPlayer(int whichOne)
    {
        List<Button> dieButtons = new List<Button>();
        
        
        Transform refTransform = whichOne != 0 ? PlayerDieTransform : CompDieTransform;
        
        

        for (int i = 0; i < 30; i++)
        {
            Button buttonBg = new GameObject("ButtonBg").AddComponent<Button>();
            buttonBg.image = buttonBg.gameObject.AddComponent<Image>();
            buttonBg.image.sprite = refBG.sprite;

            RectTransform buttonRect = buttonBg.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(50, 50);

            buttonRect.transform.parent = refTransform;
            buttonRect.transform.position = Vector2.zero;
            buttonRect.transform.localScale = Vector2.one;

            dieButtons.Add(buttonBg);
        }

        return dieButtons;
    }

    public List<Button> createTilesOnLayers(GameObject actualBoard)
    {
        
        int checkVal = 0;
        int tagSet = 0;

        for (int i = 0; i < 64; i++)
        {
           

            RectTransform buttonRect = allArray[i].GetComponent<RectTransform>();//buttonBg.GetComponent<RectTransform>();

            int charX = buttonRect.GetComponent<GridInfo>().x;
            int charY = buttonRect.GetComponent<GridInfo>().y;
            
            buttonRect.GetComponent<GridInfo>().isEmpty = false;

         
                    
            allArray[i].tag = tagSet.ToString();
        
            
                tagSet = tagSet + 1;
        
        }

        return allArray;
    }

    /*int selectProperImage(int i, int i1, int a)
    {
        int val = 0;

        if (a == 1)
        {
            if (i % 2 == 0)
            {
                if (i1 % 2 == 0)
                    val = 3;
                else
                    val = 2;
            }
            else
            {
                if (i1 % 2 == 0)
                    val = 1;
                else
                    val = 0;
            }
        }
        else
        {
            if (i % 2 == 0)
            {
                if (i1 % 2 == 0)
                    val = 0;
                else
                    val = 1;
            }
            else
            {
                if (i1 % 2 == 0)
                    val = 2;
                else
                    val = 3;
            }
        }

        return val;
    }
    */

    public float CalculateOffsetX(float x)
    {
        
        float calculatedXOffset=0;
        // Calculate the X offset based on your game's specific logic
        calculatedXOffset = Screen.width / 768;
        // ...
        return calculatedXOffset;
    }

    public float CalculateOffsetY(float y)
    {
        float calculatedYOffset=0;
        // Calculate the Y offset based on your game's specific logic
        // ...

        calculatedYOffset = Screen.height / 1152;

        return calculatedYOffset;
    }

    public void SetEnableDice(int whichDice)
    {
        //UIManager.Instance.SetEnableDice(whichDice);
    }

   
}
