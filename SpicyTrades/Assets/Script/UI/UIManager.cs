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

	private Vector3 priceWindowPos;
	private UIList windowList;

	private void Awake()
	{
		Instance = this;
		windowList = pricePanel.contentBase.GetComponent<UIList>();
	}

	public static void ShowPricePanel(SettlementTile tile)
	{
		Instance.priceWindowPos = tile.WolrdPos;
		Instance.pricePanel.Show(tile);
	}

	private void Update()
	{
		var pos = Instance.camera.WorldToScreenPoint(priceWindowPos);
		Instance.pricePanel.Move(pos);
	}
}
