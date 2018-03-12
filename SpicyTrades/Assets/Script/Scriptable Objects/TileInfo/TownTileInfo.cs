using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile/Town")]
public class TownTileInfo : TileInfo
{
	public new readonly TileType tileType = TileType.Town;
	public TownType townType;
}

public enum TownType
{
	Town,
	Village,
	Capital
}
