﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Town Type/Village")]
public class Village : TownType
{
	public override TownTile Initialize(TownTile tile)
	{
		return base.Initialize(tile);
	}
}
