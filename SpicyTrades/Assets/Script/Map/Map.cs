using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Map
{

	public Tile[] Tiles { get; private set; }
	public int Height { get; private set; }
	public int Width{ get; private set; }
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
}
