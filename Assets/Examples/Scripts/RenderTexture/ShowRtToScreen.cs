using UnityEngine;
using System.Collections;

public class ShowRtToScreen : MonoBehaviour
{
	[SerializeField]
	RenderTexture output;
	public void SetTexture(RenderTexture rt) { output = rt; }

	void OnRenderImage(RenderTexture s, RenderTexture d)
	{
		if (output == null)
			Graphics.Blit(s, d);
		else
			Graphics.Blit(output, d);
	}
}
