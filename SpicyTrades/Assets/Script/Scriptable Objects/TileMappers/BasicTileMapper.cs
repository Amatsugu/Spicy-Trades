using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile Mapper/Basic")]
public class BasicTileMapper : TileMapper
{
	public TileInfo Tile;

	public override TileInfo GetTile(float sample)
	{
		return Tile;
	}

	public override float GetMoveCost(float sample)
	{
		return 1f;
	}
}
