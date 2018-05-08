using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NetworkManager;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UILoginPanel : UIPanel
{
	public TMP_InputField login;
	public TMP_InputField password;
	
	public void Login()
	{
		Debug.Log("Logging in...");
		var loginString = login.text;
		var passwordString = password.text;
		if(string.IsNullOrEmpty(loginString) || string.IsNullOrEmpty(passwordString))
		{
			Debug.Log("Empty Login");
			return;
		}
		if(SpicyNetwork.Login(loginString, passwordString))
		{
			SceneManager.LoadScene("main");
			return;
		}
		Debug.Log("Failed to login	");
	}

	public void Register()
	{
	}
}
