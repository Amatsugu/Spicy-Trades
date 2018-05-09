using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRegisterPanel : UIPanel {

	public UILoginPanel loginPanel;

	public void CloseRegister()
	{
		Hide();
		loginPanel.Show();
	}
}
