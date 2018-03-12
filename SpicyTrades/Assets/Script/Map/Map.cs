using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using UnityEngine;

[Serializable]
public class Map : IEnumerable<Tile>
{
	public Tile[] Tiles { get; private set; }
	public List<TownTile> Towns { get; private set; }
	public TownTile Capital { get; private set; }
	public List<Player> Players { get; private set; }
	public Player CurrentPlayer { get; private set; }
	public int Height { get; private set; }
	public int Width { get; private set; }
	public int Length { get
		{
			return Height * Width;
		}
	}

	public readonly int Seed;

	public int TileCount
	{
		get
		{
			return Tiles.Length;
		}
	}

	public Map(int height, int width, int seed = 0)
	{
		Debug.Log(seed);
		UnityEngine.Random.InitState(seed);
		Height = height;
		Width = width;
		Tiles = new Tile[height * width];
		Towns = new List<TownTile>();
		Players = new List<Player>();
	}

	public Tile this[int i]
	{
		set
		{
			Tiles[i] = value;
		}
		get
		{
			return Tiles[i];
		}
	}

	public Tile this[int x, int y, int z]
	{
		get
		{
			if (-x - y != z)
				return null;
			int oX = x + y / 2;
			if (oX < 0 || oX >= Width)
				return null;
			if (y >= Height)
				return null;
			int index = x + y * Width + y / 2;
			if (index < 0 || index > Tiles.Length)
				return null;
			return Tiles[index];
		}
		
	}

	public Tile GetTile(int x, int y, int z)
	{
		return this[x, y, z];
	}

	public IEnumerable<TownTile> GetTowns()
	{
		return from Tile t in Tiles where t.GetType() == typeof(TownTile) select t as TownTile;
	}

	public string ToJSON() //TODO: Implement Proper Serialization
	{
		return JsonUtility.ToJson(this);
	}

	public static Map FromJSON(string json)
	{
		return JsonUtility.FromJson<Map>(json);
	}

	IEnumerator<Tile> IEnumerable<Tile>.GetEnumerator()
	{
		return ((IEnumerable<Tile>)Tiles).GetEnumerator();
	}

	public IEnumerator GetEnumerator()
	{
		return ((IEnumerable<Tile>)Tiles).GetEnumerator();
	}

	public TownTile MakeTown(Tile tile, TownTileInfo town)
	{
		var t = new TownTile(town, tile.parent, tile.Position, tile.outerRadius);
		this[tile.Position.ToIndex()] = t;
		Towns.Add(t);
		return t;
	}

	public TownTile MakeCapital(Tile tile, TownTileInfo capital)
	{
		tile.Destroy();
		this[tile.Position.ToIndex()] = Capital = new TownTile(capital, tile.parent, tile.Position, tile.outerRadius);
		foreach (Tile t in tile.GetNeighbors())
			ReplaceTile(t, capital);
		return Capital;
	}

	public Player AddPlayer(Player player, bool currentPlayer = false)
	{
		if (currentPlayer)
			CurrentPlayer = player;
		Players.Add(player);
		return player;
	}

	public void Destroy()
	{
		foreach(Tile t in this)
			t.Destroy();
		foreach (var player in Players)
			UnityEngine.Object.Destroy(player.gameObject);
	}

	public Tile ReplaceTile(Tile oldTile, TileInfo newTile, bool preserveColor = false, bool preserveCost = false)
	{
		var pos = oldTile.Position;
		var wPos = oldTile.WolrdPos;
		//var wRot = oldTile.transform.rotation;
		var cost = oldTile.Cost;
		var col = oldTile.tileInfo.color;
		oldTile.Destroy();
		Tile nTile;
		switch(newTile.tileType)
		{
			case TileType.Tile:
				nTile = new Tile(newTile, oldTile.parent, pos, oldTile.outerRadius);
				break;
			case TileType.Resource:
				nTile = new ResourceTile(newTile as ResourceTileInfo, oldTile.parent, pos, oldTile.outerRadius);
				break;
			case TileType.Town:
				nTile = new TownTile(newTile as TownTileInfo, oldTile.parent, pos, oldTile.outerRadius);
				break;
			default:
				nTile = new Tile(newTile, oldTile.parent, pos, oldTile.outerRadius);
				break;
		}
		//var g = GameObject.Instantiate(newTile, wPos, wRot, oldTile.transform.parent);
		if (preserveColor)
			nTile.SetColor(col, true);
		this[pos.ToIndex()] = nTile;
		if (preserveCost)
			nTile.SetWeight(cost);
		return nTile;
	}
}
