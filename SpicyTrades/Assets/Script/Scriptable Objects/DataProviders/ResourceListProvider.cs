using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ResourceListProvider : ListProvider<ResourceTileInfo>
{
	public ResourceTileInfo basicFood;

	public virtual ResourceTileInfo GetResourceByName(string name)
	{
		return items.SingleOrDefault(r => r.name == name);
	}

}
