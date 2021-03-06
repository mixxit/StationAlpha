﻿using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
	public Transform spawnPoint;
	public bool playMode = false;
	private Server parent_server;
	private Client parent_client;
	public string type = "none";
	private float timer = 0; // set duration time in seconds in the Inspector
	private float ticklength = 3;
	public string localName; // Holds the local player name
	private Vector3 namePlatePos;
	private GUIStyle namePlate = new GUIStyle();
	private User userCache;

	void Start ()
	{
		this.spawnPoint = this.gameObject.transform;
		rigidbody.isKinematic = !networkView.isMine;
		rigidbody.useGravity = networkView.isMine;
		foreach (Renderer mr in GetComponentsInChildren<Renderer>())
		{
			mr.enabled = false;
		}
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		if (timer > 0)
		{
			timer -= Time.deltaTime;
		}
		if (timer <= 0)
		{
			timer = ticklength;
		}
	}

	void OnGUI() 
	{
		if (playMode == true) {
			// Place the name plate where the gameObject (player prefab) is
			namePlatePos = Camera.main.WorldToScreenPoint (gameObject.transform.position);  
			GUI.Label (new Rect ((namePlatePos.x - 25), (Screen.height - namePlatePos.y - 60), 100, 50), localName, namePlate);  

			// Client HUD
			// Oxygen
			if (this.userCache != null)
			{
				GUILayout.Label ("Oxygen: " + this.userCache.getOxygen());
				GUILayout.Label ("Health: " + this.userCache.getHealth());
				GUILayout.Label ("Food: " + this.userCache.getFood());
				GUILayout.Label ("Drink: " + this.userCache.getDrink ());
				GUILayout.Label ("Role: " + getRoleNameFromRoleID(this.userCache.getRole ()));
				//GUILayout.Label ("PlayMode: " + this.userCache.getPlayMode ());

			}
		}
	}

	void OnNetworkInstantiate(NetworkMessageInfo info) 
	{
		//networkView.RPC ("setParent", RPCMode.Server, "Network object initialized");
		if (!this.transform.parent) 
		{
			GameObject server = GameObject.Find("Server");
			GameObject client = GameObject.Find("Client");
			// if server
			if (server)
			{
				this.type = "server";
				this.transform.parent = server.gameObject.transform;
				this.parent_server = (Server)server.GetComponent(typeof(Server));
				SendMessageUpwards ("NetworkObjectMessage", "NetworkObject initaliased and attached", SendMessageOptions.RequireReceiver);
			}
			if(client) 
			{
				if(networkView.isMine)
				{
					this.type = "client";
					this.transform.parent = client.gameObject.transform;
					Debug.Log ("My network object has been intantiated");
					SendMessageUpwards ("InstantiateDone", "NetworkObject initaliased and attached", SendMessageOptions.RequireReceiver);
					this.parent_client = (Client)client.GetComponent(typeof(Client));
				} else {
					BroadcastMessage("BecomeDocile", true);
				}
			}
		}
	}
	
	[RPC]
	void NetworkCmd(string data)
	{
		Debug.Log ("Network Cmd being received");
		if (this.type.Equals("server"))
		{
			parent_server.NetworkCmdReceiver(this.gameObject,data);
		}
		if (this.type.Equals("client"))
		{
			if(networkView.isMine)
			{
				// does not contain a sender as this is always from the server
				parent_client.NetworkCmdReceiver(data);
			}
		}
	}

	[RPC]
	void syncState(string data)
	{
		User userdata = (User)BFStringToObject (data);
		Debug.Log ("Recieved syncState for user " + userdata.getUsername());
		this.userCache = userdata;

		if (this.playMode != userdata.getPlayMode())
		{
			this.playMode = userdata.getPlayMode();
			if (this.playMode == true)
			{
				foreach (Renderer mr in GetComponentsInChildren<Renderer>())
				{
					mr.enabled = true;
				}
			} else {
				foreach (Renderer mr in GetComponentsInChildren<Renderer>())
				{
					mr.enabled = false;
				}
			}
		}
	}


	public string ObjectToBFString(object obj)
	{
		using (MemoryStream ms = new MemoryStream())
		{
			new BinaryFormatter().Serialize(ms, obj);         
			return System.Convert.ToBase64String(ms.ToArray());
		}
	}
	
	public object BFStringToObject(string base64String)
	{    
		byte[] bytes = System.Convert.FromBase64String(base64String);
		using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
		{
			ms.Write(bytes, 0, bytes.Length);
			ms.Position = 0;
			return new BinaryFormatter().Deserialize(ms);
		}
	}
	
	[RPC]
	void InitLogMessage(string message)
	{
		SendMessageUpwards ("NetworkObjectMessage", message, SendMessageOptions.RequireReceiver);
	}

	private string getRoleNameFromRoleID(int role)
	{
		switch (role) 
		{
			case 0:
				return "civilian";
			case 1:
				return "security officer";
			case 2:
				return "doctor";
			case 3:
				return "scientist";
			case 4:
				return "engineer";
			case 5:
				return "pilot";
			case 6:
				return "technician";
			case 7:
				return "overseer";
			case 8:
				return "psychiatrist";
			case 9:
				return "communications officer";
			case 10:
				return "chef";
			case 11:
				return "cleaner";
			case 12:
				return "administrator";
			case 13:
				return "musician";
			case 14:
				return "journalist";
			default:
				return "civilian";
		}
	}

	private string getNameFromCharacter(int character)
	{
		switch (character) 
		{
			case 1:
				return "Reward_Cooker";
			case 2:
				return "Euglan_Homitch";
			case 3:
				return "Mine_Manes";
			case 4:
				return "Crence_Jonand";
			case 5:
				return "Homy_Warte";
			case 6:
				return "Tory_Rookson";
			case 7:
				return "Samy_Butley";
			case 8:
				return "Kenny_Yourphylo";
			case 9:
				return "Raige_Campbins";
			case 10:
				return "Waltodd_Cooker";
			case 11:
				return "Heward_Harray";
			case 12:
				return "Eustin_Lezal";
			case 13:
				return "Jesse_Halley";
			case 14:
				return "Frickeith_Coopatt";
			case 15:
				return "Stophy_Baily";
			case 16:
				return "Griany_Arrill";
			case 17:
				return "Juston_Righte";
			case 18:
				return "Damy_Harre";
			case 19:
				return "Amuer_Rownes";
			case 20:
				return "Phomy_Reson";
			case 21:
				return "Lora_Tera";
			case 22:
				return "Amah_Yournes";
			case 23:
				return "Dora_Welley";
			case 24:
				return "Laura_Monson";
			case 25:
				return "Jacquel_Grezal";
			case 26:
				return "Endan_Hompson";
			case 27:
				return "Jenna_Yourphylo";
			case 28:
				return "Mela_Rodre";
			case 29:
				return "Paule_Maner";
			case 30:
				return "July_Pery";
			case 31:
				return "Dennio_Walker";
			case 32:
				return "Gery_Butlee";
			case 33:
				return "Amip_Andes";
			case 34:
				return "Jone_Prodry";
			case 35:
				return "Stinio_Stinez";
			case 36:
				return "Juane_Wilson";
			case 37:
				return "Raymy_Ander";
			case 38:
				return "Jerey_Parker";
			case 39:
				return "Brichy_Artis";
			case 40:
				return "Damy_Derson";
			case 41:
				return "Juane_Righte";
			case 42:
				return "Bertoph_Rosson";
			case 43:
				return "Awrer_Johnson";
			case 44:
				return "Riston_Watson";
			case 45:
				return "Jesse_Colly";
			case 46:
				return "Arryne_Parker";
			case 47:
				return "Damy_Rookson";
			case 48:
				return "Randy_Phardson";
			case 49:
				return "Gralphy_Jenking";
			case 50:		
				return "Ason_Robell";
			default:
				return "Player";
		}
	}

}
