using UnityEngine;
using System.Collections;
using sugi.cc;

public class GaussianToRenderTexture : MonoBehaviour
{
	[Range(0, 5)]
	public int
		nIterations = 3,
		lod = 1;
	public bool createNewRt;
	[SerializeField]
	RenderTexture output;
	public RenderTextureEvent onCreate;

	public void ApplyGaussianFilter(RenderTexture rt)
	{
		CheckOutput(rt);
		rt.ApplyGaussianFilter(nIterations, lod, output);
	}

	void CheckOutput(RenderTexture s)
	{
		if (!createNewRt)
		{
			if (output != s)
			{
				Helper.ReleaseRenderTexture(output);
				output = s;
			}
			return;
		}
		output = output == s ? null : output;
		if (Helper.CheckRtSize(s, output))
		{
			output = Helper.CreateRenderTexture(s, output);
			output.name = name + ".gaussianToRenderTexture";
			onCreate.Invoke(output);
			TextureViewer.AddTexture(output);
		}
	}

	void OnDestroy()
	{
		Helper.ReleaseRenderTexture(output);
	}
}
