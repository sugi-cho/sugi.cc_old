using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ReconnectableNetworkManager : NetworkManager
{
	public bool isServer;
	public bool isClient;

	void Start()
	{
		Invoke("Connect", 1f);

	}
	public void Connect()
	{
		if (isNetworkActive)
			return;
		if (isServer)
		{
			if (isClient)
				StartHost();
			else
				StartServer();
		}
		else if (isClient)
			StartClient();
		else
			Invoke("Connect", 1f);
	}
	public override void OnClientError(NetworkConnection conn, int errorCode)
	{
		Debug.LogFormat("error code {0}", errorCode);
		Invoke("Connect", 1f);
		base.OnClientError(conn, errorCode);
	}
	public override void OnClientDisconnect(NetworkConnection conn)
	{
		Debug.Log("disconnected");
		Invoke("Connect", 1f);
		base.OnClientDisconnect(conn);
	}
}
