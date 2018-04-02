using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TradePackage
{
	public TradePackageType PackageType { get; set; }
	public string Resource { get; set; }
	public int ResourceUnits { get; set; }
	public float Money { get; set; }
}

public enum TradePackageType
{
	Resource,
	Money,
	Mixed,
	Food
}
