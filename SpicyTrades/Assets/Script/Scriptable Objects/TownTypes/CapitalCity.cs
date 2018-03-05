using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Town Type/Capital")]
public class CapitalCity : TownType {
	public override TownTile Initialize(TownTile tile)
	{
		return base.Initialize(tile);
	}
}
