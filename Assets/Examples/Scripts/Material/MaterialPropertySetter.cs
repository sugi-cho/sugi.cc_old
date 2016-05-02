using UnityEngine;
using System.Collections;
using System.Linq;
using sugi.cc;
using UnityEngine.Events;

public class MaterialPropertySetter : MonoBehaviour
{
	public Material[] materials;
	public Renderer[] renderers;
	public bool toGlobal, toLocal;

	public BufferProperty[] bufferProperties;
	public ColorProperty[] colorProperties;
	public FloatProperty[] floatProperties;
	public MatrixProperty[] matrixProperties;
	public TextureProperty[] textureProperties;
	public VectorProperty[] vectorProperties;

	public void SetProperty(BufferProperty property)
	{
		foreach (var mat in materials)
			mat.SetBuffer(property.name, property.val);
		if (toGlobal)
			Shader.SetGlobalBuffer(property.name, property.val);
	}
	public void SetProperty(ColorProperty property)
	{
		foreach (var mat in materials)
			mat.SetColor(property.name, property.val);
		foreach (var r in renderers)
			r.SetColor(property.name, property.val);
		if (toGlobal)
			Shader.SetGlobalColor(property.name, property.val);
	}
	public void SetProperty(FloatProperty property)
	{
		foreach (var mat in materials)
			mat.SetFloat(property.name, property.val);
		foreach (var r in renderers)
			r.SetFloat(property.name, property.val);
		if (toGlobal)
			Shader.SetGlobalFloat(property.name, property.val);
	}
	public void SetProperty(MatrixProperty property)
	{
		foreach (var mat in materials)
			mat.SetMatrix(property.name, property.val);
		foreach (var r in renderers)
			r.SetMatrix(property.name, property.val);
		if (toGlobal)
			Shader.SetGlobalMatrix(property.name, property.val);
	}
	public void SetProperty(TextureProperty property)
	{
		foreach (var mat in materials)
			mat.SetTexture(property.name, property.val);
		foreach (var r in renderers)
			r.SetTexture(property.name, property.val);
		if (toGlobal)
			Shader.SetGlobalTexture(property.name, property.val);
	}
	public void SetProperty(VectorProperty property)
	{
		foreach (var mat in materials)
			mat.SetVector(property.name, property.val);
		foreach (var r in renderers)
			r.SetVector(property.name, property.val);
		if (toGlobal)
			Shader.SetGlobalVector(property.name, property.val);
	}

	void Start()
	{
		var rs = GetComponentsInChildren<Renderer>();
		if (toLocal)
			renderers = Helper.MargeArray(renderers, rs);

		bufferProperties.ToList().ForEach(b => { SetProperty(b); });
		colorProperties.ToList().ForEach(b => { SetProperty(b); });
		floatProperties.ToList().ForEach(b => { SetProperty(b); });
		matrixProperties.ToList().ForEach(b => { SetProperty(b); });
		textureProperties.ToList().ForEach(b => { SetProperty(b); });
		vectorProperties.ToList().ForEach(b => { SetProperty(b); });
	}

	public abstract class PropertyBase
	{
		public string name = "_Property";
	}

	[System.Serializable]
	public class BufferProperty : PropertyBase
	{
		public ComputeBuffer val;
	}
	[System.Serializable]
	public class ColorProperty : PropertyBase
	{
		public Color val;
	}
	[System.Serializable]
	public class FloatProperty : PropertyBase
	{
		public float val;
	}
	[System.Serializable]
	public class MatrixProperty : PropertyBase
	{
		public Matrix4x4 val;
	}
	[System.Serializable]
	public class TextureProperty : PropertyBase
	{
		public Texture val;
	}
	[System.Serializable]
	public class VectorProperty : PropertyBase
	{
		public Vector4 val;
	}

	[System.Serializable]
	public class BufferPropertyEvent : UnityEvent<BufferProperty> { }
	[System.Serializable]
	public class ColorPropertyEvent : UnityEvent<ColorProperty> { }
	[System.Serializable]
	public class FloatPropertyEvent : UnityEvent<FloatProperty> { }
	[System.Serializable]
	public class MatrixPropertyEvent : UnityEvent<MatrixProperty> { }
	[System.Serializable]
	public class TexturePropertyEvent : UnityEvent<TextureProperty> { }
	[System.Serializable]
	public class VectorPropertyEvent : UnityEvent<VectorProperty> { }
}
