using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using sugi.cc;

//server controller of scene
public class NetworkController : NetworkBehaviour
{
	#region instance
	public static NetworkController Instance
	{
		get
		{
			if (_Instance == null)
				_Instance = FindObjectOfType<NetworkController>();
			return _Instance;
		}
	}
	static NetworkController _Instance;
	#endregion
	public static void Remove(NetworkInstanceId nid)
	{
		if (NetworkServer.active)
		{
			var go = NetworkServer.FindLocalObject(nid);
			if (go == null)
				return;
			Instance.RemoveObject(go);
		}

	}
	public static void SendMessageToObject(NetworkInstanceId nid, string message)
	{
		Instance.RpcSendMessage(nid, message);
	}

	public NetworkSetting setting;
	public GameObject obj;
	GameObject[] objs;

	// Use this for initialization
	void Start()
	{
		SettingManager.AddSettingMenu(setting, "Network/setting.json");
		if (isServer)
		{
			objs = new GameObject[100];
			for (var i = 0; i < 100; i++)
			{
				var newObj = Instantiate<GameObject>(obj);
				newObj.transform.position = transform.position + Random.insideUnitSphere * 5f;
				NetworkServer.Spawn(newObj);
				objs[i] = newObj;
			}
		}
	}

	void Update()
	{
		if (!Input.GetMouseButtonDown(0))
			return;

		if (isServer)
		{
			var idx = Random.Range(0, objs.Length);
			var go = objs[idx];
			if (go != null)
			{
				var nid = go.GetComponent<NetworkIdentity>().netId;
				Debug.Log(NetworkServer.FindLocalObject(nid));
				RpcSendMessage(nid, "SayHello");
			}
		}
	}

	[Server]
	void RemoveObject(GameObject go)
	{
		NetworkServer.Destroy(go);
	}

	[ClientRpc]
	void RpcSendMessage(NetworkInstanceId nid, string message)
	{
		var go = ClientScene.FindLocalObject(nid);
		if (go == null)
			return;
		go.SendMessage(message, SendMessageOptions.DontRequireReceiver);
	}

	void SyncSetting()
	{
		var json = JsonUtility.ToJson(setting);
		RpcSync(json);
	}
	[ClientRpc]
	void RpcSync(string json)
	{
		JsonUtility.FromJsonOverwrite(json, setting);
		setting.dataEditor = new DataUI.FieldEditor(setting);
	}

	[System.Serializable]
	public class NetworkSetting : SettingManager.Setting
	{
		public float value1;
		public float value2;
		public Vector3 position;

		public override void OnGUIFunc()
		{
			base.OnGUIFunc();
			if (NetworkServer.active)
				if (GUILayout.Button("Sync All Clients"))
					NetworkController.Instance.SyncSetting();
		}
	}
}
