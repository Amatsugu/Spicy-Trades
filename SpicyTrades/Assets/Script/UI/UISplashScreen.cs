using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISplashScreen : UIPanel
{
	public UILoginPanel loginPanel;

	public void StartGame()
	{
		Hide();
		loginPanel.Show();
	}
}
