using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile/Basic")]
public class TileInfo : ScriptableObject
{
	public string tag;
	public Sprite sprite;
	public Color color = Color.white;
	public int cost;
	public TileType TileType { get; protected set; }

	public List<TileRenderer> tileRenderers = new List<TileRenderer>();
}

public enum TileType
{
	Tile,
	Settlement,
	Resource,
	Factory
}