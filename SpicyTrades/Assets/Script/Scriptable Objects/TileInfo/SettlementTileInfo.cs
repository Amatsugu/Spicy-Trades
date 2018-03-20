using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile/Settlement")]
public class SettlementTileInfo : TileInfo
{
	public new readonly TileType tileType = TileType.Town;
	public SettlementType settlementType;
	public float foodPerPop = 0.5f;
}

public enum SettlementType
{
	Town,
	Village,
	Capital
}
