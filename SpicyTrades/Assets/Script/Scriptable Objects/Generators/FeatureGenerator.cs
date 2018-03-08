using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FeatureGenerator : ScriptableObject
{
	public string GeneratorName { get; set; }
	public abstract void Generate(Map map);
}
