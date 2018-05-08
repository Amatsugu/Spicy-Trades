using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UISettlementMarketPanel : UIPanel
{
	public Image itemIcon;
	public TextMeshProUGUI itemName;
	public TextMeshProUGUI itemPrice;
	public TextMeshProUGUI itemDescription;
	public RectTransform contentBase;
	public RectTransform resoruceItemUI;


	public TMP_InputField countInput;

	public Button buyButton;
	public TextMeshProUGUI buyButtonText;

	public Button buyTab;
	public Button sellTab;

	private ResourceTileInfo _selectedResource;
	private SettlementTile _currentSettlement;

	private List<UIResourceListItem> _list = new List<UIResourceListItem>();

	private MarketMode _curMode;

	public enum MarketMode
	{
		Buy,
		Sell
	}

	public void Show(SettlementTile settlement)
	{
		Show();
		titleText.text = $"{settlement.Name}'s Market";
		_currentSettlement = settlement;
		var res = settlement.ResourceCache.Keys.ToArray();
		_selectedResource = res.Length == 0 ? null : res[0];
		ShowItemInfo();
		RenderResources();
	}

	public void ShowItemInfo()
	{
		if (_selectedResource == null)
		{
			itemName.text = "No Item";
			itemIcon.sprite = null;
			itemPrice.text = "";
			itemDescription.text = "";
			UpdateBuyButton();
			return;
		}
		itemIcon.sprite = _selectedResource.icon;
		itemName.text = _selectedResource.PrettyName;
		if(_curMode == MarketMode.Buy)
		{
			int count = 0;
			if(GameMaster.Player.inventory.Count != 0)
			{
				var res = GameMaster.Player.inventory.FirstOrDefault(inv => inv.Resource.Match(_selectedResource));
				count = (res != null) ? (int)res.Resource.count : 0;
			}
			itemName.text += $" ({count} Owned)";
		}
		var value = (_currentSettlement.ResourceCache.ContainsKey(_selectedResource)) ? _currentSettlement.ResourceCache[_selectedResource][1] : 1.5f;
		var unitPrice = new Coin(value * _selectedResource.basePrice);
		itemPrice.text = unitPrice.ToString(" ");
		itemDescription.text = $"{_selectedResource.description} \n {_selectedResource.tooltip}" ;
		UpdateBuyButton();
	}

	public void RenderResources()
	{
		int resCount;
		if (_curMode == MarketMode.Buy)
			resCount = _currentSettlement.ResourceCache.Keys.Count;
		else
			resCount = GameMaster.Player.inventory.Count;
		if(_list.Count < resCount)
		{                            
			for (int i = _list.Count; i < resCount; i++)
			{
				var resUI = Instantiate(resoruceItemUI, contentBase);
				resUI.anchoredPosition = new Vector2(0, -i * (resUI.rect.height + 5));
				_list.Add(resUI.GetComponent<UIResourceListItem>());
			}
		}
		var r = 0;
		if (_curMode == MarketMode.Buy)
		{
			foreach (var res in _currentSettlement.ResourceCache)
			{
				if (res.Value[0] == 0)
					continue;
				var resUI = _list[r++];
				resUI.gameObject.SetActive(true);
				resUI.iconImage.sprite = res.Key.icon;
				resUI.nameText.text = res.Key.PrettyName;
				resUI.priceText.text = new Coin(res.Key.basePrice * res.Value[1]).ToString();
				resUI.button.onClick.RemoveAllListeners();
				resUI.button.onClick.AddListener(() =>
				{
					_selectedResource = res.Key;
					ShowItemInfo();
				});
			}
		}else
		{
			var resList = GameMaster.Registry.resourceList.GetResourceList();
			foreach (var invItem in GameMaster.Player.inventory)
			{
				var res = resList.Single(resI => invItem.Resource.Match(resI));
				var resUI = _list[r++];
				resUI.gameObject.SetActive(true);
				resUI.iconImage.sprite = res.icon;
				resUI.nameText.text = res.PrettyName;
				var value = (_currentSettlement.ResourceCache.ContainsKey(res)) ? _currentSettlement.ResourceCache[res][1] : 1.5f;
				resUI.priceText.text = new Coin(res.basePrice * value).ToString();
				resUI.button.onClick.RemoveAllListeners();
				resUI.button.onClick.AddListener(() =>
				{
					_selectedResource = res;
					ShowItemInfo();
				});
			}
		}
		if(_list.Count > 0)
			contentBase.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, r * (_list.First().GetComponent<RectTransform>().rect.height + 5));

		for (int i = r; i < _list.Count; i++)
		{
			_list[i].gameObject.SetActive(false);
		}
	}

	public void UpdateBuyButton()
	{
		if (string.IsNullOrEmpty(countInput.text) || _selectedResource == null)
		{
			buyButtonText.text = _curMode.ToString();
			buyButton.onClick.RemoveAllListeners();
			buyButton.interactable = false;
		}
		else
		{
			buyButton.interactable = true;
			var value = (_currentSettlement.ResourceCache.ContainsKey(_selectedResource)) ? _currentSettlement.ResourceCache[_selectedResource][1] : 1.5f;
			var unitPrice = new Coin(value * _selectedResource.basePrice);
			var count = int.Parse(countInput.text);
			if (_curMode == MarketMode.Buy)
			{
				count = (int)Mathf.Min(count, _currentSettlement.ResourceCache[_selectedResource][0]);
				buyButton.interactable = (unitPrice * count) <= GameMaster.Player.Money;
			}
			else
				count = (int)Mathf.Min(count, GameMaster.Player.inventory.Single(res => res.Resource.Match(_selectedResource)).Resource.count);
			if (count == 0)
				buyButton.interactable = false;
			countInput.text = count.ToString();
			buyButtonText.text =  $"{_curMode} ({(_curMode == MarketMode.Buy ? "-" : "+")}{(count * unitPrice).ToString(" ")})";
			buyButton.onClick.RemoveAllListeners();
			if (_curMode == MarketMode.Buy)
				buyButton.onClick.AddListener(() => 
				{
					_currentSettlement.Buy(_selectedResource, count, GameMaster.Player);
					if (_currentSettlement.ResourceCache[_selectedResource][0] == 0)
						_selectedResource = _currentSettlement.ResourceCache.Keys.FirstOrDefault(res => _currentSettlement.ResourceCache[res][0] != 0);
					Refresh();
				});
			else
				buyButton.onClick.AddListener(() => 
				{
					GameMaster.Player.Sell(_selectedResource, count, _currentSettlement);
					if(GameMaster.Player.inventory.FirstOrDefault(inv => inv.Resource.Match(_selectedResource)) == null)
					{
						if (GameMaster.Player.inventory.Count == 0)
							_selectedResource = null;
						else
							_selectedResource = GameMaster.Registry.resourceList.GetResourceList().Single(res => GameMaster.Player.inventory.First().Resource.Match(res));
					}
					Refresh();
				});
		}
	}

	public void SwitchToBuy()
	{
		if (_curMode == MarketMode.Buy)
			return;
		var sellRect = sellTab.GetComponent<RectTransform>();
		var buyRect = buyTab.GetComponent<RectTransform>();
		var posSell = sellRect.GetSiblingIndex();
		var posBuy = buyRect.GetSiblingIndex();
		sellRect.SetSiblingIndex(posBuy);
		buyRect.SetSiblingIndex(posSell);
		_curMode = MarketMode.Buy;
		var res = _currentSettlement.ResourceCache.Keys.ToArray();
		_selectedResource = res.Length == 0 ? null : res[0];
		Refresh();
	}

	private void OnGUI()
	{
		return;
		GUILayout.Label(" ");
		GUILayout.Label(" ");
		GUILayout.Label(" ");
		GUILayout.Label(" ");
		GUILayout.Label(_currentSettlement.ResourceNeeds.Count.ToString());
		GUILayout.Label(_currentSettlement.currentEvents.Count.ToString());
		foreach(var need in _currentSettlement.ResourceNeeds)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(need.resource);
			GUILayout.Label(need.type.ToString());
			GUILayout.Label(need.count.ToString());
			GUILayout.EndHorizontal();
		}
	}

	public void SwitchToSell()
	{
		if (_curMode == MarketMode.Sell)
			return;
		var sellRect = sellTab.GetComponent<RectTransform>();
		var buyRect = buyTab.GetComponent<RectTransform>();
		var posSell = sellRect.GetSiblingIndex();
		var posBuy = buyRect.GetSiblingIndex();
		sellRect.SetSiblingIndex(posBuy);
		buyRect.SetSiblingIndex(posSell);
		_curMode = MarketMode.Sell;
		if (GameMaster.Player.inventory.Count == 0)
			_selectedResource = null;
		else
			_selectedResource = GameMaster.Registry.resourceList.GetResourceList().Single(resI => GameMaster.Player.inventory.First().Resource.Match(resI));
		Refresh();
	}

	public void Refresh()
	{
		if (_currentSettlement == null)
			return;
		RenderResources();
		ShowItemInfo();
	}

	public override void Hide()
	{
		base.Hide();
		_currentSettlement = null;
		_selectedResource = null;
		//if (GameMaster.CameraPan != null)
		//	GameMaster.CameraPan.isPaused = false;
	}
}
