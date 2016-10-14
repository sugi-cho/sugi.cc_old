using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using DataUI;

namespace sugi.cc
{
    public class SettingManager : MonoBehaviour
    {
        public static void AddSettingMenu(Setting setting, string filePath)
        {
            setting.LoadSettingFromFile(filePath);
            setting.dataEditor = new FieldEditor(setting);
            if (!Instance.settings.Contains(setting))
            {
                Instance.settings.Add(setting);
                Instance.settings = Instance.settings.OrderBy(b => b.filePath).ToList();
            }
        }
        public static void AddExtraGuiFunc(System.Action func)
        {
            if (!Instance.extraGuiFuncList.Contains(func))
            {
                Instance.extraGuiFunc += func;
                Instance.extraGuiFuncList.Add(func);
            }
        }

        public static GUIStyle BoxStyle { get { return Instance.boxStyle; } }

        #region instance

        public static SettingManager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new GameObject("SettingManager").AddComponent<SettingManager>();
                return _Instance;
            }
        }

        static SettingManager _Instance;

        #endregion

        public static KeyCode EditKey = KeyCode.E;

        List<Setting> settings = new List<Setting>();
        Setting currentSetting;
        bool edit;
        Rect windowRect = Rect.MinMaxRect(0, 0, Mathf.Min(Screen.width, 1024f), Mathf.Min(Screen.height, 768f));
        Vector2 scroll;
        System.Action extraGuiFunc;
        List<System.Action> extraGuiFuncList = new List<System.Action>();

        GUIStyle boxStyle { get { if (_style == null) { _style = new GUIStyle("box"); } return _style; } }
        [SerializeField]
        GUIStyle _style;

        public void HideGUI()
        {
            edit = false;
            Cursor.visible = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(EditKey))
                edit = !edit;
            Cursor.visible = edit;
        }

        void OnGUI()
        {
            if (!edit)
                return;
            windowRect = GUI.Window(GetInstanceID(), windowRect, OnWindow, "Settings");
        }

        void OnWindow(int id)
        {
            scroll = GUILayout.BeginScrollView(scroll);
            settings.ForEach(setting =>
            {
                if (setting.edit)
                {
                    GUILayout.Space(16);

                    GUILayout.BeginVertical(boxStyle);
                    GUI.contentColor = Color.yellow;
                    GUILayout.Label(setting.filePath);
                    GUI.contentColor = Color.white;

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(16f);

                    GUILayout.BeginVertical();
                    setting.OnGUIFunc();
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Save and Close"))
                        setting.SaveAndClose();
                    if (NetworkServer.active && setting.canSync)
                        if (GUILayout.Button("Sync Setting"))
                            setting.SyncSetting();
                    if (GUILayout.Button("Cancel"))
                        setting.CancelAndClose();
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();

                    GUILayout.Space(16);
                }
                else if (GUILayout.Button(setting.filePath))
                    setting.edit = true;
            });
            if (extraGuiFunc != null)
                extraGuiFunc.Invoke();
            GUILayout.EndScrollView();
            GUI.DragWindow();
        }

        void OnRenderObject()
        {
            settings.ForEach(setting =>
            {
                setting.OnRenderObjectFunc(Camera.current);
            });
        }

        [System.Serializable]
        public abstract class Setting
        {
            public FieldEditor dataEditor { get; set; }

            public string filePath { get; set; }

            public bool edit { get; set; }
            public bool canSync
            {
                get { return _canSync; }
                set { if (value) NetworkMessageManager.AddHandler(LoadSettingFromJson); _canSync = value; }
            }
            bool _canSync;

            //use this Function as intisialize.
            public void LoadSettingFromFile(string path)
            {
                filePath = path;
                Helper.LoadJsonFile(this, filePath);
                OnLoad();
            }

            public void LoadSettingFromJson(string json)
            {
                JsonUtility.FromJsonOverwrite(json, this);
                dataEditor.Load();
                Save();//save to json
                OnLoad();//re initializing
            }
            void LoadSettingFromJson(NetworkMessage netMsg)
            {
                var msg = netMsg.ReadMessage<StringMessage>();
                LoadSettingFromJson(msg.value);
            }

            public void Save()
            {
                Helper.SaveJsonFile(this, filePath);
            }

            public void SaveAndClose()
            {
                Save();
                edit = false;
                OnClose();
            }

            public void SyncSetting()
            {
                var json = JsonUtility.ToJson(this);
                if (NetworkServer.active)
                    NetworkMessageManager.SendNetworkMessageToAll(LoadSettingFromJson, new StringMessage() { value = json });
            }

            public void CancelAndClose()
            {
                Helper.LoadJsonFile(this, filePath);
                dataEditor = new FieldEditor(this);
                edit = false;
                OnClose();
            }

            public virtual void OnGUIFunc()
            {
                dataEditor.OnGUI();
            }

            public virtual void OnRenderObjectFunc(Camera cam) { }

            protected virtual void OnLoad()
            {
            }

            protected virtual void OnClose()
            {
            }
        }
    }
}