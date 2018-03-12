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
	public int Height { get; private set; }
	public int Width { get; private set; }
	public int Length { get
		{
			return Height * Width;
		}
	}

	public int TileCount
	{
		get
		{
			return Tiles.Length;
		}
	}

	public Map(int height, int width)
	{
		Height = height;
		Width = width;
		Tiles = new Tile[height * width];
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

	public string ToJSON()
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

	public TownTile MakeTown(Tile tile, GameObject town)
	{

		return ReplaceTile(tile, town) as TownTile;
	}

	public Tile ReplaceTile(Tile oldTile, GameObject newTile, bool preserveColor = false, bool preserveCost = false)
	{
		var pos = oldTile.Position;
		var wPos = oldTile.transform.position;
		var wRot = oldTile.transform.rotation;
		var cost = oldTile.Cost;
		var col = oldTile.GetColor();
		GameObject.Destroy(oldTile.gameObject);
		var g = GameObject.Instantiate(newTile, wPos, wRot, oldTile.transform.parent);
		if(preserveColor)
			g.GetComponent<SpriteRenderer>().color = col;
		var t = this[pos.ToIndex()] = g.GetComponent<Tile>().SetPos(pos);
		if (preserveCost)
			t.SetWeight(cost);
		return t;
	}
}
