using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryTile : Tile
{
	public FactoryTile(FactoryTileInfo tileInfo, Transform parent, HexCoords hexCoords, float outerRadius) : base(tileInfo, parent, hexCoords, outerRadius)
	{
		this.tileInfo = tileInfo;
	}
}
