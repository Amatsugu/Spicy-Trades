using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile Renderer/Mill")]
public class MillRenderer : TileRenderer
{
	public Sprite millBase;
	public Sprite millBlade;
	public float speed;
	public Vector3 bladeOffset;
	public int sortOrder = 3;

	public override void PostRender(Tile tile, object renderData)
	{
	}

	public override void RenderInit(Tile tile)
	{
		var body = new GameObject().AddComponent<SpriteRenderer>();
		body.transform.parent = tile.ThisTransform;
		body.transform.localPosition = Vector3.zero;
		body.transform.rotation = Quaternion.Euler(-45, 0, 0);
		body.sprite = millBase;
		body.sortingOrder = sortOrder;
		var blade = new GameObject().AddComponent<SpriteRenderer>();
		blade.gameObject.AddComponent<Rotator>().speed = speed;
		blade.transform.parent = body.transform;
		blade.transform.localPosition = bladeOffset;
		blade.sprite = millBlade;
		blade.sortingOrder = sortOrder + 1;
	}
}
