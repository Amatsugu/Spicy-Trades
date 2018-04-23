using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Data Providers/Local Resource List")]
public class LocalResourceList : ResourceListProvider
{
	public override ResourceTileInfo GetResourceByName(string name)
	{
		return items.SingleOrDefault(r => r.name == name);
	}
}
