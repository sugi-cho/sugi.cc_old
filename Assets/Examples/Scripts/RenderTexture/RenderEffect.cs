using UnityEngine;
using sugi.cc;

public class RenderEffect : MonoBehaviour
{
	public Material[] effects;
	public bool show = true;
	public TextureWrapMode wrapMode;
	public int downSample = 0;

	[SerializeField]
	RenderTexture output;
	RenderTexture[] rts = new RenderTexture[2];
	public RenderTextureEvent onCreate;
	public RenderTextureEvent onUpdate;

	void OnRenderImage(RenderTexture s, RenderTexture d)
	{
		if (Helper.CheckRtSize(s, output))
		{
			output = Helper.CreateRenderTexture(s, output, downSample);
			rts = Helper.CreateRts(output, rts, downSample);
			output.name = name + ".onRenderImage";
			onCreate.Invoke(output);
			TextureViewer.AddTexture(output);
		}
		Graphics.Blit(s, rts[0]);
		foreach (var m in effects)
		{
			Graphics.Blit(rts[0], rts[1], m);
			rts.Swap();
		}

		Graphics.Blit(rts[0], output);
		onUpdate.Invoke(output);
		if (show)
			Graphics.Blit(output, d);
		else
			Graphics.Blit(s, d);
	}

	void OnDestroy()
	{
		foreach (var rt in rts)
			Helper.ReleaseRenderTexture(rt);
		Helper.ReleaseRenderTexture(output);
	}
}

