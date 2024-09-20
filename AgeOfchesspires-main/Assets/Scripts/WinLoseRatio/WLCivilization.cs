using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static GameConstants;

[Serializable]
public class WLCivilization
{
    public int Id;
    public int Wins;
    public int Losses;
    public int Draw;

    public WLCivilization(int id)
    {
        Id = id;
        Wins = 0;
        Losses = 0;
        Draw = 0;
    }

    public void RecordWin()
    {
        Wins++;
    }

    public void RecordLoss()
    {
        Losses++;
    }

    public void RecordDraw()
    {
        Draw++;
    }

   

    public float GetWinLossRatio()
    {
        if (Losses == 0)
        {
            return Wins;
        }
        return (float)Wins / Losses;
    }
}

[Serializable]
public class WLPlayerData
{
    public List<WLCivilization> Civilizations;

    public WLPlayerData()
    {
        Civilizations = new List<WLCivilization>();
        for (int i = 0; i < 10; i++)
        {
            Civilizations.Add(new WLCivilization(i));
        }
    }
}

