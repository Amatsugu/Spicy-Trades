using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNeed
{
	public string Resource { get; set; }
	public float Count { get; set; }
	public NeedType Type { get; set; }
	public SettlementEvent Source { get; set; }
}

public enum NeedType
{
	Resource, 
	Category,
	Money,
	Event
}
