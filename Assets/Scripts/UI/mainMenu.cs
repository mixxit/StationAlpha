using UnityEngine;
using System.Collections;

public class mainMenu : MonoBehaviour {

	public int version = 0;
	public string gameName = "AstroWorld";
	public string serveraddress = "play.fallofanempire.com";
	private bool isRefreshingHostList = false;
	private HostData[] hostList;

	// Use this for initialization
	void Start () {
		if (!PlayerPrefs.GetString ("serveraddress").Equals ("")) {
			serveraddress = PlayerPrefs.GetString ("serveraddress");
		} else {
			serveraddress = "play.fallofanempire.com";
		}
	}

	void Awake()
	{

	}

	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
		
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical();

		if (!Network.isClient)
		{
			if (GUILayout.Button("Get Network Servers"))
				RefreshHostList();
		
			
			if (hostList != null)
			{
				for (int i = 0; i < hostList.Length; i++)
				{
					
					if (GUILayout.Button(hostList[i].gameType + " - " + hostList[i].ip[0]))
					{

						serveraddress = hostList[i].ip[0];
						PlayerPrefs.SetString("serveraddress", serveraddress);
						Application.LoadLevel ("Client");

					}
					
				}
			}
		}
		GUILayout.EndVertical ();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		
		GUILayout.EndArea();



	}
		
	void Update()
	{
		if (isRefreshingHostList && MasterServer.PollHostList().Length > 0)
		{


			isRefreshingHostList = false;
			hostList = MasterServer.PollHostList();
		}
	}
	
	private void RefreshHostList()
	{
		//MasterServer.ClearHostList();
		MasterServer.ipAddress = "play.fallofanempire.com";
		MasterServer.port = 23466;

		if (!isRefreshingHostList)
		{
			isRefreshingHostList = true;
			MasterServer.RequestHostList(gameName+version.ToString());
		}
	}
	

}
