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
			Debug.LogWarning("Empty Login");
			return;
		}
		if(SpicyNetwork.Login(loginString, passwordString))
		{
			var joined = false;
			var rooms = SpicyNetwork.ListRooms();
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
			}
			SceneManager.LoadScene("main");
			return;
		}
		Debug.LogError("Failed to login");
	}

	public void Register()
	{
	}
}
