using UnityEngine;
using System.Collections;
using sugi.cc;

public class TestData : MonoBehaviour
{
	[SerializeField]
	SettingData data;
	// Use this for initialization
	void Start ()
	{
		SettingManager.AddSettingMenu (data, "test/setting.json");
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	[System.Serializable]
	public class SettingData : SettingManager.Setting
	{
		public float f;
		public int i;
		public Vector4 v4;
		public Vector3 v3;
		public Vector2 v2;
		public Color col;
		public Matrix4x4 m4x4;

		public override void OnGUIFunc()
		{
			base.OnGUIFunc();
		}
		protected override void OnLoad()
		{
			base.OnLoad();
		}
		protected override void OnClose()
		{
			base.OnClose();
		}
	}
}
