using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
	public float moveSpeed = 1f;
	public bool isMoving = false;
	public Text resourceList;

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
		var res = tile.Resources;
		if (res == null)
			return;
		if(resourceList == null)
			resourceList = GameObject.Find("Text").GetComponent<Text>();
		resourceList.text = "";
		foreach (var r in res)
			resourceList.text += "<b><color=#"+ColorUtility.ToHtmlStringRGB(r.color)+">" + r.ResourceName + "</color></b> [" + r.category.ToString() + "]: Y=" + r.yeild + " | W=" + r.requiredWorkers + "\n";
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
		SetTile(path.Last() as TownTile);
		isMoving = false;
	}
}
