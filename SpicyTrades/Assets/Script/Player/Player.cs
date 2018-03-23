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
		UIManager.ShowPricePanel(tile);
		transform.position = _curTile.WolrdPos;
		/*if(hudText == null)
			hudText = GameObject.Find("Text").GetComponent<TextMeshProUGUI>();
		var sb = new StringBuilder();
		sb.AppendLine(tile.Name + " [" + tile.tileInfo.settlementType.ToString() + "] | Population: " + tile.Population);
		var res = tile.ResourceCache;
		if (res != null)
		{
			foreach (var r in res)
				sb.AppendLine("<b>" + r.Key.PrettyName + "</b> [" + r.Key.category.ToString() + "]:" + r.Value[0] + " | Value=" + r.Value[1] * 100 + "%");
		}
		if(tile.ResourceNeeds != null)
		{
			foreach (var r in tile.ResourceNeeds)
				sb.AppendLine("<b>" + r.Resource+ "</b> [" + r.PackageType.ToString() + "]:" + r.ResourceUnits + " | $=" + r.Money);
		}
		hudText.text = sb.ToString();*/
	}

	public void MoveTo(SettlementTile tile)
	{
		if (isMoving)
			return;
		isMoving = true;
		if (_curAnimation != null)
			StopCoroutine(_curAnimation);
		StartCoroutine(MoveAnimation(Pathfinder.FindPath(GameMaster.GameMap[HexCoords.FromPosition(transform.position)], tile)));
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
