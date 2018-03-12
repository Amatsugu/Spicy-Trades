using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
	public float moveSpeed = 1f;
	public bool isMoving = false;

	private TownTile _curTile;
	private SpriteRenderer _sprite;

	private void Start()
	{
		_sprite = GetComponent<SpriteRenderer>();
	}

	public void SetTile(TownTile tile)
	{
		_curTile = tile;
		transform.position = _curTile.WolrdPos;
	}

	public void MoveTo(TownTile tile)
	{
		if (isMoving)
			return;
		isMoving = true;
		StartCoroutine(MoveAnimation(Pathfinder.FindPath(_curTile, tile)));
	}

	IEnumerator MoveAnimation(Tile[] path)
	{
		for (int i = 1; i < path.Length; i++)
		{
			float time = 0;
			while(time < 1)
			{
				transform.position = Vector3.Lerp(path[i-1].WolrdPos, path[i].WolrdPos, time += Time.deltaTime * moveSpeed);
				if (path[i - 1].WolrdPos.x > path[i].WolrdPos.x)
					_sprite.flipX = true;
				else
					_sprite.flipX = false;
				yield return new WaitForEndOfFrame();
			}
		}
		_curTile = path.Last() as TownTile;
		isMoving = false;
	}
}
