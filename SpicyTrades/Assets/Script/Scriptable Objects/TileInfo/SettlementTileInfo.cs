using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile/Settlement")]
public class SettlementTileInfo : TileInfo
{
	public SettlementType settlementType;
	public float foodPerPop = 0.5f;

	private void OnEnable()
	{
		TileType = TileType.Settlement;
	}
}

public enum SettlementType
{
	Village = 0, 
	Town = 1,
	Capital = 2
}
