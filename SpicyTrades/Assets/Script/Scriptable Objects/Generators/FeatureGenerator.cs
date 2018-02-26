using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FeatureGenerator : ScriptableObject
{
	public GameObject TownTile;
	public GameObject RoadTile;

	public abstract void Generate(Map map);
}
