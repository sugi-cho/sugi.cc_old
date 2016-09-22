using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Linq;
using Osc;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;


namespace sugi.cc
{
    public class OscController : OscPort
    {
        public static OscController Instance { get { if (_Instance == null) _Instance = FindObjectOfType<OscController>(); return _Instance; } }
        static OscController _Instance;

        public bool dontDestroyOnLoad;
        public PathEventPair[] oscEvents;

        Dictionary<string, UnityAction<Capsule>> oscActionMap;
        Dictionary<string, LinkedList<string>> oscSendPathInfoMap;

        [SerializeField]
        List<OscCallback> OscCallbackList;
        bool showReserve;
        bool showSend;

        Socket _udp;
        byte[] _receiveBuffer;
        Thread _reader;

        public delegate void OscDelegate(object[] data);


        public void AddCallbacks(Object target)
        {
            target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)
                 .Select(b => new { attrs = OscAttribute.GetCustomAttributes(b, typeof(OscAttribute)), method = b })
                 .Where(b => 0 < b.attrs.Length)
                 .ToList().ForEach(b =>
                 {
                     foreach (OscAttribute attr in b.attrs)
                     {
                         var path = attr.oscPath;
                         var callback = new OscCallback(path, target, b.method.Name);
                         AddCallback(path, callback);
                     }
                 });
        }
        public void AddCallback(string path, OscDelegate oscDelegate)
        {
            var oscCalllback = new OscCallback(path, oscDelegate);
            AddCallback(path, oscCalllback);

        }
        void AddCallback(string path, OscCallback callback)
        {
            if (oscActionMap == null)
                oscActionMap = new Dictionary<string, UnityAction<Capsule>>();
            if (OscCallbackList == null)
                OscCallbackList = new List<OscCallback>();

            if (oscActionMap.ContainsKey(path))
                oscActionMap[path] += callback.Invoke;
            else
                oscActionMap[path] = callback.Invoke;

            if (!OscCallbackList.Contains(callback))
            {
                OscCallbackList.Add(callback);
                OscCallbackList = OscCallbackList.OrderBy(b => b.oscPath).ToList();
            }
        }

        protected override void OnEnable()
        {
            var setting = new Setting()
            {
                localPort = this.localPort,
                defaultRemoteHost = this.defaultRemoteHost,
                defaultRemotePort = this.defaultRemotePort,
                limitReceiveBiuffer = this.limitReceiveBuffer
            };
            SettingManager.AddSettingMenu(setting, "OscControll/setting.json");
            SettingManager.AddExtraGuiFunc(ShowReceivedOscOnGUI);
            foreach (var oscEvent in oscEvents)
            {
                var path = oscEvent.path;
                var onOsc = oscEvent.onOsc;
                for (var i = 0; i < onOsc.GetPersistentEventCount(); i++)
                {
                    var target = onOsc.GetPersistentTarget(i);
                    var method = onOsc.GetPersistentMethodName(i);
                    var callback = new OscCallback(path, target, method);
                    AddCallback(path, callback);
                }
            }

            try
            {
                base.OnEnable();

                _udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _udp.Bind(new IPEndPoint(IPAddress.Any, localPort));

                _receiveBuffer = new byte[BUFFER_SIZE];

                _reader = new Thread(Reader);
                _reader.Start();
                if (dontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);
            }
            catch (System.Exception e)
            {
                RaiseError(e);
                enabled = false;
            }

        }
        protected override void OnDisable()
        {
            if (_udp != null)
            {
                _udp.Close();
                _udp = null;
            }
            if (_reader != null)
            {
                _reader.Abort();
                _reader = null;
            }

            base.OnDisable();
        }

        protected override void Update()
        {
            lock (_received)
                while (_received.Count > 0)
                {
                    var c = _received.Dequeue();
                    OnReceive.Invoke(c);
                    if (oscActionMap.ContainsKey(c.message.path))
                        oscActionMap[c.message.path].Invoke(c);
                }
        }

        public new void Send(MessageEncoder oscMessage, IPEndPoint remote)
        {
            Send(oscMessage.Encode(), remote);
            AddOscSendData(oscMessage, remote);
        }
        void AddOscSendData(MessageEncoder oscMessage, IPEndPoint remote)
        {
            var path = oscMessage.path;
            if (oscSendPathInfoMap == null)
                oscSendPathInfoMap = new Dictionary<string, LinkedList<string>>();
            LinkedList<string> sendInfoList;
            if (oscSendPathInfoMap.ContainsKey(path))
                sendInfoList = oscSendPathInfoMap[path];
            else
            {
                sendInfoList = new LinkedList<string>();
                oscSendPathInfoMap.Add(path, sendInfoList);
            }
            var infoText = remote.ToString() + " { ";
            foreach (var o in oscMessage.rawdata)
                infoText += o + ", ";
            infoText += "} ";
            sendInfoList.AddLast(infoText);
            while (5 < sendInfoList.Count)
                sendInfoList.RemoveFirst();
        }
        public override void Send(byte[] oscData, IPEndPoint remote)
        {
            try
            {
                _udp.SendTo(oscData, remote);
            }
            catch (System.Exception e)
            {
                RaiseError(e);
            }
        }

        void Reader()
        {
            while (_udp != null)
            {
                try
                {
                    var clientEndpoint = new IPEndPoint(IPAddress.Any, 0);
                    var fromendpoint = (EndPoint)clientEndpoint;
                    var length = _udp.ReceiveFrom(_receiveBuffer, ref fromendpoint);
                    if (length == 0 || (clientEndpoint = fromendpoint as IPEndPoint) == null)
                        continue;

                    _oscParser.FeedData(_receiveBuffer, length);
                    while (_oscParser.MessageCount > 0)
                    {
                        lock (_received)
                        {
                            var msg = _oscParser.PopMessage();
                            Receive(new Capsule(msg, clientEndpoint));
                        }
                    }
                }
                catch (System.Exception e)
                {
                    RaiseError(e);
                }
            }
        }

        [System.Serializable]
        public class OscMessageEvent : UnityEvent<object[]> { }
        [System.Serializable]
        public struct PathEventPair
        {
            public string path;
            public OscMessageEvent onOsc;
        }

        void ShowReceivedOscOnGUI()
        {
            showReserve = GUILayout.Toggle(showReserve, "show reserved osc info?");
            if (showReserve)
            {
                GUILayout.BeginVertical(SettingManager.BoxStyle);
                foreach (var callback in OscCallbackList)
                    callback.ShowInfoGUI();
                GUILayout.EndVertical();
            }
            showSend = GUILayout.Toggle(showSend, "show send osc info?");
            if (showSend)
            {
                GUILayout.BeginVertical(SettingManager.BoxStyle);
                foreach (var pair in oscSendPathInfoMap)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(pair.Key);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    foreach (var infoText in pair.Value)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(infoText);
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndVertical();
            }
        }

        [System.Serializable]
        public class OscCallback
        {
            public string oscPath;
            OscDelegate oscDelegate;
            int invokeCount = 0;
            bool showLatestData;

            LinkedList<Capsule> latestOscList;

            public OscCallback(string path, Object target, string method)
            {
                oscPath = path;
                oscDelegate = (OscDelegate)OscDelegate.CreateDelegate(typeof(OscDelegate), target, method);
                latestOscList = new LinkedList<Capsule>();
            }
            public OscCallback(string path, OscDelegate method)
            {
                oscPath = path;
                oscDelegate = method;
                latestOscList = new LinkedList<Capsule>();
            }
            public void Invoke(Capsule c)
            {
                oscDelegate(c.message.data);
                invokeCount++;
                latestOscList.AddLast(c);
                while (10 < latestOscList.Count)
                    latestOscList.RemoveFirst();
            }
            public void ShowInfoGUI()
            {
                GUILayout.BeginHorizontal();
                showLatestData = GUILayout.Toggle(showLatestData, oscPath);
                GUILayout.FlexibleSpace();
                GUILayout.Label(string.Format("{0}.{1}", oscDelegate.Target.ToString(), oscDelegate.Method.ToString()));
                GUILayout.FlexibleSpace();
                GUILayout.Label(invokeCount.ToString());
                GUILayout.EndHorizontal();

                if (!showLatestData) return;
                foreach (var c in latestOscList)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(string.Format("{0}:{1}", c.ip.ToString(), c.message.ToString()));
                    GUILayout.EndHorizontal();
                }
            }
        }

        [System.Serializable]
        public class Setting : SettingManager.Setting
        {
            public int localPort = 0;
            public string defaultRemoteHost = "localhost";
            public int defaultRemotePort = 10000;
            public int limitReceiveBiuffer = 10;

            protected override void OnLoad()
            {
                Instance.localPort = localPort;
                Instance.defaultRemoteHost = defaultRemoteHost;
                Instance.defaultRemotePort = defaultRemotePort;
                Instance.limitReceiveBuffer = limitReceiveBiuffer;
            }
            public override void OnGUIFunc()
            {
                base.OnGUIFunc();
                GUI.contentColor = Color.red;
                GUILayout.Label("need restart to apply setting!!");
                GUI.contentColor = Color.white;
            }
        }
    }
    public class OscAttribute : System.Attribute
    {
        public string oscPath;
        public OscAttribute(string path)
        {
            oscPath = path;
        }
    }
}