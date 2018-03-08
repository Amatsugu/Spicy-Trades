using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownTile : SmartTile
{
	public TownType townType;

	public int Population { get; set; }
	public List<ResourceTile> Resources { get; private set; }

	public TownTile AddResource(ResourceTile resource)
	{
		if (Resources == null)
			Resources = new List<ResourceTile>();
		Resources.Add(resource);
		return this;
	}

}
