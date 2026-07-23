using System;
using UnityEngine;

// Token: 0x02000105 RID: 261
[CreateAssetMenu(fileName = "DLC", menuName = "DLC/Content/Toy Metadata")]
public class DLCContentMetadata_Toy : DLCContentMetadata
{
	// Token: 0x060005D3 RID: 1491 RVA: 0x00021BF6 File Offset: 0x0001FDF6
	public override void Register()
	{
		SRSingleton<GameContext>.Instance.LookupDirector.RegisterToy(this.toyDefinition, this.prefab);
		SRSingleton<GameContext>.Instance.ToyDirector.Register(this.toyDefinition.ToyId);
	}

	// Token: 0x040005A5 RID: 1445
	public GameObject prefab;

	// Token: 0x040005A6 RID: 1446
	public ToyDefinition toyDefinition;
}
