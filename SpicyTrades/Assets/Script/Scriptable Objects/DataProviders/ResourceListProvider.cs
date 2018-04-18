using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ResourceListProvider : ScriptableObject
{
	public ResourceTileInfo basicFood;

	public abstract ResourceTileInfo[] GetResourceList();

	public virtual ResourceTileInfo GetResourceByName(string name)
	{
		return GetResourceList().SingleOrDefault(r => r.name == name);
	}

}
