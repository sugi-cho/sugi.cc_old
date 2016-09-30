using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

namespace sugi.cc
{
    [System.Serializable]
    public class NetworkMessageEvent : UnityEvent<NetworkMessage> { }
    public class NetworkMessageManager : MonoBehaviour
    {
        public static NetworkMessageManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = FindObjectOfType<NetworkMessageManager>();
                    if (_Instance == null) _Instance = new GameObject("NetworkMessageManager").AddComponent<NetworkMessageManager>();
                    DontDestroyOnLoad(_Instance);
                }
                return _Instance;
            }
        }
        static NetworkMessageManager _Instance;

        public NetworkMessageEvent onNetworkMessage;
        Dictionary<short, NetworkMessageDelegate> handlerMap;

        [SerializeField, Header("For Test")]
        bool isTest;
        [SerializeField]
        NetworkIdentity testObj;


        public void SendMessageToServer(NetworkMessageDelegate handler, MessageBase msg)
        {
            var client = NetworkManager.singleton.client;
            var msgType = GetMsgType(handler);
            client.Send(msgType, msg);
        }


        // Use this for initialization
        void Start()
        {
            BuildHandlersFromUnityEvents();
            RegistorHandlerToServer();
            var reconnectableManager = FindObjectOfType<ReconnectableNetworkManager>();
            if (reconnectableManager != null)
                reconnectableManager.onClientConnect.AddListener(RegistorHandlersToClient);
        }
        void BuildHandlersFromUnityEvents()
        {
            handlerMap = Enumerable.Range(0, onNetworkMessage.GetPersistentEventCount())
                .ToDictionary(
                idx => (short)(MsgType.Highest + 1 + idx),
                idx =>
                {
                    var target = onNetworkMessage.GetPersistentTarget(idx);
                    var methodName = onNetworkMessage.GetPersistentMethodName(idx);
                    var handler = (NetworkMessageDelegate)System.Delegate.CreateDelegate(typeof(NetworkMessageDelegate), target, methodName);
                    return handler;
                }
            );
        }
        void RegistorHandlerToServer()
        {
            NetworkServer.ClearHandlers();
            foreach (var pair in handlerMap)
                if (!NetworkServer.handlers.ContainsKey(pair.Key))
                    NetworkServer.RegisterHandler(pair.Key, pair.Value);
        }
        void RegistorHandlersToClient()
        {
            var client = NetworkManager.singleton.client;
            foreach (var pair in handlerMap)
                if (!client.handlers.ContainsKey(pair.Key))
                    client.RegisterHandler(pair.Key, pair.Value);
        }
        #region sampleCode for SendNetworkMessage
        public void SendSample(NetworkMessage netMessage)
        {
            var msg = netMessage.ReadMessage<MessageSample>();
            Debug.Log(msg.sampleText);
            if (testObj != null && NetworkServer.active)
            {
                var go = Instantiate<NetworkIdentity>(testObj).gameObject;
                NetworkServer.Spawn(go);
                Debug.Log("spawn");
            }
        }
        class MessageSample : MessageBase
        {
            public string sampleText;
        }
        #endregion

        short GetMsgType(NetworkMessageDelegate handler)
        {
            if (!handlerMap.ContainsValue(handler))
                return -1;
            var msgType = handlerMap.Where(pair => pair.Value == handler).First().Key;
            return msgType;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0) && isTest)
                SendMessageToServer(SendSample, new MessageSample() { sampleText = "this is client" });
        }
    }
}