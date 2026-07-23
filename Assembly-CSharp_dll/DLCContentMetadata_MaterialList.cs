using System;
using UnityEngine;

// Token: 0x02000103 RID: 259
[CreateAssetMenu(fileName = "DLC", menuName = "DLC/Content/Shader Materials Metadata")]
public class DLCContentMetadata_MaterialList : DLCContentMetadata
{
	// Token: 0x060005CF RID: 1487 RVA: 0x00021BCC File Offset: 0x0001FDCC
	public override void Register()
	{
		SRSingleton<GameContext>.Instance.SlimeShaders.RegisterAdditionalMaterials(this.cloakableMaterials);
	}

	// Token: 0x040005A2 RID: 1442
	[Tooltip("The set of added materials that can be cloaked.")]
	public Material[] cloakableMaterials;
}
