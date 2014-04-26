using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using System.Xml;

public class Server : MonoBehaviour
{
	private string currentForm = "initialize";
	public GameObject networkObjectMaster;
	public GameObject networkObject;
	public Vector2 scrollPosition;
	public int networkversion;
	private string initLog = "Initializing...";
	public int oxygen = 100;

	public GameObject spawnPoint;

	// max online players determined by above and initialized in start
	int maxonlineplayers = 50;
	
	
	private Dictionary<string, User> dbUsers = new Dictionary<string, User>();
	private Dictionary<string, Task> dbTasks = new Dictionary<string, Task>();
	private Dictionary<string,string> playerIdList = new Dictionary<string, string>();
	float timer = 0; // set duration time in seconds in the Inspector
	float ticklength = 10;
	

	// Use this for initialization
	void Start()
	{
		InitializeServer();
	}
	

	void OnGUI()
	{
		if (currentForm.Equals("initialize"))
		{
			GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
			//GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			//GUILayout.FlexibleSpace();
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height));

			GUILayout.Label(initLog);
			GUILayout.EndScrollView();
			
			
			//GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			//GUILayout.FlexibleSpace();
			GUILayout.EndArea();
			
		}
	}
	

	
	// Update is called once per frame
	void Update()
	{
		if (timer > 0)
		{
			timer -= Time.deltaTime;
		}
		if (timer <= 0)
		{
			// tick 
			//setInitLog("A universal tick has occured");
			processTick();
			timer = ticklength;
		}
	}

	
	void processTick()
	{
		ArrayList taskstoremove = new ArrayList();

		foreach (KeyValuePair<string, Task> entry in dbTasks)
		{
			Task task = entry.Value;
			if (task.getTimeleft() == 0)
			{
				processTask(task);
				taskstoremove.Add(entry.Key);
				
				
			} else
			{
				task.setTimeleft(task.getTimeleft() - 1);
			}
			
		}
		
		int count = 0;
		foreach (string taskname in taskstoremove)
		{
			dbTasks.Remove(taskname);
			count++;
		}
		//setInitLog(count + " tasks processed and removed");
		//syncClientsCache();
		
	}

	void syncClientsCache()
	{
		foreach (NetworkPlayer np in Network.connections) 
		{
			if (playerIdList.ContainsKey (np.guid))
			{
				//todo
				string usercache = "";
				string scenecache = "";
				//SendCmd (np, "synccache", usercache,scenecache);
			}
		}
	}

	void processTask(Task task)
	{
		switch (task.getTaskname())
		{

			default:
				break;
		}
		
	}
	
	string getBFStringFromUser(User obj)
	{
		MemoryStream ms = new MemoryStream();
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(ms, obj); //Save the list
		return System.Convert.ToBase64String(ms.GetBuffer());
	}
	
	User getUserFromBFString(string str)
	{
		MemoryStream ms = new MemoryStream(System.Convert.FromBase64String(str)); //Create an input stream from the string
		BinaryFormatter bf = new BinaryFormatter();
		User obj = (User)bf.Deserialize(ms);
		return obj;
	}
	
	string getBFStringFromList(ArrayList list)
	{
		MemoryStream ms = new MemoryStream();
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(ms, list); //Save the list
		return System.Convert.ToBase64String(ms.GetBuffer());
	}
	
	ArrayList getListFromBFString(string data)
	{
		MemoryStream ms = new MemoryStream(System.Convert.FromBase64String(data)); //Create an input stream from the string
		BinaryFormatter bf = new BinaryFormatter();
		ArrayList list = (ArrayList)bf.Deserialize(ms);
		return list;
	}
	
	public void WriteToXml()
	{
		//setInitLog("Saving...");
		string filepath = Application.dataPath + @"/users.xml";
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(filepath);
		XmlElement elmRoot = xmlDoc.DocumentElement;
		elmRoot.RemoveAll(); // remove all inside
		
		XmlElement elmUsers = xmlDoc.CreateElement("users"); 
		foreach (KeyValuePair<string, User> entry in dbUsers)
		{
			User user = entry.Value;
			XmlElement elmUser = xmlDoc.CreateElement("user"); // create the x node.
			elmUser.SetAttribute("username", user.getUsername());
			//setInitLog("Saving " + user.getUsername());
			elmUser.SetAttribute("password", user.getPassword());
			elmUser.SetAttribute("credits", user.getCredits().ToString ());
			elmUser.SetAttribute("x", user.getX().ToString ());
			elmUser.SetAttribute("y", user.getY().ToString ());
			elmUser.SetAttribute("z", user.getZ().ToString ());
			elmUser.SetAttribute("s", user.getS().ToString ());
			elmUser.SetAttribute("character", user.getCharacter().ToString ());
			elmUser.SetAttribute("oxygen", user.getOxygen().ToString ());
			elmUser.SetAttribute("health", user.getHealth().ToString ());
			elmUser.SetAttribute("food", user.getFood().ToString ());
			elmUser.SetAttribute("drink", user.getDrink().ToString ());
			elmUser.SetAttribute("role", user.getRole().ToString ());

			elmUsers.AppendChild(elmUser); // make the node the parent.
			//setInitLog ("Wrote user " + user.getUsername());
		}
		elmRoot.AppendChild(elmUsers);
		xmlDoc.Save(filepath); // save file.
		//setInitLog("Saved");
	}
	
	public void LoadFromXml()
	{
		dbUsers.Clear();
		
		string filepath = Application.dataPath + @"/users.xml";
		XmlDocument xmlDoc = new XmlDocument();
		if (File.Exists(filepath))
		{
			xmlDoc.Load(filepath);
			XmlNodeList userList = xmlDoc.GetElementsByTagName("users");
			foreach (XmlNode userInfo in userList)
			{
				XmlNodeList usercontent = userInfo.ChildNodes;
				foreach (XmlNode userItems in usercontent)
				{
					if (userItems.Name == "user")
					{
						string username = userItems.Attributes ["username"].Value;
						string password = userItems.Attributes ["password"].Value;
						int credits = int.Parse (userItems.Attributes ["credits"].Value);
						int x = int.Parse (userItems.Attributes ["x"].Value);
						int y = int.Parse (userItems.Attributes ["y"].Value);
						int z = int.Parse (userItems.Attributes ["z"].Value);
						int s = int.Parse (userItems.Attributes ["s"].Value);
						int character = int.Parse (userItems.Attributes ["character"].Value);
						int oxygen = int.Parse (userItems.Attributes ["oxygen"].Value);
						int health = int.Parse (userItems.Attributes ["health"].Value);
						int food = int.Parse (userItems.Attributes ["food"].Value);
						int drink = int.Parse (userItems.Attributes ["drink"].Value);
						int role = int.Parse (userItems.Attributes ["role"].Value);

						User user = new User(username, password, x, y, z, s, credits, character, oxygen, health, food, drink, role);
						dbUsers.Add(username, user);
						setInitLog("Loaded user: " + user.getUsername());
					}
					
					
				}
			}
			
			
			
		} else
		{
			setInitLog("Missing Database.. creating");
			
			
			// create file
			XmlWriter xmlWriter = XmlWriter.Create(Application.dataPath + @"/users.xml");
			xmlWriter.WriteStartDocument();
			xmlWriter.WriteStartElement("db");
			xmlWriter.WriteStartElement("users");
			
			// end users
			xmlWriter.WriteEndElement();
			
			
			// end db
			xmlWriter.WriteEndElement();
			
			xmlWriter.WriteEndDocument();
			xmlWriter.Close();
			setInitLog("Wrote initial planet and database");
			
			
			
			LoadFromXml();
		}
	}
	

	
	public void setInitLog(string log)
	{
		initLog = log + "\n" + initLog;
	}
	
	void NetworkObjectMessage(string message)
	{
		setInitLog(message);
	}
	
	void processCommand(GameObject sender, string command, string arg1, string arg2)
	{
		switch (command)
		{
		case "auth":
			processAuth(sender, arg1, arg2);
			break;
		case "displayplay":
			processDisplayPlay(sender);
			break;
		case "register":
			processRegister(sender, arg1, arg2);
			break;
		default:
			setInitLog("ERROR_INVALIDCOMMAND: Received " + command + " request: (" + arg1 + ") (" + arg2 + ")");
			break;
		}
	}

	
	bool playerHasTaskQueued(string username, string taskname)
	{
		Task task = getTaskFromQueue(username, taskname);
		if (task != null)
		{
			return true;
		} else
		{
			return false;
		}
	}
	
	Task getTaskFromQueue(string username, string taskname)
	{
		taskname = username + "_" + taskname;
		if (dbTasks.ContainsKey(taskname))
		{
			return dbTasks [taskname];
		} else
		{
			return null;
		}
	}
	
	void addTaskToQueue(string taskname, Task task)
	{
		dbTasks.Add(taskname, task);
	}

	void setUserCredits(string username, int newcredits)
	{
		User user = getUserFromUsername(username);
		user.setCredits(newcredits);
		WriteToXml();
		
	}
	
	int getUserCredits(string username)
	{
		User user = getUserFromUsername(username);
		if (user != null)
		{
			return user.getCredits();
		} else
		{
			return 0;
		}
	}

	string getUsernameFromGuid(string guid)
	{
		return this.playerIdList [guid];
	}
	
	User getUserFromSender(GameObject sender)
	{
		return this.dbUsers [sender.networkView.owner.guid];
	}
	
	User getUserFromUsername(string username)
	{
		return dbUsers [username];
	}
	
	void processDisplayPlay(GameObject sender)
	{
		string username = getUsernameFromSender(sender);
		string playScreendata = getPlayscreendata(username);
		SendCmd(sender, "showscreen", "game_play", playScreendata);
	}

	string getPlayscreendata(string username)
	{
		string playdata = "";
		User user = getUserFromUsername(username);
		if (user != null)
		{
			int x = user.getX();
			int y = user.getY();
			int z = user.getZ();
			int s = user.getS();

			int credits = user.getCredits();
			int character = user.getCharacter();

			int oxygen = user.getOxygen();
			int health = user.getHealth();
			int food = user.getFood();
			int drink = user.getDrink();
			int role = user.getRole();

			playdata = username + "," +
				x.ToString() + "," + 
				y.ToString() + "," +
				z.ToString() + "," +
				s.ToString() + "," +
				credits.ToString()+ "," +
				character.ToString() + "," +
				oxygen.ToString() + "," +
				health.ToString() + "," +
				food.ToString() + "," +
				drink.ToString() + "," +
				role.ToString();
		}
		return playdata;
	}
	
	string getUsernameFromSender(GameObject sender)
	{
		string username = getUsernameFromGuid(sender.networkView.owner.guid);
		return username;
	}

	string getUsernameFromNetworkPlayer(NetworkPlayer networkPlayer)
	{
		string username = getUsernameFromGuid(networkPlayer.guid);
		return username;
	}

	int getNextAvailableCharacter()
	{
		int newcharacter = dbUsers.Count + 1;
		return newcharacter;
	}

	void processRegister(GameObject sender, string username, string password)
	{
		setInitLog("Registering User (" + username + ") (" + password + ")");

		int currentplayercount = dbUsers.Count;
		if (dbUsers.Count >= maxonlineplayers) {
			setInitLog("ERR_REGISTER_FULLSERVER: Error server is full during register: (" + username + ")");
			SendCmd(sender, "popup", "error", "the server is already at its player limit");
		} else {
			if (!dbUsers.ContainsKey (username)) {
				setInitLog ("Register free (" + username + ")");
				int nextcharacterid = getNextAvailableCharacter ();
				User user = new User (username, password, 0, 0, 0, 0, 0, nextcharacterid, 100, 100, 100, 100, 0);
				this.dbUsers.Add (username, user);
				setInitLog ("Writing database for new user (" + username + ")");
				WriteToXml ();   
				SendCmd (sender, "popup", "info", "registered");
				
			} else {
				setInitLog ("ERR_REGISTER_USEREXISTS: Error all crew have been filled: (" + username + ")");
				SendCmd (sender, "popup", "error", "Server Full, try another (no crew free)");
			}
		}
	}
	
	bool isLoggedIn(string username)
	{
		if (playerIdList.ContainsValue(username))
		{
			return true;
		} else
		{
			return false;
		}
	}

	void setPlayerPlaymode(string username, bool mode)
	{
		dbUsers [username].setPlaymode (mode);
	}

	
	void processAuth(GameObject sender, string username, string password)
	{
		setInitLog("Processing Auth (" + username + ") (" + password + ")");
		
		if (dbUsers.ContainsKey(username))
		{
			// check if account is already logged in
			if (!isLoggedIn(username))
			{
				if (password.Equals(dbUsers [username].getPassword()))
				{
					setInitLog("Authenticated (" + username + ")");
					// assign player to the guidtoaccountlist

					// mark the player as active
					setPlayerPlaymode(username, true);
					this.playerIdList.Add(sender.networkView.owner.guid, username);

					processDisplayPlay(sender);
				} else
				{
					setInitLog("ERR_INVALIDAUTH_PASS: Invalid Auth (" + username + ")");
					SendCmd(sender, "popup", "error", "invalid pass");
				}
			} else
			{
				setInitLog("ERR_INVALIDAUTH_LOGGEDIN: Already logged in elsewhere (" + username + ")");
				SendCmd(sender, "popup", "error", "sorry, logged in elsewhere");
			}
			
		} else
		{
			setInitLog("ERR_INVALIDAUTH_USER: Invalid Auth (" + username + ")");
			SendCmd(sender, "popup", "error", "invalid user");
		}
	}
	
	void SendCmd(GameObject receiver, string command, string arg1, string arg2)
	{
		ArrayList list = new ArrayList();
		list.Add(command);
		list.Add(arg1);
		list.Add(arg2);
		string data = getBFStringFromList(list);
		receiver.networkView.RPC("NetworkCmd", RPCMode.Others, data);
	}
	
	public void NetworkCmdReceiver(GameObject sender, string data)
	{
		ArrayList list = getListFromBFString(data);
		
		string command = (string)list [0];
		string arg1 = (string)list [1];
		string arg2 = (string)list [2];
		
		processCommand(sender, command, arg1, arg2);
	}
	
	void InitalizeNetworkObject()
	{   
		setInitLog("Initializing NetworkObject...");
		networkObject = (GameObject)Network.Instantiate(networkObjectMaster, spawnPoint.transform.position, spawnPoint.transform.rotation, 0);
		networkObject.networkView.RPC ("syncState", RPCMode.All, networkObject.networkView.viewID, 0);
		BroadcastMessage ("BecomeDocile", true);
	}
	
	void InitializeServer()
	{
		setInitLog("Starting Server...");
		//Network.SetLevelPrefix (networkversion);
		bool useNat = !Network.HavePublicAddress();
		
		Network.InitializeServer(maxonlineplayers, 25000, useNat);
		LoadFromXml();
		InitalizeNetworkObject();
	}
	
	void OnServerInitialized()
	{
		setInitLog("Server started and listening");
	}
	
	void OnPlayerConnected(NetworkPlayer player)
	{
		setInitLog("Player connected from " + player.ipAddress + ":" + player.port);
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		setInitLog("Player (" + player + ") disconnected");
		// remove player from guidtoaccountlist
		if (playerIdList.ContainsKey(player.guid))
		{
			string username = getUsernameFromNetworkPlayer(player);
			setPlayerPlaymode(username,false);
			setInitLog("Cleared GUID to Account link and set playmode false for (" + username + ")");
			this.playerIdList.Remove(player.guid);
		}
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
	
	
}
