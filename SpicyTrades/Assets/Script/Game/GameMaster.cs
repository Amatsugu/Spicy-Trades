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
		if (Instance._resourceValueCache == null)
			Instance._resourceValueCache = new Dictionary<SettlementTile, Dictionary<ResourceTileInfo, float>>();
		var resourceValueCache = Instance._resourceValueCache;
		var resourceCache = settlement.ResourceCache;
		if (resourceCache == null)
			return;
		if(resourceValueCache.ContainsKey(settlement))
		{
			AddToCache(resourceValueCache[settlement], resourceCache);
		}else
		{
			var valueCache = new Dictionary<ResourceTileInfo, float>();
			resourceValueCache.Add(settlement, valueCache);
			AddToCache(valueCache, resourceCache);
		}
	}

	private static void AddToCache(Dictionary<ResourceTileInfo, float> valueCache, Dictionary<ResourceTileInfo, float[]> resourceCache)
	{
		foreach (var res in resourceCache.Keys)
		{
			if (valueCache.ContainsKey(res))
				valueCache[res] = resourceCache[res][1];
			else
				valueCache.Add(res, resourceCache[res][1]);
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

			UIManager.ShowPricePanel(tile as SettlementTile);
			//Instance._map.CurrentPlayer.MoveTo(tile as SettlementTile);
		}
#if DEBUG
		var n = tile.GetNeighbors();
		foreach (Tile t in n)
			if (t != null)
				Debug.DrawLine(tile.WolrdPos, t.WolrdPos, Color.white, 3);
#endif
	}
}
