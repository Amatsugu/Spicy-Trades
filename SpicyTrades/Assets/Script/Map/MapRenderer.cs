using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapRenderer : MonoBehaviour
{
	public MapGenerator generator;
	public GameObject smartTile;
	public GameRegistry registry;
	public Map map;
	public float nextTick;

    // Use this for initialization
    void Awake()
    {
		var startTime = System.DateTime.Now;
		GameMaster.Generator = generator;
		GameMaster.GameMap = map = generator.GenerateMap(transform);
		GameMaster.Registry = registry;
		generator.GenerateFeatures(map);
		foreach (Tile t in map)
		{
			t.TileRender();
			t.PostRender();
		}
		map.Simulate(1);
		nextTick = Time.time + GameMaster.TickRate;
		Debug.Log("Generation Time: " + (DateTime.Now - startTime).TotalMilliseconds + "ms");
    }

    // Update is called once per frame
    void Update()
    {
		if(nextTick <= Time.time)
		{
			//Debug.Log("Simulate");
			var time = DateTime.Now;
			map.Simulate(1);
			nextTick = Time.time + GameMaster.TickRate;
		}
		if (!generator.Regen)
			return;
		generator.Regen = false;
		map.Destroy();
		Awake();
    }

	private void OnGUI()
	{
		GUI.skin.label.fontSize = 20;
		GUI.skin.label.normal.textColor = Color.black;
		GUI.skin.label.fontStyle = FontStyle.Bold;
		GUILayout.Label("Time to next Tick " + (nextTick - Time.time));
		GUILayout.Label(GameMaster.CurrentTick.ToString());
	}


}
