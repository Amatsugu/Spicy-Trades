using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileToucher : MonoBehaviour
{
	public Tile target;

	private void OnMouseUpAsButton()
	{
		MapRenderer.TouchTile(target);
	}

	private void OnMouseEnter()
	{
		target.Hover();	
	}

	private void OnMouseExit()
	{
		target.Blur();
	}
}
