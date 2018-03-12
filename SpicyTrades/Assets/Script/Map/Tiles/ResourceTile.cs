using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceTile : Tile
{
	public ResourceType resourceType;

	public override void TileInit()
	{
		base.TileInit();
		Debug.Log("super");
		tileRenderers.Add(resourceType.TileRenderer);
	}
}
