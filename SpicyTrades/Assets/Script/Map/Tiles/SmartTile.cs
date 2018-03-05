using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SmartTile : Tile {

	public SmartTileType tileType;
	public string[] connectTo;

	private bool _startUpdate = false;
	private SpriteRenderer[] _sides = new SpriteRenderer[6];
	// Use this for initialization
	void Start ()
	{
		if (tileType == null)
			return;
		var n = GetNeighbors();
		for (int i = 0; i < n.Length; i++)
		{
			RenderSide(i);
			if (n[i] == null)
				continue;
			//if (n[i].GetType() == typeof(SmartTile))
			//	(n[i] as SmartTile).UpdateSides();
		}
	}

	private void LateUpdate()
	{
		if(!_startUpdate)
		{
			_startUpdate = true;
			UpdateSides();
		}
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

	public void UpdateSides()
	{
		var n = GetNeighbors();
		for (int i = 0; i < n.Length; i++)
		{
			if (n[i] == null)
			{
				_sides[i].enabled = !tileType.invert;
				continue;
			}
			if(connectTo.Contains(n[i].tag))
			{
				_sides[i].enabled = tileType.invert;
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
			}else
			{
				_sides[i].enabled = !tileType.invert;
				continue;
			}

		}
		if (tileType.invert)
			return;
		bool[] activations = new bool[_sides.Length];
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
				activations[i] = true;
				if(l.enabled != r.enabled)
					c.flipY = (i == 0) ? !r.enabled : !l.enabled;
				continue;
			}else
			{
				if (l.enabled && r.enabled)
				{
					c.sprite = tileType.GetCorner(i, true);
					activations[i] = true;
					continue;
				}else if(r.enabled && !l.enabled)
				{
					var right = true;
					if (c.flipX)
						right = !right;
					if (c.flipY)
						right = !right;
					c.sprite = tileType.GetCorner(i, false, right);
					activations[i] = true;
					continue;
				}else if(!r.enabled && l.enabled)
				{
					var right = false;
					if (c.flipX)
						right = !right;
					if (c.flipY)
						right = !right;
					c.sprite = tileType.GetCorner(i, false, right);
					activations[i] = true;
					continue;
				}
			}
		}

		for (int i = 0; i < activations.Length; i++)
		{
			if (activations[i] == true)
				_sides[i].enabled = true;
		}
	}
	
}
