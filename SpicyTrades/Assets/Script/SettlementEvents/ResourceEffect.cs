using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Resource Effect")]
public class ResourceEffect : SettlementEffect
{
	public ResourceTileInfo resource;
	public int count;

	public override void Effect(SettlementTile target)
	{
		if (count > 0)
			target.AddResource(resource, count);
		else
			target.TakeResource(resource, count);
	}
}
