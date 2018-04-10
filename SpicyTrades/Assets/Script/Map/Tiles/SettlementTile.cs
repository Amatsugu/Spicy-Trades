using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettlementTile : Tile
{
	public SettlementType townType;
	public string Name;
	public int Population { get; set; }
	public SettlementTile Center { get; set; }
	public List<ResourceTileInfo> Resources { get; private set; }
	public Coin Money = new Coin(500000);
	public const int maxResourceStorage = 1000;
	public Dictionary<ResourceTileInfo, float[]> ResourceCache { get; private set; }
	public List<ResourceNeed> ResourceNeeds { get; private set; }
	public new SettlementTileInfo tileInfo;
	public List<SettlementEvent> eventPool; //TODO: make event list
	public List<ActiveEvent> currentEvents;

	public SettlementTile(SettlementTileInfo tileInfo, Transform parent, HexCoords hexCoords, float outerRadius, SettlementTile center = null) : base(tileInfo, parent, hexCoords, outerRadius)
	{
		Center = center ?? this;
		this.tileInfo = tileInfo;
		eventPool = new List<SettlementEvent>();
		currentEvents = new List<ActiveEvent>();
	}

	public SettlementTile AddResource(ResourceTileInfo resource, int units = -1)
	{
		if (Resources == null)
		{
			Resources = new List<ResourceTileInfo>();
			ResourceCache = new Dictionary<ResourceTileInfo, float[]>();
			ResourceNeeds = new List<ResourceNeed>();
		}
		Resources.Add(resource);
		if (!ResourceCache.ContainsKey(resource))
		{
			var f = 0;
			ResourceCache.Add(resource, new float[] { f, Mathf.Max(.5f, Mathf.Min(1.5f, (1.5f - (f / maxResourceStorage))))});
		}
		return this;
	}

	public void Simulate()
	{
		DetermineNeeds();
		//Work Resources
		foreach(var res in Resources)
		{
			ResourceCache[res][0] += res.yeild;
			if (ResourceCache[res][0] > maxResourceStorage)
				ResourceCache[res][0] = maxResourceStorage;
			ResourceCache[res][1] = Mathf.Max(.5f, Mathf.Min(1.5f, (1.5f - (ResourceCache[res][0] / maxResourceStorage)))); //Recalculate Value
		}
		//Satisfy Needs
		SatisfyNeeds();
	}

	private void DetermineNeeds()
	{
		//Make Food Need
		ResourceNeeds.Add(new ResourceNeed
		{
			Type = NeedType.Category,
			Resource = "Food",
			Count = Mathf.CeilToInt(Population * (tileInfo as SettlementTileInfo).foodPerPop)
		});
		//Event Resources
	}

	public void PickEvent()
	{
		var groupedEvents = eventPool.GroupBy(e => e.Chance, e => e);
		var pick = Random.Range(0, 1f);
		pick = 1f - (pick * pick);
		pick = (float)MathUtils.Map(pick, 0, 1, 0, 100);
		var pickedEvents = groupedEvents.Aggregate((e1, e2) => Mathf.Abs(e1.Key - pick) < Mathf.Abs(e2.Key - pick) ? e1 : e2).ToArray();
		currentEvents.Add(new ActiveEvent(pickedEvents[Random.Range(0, pickedEvents.Length - 1)]));
	}

	public bool TakeResource(ResourceTileInfo resource, int units)
	{
		float[] res;
		if (!ResourceCache.TryGetValue(resource, out res))
			return false;
		if (res[0] < units)
			return false;
		res[0] -= units;
		res[1] = Mathf.Max(.5f, Mathf.Min(1.5f, (1.5f - (res[0] / maxResourceStorage))));
		return true;
	}

	private void SatisfyNeeds()
	{
		foreach (var need in ResourceNeeds)
		{
			if(need.Type == NeedType.Category) //Categoric Needs
			{
				ResourceCategory cat = (ResourceCategory)System.Enum.Parse(typeof(ResourceCategory), need.Resource);
				foreach (var res in ResourceCache.Keys)
				{
					if (res.category != cat)
						continue;
					var curCache = ResourceCache[res];
					if (curCache[0] >= need.Count)
					{
						curCache[0] -= need.Count;
						need.Count = 0;
					}
					else
					{
						var unitsTaken = Mathf.FloorToInt(curCache[0]);
						need.Count -= unitsTaken;
						curCache[0] -= unitsTaken;
					}
					if (need.Count == 0)
						break;
				}
			}
			else if (need.Type == NeedType.Money) //Money
			{
				if (Money >= need.Count)
				{
					Money -= need.Count;
					need.Count = 0;
				}
				else
				{
					need.Count = (need.Count - Money).Value;
					Money = new Coin(0);
				}
			}
			else //Resources
			{
				var curCache = ResourceCache[ResourceCache.Keys.Single(res => res.name == need.Resource)];
				if (curCache[0] >= need.Count)
				{
					curCache[0] = 0;
					need.Count = 0;
				}
				else
				{
					var unitsTaken = Mathf.FloorToInt(curCache[0]);
					need.Count -= unitsTaken;
					curCache[0] -= unitsTaken;
				}
			}

		}
		ResourceNeeds.RemoveAll(n => n.Count == 0);
		RecalculateAssetValue();
	}

	private void RecalculateAssetValue()
	{
		foreach(var res in ResourceCache.Keys)
		{
			ResourceCache[res][1] = Mathf.Max(.5f, Mathf.Min(1.5f, (1.5f - (ResourceCache[res][0] / maxResourceStorage)))); //Recalculate Value
		}
	}

	//TODO: Trades
	public void NegotiateTrade()
	{
	}

	public bool Buy(ResourceTileInfo resource, int units, Player player = null)
	{
		if (player == null)
			player = GameMaster.Player;
		var cost = units * ResourceCache[resource][1];
		if (player.Money < cost)
			return false;
		if (TakeResource(resource, units))
		{
			player.AddItem(new InventoryItem
			{
				Package = new TradePackage
				{
					Resource = resource.name,
					ResourceUnits = units
				},
				Cost = cost
			});
			player.TakeMoney(cost);
			return true;
		}
		return false;
	}

}
