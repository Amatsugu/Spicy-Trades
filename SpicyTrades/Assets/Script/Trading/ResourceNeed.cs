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
	public SettlementEvent source;
}

public enum NeedType
{
	Resource, 
	Category,
	Money,
	Event
}
