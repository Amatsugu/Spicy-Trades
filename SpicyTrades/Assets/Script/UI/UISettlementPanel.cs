using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISettlementPanel : UIPanel
{
	public TextMeshProUGUI titleText;
	public GameObject resourceItemPanel;

	public void Show(SettlementTile tile)
	{
		Show();

	}

	public override void Hide()
	{
		base.Hide();
	}
}