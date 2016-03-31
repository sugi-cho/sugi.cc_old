using UnityEngine;
using UnityEngine.Rendering;

using System.Linq;
using System.Runtime.InteropServices;

public class InstancingMassMeshes : MonoBehaviour
{

	public int numInstances = 100;
	public Mesh mesh;
	public Material material;
	public int[] shaderPasses = new int[] { 0};
	public MeshTopology topology = MeshTopology.Triangles;

	public CameraEvent commandAt = CameraEvent.AfterGBuffer;
	public Camera[] targetCams;
	public float initPosRange = 10f;

	public ComputeShader updater;

	ComputeBuffer meshIndicesBuffer;
	ComputeBuffer meshVertexDataBuffer;
	ComputeBuffer instanceDataBuffer;

	void Start()
	{
		SetData();
	}
	void OnDestroy()
	{
		ReleaseData();
	}
	void Update()
	{
		UpdateData();
	}

	void ReleaseData()
	{
		new[] { meshIndicesBuffer, meshVertexDataBuffer, instanceDataBuffer }
		.Where(b => b != null).ToList().ForEach(b =>
		{
			b.Release();
			b = null;
		});
	}
	void SetData()
	{
		var indices = mesh.GetIndices(0);
		var vertexDataArray = Enumerable.Range(0, mesh.vertexCount).Select(b =>
		{
			return new VertexData()
			{
				vert = mesh.vertices[b],
				normal = mesh.normals[b],
				uv = mesh.uv[b],
				tangent = mesh.tangents[b]
			};
		}).ToArray();
		var instanceDataArray = Enumerable.Range(0, numInstances).Select(b =>
		{
			return new InstanceData()
			{
				position = new Vector3(Random.Range(-1f,1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * initPosRange,
				rotation = Random.rotation,
				scale = Random.value,
			};
		}).ToArray();

		meshIndicesBuffer = new ComputeBuffer(indices.Length, Marshal.SizeOf(typeof(int)));
		meshVertexDataBuffer = new ComputeBuffer(vertexDataArray.Length, Marshal.SizeOf(typeof(VertexData)));
		instanceDataBuffer = new ComputeBuffer(numInstances, Marshal.SizeOf(typeof(InstanceData)));

		meshIndicesBuffer.SetData(indices);
		meshVertexDataBuffer.SetData(vertexDataArray);
		instanceDataBuffer.SetData(instanceDataArray);

		material.SetBuffer("_Indices", meshIndicesBuffer);
		material.SetBuffer("_vData", meshVertexDataBuffer);
		material.SetBuffer("_iData", instanceDataBuffer);

		var cb = new CommandBuffer();
		cb.name = name + ".instancingMassMeshes";
		foreach(var shaderPass in shaderPasses)
			cb.DrawProcedural(Matrix4x4.identity, material, shaderPass, topology, indices.Length, numInstances);
		foreach (var c in targetCams)
			c.AddCommandBuffer(commandAt, cb);
	}
	void UpdateData()
	{
		var kernel = updater.FindKernel("CSMain");
		updater.SetBuffer(kernel, "_iData", instanceDataBuffer);
		updater.Dispatch(kernel, numInstances / 1024+1, 1, 1);
	}

	#region struct for ComputeBuffer
	struct VertexData
	{
		public Vector3 vert;
		public Vector3 normal;
		public Vector2 uv;
		public Vector4 tangent;
	}
	struct InstanceData
	{
		public Vector3 position;
		public Quaternion rotation;
		public float scale;//i use only uniform scale!
	}
	#endregion
}
