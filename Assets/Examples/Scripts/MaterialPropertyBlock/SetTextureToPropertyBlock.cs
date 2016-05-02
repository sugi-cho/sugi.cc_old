using UnityEngine;
using System.Collections;
using sugi.cc;

public class SetTextureToPropertyBlock : MonoBehaviour
{
	public MaterialPropertySetter.TextureProperty[] properties;
	public string propertyName = "_MainTex";
	Renderer[] renderers { get { if (_r == null) _r = GetComponentsInChildren<Renderer>(); return _r; } }
	Renderer[] _r;


	public void SetTexture(Texture tex)
	{
		foreach (var r in renderers)
			r.SetTexture(propertyName, tex);
	}

	public void SetProperty(MaterialPropertySetter.TextureProperty property)
	{
		foreach (var r in renderers)
			r.SetTexture(property.name, property.val);
	}
}
