using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public Transform spawnPoint;
	public bool playMode = false;
	private Server parent_server;
	private Client parent_client;
	private string type = "none";
	private float timer = 0; // set duration time in seconds in the Inspector
	private float ticklength = 3;
	public string localName; // Holds the local player name
	private Vector3 namePlatePos;
	private GUIStyle namePlate = new GUIStyle();
	public int rank = 0;

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

	void SetRank(int rank)
	{
		Debug.Log ("Setting rank");
		this.rank = rank;
		localName = getNameFromRank (rank);
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
			// tick 
			//setInitLog("A universal tick has occured");
			processTick();
			timer = ticklength;
		}
	}


	
	void OnGUI() 
	{
		if (playMode == true) {
			// Place the name plate where the gameObject (player prefab) is
			namePlatePos = Camera.main.WorldToScreenPoint (gameObject.transform.position);  
			GUI.Label (new Rect ((namePlatePos.x - 25), (Screen.height - namePlatePos.y - 60), 100, 50), localName, namePlate);  
		}
	}

	void processTick()
	{
		if (networkView.isMine) 
		{
			if (playMode == true) {
				Debug.Log ("Replicating playMode");
				networkView.RPC ("syncState", RPCMode.All, networkView.viewID, 1);
			}

			if (playMode == false) {
				Debug.Log ("Replicating playMode");
				networkView.RPC ("syncState", RPCMode.All, networkView.viewID, 0);
			}

		}
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
			int rank = this.rank;
			stream.Serialize(ref rank);
		} else {
			int rank = 0;
			stream.Serialize(ref rank);
			this.rank = rank;

		}
	}

	private string getNameFromRank(int rank)
	{
		switch (rank) 
		{
			case 1:
				return "Reward_Cooker";
				break; 
				
			case 2:
				
				return "Euglan_Homitch";
				break; 
				
			case 3:
				
				return "Mine_Manes";
				break; 
				
			case 4:
				
				return "Crence_Jonand";
				break; 
				
			case 5:
				
				return "Homy_Warte";
				break; 
				
			case 6:
				
				return "Tory_Rookson";
				break; 
				
			case 7:
				
				return "Samy_Butley";
				break; 
				
			case 8:
				
				return "Kenny_Yourphylo";
				break; 
				
			case 9:
				
				return "Raige_Campbins";
				break; 
				
			case 10:
				
				return "Waltodd_Cooker";
				break; 
				
			case 11:
				
				return "Heward_Harray";
				break; 
				
			case 12:
				
				return "Eustin_Lezal";
				break; 
				
			case 13:
				
				return "Jesse_Halley";
				break; 
				
			case 14:
				
				return "Frickeith_Coopatt";
				break; 
				
			case 15:
				
				return "Stophy_Baily";
				break; 
				
			case 16:
				
				return "Griany_Arrill";
				break; 
				
			case 17:
				
				return "Juston_Righte";
				break; 
				
			case 18:
				
				return "Damy_Harre";
				break; 
				
			case 19:
				
				return "Amuer_Rownes";
				break; 
				
			case 20:
				
				return "Phomy_Reson";
				break; 
				
			case 21:
				
				return "Lora_Tera";
				break; 
				
			case 22:
				
				return "Amah_Yournes";
				break; 
		case 23:
				return "Dora_Welley";
				break; 
			case 24:
				return "Laura_Monson";
				break; 
			case 25:
				return "Jacquel_Grezal";
				break; 
			case 26:
				return "Endan_Hompson";
				break; 
			case 27:
				return "Jenna_Yourphylo";
				break; 
			case 28:
				return "Mela_Rodre";
				break; 
			case 29:
				return "Paule_Maner";
				break; 
			case 30:
				return "July_Pery";
				break; 
			case 31:
				return "Dennio_Walker";
				break; 
			case 32:
				return "Gery_Butlee";
				break; 
			case 33:
				return "Amip_Andes";
				break; 
			case 34:
				return "Jone_Prodry";
				break; 
			case 35:
				return "Stinio_Stinez";
				break; 
			case 36:
				return "Juane_Wilson";
				break; 
			case 37:
				return "Raymy_Ander";
				break; 
			case 38:
				return "Jerey_Parker";
				break; 
			case 39:
				return "Brichy_Artis";
				break; 
			case 40:
				return "Damy_Derson";
				break; 
			case 41:
				return "Juane_Righte";
				break; 
			case 42:
				return "Bertoph_Rosson";
				break; 
			case 43:
				return "Awrer_Johnson";
				break; 
			case 44:
				return "Riston_Watson";
				break; 
			case 45:
				
				return "Jesse_Colly";
				break; 
			case 46:
				return "Arryne_Parker";
				break; 
			case 47:
				return "Damy_Rookson";
				break; 
			case 48:
				return "Randy_Phardson";
				break; 
			case 49:
				return "Gralphy_Jenking";
				break; 
			case 50:		
				return "Ason_Robell";
				break;
			default:
				return "Player";
				break;

		}

	}

	
	[RPC]
	void syncState(NetworkViewID viewID, int data)
	{
		Debug.Log ("playMode Synced");
		//state sycnronisation
		if (data == 1) {
			this.playMode = true;
			foreach (Renderer mr in GetComponentsInChildren<Renderer>()) {
				mr.enabled = true;
			}
		}

		if (data == 2) {
			this.playMode = false;
			foreach (Renderer mr in GetComponentsInChildren<Renderer>()) {
				mr.enabled = false;
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
	void InitLogMessage(string message)
	{
		SendMessageUpwards ("NetworkObjectMessage", message, SendMessageOptions.RequireReceiver);
	}

}
