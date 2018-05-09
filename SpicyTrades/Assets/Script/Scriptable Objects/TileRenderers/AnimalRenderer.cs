using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile Renderer/Animal")]
public class AnimalRenderer : TileRenderer
{
	public Sprite animalSprite;
	public int count;
	public float range;
	public int sortOrder = 3;
	public float minActionTime = 2;
	public float maxActionTime = 5;
	public float speed = 0.5f;

	public override void PostRender(Tile tile, object renderData)
	{
		
	}

	public override void RenderInit(Tile tile)
	{
		var animals = new List<AnimalData>();
		for (int i = 0; i < count; i++)
		{
			var animalGO = new GameObject();
			var transform = animalGO.transform;
			transform.parent = tile.ThisTransform;
			transform.localPosition = new Vector3(Random.Range(-range, range), Random.Range(-range, range));
			transform.rotation = Quaternion.Euler(-45, 0, 0);
			var sr = animalGO.AddComponent<SpriteRenderer>();
			sr.flipX = Random.Range(0, 1) == 1;
			sr.sprite = animalSprite;
			sr.sortingOrder = sortOrder;
			var animal = animalGO.AddComponent<AnimalData>();
			animal.GameObject = animalGO;
			animal.Transform = transform;
			animal.NextActionTime = Time.time + Random.Range(minActionTime, maxActionTime);
			animal.SpriteRenderer = sr;
			animal.Speed = speed;
			animals.Add(animal);
		}
		tile.SetRenderData(nameof(AnimalRenderer), animals);
	}

	public override void RenderUpdate(Tile tile, object renderData)
	{
		var data = renderData as List<AnimalData>;
		foreach(var animal in data)
		{
			if (animal.NextActionTime > Time.time)
				continue;
			animal.NextActionTime = Time.time + Random.Range(minActionTime, maxActionTime);
			var range = this.range / 2;
			var newPos = new Vector3(Random.Range(-range, range), Random.Range(-range, range));
			animal.SpriteRenderer.flipX = newPos.x > animal.Transform.position.x;
			animal.WalkTo(newPos);
		}
	}

	
}
