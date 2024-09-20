using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MatchHistoryManager : MonoBehaviour
{
    private string filePath;

    [SerializeField]
    private GameObject matchEntryPrefab;

    [SerializeField]
	private Transform matchHistoryContent;

	[SerializeField]
    private MatchHistory matchHistory;
    [SerializeField]
    private List<Sprite> CivDps;
    [SerializeField]
    private GameObject noHistoryLabel;
    private void Start()
	{
		// Define the file path
		filePath = Path.Combine(Application.persistentDataPath, "matchHistory.json");


		// Load match history from file if it exists, otherwise create a new one
		//if (File.Exists(filePath))
		//{
			LoadMatchHistory();
			Debug.Log("Match history loaded from file.");
		//}
		//else
		//{
		//	InitializeMatchHistory();
		//	SaveMatchHistory();
		//	Debug.Log("Match history file created and saved.");
		//}

        UpdateMatchHistoryUI();

    }

    // Initialize with some example data
    /*private void InitializeMatchHistory()
	{
		matchHistory = new MatchHistory();

		MatchEntry match1 = new MatchEntry
		{
			player = new Player { username = "Player1", eloBadge = 1, eloRating = 1200 },
			opponent = new Player { username = "Opponent1", eloBadge = 2, eloRating = 1100 }
		};

		MatchEntry match2 = new MatchEntry
		{
			player = new Player { username = "Player1", eloBadge = 3, eloRating = 1225 },
			opponent = new Player { username = "Opponent2", eloBadge = 2, eloRating = 1300 }
		};

		AddMatchEntry(match1);
        AddMatchEntry(match2);
	}*/

	// Serialize MatchHistory to JSON string and save to file
	public void SaveMatchHistory()
	{
		string jsonString = JsonUtility.ToJson(matchHistory, true);
		File.WriteAllText(filePath, jsonString);
		Debug.Log("Match history saved to file.");
	}

	// Load and deserialize MatchHistory from JSON file
	public void LoadMatchHistory()
	{
		if (File.Exists(filePath))
		{
			string jsonString = File.ReadAllText(filePath);
			matchHistory = JsonUtility.FromJson<MatchHistory>(jsonString);
			Debug.Log("Match history loaded from file.");

		}
		else
		{
			noHistoryLabel.SetActive(true);
		}
		
	}

	// For testing: Add a new match entry and save it
	public void AddMatchEntry(MatchEntry newMatch)
	{
		
        matchHistory.matchHistory.Add(newMatch);
		SaveMatchHistory();
		Debug.Log("New match entry added and saved.");

		
	}

    public void UpdateMatchHistoryUI()
    {
		foreach (MatchEntry matchEntry in matchHistory.matchHistory)
		{
			GameObject gameObject = Instantiate(matchEntryPrefab);
			MatchEnteryCell matchEnteryCell = gameObject.GetComponent<MatchEnteryCell>();
            matchEnteryCell.playerUserName = matchEntry.player.username;
			matchEnteryCell.playerEloBadge = matchEntry.player.eloBadge;
			matchEnteryCell.playerEloRating = matchEntry.player.eloRating;
			matchEnteryCell.oppUserName = matchEntry.opponent.username;
			matchEnteryCell.oppEloBadge = matchEntry.opponent.eloBadge;
			matchEnteryCell.oppEloRating = matchEntry.opponent.eloRating;
			matchEnteryCell.UpdateUI(CivDps[matchEntry.player.civ], CivDps[matchEntry.opponent.civ]);

			gameObject.transform.parent = matchHistoryContent;
		}
    }
}
