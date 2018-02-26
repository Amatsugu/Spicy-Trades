using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder {

	public static Tile[] FindPath(Tile a, Tile b)
	{
		//Debug.Log("A: " + a.position.ToString() + " B: " + b.position.ToString());
		List<PathNode> open = new List<PathNode>
		{
			new PathNode(a, 1)
		}, closed = new List<PathNode>();
		var B = new PathNode(b, 1);
		while (open.Count > 0)
		{
			var n = open.Aggregate((c, d) => c.CalculateF(b) < d.CalculateF(b) ? c : d);
			open.Remove(n);
			closed.Add(n);
			if (closed.Contains(B))
				break;
			foreach (Tile t in n.Tile.GetNeighboringTiles())
			{
				if (t == null)
					continue;
				var adj = new PathNode(t, n.G + 1, n);
				if (closed.Contains(adj))
				{
					Debug.DrawLine(n.Tile.transform.position, t.transform.position, Color.red, 30);
					continue;
				}
				Debug.DrawLine(n.Tile.transform.position, t.transform.position, Color.green, 30);
				if (!open.Contains(adj))
				{
					open.Add(adj);
				}else
				{
					int o = open.IndexOf(adj);
					if (adj.CalculateF(b) < open[o].CalculateF(b))
						open[o] = adj;
				}
			}
		}
		var curNode = closed.Last();
		if (curNode == null)
			return null;
		List<Tile> path = new List<Tile>();
		do
		{
			path.Add(curNode.Tile);
			curNode = curNode.src;
		} while (curNode != null);
		path.Reverse();
		return path.ToArray();
	}
}

public class PathNode
{
	public Tile Tile { get; private set; }
	public int G;
	public PathNode src;

	public PathNode(Tile tile, int g, PathNode srcNode = null)
	{
		Tile = tile;
		G = g;
		src = srcNode;
	}

	public float CalculateH(Tile b)
	{
		var d = (Tile.transform.position - b.transform.position);
		return Mathf.Abs(d.x) + Mathf.Abs(d.y);
	}

	public float CalculateF(Tile b)
	{
		return (G * Tile.Cost) + CalculateH(b) ;
	}

	// override object.Equals
	public override bool Equals(object obj)
	{
		if (obj == null || GetType() != obj.GetType())
		{
			return false;
		}

		var n = obj as PathNode;
		return n.Tile == Tile;
	}

	// override object.GetHashCode
	public override int GetHashCode()
	{
		return Tile.GetHashCode();
	}
}