using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class UISettlementPanel : UIPanel
{
	public TextMeshProUGUI descriptionText;

	public Button marketButton;
	public UISettlementMarketPanel marketPanel;
	public Button eventButton;
	public UISettlementEventPanel eventPanel;

	public void Show(SettlementTile settlement)
	{
		Show();
		if(GameMaster.CameraPan != null)
			GameMaster.CameraPan.isPaused = true;
		titleText.text = settlement.Name;
		descriptionText.text = settlement.Description;
		marketButton.onClick.RemoveAllListeners();
		marketButton.onClick.AddListener(() => marketPanel.Show(settlement));
		eventButton.onClick.RemoveAllListeners();
		eventButton.onClick.AddListener(() => eventPanel.Show(settlement));
	}

	public override void Hide()
	{
		if(GameMaster.CameraPan != null)
			GameMaster.CameraPan.isPaused =	false;
		base.Hide();
		marketPanel.Hide();
	}
}
