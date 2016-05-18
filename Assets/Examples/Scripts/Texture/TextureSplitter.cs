using UnityEngine;
using System.Collections;
using sugi.cc;

public class TextureSplitter : MonoBehaviour
{

    public int horizonalSplitCount = 2;
    public int verticalSplitCount = 1;
    public RenderTextureEvent[] onUpdate;
    public Material splitter;

    [SerializeField]
    RenderTexture[] outputs;

    public RenderTextureFormat format;
    public FilterMode filterMode;
    public TextureWrapMode wrapMode;

    public void Split(Texture tex)
    {
        if (!IsValidOutputSize(tex))
            CreateOutputs(tex);
        if (splitter == null)
            splitter = new Material(Shader.Find("Hidden/TextureSplitter"));
        for (var y = 0; y < verticalSplitCount; y++)
            for (var x = 0; x < horizonalSplitCount; x++)
            {
                var i = y * horizonalSplitCount + x;
                var output = outputs[i];
                var prop = new Vector4(
                    (float)x / horizonalSplitCount,
                    (float)y / verticalSplitCount,
                    1f / horizonalSplitCount,
                    1f / verticalSplitCount
                );
                splitter.SetVector("_Prop", prop);
                Graphics.Blit(tex, output, splitter);
                if (i < onUpdate.Length)
                    onUpdate[i].Invoke(output);
            }
    }
    bool IsValidOutputSize(Texture tex)
    {
        var width = tex.width / horizonalSplitCount;
        var height = tex.height / verticalSplitCount;
        if (outputs == null || outputs.Length != horizonalSplitCount * verticalSplitCount)
            return false;
        foreach (var rt in outputs)
            if (rt.width != width || rt.height != height)
                return false;
        return true;

    }
    void CreateOutputs(Texture tex)
    {
        var numRts = horizonalSplitCount * verticalSplitCount;
        var width = tex.width / horizonalSplitCount;
        var height = tex.height / verticalSplitCount;
        horizonalSplitCount = Mathf.Max(1, horizonalSplitCount);
        verticalSplitCount = Mathf.Max(1, verticalSplitCount);
        if (outputs == null || outputs.Length != numRts)
            outputs = new RenderTexture[numRts];
        for (var y = 0; y < verticalSplitCount; y++)
            for (var x = 0; x < horizonalSplitCount; x++)
            {
                var i = y * horizonalSplitCount + x;
                outputs[i] = Helper.CreateRenderTexture(width, height, outputs[i], format);
                outputs[i].filterMode = filterMode;
                outputs[i].wrapMode = wrapMode;
                outputs[i].name = string.Format("{0}_({1},{2})", this, x, y);
            }
    }
}
