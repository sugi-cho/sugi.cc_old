using UnityEngine;
using System.Collections;
using sugi.cc;

public class ScreenRecorder : MonoBehaviour
{
	#region Instance
	static ScreenRecorder Instance
	{
		get
		{
			if (_Instance == null)
				_Instance = FindObjectOfType<ScreenRecorder>();
			return _Instance;
		}
	}
	static ScreenRecorder _Instance;
	#endregion

	public RecordSetting setting;
	public bool autoRecord;

	int frameCount;
	bool recording;

	void Start()
	{
		if (autoRecord) StartRecording();
		SettingManager.AddSettingMenu(setting, "RecordSetting/setting.json");
	}

	void StartRecording()
	{
		System.IO.Directory.CreateDirectory("Capture");
		Time.captureFramerate = setting.frameRate;
		frameCount = -1;
		recording = true;
	}

	void Update()
	{
		if (recording)
		{
			if (Input.GetMouseButtonDown(0))
			{
				recording = false;
				enabled = false;
			}
			else
			{
				if (frameCount > 0)
				{
					var name = "Capture/frame" + frameCount.ToString("0000") + ".png";
					Application.CaptureScreenshot(name, setting.superSize);
				}

				frameCount++;

				if (frameCount > 0 && frameCount % setting.frameRate == 0)
				{
					Debug.Log((frameCount / setting.frameRate).ToString() + " seconds elapsed.");
				}
			}
		}
	}

	[System.Serializable]
	public class RecordSetting : SettingManager.Setting
	{
		public int superSize = 0;
		public int frameRate = 30;

		public override void OnGUIFunc()
		{
			base.OnGUIFunc();
			if (GUILayout.Button("Start Recording"))
			{
				ScreenRecorder.Instance.StartRecording();
				SettingManager.Instance.HideGUI();
			}
		}
	}
}