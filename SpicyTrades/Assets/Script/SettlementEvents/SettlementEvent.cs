using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data Providers/Event")]
public class SettlementEvent : ScriptableObject
{
	public string Description { get; set; }
	[Range(0, 100)]
	public int Chance;

	public List<TradePackage> resourceDemands = new List<TradePackage>();
	public int duration;

}
