using UnityEngine;
using System.Collections;
using sugi.cc;

public class CreateQuadWarpTexture : MonoBehaviour
{
    public Color backGroundColor;
    public RenderTextureFormat format = RenderTextureFormat.ARGBFloat;
    public Vector2 bottomLeft;
    public Vector2 bottomRight;
    public Vector2 upperLeft;
    public Vector2 upperRight;
    public float supperSize = 1f;
    [SerializeField]
    RenderTexture output;

    Material warper { get { if (_warper == null) _warper = new Material(Shader.Find("Hidden/QuadWarp")); return _warper; } }
    Material _warper;

    void OnDestroy()
    {
        Helper.ReleaseRenderTexture(output);
    }

    public void WarpTexture(Texture tex)
    {
        if (!IsValidOutputSize(tex))
            CreateOutput(tex);
        Graphics.SetRenderTarget(output);
        GL.Clear(true, true, backGroundColor);

        warper.SetTexture("_MainTex", tex);
        warper.SetVector("_Prop1", new Vector4(bottomLeft.x, bottomLeft.y, bottomRight.x, bottomRight.y));
        warper.SetVector("_Prop2", new Vector4(upperLeft.x, upperLeft.y, upperRight.x, upperRight.y));
        warper.DrawFullScreenQuadNxN();
    }
    bool IsValidOutputSize(Texture tex)
    {
        var width = Mathf.FloorToInt(tex.width * supperSize);
        var height = Mathf.FloorToInt(tex.height * supperSize);
        return output != null && output.width == width && output.height == height;
    }
    void CreateOutput(Texture tex)
    {
        var width = Mathf.FloorToInt(tex.width * supperSize);
        var height = Mathf.FloorToInt(tex.height * supperSize);
        output = Helper.CreateRenderTexture(width, height, output, format);
        output.name = string.Format("{0}.CreateOutput", this);
    }
}
