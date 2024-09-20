using System;
using System.Collections.Generic;

[Serializable]
public class Player
{
	public int civ;
	public string username;
	public string eloBadge;
	public int eloRating;
}

[Serializable]
public class MatchEntry
{
	public Player player;
	public Player opponent;
}

[Serializable]
public class MatchHistory
{
	public List<MatchEntry> matchHistory = new List<MatchEntry>();
}
