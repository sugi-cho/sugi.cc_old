using UnityEngine;
using System.Collections;
using sugi.cc;
using System.Reflection;

public class Test : MonoBehaviour
{
    [Test(2)]
    public Material drawMat;
    public RenderTexture rt;

    Camera cam { get { if (_cam == null) _cam = GetComponent<Camera>(); return _cam; } }
    Camera _cam;
    void Start()
    {
        var fis = GetType().GetFields();
        foreach (var fi in fis)
        {
            var attr = System.Attribute.GetCustomAttribute(fi, typeof(TestAttribute)) as TestAttribute;
        }
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
