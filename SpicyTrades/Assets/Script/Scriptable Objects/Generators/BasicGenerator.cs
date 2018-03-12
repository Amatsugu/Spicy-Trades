using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map Generator/Basic")]
public class BasicGenerator : MapGenerator
{
	public override Tile Generate(int x, int y, Transform parent = null)
	{
		return CreateTile(x, y, parent).SetColor(Color.HSVToRGB((x + Size.x) / (2 * Size.x), Mathf.Lerp(.4f, 1, (y + Size.y) / (2 * Size.y)), 1));
	}
}
