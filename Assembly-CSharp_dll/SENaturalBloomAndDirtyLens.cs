using System;
using UnityEngine;

// Token: 0x020007EA RID: 2026
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Sonic Ether/SE Natural Bloom and Dirty Lens")]
public class SENaturalBloomAndDirtyLens : MonoBehaviour
{
	// Token: 0x06002A61 RID: 10849 RVA: 0x0009F37C File Offset: 0x0009D57C
	private void Start()
	{
		this.isSupported = true;
		if (!this.material)
		{
			this.material = new Material(this.shader);
		}
		if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
		{
			this.isSupported = false;
		}
	}

	// Token: 0x06002A62 RID: 10850 RVA: 0x0009F3B2 File Offset: 0x0009D5B2
	private void OnDisable()
	{
		if (this.material)
		{
			UnityEngine.Object.DestroyImmediate(this.material);
		}
	}

	// Token: 0x06002A63 RID: 10851 RVA: 0x0009F3CC File Offset: 0x0009D5CC
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.isSupported)
		{
			Graphics.Blit(source, destination);
			return;
		}
		if (!this.material)
		{
			this.material = new Material(this.shader);
		}
		this.material.hideFlags = HideFlags.HideAndDontSave;
		this.material.SetFloat("_BloomIntensity", Mathf.Exp(this.bloomIntensity) - 1f);
		this.material.SetFloat("_LensDirtIntensity", Mathf.Exp(this.lensDirtIntensity) - 1f);
		source.filterMode = FilterMode.Bilinear;
		int num = source.width / 2;
		int num2 = source.height / 2;
		RenderTexture source2 = source;
		int num3 = 2;
		for (int i = 0; i < 6; i++)
		{
			RenderTexture renderTexture = RenderTexture.GetTemporary(num, num2, 0, source.format);
			renderTexture.filterMode = FilterMode.Bilinear;
			Graphics.Blit(source2, renderTexture, this.material, 1);
			source2 = renderTexture;
			float num4;
			if (i > 1)
			{
				num4 = 1f;
			}
			else
			{
				num4 = 0.5f;
			}
			if (i == 2)
			{
				num4 = 0.75f;
			}
			for (int j = 0; j < num3; j++)
			{
				this.material.SetFloat("_BlurSize", (this.blurSize * 0.5f + (float)j) * num4);
				RenderTexture temporary = RenderTexture.GetTemporary(num, num2, 0, source.format);
				temporary.filterMode = FilterMode.Bilinear;
				Graphics.Blit(renderTexture, temporary, this.material, 2);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
				temporary = RenderTexture.GetTemporary(num, num2, 0, source.format);
				temporary.filterMode = FilterMode.Bilinear;
				Graphics.Blit(renderTexture, temporary, this.material, 3);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
			}
			switch (i)
			{
			case 0:
				this.material.SetTexture("_Bloom0", renderTexture);
				break;
			case 1:
				this.material.SetTexture("_Bloom1", renderTexture);
				break;
			case 2:
				this.material.SetTexture("_Bloom2", renderTexture);
				break;
			case 3:
				this.material.SetTexture("_Bloom3", renderTexture);
				break;
			case 4:
				this.material.SetTexture("_Bloom4", renderTexture);
				break;
			case 5:
				this.material.SetTexture("_Bloom5", renderTexture);
				break;
			}
			RenderTexture.ReleaseTemporary(renderTexture);
			num /= 2;
			num2 /= 2;
		}
		this.material.SetTexture("_LensDirt", this.lensDirtTexture);
		Graphics.Blit(source, destination, this.material, 0);
	}

	// Token: 0x0400297B RID: 10619
	[Range(0f, 0.4f)]
	public float bloomIntensity = 0.05f;

	// Token: 0x0400297C RID: 10620
	public Shader shader;

	// Token: 0x0400297D RID: 10621
	private Material material;

	// Token: 0x0400297E RID: 10622
	public Texture2D lensDirtTexture;

	// Token: 0x0400297F RID: 10623
	[Range(0f, 0.95f)]
	public float lensDirtIntensity = 0.05f;

	// Token: 0x04002980 RID: 10624
	private bool isSupported;

	// Token: 0x04002981 RID: 10625
	private float blurSize = 4f;

	// Token: 0x04002982 RID: 10626
	public bool inputIsHDR;
}
