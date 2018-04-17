using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class SettlementEffect : ScriptableObject
{
	public abstract void Effect(SettlementTile target);
}
