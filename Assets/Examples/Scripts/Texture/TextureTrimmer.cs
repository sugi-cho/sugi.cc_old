using UnityEngine;
using System.Collections;
using sugi.cc;

public class TextureTrimmer : MonoBehaviour
{
    public Vector4 uvTrimOp;//uv(minXY,maxXY)
    public Material drawMat;
    public float sizeFactor = 1f;
    public RenderTextureFormat format;
    public Color clearColor;
    public int numDivisions = 10;
    [SerializeField]
    RenderTexture output;

    public RenderTextureEvent onUpdate;

    public void TrimTexture(Texture tex)
    {
        if (!IsValidOutputSize(tex))
            CreateOutput(tex);
        Graphics.SetRenderTarget(output);
        GL.Clear(true, true, clearColor);

        drawMat.SetTexture("_Tex", tex);
        drawMat.DrawFullScreenQuadNxN(numDivisions, uvTrimOp.x, uvTrimOp.y, uvTrimOp.z, uvTrimOp.w);

        onUpdate.Invoke(output);
    }

    bool IsValidOutputSize(Texture tex)
    {
        var width = Mathf.FloorToInt(tex.width * sizeFactor);
        var height = Mathf.FloorToInt(tex.height * sizeFactor);
        return output != null && output.width == width && output.height == height;
    }
    void CreateOutput(Texture tex)
    {
        var width = Mathf.FloorToInt(tex.width * sizeFactor);
        var height = Mathf.FloorToInt(tex.height * sizeFactor);
        output = Helper.CreateRenderTexture(width, height, output, format);
        output.name = string.Format("{0}.CreateOutput", this);
    }
}
