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
        public ConnectEvent onClientConnect;
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
            base.OnClientConnect(conn);
            foreach (var networkPrefabResourcePath in networkPrefabResourcePathes)
            {
                var netPrefabs = Resources.LoadAll<NetworkIdentity>(networkPrefabResourcePath);
                foreach (var netPrefab in netPrefabs)
                    ClientScene.RegisterPrefab(netPrefab.gameObject);
            }
            NetworkMessageManager.RegistorHandlerToClient();
            if (NetworkMessageManager.onClientConnect != null)
                NetworkMessageManager.onClientConnect.Invoke(conn);
            onClientConnect.Invoke(conn);
        }
        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
            if (NetworkMessageManager.onServerConnect != null)
                NetworkMessageManager.onServerConnect.Invoke(conn);
            onServerConnect.Invoke(conn);
        }
        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkMessageManager.RegistorHandlerToServer();
            if (NetworkMessageManager.onStartServer != null)
                NetworkMessageManager.onStartServer.Invoke();
            onStartServer.Invoke();
        }

        [System.Serializable]
        public class Setting : SettingManager.Setting
        {
            public bool isServer;
            public bool isClient;

            public string networkAddress = "localhost";
            public int networkPort = 7777;

            bool showConnections;

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

                if (NetworkServer.active)
                {
                    GUILayout.BeginVertical("box");
                    if (showConnections = GUILayout.Toggle(showConnections, "show connections"))
                    {
                        foreach (var conn in NetworkServer.connections)
                        {
                            if (conn == null)
                            {
                                var tmp = GUI.contentColor;
                                GUI.contentColor = Color.red;
                                GUILayout.Label(string.Format("connection.#{0} is null", NetworkServer.connections.IndexOf(conn).ToString("00")));
                                GUI.contentColor = tmp;
                            }
                            else
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("address:" + conn.address);
                                GUILayout.Label("connection ID:" + conn.connectionId);
                                GUILayout.Label("isConnected:" + conn.isConnected);
                                GUILayout.EndHorizontal();
                            }
                        }
                    }
                    GUILayout.EndVertical();
                }
                else if (singleton.client != null && singleton.client.connection != null)
                {
                    GUILayout.BeginVertical("box");
                    if (showConnections = GUILayout.Toggle(showConnections, "show connection"))
                    {
                        var conn = singleton.client.connection;
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("address:" + conn.address);
                        GUILayout.Label("connection ID:" + conn.connectionId);
                        GUILayout.Label("isConnected:" + conn.isConnected);
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
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