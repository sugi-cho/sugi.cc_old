using UnityEngine;
using System.Collections;
using sugi.cc;

public class DrawBackground : MonoBehaviour
{
	public Material bgMat;
	public int pass = 0;
	public Color clearColor;
	public RenderTexture output;
	// Use this for initialization
	void Start ()
	{
		output = Helper.CreateRenderTexture (Screen.width, Screen.height, output);
	}

	void OnPostRender ()
	{
		Graphics.SetRenderTarget (output);
		GL.Clear (true, true, clearColor);
		bgMat.DrawFullscreenQuad (pass);
	}
}
