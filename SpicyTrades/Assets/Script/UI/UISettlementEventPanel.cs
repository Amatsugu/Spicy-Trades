using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISettlementEventPanel : UIPanel
{
	public RectTransform uiListItem;
	public RectTransform contentBase;
	public Button contributeButton;
	public Image itemIcon;
	public TextMeshProUGUI itemName;
	public TextMeshProUGUI itemPrice;
	public TextMeshProUGUI itemDescription;

	private List<UIResourceListItem> _list = new List<UIResourceListItem>();
	private SettlementTile _currentSettlement;
	private ResourceIdentifier _selectedNeed;
	public void Show(SettlementTile settlement)
	{
		base.Show();
		titleText.text = settlement.Name + "'s Current Events";
		_currentSettlement = settlement;
		RefreshList();
	}

	public void RefreshList()
	{
		var resCount = _currentSettlement.currentEvents.Sum(c => c.ResourceNeeds.Length);
		var needs = _currentSettlement.currentEvents.SelectMany(e => e.ResourceNeeds);
		if (_selectedNeed == null || _selectedNeed.count == 0 || _selectedNeed.source.EndTime <= GameMaster.CurrentTick)
		{
			_selectedNeed = needs.FirstOrDefault(need => need.count > 0);
			UpdateInfo(_selectedNeed);
		}
		if (_list.Count < resCount)
		{
			for (int i = _list.Count; i < resCount; i++)
			{
				var resUI = Instantiate(uiListItem, contentBase);
				resUI.anchoredPosition = new Vector2(0, -i * (resUI.rect.height + 5));
				_list.Add(resUI.GetComponent<UIResourceListItem>());
				resUI.gameObject.SetActive(false);
			}
		}
		var n = 0;
		var resList = GameMaster.Registry.resourceList.GetResourceList();
		foreach (var need in needs)
		{
			if (need.count == 0)
				continue;
			var UIItem = _list[n++];
			UIItem.gameObject.SetActive(true);
			UIItem.nameText.text = need.resource;
			UIItem.priceText.text = need.count.ToString();
			UIItem.iconImage.sprite = resList.First(res => need.Match(res)).icon;
			UIItem.button.onClick.RemoveAllListeners();
			UIItem.button.onClick.AddListener(() => UpdateInfo(need));
		}
		contentBase.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, n * (_list.First().GetComponent<RectTransform>().rect.height + 5));
		for (int i = n; i < _list.Count; i++)
		{
			_list[i].gameObject.SetActive(false);
		}
	}

	private void UpdateInfo(ResourceIdentifier need)
	{
		if (_selectedNeed == null)
			return;
		_selectedNeed = need;
		itemName.text = _selectedNeed.resource;
		var resList = GameMaster.Registry.resourceList.GetResourceList();
		var price = resList.Where(res => _selectedNeed.Match(res)).Average(res => res.basePrice) * 10;
		itemPrice.text = "<color=#ff0064>" + price + "</color>";
		itemIcon.sprite = resList.First(res => need.Match(res)).icon;
		var matchedPlayerItems = GameMaster.Player.inventory.SelectMany(inv => resList.Where(res => inv.Resource.count > 0 && inv.Resource.Match(res)));
		var sb = new StringBuilder();
		sb.Append("<b>");
		sb.Append(need.source.Name);
		sb.AppendLine("</b>");
		sb.AppendLine(need.source.Event.description);
		if(matchedPlayerItems.Count() > 0)
		{
			sb.AppendLine("Your Matching Items:");
			foreach(var item in matchedPlayerItems)
			{
				sb.Append(item.PrettyName);
				sb.Append(" [");
				sb.Append(GameMaster.Player.inventory.First(inv => inv.Resource.Match(item)).Resource.count);
				sb.AppendLine("]");
			}
		}else
		{
			sb.AppendLine("You have no matching Items");
		}
		itemDescription.text = sb.ToString();
	}
}
