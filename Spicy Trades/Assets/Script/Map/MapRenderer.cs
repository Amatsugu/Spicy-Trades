using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapRenderer : MonoBehaviour
{

    public Vector2 size = new Vector2(5, 5);
	public float outerRadius = 0.433f;
    public Transform tile;

	public static Tile[] Tiles;
	public static MapRenderer Map;
	public static float InnerRadius;
	private List<Transform> _tiles;
	public bool regen = false;

	private static Tile A, B;
	private static bool endPoint = true;
	private static Tile[] oldPath = null;
    // Use this for initialization
    void Start()
    {
		Map = this;
		InnerRadius = outerRadius * Mathf.Sqrt(3)/2;
		Tiles = new Tile[(int)(size.x * size.y * 4)];
		_tiles = new List<Transform>();
        for (int y = 0, i = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
				var pos = new Vector3
				{
					y = y * (InnerRadius * 1.5f),
					x = (x + y * .5f - y/2) * (InnerRadius * 2f),
				};
				var g = Instantiate(tile, pos, Quaternion.Euler(0, 0, 90), transform);
                g.GetComponent<SpriteRenderer>().color = Color.HSVToRGB((x + size.x)/(2*size.x), Mathf.Lerp(.4f, 1, (y + size.y) / (2*size.y)), 1);
				var t = g.GetComponent<Tile>().SetPos(x, y);
				if (Tiles[i] != null)
					Debug.Log("Collision");
				Tiles[i++] = t;
				_tiles.Add(g);
            }
        }
		
    }

    // Update is called once per frame
    void Update()
    {
		if (!regen)
			return;
		regen = false;
		for (int i = 0; i < _tiles.Count; i++)
		{
			Destroy(_tiles[i].gameObject);
		}
		Start();
    }

	public static Tile GetTile(int x, int y, int z)
	{
		int index = x + y * (int)Map.size.x + y / 2;
		if (index < 0 || index > Tiles.Length)
			return null;
		return Tiles[index];
	}

	public static void TouchTile(Tile tile)
	{
		if (endPoint)
			A = tile;
		else
			B = tile;
		if (oldPath != null)
			foreach (var t in oldPath)
				t.ResetColor();
		endPoint = !endPoint;
		if(A != null && B != null)
		{
			oldPath = Pathfinder.FindPath(A, B);
			foreach (var t in oldPath)
				t.SetColor(Color.white);
		}
		if(A != null)
			A.SetColor(Color.green);
		if(B != null)
			B.SetColor(Color.grey);
	}
}
