using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000407 RID: 1031
public class MaterialStealthController
{
	// Token: 0x06001589 RID: 5513 RVA: 0x00053C88 File Offset: 0x00051E88
	public MaterialStealthController(GameObject gameObject)
	{
		this.slimeShaders = SRSingleton<GameContext>.Instance.SlimeShaders;
		this.cloakMaterial = this.slimeShaders.cloakMaterial;
		this.UpdateMaterials(gameObject);
	}

	// Token: 0x0600158A RID: 5514 RVA: 0x00053CF0 File Offset: 0x00051EF0
	public void UpdateMaterials(GameObject gameObject)
	{
		this.cloakingMats.Clear();
		this.renderers.Clear();
		this.rendererOriginalMaterials.Clear();
		foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
		{
			foreach (Material material in renderer.sharedMaterials)
			{
				if (material != null && this.slimeShaders.cloakableShaders.Contains(material.shader))
				{
					this.cloakingMats.Add(material);
					if (material.HasProperty(MaterialStealthController.topColorPropertyId))
					{
						this.colorsPropertyBlock.SetColor(MaterialStealthController.topColorPropertyId, material.GetColor(MaterialStealthController.topColorPropertyId));
						this.colorsPropertyBlock.SetColor(MaterialStealthController.middleColorPropertyId, material.GetColor(MaterialStealthController.middleColorPropertyId));
						this.colorsPropertyBlock.SetColor(MaterialStealthController.bottomColorPropertyId, material.GetColor(MaterialStealthController.bottomColorPropertyId));
					}
				}
			}
			this.renderers.Add(renderer);
			this.rendererOriginalMaterials[renderer] = renderer.sharedMaterials.ToArray<Material>();
		}
		this.cloakingMats.Add(this.cloakMaterial);
	}

	// Token: 0x0600158B RID: 5515 RVA: 0x00053E30 File Offset: 0x00052030
	public void SetOpacity(float opacity)
	{
		bool flag = opacity >= 0.99f;
		bool flag2 = false;
		for (int i = 0; i < this.renderers.Count; i++)
		{
			Renderer renderer2 = this.renderers[i];
			if (renderer2 == null)
			{
				flag2 = true;
			}
			else
			{
				Material[] sharedMaterials = renderer2.sharedMaterials;
				for (int j = 0; j < sharedMaterials.Length; j++)
				{
					Material material = sharedMaterials[j];
					if (this.cloakingMats.Contains(material))
					{
						if (!flag && material != this.cloakMaterial)
						{
							sharedMaterials[j] = this.cloakMaterial;
						}
						else if (flag && material == this.cloakMaterial)
						{
							sharedMaterials[j] = this.rendererOriginalMaterials[renderer2][j];
						}
						MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
						renderer2.GetPropertyBlock(materialPropertyBlock, j);
						materialPropertyBlock.SetFloat(MaterialStealthController.alphaPropertyId, flag ? 1f : opacity);
						materialPropertyBlock.SetColor(MaterialStealthController.topColorPropertyId, this.colorsPropertyBlock.GetColor(MaterialStealthController.topColorPropertyId));
						materialPropertyBlock.SetColor(MaterialStealthController.middleColorPropertyId, this.colorsPropertyBlock.GetColor(MaterialStealthController.middleColorPropertyId));
						materialPropertyBlock.SetColor(MaterialStealthController.bottomColorPropertyId, this.colorsPropertyBlock.GetColor(MaterialStealthController.bottomColorPropertyId));
						renderer2.SetPropertyBlock(materialPropertyBlock, j);
					}
				}
				renderer2.sharedMaterials = sharedMaterials;
			}
		}
		if (flag2)
		{
			this.renderers.RemoveAll((Renderer renderer) => renderer == null);
		}
	}

	// Token: 0x04001487 RID: 5255
	private const float CLOAK_THRESHOLD = 0.99f;

	// Token: 0x04001488 RID: 5256
	private static readonly int alphaPropertyId = Shader.PropertyToID("_Alpha");

	// Token: 0x04001489 RID: 5257
	private static readonly int topColorPropertyId = Shader.PropertyToID("_TopColor");

	// Token: 0x0400148A RID: 5258
	private static readonly int middleColorPropertyId = Shader.PropertyToID("_MiddleColor");

	// Token: 0x0400148B RID: 5259
	private static readonly int bottomColorPropertyId = Shader.PropertyToID("_BottomColor");

	// Token: 0x0400148C RID: 5260
	private readonly Material cloakMaterial;

	// Token: 0x0400148D RID: 5261
	private readonly HashSet<Material> cloakingMats = new HashSet<Material>();

	// Token: 0x0400148E RID: 5262
	private readonly List<Renderer> renderers = new List<Renderer>();

	// Token: 0x0400148F RID: 5263
	private readonly Dictionary<Renderer, Material[]> rendererOriginalMaterials = new Dictionary<Renderer, Material[]>();

	// Token: 0x04001490 RID: 5264
	private SlimeShaders slimeShaders;

	// Token: 0x04001491 RID: 5265
	private readonly MaterialPropertyBlock colorsPropertyBlock = new MaterialPropertyBlock();
}
