using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using sugi.cc;

public class DrawEffect : MonoBehaviour
{
	public Material mat;

	public Texture bTex;
	public Texture sTex;
	public bool debug;

	public RenderTexture targetTex;

	void Update ()
	{
		if (!Input.GetMouseButton (0) || !debug)
			return;
		var pos = Input.mousePosition;
		pos.x /= Screen.width;
		pos.y /= Screen.height;
		Draw (pos, Color.white, 0.3f, bTex, sTex);
	}

	public void Draw (Vector2 uv, Color color, float radius, Texture brush = null, Texture colorSampler = null)
	{
		uv.x = uv.x % 1.0f;
		uv.y = uv.y % 1.0f;

		if (targetTex == null)
			return;
		
		mat.SetTexture ("_MainTex", brush);
		mat.SetTexture ("_ScreenTex", colorSampler);
		mat.SetColor ("_Color", color);

		targetTex.DrawTexture (uv, radius, brush, mat);
	}
}
