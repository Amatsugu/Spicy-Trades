using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile Mapper/Basic")]
public class BasicTileMapper : TileMapper
{
	public Transform Tile;

	public override Transform GetTile(float sample)
	{
		return Tile;
	}

	public override Color GetColor(float sample)
	{
		return Random.ColorHSV();
	}

	public override float GetMoveCost(float sample)
	{
		return 1f;
	}
}
