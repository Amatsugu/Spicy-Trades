using LuminousVector;
using System;
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

	private Vector3 priceWindowPos;
	private UIList windowList;

	private void Awake()
	{
		Instance = this;
		GameMaster.GameMap.OnMapSimulate += m =>
		{
			if(settlementPanel.IsOpen)
			{
				settlementPanel.Hide();
				ShowSettlementPanel(GameMaster.Player.CurrentTile);
			}
		};
		windowList = pricePanel.contentBase.GetComponent<UIList>();
	}

	public static void ShowPricePanel(SettlementTile tile)
	{
		if (Instance.settlementPanel.IsOpen)
			return;
		Instance.priceWindowPos = tile.WolrdPos;
		Instance.pricePanel.Show(tile);
	}

	public static void ShowInventory()
	{

	}

	public static void ShowSettlementPanel(SettlementTile tile)
	{
		Instance.settlementPanel.Show(tile);
	}


	private void Update()
	{
		var pos = Instance.camera.WorldToScreenPoint(priceWindowPos);
		Instance.pricePanel.Move(pos);
	}
}
