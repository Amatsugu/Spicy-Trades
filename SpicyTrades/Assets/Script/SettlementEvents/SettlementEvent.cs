using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data Providers/Event")]
[Serializable]
public class SettlementEvent
{
	public string name;
	public string description;
	[Range(0, 100)]
	public int Chance;

	public List<ResourceNeed> resourceDemands = new List<ResourceNeed>();
	public int duration;
	public int cooldown;
	public SettlementType location;

	public EventCompletion completionEffect;

}
