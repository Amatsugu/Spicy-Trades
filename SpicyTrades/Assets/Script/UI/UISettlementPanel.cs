using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class UISettlementPanel : UIPanel
{
	public TextMeshProUGUI titleText;
	public RectTransform contentBase;
	public GameObject resourceItemPanel;
	public TextMeshProUGUI infoText;
	public Button buyButton;

	public void Show(SettlementTile tile)
	{
		Show();
		if(GameMaster.CameraPan != null)
			GameMaster.CameraPan.isPaused = true;
		titleText.text = tile.Name;
		infoText.text = "Select an Item";
		var rCache = tile.ResourceCache;
		if (rCache == null)
			return;
		int i = 0;
		foreach (var res in rCache.Keys)
		{
			var li = Instantiate(resourceItemPanel, contentBase).GetComponent<UIResourceListItem>();
			li.nameText.text = res.PrettyName;
			li.priceText.text = (res.basePrice * rCache[res][1]).ToString();
			li.iconImage.sprite = res.sprite;
			var rt = li.GetComponent<RectTransform>();
			rt.anchoredPosition = new Vector2(0, i++ * -100);
			var btn = li.GetComponent<Button>();
			var btnClick = new Button.ButtonClickedEvent();
			btnClick.AddListener(() =>
			{
				buyButton.interactable = true;
				var sb = new StringBuilder();
				sb.AppendLine(res.PrettyName);
				sb.AppendLine(li.priceText.text);
				sb.AppendLine((rCache[res][1] * 100) + "%");
				infoText.text = sb.ToString();
			});
			btn.onClick = btnClick;
		}
		contentBase.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rCache.Count * 50);

	}

	public override void Hide()
	{
		base.Hide();
		buyButton.interactable = false;
		DestroyChildren(contentBase);
		if(GameMaster.CameraPan != null)
			GameMaster.CameraPan.isPaused =	false;
	}
}