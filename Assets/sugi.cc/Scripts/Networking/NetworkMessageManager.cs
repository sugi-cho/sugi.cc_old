using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

namespace sugi.cc
{
    public class EmptyMessage : UnityEngine.Networking.NetworkSystem.EmptyMessage { }
    public class FloatMessage : MessageBase { public float value; }
    public class IntegerMessage : UnityEngine.Networking.NetworkSystem.IntegerMessage { }
    public class StringMessage : UnityEngine.Networking.NetworkSystem.StringMessage { }
    public class Vector3Message : MessageBase { public Vector3 value; }
    public class BoolMessage : MessageBase { public bool value; }

    /// <summary>
    /// use ReconnectableNetworkManager to Enable "onServerConnect", "onClientConnect" and "onStartServer" delegate.
    /// </summary>
    public class NetworkMessageManager : MonoBehaviour
    {
        public delegate void OnConnect(NetworkConnection conn);
        [System.Serializable]
        public class NetworkMessageEvent : UnityEngine.Events.UnityEvent<NetworkMessage> { }

        #region MonoBehaviourFunc
        public NetworkMessageEvent registeredHandlers;
        void Start()
        {
            var numHandlers = registeredHandlers.GetPersistentEventCount();
            for (var i = 0; i < numHandlers; i++)
            {
                var target = registeredHandlers.GetPersistentTarget(i);
                var methodName = registeredHandlers.GetPersistentMethodName(i);
                var handler = (NetworkMessageDelegate)System.Delegate.CreateDelegate(typeof(NetworkMessageDelegate), target, methodName);
                AddHandler(handler);
            }
        }
        #endregion

        public static OnConnect onServerConnect { get; set; }
        public static OnConnect onClientConnect { get; set; }
        public static System.Action onStartServer { get; set; }

        static NetworkClient client { get { return NetworkManager.singleton.client; } }
        static bool showInfo;

        static string GetIdentifier(NetworkMessageDelegate handler) { return handler.Target.ToString() + handler.Method.Name; }

        static List<NetworkMessageDelegate> handlerList;
        static Dictionary<short, NetworkMessageDelegate> handlerMap;

        public static void AddHandler(NetworkMessageDelegate handler)
        {
            if (handlerList == null) handlerList = new List<NetworkMessageDelegate>();
            if (!handlerList.Contains(handler)) handlerList.Add(handler);
        }

        public static void RegistorHandlerToServer()
        {
            if (handlerList == null) return;
            var msgType = MsgType.Highest;
            handlerMap = handlerList.OrderBy(handler => GetIdentifier(handler)).ToDictionary(b => ++msgType, b => b);
            foreach (var pair in handlerMap)
            {
                NetworkServer.RegisterHandler(pair.Key, pair.Value);
            }
            SettingManager.AddExtraGuiFunc(ShowNetworkMessageInfo);
        }
        public static void RegistorHandlerToClient()
        {
            if (handlerList == null) return;
            var msgType = MsgType.Highest;
            handlerMap = handlerList.OrderBy(handler => GetIdentifier(handler)).ToDictionary(b => ++msgType, b => b);
            foreach (var pair in handlerMap)
            {
                client.RegisterHandler(pair.Key, pair.Value);
            }
            SettingManager.AddExtraGuiFunc(ShowNetworkMessageInfo);
        }

        public static void SendNetworkMessage(NetworkMessageDelegate handler, MessageBase message)
        {
            SendNetworkMessage(client.connection, handler, message);
        }

        public static void SendNetworkMessage(NetworkConnection conn, NetworkMessageDelegate handler, MessageBase message)
        {
            var msgType = handlerMap.Where(b => b.Value == handler).FirstOrDefault().Key;
            conn.Send(msgType, message);
        }

        public static void SendNetworkMessageToAll(NetworkMessageDelegate handler, MessageBase message)
        {
            var msgType = handlerMap.Where(b => b.Value == handler).FirstOrDefault().Key;
            NetworkServer.SendToAll(msgType, message);
        }

        static void ShowNetworkMessageInfo()
        {
            GUILayout.BeginVertical("box");
            showInfo = GUILayout.Toggle(showInfo, "show registered NetworkMessage Info?");
            if (showInfo)
            {
                foreach (var pair in handlerMap)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(pair.Key.ToString("000") + ":");
                    GUILayout.Label(GetIdentifier(pair.Value));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }
    }
}