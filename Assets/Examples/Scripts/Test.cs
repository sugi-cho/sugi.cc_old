using UnityEngine;
using System.Collections;
using sugi.cc;

public class Test : MonoBehaviour
{
    public Material drawMat;
    public RenderTexture rt;

    Camera cam { get { if (_cam == null) _cam = GetComponent<Camera>(); return _cam; } }
    Camera _cam;
    void Start()
    {
        rt = new RenderTexture(512, 512, 24);
    }

    void Update()
    {
        cam.targetTexture = rt;
        RenderTexture.active = rt;
        drawMat.DrawFullscreenQuad();
        cam.targetTexture = null;
    }

    void OnPostRender()
    {
        cam.targetTexture = rt;
        drawMat.DrawFullScreenQuadNxN();
        cam.targetTexture = null;
    }
}
