using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NetworkManager;
using Newtonsoft.Json;

public class Player
{
	public string Id { get; private set; }
	public string Username { get; private set; }
	public Coin Money { get; private set; }
	[JsonIgnore]
	public PlayerObject playerObject;
	[JsonIgnore]
	public SettlementTile CurrentTile
	{
		get
		{
			return GameMaster.GameMap[_curTile.ToIndex()] as SettlementTile;
		}
	}
	[JsonProperty]
	private HexCoords _curTile;
	
	public List<InventoryItem> inventory;

	public Player(PlayerObject player)
	{
		playerObject = player;
		player.SetPlayer(this);
		inventory = new List<InventoryItem>();
		Money = new Coin(10000f);
		GameMaster.GameMap.OnMapSimulate += m => GameMaster.CachePrices(CurrentTile);
	}

	public void MoveTo(SettlementTile tile)
	{
		playerObject.MoveTo(tile);
		new Transaction
		{
			type = TransactionType.Move,
			playerId = Id,
			targetSettlement = tile.Position
		};//TODO: Sync Transactions
	}

	public void SetTile(SettlementTile tile)
	{
		_curTile = tile.Position;
		GameMaster.CachePrices(tile);
		UIManager.ShowSettlementPanel(tile);
		playerObject.transform.position = tile.WolrdPos;
	}

	public void AddItem(InventoryItem item)
	{
		var invItem = inventory.FirstOrDefault(i => i.Package.Resource == item.Package.Resource);
		if (invItem == null)
		{
			invItem = new InventoryItem
			{
				Package = item.Package,
				Cost = item.Cost
			};
			inventory.Add(invItem);
		}else
		{
			invItem.Cost = (invItem.Cost + invItem.Cost)/2f;
			var p = invItem.Package;
			p.ResourceUnits = p.ResourceUnits + item.Package.ResourceUnits;
			invItem.Package = p;
		}
	}

	public bool TakeItem(InventoryItem item)
	{
		var invItem = inventory.First(i => i.Package.Resource == item.Package.Resource);
		if (invItem == null)
			return false;
		else
		{
			if(invItem.Package.ResourceUnits < item.Package.ResourceUnits)
				return false;
			else
			{
				if (invItem.Package.ResourceUnits == item.Package.ResourceUnits)
				{
					inventory.Remove(invItem);
					return true;
				}else
				{
					var p = invItem.Package;
					p.ResourceUnits -= item.Package.ResourceUnits;
					invItem.Package = p;
					return true;
				}
			}
		}
	}

	public void AddMoney(float ammount)
	{
		ammount = Mathf.Abs(ammount);
		Money += ammount; 
	}

	public bool TakeMoney(float ammount)
	{
		ammount = Mathf.Abs(ammount);
		if (ammount > Money)
			return false;
		Money -= ammount;
		return true;
	}

#if DEBUG
	public void LogItems()
	{
		foreach (var p in inventory)
			Debug.Log(p.Package.Resource + " : " + p.Package.ResourceUnits);
	}
#endif

}
