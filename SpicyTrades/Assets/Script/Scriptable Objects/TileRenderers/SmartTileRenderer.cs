using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile Renderer/Smart Tile")]
public class SmartTileRenderer : TileRenderer
{
	public SmartTileType tileType;
	public String[] connectTo;
	public override void RenderInit(Tile tile)
	{
		SpriteRenderer[] sides = new SpriteRenderer[6];
		if (tileType == null)
			return;
		var n = tile.GetNeighbors();
		for (int i = 0; i < n.Length; i++)
		{
			RenderSide(i, sides, tile);
			if (n[i] == null)
				continue;
		}
		tile.SetRenderData(name, sides);
	}

	void RenderSide(int side, SpriteRenderer[] sides, Tile tile)
	{
		var g = new GameObject
		{
			name = side.ToString()
		};
		var sr = sides[side] = g.AddComponent<SpriteRenderer>();
		sr.sprite = tileType.GetSprite(side);
		sr.sortingOrder = 1;
		g.transform.parent = tile.ThisTransform;
		g.transform.localPosition = Vector3.zero;
		if (tileType.dualSprite)
		{
			if (side >= 2 && side < 5)
				sr.flipX = true;
			if (side > 3)
				sr.flipY = true;
		}
		if (tileType.inheritColor)
			sr.color = tile.GetColor();
	}

	public void UpdateSides(SpriteRenderer[] sides, Tile tile)
	{
		var n = tile.GetNeighbors();
		for (int i = 0; i < n.Length; i++)
		{
			if (n[i] == null)
			{
				sides[i].enabled = !tileType.invert;
				continue;
			}
			if (connectTo.Contains(n[i].Tag))
			{
				sides[i].enabled = tileType.invert;
				continue;
			}
			if (!n[i].tileInfo.tileRenderers.Any(r => r != null && r.GetType() == typeof(SmartTileRenderer)))
			{
				sides[i].enabled = !tileType.invert;
				continue;
			}
			if (!tile.tileInfo.tileRenderers.Any(r => r.GetType() == typeof(SmartTileRenderer) && (r as SmartTileRenderer).tileType == tileType))
			{
				sides[i].enabled = tileType.invert;
				continue;
			}
			else
			{
				sides[i].enabled = !tileType.invert;
				continue;
			}

		}
		if (tileType.invert)
			return;
		bool[] activations = new bool[sides.Length];
		for (int i = 0; i < sides.Length; i++)
		{
			int iP = (i - 1 < 0) ? sides.Length - 1 : i - 1;
			int iN = (i + 1) % sides.Length;
			var l = sides[iP];
			var r = sides[iN];
			var c = sides[i];
			if (sides[i].enabled)
				continue;
			if (!l.enabled && !r.enabled)
				continue;
			if (i == 0 || i == 3)
			{
				c.sprite = tileType.GetCorner(i, l.enabled && r.enabled);
				activations[i] = true;
				if (l.enabled != r.enabled)
					c.flipY = (i == 0) ? !r.enabled : !l.enabled;
				continue;
			}
			else
			{
				if (l.enabled && r.enabled)
				{
					c.sprite = tileType.GetCorner(i, true);
					activations[i] = true;
					continue;
				}
				else if (r.enabled && !l.enabled)
				{
					var right = true;
					if (c.flipX)
						right = !right;
					if (c.flipY)
						right = !right;
					c.sprite = tileType.GetCorner(i, false, right);
					activations[i] = true;
					continue;
				}
				else if (!r.enabled && l.enabled)
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
				sides[i].enabled = true;
		}
	}

	public override void PostRender(Tile tile, object renderData)
	{
		UpdateSides(renderData as SpriteRenderer[], tile);
	}
}
