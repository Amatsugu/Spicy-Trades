using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UILoginPanel : UIPanel
{
	public TMP_InputField login;
	public TMP_InputField password;

	public void Login()
	{
		var loginString = login.text;
		var passwordString = password.text;
		if(string.IsNullOrEmpty(loginString) || string.IsNullOrEmpty(passwordString))
		{
			//TODO: Give Error
			return;
		}
	}
}
