using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

//this script for player prefab object local controller of scsene
public class NetworkPlayer : NetworkBehaviour
{

	void Start()
	{
		Debug.Log(isLocalPlayer);
		Debug.Log(isServer);
		Debug.Log(isClient);
	}

	void SayHello()
	{
		Debug.Log("hello" + this.name);
	}

	void Update()
	{
		if (!isLocalPlayer)
			return;
		//do something
	}

}
