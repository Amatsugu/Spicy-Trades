using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIResourceListItem : MonoBehaviour
{
	public TextMeshProUGUI nameText;
	public TextMeshProUGUI priceText;
	public Image iconImage;
	public Button button;
	public RectTransform rectTransform;

	private void Start()
	{
		rectTransform = GetComponent<RectTransform>();
	}
}
