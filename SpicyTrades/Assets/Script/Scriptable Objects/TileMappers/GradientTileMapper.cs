using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile Mapper/Gradient")]
public class GradientTileMapper : TileMapper
{
	public Gradient tileGradient = new Gradient();
	public AnimationCurve moveCostCurve = new AnimationCurve();
	public TileInfo[] Tiles;
	public Dictionary<Color, TileInfo> tiles = new Dictionary<Color, TileInfo>();


	public void OnEnable()
	{
		tiles.Clear();
		for (int i = 0; i < Tiles.Length; i++)
		{
			if (i > Tiles.Length - 1)
				tiles.Add(tileGradient.colorKeys[i].color, null);
			else
				tiles.Add(tileGradient.colorKeys[i].color, Tiles[i]);
		}
	}

	public override TileInfo GetTile(float sample)
	{
		return tiles[tileGradient.Evaluate(sample)];
	}

	public override float GetMoveCost(float sample)
	{
		return moveCostCurve.Evaluate(sample);
	}
}
