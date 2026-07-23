using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UiParticles
{
	// Token: 0x02000BDA RID: 3034
	[RequireComponent(typeof(ParticleSystem))]
	public class UiParticles : MaskableGraphic
	{
		// Token: 0x1700049C RID: 1180
		// (get) Token: 0x060056C7 RID: 22215 RVA: 0x0010683D File Offset: 0x00104A3D
		// (set) Token: 0x060056C8 RID: 22216 RVA: 0x00106845 File Offset: 0x00104A45
		public ParticleSystem ParticleSystem
		{
			get
			{
				return this.m_ParticleSystem;
			}
			set
			{
				if (SetPropertyUtility.SetClass<ParticleSystem>(ref this.m_ParticleSystem, value))
				{
					this.SetAllDirty();
				}
			}
		}

		// Token: 0x1700049D RID: 1181
		// (get) Token: 0x060056C9 RID: 22217 RVA: 0x0010685B File Offset: 0x00104A5B
		// (set) Token: 0x060056CA RID: 22218 RVA: 0x00106863 File Offset: 0x00104A63
		public ParticleSystemRenderer particleSystemRenderer
		{
			get
			{
				return this.m_ParticleSystemRenderer;
			}
			set
			{
				if (SetPropertyUtility.SetClass<ParticleSystemRenderer>(ref this.m_ParticleSystemRenderer, value))
				{
					this.SetAllDirty();
				}
			}
		}

		// Token: 0x1700049E RID: 1182
		// (get) Token: 0x060056CB RID: 22219 RVA: 0x00106879 File Offset: 0x00104A79
		public override Texture mainTexture
		{
			get
			{
				if (this.material != null && this.material.mainTexture != null)
				{
					return this.material.mainTexture;
				}
				return Graphic.s_WhiteTexture;
			}
		}

		// Token: 0x060056CC RID: 22220 RVA: 0x001068B0 File Offset: 0x00104AB0
		protected override void Awake()
		{
			ParticleSystem component = base.GetComponent<ParticleSystem>();
			ParticleSystemRenderer component2 = base.GetComponent<ParticleSystemRenderer>();
			if (this.m_Material == null)
			{
				this.m_Material = component2.sharedMaterial;
			}
			base.Awake();
			this.ParticleSystem = component;
			this.particleSystemRenderer = component2;
		}

		// Token: 0x060056CD RID: 22221 RVA: 0x001068F9 File Offset: 0x00104AF9
		public override void SetMaterialDirty()
		{
			base.SetMaterialDirty();
			if (this.particleSystemRenderer != null)
			{
				this.particleSystemRenderer.sharedMaterial = this.m_Material;
			}
		}

		// Token: 0x060056CE RID: 22222 RVA: 0x00106920 File Offset: 0x00104B20
		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			if (this.ParticleSystem == null)
			{
				base.OnPopulateMesh(toFill);
				return;
			}
			this.GenerateParticlesBillboards(toFill);
		}

		// Token: 0x060056CF RID: 22223 RVA: 0x00106940 File Offset: 0x00104B40
		private void InitParticlesBuffer()
		{
			ParticleSystem.MainModule main = this.ParticleSystem.main;
			if (this.m_Particles == null || this.m_Particles.Length < main.maxParticles)
			{
				this.m_Particles = new ParticleSystem.Particle[main.maxParticles];
			}
		}

		// Token: 0x060056D0 RID: 22224 RVA: 0x00106984 File Offset: 0x00104B84
		private void GenerateParticlesBillboards(VertexHelper vh)
		{
			this.InitParticlesBuffer();
			int particles = this.ParticleSystem.GetParticles(this.m_Particles);
			vh.Clear();
			for (int i = 0; i < particles; i++)
			{
				this.DrawParticleBillboard(this.m_Particles[i], vh);
			}
		}

		// Token: 0x060056D1 RID: 22225 RVA: 0x001069D0 File Offset: 0x00104BD0
		private void DrawParticleBillboard(ParticleSystem.Particle particle, VertexHelper vh)
		{
			Vector3 vector = particle.position;
			Quaternion rotation = Quaternion.Euler(particle.rotation3D);
			if (this.ParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World)
			{
				vector = base.rectTransform.InverseTransformPoint(vector);
			}
			Vector3 currentSize3D = particle.GetCurrentSize3D(this.ParticleSystem);
			Vector3 vector2 = new Vector3(-currentSize3D.x * 0.5f, currentSize3D.y * 0.5f);
			Vector3 vector3 = new Vector3(currentSize3D.x * 0.5f, currentSize3D.y * 0.5f);
			Vector3 vector4 = new Vector3(currentSize3D.x * 0.5f, -currentSize3D.y * 0.5f);
			Vector3 vector5 = new Vector3(-currentSize3D.x * 0.5f, -currentSize3D.y * 0.5f);
			vector2 = rotation * vector2 + vector;
			vector3 = rotation * vector3 + vector;
			vector4 = rotation * vector4 + vector;
			vector5 = rotation * vector5 + vector;
			Color32 currentColor = particle.GetCurrentColor(this.ParticleSystem);
			int currentVertCount = vh.currentVertCount;
			Vector2[] array = new Vector2[4];
			if (!this.ParticleSystem.textureSheetAnimation.enabled)
			{
				array[0] = new Vector2(0f, 0f);
				array[1] = new Vector2(0f, 1f);
				array[2] = new Vector2(1f, 1f);
				array[3] = new Vector2(1f, 0f);
			}
			else
			{
				ParticleSystem.TextureSheetAnimationModule textureSheetAnimation = this.ParticleSystem.textureSheetAnimation;
				float num = particle.startLifetime - particle.remainingLifetime;
				float num2 = particle.startLifetime / (float)textureSheetAnimation.cycleCount;
				float time = num % num2 / num2;
				float num3 = textureSheetAnimation.frameOverTime.Evaluate(time);
				int num4 = textureSheetAnimation.numTilesY * textureSheetAnimation.numTilesX;
				float num5 = Mathf.Clamp(Mathf.Floor(num3 * (float)num4), 0f, (float)(num4 - 1));
				int num6 = (int)num5 % textureSheetAnimation.numTilesX;
				int num7 = (int)num5 / textureSheetAnimation.numTilesY;
				float num8 = 1f / (float)textureSheetAnimation.numTilesX;
				float num9 = 1f / (float)textureSheetAnimation.numTilesY;
				num7 = textureSheetAnimation.numTilesY - 1 - num7;
				float num10 = (float)num6 * num8;
				float num11 = (float)num7 * num9;
				float x = num10 + num8;
				float y = num11 + num9;
				array[0] = new Vector2(num10, num11);
				array[1] = new Vector2(num10, y);
				array[2] = new Vector2(x, y);
				array[3] = new Vector2(x, num11);
			}
			vh.AddVert(vector5, currentColor, array[0]);
			vh.AddVert(vector2, currentColor, array[1]);
			vh.AddVert(vector3, currentColor, array[2]);
			vh.AddVert(vector4, currentColor, array[3]);
			vh.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
			vh.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
		}

		// Token: 0x060056D2 RID: 22226 RVA: 0x00106CF4 File Offset: 0x00104EF4
		protected virtual void Update()
		{
			if (this.ParticleSystem != null && this.ParticleSystem.isPlaying)
			{
				this.SetVerticesDirty();
			}
			if (this.particleSystemRenderer != null && this.particleSystemRenderer.enabled)
			{
				this.particleSystemRenderer.enabled = false;
			}
		}

		// Token: 0x04004317 RID: 17175
		[FormerlySerializedAs("m_ParticleSystem")]
		private ParticleSystem m_ParticleSystem;

		// Token: 0x04004318 RID: 17176
		[FormerlySerializedAs("m_ParticleSystemRenderer")]
		private ParticleSystemRenderer m_ParticleSystemRenderer;

		// Token: 0x04004319 RID: 17177
		private ParticleSystem.Particle[] m_Particles;
	}
}
