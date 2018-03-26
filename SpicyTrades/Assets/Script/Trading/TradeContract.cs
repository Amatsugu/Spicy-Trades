using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TradeContract
{
	public HexCoords Target { get; private set; }
	public HexCoords Source { get; private set; }

	public TradePackage SourcePackage { get; set; }
	public TradePackage TargetPackage { get; set; }

	public TradeContract(HexCoords source, HexCoords target, TradePackage packageSrource, TradePackage packageTarget)
	{
		Target = target;
		Source = source;
		SourcePackage = packageSrource;
		TargetPackage = packageTarget;
	}
}
