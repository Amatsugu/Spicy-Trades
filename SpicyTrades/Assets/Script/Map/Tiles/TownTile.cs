using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownTile : SmartTile
{
	public TownType townType;

	public int Population { get; set; }

	public TownTile Initialize()
	{
		return townType.Initialize(this);
	}

}
