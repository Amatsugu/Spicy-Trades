using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettlementTile : Tile
{
	public SettlementType townType;
	public string Name;
	public int Population { get; set; }
	public List<ResourceTileInfo> Resources { get; private set; }
	public float Money = 500000;
	public const int maxResourceStorage = 1000;
	public Dictionary<ResourceTileInfo, float[]> ResourceCache { get; private set; }
	public List<TradePackage> ResourceNeeds { get; private set; }

	public SettlementTile(SettlementTileInfo tileInfo, Transform parent, HexCoords hexCoords, float outerRadius) : base(tileInfo, parent, hexCoords, outerRadius)
	{
	}


	public SettlementTile AddResource(ResourceTileInfo resource)
	{
		if (Resources == null)
		{
			Resources = new List<ResourceTileInfo>();
			ResourceCache = new Dictionary<ResourceTileInfo, float[]>();
			ResourceNeeds = new List<TradePackage>();
		}
		Resources.Add(resource);
		if (!ResourceCache.ContainsKey(resource))
			ResourceCache.Add(resource, new float[] { resource.yeild, 1f });
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

}
