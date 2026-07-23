using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200002F RID: 47
[AddComponentMenu("Dynamic Bone/Dynamic Bone")]
public class DynamicBone : MonoBehaviour
{
	// Token: 0x060000B6 RID: 182 RVA: 0x000069BF File Offset: 0x00004BBF
	private void Start()
	{
		this.SetupParticles();
	}

	// Token: 0x060000B7 RID: 183 RVA: 0x000069C7 File Offset: 0x00004BC7
	private void Update()
	{
		if (this.m_Weight > 0f)
		{
			this.InitTransforms();
		}
	}

	// Token: 0x060000B8 RID: 184 RVA: 0x000069DC File Offset: 0x00004BDC
	private void LateUpdate()
	{
		if (this.m_Weight > 0f)
		{
			this.UpdateDynamicBones(Time.deltaTime);
		}
	}

	// Token: 0x060000B9 RID: 185 RVA: 0x000069F6 File Offset: 0x00004BF6
	private void OnEnable()
	{
		this.ResetParticlesPosition();
		this.m_ObjectPrevPosition = base.transform.position;
	}

	// Token: 0x060000BA RID: 186 RVA: 0x00006A0F File Offset: 0x00004C0F
	private void OnDisable()
	{
		this.InitTransforms();
	}

	// Token: 0x060000BB RID: 187 RVA: 0x00006A18 File Offset: 0x00004C18
	private void OnValidate()
	{
		this.m_UpdateRate = Mathf.Max(this.m_UpdateRate, 0f);
		this.m_Damping = Mathf.Clamp01(this.m_Damping);
		this.m_Elasticity = Mathf.Clamp01(this.m_Elasticity);
		this.m_Stiffness = Mathf.Clamp01(this.m_Stiffness);
		this.m_Inert = Mathf.Clamp01(this.m_Inert);
		this.m_Radius = Mathf.Max(this.m_Radius, 0f);
		if (Application.isEditor && Application.isPlaying)
		{
			this.InitTransforms();
			this.SetupParticles();
		}
	}

	// Token: 0x060000BC RID: 188 RVA: 0x00006AB0 File Offset: 0x00004CB0
	private void OnDrawGizmosSelected()
	{
		if (!base.enabled || this.m_Root == null)
		{
			return;
		}
		if (Application.isEditor && !Application.isPlaying && base.transform.hasChanged)
		{
			this.InitTransforms();
			this.SetupParticles();
		}
		Gizmos.color = Color.white;
		foreach (DynamicBone.Particle particle in this.m_Particles)
		{
			if (particle.m_ParentIndex >= 0)
			{
				DynamicBone.Particle particle2 = this.m_Particles[particle.m_ParentIndex];
				Gizmos.DrawLine(particle.m_Position, particle2.m_Position);
			}
			if (particle.m_Radius > 0f)
			{
				Gizmos.DrawWireSphere(particle.m_Position, particle.m_Radius * this.m_ObjectScale);
			}
		}
	}

	// Token: 0x060000BD RID: 189 RVA: 0x00006B98 File Offset: 0x00004D98
	public void SetWeight(float w)
	{
		if (this.m_Weight != w)
		{
			if (w == 0f)
			{
				this.InitTransforms();
			}
			else if (this.m_Weight == 0f)
			{
				this.ResetParticlesPosition();
				this.m_ObjectPrevPosition = base.transform.position;
			}
			this.m_Weight = w;
		}
	}

	// Token: 0x060000BE RID: 190 RVA: 0x00006BE9 File Offset: 0x00004DE9
	public float GetWeight()
	{
		return this.m_Weight;
	}

	// Token: 0x060000BF RID: 191 RVA: 0x00006BF4 File Offset: 0x00004DF4
	private void UpdateDynamicBones(float t)
	{
		if (this.m_Root == null)
		{
			return;
		}
		this.m_ObjectScale = Mathf.Abs(base.transform.lossyScale.x);
		this.m_ObjectMove = base.transform.position - this.m_ObjectPrevPosition;
		this.m_ObjectPrevPosition = base.transform.position;
		int num = 1;
		if (this.m_UpdateRate > 0f)
		{
			float num2 = 1f / this.m_UpdateRate;
			this.m_Time += t;
			num = 0;
			while (this.m_Time >= num2)
			{
				this.m_Time -= num2;
				if (++num >= 3)
				{
					this.m_Time = 0f;
					break;
				}
			}
		}
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				this.UpdateParticles1();
				this.UpdateParticles2();
				this.m_ObjectMove = Vector3.zero;
			}
		}
		else
		{
			this.SkipUpdateParticles();
		}
		this.ApplyParticlesToTransforms();
	}

	// Token: 0x060000C0 RID: 192 RVA: 0x00006CE8 File Offset: 0x00004EE8
	private void SetupParticles()
	{
		this.m_Particles.Clear();
		if (this.m_Root == null)
		{
			return;
		}
		this.m_LocalGravity = this.m_Root.InverseTransformDirection(this.m_Gravity);
		this.m_ObjectScale = base.transform.lossyScale.x;
		this.m_ObjectPrevPosition = base.transform.position;
		this.m_ObjectMove = Vector3.zero;
		this.m_BoneTotalLength = 0f;
		this.AppendParticles(this.m_Root, -1, 0f);
		foreach (DynamicBone.Particle particle in this.m_Particles)
		{
			particle.m_Damping = this.m_Damping;
			particle.m_Elasticity = this.m_Elasticity;
			particle.m_Stiffness = this.m_Stiffness;
			particle.m_Inert = this.m_Inert;
			particle.m_Radius = this.m_Radius;
			if (this.m_BoneTotalLength > 0f)
			{
				float time = particle.m_BoneLength / this.m_BoneTotalLength;
				if (this.m_DampingDistrib.keys.Length != 0)
				{
					particle.m_Damping *= this.m_DampingDistrib.Evaluate(time);
				}
				if (this.m_ElasticityDistrib.keys.Length != 0)
				{
					particle.m_Elasticity *= this.m_ElasticityDistrib.Evaluate(time);
				}
				if (this.m_StiffnessDistrib.keys.Length != 0)
				{
					particle.m_Stiffness *= this.m_StiffnessDistrib.Evaluate(time);
				}
				if (this.m_InertDistrib.keys.Length != 0)
				{
					particle.m_Inert *= this.m_InertDistrib.Evaluate(time);
				}
				if (this.m_RadiusDistrib.keys.Length != 0)
				{
					particle.m_Radius *= this.m_RadiusDistrib.Evaluate(time);
				}
			}
			particle.m_Damping = Mathf.Clamp01(particle.m_Damping);
			particle.m_Elasticity = Mathf.Clamp01(particle.m_Elasticity);
			particle.m_Stiffness = Mathf.Clamp01(particle.m_Stiffness);
			particle.m_Inert = Mathf.Clamp01(particle.m_Inert);
			particle.m_Radius = Mathf.Max(particle.m_Radius, 0f);
		}
	}

	// Token: 0x060000C1 RID: 193 RVA: 0x00006F40 File Offset: 0x00005140
	private void AppendParticles(Transform b, int parentIndex, float boneLength)
	{
		DynamicBone.Particle particle = new DynamicBone.Particle();
		particle.m_Transform = b;
		particle.m_ParentIndex = parentIndex;
		if (b != null)
		{
			particle.m_Position = (particle.m_PrevPosition = b.position);
			particle.m_InitLocalPosition = b.localPosition;
			particle.m_InitLocalRotation = b.localRotation;
		}
		else
		{
			Transform transform = this.m_Particles[parentIndex].m_Transform;
			if (this.m_EndLength > 0f)
			{
				Transform parent = transform.parent;
				if (parent != null)
				{
					particle.m_EndOffset = transform.InverseTransformPoint(transform.position * 2f - parent.position) * this.m_EndLength;
				}
				else
				{
					particle.m_EndOffset = new Vector3(this.m_EndLength, 0f, 0f);
				}
			}
			else
			{
				particle.m_EndOffset = transform.InverseTransformPoint(base.transform.TransformDirection(this.m_EndOffset) + transform.position);
			}
			particle.m_Position = (particle.m_PrevPosition = transform.TransformPoint(particle.m_EndOffset));
		}
		if (parentIndex >= 0)
		{
			boneLength += (this.m_Particles[parentIndex].m_Transform.position - particle.m_Position).magnitude;
			particle.m_BoneLength = boneLength;
			this.m_BoneTotalLength = Mathf.Max(this.m_BoneTotalLength, boneLength);
		}
		int count = this.m_Particles.Count;
		this.m_Particles.Add(particle);
		if (b != null)
		{
			for (int i = 0; i < b.childCount; i++)
			{
				bool flag = false;
				if (this.m_Exclusions != null)
				{
					using (List<Transform>.Enumerator enumerator = this.m_Exclusions.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current == b.GetChild(i))
							{
								flag = true;
								break;
							}
						}
					}
				}
				if (!flag)
				{
					this.AppendParticles(b.GetChild(i), count, boneLength);
				}
			}
			if (b.childCount == 0 && (this.m_EndLength > 0f || this.m_EndOffset != Vector3.zero))
			{
				this.AppendParticles(null, count, boneLength);
			}
		}
	}

	// Token: 0x060000C2 RID: 194 RVA: 0x00007188 File Offset: 0x00005388
	private void InitTransforms()
	{
		for (int i = 0; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			if (particle.m_Transform != null)
			{
				particle.m_Transform.localPosition = particle.m_InitLocalPosition;
				particle.m_Transform.localRotation = particle.m_InitLocalRotation;
			}
		}
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x000071E8 File Offset: 0x000053E8
	private void ResetParticlesPosition()
	{
		foreach (DynamicBone.Particle particle in this.m_Particles)
		{
			if (particle.m_Transform != null)
			{
				particle.m_Position = (particle.m_PrevPosition = particle.m_Transform.position);
			}
			else
			{
				Transform transform = this.m_Particles[particle.m_ParentIndex].m_Transform;
				particle.m_Position = (particle.m_PrevPosition = transform.TransformPoint(particle.m_EndOffset));
			}
		}
	}

	// Token: 0x060000C4 RID: 196 RVA: 0x00007290 File Offset: 0x00005490
	private void UpdateParticles1()
	{
		Vector3 vector = this.m_Gravity;
		Vector3 normalized = this.m_Gravity.normalized;
		Vector3 lhs = this.m_Root.TransformDirection(this.m_LocalGravity);
		Vector3 b = normalized * Mathf.Max(Vector3.Dot(lhs, normalized), 0f);
		vector -= b;
		vector = (vector + this.m_Force) * this.m_ObjectScale;
		for (int i = 0; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			if (particle.m_ParentIndex >= 0)
			{
				Vector3 a = particle.m_Position - particle.m_PrevPosition;
				Vector3 b2 = this.m_ObjectMove * particle.m_Inert;
				particle.m_PrevPosition = particle.m_Position + b2;
				particle.m_Position += a * (1f - particle.m_Damping) + vector + b2;
			}
			else
			{
				particle.m_PrevPosition = particle.m_Position;
				particle.m_Position = particle.m_Transform.position;
			}
		}
	}

	// Token: 0x060000C5 RID: 197 RVA: 0x000073C8 File Offset: 0x000055C8
	private void UpdateParticles2()
	{
		this.movePlane.SetNormalAndPosition(Vector3.zero, Vector3.zero);
		for (int i = 1; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			DynamicBone.Particle particle2 = this.m_Particles[particle.m_ParentIndex];
			float magnitude;
			if (particle.m_Transform != null)
			{
				magnitude = (particle2.m_Transform.position - particle.m_Transform.position).magnitude;
			}
			else
			{
				magnitude = particle2.m_Transform.localToWorldMatrix.MultiplyVector(particle.m_EndOffset).magnitude;
			}
			float num = Mathf.Lerp(1f, particle.m_Stiffness, this.m_Weight);
			if (num > 0f || particle.m_Elasticity > 0f)
			{
				Matrix4x4 localToWorldMatrix = particle2.m_Transform.localToWorldMatrix;
				localToWorldMatrix.SetColumn(3, particle2.m_Position);
				Vector3 a;
				if (particle.m_Transform != null)
				{
					a = localToWorldMatrix.MultiplyPoint3x4(particle.m_Transform.localPosition);
				}
				else
				{
					a = localToWorldMatrix.MultiplyPoint3x4(particle.m_EndOffset);
				}
				Vector3 a2 = a - particle.m_Position;
				particle.m_Position += a2 * particle.m_Elasticity;
				if (num > 0f)
				{
					a2 = a - particle.m_Position;
					float magnitude2 = a2.magnitude;
					float num2 = magnitude * (1f - num) * 2f;
					if (magnitude2 > num2)
					{
						particle.m_Position += a2 * ((magnitude2 - num2) / magnitude2);
					}
				}
			}
			if (this.m_Colliders != null)
			{
				float particleRadius = particle.m_Radius * this.m_ObjectScale;
				foreach (DynamicBoneCollider dynamicBoneCollider in this.m_Colliders)
				{
					if (dynamicBoneCollider != null && dynamicBoneCollider.enabled)
					{
						dynamicBoneCollider.Collide(ref particle.m_Position, particleRadius);
					}
				}
			}
			if (this.m_FreezeAxis != DynamicBone.FreezeAxis.None)
			{
				switch (this.m_FreezeAxis)
				{
				case DynamicBone.FreezeAxis.X:
					this.movePlane.SetNormalAndPosition(particle2.m_Transform.right, particle2.m_Position);
					break;
				case DynamicBone.FreezeAxis.Y:
					this.movePlane.SetNormalAndPosition(particle2.m_Transform.up, particle2.m_Position);
					break;
				case DynamicBone.FreezeAxis.Z:
					this.movePlane.SetNormalAndPosition(particle2.m_Transform.forward, particle2.m_Position);
					break;
				}
				particle.m_Position -= this.movePlane.normal * this.movePlane.GetDistanceToPoint(particle.m_Position);
			}
			Vector3 a3 = particle2.m_Position - particle.m_Position;
			float magnitude3 = a3.magnitude;
			if (magnitude3 > 0f)
			{
				particle.m_Position += a3 * ((magnitude3 - magnitude) / magnitude3);
			}
		}
	}

	// Token: 0x060000C6 RID: 198 RVA: 0x000076FC File Offset: 0x000058FC
	private void SkipUpdateParticles()
	{
		foreach (DynamicBone.Particle particle in this.m_Particles)
		{
			if (particle.m_ParentIndex >= 0)
			{
				Vector3 b = this.m_ObjectMove * particle.m_Inert;
				particle.m_PrevPosition += b;
				particle.m_Position += b;
				DynamicBone.Particle particle2 = this.m_Particles[particle.m_ParentIndex];
				float magnitude;
				if (particle.m_Transform != null)
				{
					magnitude = (particle2.m_Transform.position - particle.m_Transform.position).magnitude;
				}
				else
				{
					magnitude = particle2.m_Transform.localToWorldMatrix.MultiplyVector(particle.m_EndOffset).magnitude;
				}
				float num = Mathf.Lerp(1f, particle.m_Stiffness, this.m_Weight);
				if (num > 0f)
				{
					Matrix4x4 localToWorldMatrix = particle2.m_Transform.localToWorldMatrix;
					localToWorldMatrix.SetColumn(3, particle2.m_Position);
					Vector3 a;
					if (particle.m_Transform != null)
					{
						a = localToWorldMatrix.MultiplyPoint3x4(particle.m_Transform.localPosition);
					}
					else
					{
						a = localToWorldMatrix.MultiplyPoint3x4(particle.m_EndOffset);
					}
					Vector3 a2 = a - particle.m_Position;
					float magnitude2 = a2.magnitude;
					float num2 = magnitude * (1f - num) * 2f;
					if (magnitude2 > num2)
					{
						particle.m_Position += a2 * ((magnitude2 - num2) / magnitude2);
					}
				}
				Vector3 a3 = particle2.m_Position - particle.m_Position;
				float magnitude3 = a3.magnitude;
				if (magnitude3 > 0f)
				{
					particle.m_Position += a3 * ((magnitude3 - magnitude) / magnitude3);
				}
			}
			else
			{
				particle.m_PrevPosition = particle.m_Position;
				particle.m_Position = particle.m_Transform.position;
			}
		}
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x00007938 File Offset: 0x00005B38
	private void ApplyParticlesToTransforms()
	{
		for (int i = 1; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			DynamicBone.Particle particle2 = this.m_Particles[particle.m_ParentIndex];
			if (particle2.m_Transform.childCount <= 1)
			{
				Vector3 direction;
				if (particle.m_Transform != null)
				{
					direction = particle.m_Transform.localPosition;
				}
				else
				{
					direction = particle.m_EndOffset;
				}
				Quaternion lhs = Quaternion.FromToRotation(particle2.m_Transform.TransformDirection(direction), particle.m_Position - particle2.m_Position);
				particle2.m_Transform.rotation = lhs * particle2.m_Transform.rotation;
			}
			if (particle.m_Transform)
			{
				particle.m_Transform.position = particle.m_Position;
			}
		}
	}

	// Token: 0x040000FA RID: 250
	public Transform m_Root;

	// Token: 0x040000FB RID: 251
	public float m_UpdateRate = 60f;

	// Token: 0x040000FC RID: 252
	[Range(0f, 1f)]
	public float m_Damping = 0.1f;

	// Token: 0x040000FD RID: 253
	public AnimationCurve m_DampingDistrib;

	// Token: 0x040000FE RID: 254
	[Range(0f, 1f)]
	public float m_Elasticity = 0.1f;

	// Token: 0x040000FF RID: 255
	public AnimationCurve m_ElasticityDistrib;

	// Token: 0x04000100 RID: 256
	[Range(0f, 1f)]
	public float m_Stiffness = 0.1f;

	// Token: 0x04000101 RID: 257
	public AnimationCurve m_StiffnessDistrib;

	// Token: 0x04000102 RID: 258
	[Range(0f, 1f)]
	public float m_Inert;

	// Token: 0x04000103 RID: 259
	public AnimationCurve m_InertDistrib;

	// Token: 0x04000104 RID: 260
	public float m_Radius;

	// Token: 0x04000105 RID: 261
	public AnimationCurve m_RadiusDistrib;

	// Token: 0x04000106 RID: 262
	public float m_EndLength;

	// Token: 0x04000107 RID: 263
	public Vector3 m_EndOffset = Vector3.zero;

	// Token: 0x04000108 RID: 264
	public Vector3 m_Gravity = Vector3.zero;

	// Token: 0x04000109 RID: 265
	public Vector3 m_Force = Vector3.zero;

	// Token: 0x0400010A RID: 266
	public List<DynamicBoneCollider> m_Colliders;

	// Token: 0x0400010B RID: 267
	public List<Transform> m_Exclusions;

	// Token: 0x0400010C RID: 268
	public DynamicBone.FreezeAxis m_FreezeAxis;

	// Token: 0x0400010D RID: 269
	private Vector3 m_LocalGravity = Vector3.zero;

	// Token: 0x0400010E RID: 270
	private Vector3 m_ObjectMove = Vector3.zero;

	// Token: 0x0400010F RID: 271
	private Vector3 m_ObjectPrevPosition = Vector3.zero;

	// Token: 0x04000110 RID: 272
	private float m_BoneTotalLength;

	// Token: 0x04000111 RID: 273
	private float m_ObjectScale = 1f;

	// Token: 0x04000112 RID: 274
	private float m_Time;

	// Token: 0x04000113 RID: 275
	private float m_Weight = 1f;

	// Token: 0x04000114 RID: 276
	private List<DynamicBone.Particle> m_Particles = new List<DynamicBone.Particle>();

	// Token: 0x04000115 RID: 277
	private Plane movePlane;

	// Token: 0x02000030 RID: 48
	public enum FreezeAxis
	{
		// Token: 0x04000117 RID: 279
		None,
		// Token: 0x04000118 RID: 280
		X,
		// Token: 0x04000119 RID: 281
		Y,
		// Token: 0x0400011A RID: 282
		Z
	}

	// Token: 0x02000031 RID: 49
	private class Particle
	{
		// Token: 0x0400011B RID: 283
		public Transform m_Transform;

		// Token: 0x0400011C RID: 284
		public int m_ParentIndex = -1;

		// Token: 0x0400011D RID: 285
		public float m_Damping;

		// Token: 0x0400011E RID: 286
		public float m_Elasticity;

		// Token: 0x0400011F RID: 287
		public float m_Stiffness;

		// Token: 0x04000120 RID: 288
		public float m_Inert;

		// Token: 0x04000121 RID: 289
		public float m_Radius;

		// Token: 0x04000122 RID: 290
		public float m_BoneLength;

		// Token: 0x04000123 RID: 291
		public Vector3 m_Position = Vector3.zero;

		// Token: 0x04000124 RID: 292
		public Vector3 m_PrevPosition = Vector3.zero;

		// Token: 0x04000125 RID: 293
		public Vector3 m_EndOffset = Vector3.zero;

		// Token: 0x04000126 RID: 294
		public Vector3 m_InitLocalPosition = Vector3.zero;

		// Token: 0x04000127 RID: 295
		public Quaternion m_InitLocalRotation = Quaternion.identity;
	}
}
