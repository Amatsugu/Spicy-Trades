using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeKnowledge
{
	public int AquisitionTick { get; set; }
	public Dictionary<ResourceTileInfo, float> Cache { get; set; }
	public int Age
	{
		get
		{
			return GameMaster.CurrentTick - AquisitionTick;
		}
	}

	public TradeKnowledge()
	{
		Cache = new Dictionary<ResourceTileInfo, float>();
	}

}
