using UnityEngine;
using System.Collections;
using sugi.cc;

public class TestGaussian : MonoBehaviour
{

	public Texture tex;
	[Range (0, 5)]
	public int nIterations = 3;
	[Range (0, 5)]
	public int lod = 1;
	[SerializeField]
	RenderTexture rt;

	// Use this for initialization
	void Start ()
	{
		rt = Helper.CreateRenderTexture (tex.width, tex.height);
	}

	void Update ()
	{
		Graphics.Blit (tex, rt);
		rt.ApplyGaussianFilter (nIterations, lod);
	}
}
