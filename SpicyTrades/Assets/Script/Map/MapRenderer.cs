using NetworkManager;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MapRenderer : MonoBehaviour
{
	public MapGenerator generator;
	public GameObject smartTile;
	public GameRegistry registry;
	public Map map;
	public float nextTick;
	public int ticks;
	public float nextSecond;
	public float tickRate;
	public float frames;
	public float frameRate;
	public float averageTick;


    // Use this for initialization
    void Awake()
    {
		var startTime = System.DateTime.Now;
		GameMaster.Generator = generator;
		GameMaster.GameMap = map = generator.GenerateMap(transform);
		GameMaster.Registry = registry;
		generator.GenerateFeatures(map);
		GameMaster.Ready();
		foreach (Tile t in map)
		{
			t.TileRender();
			t.PostRender();
		}
		map.Simulate(1);
		nextTick = Time.time + GameMaster.TickRate;
		nextSecond = Time.time + 1;
		Debug.Log("Generation Time: " + (DateTime.Now - startTime).TotalMilliseconds + "ms");
    }

    // Update is called once per frame
    void Update()
    {
		if(Time.time > nextSecond)
		{
			tickRate = ticks / (Time.time - (nextSecond - 1));
			nextSecond = Time.time + 1;
			ticks = 0;
		}
		frameRate = 1 / Time.deltaTime;
		if(nextTick <= Time.time)
		{
			var time = DateTime.Now;
			map.Simulate(1);
			if (!GameMaster.Offline)
			{
				Net().GetAwaiter().GetResult();
				//SpicyNetwork.DoMainClientStuff();
			}
			nextTick = Time.time + GameMaster.TickRate;
			ticks++;
		}
		if (!generator.Regen)
			return;
		generator.Regen = false;
		map.Destroy();
		Awake();
    }

	public async Task Net()
	{
		await Task.Run(() => SpicyNetwork.DoMainClientStuff());
	}

	private void OnApplicationQuit()
	{
		if (GameMaster.Offline)
			return;
		Debug.Log("Exit");
		SpicyNetwork.LeaveRoom();
		SpicyNetwork.Logout();
	}

	private void OnGUI()
	{
		GUI.skin.label.fontSize = 20;
		GUI.skin.label.normal.textColor = Color.black;
		GUI.skin.label.fontStyle = FontStyle.Bold;
		GUILayout.Label($"{tickRate} tps");
		GUILayout.Label($"{frameRate} fps");
	}


}
