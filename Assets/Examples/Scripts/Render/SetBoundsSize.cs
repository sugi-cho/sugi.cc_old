using UnityEngine;
using System.Collections;

public class SetBoundsSize : MonoBehaviour
{
	public Bounds bounds;
	Mesh mesh;
	// Use this for initialization
	void Start ()
	{
		mesh = GetComponent<MeshFilter> ().mesh;
		mesh.bounds = bounds;
	}
	void OnDestroy ()
	{
		if (mesh != null)
			Destroy (mesh);
	}
}
