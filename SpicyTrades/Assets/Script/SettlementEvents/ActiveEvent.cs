using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ActiveEvent
{
	public float StartTime { get; private set; }
	public SettlementEvent Event { get; private set; }
	public float EndTime { get; private set; }

	public ActiveEvent(SettlementEvent @event)
	{
		Event = @event;
		StartTime = Time.time;
		EndTime = StartTime + Event.duration;
	}
}
