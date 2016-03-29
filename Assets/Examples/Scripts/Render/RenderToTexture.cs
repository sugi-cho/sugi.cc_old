using UnityEngine;
using sugi.cc;

public class RenderToTexture : MonoBehaviour
{

	public int texSize = 256;
	public string propName = "_PropName";
	public Material initEffect;
	public Material[] effects;
	public bool 
		show = true,
		blur = false;
	[Range (0, 5)]
	public int
		nIterations = 3,
		lod = 1;
	public TextureWrapMode wrapMode;

	[SerializeField]
	RenderTexture[]
		rts = new RenderTexture[2];

	void Start ()
	{
		CreateRTs ();
		if (initEffect != null)
			BlitEffect (initEffect);
	}

	void Update ()
	{
		foreach (var effect in effects)
			BlitEffect (effect);
		
		var output = rts [0];
		if (blur)
			output.ApplyGaussianFilter (nIterations, lod);
		Shader.SetGlobalTexture (propName, output);
	}

	void BlitEffect (Material effect)
	{
		Graphics.Blit (rts [0], rts [1], effect);
		SwapRTs ();
	}

	void CreateRTs ()
	{
		if (rts [0] == null) {
			for (var i = 0; i < rts.Length; i++) {
				rts [i] = Helper.CreateRenderTexture (texSize, texSize, rts [i]);
				rts [i].wrapMode = wrapMode;
			}
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
	}
}
