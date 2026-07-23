using System;
using UnityEngine;

// Token: 0x02000008 RID: 8
[RequireComponent(typeof(Camera))]
[ImageEffectAllowedInSceneView]
[ExecuteInEditMode]
public class Pixelize : MonoBehaviour
{
	// Token: 0x17000013 RID: 19
	// (get) Token: 0x06000029 RID: 41 RVA: 0x00002AA4 File Offset: 0x00000CA4
	private Shader ScreenAndMaskShader
	{
		get
		{
			if (this._screenAndMaskShader == null)
			{
				this._screenAndMaskShader = Shader.Find("Hidden/PostProcess/Pixelize/ScreenAndMask");
			}
			return this._screenAndMaskShader;
		}
	}

	// Token: 0x17000014 RID: 20
	// (get) Token: 0x0600002A RID: 42 RVA: 0x00002ACA File Offset: 0x00000CCA
	private Material ScreenAndMaskMaterial
	{
		get
		{
			if (this._screenAndMaskMaterial == null)
			{
				this._screenAndMaskMaterial = new Material(this.ScreenAndMaskShader);
			}
			return this._screenAndMaskMaterial;
		}
	}

	// Token: 0x17000015 RID: 21
	// (get) Token: 0x0600002B RID: 43 RVA: 0x00002AF1 File Offset: 0x00000CF1
	private RenderTexture TemporaryRenderTarget
	{
		get
		{
			if (this._temporaryRenderTexture == null)
			{
				this.CreateTemporaryRenderTarget();
			}
			return this._temporaryRenderTexture;
		}
	}

	// Token: 0x17000016 RID: 22
	// (get) Token: 0x0600002C RID: 44 RVA: 0x00002B0D File Offset: 0x00000D0D
	private Shader CombineLayersShader
	{
		get
		{
			if (this._combineLayersShader == null)
			{
				this._combineLayersShader = Shader.Find("Hidden/PostProcess/Pixelize/CombineLayers");
			}
			return this._combineLayersShader;
		}
	}

	// Token: 0x17000017 RID: 23
	// (get) Token: 0x0600002D RID: 45 RVA: 0x00002B33 File Offset: 0x00000D33
	private Material CombineLayersMaterial
	{
		get
		{
			if (this._combineLayersMaterial == null)
			{
				this._combineLayersMaterial = new Material(this.CombineLayersShader);
			}
			return this._combineLayersMaterial;
		}
	}

	// Token: 0x0600002E RID: 46 RVA: 0x00002B5A File Offset: 0x00000D5A
	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		this.CheckTemporaryRenderTarget();
		Graphics.Blit(src, this.TemporaryRenderTarget, this.ScreenAndMaskMaterial);
		Graphics.Blit(this.TemporaryRenderTarget, dest, this.CombineLayersMaterial);
	}

	// Token: 0x0600002F RID: 47 RVA: 0x00002B88 File Offset: 0x00000D88
	private void CreateTemporaryRenderTarget()
	{
		this._temporaryRenderTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
		this._temporaryRenderTexture.useMipMap = true;
		this._temporaryRenderTexture.autoGenerateMips = true;
		this._temporaryRenderTexture.wrapMode = TextureWrapMode.Clamp;
		this._temporaryRenderTexture.filterMode = FilterMode.Point;
		this._temporaryRenderTexture.Create();
	}

	// Token: 0x06000030 RID: 48 RVA: 0x00002BE9 File Offset: 0x00000DE9
	private void CheckTemporaryRenderTarget()
	{
		if (this.TemporaryRenderTarget.width != Screen.width || this.TemporaryRenderTarget.width != Screen.height)
		{
			this.ReleaseTemporaryRenderTarget();
		}
	}

	// Token: 0x06000031 RID: 49 RVA: 0x00002C15 File Offset: 0x00000E15
	private void ReleaseTemporaryRenderTarget()
	{
		this._temporaryRenderTexture.Release();
		this._temporaryRenderTexture = null;
	}

	// Token: 0x04000013 RID: 19
	private Shader _screenAndMaskShader;

	// Token: 0x04000014 RID: 20
	private Material _screenAndMaskMaterial;

	// Token: 0x04000015 RID: 21
	private RenderTexture _temporaryRenderTexture;

	// Token: 0x04000016 RID: 22
	private Shader _combineLayersShader;

	// Token: 0x04000017 RID: 23
	private Material _combineLayersMaterial;
}
