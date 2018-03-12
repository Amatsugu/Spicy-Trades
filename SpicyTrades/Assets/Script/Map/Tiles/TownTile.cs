using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownTile : Tile
{
	public TownType townType;
	public string Name;

	public TownTile(TownTileInfo tileInfo, Transform parent, HexCoords hexCoords, float outerRadius) : base(tileInfo, parent, hexCoords, outerRadius)
	{
	}

	public int Population { get; set; }
	public List<ResourceTileInfo> Resources { get; private set; }

	public TownTile AddResource(ResourceTileInfo resource)
	{
		if (Resources == null)
			Resources = new List<ResourceTileInfo>();
		Resources.Add(resource);
		return this;
	}

}
