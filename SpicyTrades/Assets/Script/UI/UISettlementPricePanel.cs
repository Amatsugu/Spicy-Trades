using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISettlementPricePanel : UIPanel
{
	public TextMeshProUGUI titleText;
	public RectTransform contentBase;
	public GameObject resourceListItem;
	public Button travelButton;

	public void Show(SettlementTile tile)
	{
		Show();
		DestroyChildren(contentBase);
		titleText.text = tile.Name;
		var rCache = tile.ResourceCache;
		if (rCache == null)
			return;
		var i = 0;
		foreach (var res in rCache.Keys)
		{
			var li = Instantiate(resourceListItem, contentBase).GetComponent<UIResourceListItem>();
			li.nameText.text = res.PrettyName;
			li.priceText.text = (res.basePrice * rCache[res][1]).ToString();
			li.iconImage.sprite = res.sprite;
			var rt = li.GetComponent<RectTransform>();
			rt.anchoredPosition = new Vector2(70, i++ * -50);
		}
		contentBase.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rCache.Count * 50);
		var clickEvent = new Button.ButtonClickedEvent();
		clickEvent.AddListener(() =>
		{
			GameMaster.Player.MoveTo(tile);
			Hide();
		});
		travelButton.onClick = clickEvent;
	}

	public override void Hide()
	{
		base.Hide();
		DestroyChildren(contentBase);

	}

	public void Move(Vector2 position, bool centered = true)
	{
		if(centered)
			position.x -= PanelBase.rect.width / 2;
		PanelBase.position = ContrainToScreen(position, PanelBase.rect);
	}
}
