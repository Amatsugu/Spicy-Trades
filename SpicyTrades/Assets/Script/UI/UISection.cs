using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UISection : MonoBehaviour
{
	public RectTransform rectTransform;
	public TextMeshProUGUI sectionTitle;
	public List<UIResourceListItem> items;
	public float Height => rectTransform.rect.height;

	private void Start()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	public void AddItem(UIResourceListItem item)
	{
		items.Add(item);
		item.rectTransform.SetParent(rectTransform);
		item.gameObject.SetActive(true);
		item.rectTransform.anchoredPosition = new Vector2(0, -80 - ((items.Count-1) * 105));
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 80 + (items.Count * (100 + 5)));
	}

	public void Clear()
	{
		foreach (var item in items)
		{
			item.rectTransform.parent = null;
			item.gameObject.SetActive(false);
		}
		items.Clear();
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 80);
	}
}
