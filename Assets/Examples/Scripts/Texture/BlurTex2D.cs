using UnityEngine;
using sugi.cc;

public class BlurTex2D : MonoBehaviour {

    public Texture2D tex2d;
    public RenderTexture output;
    public int itr = 3;
    public int ds = 1;

    public TextureEvent onCreateTex;

    int pItr;
    int pDs;

	// Use this for initialization
	void Start () {
        output = Helper.CreateRenderTexture(tex2d.width, tex2d.height, output, RenderTextureFormat.ARGB32);
        onCreateTex.Invoke(output);
        ApplyGaussianFilter();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (pItr != itr || pDs != ds)
            ApplyGaussianFilter();
    }

    void ApplyGaussianFilter()
    {
        Graphics.Blit(tex2d, output);
        output.ApplyGaussianFilter(itr, ds);
        pItr = itr;
        pDs = ds;
    }
}
