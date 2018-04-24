using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
	public float moveSpeed = 1f;
	public bool isMoving = false;

	private SpriteRenderer _sprite;
	private Coroutine _curAnimation;
	private Player _player;

	public void Start()
	{
		_sprite = GetComponent<SpriteRenderer>();
	}

	public void MoveTo(SettlementTile tile)
	{
		if (isMoving)
			return;
		isMoving = true;
		if (_curAnimation != null)
			StopCoroutine(_curAnimation);
		_curAnimation = StartCoroutine(MoveAnimation(Pathfinder.FindPath(GameMaster.GameMap[HexCoords.FromPosition(transform.position)], tile.Center)));
	}

	public void SetPlayer(Player player)
	{
		_player = player;
	}

	IEnumerator MoveAnimation(Tile[] path)
	{
		for (int i = 1; i < path.Length; i++)
		{
			float time = 0;
			while (time < 1)
			{
				transform.position = Vector3.Lerp(path[i - 1].WolrdPos, path[i].WolrdPos, time += Time.deltaTime * moveSpeed);
				if (path[i - 1].WolrdPos.x > path[i].WolrdPos.x)
					_sprite.flipX = true;
				else
					_sprite.flipX = false;
				yield return new WaitForEndOfFrame();
			}
		}
		_player.SetTile(path.Last() as SettlementTile);
		isMoving = false;
	}
}
