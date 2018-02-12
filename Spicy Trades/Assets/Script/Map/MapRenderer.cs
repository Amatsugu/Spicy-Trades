using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRenderer : MonoBehaviour
{

    public Vector2 size = new Vector2(5, 5);
	public float outerRadius = 0.433f;
    public Transform tile;

	public static Tile[] Tiles;
	public static MapRenderer Map;
	private List<Transform> _tiles;
	public bool regen = false;

    // Use this for initialization
    void Start()
    {
		Map = this;
		var ir = outerRadius * Mathf.Sqrt(3)/2;
		Tiles = new Tile[(int)(size.x * size.y * 4)];
		_tiles = new List<Transform>();
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
				var pos = new Vector3
				{
					x = x * (ir * 1.5f),
					y = (y + x * .5f - x/2) * (ir * 2f),
				};
				var g = Instantiate(tile, pos, Quaternion.identity, transform);
                g.GetComponent<SpriteRenderer>().color = Color.HSVToRGB((x + size.x)/(2*size.x), Mathf.Lerp(.4f, 1, (y + size.y) / (2*size.y)), 1);
				Tiles[(x) + (y) * (int)size.x] = g.GetComponent<Tile>().SetPos(x, y);
				_tiles.Add(g);
            }
        }
		var t = Tile.GetNeighboringTiles(14, 17);
		foreach(Tile tt in t)
		{
			if (tt == null)
				continue;
			tt.SetColor(Color.red);
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

	public static Tile GetTile(int x, int y)
	{
		if (x < 0 || y < 0)
			return null;
		if (x > Map.size.x || y > Map.size.y)
			return null;
		return Tiles[(x) + (y) * (int)Map.size.x];
	}
}
