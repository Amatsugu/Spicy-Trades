using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile Renderer/Color")]
public class TileColorRenderer : TileRenderer
{
	public Color color;

	public override void PostRender(Tile tile, object renderData)
	{
	}

	public override void RenderInit(Tile tile)
	{
		tile.SetColor(color, true);
	}
}
