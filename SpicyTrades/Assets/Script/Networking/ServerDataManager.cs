using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Npgsql;
using System;

namespace NetworkManager
{			
	public class CID
	{
		private PID player;
		private IPEndPoint connection;
		private Room currentRoom;

		public CID (PID self, IPEndPoint conn)
		{
			player = self;
			connection = conn;
		}

		public PID GetPID ()
		{
			return player;
		}

		public IPEndPoint GetConn ()
		{
			return connection;
		}

		public void SetCurrentRoom (Room room)
		{
			currentRoom = room;
		}

		public Room GetCurrentRoom ()
		{
			return currentRoom;
		}
	}
	public class ENV
	{
		public IPEndPoint end;
		public byte[] message;
		public string self;
		public int count = 0;
		public ENV(IPEndPoint e, byte[] msg,string self){
			end = e;
			this.self = self;
			message = msg;
		}
		public IPEndPoint GetEndPoint(){
			return end;
		}
		public byte[] GetMessage(){
			count++;
			if (count > 5) {
				return null;
			} else {
				return message;
			}
		}
		public string GetSession(){
			return self;
		}
	}
	public static class ServerDataManager
	{
		//Unlike the client data will live in this class
		private static Dictionary<string, Room> Rooms = new Dictionary<string, Room> ();
		private static Dictionary<string, PID> PIDs = new Dictionary<string, PID> ();
		public static Dictionary<string, CID> Connections = new Dictionary<string, CID> ();
		public static Dictionary<string, ENV> LastMessages = new Dictionary<string, ENV> ();
		public static Dictionary<IPEndPoint, string> END2SESSION = new Dictionary<IPEndPoint, string> ();
		// the string here is the users session key
		public static Dictionary<string, string> pidToClient = new Dictionary<string, string> ();
		private static uint ROOMID = 0;
		// when converted to hex we get our 8byte id
		private static uint PLAYERID = 0;
		// when converted to hex we get our 8byte id
		public static void INIT ()
		{
			SpicyNetwork.DataRecieved += OnDataRecieved;
			Thread t = new Thread(new ThreadStart(ThreadProc));
			t.Start();
		}
		public static string GenRoomID ()
		{
			ROOMID++;
			return ROOMID.ToString ("X8");
		}

		public static void OnDataRecieved (object sender, DataRecievedArgs e)
		{
			byte command = e.RawResponse [0];
			byte[] data = e.RawResponse.SubArray (1, e.RawResponse.Length - 1);
			IPEndPoint send = e.SenderRef;
			object[] objects;
			PID player;
			string pid;
			string self;
			Message msg;
			string roomid;
			string playerid;
			byte[] dat;
			string id = "";
			Room temproom;
			object[] datO;
			switch (command) {
			case SpicyNetwork.HELLO:
				objects = NetUtils.FormCommand (data, new string[] { "s" });
				self = (string)objects [0];
				Console.WriteLine ("Got a hello from the client!");
                    //Handle data for this
				break;
			case SpicyNetwork.LOGOUT:
				try{
					self = (string)(NetUtils.FormCommand (data, new string[] { "s" }) [0]);
					Logout (self,send);
				} catch {
					Console.WriteLine ("Logout error... Looking up player from IPEndPoint!");
					Logout (send);
				}
				break;
			case SpicyNetwork.LOGIN:
				objects = NetUtils.FormCommand (data, new string[] { "s", "s" });
				string username = (string)objects [0];
				string password = (string)objects [1];
//				if (username == "epicknex" && password == "password") {
//					id = "12345678";
//					Console.WriteLine ("¯\\_(ツ)_/¯ you logged in nice!");
//				} else if (username == "testacc" && password == "password") {
//					id = "87654321";
//					Console.WriteLine ("¯\\_(ツ)_/¯ you logged in nice!");
//				} else {
//					Console.WriteLine ("¯\\_(ツ)_/¯ you done goof!");
//					SendError (command, SpicyNetwork.INVALID_USERPASS, "Invalid Username or Password", send);
//					break;
//				}
				PID tempuserLogin = Login(username, password);
				id = tempuserLogin.GetID();
				if (pidToClient.ContainsKey (id) && PIDs.ContainsKey (id)) {
					Console.WriteLine ("¯\\_(ツ)_/¯ Dup login detected...!");
					SpicyNetwork.SendData (NetUtils.PieceCommand (new object[] {
						command,
						SpicyNetwork.NO_ERROR,
						PIDs [id],
						pidToClient [id]
					}), send);
					break;
				} else {
					try {
						PIDs.Remove(id);
						pidToClient.Remove(id);
					} catch {
						//
					}
				}
		        //END OF FAKE STUFF
				string sessionkey = GenUniqueSessionKey ();
				END2SESSION[send]=sessionkey;
				PID tempPID = new PID (id, username, false);
				PIDs [id] = tempPID;
                    //Need to get the players name... You are not a friend of yourself are you?
				Connections [sessionkey] = new CID (tempPID, send);
				pidToClient[id] = sessionkey;
				Console.WriteLine ("Sending data");
				SpicyNetwork.SendData (NetUtils.PieceCommand (new object[] {
					command,
					SpicyNetwork.NO_ERROR,
					tempPID,
					sessionkey
				}), send);
				break;
			case SpicyNetwork.REGISTER:
				objects = NetUtils.FormCommand (data, new string[] { "s", "s", "s", "s" });//user pass, email
				string reg_username = (string)objects [1];
				string reg_password = (string)objects [2];
				string reg_email = (string)objects [0];
				try {
					Register(reg_username, reg_password, reg_email);
					SpicyNetwork.SendData (NetUtils.PieceCommand (new object[] { command, SpicyNetwork.NO_ERROR}), send);
				} catch {
					SpicyNetwork.SendData (NetUtils.PieceCommand (new object[] { command, SpicyNetwork.INVALID_USERPASS,"Cannot create an account with those credentials!"}), send);
				}
				break;
			case SpicyNetwork.UPDATES:
                    //NOT IMPLEMENTED IN THIS VERSION
				break;
			case SpicyNetwork.DOUPDATES:
                    //NOT IMPLEMENTED IN THIS VERSION
				break;
			case SpicyNetwork.LISTR: //Gets the roomids
				Console.WriteLine ("List Data");
				objects = NetUtils.FormCommand (data, new string[] { "s", "i", "i" });
				self = (string)objects [0];
				if (!PlayerLoggedIn (self)) {
					Console.WriteLine ("¯\\_(ツ)_/¯ you done goof!");
					SendError (command, SpicyNetwork.INVALID_CID, "Your session key is invalid!", send);
					break;
				}
				int pos = (int)objects [1];
				int rcount = Rooms.Count;
				string[] roomlist = Rooms.Keys.ToArray ();
				for (int i=0;i<roomlist.Length;i++) {
					roomlist [i] = roomlist[i];
				}
				SpicyNetwork.SendData (NetUtils.PieceCommand (new object[] { command, SpicyNetwork.NO_ERROR, roomlist }), send);
				break;
			case SpicyNetwork.JROOM:
				Console.WriteLine ("Attempting to join a room");
				objects = NetUtils.FormCommand (data, new string[] { "s", "s" });
				roomid = (string)objects [1];
				self = (string)objects [0];
				if (!PlayerLoggedIn (self)) {
					Console.WriteLine ("¯\\_(ツ)_/¯ you done goof!");
					SendError (command, SpicyNetwork.INVALID_CID, "Your session key is invalid!", send);
					break;
				}
				if (InARoom (self)) {
					SendError (command, SpicyNetwork.CANT_JOIN_ROOM, "Cannot join a room while you are within one!", send);
					break;
				}
				if (Rooms.TryGetValue (roomid, out temproom)) {
					if (RoomExists (roomid) && Rooms [roomid].GetNumPlayers () != Room.MAX_MEMBERS) {
						player = GetPlayerPID (self);
						temproom.AddMember (player, true);
						GetPlayerCID (self).SetCurrentRoom (temproom);
						SpicyNetwork.SendData (NetUtils.PieceCommand (new object[] { command, SpicyNetwork.NO_ERROR, roomid }), send);
						datO = new object[] {
							SpicyNetwork.JOINO,
							SpicyNetwork.NO_ERROR,
							player.GetID ()
						};
						SendToRoom(datO,roomid,self);
					} else {
						SendError (command, SpicyNetwork.CANT_JOIN_ROOM, "Cannot join Room: Room full!", send);
					}
				} else {
					SendError (command, SpicyNetwork.CANT_JOIN_ROOM, "Attempt to join a non existing room!", send);
				}

				Console.WriteLine ("Room joined!");
				break;
			case SpicyNetwork.CHAT:
				Console.WriteLine ("Chat Data");
				objects = NetUtils.FormCommand (data, new string[] { "s", "m" });
				self = (string)objects [0];
				if (!PlayerLoggedIn (self)) {
					Console.WriteLine ("¯\\_(ツ)_/¯ you done goof!");
					SendError (command, SpicyNetwork.INVALID_CID, "Your session key is invalid!", send);
					break;
				}
				msg = (Message)objects [1];
				datO = new object[] { command, SpicyNetwork.NO_ERROR, msg };
				SendToAll (datO,self);
				Console.WriteLine ("Chat Data!");
				break;
			case SpicyNetwork.REQUEST:
                    //self, playerid 
				Console.WriteLine ("Request Data");
				objects = NetUtils.FormCommand (data, new string[] { "s", "s" });
				self = (string)objects [0];
				if (!PlayerLoggedIn (self)) {
					Console.WriteLine ("¯\\_(ツ)_/¯ you done goof!");
					SendError (command, SpicyNetwork.INVALID_CID, "Your session key is invalid!", send);
					break;
				}
				playerid = (string)objects [1];
				SpicyNetwork.SendData (NetUtils.PieceCommand (new object[] {
					command,
					SpicyNetwork.NO_ERROR,
					GetPlayerPID (self).GetID ()
				}),send);
				Console.WriteLine ("Sent Request");
                    //TODO FINISH THIS! This needs the database, but will alert the user if online
				break;
			case SpicyNetwork.LISTF:
                    //self
				objects = NetUtils.FormCommand (data, new string[] { "s" });
                    //TODO FINISH THIS! Needs database
				break;
			case SpicyNetwork.LISTRF:
                    //self
				objects = NetUtils.FormCommand (data, new string[] { "s" });
                    //TODO FINISH THIS! Needs database
				break;
			case SpicyNetwork.ADDF:
                    //self, playerid
				objects = NetUtils.FormCommand (data, new string[] { "s", "s" });
                    //TODO FINISH THIS! Needs database
				break;
			case SpicyNetwork.FORMR:
                    //self, roompassword
				Console.WriteLine ("Creating room!");
				objects = NetUtils.FormCommand (data, new string[] { "s", "s" });
				self = (string)objects [0];
				if (!PlayerLoggedIn (self)) {
					Console.WriteLine ("¯\\_(ツ)_/¯ you done goof!");
					SendError (command, SpicyNetwork.INVALID_CID, "Your session key is invalid!", send);
					break;
				}
				if (InARoom (self)) {
					SpicyNetwork.SendData (NetUtils.PieceCommand (new object[] { command, SpicyNetwork.NO_ERROR, GetRoomID(self)}), send);
					break;
				}
				roomid = GenRoomID ();
				if (Rooms.ContainsKey (roomid)) {
					SendError (command, SpicyNetwork.ROOM_ERROR, "Cannot form a room that already exists!", send);
					break;
				}
				string pass = (string)objects [1];
				temproom = new Room (roomid, pass, true);
				Rooms.Add (roomid, temproom);// Server is always a "host"
				player = GetPlayerPID (self);
				temproom.AddMember (player, true);
				GetPlayerCID (self).SetCurrentRoom (temproom);
				Console.WriteLine ("Sending Room Data");
				SpicyNetwork.SendData (NetUtils.PieceCommand (new object[] { command, SpicyNetwork.NO_ERROR, temproom.GetRoomID()}), send);
				//SpicyNetwork.SendData (NetUtils.PieceCommand (new object[] { SpicyNetwork.GROOM, SpicyNetwork.NO_ERROR, temproom }), send);//Send a copy of the room data to the player
				Console.WriteLine ("Room Data Sent!");
				break;
			case SpicyNetwork.IHOST:
				Console.WriteLine ("Host Data");
				objects = NetUtils.FormCommand (data, new string[] { "s" });
				self = (string)objects [0];
				if (!PlayerLoggedIn (self)) {
					Console.WriteLine ("¯\\_(ツ)_/¯ you done goof!");
					SendError (command, SpicyNetwork.INVALID_CID, "Your session key is invalid!", send);
					break;
				}
				if (!InARoom (self)) {
					SendError (command, SpicyNetwork.ROOM_ERROR, "Cannot be a host of a room when you are not in one!", send);
					break;
				}
				roomid = GetRoomID (self);
				if (Rooms [roomid].GetHost ().GetID () == GetPlayerPID (self).GetID ()) {
					SpicyNetwork.SendData (NetUtils.PieceCommand (new object[] { command, SpicyNetwork.NO_ERROR, true }), send);
				} else {
					SpicyNetwork.SendData (NetUtils.PieceCommand (new object[] { command, SpicyNetwork.NO_ERROR, false }), send);
				}
				Console.WriteLine ("Sent Host data");
				break;
			case SpicyNetwork.KICK:
                    //self, playerid
				Console.WriteLine ("Kicking player");
				objects = NetUtils.FormCommand (data, new string[] { "s", "s"});
				self = (string)objects [0];
				if (!PlayerLoggedIn (self)) {
					Console.WriteLine ("¯\\_(ツ)_/¯ you done goof!");
					SendError (command, SpicyNetwork.INVALID_CID, "Your session key is invalid!", send);
					break;
				}
				if (!InARoom (self)) {
					SendError (command, SpicyNetwork.ROOM_ERROR, "Cannot Kick someone when you are not in a room!", send);
					break;
				}
				roomid = GetRoomID (self);
				if (Rooms [roomid].GetHost ().GetID () != GetPlayerPID (self).GetID ()) {
					SendError (command, SpicyNetwork.ROOM_ERROR, "Cannot Kick someone from a room if you are not the host!", send);
					break;
				}
				playerid = (string)objects [2];
				Rooms [roomid].Kick (PIDs [playerid]);
				dat = NetUtils.PieceCommand (new object[] { command, SpicyNetwork.NO_ERROR, roomid, playerid });
				SendToRoom (dat, roomid);
				Console.WriteLine ("Kicked");
				break;
			case SpicyNetwork.INVITEF:
                    //self, playerid
				Console.WriteLine ("Inviting player");
				objects = NetUtils.FormCommand (data, new string[] { "s", "s"});
				self = (string)objects [0];
				if (!PlayerLoggedIn (self)) {
					Console.WriteLine ("¯\\_(ツ)_/¯ you done goof!");
					SendError (command, SpicyNetwork.INVALID_CID, "Your session key is invalid!", send);
					break;
				}
				if (!InARoom (self)) {
					SendError (command, SpicyNetwork.ROOM_ERROR, "Cannot invite a friend to a room while you are not in one!", send);
					break;
				}
				playerid = (string)objects [1];
				roomid = GetRoomID (self);
				if (PlayerExists (playerid) && RoomExists (roomid) && ISOnfriendList (self, playerid)) {
					SendToPlayer (NetUtils.PieceCommand (new object[] {
						command,
						SpicyNetwork.NO_ERROR,
						playerid,
						roomid
					}), playerid);
				} else {
					SpicyNetwork.SendData (NetUtils.PieceCommand (new object[] { }), send);
					SendError (command, SpicyNetwork.INVALID_FID, "Invalid playerid!", send);
					break;
				}
				Console.WriteLine ("Invite sent");
				break;
			case SpicyNetwork.READY:
                    //self, isready. roomid
				Console.WriteLine ("Got ready");
				objects = NetUtils.FormCommand (data, new string[] { "s", "bo" });
				self = (string)objects [0];
				if (!PlayerLoggedIn (self)) {
					Console.WriteLine ("¯\\_(ツ)_/¯ you done goof!");
					SendError (command, SpicyNetwork.INVALID_CID, "Your session key is invalid!", send);
					break;
				}
				if (!InARoom (self)) {
					SendError (command, SpicyNetwork.ROOM_ERROR, "Attempt to set room ready when you are not in a room!", send);
					break;
				}
				bool isReady = (bool)objects [1];
				roomid = GetRoomID (self);
				Rooms [roomid].SetReady (isReady, GetPlayerPID (self));
				datO = new object[] {
					SpicyNetwork.READYO,
					SpicyNetwork.NO_ERROR,
					GetPlayerPID (self).GetID (),
					isReady,
				};
				SendToRoom (datO, roomid,self);
				SpicyNetwork.SendData (NetUtils.PieceCommand (new object[] {
					SpicyNetwork.READY,
					SpicyNetwork.NO_ERROR,
				}), send);
				Console.WriteLine ("Sending ready");
				break;
			case SpicyNetwork.LEAVER: // only sent if you are in the room
                    //self, roomid
				Console.WriteLine ("Leaving room");
				objects = NetUtils.FormCommand (data, new string[] { "s" });
				self = (string)objects [0];
				if (!PlayerLoggedIn (self)) {
					Console.WriteLine ("¯\\_(ツ)_/¯ you done goof!");
					SendError (command, SpicyNetwork.INVALID_CID, "Your session key is invalid!", send);
					break;
				}
				if (!InARoom (self)) {
					SendError (command, SpicyNetwork.ROOM_ERROR, "Attempt to leave a room when you are not in one!", send);
					break;
				}
				roomid = GetRoomID (self);
				if (!Rooms.ContainsKey (roomid)) {
					SendError (command, SpicyNetwork.ROOM_ERROR, "Attempt to leave a room that nolonger exists!", send);
					GetPlayerCID (self).SetCurrentRoom (null);
					break;
				}
				GetPlayerCID (self).SetCurrentRoom (null);
				Rooms [roomid].RemoveMember (GetPlayerPID (self));
				if (Rooms [roomid].GetNumPlayers () == 0) {
					Rooms.Remove (roomid);
					break;
				}
				dat = NetUtils.PieceCommand (new object[] {
					SpicyNetwork.LEAVERO,
					SpicyNetwork.NO_ERROR,
					GetPlayerPID (self).GetID ()
				});
				SendToRoom (dat, roomid);
				SpicyNetwork.SendData (NetUtils.PieceCommand (new object[] {
					SpicyNetwork.LEAVER,
					SpicyNetwork.NO_ERROR,
					GetPlayerPID (self).GetID ()
				}), send);
				Console.WriteLine ("Room left");
				break;
			case SpicyNetwork.INIT:
				self = (string)NetUtils.FormCommand (data, new string[] { "s" }) [0];
				if (!PlayerLoggedIn (self)) {
					Console.WriteLine ("¯\\_(ツ)_/¯ you done goof!");
					SendError (command, SpicyNetwork.INVALID_CID, "Your session key is invalid!", send);
					break;
				}
				GetPlayerPID (self).SetConnection (send); // Allows for faster player lookup
				SpicyNetwork.SendData (NetUtils.PieceCommand (new object[] { command, SpicyNetwork.NO_ERROR }), send);
				break;
			case SpicyNetwork.CHATDM:
                    //self, msg, playerid
				Console.WriteLine ("ChatDM Data");
				objects = NetUtils.FormCommand (data, new string[] { "s", "m", "s" });
				self = (string)objects [0];
				if (!PlayerLoggedIn (self)) {
					Console.WriteLine ("¯\\_(ツ)_/¯ you done goof!");
					SendError (command, SpicyNetwork.INVALID_CID, "Your session key is invalid!", send);
					break;
				}
				msg = (Message)objects [1];
				playerid = (string)objects [2];
				datO = new object[] { command, SpicyNetwork.NO_ERROR, msg };
				SendToPlayer (datO, playerid,self);
				Console.WriteLine ("Sent Chat Data");
				break;
			case SpicyNetwork.CHATRM:
                    //self, msg, roomid
				Console.WriteLine ("ChatRM Data");
				objects = NetUtils.FormCommand (data, new string[] { "s", "m"});
				self = (string)objects [0];
				if (!PlayerLoggedIn (self)) {
					Console.WriteLine ("¯\\_(ツ)_/¯ you done goof!");
					SendError (command, SpicyNetwork.INVALID_CID, "Your session key is invalid!", send);
					break;
				}
				if (!InARoom (self)) {
					SendError (command, SpicyNetwork.ROOM_ERROR, "Cannot send a message to a room when you are not in a room!", send);
					break;
				}
				msg = (Message)objects [1];
				roomid = GetPlayerCID (self).GetCurrentRoom ().GetRoomID ();
				datO = new object[] { command, SpicyNetwork.NO_ERROR, msg };
				SendToRoom (datO, roomid,self);
				Console.WriteLine ("Sent Chat Data");
				break;
			case SpicyNetwork.ROOMS:
				Console.WriteLine ("Room List");
				objects = NetUtils.FormCommand (data, new string[] { "s" });
				self = (string)objects [0];
				if (!PlayerLoggedIn (self)) {
					Console.WriteLine ("¯\\_(ツ)_/¯ you done goof!");
					SendError (command, SpicyNetwork.ROOMS, "Your session key is invalid!", send);
					break;
				}
				SpicyNetwork.SendData (NetUtils.PieceCommand (new object[] { command, SpicyNetwork.NO_ERROR, Rooms.Count }), send);
				Console.WriteLine ("Sending room list");
				break;
			case SpicyNetwork.GROOM:
				objects = NetUtils.FormCommand (data, new string[] { "s", "s" });
				self = (string)objects [0];
				if (!PlayerLoggedIn (self)) {
					Console.WriteLine ("¯\\_(ツ)_/¯ you done goof!");
					SendError (command, SpicyNetwork.INVALID_CID, "Your session key is invalid!", send);
					break;
				}
				if(!Rooms.ContainsKey((string)objects [1])){
					SendError (command, SpicyNetwork.UNKNOWN_ERROR, "Your session key is invalid!", send);
					break;
				}
				SpicyNetwork.SendData (NetUtils.PieceCommand (new object[] { command, SpicyNetwork.NO_ERROR, Rooms [(string)objects [1]] }), send);
				Console.WriteLine ("Sending room data");
				break;
			case SpicyNetwork.GPID:
				Console.WriteLine ("Get Request!");
				objects = NetUtils.FormCommand (data, new string[] { "s", "s" });
				self = (string)objects [0];
				if (!PlayerLoggedIn (self)) {
					Console.WriteLine ("¯\\_(ツ)_/¯ you done goof!");
					SendError (command, SpicyNetwork.INVALID_CID, "Your session key is invalid!", send);
					break;
				}
				string key = (string)objects [1];
				if (PIDs.ContainsKey (key)) {
					Console.WriteLine ("Sending PID data");
					SpicyNetwork.SendData (NetUtils.PieceCommand (new object[] { command, SpicyNetwork.NO_ERROR, PIDs [key] }), send);
				} else {
					Console.WriteLine ("NO PID" + key);
					SendError (command, SpicyNetwork.INVALID_UID, "Cannot retreive pid data", send);
				}
				Console.WriteLine ("Sending pid data");
				break;

			case SpicyNetwork.SYNC:
                    //self, randomid, groups.Length, i, groups[i] 
				bool isBytes;
				if (data [0] == 0) {
					objects = NetUtils.FormCommand (data, new string[] { "bo", "s", "s"});
				} else {
					objects = NetUtils.FormCommand (data, new string[] { "bo", "b[]", "s"});
				}
				isBytes = (bool)objects [0];
				self = (string)objects [1];
				if (!PlayerLoggedIn (self)) {
					Console.WriteLine ("¯\\_(ツ)_/¯ you done goof!");
					SendError (command, SpicyNetwork.INVALID_CID, "Your session key is invalid!", send);
					break;
				}
				string payload = (string)objects [2];
				if (isBytes) {
					datO = new object[] { SpicyNetwork.SYNCB, SpicyNetwork.NO_ERROR, payload };
				} else {
					datO = new object[] { SpicyNetwork.SYNCO, SpicyNetwork.NO_ERROR, payload };
				}
				SendToRoom (datO, GetRoomID(self),self);// remove the self tag and send the data to the clients
				break;
			case SpicyNetwork.RELAY:
				objects = NetUtils.FormCommand (data, new string[] { "s", "s"});
				self = (string)objects [0];
				if (!PlayerLoggedIn (self)) {
					Console.WriteLine ("¯\\_(ツ)_/¯ you done goof!");
					SendError (command, SpicyNetwork.INVALID_CID, "Your session key is invalid!", send);
					break;
				}
				LastMessages.Remove((string)objects[1]);
				break;
			default:
				Console.WriteLine ("Unknown command!");
				break;
			}
		}
		public static bool PlayerLoggedIn(string cid){
			return Connections.ContainsKey(cid);
		}
		public static string GetRoomID(string cid){
			return GetPlayerCID (cid).GetCurrentRoom ().GetRoomID ();
		}
		public static bool InARoom(string cid){
			return GetPlayerCID (cid).GetCurrentRoom () != null;
		}
		public static PID GetPlayerPID (string cid)
		{
			return Connections [cid].GetPID (); //Get the playerid from self(the cid)
		}
		public static CID GetPlayerCID (string cid)
		{
			return Connections [cid]; //Get the playerid from self(the cid)
		}
		public static bool RoomExists (string roomid0)
		{
			return Rooms [roomid0] != null;
		}

		public static bool PlayerExists (string roomid0)
		{
			return PIDs.ContainsKey(roomid0);
		}

		public static bool ISOnfriendList (string cid, string player)
		{
			// Need the database to be done
			return true; // FIX THIS!
		}
		public static void SendDataEncsured(object[] data, IPEndPoint send,string self){
			string key = GenUniqueSessionKey ().Substring(0,8);
			object[] tempARR = new object[data.Length + 1];
			for (int i=0; i<tempARR.Length-1;i++){
				tempARR [i] = data [i];
			}
			tempARR [tempARR.Length - 1] = key;
			byte[] tempb = NetUtils.PieceCommand (tempARR);
			LastMessages.Add (key, new ENV (send, tempb, self));
			SpicyNetwork.SendData (tempb, send);
		}
		public static void ResendData(){
			//Get KeyValues of all messages and resend all that exist
			ENV[] list = LastMessages.Values.ToArray();
			byte[] temp;
			for(int i=0;i<list.Length;i++){
				temp = list [i].message;
				if(temp==null){
					Logout (list [i].end);
					continue;
				}
				SpicyNetwork.SendData (temp,list [i].end);
			}
		}
		public static void Logout(string self,IPEndPoint send){
			try {
				if (!PlayerLoggedIn (self)) {
					Console.WriteLine ("¯\\_(ツ)_/¯ you done goof!");
					SendError (SpicyNetwork.LOGOUT, SpicyNetwork.INVALID_CID, "Your session key is invalid!", send);
					return;
				}

				string pid = GetPlayerPID (self).GetID ();
				Connections.Remove(self);
				PIDs.Remove(pid);
				pidToClient.Remove(pid);
				SpicyNetwork.SendData(NetUtils.PieceCommand(new object[] { SpicyNetwork.LOGOUT, SpicyNetwork.NO_ERROR }), send);
			} catch {
				Console.WriteLine ("User already logged out!");
				SpicyNetwork.SendData(NetUtils.PieceCommand(new object[] { SpicyNetwork.LOGOUT, SpicyNetwork.NO_ERROR }), send);
			}
		}
		public static void Logout(IPEndPoint send){
			string session = END2SESSION [send];
			Logout (session, send);
		}
		public static void ThreadProc(){
			while(true){
				Thread.Sleep (100);
				ResendData ();
			}
		}
		public static void SendToAll (byte[] data)
		{
			foreach (var kvp in Connections.ToArray()) {
				SpicyNetwork.SendData (data, kvp.Value.GetConn ());
			}
		}

		public static void SendToRoom (byte[] data, string roomid)
		{
			PID[] pids = Rooms [roomid].GetMembers ();
			for (int i = 0; i < pids.Length; i++) {
				if (pids [i] != null) {
					SpicyNetwork.SendData (data, pids [i].GetConnection ());
				}
			}
		}
		public static void SendToPlayer (byte[] data, string playerid)
		{
			if(PlayerExists(playerid))
				SpicyNetwork.SendData (data, PIDs [playerid].GetConnection ());
		}
		public static void SendToAll (object[] data,string self)
		{
			foreach (var kvp in Connections.ToArray()) {
				SendDataEncsured (data, kvp.Value.GetConn (),self);
			}
		}

		public static void SendToRoom (object[] data, string roomid,string self)
		{
			PID[] pids = Rooms [roomid].GetMembers ();
			for (int i = 0; i < pids.Length; i++) {
				if (pids [i] != null) {
					Console.WriteLine("Sending To: "+pids [i].GetName());
					SendDataEncsured (data, pids [i].GetConnection (),self);
				}
			}
		}
		public static void SendToPlayer (object[] data, string playerid,string self)
		{
			if(PlayerExists(playerid))
				SendDataEncsured (data, PIDs [playerid].GetConnection (),self);
		}
		public static string GenUniqueSessionKey ()
		{
			return Guid.NewGuid ().ToString ("N");
		}

		public static void SendError (byte cmd, byte err, string msg, IPEndPoint send)
		{
			Console.WriteLine ("Error: "+msg);
			SpicyNetwork.SendData (NetUtils.PieceCommand (new object[] { cmd, err, msg }), send);
		}

		public const string CONN_STRING = "Host=localhost;Username=mylogin;Password=mypass;Database=public";
		public static PID Login(string email1, string password1)
		{
			using (var conn = new NpgsqlConnection(CONN_STRING))
			{
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = $"SELECT password WHERE email = {Uri.EscapeDataString(email1)}";
					string passwordHash = (string)cmd.ExecuteScalar();
					if (!VerifyPassword(password1, passwordHash))
						return null;

					cmd.CommandText = $"SELECT username, email  WHERE password = {passwordHash}";
					using (var reader = cmd.ExecuteReader())
					{
						return new PID(Uri.UnescapeDataString(reader.GetString(0)), reader.GetString(1),false);

					}

				}

			}
		}

		public static bool VerifyPassword(string password, string passwordHash)
		{
			if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
				return false;

			byte[] hashBytes = Convert.FromBase64String(passwordHash);

			byte[] salt = new byte[16];
			Array.Copy(hashBytes, 0, salt, 0, 16);

			var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
			byte[] hash = pbkdf2.GetBytes(20);

			for (int i = 0; i < 20; i++)
				if (hashBytes[i + 16] != hash[i])
					return false;
			return true;
		}
		internal static bool CheckEmailExists(string email)
		{
			using (var con = new NpgsqlConnection(CONN_STRING))
			{
				using (var _cmd = con.CreateCommand())
				{
					_cmd.CommandText = $"SELECT email FROM players WHERE email = '{Uri.EscapeDataString(email)}'";
					return (_cmd.ExecuteReader().HasRows);
				}
			}
		}



		internal static bool CheckFriendExists(string username)
		{
			using (var con = new NpgsqlConnection(CONN_STRING))
			{
				using (var _cmd = con.CreateCommand())
				{
					_cmd.CommandText = $"SELECT email FROM players WHERE username = '{Uri.EscapeDataString(username)}'";
					return _cmd.ExecuteReader().HasRows;
				}
			}
		}
			
		private static bool Register(string username1, string password1, string email1)
		{
			try
			{  
				using (var conn = new NpgsqlConnection(CONN_STRING))
				{

					if (CheckEmailExists(email1))
						conn.Open();
					using (var cmd = conn.CreateCommand())
					{
						cmd.CommandText = $"INSERT INTO players(player_ID,username,email,password) VALUES ( default, '{Uri.EscapeDataString(username1)}' , '{HashPassword(password1)}' , '{Uri.EscapeDataString(email1)}')";
						cmd.ExecuteNonQuery();
						return true;
					}
				}
			}
			catch (Exception e)
			{
				return false;
			}

		}
		private static string HashPassword(string password)
		{
			byte[] salt;
			new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

			var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
			byte[] hash = pbkdf2.GetBytes(20);

			byte[] hashBytes = new byte[36];
			Array.Copy(salt, 0, hashBytes, 0, 16);
			Array.Copy(hash, 0, hashBytes, 16, 20);

			return Convert.ToBase64String(hashBytes);
		}
		private static bool AddFriend (string selfusername, string username1)
		{
			try
			{
				using (var conn = new NpgsqlConnection(CONN_STRING))
				{

					if (CheckFriendExists(username1))

						conn.Open();
					using (var cmd = conn.CreateCommand())
					{
						cmd.CommandText = $"INSERT INTO FriendsList VALUES (default, '{Uri.EscapeDataString(username1) }'), ({ Uri.EscapeDataString(selfusername )}";
						cmd.ExecuteNonQuery();
						return true;
					}
				}
			}
			catch (Exception e)
			{
				return false;
			}
		}
	}
}
