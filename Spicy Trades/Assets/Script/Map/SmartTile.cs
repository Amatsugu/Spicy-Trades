using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartTile : Tile {

	public SmartTileType tileType;


	private SpriteRenderer[] _sides = new SpriteRenderer[6];
	// Use this for initialization
	void Start ()
	{
		if (tileType == null)
			return;
		var n = GetNeighboringTiles();
		for (int i = 0; i < n.Length; i++)
		{
			RenderSide(i);
			if (n[i] == null)
				continue;
			if (n[i].GetType() == typeof(SmartTile))
				(n[i] as SmartTile).UpdateSides();
		}
		UpdateSides();
	}

	void RenderSide(int side)
	{
		var g = new GameObject
		{
			name = side.ToString()
		};
		var sr = _sides[side] = g.AddComponent<SpriteRenderer>();
		sr.sprite = tileType.GetSprite(side);
		sr.sortingOrder = 1;
		g.transform.parent = transform;
		g.transform.localPosition = Vector3.zero;
		if(tileType.dualSprite)
		{
			if (side >= 2 && side < 5)
				sr.flipX = true;
			if (side > 3)
				sr.flipY = true;
		}
		if (tileType.inheritColor)
			sr.color = _sprite.color;
	}

	void UpdateSides()
	{
		var n = GetNeighboringTiles();
		for (int i = 0; i < n.Length; i++)
		{
			if (n[i] == null)
			{
				_sides[i].enabled = !tileType.invert;
				continue;
			}
			if (n[i].GetType() != typeof(SmartTile))
			{
				
				_sides[i].enabled = !tileType.invert;
				continue;
			}
			var t = n[i] as SmartTile;
			if (t.tileType == tileType)
			{
				_sides[i].enabled = tileType.invert;
				continue;
			}
		}

		for (int i = 0; i < _sides.Length; i++)
		{
			int iP = (i - 1 < 0) ? _sides.Length - 1 : i -1;
			int iN = (i + 1) % _sides.Length;
			var l = _sides[iP];
			var r = _sides[iN];
			var c = _sides[i];
			if (_sides[i].enabled)
				continue;
			if (!l.enabled && !r.enabled)
				continue;
			if (i == 0 || i == 3)
			{
				c.sprite = tileType.GetCorner(i, l.enabled && r.enabled);
				c.enabled = true;
				if(l.enabled != r.enabled)
					c.flipY = (i == 0) ? !r.enabled : !l.enabled;
				continue;
			}else
			{
				//c.sprite = tileType.GetCorner(i, l.enabled && r.enabled);
				//c.enabled = true;
				//continue;
			}
		}
	}
	
}
