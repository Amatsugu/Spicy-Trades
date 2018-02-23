using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileMapper : ScriptableObject
{
	public abstract Transform GetTile(float sample);

	public abstract Color GetColor(float sample);

	public abstract float GetMoveCost(float sample);
}
