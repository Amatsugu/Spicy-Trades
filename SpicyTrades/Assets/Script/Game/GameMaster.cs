using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster
{
	public static GameMaster Instance
	{
		get
		{
			return _instance ?? (_instance = new GameMaster());
		}
	}
	public static Map GameMap
	{
		get
		{
			return Instance._map;
		}
	}
	public static Player Player
	{
		get
		{
			return Instance._map.CurrentPlayer;
		}
	}

	public static List<Player> Players
	{
		get
		{
			return Instance._map.Players;
		}
	}

	public static MapGenerator Generator
	{
		get
		{
			return Instance._generator;
		}
	}

	private static GameMaster _instance;

	private Map _map;
	private MapGenerator _generator;
	private Dictionary<SettlementTile, Dictionary<ResourceTileInfo, float>> _resourceValueCache;

	public static void SetGenerator(MapGenerator generator)
	{
		Instance._generator = generator;
	}
	
	public static void CachePrices(SettlementTile settlement)
	{
		var rvc = Instance._resourceValueCache;
		var rc = settlement.ResourceCache;
		if(rvc.ContainsKey(settlement))
		{
			AddToCache(rvc[settlement], rc);
		}else
		{
			AddToCache(rvc[settlement], rc);
		}
	}

	private static void AddToCache(Dictionary<ResourceTileInfo, float> vc, Dictionary<ResourceTileInfo, float[]> rc)
	{
		foreach (var res in rc.Keys)
		{
			if (vc.ContainsKey(res))
				vc[res] = rc[res][1];
			else
				vc.Add(res, rc[res][1]);
		}
	}

	public static void SetMap(Map map)
	{
		Instance._map = map;
	}

	public static Tile GetTile(int x, int y, int z)
	{
		return Instance._map[x, y, z];
	}

	public static void TouchTile(Tile tile)
	{
		if (tile.GetType() == typeof(SettlementTile))
		{
			Instance._map.CurrentPlayer.MoveTo(tile as SettlementTile);
		}
#if DEBUG
		var n = tile.GetNeighbors();
		foreach (Tile t in n)
			if (t != null)
				Debug.DrawLine(tile.WolrdPos, t.WolrdPos, Color.white, 3);
#endif
	}
}
