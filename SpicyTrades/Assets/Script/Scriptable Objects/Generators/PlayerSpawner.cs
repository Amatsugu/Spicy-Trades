using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Feature Generator/Player Spawner")]
public class PlayerSpawner : FeatureGenerator
{
	public GameObject playerPrefab;
	public override void Generate(Map map)
	{
		var playerCharacter = Instantiate(playerPrefab, map.Capital.WolrdPos, Quaternion.Euler(-60, 0, 0)).GetComponent<PlayerObject>();
		var player = new Player(playerCharacter);
		player.SetTile(map.Capital);
		map.AddPlayer(player, true);
	}
}
