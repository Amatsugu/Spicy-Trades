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
	public int Influence { get; set; }

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

	public Player(PlayerObject player, string id = null)
	{
		playerObject = player;
		player.SetPlayer(this);
		Id = id;
		inventory = new List<InventoryItem>();
#if DEBUG
		Money = new Coin(int.MaxValue);
#else
		Money = new Coin(10000f);
#endif
		GameMaster.GameMap.OnMapSimulate += m => GameMaster.CachePrices(CurrentTile);
	}

	public void MoveTo(SettlementTile tile, bool makeTransaction = true)
	{
		playerObject.MoveTo(tile);
		if(makeTransaction)
		{
			GameMaster.SendTransaction(new Transaction
			{
				type = TransactionType.Move,
				playerId = Id,
				targetSettlement = tile.Position
			});
		}
	}

	public void SetTile(SettlementTile tile, bool showUI = true)
	{
		_curTile = tile.Position;
		GameMaster.CachePrices(tile);
		if(showUI)
			UIManager.ShowSettlementPanel(tile);
		playerObject.transform.position = tile.WolrdPos;
	}

	public void AddItem(InventoryItem item)
	{
		var invItem = inventory.FirstOrDefault(i => i.Resource.resource == item.Resource.resource);
		if (invItem == null)
		{
			invItem = new InventoryItem
			{
				Resource = item.Resource,
				Cost = item.Cost
			};
			inventory.Add(invItem);
		}else
		{
			invItem.Cost = (invItem.Cost + invItem.Cost)/2f;
			invItem.Resource.count += item.Resource.count;
		}
	}

	public bool		TakeItem(InventoryItem item)
	{
		var invItem = inventory.First(i => i.Resource.resource == item.Resource.resource);
		if (invItem == null)
			return false;
		else
		{
			if(invItem.Resource.count < item.Resource.count)
				return false;
			else
			{
				if (invItem.Resource.count == item.Resource.count)
				{
					inventory.Remove(invItem);
					return true;
				}else
				{
					invItem.Resource.count -= item.Resource.count;
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

	public bool Sell(ResourceTileInfo resource, float count, SettlementTile settlement, bool makeTransaction = true)
	{
		float price;
		if (settlement.ResourceCache.ContainsKey(resource))
			price = settlement.ResourceCache[resource][1] * resource.basePrice;
		else
			price = 1.5f * resource.basePrice;
		if (settlement.Money >= price)
		{
			settlement.AddResource(resource, count);
			settlement.Money -= price;
			AddMoney(price);
			TakeItem(new InventoryItem
			{
				Cost = price,
				Resource = new ResourceIdentifier
				{
					resource = resource.name,
					count = count
				}
			});
			if(makeTransaction)
			{
				GameMaster.SendTransaction(new Transaction
				{
					type = TransactionType.Sell,
					playerId = Id,
					resources = new ResourceIdentifier
					{
						resource = resource.name,
						count = count
					}
				});
			}
			return true;
		}
		return false;
	}

#if DEBUG
	public void LogItems()
	{
		foreach (var p in inventory)
			Debug.Log(p.Resource.resource + " : " + p.Resource.count);
	}
#endif

}
