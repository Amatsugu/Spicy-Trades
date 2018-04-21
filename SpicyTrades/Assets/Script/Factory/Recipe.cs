using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipe : ScriptableObject
{
	public ResourceTileInfo inputA;
	public int inputACount;
	public ResourceTileInfo inputB;
	public int inputBCount;

	public ResourceTileInfo output;
	public int outputCount;

	public string factoryType;

	public void OnEnable()
	{
		if (inputA == null)
			inputACount = 0;
		if (inputB == null)
			inputBCount = 0;
	}
}
