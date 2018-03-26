using LuminousVector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance;

	public RectTransform settlementInfoWindow;
	public TextMeshProUGUI settlementInfoTitleText;
	public RectTransform settlementInfoScrollView;
	public GameObject resourceListItem;
	public new Camera camera;

	private Vector3 priceWindowPos;
	private UIList windowList;

	private void Awake()
	{
		Instance = this;
		windowList = settlementInfoScrollView.GetComponent<UIList>();
	}

	public static void ShowPricePanel(SettlementTile tile)
	{
		var sw = Instance.settlementInfoWindow;
		Instance.settlementInfoTitleText.text = tile.Name;
		var windowContent = Instance.settlementInfoScrollView;
		sw.gameObject.SetActive(true);
		DestroyChildren(windowContent);
		Instance.priceWindowPos = tile.WolrdPos;
		var rCache = tile.ResourceCache;
		if (rCache == null)
			return;
		var i = 0;
		foreach(var res in rCache.Keys)
		{
			var li = GameObject.Instantiate(Instance.resourceListItem, windowContent).GetComponent<UIResourceListItem>();
			li.nameText.text = res.PrettyName;
			li.priceText.text = (res.basePrice * rCache[res][1]).ToString();
			li.iconImage.sprite = res.sprite;
			var rt = li.GetComponent<RectTransform>();
			rt.anchoredPosition = new Vector2(70, i++ * -50);
		}
		windowContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rCache.Count * 50);
	}

	private void Update()
	{
		var sw = Instance.settlementInfoWindow;
		var pos = Instance.camera.WorldToScreenPoint(priceWindowPos);
		pos.x -= sw.rect.width / 2;
		pos = ContrainToScreen(pos, sw.rect);
		sw.position = pos;

	}

	private static void DestroyChildren(Transform transform)
	{
		for(int i = 0; i < transform.childCount; i++)
		{
			Destroy(transform.GetChild(i).gameObject);
		}

	}

	private Vector3 ContrainToScreen(Vector3 pos, Rect rect)
	{
		if (pos.x < 0)
			pos.x = 0;
		if (pos.x + rect.width > Screen.width)
			pos.x = Screen.width - rect.width;
		if (pos.y - rect.height < 0)
			pos.y = rect.height;
		if (pos.y > Screen.height)
			pos.y = Screen.height;
		return pos;
	}
}
