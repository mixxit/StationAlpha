using UnityEngine;
using System.Collections;

public class mainMenu : MonoBehaviour {

	public string serveraddress = "play.fallofanempire.com";


	// Use this for initialization
	void Start () {
		if (!PlayerPrefs.GetString ("serveraddress").Equals ("")) {
			serveraddress = PlayerPrefs.GetString ("serveraddress");
		} else {
			serveraddress = "play.fallofanempire.com";
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI () 
	{
		GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));

		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		GUILayout.Label ("Server Address:");
		serveraddress = GUILayout.TextField (serveraddress);
		if (GUILayout.Button ("Connect")) {
			PlayerPrefs.SetString("serveraddress", serveraddress);
			Application.LoadLevel ("Client");

		};
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();



		GUILayout.EndArea();
	}
}
