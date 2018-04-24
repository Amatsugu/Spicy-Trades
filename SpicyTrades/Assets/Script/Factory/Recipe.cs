using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data Providers/Recipe")]
public class Recipe : ScriptableObject
{
	public ResourceNeed inputA;
	public ResourceNeed inputB;

	public ResourceTileInfo output;
	public int outputCount;

	public string factoryType;
}
