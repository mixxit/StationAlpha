using UnityEngine;

using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

public class Client : MonoBehaviour {
	
	private string popuptype;
	private string popupmessagetext;
	private string currentForm = "initialize";
	private string lastForm = "";
	public GameObject networkObjectMaster;
	public GameObject networkObject;
	private string username = "";
	private string password = "";
	public int networkversion;
	private string initLog = "Initializing...";
	private bool popup = false;
	public Texture2D BgImgFile;
	private User formUserCache;
	public Vector2 scrollPosition;
	Rect windRect;
	public GUISkin customSkin;
	public string ipaddress = "127.0.0.1";
	private int port = 25005;
	public AudioClip sound;
	public AudioClip music;
	public AudioSource audio;
	public AudioSource musicsource;
	bool playmode = false;
	public GameObject playerPrefab;
	public GameObject spawnPoint;
	public Object player;

	// Use this for initialization
	void Start () {
		ipaddress = PlayerPrefs.GetString ("serveraddress");
		InitializeClient ();
		audio = gameObject.AddComponent<AudioSource>();
		musicsource = gameObject.AddComponent<AudioSource>();
		musicsource.clip = music;
		musicsource.loop = true;
		musicsource.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void Awake(){
		
		windRect = new Rect(0, 0, Screen.width, Screen.height);
	}
	
	void OnGUI () {
		if (playmode == false) 
		{
			GUI.skin = customSkin;
			GUI.skin.button.wordWrap = true;
		
			//GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), BgImgFile);
			windRect = GUILayout.Window (0, windRect, None, "test");
		
			if (popup) {
				GUI.Window (1, new Rect ((Screen.width / 2) - 150, (Screen.height / 2) - 75, 300, 250), ShowPopup, popuptype);
			}
		} else {


		}
	}
	void setInitLog(string log)
	{
		initLog = log;
	}
	
		
	public void NetworkCmdReceiver(string data)
	{
		ArrayList list = getListFromString(data);
		
		string command = (string)list [0];
		string arg1 = (string)list [1];
		string arg2 = (string)list [2];
		
		processCommand(command, arg1, arg2);
	}
	void processCommand(string command, string arg1, string arg2)
	{
		switch (command)
		{
			case "popup":
				popupMessage(arg1, arg2);
				break;
			case "showscreen":
				showScreen(arg1, arg2);
				break;
			case "synccache":
				syncCache(arg1,arg2);
				break;
			default:
				setInitLog("ERROR_INVALIDCOMMAND: Received " + command + " request: (" + arg1 + ") (" + arg2 + ")");
				break;
		}
	}

	void syncCache(string playercache, string scenecache)
	{

	}
	
	void showScreen(string gui, string objectdata)
	{
		currentForm = gui;
		if (objectdata != null)
		{
			switch(gui)
			{
				case "game_play":

					string[] userinfo = objectdata.Split(',');
					string username = userinfo[0]; 
					int x = int.Parse(userinfo[1]);
					int y = int.Parse(userinfo[2]);
					int z = int.Parse(userinfo[3]);
					int s = int.Parse(userinfo[4]);
					int credits = int.Parse(userinfo[5]);
					int character = int.Parse(userinfo[6]);
					int oxygen = int.Parse(userinfo[7]);
					int health = int.Parse(userinfo[8]);
					int food = int.Parse(userinfo[9]);
					int drink = int.Parse(userinfo[10]);
					int role = int.Parse(userinfo[11]);

					CloseLoginAndStartGame(username);
					break;
				default:

					break;
					
				}
		}
	}

	void CloseLoginAndStartGame(string username)
	{
		currentForm = null;
		playmode = true;
		BroadcastMessage ("BecomeDocile", false);
		//SpawnPlayer ();

	}

	void popupMessage(string arg1,string arg2)
	{
		popuptype = arg1;
		popupmessagetext = arg2;
		popup = true;
		
	}
	
	void ShowPopup(int windowID)
	{
		GUILayout.Label(popuptype);
		GUILayout.FlexibleSpace();
		// You may put a label to show a message to the player
		GUILayout.Label(popupmessagetext,GUIStyle.none);
		// You may put a button to close the pop up too 
		GUILayout.FlexibleSpace();
		if (GUILayoutSoundButton("OK"))
		{
			if (popuptype.Equals("disconnected"))
			{
				Application.Quit();
			} else {
				popup = false;
			}
		}
	}

	void None(int windowID)
	{

		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		// CONTENT
		
		if (currentForm.Equals("initialize"))
		{
			if ( GUILayoutSoundButton("Exit" ) ) {
				Application.Quit();
			}
			if ( GUILayoutSoundButton("Music Off" ) ) {
				musicsource.Stop();
			}
			GUILayout.Label (initLog);
			
			
		}

		switch (currentForm)
		{
		case "login":
			GUILayout.Label ("Username");
			username = GUILayout.TextField (username);
			GUILayout.Label ("Password");
			password = GUILayout.PasswordField(password, "*"[0], 25);
			if (GUILayoutSoundButton ("Login")) {
				SendLogin();
			}
			if (GUILayoutSoundButton ("Quit")) {
				//DoStuff();
				Application.Quit();
			}
			
			if (GUILayoutSoundButton ("New Account")) {
				Register();
			}
			break;
		default:

			break;
		}
		// SCROLL MANAGER FOOTER
		
		GUILayout.EndScrollView();
		
		/*
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.EndArea();*/
	}
	void SendPurchaseResearch(string researchname)
	{
		SendCmd("buyresearch",researchname, "");
	}
	

	void Register()
	{
		if (!username.Equals("") && !password.Equals(""))
		{
			SendCmd("register",username,password);
		} else
		{
			popupMessage("Error", "Please insert a username and password to register");
		}
	}
	
	void SendLogin()
	{
		if (!username.Equals ("") && !password.Equals ("")) 
		{
			SendCmd("auth",username,password);
			
		}
	}

	
	void SendCmd(string command, string arg1, string arg2)
	{
		ArrayList list = new ArrayList ();
		list.Add (command);
		list.Add (arg1);
		list.Add (arg2);
		string data = getStringFromList (list);
		this.networkObject.networkView.RPC("NetworkCmd",RPCMode.Server,data);
	}
	
	void OnConnectedToServer() {
		setInitLog("Connected to server");
		InitalizeNetworkObject ();
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info) {
		popupMessage("disconnected", "lost connection to server");
	}
	
	void InitalizeNetworkObject()
	{
		setInitLog ("Initializing NetworkObject...");
		networkObject = (GameObject)Network.Instantiate (networkObjectMaster,spawnPoint.transform.position, spawnPoint.transform.rotation, 0);
		BroadcastMessage ("BecomeDocile",true);
	}
	
	void NetworkObjectMessage(string message)
	{
		setInitLog (message);
	}
	
	void InstantiateDone(string message)
	{
		setInitLog (message);
		PresentLogin ();
	}
	
	void PresentLogin()
	{
		setForm("login");
	}
	void setForm(string formname)
	{
		currentForm = formname;
	}
	
	void InitializeClient()
	{
		setInitLog ("Connecting to server...");
		Network.Connect (ipaddress, port);
		
	}
	
	void OnFailedToConnect(NetworkConnectionError error) {
		setInitLog("Could not connect to server: " + error);
	}
	
	string getStringFromList(ArrayList list)
	{
		MemoryStream ms = new MemoryStream ();
		BinaryFormatter bf = new BinaryFormatter ();
		bf.Serialize(ms, list); //Save the list
		return System.Convert.ToBase64String(ms.GetBuffer());
	}
	
	string getStringFromUser(User obj)
	{
		MemoryStream ms = new MemoryStream ();
		BinaryFormatter bf = new BinaryFormatter ();
		bf.Serialize(ms, obj); //Save the list
		return System.Convert.ToBase64String(ms.GetBuffer());
	}
	
	User getUserFromString(string str)
	{
		MemoryStream ms = new MemoryStream(System.Convert.FromBase64String(str)); //Create an input stream from the string
		BinaryFormatter bf = new BinaryFormatter ();
		User obj = (User)bf.Deserialize (ms);
		return obj;
	}
	
	ArrayList getListFromString(string data)
	{
		MemoryStream ms = new MemoryStream(System.Convert.FromBase64String(data)); //Create an input stream from the string
		BinaryFormatter bf = new BinaryFormatter ();
		ArrayList list = (ArrayList)bf.Deserialize (ms);
		return list;
	}
	
	bool GUILayoutSoundButton (string text) 
	{
		if(GUILayout.Button(text)) {
			audio.PlayOneShot(sound);
			return true;
		}
		return false;
	}
	
	bool GUILayoutSoundButton (Texture image)
	{
		if(GUILayout.Button(image)) {
			audio.PlayOneShot(sound);
			return true;
		}
		return false;
	}
	
	bool GUILayoutSoundButton (GUIContent content)
	{
		if(GUILayout.Button(content)) {
			audio.PlayOneShot(sound);
			return true;
		}
		return false;
	}
	
	bool GUILayoutSoundButton (string text,GUIStyle style)
	{
		if(GUILayout.Button(text, style)) {
			audio.PlayOneShot(sound);
			return true;
		}
		return false;
	}
	
	bool GUILayoutSoundButton (Texture image,GUIStyle style)
	{
		if(GUILayout.Button(image, style)) {
			audio.PlayOneShot(sound);
			return true;
		}
		return false;
	}
	
	bool GUILayoutSoundButton (Texture image,GUIStyle style, GUILayoutOption style2)
	{
		if(GUILayout.Button(image, style, style2)) {
			audio.PlayOneShot(sound);
			return true;
		}
		return false;
	}
	
	bool GUILayoutSoundButton (Texture image,GUIStyle style, GUILayoutOption style2, GUILayoutOption style3)
	{
		if(GUILayout.Button(image, style, style2, style3)) {
			audio.PlayOneShot(sound);
			return true;
		}
		return false;
	}
	
	bool GUILayoutSoundButton (GUIContent content,GUIStyle style){
		if(GUILayout.Button(content, style)) {
			audio.PlayOneShot(sound);
			return true;
		}
		return false;
	}
	
	//
	
	bool GUISoundButton (Rect position,string text) 
	{
		if(GUI.Button(position, text)) {
			audio.PlayOneShot(sound);
			return true;
		}
		return false;
	}
	
	bool GUISoundButton (Rect position,Texture image)
	{
		if(GUI.Button(position, image)) {
			audio.PlayOneShot(sound);
			return true;
		}
		return false;
	}
	
	bool GUISoundButton (Rect position,GUIContent content)
	{
		if(GUI.Button(position, content)) {
			audio.PlayOneShot(sound);
			return true;
		}
		return false;
	}
	
	bool GUISoundButton (Rect position,string text,GUIStyle style)
	{
		if(GUI.Button(position, text, style)) {
			audio.PlayOneShot(sound);
			return true;
		}
		return false;
	}
	
	bool GUISoundButton (Rect position,Texture image,GUIStyle style)
	{
		if(GUI.Button(position, image, style)) {
			audio.PlayOneShot(sound);
			return true;
		}
		return false;
	}
	
	bool GUISoundButton (Rect position,GUIContent content,GUIStyle style){
		if(GUI.Button(position, content, style)) {
			audio.PlayOneShot(sound);
			return true;
		}
		return false;
	}
}
