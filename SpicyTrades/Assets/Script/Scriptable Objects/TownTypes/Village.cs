using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Town Type/Village")]
public class Village : TownType
{
	public override void Initialize(TownTile tile)
	{
		base.Initialize(tile);
	}
}
