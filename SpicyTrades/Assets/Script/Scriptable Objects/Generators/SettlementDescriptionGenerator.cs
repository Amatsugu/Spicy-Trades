using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Feature Generator/Description Generator")]
public class SettlementDescriptionGenerator : FeatureGenerator {
	public override void Generate(Map map)
	{
		var settlements = map.Settlements;
		foreach(SettlementTile settlement in settlements)
		{
			//Wood
			if (settlement.HasResource(new ResourceIdentifier
			{
				type = NeedType.Tag,
				resource = "Wood",
				count = 0
			}))
			{
				settlement.Description = "A large, dense forest spans your view. Some trees are as tall as the clouds themselves, dark and looming. It seems the local population gathers their wood from here. ";
			}
			//Water
			if(settlement.GetNeighbors().Any(n => n != null && (n.Tag == "Water" || n.GetNeighbors().Any(nn => nn != null && nn.Tag == "Water"))))
			{
				settlement.Description += "Large wells can be seen throughout the area; collecting water from deep below the surface and from other nearby sources. ";
			}
			//Crops
			if(settlement.HasResource(new ResourceIdentifier
			{
				type = NeedType.Tag,
				resource = "Crop",
				count = 0
			}))
			{
				settlement.Description += "You see large farm areas are set aside for the growing of crops. There are some farmers hard at work gathering the season's bountiful harvest. ";
			}
			//Ore
			if(settlement.HasResource(new ResourceIdentifier
			{
				type = NeedType.Tag,
				resource = "Ore",
				count = 0
			}))
			{
				settlement.Description += "You notice some miners are returning from a long day. Perhaps they gather the ore from nearby quarries. The hammering from nearby buildings suggest a few local metalworkers. ";
			}
			//Textile
			if (settlement.HasResource(new ResourceIdentifier
			{
				type = NeedType.Tag,
				resource = "Textile",
				count = 0
			}))
			{
				settlement.Description += "The clothing worn by the local populace gives you a good idea about the types of textiles produced here. ";
			}
			//Gem
			if (settlement.HasResource(new ResourceIdentifier
			{
				type = NeedType.Tag,
				resource = "Gem",
				count = 0
			}))
			{
				settlement.Description += "The glint of jewelery catches your eye as you pass. The people here seem to be rather wealthy. ";
			}
		}
	}
}
