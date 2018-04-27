using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile/Resource")]
public class ResourceTileInfo : TileInfo
{
	public ResourceCategory category;
	public string[] tags;
	public int requiredWorkers = 5;
	public float yeild = 20;
	public float basePrice;
	public string description;
	public string tooltip;
	public Sprite icon;
	public Recipe recipe;

	public string PrettyName
	{
		get
		{
			return "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + name + "</color>";
		}
	}

	public void OnEnable()
	{
		TileType = TileType.Resource;
		tag = "Resource";
	}

}

public enum ResourceCategory
{
	Material,
	Food,
	Luxury,
	Fuel,
	Stategic
}
