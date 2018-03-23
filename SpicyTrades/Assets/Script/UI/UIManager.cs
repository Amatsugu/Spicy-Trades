using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance;

	public GameObject settlementInfoWindow;

	private void Awake()
	{
		Instance = this;
	}

	public static void ShowPricePanel(SettlementTile tile)
	{
		var sw = Instance.settlementInfoWindow;
		sw.SetActive(true);
		sw.transform.position = tile.WolrdPos;
	}
}
