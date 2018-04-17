using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data Providers/Event Completion")]
public class EventCompletion : ScriptableObject
{
	public SettlementEffect sucess;
	public SettlementEffect fail;
}
