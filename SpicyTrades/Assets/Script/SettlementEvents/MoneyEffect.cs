using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Money Effect")]
public class MoneyEffect : SettlementEffect
{
	public float count;

	public override void Effect(SettlementTile target)
	{
		target.Money += count;
	}
}
