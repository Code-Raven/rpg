using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Player : Unit
{
	static List<Player> m_players = new List<Player>();

	// Use this for initialization
	public override void Awake()
	{
		m_players.Add(this);
		base.Awake();
	}

	public static Player GetPlayer(int playerIndex)
	{
		return m_players[playerIndex];
	}

	public static Player GetClosestPlayer(Vector3 point, float maxRange)
	{
		Player player = m_players[0];
		float minRange = Vector3.Distance(player.transform.position, point);
		float range = minRange;

		foreach(Player child in m_players)
		{
			range = Vector3.Distance(child.transform.position, point);
			if(range < minRange)
			{
				player = child;
				minRange = range;
			}
		}

		if(minRange > maxRange)
			return null;

		return player;
	}
}
