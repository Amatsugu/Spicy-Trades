using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Data Providers/Factory List")]
public class FactoryListProvider : ListProvider<FactoryTileInfo>
{
	public FactoryTileInfo GetFactoryByType(string factoryType)
	{
		return items.First(f => f.factoryType == factoryType);
	}
}
