using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettlementEvent : ScriptableObject
{
	public string Description { get; set; }
	[Range(0, 100)]
	public int Chance;

}
