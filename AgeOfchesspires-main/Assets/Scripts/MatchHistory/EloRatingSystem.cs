using UnityEngine;
using UnityEngine.Internal;

public class EloRatingSystem : MonoBehaviour
{

    private int PlayerRating { get; set; }
    private int OppRating { get; set; }

    // Function to calculate the probability of winning
    private double Probability(int rating1, int rating2)
    {
        return 1.0 / (1.0 + Mathf.Pow(10, (rating2 - rating1) / 400.0f));
    }

    // Function to calculate the new Elo ratings
    public void CalculateEloRating(int K, float outcome, int currMyCiv, int currOppCiv, bool isDraw)
    {
        if (isDraw)
        {
            outcome = 0.5f;
        }

        OppRating = RoomPlayer.Local.GetCurrentOppELO();
        PlayerRating = GetUpdatedPlayerElo();

        // Calculate the probabilities of each player winning
        double Pa = Probability(PlayerRating, OppRating);
        double Pb = Probability(OppRating, PlayerRating);

        // Update the ratings
        PlayerRating = (int)(PlayerRating + K * (outcome - Pa));
        PlayerPrefs.SetInt("PlayerElo", PlayerRating);
        Debug.Log("Calculate Elo Rating::Player Updated Rating::" + PlayerRating);

        string badge1 = GetBadgeForRating(PlayerRating);

        OppRating = (int)(OppRating + K * ((1.0 - outcome) - Pb));
        Debug.Log("Calculate Elo Rating::OPP Updated Rating::" + OppRating);

        string badge2 = GetBadgeForRating(OppRating);

        //int currMyCiv = UIManager.Instance._myCivilization;
        //int currOppCiv = RoomPlayer.currCivOpp;

        Debug.Log("Calculate Elo Rating::1111::" + currMyCiv);

        Debug.Log("Calculate Elo Rating::2222::" + currOppCiv);

        MatchEntry matchEntery = new MatchEntry
        {
            player = new Player { 
                civ = currMyCiv,
                username = "You", 
                eloBadge = badge1, 
                eloRating = PlayerRating 
            },
            opponent = new Player {
                civ = currOppCiv,
                username = "Opponent", 
                eloBadge = badge2, 
                eloRating = OppRating
            }
        };

        GetComponent<GameLauncher>().matchHistoryManager.AddMatchEntry(matchEntery);


        GetComponent<WLPlayerHandler>().RecordGameResult(currMyCiv, outcome==1, isDraw);
    }

    public string GetBadgeForRating(int PlayerRating)
    {
        string badge1 = "Silver";
        if (PlayerRating >= 800 && PlayerRating <= 1200)
        {
            badge1 = "Gold";
        }
        else if (PlayerRating > 1200 && PlayerRating <= 1600)
        {
            badge1 = "Platinum";
        }
        else if (PlayerRating > 1600)
        {
            badge1 = "Diamond";
        }
        return badge1;
    }

    public int GetUpdatedPlayerElo()
    {
        int playerElo = PlayerPrefs.GetInt("PlayerElo",1000);
        return playerElo;
    }

    
    
    // Example usage
    private void Start()
    {
        // Initial ratings
        /*int playerARating = 1200;
        int playerBRating = 1000;

        // K-factor
        int K = 30;

        // Outcome: 1 for player A win, 0 for player B win, 0.5 for draw
        double outcome = 1; // Player A wins

        // Calculate new ratings
        CalculateEloRating(ref playerARating, ref playerBRating, K, outcome);

        // Display the new ratings
        Debug.Log($"Player A's new rating: {playerARating}");
        Debug.Log($"Player B's new rating: {playerBRating}");*/
    }
}
