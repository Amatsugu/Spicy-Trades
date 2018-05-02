using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryItem
{
	public ResourceTileInfo ActualResource
	{ get
		{
			if (_actualResource == null)
				return _actualResource = GameMaster.Registry.resourceList.GetResourceList().First(res => Resource.Match(res));
			else
				return _actualResource;
		}
	}
	public ResourceIdentifier Resource { get; set; }
	public float Cost { get; set; }

	public ResourceTileInfo _actualResource;
}

