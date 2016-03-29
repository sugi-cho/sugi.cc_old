using UnityEngine;
using sugi.cc;

public class RenderEffect : MonoBehaviour
{
	public string propName = "_PropName";
	public Material[] effects;
	public bool 
		show = true,
		blur = false;
	[Range (0, 5)]
	public int
		nIterations = 3,
		lod = 1;
	public TextureWrapMode wrapMode;
	public int downSample = 0;

	public RenderTexture
		output;
	RenderTexture[]
		rts = new RenderTexture[2];

	void OnRenderImage (RenderTexture s, RenderTexture d)
	{
		CheckRTs (s);
		Graphics.Blit (s, rts [0]);
		foreach (var m in effects) {
			Graphics.Blit (rts [0], rts [1], m);
			SwapRTs ();
		}

		Graphics.Blit (rts [0], output);
		if (blur)
			output.ApplyGaussianFilter (nIterations, lod);
		Shader.SetGlobalTexture (propName, output);
		if (show)
			Graphics.Blit (output, d);
		else
			Graphics.Blit (s, d);
	}

	void CheckRTs (RenderTexture s)
	{
		if (rts [0] == null || rts [0].width != s.width >> downSample || rts [0].height != s.height >> downSample) {
			for (var i = 0; i < rts.Length; i++) {
				var rt = rts [i];
				rts [i] = Helper.CreateRenderTexture (s, rt, downSample);
				rts [i].wrapMode = wrapMode;
			}
			output = Helper.CreateRenderTexture (s, output, downSample);
			output.wrapMode = wrapMode;
		}
	}

	void SwapRTs ()
	{
		var tmp = rts [0];
		rts [0] = rts [1];
		rts [1] = tmp;
	}

	void OnDisabled ()
	{
		foreach (var rt in rts)
			Helper.ReleaseRenderTexture (rt);
		Helper.ReleaseRenderTexture (output);
	}
}

