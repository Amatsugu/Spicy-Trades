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
	public float Money = 500000;
	public const int maxResourceStorage = 1000;
	public Dictionary<ResourceTileInfo, float[]> ResourceCache { get; private set; }
	public List<TradePackage> ResourceNeeds { get; private set; }
	public new SettlementTileInfo tileInfo;
	public List<SettlementEvent> eventPool; //TODO: make event list
	public List<SettlementEvent> currentEvents;

	public SettlementTile(SettlementTileInfo tileInfo, Transform parent, HexCoords hexCoords, float outerRadius, SettlementTile center = null) : base(tileInfo, parent, hexCoords, outerRadius)
	{
		Center = center ?? this;
		this.tileInfo = tileInfo;
		eventPool = new List<SettlementEvent>();
		currentEvents = new List<SettlementEvent>();
	}

	public SettlementTile AddResource(ResourceTileInfo resource, int units = -1)
	{
		if (Resources == null)
		{
			Resources = new List<ResourceTileInfo>();
			ResourceCache = new Dictionary<ResourceTileInfo, float[]>();
			ResourceNeeds = new List<TradePackage>();
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
		//Make Food Need
		ResourceNeeds.Add(new TradePackage
		{
			PackageType = TradePackageType.Food,
			ResourceUnits = Mathf.CeilToInt(Population * (tileInfo as SettlementTileInfo).foodPerPop)
		});
		//Work Resources
		foreach(var res in Resources)
		{
			ResourceCache[res][0] += res.yeild;
			/*if (_resourceCache[res.name][0] > maxResourceStorage)
				_resourceCache[res.name][0] = maxResourceStorage;*/
			ResourceCache[res][1] = Mathf.Max(.5f, Mathf.Min(1.5f, (1.5f - (ResourceCache[res][0] / maxResourceStorage))));
		}
		//Satisfy Needs
		SatisfyNeeds();
	}

	public void PickEvents()
	{
		var groupedEvents = eventPool.GroupBy(e => e.Chance, e => e);
		var pick = Random.Range(0, 1f);
		pick = 1f - (pick * pick);
		pick = (float)MathUtils.Map(pick, 0, 1, 0, 100);
		var pickedEvents = groupedEvents.Aggregate((e1, e2) => Mathf.Abs(e1.Key - pick) < Mathf.Abs(e2.Key - pick) ? e1 : e2).ToArray();
		currentEvents.Add(pickedEvents[Random.Range(0, pickedEvents.Length - 1)]);
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
		List<TradePackage> extraNeeds = new List<TradePackage>();
		foreach (var need in ResourceNeeds)
		{
			int unitsNeeded;
			if (need.PackageType == TradePackageType.Food) //Foods
			{
				unitsNeeded = need.ResourceUnits;
				foreach (var res in ResourceCache.Keys)
				{
					if (res.category != ResourceCategory.Food)
						continue;
					var curCache = ResourceCache[res];
					if (curCache[0] >= unitsNeeded)
					{
						curCache[0] -= unitsNeeded;
						unitsNeeded = 0;
					}
					else
					{
						var unitsTaken = Mathf.FloorToInt(curCache[0]);
						unitsNeeded -= unitsTaken;
						curCache[0] -= unitsTaken;
					}
					if (unitsNeeded == 0)
						break;
					extraNeeds.Add(new TradePackage
					{
						PackageType = TradePackageType.Food,
						ResourceUnits = unitsNeeded
					});
				}
			}
			else if (need.PackageType == TradePackageType.Money) //Money
			{
				if (Money > need.Money)
					Money -= need.Money;
				else
				{
					extraNeeds.Add(new TradePackage
					{
						PackageType = TradePackageType.Money,
						Money = need.Money - Money
					});
					Money = 0;
				}
			}
			else if (need.PackageType == TradePackageType.Mixed) //Mixed
			{
				var moneyNeeded = need.Money;
				if (Money > moneyNeeded)
				{
					Money -= moneyNeeded;
					moneyNeeded = 0;
				}
				else
				{
					extraNeeds.Add(new TradePackage
					{
						PackageType = TradePackageType.Money,
						Money = moneyNeeded - Money
					});
					Money = 0;
				}
				unitsNeeded = need.ResourceUnits;
				var curCache = (from ResourceTileInfo res in ResourceCache.Keys where res.name == need.Resource select ResourceCache[res]).First();
				if (curCache[0] >= unitsNeeded)
				{
					curCache[0] = 0;
					unitsNeeded = 0;
				}
				else
				{
					var unitsTaken = Mathf.FloorToInt(curCache[0]);
					unitsNeeded -= unitsTaken;
					curCache[0] -= unitsTaken;
				}
				if (unitsNeeded > 0)
				{
					extraNeeds.Add(new TradePackage
					{
						PackageType = need.PackageType,
						Resource = need.Resource,
						ResourceUnits = unitsNeeded
					});
				}
			}
			else //Resources
			{
				unitsNeeded = need.ResourceUnits;
				var curCache = (from ResourceTileInfo res in ResourceCache.Keys where res.name == need.Resource select ResourceCache[res]).First();
				if (curCache[0] >= unitsNeeded)
				{
					curCache[0] = 0;
					unitsNeeded = 0;
				}
				else
				{
					var unitsTaken = Mathf.FloorToInt(curCache[0]);
					unitsNeeded -= unitsTaken;
					curCache[0] -= unitsTaken;
				}
				if (unitsNeeded > 0)
				{
					extraNeeds.Add(new TradePackage
					{
						PackageType = need.PackageType,
						Resource = need.Resource,
						ResourceUnits = unitsNeeded
					});
				}
			}

		}
		ResourceNeeds.Clear();
		ResourceNeeds.AddRange(extraNeeds);
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
