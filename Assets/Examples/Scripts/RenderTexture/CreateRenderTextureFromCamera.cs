using UnityEngine;
using System.Collections;
using sugi.cc;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Camera))]
public class CreateRenderTextureFromCamera : MonoBehaviour
{
	public RenderTextureProps camProperty;
	public Shader replacementShader;
	public Material postEffectMat;

	[SerializeField]
	private RenderTexture output;
	public RenderTextureEvent onCreate;
	public RenderTextureEvent onUpdate;

	// Use this for initialization
	void Start()
	{
		var cam = GetComponent<Camera>();
		cam.targetTexture = CreateOutput(cam);
		if (replacementShader != null)
			cam.SetReplacementShader(replacementShader, null);
	}

	void OnRenderImage(RenderTexture s, RenderTexture d)
	{
		if (postEffectMat == null)
			Graphics.Blit(s, d);
		else
			Graphics.Blit(s, d, postEffectMat);
	}

	RenderTexture CreateOutput(Camera cam)
	{
		cam.depthTextureMode = camProperty.depthMode;
		cam.depth = camProperty.depth;
		var width = camProperty.overrideTextureSize ? camProperty.width : cam.pixelWidth;
		var height = camProperty.overrideTextureSize ? camProperty.height : cam.pixelHeight;
		output = new RenderTexture(width, height, camProperty.depth, camProperty.format);
		output.filterMode = camProperty.filterMode;
		output.wrapMode = camProperty.wrapMode;
		output.antiAliasing = camProperty.antiAliasing;
		onCreate.Invoke(output);
		return output;
	}

	[System.Serializable]
	public class RenderTextureProps
	{
		public bool overrideTextureSize;
		public int width = 512;
		public int height = 512;
		public int depth = 24;
		public RenderTextureFormat format;
		public FilterMode filterMode = FilterMode.Bilinear;
		public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
		public int antiAliasing = 1;
		public DepthTextureMode depthMode = DepthTextureMode.DepthNormals;
	}
}
