using UnityEngine;
using System.Collections;
using sugi.cc;

public class DrawTextureToRenderTexture : MonoBehaviour
{
	public Texture brushTex;
	public Material drawMat;
	[SerializeField]
	RenderTexture output;

	public void SetTexture(RenderTexture rt) { output = rt; }
	public void Draw(Vector2 uv, float radius)
	{
		if (output == null) return;
		output.DrawTexture(uv, radius, brushTex, drawMat);
	}
}
