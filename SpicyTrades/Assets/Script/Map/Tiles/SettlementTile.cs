using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettlementTile : Tile
{
	public SettlementType SettlementType
	{
		get
		{
			return tileInfo.settlementType;
		}
	}
	public string Name;
	public int Population { get; set; }
	public SettlementTile Center { get; set; }
	public List<ResourceTileInfo> Resources { get; private set; }
	public List<FactoryTileInfo> Factories { get; private set; }
	public Coin Money = new Coin(500000);
	public const int maxResourceStorage = 1000;

	public Dictionary<ResourceTileInfo, float[]> ResourceCache { get; private set; }
	public List<ResourceNeed> ResourceNeeds { get; private set; }
	public List<Recipe> Recipes { get; private set; }
	public new SettlementTileInfo tileInfo;
	public List<SettlementEvent> eventPool; //TODO: make event list
	public List<ActiveEvent> currentEvents;
	private int _nextEventTick;
	private SettlementTile _capital;

	public SettlementTile(SettlementTileInfo tileInfo, Transform parent, HexCoords hexCoords, float outerRadius, SettlementTile center = null) : base(tileInfo, parent, hexCoords, outerRadius)
	{
		Center = center ?? this;
		this.tileInfo = tileInfo;
		currentEvents = new List<ActiveEvent>();
		_nextEventTick = GameMaster.CurrentTick;
		Resources = new List<ResourceTileInfo>();
		Factories = new List<FactoryTileInfo>();
		Recipes = new List<Recipe>();
		ResourceCache = new Dictionary<ResourceTileInfo, float[]>();
		ResourceNeeds = new List<ResourceNeed>();
	}

	public SettlementTile RegisterResource(ResourceTileInfo resource)
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
			ResourceCache.Add(resource, new float[] { f, GetResourceValue(f) });
		}
		return this;
	}

	public void AddResource(ResourceTileInfo resource, float count)
	{
		if (_capital == null)
			_capital = GameMaster.GameMap.Capital;
		count = Mathf.Floor(count);
		if (!ResourceCache.ContainsKey(resource))
		{
			if(SettlementType != SettlementType.Capital && count > maxResourceStorage)
			{
				var extra = count - maxResourceStorage;
				_capital.AddResource(resource, extra);
				count = maxResourceStorage;
				Money += resource.basePrice;
			}
			ResourceCache.Add(resource, new float[] { count, GetResourceValue(count) });
		}
		else
		{
			var cache = ResourceCache[resource];
			if (SettlementType != SettlementType.Capital && cache[0] + count > maxResourceStorage)
			{
				var extra = (cache[0] + count) - maxResourceStorage;
				_capital.AddResource(resource, extra);
				count -= extra;
				Money += resource.basePrice;
			}
			cache[0] += count;
			cache[1] = GetResourceValue(count);
		}
	}

	public bool HasResource(ResourceNeed resource)
	{
		if (resource.type == NeedType.Category) //Categoric Needs
		{
			ResourceCategory cat = (ResourceCategory)System.Enum.Parse(typeof(ResourceCategory), resource.resource);
			foreach (var res in ResourceCache.Keys)
			{
				if (res.category != cat)
					continue;
				if (HasResource(res, (int)resource.count))
					return true;
			}
			return false;
		}
		else if (resource.type == NeedType.Tag) //Tagged
		{
			var resCache = ResourceCache.Keys.Where(r => r.tags.Contains(resource.resource));
			if (resCache.Count() == 0)
				return false;
			foreach (var res in resCache)
			{
				if (HasResource(res, (int)resource.count))
					return true;
			}
			return false;
		}
		else if (resource.type == NeedType.Money) //Money
		{
			return (Money >= resource.count);
		}
		else //Resources
		{
			var res = ResourceCache.Keys.SingleOrDefault(key => key.name == resource.resource);
			if (res == null)
				return false;
			var curCache = ResourceCache[res];
			if (HasResource(res, (int)resource.count))
				return true;
			return false;
		}
	}

	public void RegisterFactory(FactoryTileInfo factory)
	{
		Factories.Add(factory);
	}

	public void RegisterRecipe(Recipe recipe)
	{
		Recipes.Add(recipe);
	}

	public bool HasResource(ResourceTileInfo resource, int count)
	{
		if (ResourceCache.ContainsKey(resource))
		{
			return ResourceCache[resource][0] >= count;
		}
		else
			return false;
	}

	private float GetResourceValue(float supply)
	{
		if (SettlementType != SettlementType.Capital)
			return (float)MathUtils.Map(supply, 0, maxResourceStorage, 1.5f, .5f);
		else
			return 1f;
	}

	public void Simulate()
	{
		//Determine Resource Needs
		DetermineNeeds();
		//Work Resources
		foreach(var res in Resources)
		{
			AddResource(res, res.yeild);
		}
		//Satisfy Needs
		SatisfyNeeds();
		//Craft
		foreach (var recipe in Recipes)
		{
			Factories.First(f => f.factoryType == recipe.factoryType).Craft(recipe, this);
		}
		//Select Events
		if (_nextEventTick <= GameMaster.CurrentTick)
			PickEvent();
	}

	private void DetermineNeeds()
	{
		//Make Food Needs
		ResourceNeeds.Add(new ResourceNeed
		{
			type = NeedType.Category,
			resource = "Food",
			count = Mathf.CeilToInt(Population * (tileInfo as SettlementTileInfo).foodPerPop)
		});
	}

	private void PickEvent()
	{
		//Remove Completed Events
		currentEvents.RemoveAll(e =>
		{
			if (e.EndTime > GameMaster.CurrentTick)
				return false;
			if(e.Event.completionEffect != null)
			{
				var ce = e.Event.completionEffect;
				if (e.ResourceNeeds.Any(neeed => neeed.count > 0))
				{
					if (ce.fail != null)
						ce.fail.Effect(this);
				}
				else
				{
					if (ce.sucess != null)
						ce.sucess.Effect(this);
				}
			}
			return true;
		});
		//Select New Events
		if (eventPool == null)
			eventPool = GameMaster.Registry.eventPool.events;
		//Debug.Log("Picking Event");
		var groupedEvents = eventPool.Where(e => e.location <= SettlementType).GroupBy(e => e.Chance, e => e);
		var pick = Random.Range(0, 1f);
		pick = 1f - (pick * pick);
		pick = (float)MathUtils.Map(pick, 0, 1, 0, 100);
		var pickedEvents = groupedEvents.Aggregate((e1, e2) => Mathf.Abs(e1.Key - pick) < Mathf.Abs(e2.Key - pick) ? e1 : e2).ToArray();
		if (Mathf.Abs(pickedEvents.First().Chance - pick) > 5)
			return;
		var pickedEvent = pickedEvents[Random.Range(0, pickedEvents.Length - 1)];
		if (currentEvents.Any(e => e.Event == pickedEvent))
			return;
		_nextEventTick = GameMaster.CurrentTick + pickedEvent.cooldown;
		var activeEvent = new ActiveEvent(pickedEvent, this);
		currentEvents.Add(activeEvent);
		var resNeed = new ResourceNeed[pickedEvent.resourceDemands.Count];
		activeEvent.ResourceNeeds = resNeed;
		for (int i = 0; i < resNeed.Length; i++)
		{
			var resDemmands = pickedEvent.resourceDemands[i];
			resNeed[i] = new ResourceNeed
			{
				resource = resDemmands.resource,
				count = resDemmands.count * Population,
				type = resDemmands.type,
				source = activeEvent
			};
		}
		ResourceNeeds.AddRange(resNeed);
	}

	public bool TakeResource(ResourceNeed need)
	{
		if (need.count == 0)
			return true;
		if (need.type == NeedType.Category) //Categoric Needs
		{
			ResourceCategory cat = (ResourceCategory)System.Enum.Parse(typeof(ResourceCategory), need.resource);
			foreach (var res in ResourceCache.Keys)
			{
				if (res.category != cat)
					continue;
				if (TakeResource(res, (int)need.count))
					return true;
			}
			return false;
		}
		else if (need.type == NeedType.Tag) //Tagged
		{
			var resCache = ResourceCache.Keys.Where(r => r.tags.Contains(need.resource));
			if (resCache.Count() == 0)
				return false;
			foreach (var res in resCache)
			{
				if (TakeResource(res, (int)need.count))
					return true;
			}
			return false;
		}
		else if (need.type == NeedType.Money) //Money
		{
			if (Money >= need.count)
			{
				Money -= need.count;
				return true;
			}
			return false;
		}
		else //Resources
		{
			var res = ResourceCache.Keys.SingleOrDefault(key => key.name == need.resource);
			if (res == null)
				return false;
			var curCache = ResourceCache[res];
			if (TakeResource(res, (int)need.count))
				return true;
			return false;
		}
	}

	public bool TakeResource(ResourceTileInfo resource, int units)
	{
		if (units == 0)
			return true;
		float[] res;
		if (!ResourceCache.TryGetValue(resource, out res))
			return false;
		if (res[0] < units)
			return false;
		res[0] -= units;
		res[1] = GetResourceValue(res[0]); 
		return true;
	}

	private void SatisfyNeeds()
	{
		foreach (var need in ResourceNeeds)
		{
			if (need.count == 0)
				continue;
			if(need.type == NeedType.Category) //Categoric Needs
			{
				ResourceCategory cat = (ResourceCategory)System.Enum.Parse(typeof(ResourceCategory), need.resource);
				foreach (var res in ResourceCache.Keys)
				{
					if (res.category != cat)
						continue;
					if (TakeResource(res, (int)need.count))
						need.count = 0;
					else
					{
						var unitsTaken = Mathf.FloorToInt(ResourceCache[res][0]);
						TakeResource(res, unitsTaken);
						need.count -= unitsTaken;
					}
					if (need.count == 0)
						break;
				}
			}
			else if(need.type == NeedType.Tag) //Tagged
			{
				var resCache = ResourceCache.Keys.Where(r => r.tags.Contains(need.resource));
				if (resCache.Count() == 0)
					continue;
				foreach(var res in resCache)
				{
					if (TakeResource(res, (int)need.count))
						need.count = 0;
					else
					{
						var unitsTaken = Mathf.FloorToInt(ResourceCache[res][0]);
						TakeResource(res, unitsTaken);
						need.count -= unitsTaken;
					}
					if (need.count == 0)
						break;
				}
			}
			else if (need.type == NeedType.Money) //Money
			{
				if (Money >= need.count)
				{
					Money -= need.count;
					need.count = 0;
				}
				else
				{
					need.count = (need.count - Money).Value;
					Money = new Coin(0);
				}
			}
			else //Resources
			{
				var res = ResourceCache.Keys.SingleOrDefault(key => key.name == need.resource);
				if (res == null)
					continue;
				var curCache = ResourceCache[res];
				if (TakeResource(res, (int)need.count))
					need.count = 0;
				else
				{
					var unitsTaken = Mathf.FloorToInt(ResourceCache[res][0]);
					TakeResource(res, unitsTaken);
					need.count -= unitsTaken;
				}
			}

		}
		ResourceNeeds.RemoveAll(n => n.count == 0);
		RecalculateAssetValue();
	}

	private void RecalculateAssetValue()
	{
		foreach(var res in ResourceCache.Values)
		{
			res[1] = GetResourceValue(res[0]); //Recalculate Value
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
		var cost = resource.basePrice * units * ResourceCache[resource][1];
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
