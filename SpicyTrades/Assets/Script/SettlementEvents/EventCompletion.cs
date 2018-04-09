using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventCompletion : ScriptableObject
{
	public abstract void Complete();

	public abstract void Fail();
}
