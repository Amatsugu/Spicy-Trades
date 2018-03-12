﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapRenderer : MonoBehaviour
{
	public MapGenerator generator;
	public GameObject smartTile;
	public Map map;
	public static MapRenderer Instance;

    // Use this for initialization
    void Start()
    {
		var startTime = System.DateTime.Now;
		Instance = this;
		map = generator.GenerateMap(transform);
		generator.GenerateFeatures(map);
		foreach (Tile t in map)
		{
			t.TileInit();
			t.TileRender();
		}
		Debug.Log("Generation Time: " + (System.DateTime.Now - startTime).TotalMilliseconds + "ms");
    }

    // Update is called once per frame
    void Update()
    {
		if (!generator.Regen)
			return;
		generator.Regen = false;
		for (int i = 0; i < transform.childCount; i++)
		{
			Destroy(transform.GetChild(i).gameObject);
		}
		Start();
    }

	public static Tile GetTile(int x, int y, int z)
	{
		return Instance.map[x, y, z];
	}

	public static void TouchTile(Tile tile)
	{
#if DEBUG
		var n = tile.GetNeighbors();
		foreach (Tile t in n)
			if (t != null)
				Debug.DrawLine(tile.transform.position, t.transform.position, Color.white, 3);
#endif
	}
}
