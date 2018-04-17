using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResourceNeed
{
	public string resource;
	public float count;
	public NeedType type;
	[HideInInspector]
	public ActiveEvent source;
}

public enum NeedType
{
	Resource,
	Category,
	Tag,
	Money
}
