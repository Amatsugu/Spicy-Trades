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
	public RectTransform sectionItem;
	public RectTransform contentBase;
	public Button contributeButton;
	public TextMeshProUGUI contributionText;
	public Image itemIcon;
	public TextMeshProUGUI itemName;
	public TextMeshProUGUI itemPrice;
	public TextMeshProUGUI itemDescription;
	public TMP_InputField countInput;

	private List<UIResourceListItem> _list = new List<UIResourceListItem>();
	private List<UISection> _sections = new List<UISection>();
	private SettlementTile _currentSettlement;
	private ResourceIdentifier _selectedNeed;
	public void Show(SettlementTile settlement)
	{
		base.Show();
		titleText.text = $"{settlement.Name}'s Current Events";
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
				_list.Add(resUI.GetComponent<UIResourceListItem>());
				resUI.gameObject.SetActive(false);
			}
		}
		var eventCount = _currentSettlement.currentEvents.Count;
		if (_sections.Count < eventCount)
		{
			for(var i = _sections.Count; i < eventCount; i++)
			{
				var section = Instantiate(sectionItem, contentBase);
				_sections.Add(section.GetComponent<UISection>());
				section.gameObject.SetActive(false);
			}
		}
		var n = 0;
		var s = 0;
		var resList = GameMaster.Registry.resourceList.GetResourceList();
		var groupedNeeds = needs.GroupBy(need => need.source, need => need);
		foreach (var group in groupedNeeds)
		{
			if (group.Count() == 0 || group.All(need => need.count == 0))
				continue;
			var section = _sections[s++];
			section.Clear();
			section.sectionTitle.text = group.Key.Name;
			section.gameObject.SetActive(true);
			section.rectTransform.anchoredPosition = new Vector2(0, -_sections.Take(s-1).Sum(sec => sec.Height));
			foreach (var need in group)
			{
				if (need.count == 0)
					continue;
				var UIItem = _list[n++];
				section.AddItem(UIItem);
				UIItem.gameObject.SetActive(true);
				UIItem.nameText.text = need.resource;
				UIItem.priceText.text = need.count.ToString();
				UIItem.iconImage.sprite = resList.First(res => need.Match(res)).icon;
				UIItem.button.onClick.RemoveAllListeners();
				UIItem.button.onClick.AddListener(() => UpdateInfo(need));
			}
		}
		for (int i = n; i < _list.Count; i++)
		{
			_list[i].gameObject.SetActive(false);
		}
		for (int i = s; i < _sections.Count; i++)
		{
			_sections[i].Clear();
			_sections[i].gameObject.SetActive(false);
		}
		contentBase.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _sections.Sum(section => section.gameObject.activeInHierarchy ? section.Height : 0));
	}

	private void UpdateInfo(ResourceIdentifier need)
	{
		if (_selectedNeed == null)
		{
			itemName.text = "No Item";
			itemIcon.sprite = null;
			itemPrice.text = "";
			itemDescription.text = "";
			return;
		}
		_selectedNeed = need;
		UpdateContributeButton();
		var count = int.Parse(countInput.text);
		
		itemName.text = _selectedNeed.resource;
		var resList = GameMaster.Registry.resourceList.GetResourceList();
		var price = (int)(resList.Where(res => _selectedNeed.Match(res)).Average(res => res.basePrice) * 10);
		itemPrice.text = $"<color=#ff0064>{price}</color>";
		itemIcon.sprite = resList.First(res => need.Match(res)).icon;
		var matchedPlayerItems = GameMaster.Player.inventory.Where(inv => need.Match(inv.ActualResource)).Select(inv => inv.ActualResource);
		var sb = new StringBuilder();
		sb.Append("<b>");
		sb.Append(need.source.Name);
		sb.AppendLine("</b>");
		sb.AppendLine(need.source.Event.description);
		if(matchedPlayerItems.Count() > 0)
		{
			contributeButton.interactable = (count > 0);
			sb.AppendLine("Your Matching Items:");
			foreach(var item in matchedPlayerItems)
			{
				sb.Append(item.PrettyName);
				sb.Append(" [");
				sb.Append(GameMaster.Player.inventory.First(inv => inv.Resource.Match(item)).Resource.count);
				sb.AppendLine("]");
			}
			contributeButton.onClick.RemoveAllListeners();
			contributeButton.onClick.AddListener(() =>
			{
				var item = GameMaster.Player.inventory.First(inv => inv.Resource.Match(matchedPlayerItems.First()));
				if (GameMaster.Player.TakeItem(new InventoryItem
				{
					Resource = new ResourceIdentifier
					{
						resource = item.Resource.resource,
						count = count,
					}
				}))
				GameMaster.Player.Influence += price;
				RefreshList();
				UpdateInfo(_selectedNeed);
			});
		}else
		{
			contributeButton.interactable = false;
			sb.AppendLine("You have no matching Items");
		}
		itemDescription.text = sb.ToString();
	}

	public void UpdateContributeButton()
	{
		var count = int.Parse(countInput.text);
		var need = _selectedNeed;
		if(need == null)
		{
			contributionText.text = "Contribute";
			return;
		}
		if (count < 0)
		{
			countInput.text = "0";
		}
		if (count > need.count)
		{
			countInput.text = need.count.ToString();
			count = (int)need.count;
		}
		var price = (int)(GameMaster.Registry.resourceList.GetResourceList().Where(res => _selectedNeed.Match(res)).Average(res => res.basePrice) * 10);
		contributionText.text = $"Contribute (+<color=#ff0064>{count * price}</color>)";
	}
}
