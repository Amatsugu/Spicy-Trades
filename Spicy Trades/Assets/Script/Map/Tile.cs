using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
	public HexCoords position;
	public float cost;
	public Vector3 wolrdPos
	{
		get
		{
			return transform.position;
		}
	}

	private TextMesh _pText;
	private SpriteRenderer _sprite;
	private Color _sCol;
	private Color _hCol;
	private Color _curCol;

	// Use this for initialization
	void Start()
	{
		_pText = GetComponentInChildren<TextMesh>();
		_sprite = GetComponent<SpriteRenderer>();
		_curCol = _sCol = _sprite.color;
		_hCol = Color.white;
	}

	public Tile SetPos(int x, int y)
	{
		position = HexCoords.FromOffsetCoords(x, y);
		if (_pText == null)
			Start();
		return this;
	}

	public Tile SetWeight(float cost)
	{
		this.cost = cost;
		//_pText.text = cost.ToString();
		return this;
	}

	private void OnMouseUp()
	{
		MapRenderer.TouchTile(this);
	}

	private void OnMouseEnter()
	{
		_sprite.color = _hCol;
	}

	private void OnMouseExit()
	{
		_sprite.color = _curCol;
	}

	public Tile SetColor(Color color)
	{
		_sprite.color = _curCol = color;
		return this;
	}

	public Tile ResetColor()
	{
		_sprite.color = _curCol = _sCol;
		return this;
	}

	public Tile[] GetNeighboringTiles()
	{
		return GetNeighboringTiles(this);
	}

	public static Tile[] GetNeighboringTiles(Tile t)
	{
		return GetNeighboringTiles(t.position);
	}

	public static Tile[] GetNeighboringTiles(HexCoords pos)
	{
		return GetNeighboringTiles(pos.X, pos.Y, pos.Z);
	}

	public static Tile[] GetNeighboringTiles(int x, int y, int z)
	{
		var tiles = new Tile[6];
		tiles[0] = MapRenderer.GetTile(x - 1, y, z + 1); //Left
		tiles[1] = MapRenderer.GetTile(x - 1, y + 1, z); //Top Left
		tiles[2] = MapRenderer.GetTile(x, y + 1, z - 1); //Top Right
		tiles[3] = MapRenderer.GetTile(x + 1, y, z - 1); //Right
		tiles[4] = MapRenderer.GetTile(x + 1, y - 1, z); //Bottom Right
		tiles[5] = MapRenderer.GetTile(x, y - 1, z + 1); //Bottom Left
		return tiles;
	}

	// override object.Equals
	public override bool Equals(object obj)
	{
		if (obj == null || GetType() != obj.GetType())
		{
			return false;
		}

		return position == (obj as Tile).position;
	}

	// override object.GetHashCode
	public override int GetHashCode()
	{
		return position.GetHashCode();
	}
}
