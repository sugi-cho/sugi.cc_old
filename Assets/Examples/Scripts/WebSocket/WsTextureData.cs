using UnityEngine;
using sugi.cc;

public class WsTextureData : WebSocketDataBehaviour<WsTextureData.Data>
{
    public Texture2D tex2d;
    public Texture2D recervedTex;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SendData(tex2d);
        lock (receivedData)
            while (0 < receivedData.Count)
            {
                var data = receivedData.Dequeue();
                var newTex = new Texture2D(data.texWidth, data.texheight);
                newTex.LoadImage(data.texData);
                recervedTex = newTex;
                data = null;
            }
    }

    public void SendData(Texture2D tex)
    {
        var data = new Data()
        {
            texWidth = tex.width,
            texheight = tex.height,
            texData = tex.EncodeToPNG(),
        };
        SendData(data);
    }

    [System.Serializable]
    public class Data
    {
        public int texWidth = 512;
        public int texheight = 512;
        public byte[] texData;
    }
}
