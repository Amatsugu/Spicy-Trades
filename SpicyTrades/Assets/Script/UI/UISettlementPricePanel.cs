using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISettlementPricePanel : UIPanel
{
	public RectTransform contentBase;
	public GameObject resourceListItem;
	public TextMeshProUGUI noPriceText;
	public Button travelButton;

	public void Show(SettlementTile tile)
	{
		Show();
		Debug.Log("show");
		if (GameMaster.CameraPan != null)
			GameMaster.CameraPan.isPaused = true;
		DestroyChildren(contentBase);
		titleText.text = tile.Name;
		var clickEvent = new Button.ButtonClickedEvent();
		clickEvent.AddListener(() =>
		{
			Hide();
			GameMaster.Player.MoveTo(tile);
		});
		travelButton.onClick = clickEvent;
		if (!GameMaster.PriceKnowledge.ContainsKey(tile))
		{
			noPriceText.gameObject.SetActive(true);
			return;
		}
		else
			noPriceText.gameObject.SetActive(false);
		var rCache = GameMaster.PriceKnowledge[tile].Cache;
		var i = 0;
		foreach (var res in rCache.Keys)
		{
			var li = Instantiate(resourceListItem, contentBase).GetComponent<UIResourceListItem>();
			li.nameText.text = res.PrettyName;
			li.priceText.text = new Coin((res.basePrice * rCache[res])).ToString();
			li.iconImage.sprite = res.icon;
			var rt = li.GetComponent<RectTransform>();
			rt.anchoredPosition = new Vector2(70, i++ * -50);
		}
		contentBase.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rCache.Count * 50);
	}

	public override void Hide()
	{
		if (GameMaster.CameraPan != null)
			GameMaster.CameraPan.isPaused = false;
		noPriceText.gameObject.SetActive(false);
		base.Hide();
		DestroyChildren(contentBase);
	}
}
