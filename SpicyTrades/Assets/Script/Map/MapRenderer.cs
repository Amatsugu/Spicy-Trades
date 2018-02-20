using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapRenderer : MonoBehaviour
{
	public MapGenerator generator;
	public GameObject smartTile;
	public Tile[] Tiles;
	public static MapRenderer Map;

	private Tile A, B;
	private bool endPoint = true;
	private static Tile[] oldPath = null;
    // Use this for initialization
    void Start()
    {
		Map = this;
		Tiles = new Tile[(int)(generator.Size.x * generator.Size.y)];
        for (int y = 0, i = 0; y < generator.Size.y; y++)
        {
            for (int x = 0; x < generator.Size.x; x++)
            {
				Tiles[i++] = generator.Generate(x, y, transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
		if (!generator.Regen)
			return;
		Debug.Log("Regen");
		generator.Regen = false;
		for (int i = 0; i < transform.childCount; i++)
		{
			Destroy(transform.GetChild(i).gameObject);
		}
		Start();
    }

	public static Tile GetTile(int x, int y, int z)
	{
		if (-x - y != z)
			return null;
		int oX = x + y / 2;
		if (oX < 0 || oX >= Map.generator.Size.x)
			return null;
		if (y >= Map.generator.Size.y)
			return null;
		int index = x + y * (int)Map.generator.Size.x + y / 2;
		if (index < 0 || index > Map.Tiles.Length)
			return null;
		return Map.Tiles[index];
	}

	public static void TouchTile(Tile tile)
	{
		var pos = tile.position;
		var wPos = tile.transform.position;
		var wRot = tile.transform.rotation;
		var cost = tile.cost;
		var col = tile.GetColor();
		var g = Instantiate(Map.smartTile, wPos, wRot, tile.transform.parent);
		g.GetComponent<SpriteRenderer>().color = col;
		Map.Tiles[pos.ToIndex()] = g.GetComponent<Tile>().SetPos(pos).SetWeight(cost);
		//Map.Tiles[pos.ToIndex()] = g.AddComponent<SmartTile>().SetPos(pos).SetWeight(cost);

#if DEBUG
		var n = tile.GetNeighboringTiles();
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
