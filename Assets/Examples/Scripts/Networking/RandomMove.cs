using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using sugi.cc;

public class RandomMove : NetworkBehaviour
{
	CoonsCurve moveCurve;
	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (!isServer)
			return;
		transform.position += Random.insideUnitSphere * Time.deltaTime;
	}
}
