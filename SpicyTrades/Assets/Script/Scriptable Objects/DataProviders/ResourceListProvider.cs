using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceListProvider : ScriptableObject
{
	public ResourceTileInfo basicFood;

	public abstract ResourceTileInfo[] GetResourceList();

}
