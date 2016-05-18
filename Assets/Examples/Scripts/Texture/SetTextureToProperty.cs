using UnityEngine;
using System.Collections;
using sugi.cc;

public class SetTextureToProperty : MonoBehaviour
{
    public bool
        toGlobal;
    public Material[] targetMats;
    public Renderer[] targetRenderers;
    public string propertyName = "_Tex";
    public void SetTexture(Texture tex)
    {
        if (toGlobal)
            Shader.SetGlobalTexture(propertyName, tex);
        foreach (var mat in targetMats)
            mat.SetTexture(propertyName, tex);
        foreach (var r in targetRenderers)
            r.SetTexture(propertyName, tex);
    }
}
