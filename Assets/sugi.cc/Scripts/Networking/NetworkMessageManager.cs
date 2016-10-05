using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

namespace sugi.cc
{
    public class NetworkMessageManager : MonoBehaviour
    {
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
            handlerMap = handlerList.OrderBy(handler => GetIdentifier(handler)).ToDictionary(b => msgType++, b => b);
            foreach (var pair in handlerMap)
            {
                NetworkMessageDelegate handler = (NetworkMessage netMsg) =>
                {
                    pair.Value(netMsg);
                };
                NetworkServer.RegisterHandler(pair.Key, handler);
            }
            SettingManager.AddExtraGuiFunc(ShowNetworkMessageInfo);
        }
        public static void RegistorHandlerToClient()
        {
            if (handlerList == null) return;
            var msgType = MsgType.Highest;
            handlerMap = handlerList.OrderBy(handler => GetIdentifier(handler)).ToDictionary(b => msgType++, b => b);
            foreach (var pair in handlerMap)
            {
                NetworkMessageDelegate handler = (NetworkMessage netMsg) => { pair.Value(netMsg); };
                client.RegisterHandler(pair.Key, handler);
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
            GUILayout.BeginVertical(SettingManager.BoxStyle);
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