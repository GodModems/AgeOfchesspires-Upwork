using static GameConstants;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class WLPlayerHandler : MonoBehaviour {

    private WLPlayerData playerData;
    private string saveFilePath;

    [SerializeField]
    List<TMP_Text> tMP_Texts = new List<TMP_Text>();

    void Start()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "playerData.json");
        if (!File.Exists(saveFilePath))
        {
            playerData = new WLPlayerData();
            SaveData();
        }
        else
        {
            LoadData();
            UpdateUI();
            
        }
    }

    public void UpdateUI()
    {
        for (int i = 0; i < playerData.Civilizations.Count; i++)
        {
            /*tMP_Texts[i].text = ConvertFloatToRatio(playerData.Civilizations[i].GetWinLossRatio());*/
            tMP_Texts[i].text = 
                playerData.Civilizations[i].Wins +":"+
                playerData.Civilizations[i].Losses +":"+
                playerData.Civilizations[i].Draw;
        }
    }

    public string ConvertFloatToRatio(float ratio)
    {
        // Find the greatest common divisor (GCD) using Euclidean algorithm
        int gcd(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        // Convert the float ratio to a fraction
        int numerator = (int)(ratio * 10000);
        int denominator = 10000;

        // Simplify the fraction
        int divisor = gcd(numerator, denominator);
        numerator /= divisor;
        denominator /= divisor;

        return $"{numerator}:{denominator}";
    }

    public void RecordGameResult(int civilizationId, bool isWin, bool isDraw)
    {
        WLCivilization civilization = playerData.Civilizations.Find(c => c.Id == civilizationId);
        if (civilization != null)
        {
            if (isDraw)
            {
                civilization.RecordDraw();
            }
            else
            {
                if (isWin)
                {
                    civilization.RecordWin();
                }
                else
                {
                    civilization.RecordLoss();
                }
            }
            SaveData();
            UpdateUI();
        }
        else
        {
            Debug.LogError("Invalid civilization ID.");
        }
    }

    public float GetWinLossRatio(int civilizationId)
    {
        WLCivilization civilization = playerData.Civilizations.Find(c => c.Id == civilizationId);
        if (civilization != null)
        {
            return civilization.GetWinLossRatio();
        }
        else
        {
            Debug.LogError("Invalid civilization ID.");
            return 0;
        }
    }

    public void SaveData()
    {
        string jsonData = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(saveFilePath, jsonData);
    }

    public void LoadData()
    {
        if (File.Exists(saveFilePath))
        {
            string jsonData = File.ReadAllText(saveFilePath);
            playerData = JsonUtility.FromJson<WLPlayerData>(jsonData);
        }
        else
        {
            Debug.LogWarning("Save file not found.");
        }
    }
}