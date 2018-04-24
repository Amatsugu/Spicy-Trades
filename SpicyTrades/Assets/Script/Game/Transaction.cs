using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transaction
{
	public TransactionType type;
	public object playerId;
	public HexCoords targetSettlement;
	public string resource;
	public int count;
}

public enum TransactionType
{
	Buy,
	Sell,

}
