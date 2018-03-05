using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FeatureGenerator : ScriptableObject
{

	public abstract void Generate(Map map);
}
