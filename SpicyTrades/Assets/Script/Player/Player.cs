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
	public Coin Money { get; private set; }
	public SettlementTile CurrentTile
	{
		get
		{
			return _curTile;
		}
	}

	private SettlementTile _curTile;
	private SpriteRenderer _sprite;
	private Coroutine _curAnimation;
	private List<InventoryItem> _inventory;

	private void Start()
	{
		_sprite = GetComponent<SpriteRenderer>();
		_inventory = new List<InventoryItem>();
		Money = new Coin(10000f);
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
		_curAnimation = StartCoroutine(MoveAnimation(Pathfinder.FindPath(GameMaster.GameMap[HexCoords.FromPosition(transform.position)], tile.Center)));
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

	public void AddItem(InventoryItem item)
	{
		var invItem = _inventory.FirstOrDefault(i => i.Package.Resource == item.Package.Resource);
		if (invItem == null)
		{
			invItem = new InventoryItem
			{
				Package = item.Package,
				Cost = item.Cost
			};
			_inventory.Add(invItem);
		}else
		{
			invItem.Cost = invItem.Cost + invItem.Cost;
			invItem.Cost /= 2f;
			var p = invItem.Package;
			p.ResourceUnits = p.ResourceUnits + item.Package.ResourceUnits;
			invItem.Package = p;
		}
	}

	public bool TakeItem(InventoryItem item)
	{
		var invItem = _inventory.First(i => i.Package.Resource == item.Package.Resource);
		if (invItem == null)
			return false;
		else
		{
			if(invItem.Package.ResourceUnits < item.Package.ResourceUnits)
				return false;
			else
			{
				if (invItem.Package.ResourceUnits == item.Package.ResourceUnits)
				{
					_inventory.Remove(invItem);
					return true;
				}else
				{
					var p = invItem.Package;
					p.ResourceUnits -= item.Package.ResourceUnits;
					invItem.Package = p;
					return true;
				}
			}
		}
	}

	public void AddMoney(float ammount)
	{
		ammount = Mathf.Abs(ammount);
		Money += ammount; 
	}

	public bool TakeMoney(float ammount)
	{
		ammount = Mathf.Abs(ammount);
		if (ammount > Money)
			return false;
		Money -= ammount;
		return true;
	}

#if DEBUG
	public void LogItems()
	{
		foreach (var p in _inventory)
			Debug.Log(p.Package.Resource + " : " + p.Package.ResourceUnits);
	}
#endif

}
