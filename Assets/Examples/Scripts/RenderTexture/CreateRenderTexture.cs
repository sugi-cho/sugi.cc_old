using UnityEngine;
using System.Collections;
using sugi.cc;

public class CreateRenderTexture : MonoBehaviour
{
	public bool useCameraSize;
	public int width = 512;
	public int height = 512;
	public int depth = 16;
	public RenderTextureFormat format = RenderTextureFormat.ARGBFloat;
	public RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
	public TextureWrapMode wrapMode = TextureWrapMode.Repeat;
	public FilterMode filterMode = FilterMode.Bilinear;
	public Color clearColor;
	public Material[] initializeMats;
	[SerializeField]
	RenderTexture output;

	public RenderTextureEvent onCreate;

	// Use this for initialization
	void Start()
	{
		var c = GetComponent<Camera>();
		if (useCameraSize && c != null)
		{
			width = c.pixelWidth;
			height = c.pixelHeight;
		}
		output = new RenderTexture(width, height, depth, format, readWrite);
		output.wrapMode = wrapMode;
		output.filterMode = filterMode;
		Graphics.SetRenderTarget(output);
		GL.Clear(true, true, clearColor);
		foreach (var mat in initializeMats)
			Graphics.Blit(null, output, mat);
		output.name = name + ".createRenderTexture";
		onCreate.Invoke(output);
		TextureViewer.AddTexture(output);
	}
}
