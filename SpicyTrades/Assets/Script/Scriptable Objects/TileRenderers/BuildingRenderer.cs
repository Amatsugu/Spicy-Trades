using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile Renderer/Building")]
public class BuildingRenderer : TileRenderer
{
	public Sprite sprite;
	public int sortOrder = 3;
	public Vector3 offset = new Vector3(0, -.5f, 0);

	public override void PostRender(Tile tile, object renderData)
	{

	}

	public override void RenderInit(Tile tile)
	{
		if (tile.GetType() == typeof(SettlementTile) && (tile as SettlementTile).Center != tile)
			return;
		var building = new GameObject();
		building.transform.parent = tile.ThisGameObject.transform;
		building.transform.localPosition = Vector3.zero + offset;
		building.transform.rotation = Quaternion.Euler(-45, 0, 0);
		var buildingSR = building.AddComponent<SpriteRenderer>();
		buildingSR.sprite = sprite;
		buildingSR.sortingOrder = 3;
	}
}
