using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
	public HexCoords Position { get; private set; }
	public float Cost { get; private set; }
	public Vector3 WolrdPos { get; private set; }
	[JsonIgnore]
	public GameObject ThisGameObject { get; private set; }
	internal void Hover()
	{
		_sprite.color = _hCol;
	}

	internal void Blur()
	{
		_sprite.color = _curCol;
	}

	public Transform ThisTransform { get; private set; }
	public string Tag
	{
		get
		{
			return tileInfo.tag;
		}
	}
	[JsonIgnore]
	public TileInfo tileInfo;

	[JsonIgnore]
	public Transform parent;
	public float outerRadius;


	[JsonIgnore]
	private Dictionary<string, object> _renderData;
	protected SpriteRenderer _sprite;
	private Color _sCol;
	private Color _hCol;
	private Color _curCol;
	private bool _colorOveride;

	public Tile(TileInfo tileInfo, Transform parent, HexCoords hexCoords, float outerRadius)
	{
		this.tileInfo = tileInfo;
		this.parent = parent;
		this.outerRadius = outerRadius;
		Cost = tileInfo.cost;
		WolrdPos = GetPosition(hexCoords.OffsetX, hexCoords.OffsetY);
		Position = hexCoords;
	}

	private float _innerRadius
	{
		get
		{
			return outerRadius * Mathf.Sqrt(3) / 2;
		}
	}
	private Vector3 GetPosition(int x, int y)
	{
		return new Vector3
		{
			y = y * (_innerRadius * 1.5f),
			x = (x + y * .5f - y / 2) * (_innerRadius * 2f),
		};
	}

	public virtual void TileRender()
	{
		ThisGameObject = new GameObject(tileInfo.GetType().ToString())
		{
			tag = Tag
		};
		ThisTransform = ThisGameObject.transform;
		ThisTransform.position = WolrdPos;
		ThisTransform.parent = parent;
		_sprite = ThisGameObject.AddComponent<SpriteRenderer>();
		_sprite.sprite = tileInfo.sprite;
		var collider = ThisGameObject.AddComponent<PolygonCollider2D>();
		var points = new List<Vector2>();
		tileInfo.sprite.GetPhysicsShape(0, points);
		collider.SetPath(0, points.ToArray());
		ThisGameObject.AddComponent<TileToucher>().target = this;
		if(!_colorOveride)
			_sprite.color = _curCol = _sCol = tileInfo.color;
		else
			_sprite.color = _curCol = _sCol;
		_hCol = Color.white;
		if (tileInfo.tileRenderers == null)
			return;
		_renderData = new Dictionary<string, object>();
		foreach (var tr in tileInfo.tileRenderers)
			if (tr == null)
				continue;
			else
			{
				_renderData.Add(tr.name, null);
				tr.RenderInit(this);
			}
	}

	public virtual void PostRender()
	{
		if (tileInfo.tileRenderers == null)
			return;
		foreach (var tr in tileInfo.tileRenderers)
			if (tr == null)
				continue;
			else
				tr.PostRender(this, _renderData[tr.name]);
	}

	public virtual void RenderUpdate()
	{
		if (tileInfo.tileRenderers == null)
			return;
		foreach (var tr in tileInfo.tileRenderers)
			if (tr == null)
				continue;
			else
				tr.RenderUpdate(this, _renderData[tr.name]);
	}

	public void SetRenderData(string name, object data)
	{
		_renderData[name] = data;
	}

	public void RenderReset()
	{
		for (int i = 0; i < ThisTransform.childCount; i++)
		{
			UnityEngine.Object.Destroy(ThisTransform.GetChild(i).gameObject);
		}
	}

	public Tile SetWeight(float cost)
	{
		this.Cost = cost;
		return this;
	}

	public Color GetColor()
	{
		return _sCol;
	}


	public Tile SetColor(Color color, bool permanent = false)
	{
		if (_sprite != null)
			_sprite.color = color;
		_curCol = color;
		if (permanent)
		{
			_sCol = color;
			_colorOveride = true;
		}
		return this;
	}

	public Tile ResetColor()
	{
		_sprite.color = _curCol = _sCol;
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
		tiles[0] = GameMaster.GetTile(x - 1, y, z + 1); //Left
		tiles[1] = GameMaster.GetTile(x - 1, y + 1, z); //Top Left
		tiles[2] = GameMaster.GetTile(x, y + 1, z - 1); //Top Right
		tiles[3] = GameMaster.GetTile(x + 1, y, z - 1); //Right
		tiles[4] = GameMaster.GetTile(x + 1, y - 1, z); //Bottom Right
		tiles[5] = GameMaster.GetTile(x, y - 1, z + 1); //Bottom Left
		return tiles;
	}

	public void Destroy()
	{
		UnityEngine.Object.Destroy(ThisGameObject);
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
