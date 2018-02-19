using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Smart Tile/Basic")]
public class SmartTileType : ScriptableObject
{
	public Sprite[] edgeSprites = new Sprite[6];
	public Sprite[] cornerSprites = new Sprite[12];
	public bool dualSprite = false;
	public bool invert = false;
	public bool inheritColor = false;
	public Sprite GetSprite(int side)
	{
		side = side % 6;
		if (dualSprite)
		{
			if (side == 0 || side == 3)
				return edgeSprites[0];
			else
				return edgeSprites[1];
		}
		return edgeSprites[side];
	}

	public Sprite GetCorner(int side, bool dual = false)
	{
		side = side % 6;
		if(dualSprite)
		{
			if (side == 0 || side == 3)
				return cornerSprites[dual ? 1 : 0];
			else
				return cornerSprites[dual ? 3 : 2];
		}
		return cornerSprites[side];
	}
}
