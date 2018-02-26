using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Feature Generator/Towns")]
public class TownGenerator : FeatureGenerator
{
	public override void Generate(Map map)
	{
		var c = from Tile t in map where t.tag == "Ground" && t.GetNeighboringTiles().Count(nt => nt != null && nt.tag == "Water") == 3 select t;
		var cn = c.SelectMany(t => from Tile nt in t.GetNeighboringTiles() where nt != null && nt.tag == "Ground" select nt);
		var cc = from Tile t in cn where t.GetNeighboringTiles().All(nt => nt.tag == "Ground") select t;
		var ccA = cc.ToArray();
		var capitol = ccA[Random.Range(0, ccA.Length)];
		capitol.SetColor(Color.red);
	}

	
}
