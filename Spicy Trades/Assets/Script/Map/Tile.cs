using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
	public HexCoords position;
	public float cost;

	private TextMesh _pText;
	private SpriteRenderer _sprite;
	private Color _sCol;
	private Color _hCol;

	// Use this for initialization
	void Start()
	{
		_pText = GetComponentInChildren<TextMesh>();
		_sprite = GetComponent<SpriteRenderer>();
		_sCol = _sprite.color;
		_hCol = Color.white;
	}

	public Tile SetPos(int x, int y)
	{
		position = HexCoords.FromOffsetCoords(x, y);
		if (_pText == null)
			Start();
		//_pText.text = position.ToString();
		return this;
	}

	// Update is called once per frame
	void Update()
	{

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
		_sprite.color = _sCol;
	}

	public Tile SetColor(Color color)
	{
		_sprite.color = color;
		return this;
	}

	public Tile ResetColor()
	{
		_sprite.color = _sCol;
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
		tiles[0] = MapRenderer.GetTile(x - 1, y, z - 1);
		tiles[1] = MapRenderer.GetTile(x - 1, y + 1, z);
		tiles[2] = MapRenderer.GetTile(x, y + 1, z - 1);
		tiles[3] = MapRenderer.GetTile(x + 1, y, z - 1);
		tiles[4] = MapRenderer.GetTile(x + 1, y - 1, z);
		tiles[5] = MapRenderer.GetTile(x, y - 1, z - 1);
		return tiles;
	}

	// override object.Equals
	public override bool Equals(object obj)
	{
		//       
		// See the full list of guidelines at
		//   http://go.microsoft.com/fwlink/?LinkID=85237  
		// and also the guidance for operator== at
		//   http://go.microsoft.com/fwlink/?LinkId=85238
		//

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