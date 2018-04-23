using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipe : ScriptableObject
{
	public ResourceNeed inputA;
	public ResourceNeed inputB;

	public ResourceTileInfo output;
	public int outputCount;

	public string factoryType;
}
