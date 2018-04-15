using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data Providers/Event Pool")]
public class EventPool : ScriptableObject
{
	public List<SettlementEvent> events;
}
