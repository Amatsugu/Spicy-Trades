using Newtonsoft.Json;
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
	public List<ResourceIdentifier> ResourceNeeds { get; private set; }
	public List<Recipe> Recipes { get; private set; }
	public string Description { get; set; }
	[JsonIgnore]
	public new SettlementTileInfo tileInfo;
	public List<SettlementEvent> eventPool; 
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
		ResourceNeeds = new List<ResourceIdentifier>();
	}

	public SettlementTile RegisterResource(ResourceTileInfo resource)
	{
		if (Resources == null)
		{
			Resources = new List<ResourceTileInfo>();
			ResourceCache = new Dictionary<ResourceTileInfo, float[]>();
			ResourceNeeds = new List<ResourceIdentifier>();
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

	public bool HasResource(ResourceIdentifier resource)
	{
		var res = ResourceCache.Keys.FirstOrDefault(r => resource.Match(r));
		if (res == null)
			return false;
		return HasResource(res, resource.count);
	}

	public void RegisterFactory(FactoryTileInfo factory)
	{
		Factories.Add(factory);
	}

	public void RegisterRecipe(Recipe recipe)
	{
		Recipes.Add(recipe);
	}

	public bool HasResource(ResourceTileInfo resource, float count)
	{
		count = Mathf.Floor(count);
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
		ResourceNeeds.Add(new ResourceIdentifier
		{
			type = NeedType.Tag,
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
			foreach (var need in e.ResourceNeeds)
			{
				ResourceNeeds.Remove(need);
			}
			return true;
		});
		if (currentEvents.Count > tileInfo.maxEvents)
			return;
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
		var resNeed = new ResourceIdentifier[pickedEvent.resourceDemands.Count];
		activeEvent.ResourceNeeds = resNeed;
		for (int i = 0; i < resNeed.Length; i++)
		{
			var resDemmands = pickedEvent.resourceDemands[i];
			resNeed[i] = new ResourceIdentifier
			{
				resource = resDemmands.resource,
				count = resDemmands.count * Population,
				type = resDemmands.type,
				source = activeEvent
			};
		}
		ResourceNeeds.AddRange(resNeed);
	}

	public bool TakeResource(ResourceIdentifier need)
	{
		if (need.count == 0)
			return true;
		var res = ResourceCache.Keys.FirstOrDefault(r => need.Match(r));
		if (res == null)
			return false;
		return TakeResource(res, need.count);
	}

	public bool TakeResource(ResourceTileInfo resource, float units)
	{
		units = Mathf.Floor(units);
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
		foreach(var res in ResourceCache.Keys)
		{
			if (ResourceCache[res][0] == 0)
				continue;
			foreach (var need in ResourceNeeds)
			{
				if (need.count == 0)
					continue;
				if (!need.Match(res))
					continue;
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

	public bool Buy(ResourceTileInfo resource, float count, Player player = null, bool makeTransaction = true)
	{
		count = Mathf.Floor(count);
		if (player == null)
			player = GameMaster.Player;
		var cost = resource.basePrice * count * ResourceCache[resource][1];
		if (player.Money < cost)
			return false;
		if (TakeResource(resource, count))
		{
			player.AddItem(new InventoryItem
			{
				Resource = new ResourceIdentifier
				{
					resource = resource.name,
					count = count
				},
				Cost = cost
			});
			player.TakeMoney(cost);
			if (makeTransaction)
			{

				GameMaster.SendTransaction(new Transaction
				{
					type = TransactionType.Buy,
					playerId = player.Id,
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

}
