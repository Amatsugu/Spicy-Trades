using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
	public HexCoords Position { get; private set; }
	public float Cost;
	public List<TileRenderer> tileRenderers = new List<TileRenderer>();
	public Vector3 WolrdPos
	{
		get
		{
			return transform.position;
		}
	}

	private TextMesh _pText;
	protected SpriteRenderer _sprite;
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

	public virtual void TileInit()
	{

	}

	public virtual void TileRender()
	{
		if (tileRenderers == null)
			return;
		foreach (var tr in tileRenderers)
			if (tr == null)
				continue;
			else
				tr.RenderInit(this);
	}

	public void RenderReset()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			Destroy(transform.GetChild(i).gameObject);
		}
	}


	public Tile SetPos(int x, int y)
	{
		return SetPos(HexCoords.FromOffsetCoords(x, y));
	}

	public Tile SetPos(HexCoords coords)
	{
		Position = coords;
		if (_pText == null)
			Start();
		return this;
	}

	public Tile SetText(string text)
	{
		_pText.text = text;
		return this;
	}

	public Tile SetWeight(float cost)
	{
		this.Cost = cost;
		//_pText.text = cost.ToString();
		return this;
	}

	public Color GetColor()
	{
		return _sCol;
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

	public Tile ResetText()
	{
		_pText.text = "";
		return this;
	}

	public float DistanceTo(Tile t)
	{
		return Vector3.Distance(WolrdPos, t.WolrdPos);
	}

	public Tile[] GetNeighbors()
	{
		return GetNeighbors(this);
	}

	public static Tile[] GetNeighbors(Tile t)
	{
		return GetNeighbors(t.Position);
	}

	public static Tile[] GetNeighbors(HexCoords pos)
	{
		return GetNeighbors(pos.X, pos.Y, pos.Z);
	}

	public static Tile[] GetNeighbors(int x, int y, int z)
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

		return Position == (obj as Tile).Position;
	}

	// override object.GetHashCode
	public override int GetHashCode()
	{
		return Position.GetHashCode();
	}
}
