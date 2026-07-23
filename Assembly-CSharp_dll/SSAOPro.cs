using System;
using UnityEngine;

// Token: 0x020000C4 RID: 196
[ImageEffectAllowedInSceneView]
[HelpURL("http://www.thomashourdel.com/ssaopro/doc/")]
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/SSAO Pro")]
[RequireComponent(typeof(Camera))]
public class SSAOPro : MonoBehaviour
{
	// Token: 0x170000B5 RID: 181
	// (get) Token: 0x06000479 RID: 1145 RVA: 0x0001CBFF File Offset: 0x0001ADFF
	public Material Material
	{
		get
		{
			if (this.m_Material == null)
			{
				this.m_Material = new Material(this.ShaderSSAO)
				{
					hideFlags = HideFlags.HideAndDontSave
				};
			}
			return this.m_Material;
		}
	}

	// Token: 0x170000B6 RID: 182
	// (get) Token: 0x0600047A RID: 1146 RVA: 0x0001CC2E File Offset: 0x0001AE2E
	public Shader ShaderSSAO
	{
		get
		{
			if (this.m_ShaderSSAO == null)
			{
				this.m_ShaderSSAO = Shader.Find("Hidden/SSAO Pro V2");
			}
			return this.m_ShaderSSAO;
		}
	}

	// Token: 0x0600047B RID: 1147 RVA: 0x0001CC54 File Offset: 0x0001AE54
	private void OnEnable()
	{
		this.m_Camera = base.GetComponent<Camera>();
		if (this.ShaderSSAO == null)
		{
			Debug.LogWarning("Missing shader (SSAO).");
			base.enabled = false;
			return;
		}
		if (!this.ShaderSSAO.isSupported)
		{
			Debug.LogWarning("Unsupported shader (SSAO).");
			base.enabled = false;
			return;
		}
		if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			Debug.LogWarning("Depth textures aren't supported on this device.");
			base.enabled = false;
			return;
		}
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x0001CCC6 File Offset: 0x0001AEC6
	private void OnPreRender()
	{
		this.m_Camera.depthTextureMode |= (DepthTextureMode.Depth | DepthTextureMode.DepthNormals);
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x0001CCDB File Offset: 0x0001AEDB
	private void OnDisable()
	{
		if (this.m_Material != null)
		{
			UnityEngine.Object.DestroyImmediate(this.m_Material);
		}
		this.m_Material = null;
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x0001CD00 File Offset: 0x0001AF00
	[ImageEffectOpaque]
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (this.ShaderSSAO == null || Mathf.Approximately(this.Intensity, 0f))
		{
			Graphics.Blit(source, destination);
			return;
		}
		this.Material.shaderKeywords = null;
		switch (this.Samples)
		{
		case SSAOPro.SampleCount.Low:
			this.Material.EnableKeyword("SAMPLES_LOW");
			break;
		case SSAOPro.SampleCount.Medium:
			this.Material.EnableKeyword("SAMPLES_MEDIUM");
			break;
		case SSAOPro.SampleCount.High:
			this.Material.EnableKeyword("SAMPLES_HIGH");
			break;
		case SSAOPro.SampleCount.Ultra:
			this.Material.EnableKeyword("SAMPLES_ULTRA");
			break;
		}
		int num = 0;
		if (this.NoiseTexture != null)
		{
			num = 1;
		}
		if (!Mathf.Approximately(this.LumContribution, 0f))
		{
			num += 2;
		}
		num++;
		this.Material.SetMatrix("_InverseViewProject", (this.m_Camera.projectionMatrix * this.m_Camera.worldToCameraMatrix).inverse);
		this.Material.SetMatrix("_CameraModelView", this.m_Camera.cameraToWorldMatrix);
		this.Material.SetTexture("_NoiseTex", this.NoiseTexture);
		this.Material.SetVector("_Params1", new Vector4((this.NoiseTexture == null) ? 0f : ((float)this.NoiseTexture.width), this.Radius, this.Intensity, this.Distance));
		this.Material.SetVector("_Params2", new Vector4(this.Bias, this.LumContribution, this.CutoffDistance, this.CutoffFalloff));
		this.Material.SetColor("_OcclusionColor", this.OcclusionColor);
		if (this.Blur != SSAOPro.BlurMode.None)
		{
			SSAOPro.Pass pass = (this.Blur == SSAOPro.BlurMode.HighQualityBilateral) ? SSAOPro.Pass.HighQualityBilateralBlur : SSAOPro.Pass.GaussianBlur;
			int num2 = this.BlurDownsampling ? this.Downsampling : 1;
			RenderTexture temporary = RenderTexture.GetTemporary(source.width / num2, source.height / num2, 0, RenderTextureFormat.ARGB32);
			RenderTexture temporary2 = RenderTexture.GetTemporary(source.width / this.Downsampling, source.height / this.Downsampling, 0, RenderTextureFormat.ARGB32);
			Graphics.Blit(temporary, temporary, this.Material, 0);
			Graphics.Blit(source, temporary, this.Material, num);
			this.Material.SetFloat("_BilateralThreshold", this.BlurBilateralThreshold * 5f);
			for (int i = 0; i < this.BlurPasses; i++)
			{
				this.Material.SetVector("_Direction", new Vector2(1f / (float)source.width, 0f));
				Graphics.Blit(temporary, temporary2, this.Material, (int)pass);
				temporary.DiscardContents();
				this.Material.SetVector("_Direction", new Vector2(0f, 1f / (float)source.height));
				Graphics.Blit(temporary2, temporary, this.Material, (int)pass);
				temporary2.DiscardContents();
			}
			if (!this.DebugAO)
			{
				this.Material.SetTexture("_SSAOTex", temporary);
				Graphics.Blit(source, destination, this.Material, 7);
			}
			else
			{
				Graphics.Blit(temporary, destination);
			}
			RenderTexture.ReleaseTemporary(temporary);
			RenderTexture.ReleaseTemporary(temporary2);
			return;
		}
		RenderTexture temporary3 = RenderTexture.GetTemporary(source.width / this.Downsampling, source.height / this.Downsampling, 0, RenderTextureFormat.ARGB32);
		Graphics.Blit(temporary3, temporary3, this.Material, 0);
		if (this.DebugAO)
		{
			Graphics.Blit(source, temporary3, this.Material, num);
			Graphics.Blit(temporary3, destination);
			RenderTexture.ReleaseTemporary(temporary3);
			return;
		}
		Graphics.Blit(source, temporary3, this.Material, num);
		this.Material.SetTexture("_SSAOTex", temporary3);
		Graphics.Blit(source, destination, this.Material, 7);
		RenderTexture.ReleaseTemporary(temporary3);
	}

	// Token: 0x04000481 RID: 1153
	public Texture2D NoiseTexture;

	// Token: 0x04000482 RID: 1154
	public bool UseHighPrecisionDepthMap;

	// Token: 0x04000483 RID: 1155
	public SSAOPro.SampleCount Samples = SSAOPro.SampleCount.Medium;

	// Token: 0x04000484 RID: 1156
	[Range(1f, 4f)]
	public int Downsampling = 1;

	// Token: 0x04000485 RID: 1157
	[Range(0.01f, 1.25f)]
	public float Radius = 0.12f;

	// Token: 0x04000486 RID: 1158
	[Range(0f, 16f)]
	public float Intensity = 2.5f;

	// Token: 0x04000487 RID: 1159
	[Range(0f, 10f)]
	public float Distance = 1f;

	// Token: 0x04000488 RID: 1160
	[Range(0f, 1f)]
	public float Bias = 0.1f;

	// Token: 0x04000489 RID: 1161
	[Range(0f, 1f)]
	public float LumContribution = 0.5f;

	// Token: 0x0400048A RID: 1162
	[ColorUsage(false)]
	public Color OcclusionColor = Color.black;

	// Token: 0x0400048B RID: 1163
	public float CutoffDistance = 150f;

	// Token: 0x0400048C RID: 1164
	public float CutoffFalloff = 50f;

	// Token: 0x0400048D RID: 1165
	public SSAOPro.BlurMode Blur = SSAOPro.BlurMode.HighQualityBilateral;

	// Token: 0x0400048E RID: 1166
	public bool BlurDownsampling;

	// Token: 0x0400048F RID: 1167
	[Range(1f, 4f)]
	public int BlurPasses = 1;

	// Token: 0x04000490 RID: 1168
	[Range(1f, 20f)]
	public float BlurBilateralThreshold = 10f;

	// Token: 0x04000491 RID: 1169
	public bool DebugAO;

	// Token: 0x04000492 RID: 1170
	protected Shader m_ShaderSSAO;

	// Token: 0x04000493 RID: 1171
	protected Material m_Material;

	// Token: 0x04000494 RID: 1172
	protected Camera m_Camera;

	// Token: 0x020000C5 RID: 197
	public enum BlurMode
	{
		// Token: 0x04000496 RID: 1174
		None,
		// Token: 0x04000497 RID: 1175
		Gaussian,
		// Token: 0x04000498 RID: 1176
		HighQualityBilateral
	}

	// Token: 0x020000C6 RID: 198
	public enum SampleCount
	{
		// Token: 0x0400049A RID: 1178
		VeryLow,
		// Token: 0x0400049B RID: 1179
		Low,
		// Token: 0x0400049C RID: 1180
		Medium,
		// Token: 0x0400049D RID: 1181
		High,
		// Token: 0x0400049E RID: 1182
		Ultra
	}

	// Token: 0x020000C7 RID: 199
	protected enum Pass
	{
		// Token: 0x040004A0 RID: 1184
		Clear,
		// Token: 0x040004A1 RID: 1185
		GaussianBlur = 5,
		// Token: 0x040004A2 RID: 1186
		HighQualityBilateralBlur,
		// Token: 0x040004A3 RID: 1187
		Composite
	}
}
