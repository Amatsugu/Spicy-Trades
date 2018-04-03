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
	public TMP_InputField buyCountInput;

	public void Show(SettlementTile settlement)
	{
		Show();
		if(GameMaster.CameraPan != null)
			GameMaster.CameraPan.isPaused = true;
		titleText.text = settlement.Name;
		infoText.text = "Select an Item";
		var rCache = settlement.ResourceCache;
		if (rCache == null)
			return;
		int i = 0;
		foreach (var res in rCache.Keys)
		{
			var li = Instantiate(resourceItemPanel, contentBase).GetComponent<UIResourceListItem>();
			li.nameText.text = res.PrettyName;
			li.priceText.text = new Coin((res.basePrice * rCache[res][1])).ToString();
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
				sb.AppendLine("Cost: " + li.priceText.text.Replace('\n', ' '));
				sb.AppendLine("Value: " + (rCache[res][1] * 100) + "%");
				sb.AppendLine("Supply: " + rCache[res][0]);
				infoText.text = sb.ToString();
				buyButton.onClick = new Button.ButtonClickedEvent();
				buyButton.onClick.AddListener(() =>
				{
					Debug.Log("Buy " + res.name);
					settlement.Buy(res, int.Parse(buyCountInput.text));
#if DEBUG
					GameMaster.Player.LogItems();
#endif
					btn.onClick.Invoke();
				});
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