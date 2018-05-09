using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile Renderer/Resource Icon")]
public class ResourceIconRenderer : TileRenderer
{
	public Sprite renderMask;

	public override void PostRender(Tile tile, object renderData)
	{
	}

	public override void RenderInit(Tile tile)
	{
		if (tile.GetType() != typeof(ResourceTile))
			return;
		var mask = tile.ThisGameObject.AddComponent<SpriteMask>();
		mask.sprite = renderMask;
		var sprite = new GameObject();
		sprite.transform.parent = tile.ThisTransform;
		sprite.transform.localPosition = Vector3.zero;
		sprite.transform.localScale = new Vector3(.6f, .6f, .6f);
		var sr = sprite.AddComponent<SpriteRenderer>();
		sr.sortingOrder = 3;
		sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
		sr.sprite = (tile.tileInfo as ResourceTileInfo).icon;
	}
}
