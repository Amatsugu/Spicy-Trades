using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Transaction
{
	public TransactionType type;
	public string playerId;
	public string targetPlayerId;
	public HexCoords targetSettlement;
	public string resource;
	public float count;
}

public enum TransactionType
{
	Buy,
	Sell,
	Move,
	Trade,
}
