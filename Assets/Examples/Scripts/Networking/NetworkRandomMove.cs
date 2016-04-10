using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using sugi.cc;

public class NetworkRandomMove : NetworkBehaviour
{
	[SyncVar]
	public float updateInterval = 3f;
	[SyncVar(hook = "OnSetTarget")]
	Vector3 targetPos;
	Vector3 previousPos;
	float timeTargetUpdated;
	CoonsCurve moveCurve;

	// Use this for initialization
	void Start()
	{
		previousPos = transform.position;
		if (isServer)
			updateInterval *= 1f + Random.value;
		if (isServer)
			InvokeRepeating("UpdateTarget", 0, updateInterval);
	}

	void Update()
	{
		if (moveCurve == null)
			return;
		var t = (Time.timeSinceLevelLoad - timeTargetUpdated) / updateInterval;
		transform.position = moveCurve.Interpolate(t);
	}

	[Server]
	void UpdateTarget()
	{
		targetPos = Random.insideUnitSphere * 10f;
	}

	[Client]
	void OnSetTarget(Vector3 pos)
	{
		var v0 = targetPos - previousPos;
		targetPos = pos;
		previousPos = transform.position;
		var v1 = targetPos - previousPos;
		if (moveCurve == null)
			moveCurve = new CoonsCurve(previousPos, targetPos, v0, v1);
		else
			moveCurve.SetVertices(previousPos, targetPos, v0, v1);
		timeTargetUpdated = Time.timeSinceLevelLoad;
	}
}
