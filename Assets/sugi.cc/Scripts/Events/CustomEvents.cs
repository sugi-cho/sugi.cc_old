using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace sugi.cc
{
	[System.Serializable]
	public class IntEvent : UnityEvent<int> { }
	[System.Serializable]
	public class FloatEvent : UnityEvent<float> { }
	[System.Serializable]
	public class TextureEvent : UnityEvent<Texture> { }
	[System.Serializable]
	public class RenderTextureEvent : UnityEvent<RenderTexture> { }
	[System.Serializable]
	public class Matrix4x4Event : UnityEvent<Matrix4x4> { }
}