using System;
using UnityEngine;

// Token: 0x020007EB RID: 2027
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Stylized Fog")]
public class StylizedFog : MonoBehaviour
{
	// Token: 0x06002A65 RID: 10853 RVA: 0x0009F66A File Offset: 0x0009D86A
	private void Start()
	{
		this.createResources();
		this.UpdateTextures();
		this.SetKeywords();
	}

	// Token: 0x06002A66 RID: 10854 RVA: 0x0009F66A File Offset: 0x0009D86A
	private void OnEnable()
	{
		this.createResources();
		this.UpdateTextures();
		this.SetKeywords();
	}

	// Token: 0x06002A67 RID: 10855 RVA: 0x0009F67E File Offset: 0x0009D87E
	private void OnDisable()
	{
		this.clearResources();
	}

	// Token: 0x06002A68 RID: 10856 RVA: 0x0009F686 File Offset: 0x0009D886
	public void UpdateTextures()
	{
		this.setGradient();
		this.SetKeywords();
		this.updateValues();
	}

	// Token: 0x06002A69 RID: 10857 RVA: 0x0009F69C File Offset: 0x0009D89C
	private void updateValues()
	{
		if (this.fogMat == null || this.fogShader == null)
		{
			this.createResources();
		}
		if (this.mainRamp != null)
		{
			this.fogMat.SetTexture("_MainRamp", this.mainRamp);
			Shader.SetGlobalTexture("_SF_MainRamp", this.mainRamp);
		}
		if (this.useBlend && this.blendRamp != null)
		{
			this.fogMat.SetTexture("_BlendRamp", this.blendRamp);
			this.fogMat.SetFloat("_Blend", this.blend);
			Shader.SetGlobalTexture("_SF_BlendRamp", this.blendRamp);
			Shader.SetGlobalFloat("_SF_Blend", this.blend);
		}
		if (this.useNoise && this.noiseTexture != null)
		{
			this.fogMat.SetTexture("_NoiseTex", this.noiseTexture);
			this.fogMat.SetVector("_NoiseSpeed", this.noiseSpeed);
			this.fogMat.SetVector("_NoiseTiling", this.noiseTiling);
			Shader.SetGlobalTexture("_SF_NoiseTex", this.noiseTexture);
			Shader.SetGlobalVector("_SF_NoiseSpeed", this.noiseSpeed);
			Shader.SetGlobalVector("_SF_NoiseTiling", this.noiseTiling);
		}
	}

	// Token: 0x06002A6A RID: 10858 RVA: 0x0009F7EC File Offset: 0x0009D9EC
	private void setGradient()
	{
		if (this.gradientSource == StylizedFog.StylizedFogGradient.Textures)
		{
			this.mainRamp = this.rampTexture;
			if (this.useBlend)
			{
				this.blendRamp = this.rampBlendTexture;
				return;
			}
		}
		else if (this.gradientSource == StylizedFog.StylizedFogGradient.Gradients)
		{
			if (this.mainRamp != null)
			{
				UnityEngine.Object.DestroyImmediate(this.mainRamp);
			}
			this.mainRamp = this.GenerateGradient(this.rampGradient, 256, 8);
			if (this.useBlend)
			{
				if (this.blendRamp != null)
				{
					UnityEngine.Object.DestroyImmediate(this.blendRamp);
				}
				this.blendRamp = this.GenerateGradient(this.rampBlendGradient, 256, 8);
			}
		}
	}

	// Token: 0x06002A6B RID: 10859 RVA: 0x0009F898 File Offset: 0x0009DA98
	private Texture2D GenerateGradient(Gradient gradient, int gWidth, int gHeight)
	{
		Texture2D texture2D = new Texture2D(gWidth, gHeight, TextureFormat.ARGB32, false);
		texture2D.wrapMode = TextureWrapMode.Clamp;
		texture2D.hideFlags = HideFlags.HideAndDontSave;
		Color color = Color.white;
		if (gradient != null)
		{
			for (int i = 0; i < gWidth; i++)
			{
				color = gradient.Evaluate((float)i / (float)gWidth);
				for (int j = 0; j < gHeight; j++)
				{
					texture2D.SetPixel(i, j, color);
				}
			}
		}
		texture2D.Apply();
		return texture2D;
	}

	// Token: 0x06002A6C RID: 10860 RVA: 0x0009F8FC File Offset: 0x0009DAFC
	private void createResources()
	{
		if (this.fogShader == null)
		{
			this.fogShader = Shader.Find("Hidden/StylizedFog");
		}
		if (this.fogMat == null && this.fogShader != null)
		{
			this.fogMat = new Material(this.fogShader);
			this.fogMat.hideFlags = HideFlags.HideAndDontSave;
		}
		if (this.mainRamp == null || this.blendRamp == null)
		{
			this.setGradient();
		}
		if (this.cam == null)
		{
			this.cam = base.GetComponent<Camera>();
			this.cam.depthTextureMode |= DepthTextureMode.Depth;
		}
	}

	// Token: 0x06002A6D RID: 10861 RVA: 0x0009F9B0 File Offset: 0x0009DBB0
	private void clearResources()
	{
		if (this.fogMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.fogMat);
		}
		this.disableKeywords();
		this.cam.depthTextureMode = DepthTextureMode.None;
	}

	// Token: 0x06002A6E RID: 10862 RVA: 0x0009F9E0 File Offset: 0x0009DBE0
	public void SetKeywords()
	{
		switch (this.fogMode)
		{
		case StylizedFog.StylizedFogMode.Blend:
			Shader.EnableKeyword("_FOG_BLEND");
			Shader.DisableKeyword("_FOG_ADDITIVE");
			Shader.DisableKeyword("_FOG_MULTIPLY");
			Shader.DisableKeyword("_FOG_SCREEN");
			Shader.DisableKeyword("_FOG_OVERLAY");
			Shader.DisableKeyword("_FOG_DODGE");
			break;
		case StylizedFog.StylizedFogMode.Additive:
			Shader.DisableKeyword("_FOG_BLEND");
			Shader.EnableKeyword("_FOG_ADDITIVE");
			Shader.DisableKeyword("_FOG_MULTIPLY");
			Shader.DisableKeyword("_FOG_SCREEN");
			Shader.DisableKeyword("_FOG_OVERLAY");
			Shader.DisableKeyword("_FOG_DODGE");
			break;
		case StylizedFog.StylizedFogMode.Multiply:
			Shader.DisableKeyword("_FOG_BLEND");
			Shader.DisableKeyword("_FOG_ADDITIVE");
			Shader.EnableKeyword("_FOG_MULTIPLY");
			Shader.DisableKeyword("_FOG_SCREEN");
			Shader.DisableKeyword("_FOG_OVERLAY");
			Shader.DisableKeyword("_FOG_DODGE");
			break;
		case StylizedFog.StylizedFogMode.Screen:
			Shader.DisableKeyword("_FOG_BLEND");
			Shader.DisableKeyword("_FOG_ADDITIVE");
			Shader.DisableKeyword("_FOG_MULTIPLY");
			Shader.EnableKeyword("_FOG_SCREEN");
			Shader.DisableKeyword("_FOG_OVERLAY");
			Shader.DisableKeyword("_FOG_DODGE");
			break;
		case StylizedFog.StylizedFogMode.Overlay:
			Shader.DisableKeyword("_FOG_BLEND");
			Shader.DisableKeyword("_FOG_ADDITIVE");
			Shader.DisableKeyword("_FOG_MULTIPLY");
			Shader.DisableKeyword("_FOG_SCREEN");
			Shader.EnableKeyword("_FOG_OVERLAY");
			Shader.DisableKeyword("_FOG_DODGE");
			break;
		case StylizedFog.StylizedFogMode.Dodge:
			Shader.DisableKeyword("_FOG_BLEND");
			Shader.DisableKeyword("_FOG_ADDITIVE");
			Shader.DisableKeyword("_FOG_MULTIPLY");
			Shader.DisableKeyword("_FOG_SCREEN");
			Shader.DisableKeyword("_FOG_OVERLAY");
			Shader.EnableKeyword("_FOG_DODGE");
			break;
		}
		if (this.useBlend)
		{
			Shader.EnableKeyword("_FOG_BLEND_ON");
			Shader.DisableKeyword("_FOG_BLEND_OFF");
		}
		else
		{
			Shader.EnableKeyword("_FOG_BLEND_OFF");
			Shader.DisableKeyword("_FOG_BLEND_ON");
		}
		if (this.useNoise)
		{
			Shader.EnableKeyword("_FOG_NOISE_ON");
			Shader.DisableKeyword("_FOG_NOISE_OFF");
		}
		else
		{
			Shader.EnableKeyword("_FOG_NOISE_OFF");
			Shader.DisableKeyword("_FOG_NOISE_ON");
		}
		if (this.ExcludeSkybox)
		{
			Shader.EnableKeyword("_SKYBOX");
			return;
		}
		Shader.DisableKeyword("_SKYBOX");
	}

	// Token: 0x06002A6F RID: 10863 RVA: 0x0009FC14 File Offset: 0x0009DE14
	private void disableKeywords()
	{
		Shader.DisableKeyword("_FOG_BLEND");
		Shader.DisableKeyword("_FOG_ADDITIVE");
		Shader.DisableKeyword("_FOG_MULTIPLY");
		Shader.DisableKeyword("_FOG_SCREEN");
		Shader.DisableKeyword("_FOG_BLEND_OFF");
		Shader.DisableKeyword("_FOG_BLEND_ON");
		Shader.DisableKeyword("_FOG_NOISE_OFF");
		Shader.DisableKeyword("_FOG_NOISE_ON");
	}

	// Token: 0x06002A70 RID: 10864 RVA: 0x0009FC71 File Offset: 0x0009DE71
	private bool isSupported()
	{
		return this.fogShader.isSupported && !(this.fogShader == null);
	}

	// Token: 0x06002A71 RID: 10865 RVA: 0x0009FC91 File Offset: 0x0009DE91
	[ImageEffectOpaque]
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.isSupported())
		{
			Graphics.Blit(source, destination);
			return;
		}
		this.updateValues();
		Graphics.Blit(source, destination, this.fogMat);
	}

	// Token: 0x04002983 RID: 10627
	public StylizedFog.StylizedFogMode fogMode;

	// Token: 0x04002984 RID: 10628
	public bool ExcludeSkybox;

	// Token: 0x04002985 RID: 10629
	[Header("Blend")]
	[Tooltip("Use a second ramp for transition")]
	[SerializeField]
	private bool useBlend;

	// Token: 0x04002986 RID: 10630
	[Tooltip("Amount of blend between 2 gradients")]
	[Range(0f, 1f)]
	public float blend;

	// Token: 0x04002987 RID: 10631
	[Header("Gradients")]
	[Tooltip("Use ramp from textures or gradient fields")]
	public StylizedFog.StylizedFogGradient gradientSource;

	// Token: 0x04002988 RID: 10632
	public Gradient rampGradient;

	// Token: 0x04002989 RID: 10633
	public Gradient rampBlendGradient;

	// Token: 0x0400298A RID: 10634
	public Texture2D rampTexture;

	// Token: 0x0400298B RID: 10635
	public Texture2D rampBlendTexture;

	// Token: 0x0400298C RID: 10636
	[Header("Noise Texture")]
	[SerializeField]
	private bool useNoise;

	// Token: 0x0400298D RID: 10637
	public Texture2D noiseTexture;

	// Token: 0x0400298E RID: 10638
	[Space(5f)]
	[Tooltip("XY: Speed1 XY | WH: Speed2 XY")]
	public Vector4 noiseSpeed;

	// Token: 0x0400298F RID: 10639
	[Space(5f)]
	[Tooltip("XY: Tiling1 XY | WH: Tiling2 XY")]
	public Vector4 noiseTiling = new Vector4(1f, 1f, 1f, 1f);

	// Token: 0x04002990 RID: 10640
	private Camera cam;

	// Token: 0x04002991 RID: 10641
	private Texture2D mainRamp;

	// Token: 0x04002992 RID: 10642
	private Texture2D blendRamp;

	// Token: 0x04002993 RID: 10643
	private Shader fogShader;

	// Token: 0x04002994 RID: 10644
	private Material fogMat;

	// Token: 0x020007EC RID: 2028
	public enum StylizedFogMode
	{
		// Token: 0x04002996 RID: 10646
		Blend,
		// Token: 0x04002997 RID: 10647
		Additive,
		// Token: 0x04002998 RID: 10648
		Multiply,
		// Token: 0x04002999 RID: 10649
		Screen,
		// Token: 0x0400299A RID: 10650
		Overlay,
		// Token: 0x0400299B RID: 10651
		Dodge
	}

	// Token: 0x020007ED RID: 2029
	public enum StylizedFogGradient
	{
		// Token: 0x0400299D RID: 10653
		Textures,
		// Token: 0x0400299E RID: 10654
		Gradients
	}
}
