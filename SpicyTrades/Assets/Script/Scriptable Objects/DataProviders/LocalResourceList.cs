using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data Providers/Local Resource List")]
public class LocalResourceList : ResourceListProvider
{
	public ResourceTileInfo[] resources;

	public override ResourceTileInfo[] GetResourceList()
	{
		return resources;
	}
}
