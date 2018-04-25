using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ActiveEvent
{
	public float StartTime { get; private set; }
	public string Name
	{
		get
		{
			return Event.name;
		}
	}
	public SettlementEvent Event { get; private set; }
	public float EndTime { get; private set; }
	public SettlementTile EventHost { get; set; }
	public ResourceIdentifier[] ResourceNeeds { get; set; }

	public ActiveEvent(SettlementEvent @event, SettlementTile hostSettlement)
	{
		Event = @event;
		StartTime = Time.time;
		EndTime = StartTime + Event.duration;
		EventHost = hostSettlement;
	}
}
