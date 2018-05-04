using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile Renderer/Resource Icon")]
public class ResourceIconRenderer : TileRenderer
{
	public override void PostRender(Tile tile, object renderData)
	{
	}

	public override void RenderInit(Tile tile)
	{
		if (tile.GetType() != typeof(ResourceTile))
			return;
		var sprite = new GameObject();
		sprite.transform.parent = tile.ThisTransform;
		sprite.transform.localPosition = Vector3.zero;
		sprite.transform.localScale = new Vector3(.8f, .8f, .8f);
		var sr = sprite.AddComponent<SpriteRenderer>();
		sr.sortingOrder = 3;
		sr.sprite = (tile.tileInfo as ResourceTileInfo).icon;
	}
}
