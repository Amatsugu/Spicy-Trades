using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile/Resource")]
public class ResourceTileInfo : TileInfo
{
	public new readonly TileType tileType = TileType.Resource;
	public string ResourceName;
	public ResourceCategory category;
	public int requiredWorkers;
	public float yeild;

}

public enum ResourceCategory
{
	Material,
	Food,
	Luxury,
	Fuel,
	Stategic
}
