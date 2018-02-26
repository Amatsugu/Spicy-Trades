using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapRenderer : MonoBehaviour
{
	public MapGenerator generator;
	public GameObject smartTile;
	public Map map;
	public static MapRenderer Instance;

	private Tile A, B;
	private bool endPoint = true;
	private static Tile[] oldPath = null;
    // Use this for initialization
    void Start()
    {
		Instance = this;
		generator.GenerateMap(map = new Map((int)generator.Size.x,  (int)generator.Size.y), transform);
		generator.GenerateFeatures(map);
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
		/*var pos = tile.Position;
		var wPos = tile.transform.position;
		var wRot = tile.transform.rotation;
		var cost = tile.Cost;
		var col = tile.GetColor();
		var g = Instantiate(Instance.smartTile, wPos, wRot, tile.transform.parent);
		g.GetComponent<SpriteRenderer>().color = col;
		Instance.map[pos.ToIndex()] = g.GetComponent<Tile>().SetPos(pos).SetWeight(cost);*/
		//Map.Tiles[pos.ToIndex()] = g.AddComponent<SmartTile>().SetPos(pos).SetWeight(cost);
		Instance.map.ReplaceTile(tile, Instance.smartTile, true, true);

#if DEBUG
		var n = tile.GetNeighbors();
		foreach (Tile t in n)
			if (t != null)
				Debug.DrawLine(tile.transform.position, t.transform.position, Color.white, 3);
#endif
		/*
		if (Map.endPoint)
			Map.A = tile;
		else
			Map.B = tile;
		if (oldPath != null)
			foreach (var t in oldPath)
				t.ResetColor();
		Map.endPoint = !Map.endPoint;
		if(Map.A != null && Map.B != null)
		{
			oldPath = Pathfinder.FindPath(Map.A, Map.B);
			for(int i = 0; i < oldPath.Length; i++)
			{
				oldPath[i].SetColor(Color.white);
				if(i>0)
					Debug.DrawLine(oldPath[i-1].transform.position, oldPath[i].transform.position, Color.red, 3);
			}
		}
		if(Map.A != null)
			Map.A.SetColor(Color.green);
		if(Map.B != null)
			Map.B.SetColor(Color.grey);*/
	}
}
