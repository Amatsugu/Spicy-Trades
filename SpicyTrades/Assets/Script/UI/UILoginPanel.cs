using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NetworkManager;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UILoginPanel : UIPanel
{
	public TMP_InputField login;
	public TMP_InputField password;

	public UIRegisterPanel registerPanel;

	private void Start()
	{
		login.text = PlayerPrefs.GetString("user", "");
		password.text = PlayerPrefs.GetString("pass", "");
	}

	public void Login()
	{
		Debug.Log("Logging in...");
		var loginString = login.text;
		var passwordString = password.text;
		if(string.IsNullOrEmpty(loginString) || string.IsNullOrEmpty(passwordString))
		{
			Debug.LogWarning("Empty Login");
			return;
		}
		try
		{
			if(SpicyNetwork.Login(loginString, passwordString))
			{
				PlayerPrefs.SetString("user", loginString);
				PlayerPrefs.SetString("pass", passwordString);
				Debug.Log("Joining Room...");
				var joined = false;
				var rooms = SpicyNetwork.ListRooms();
                if(rooms!=null)
				    foreach (var room in rooms)
				    {
					    if (room.GetNumPlayers() < 4)
					    {
						    joined = SpicyNetwork.JoinRoom(room.GetRoomID()) != null;
						    break;
					    }
				    }
				if(!joined)
					joined = SpicyNetwork.CreateRoom() != null;

				if(!joined)
				{
					Debug.LogError("Failed to join or create room");
					return;
				}
				//GameMaster.Offline = true; //Offline Killswitch
				SceneManager.LoadScene("main");
				return;
			}
		}catch(Exception e)
		{
			GameMaster.Offline = true;
			Debug.LogWarning("Something Went wrong, switching to offline mode!");
			Debug.LogWarning($"{e.GetType().Name}:{e.Message}\n{e.StackTrace}");
			SceneManager.LoadScene("main");
			return;
		}
		Debug.LogError("Failed to login");
	}

	public void Register()
	{
		registerPanel.Show();
		Hide();
	}
}
