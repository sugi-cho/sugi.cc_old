using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class DisplayTexture : MonoBehaviour
{

    public Texture showTex;
    public void SetTextureToProperty(RenderTexture tex) { showTex = tex; }

    void OnRenderImage(RenderTexture s, RenderTexture d)
    {
        if (showTex != null)
            Graphics.Blit(showTex, d);
        else
            Graphics.Blit(s, d);
    }
}
