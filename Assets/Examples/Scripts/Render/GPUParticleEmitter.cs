using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using sugi.cc;

public class GPUParticleEmitter : MonoBehaviour
{
	public Material mat;
	public ParticleEmitOp option;


	#region MultiRenderTexture mrtex

	MultiRenderTexture mrtex {
		get {
			if (_mrtex == null)
				_mrtex = GetComponent<MultiRenderTexture> ();
			return _mrtex;
		}
	}

	MultiRenderTexture _mrtex;

	#endregion

	public void EmitParticle (
		Color col, Vector3 posision, Vector3 velocity, 
		float emission, float radius, float spread, float drag,
		float size0, float size1, float lifeTime, float wetness = 0, 
		float randSize = 0, float randLife = 0, float randWet = 0, float randVel = 0,
		Texture colorSamler = null
	)
	{
		option.color = col;
		option.emitPos = posision;
		option.initVel = velocity;
		option.colorSampler = colorSamler;

		option.emission = emission;
		option.radius = radius;
		option.spread = spread;
		option.drag = drag;

		option.startSize = size0;
		option.endSize = size1;
		option.lifeTime = lifeTime;
		option.wetness = wetness;

		option.randomSize = randSize;
		option.randomLife = randLife;
		option.randomWetness = randWet;
		option.randomVelocity = randVel;

		EmitParticle (option);
	}

	public void EmitParticle (ParticleEmitOp option)
	{
		mat = option.SetMaterialProps (mat);
		mrtex.Render (mat);
	}

	[System.Serializable]
	public class ParticleEmitOp
	{
		public Color color;
		public Vector3 emitPos;
		public Vector3 initVel;
		public Texture colorSampler;

		public float emission = 10f;
		public float radius = 0.5f;
		public float spread = 0.1f;
		public float drag = 1f;

		public float startSize = 0f;
		public float endSize = 1f;
		public float lifeTime = 10f;
		public float wetness = 0;

		public float randomSize = 0;
		public float randomLife = 0;
		public float randomWetness = 0;
		public float randomVelocity = 0;


		public Material SetMaterialProps (Material mat)
		{
			var pos = new Vector4 (emitPos.x, emitPos.y, emitPos.z, radius);
			var vel = new Vector4 (initVel.x, initVel.y, initVel.z, spread);
			var eProp = new Vector4 (startSize, endSize, lifeTime, wetness);
			var eRand = new Vector4 (randomSize, randomLife, randomWetness, randomVelocity);

			mat.SetTexture ("_MainTex", colorSampler);
			mat.SetColor ("_Color", color);
			mat.SetVector ("_EmitPos", pos);
			mat.SetVector ("_EmitVel", vel);
			mat.SetFloat ("_EmitRate", emission);
			mat.SetVector ("_EmitProp", eProp);
			mat.SetVector ("_EmitRand", eRand);
			mat.SetFloat ("_Drag", drag);
			mat.SetFloat ("_Random", Random.Range (-100f, 100f));
			return mat;
		}
	}
}