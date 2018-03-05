using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Feature Generator/Rivers")]
public class RiverGenerators : FeatureGenerator
{
	public override void Generate(Map map)
	{
		return;
		var mountains = from Tile t in map where t.tag == "Mountain" select t;

	}
}
