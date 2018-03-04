using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TownType : ScriptableObject
{
	public int MinPopulation;
	public int MaxPopulation;

	public abstract void Initialize(TownTile tile);
}
