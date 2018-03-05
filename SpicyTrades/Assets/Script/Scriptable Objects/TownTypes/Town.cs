using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Town Type/Town")]
public class Town : TownType
{
	public override TownTile Initialize(TownTile tile)
	{
		return base.Initialize(tile);
	}
}
