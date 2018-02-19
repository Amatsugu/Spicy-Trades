using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapGenerator : ScriptableObject
{
	public Vector2 Size = new Vector2(20, 20);
	public Transform Tile;
	public float OuterRadius = 0.577f;
	[HideInInspector]
	public bool Regen;

	public float InnerRadius
	{
		get
		{
			return OuterRadius * Mathf.Sqrt(3) / 2;
		}
	}
	public abstract Tile Generate(int x, int y, Transform parent = null);

	public Vector3 GetPosition(int x, int y)
	{
		return new Vector3
		{
			y = y * (InnerRadius * 1.5f),
			x = (x + y * .5f - y / 2) * (InnerRadius * 2f),
		};
	}

	public Tile CreateTile(int x, int y, Transform parent, Color col)
	{
		var g = Instantiate(Tile, GetPosition(x, y), Quaternion.identity, parent);
		g.GetComponent<SpriteRenderer>().color = col;
		return g.GetComponent<Tile>().SetPos(x, y);
	}
}
