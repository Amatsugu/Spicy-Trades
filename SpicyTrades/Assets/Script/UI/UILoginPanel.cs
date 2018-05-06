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

	private void Start()
	{
		if(!NetworkManager.Network.Connect("spicy2.luminousvector.com", 9614))
		{
			Debug.LogError("Unable to Connect to Server!");
		}
	}
	public void Login()
	{
		var loginString = login.text;
		var passwordString = password.text;
		if(string.IsNullOrEmpty(loginString) || string.IsNullOrEmpty(passwordString))
		{
			//TODO: Give Error
			return;
		}
		if(NetworkManager.Network.Login(loginString, passwordString))
		{
			SceneManager.LoadScene("main");
		}
	}

	public void Register()
	{
	}
}
