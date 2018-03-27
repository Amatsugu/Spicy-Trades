using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
	public float moveSpeed = 1f;
	public bool isMoving = false;
	public TextMeshProUGUI hudText;

	private SettlementTile _curTile;
	private SpriteRenderer _sprite;
	private Coroutine _curAnimation;

	private void Start()
	{
		_sprite = GetComponent<SpriteRenderer>();
	}

	public void SetTile(SettlementTile tile)
	{
		_curTile = tile;
		GameMaster.CachePrices(tile);
		UIManager.ShowSettlementPanel(tile);
		transform.position = _curTile.WolrdPos;
	}

	public void MoveTo(SettlementTile tile)
	{
		if (isMoving)
			return;
		isMoving = true;
		if (_curAnimation != null)
			StopCoroutine(_curAnimation);
		StartCoroutine(MoveAnimation(Pathfinder.FindPath(GameMaster.GameMap[HexCoords.FromPosition(transform.position)], tile.Center)));
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
		SetTile(path.Last() as SettlementTile);
		isMoving = false;
	}
}
