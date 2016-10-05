using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using System.Collections;

namespace sugi.cc
{
    public class ReconnectableNetworkManager : NetworkManager
    {
        public static ReconnectableNetworkManager Instance
        {
            get
            {
                if (_Instance == null) _Instance = FindObjectOfType<ReconnectableNetworkManager>();
                if (_Instance == null) _Instance = new GameObject("NetworkManager").AddComponent<ReconnectableNetworkManager>();
                return _Instance;
            }
        }
        static ReconnectableNetworkManager _Instance;

        [System.Serializable]
        public class ConnectEvent : UnityEvent<NetworkConnection> { }

        [SerializeField]
        Setting setting;
        [SerializeField]
        string settingFilePath = "Networking/setting.json";
        [SerializeField]
        string[] networkPrefabResourcePathes = new[] { "Networking/Prefabs" };
        public UnityEvent onStartServer;
        public UnityEvent onClientConnect;
        public ConnectEvent onServerConnect;

        void Start()
        {
            SettingManager.AddSettingMenu(setting, settingFilePath);
            this.Invoke(Connect, 1f);

        }

        public void Connect()
        {
            if (isNetworkActive)
                return;
            if (setting.isServer)
            {
                if (setting.isClient)
                    StartHost();
                else
                    StartServer();
            }
            else if (setting.isClient)
                StartClient();
            else
                this.Invoke(Connect, 1f);
        }

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            base.OnClientSceneChanged(conn);
            var nets = Resources.LoadAll<NetworkIdentity>("NetworkPrefabs");
            foreach (var net in nets)
                ClientScene.RegisterPrefab(net.gameObject);
        }
        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            Debug.LogFormat("error code {0}", errorCode);
            this.Invoke(Connect, 1f);
            base.OnClientError(conn, errorCode);
        }
        public override void OnClientDisconnect(NetworkConnection conn)
        {
            Debug.Log("disconnected");
            this.Invoke(Connect, 1f);
            base.OnClientDisconnect(conn);
        }


        public override void OnClientConnect(NetworkConnection conn)
        {
            foreach (var networkPrefabResourcePath in networkPrefabResourcePathes)
            {
                var netPrefabs = Resources.LoadAll<NetworkIdentity>(networkPrefabResourcePath);
                foreach (var netPrefab in netPrefabs)
                    ClientScene.RegisterPrefab(netPrefab.gameObject);
            }
            NetworkMessageManager.RegistorHandlerToClient();
            base.OnClientConnect(conn);
            onClientConnect.Invoke();
        }
        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
            onServerConnect.Invoke(conn);
        }
        public override void OnStartServer()
        {
            NetworkMessageManager.RegistorHandlerToServer();
            onStartServer.Invoke();
        }

        [System.Serializable]
        public class Setting : SettingManager.Setting
        {
            public bool isServer;
            public bool isClient;

            public string networkAddress = "localhost";
            public int networkPort = 7777;

            protected override void OnLoad()
            {
                base.OnLoad();
                singleton.networkAddress = networkAddress;
                singleton.networkPort = networkPort;
            }
            public override void OnGUIFunc()
            {
                base.OnGUIFunc();

                var noConnection = (singleton.client == null || singleton.client.connection == null || singleton.client.connection.connectionId == -1);

                if (!singleton.IsClientConnected() && !NetworkServer.active && singleton.matchMaker == null)
                {
                    if (noConnection)
                    {
                        if (GUILayout.Button("LAN Host"))
                            singleton.StartHost();
                        if (GUILayout.Button("LAN Client"))
                            singleton.StartClient();
                        if (GUILayout.Button("LAN Server Only"))
                            singleton.StartServer();
                    }
                    else
                    {
                        GUILayout.Label("Connecting to " + singleton.networkAddress + ":" + singleton.networkPort + "..");
                        if (GUILayout.Button("Cancel Connection Attempt"))
                            singleton.StopClient();
                    }
                }
                else
                {
                    if (NetworkServer.active)
                        GUILayout.Label("Server: port=" + singleton.networkPort);
                    if (NetworkClient.active)
                        GUILayout.Label("Client: address=" + singleton.networkAddress + " port=" + singleton.networkPort);
                }

                if (NetworkServer.active || NetworkClient.active)
                    if (GUILayout.Button("Stop"))
                        singleton.StopHost();
            }

            protected override void OnClose()
            {
                base.OnClose();
                singleton.networkAddress = networkAddress;
                singleton.networkPort = networkPort;
            }
        }
    }
}