using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapRenderer : MonoBehaviour
{
	public MapGenerator generator;
	public GameObject smartTile;
	public Map map;

    // Use this for initialization
    void Awake()
    {
		var startTime = System.DateTime.Now;
		GameMaster.SetGenerator(generator);
		GameMaster.SetMap(map = generator.GenerateMap(transform));
		generator.GenerateFeatures(map);
		foreach (Tile t in map)
		{
			t.TileRender();
			t.PostRender();
		}
		map.Simulate(1);
		Debug.Log("Generation Time: " + (System.DateTime.Now - startTime).TotalMilliseconds + "ms");
    }

    // Update is called once per frame
    void Update()
    {
		if (!generator.Regen)
			return;
		generator.Regen = false;
		map.Destroy();
		Awake();
    }

	
}
