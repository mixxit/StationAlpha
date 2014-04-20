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

	void Start ()
	{
		Debug.Log("Start called.");
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
			// tick 
			//setInitLog("A universal tick has occured");
			processTick();
			timer = ticklength;
		}
	}

	void processTick()
	{
		if (networkView.isMine) 
		{
			if (playMode == true) {
				Debug.Log ("Replicating playMode");
				networkView.RPC ("playmode", RPCMode.All, networkView.viewID);
			}
		}
	}


	[RPC]
	void playmode(NetworkViewID viewID)
	{
		Debug.Log ("playMode Synced");
		//state sycnronisation
		this.playMode = true;
		foreach (Renderer mr in GetComponentsInChildren<Renderer>())
		{
			mr.enabled = true;
		}
	}

	[RPC]
	void nonplaymode(NetworkViewID viewID)
	{
		//state sycnronisation

		this.playMode = false;
		foreach (Renderer mr in GetComponentsInChildren<Renderer>())
		{
			mr.enabled = false;
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
