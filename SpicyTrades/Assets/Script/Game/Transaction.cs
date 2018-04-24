using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

public class Transaction
{
	public TransactionType type;
	public string playerId;
	public string targetPlayerId;
	public HexCoords targetSettlement;
	public ResourceNeed resources;

	public void Execute()
	{
		SettlementTile settlement;
		ResourceTileInfo res;
		switch(type)
		{
			case TransactionType.Buy:
				settlement = GameMaster.GameMap[targetSettlement.ToIndex()] as SettlementTile;
				res = settlement.ResourceCache.Keys.First(r => resources.Match(r));
				settlement.Buy(res, resources.count, GameMaster.Players.First(p => p.Id == playerId));
				break;
			case TransactionType.Sell:
				settlement = GameMaster.GameMap[targetSettlement.ToIndex()] as SettlementTile;
				res = settlement.ResourceCache.Keys.First(r => resources.Match(r));
				break;
		}
	}
}

public enum TransactionType
{
	Buy,
	Sell,
	Move,
	Trade,
}
