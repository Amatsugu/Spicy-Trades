using LuminousVector;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance;
	public new Camera camera;

	public UISettlementPricePanel pricePanel;
	public UISettlementPanel settlementPanel;

	private Vector3 _priceWindowPos;
	private UIList _windowList;
	private bool _inventory;

	private void Awake()
	{
		Instance = this;
		GameMaster.GameReady += () => GameMaster.GameMap.OnMapSimulate += m =>
		{
			if (settlementPanel.IsOpen)
			{
				settlementPanel.Refresh();
				//ShowSettlementPanel(GameMaster.Player.CurrentTile);
			}
		};
		_windowList = pricePanel.contentBase.GetComponent<UIList>();
	}

	public static void ShowPricePanel(SettlementTile tile)
	{
		if (Instance.settlementPanel.IsOpen)
			return;
		Instance._priceWindowPos = tile.WolrdPos;
		Instance.pricePanel.Show(tile);
	}

	public void ShowInventory()
	{
		Instance._inventory = !Instance._inventory;
		Debug.Log(Instance._inventory);
	}

	public static void ShowSettlementPanel(SettlementTile tile)
	{
		Instance.settlementPanel.Show(tile);
	}


	private void Update()
	{
		//var pos = Instance.camera.WorldToScreenPoint(_priceWindowPos);
		//Instance.pricePanel.Move(pos);
	}

	private void OnGUI()
	{
		if (!_inventory)
			return;

		var res = GameMaster.Registry.resourceList;
		GUILayout.Label(" ");
		GUILayout.Label(" ");
		GUILayout.Label("Money: " + GameMaster.Player.Money.ToString(" "));
		GUILayout.Label("Inventory: ");
		foreach(var item in GameMaster.Player.inventory)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("\t" + res.GetResourceByName(item.Resource.resource).PrettyName);
			GUILayout.Label(new Coin(item.Cost).ToString(" "));
			GUILayout.Label("[" + item.Resource.count + "]");
			GUILayout.EndHorizontal();
		}
	}
}
