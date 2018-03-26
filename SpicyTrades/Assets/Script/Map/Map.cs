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
	public List<SettlementTile> Towns { get; private set; }
	public SettlementTile Capital { get; private set; }
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
		UnityEngine.Random.InitState(seed);
		Height = height;
		Width = width;
		Tiles = new Tile[height * width];
		Towns = new List<SettlementTile>();
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

	public Tile this[HexCoords tile]
	{
		get
		{
			return this[tile.X, tile.Y, tile.Z];
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

	public IEnumerable<SettlementTile> GetTowns()
	{
		return from Tile t in Tiles where t.GetType() == typeof(SettlementTile) select t as SettlementTile;
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

	public SettlementTile MakeTown(Tile tile, SettlementTileInfo town)
	{
		var t = new SettlementTile(town, tile.parent, tile.Position, tile.outerRadius);
		this[tile.Position.ToIndex()] = t;
		Towns.Add(t);
		return t;
	}

	public SettlementTile MakeCapital(Tile tile, SettlementTileInfo capital)
	{
		tile.Destroy();
		this[tile.Position.ToIndex()] = Capital = new SettlementTile(capital, tile.parent, tile.Position, tile.outerRadius);
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

	public byte[] Simulate(int ticks)
	{
		for (int i = 0; i < ticks; i++)
		{
			foreach (var town in Towns)
				town.Simulate();
			foreach (var town in Towns) //TODO: Do we do this
				town.NegotiateTrade();
		}
		return null;
	}

	public void Sync(byte[] simData)
	{

	}

	public Tile ReplaceTile(Tile oldTile, TileInfo newTile, bool preserveColor = false, bool preserveCost = false)
	{
		return ReplaceTile<Tile>(oldTile, newTile, preserveColor, preserveCost);
	}

	public T ReplaceTile<T>(Tile oldTile, TileInfo newTile, bool preserveColor = false, bool preserveCost = false) where T : Tile
	{
		var pos = oldTile.Position;
		var wPos = oldTile.WolrdPos;
		//var wRot = oldTile.transform.rotation;
		oldTile.Destroy();
		T nTile;
		switch(newTile.TileType)
		{
			case TileType.Resource:
				nTile = new ResourceTile(newTile as ResourceTileInfo, oldTile.parent, pos, oldTile.outerRadius) as T;
				break;
			case TileType.Tile:
				nTile = new Tile(newTile, oldTile.parent, pos, oldTile.outerRadius) as T;
				break;
			case TileType.Settlement:
				nTile = new SettlementTile(newTile as SettlementTileInfo, oldTile.parent, pos, oldTile.outerRadius) as T;
				break;
			default:
				nTile = new Tile(newTile, oldTile.parent, pos, oldTile.outerRadius) as T;
				break;
		}
		if (preserveColor)
			nTile.SetColor(oldTile.tileInfo.color, true);
		if (preserveCost)
			nTile.SetWeight(oldTile.Cost);
		this[pos.ToIndex()] = nTile;
		return nTile;
	}
}
