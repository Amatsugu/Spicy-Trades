using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile Mapper/Gradient")]
public class GradientTileMapper : TileMapper
{
	public Gradient tileGradient = new Gradient();
	public AnimationCurve moveCostCurve = new AnimationCurve();
	public Transform[] Tiles;
	public Dictionary<Color, Transform> tiles = new Dictionary<Color, Transform>();

	public bool colorize;

	public void OnEnable()
	{
		tiles.Clear();
		for (int i = 0; i < Tiles.Length; i++)
		{
			//_mapper.Tiles[i] = t[i];
			if (i > Tiles.Length - 1)
				tiles.Add(tileGradient.colorKeys[i].color, null);
			else
				tiles.Add(tileGradient.colorKeys[i].color, Tiles[i]);
		}
	}

	public override Transform GetTile(float sample)
	{
		return tiles[tileGradient.Evaluate(sample)];
		//return tiles[tileGradient.colorKeys.Aggregate((a,b) => Mathf.Abs(a.time - sample) < Mathf.Abs(b.time - sample) ? a : b).color];
	}

	public override Color GetColor(float sample)
	{
		return colorize ? tileGradient.Evaluate(sample) : Color.white;
	}

	public override float GetMoveCost(float sample)
	{
		return moveCostCurve.Evaluate(sample);
	}
}
