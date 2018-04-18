using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Data Providers/Local Resource List")]
public class LocalResourceList : ResourceListProvider
{
	public ResourceTileInfo[] resources;

	public override ResourceTileInfo[] GetResourceList()
	{
		return resources;
	}

	public override ResourceTileInfo GetResourceByName(string name)
	{
		return resources.SingleOrDefault(r => r.name == name);
	}
}
