using System.Collections;
using System.Collections.Generic;
using NetworkManager;
using UnityEngine;

[CreateAssetMenu(menuName = "Feature Generator/Player Spawner")]
public class PlayerSpawner : FeatureGenerator
{
	public GameObject playerPrefab;
	public override void Generate(Map map)
	{
		if (GameMaster.Offline)
		{
			var playerCharacter = Instantiate(playerPrefab, map.Capital.WolrdPos, Quaternion.Euler(-60, 0, 0)).GetComponent<PlayerObject>();
			var playerObj = new Player(playerCharacter);
			playerObj.SetTile(map.Capital, false);
			map.AddPlayer(playerObj, true);
		}
		else
		{
			var members = SpicyNetwork.CurrentRoom.GetMembers();
			var curPlayerId = SpicyNetwork.player.GetID();
			Debug.Log($"Current Player:{SpicyNetwork.player.GetName()} - {curPlayerId}");
			foreach (var player in members)
			{
				if (player == null)
					continue;
				var pID = player.GetID();
				Debug.Log($"Player: {player.GetName()} - {pID}");
				var playerCharacter = Instantiate(playerPrefab, map.Capital.WolrdPos, Quaternion.Euler(-60, 0, 0)).GetComponent<PlayerObject>();
				var playerObj = new Player(playerCharacter, pID);
				playerObj.SetTile(map.Capital, false);
				map.AddPlayer(playerObj, pID == curPlayerId);
			}
		}
	}
}
