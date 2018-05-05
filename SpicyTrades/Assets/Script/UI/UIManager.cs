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
	public TextMeshProUGUI moneyText;
	public TextMeshProUGUI influenceText;

	private bool _inventory;
	private bool _gameReady = false;

	private void Awake()
	{
		Instance = this;
		GameMaster.GameReady += () => _gameReady = true;
		GameMaster.GameReady += () => GameMaster.GameMap.OnMapSimulate += m =>
		{
			if (settlementPanel.IsOpen && settlementPanel.marketPanel.IsOpen)
			{
				settlementPanel.marketPanel.Refresh();
			}
			if (settlementPanel.IsOpen && settlementPanel.eventPanel.IsOpen)
			{
				settlementPanel.eventPanel.RefreshList();
			}
		};
	}

	public static void ShowPricePanel(SettlementTile tile)
	{
		if (Instance.settlementPanel.IsOpen)
			return;
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
		if (!_gameReady)
			return;
		moneyText.text = GameMaster.Player.Money.ToString(" ");
		influenceText.text = $"<color=#ff0064>{GameMaster.Player.Influence}</color>";
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
