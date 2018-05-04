using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile Renderer/UnTiled")]
public class UnTilledRenderer : TileRenderer {
	public override void PostRender(Tile tile, object renderData)
	{
	}

	public override void RenderInit(Tile tile)
	{
		var sr = tile.ThisGameObject.GetComponent<SpriteRenderer>();
		sr.flipX = Mathf.PerlinNoise(tile.WolrdPos.x, tile.WolrdPos.y) > .5f;
		sr.flipY = Mathf.PerlinNoise(tile.WolrdPos.y, tile.WolrdPos.x) > .5f;
	}

	private bool IsEven(float value)
	{
		return (value/2f) == (((int)value) / 2f);
	}
}
