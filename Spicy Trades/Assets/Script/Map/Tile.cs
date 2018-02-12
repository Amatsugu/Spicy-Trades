using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
	public int x, y;
	private TextMesh _pText;
	private SpriteRenderer _sprite;

	// Use this for initialization
	void Start()
	{
		_pText = GetComponentInChildren<TextMesh>();
		_sprite = GetComponent<SpriteRenderer>();
	}

	public Tile SetPos(int x, int y)
	{
		this.x = x;
		this.y = y;
		if (_pText == null)
			Start();
		_pText.text = "(" + x + ", " + y + ")";
		return this;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public Tile SetColor(Color color)
	{
		_sprite.color = color;
		return this;
	}

	public Tile[] GetNeighboringTiles()
	{
		return GetNeighboringTiles(this);
	}

	public static Tile[] GetNeighboringTiles(Tile t)
	{
		return GetNeighboringTiles(t.x, t.y);
	}

	public static Tile[] GetNeighboringTiles(int x, int y)
	{
		var tiles = new Tile[6];
		tiles[0] = MapRenderer.GetTile(x, y + 1);
		tiles[1] = MapRenderer.GetTile(x + 1, y);
		tiles[2] = MapRenderer.GetTile(x + 1, y + 1);
		tiles[3] = MapRenderer.GetTile(x, y - 1);
		tiles[4] = MapRenderer.GetTile(x - 1, y);
		tiles[5] = MapRenderer.GetTile(x - 1, y + 1);
		return tiles;
	}
}