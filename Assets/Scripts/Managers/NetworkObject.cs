using UnityEngine;
using System.Collections;

public class NetworkObject : MonoBehaviour {
	
	private Server parent_server;
	private Client parent_client;
	private string type = "none";
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
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
