using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapGenerator : ScriptableObject
{
	public Vector2 Size = new Vector2(20, 20);
	public TileMapper tileMapper;
	public float OuterRadius = 0.577f;
	public FeatureGenerator[] featureGenerators;
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
		return CreateTile(tileMapper.GetTile(0), x, y, parent, col);
	}

	public Tile CreateTile(Transform t, int x, int y, Transform parent, Color col)
	{
		var g = Instantiate(t, GetPosition(x, y), Quaternion.identity, parent);
		g.GetComponent<SpriteRenderer>().color = col;
		return g.GetComponent<Tile>().SetPos(x, y);
	}

	public void GenerateFeatures(Map map)
	{
		if (featureGenerators == null)
			return;
		foreach (var fg in featureGenerators)
		{
			if (fg != null)
				fg.Generate(map);
		}
	}

	public void GenerateMap(Map map, Transform parent = null)
	{
		for (int y = 0, i = 0; y < map.Height; y++)
		{
			for (int x = 0; x < map.Width; x++)
			{
				map[i++] = Generate(x, y, parent);
			}
		}
	}
}
