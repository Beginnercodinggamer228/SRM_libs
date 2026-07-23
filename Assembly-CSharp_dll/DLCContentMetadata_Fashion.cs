using System;
using UnityEngine;

// Token: 0x02000102 RID: 258
[CreateAssetMenu(fileName = "DLC", menuName = "DLC/Content/Fashion Metadata")]
public class DLCContentMetadata_Fashion : DLCContentMetadata
{
	// Token: 0x060005CD RID: 1485 RVA: 0x00021B64 File Offset: 0x0001FD64
	public override void Register()
	{
		SRSingleton<GameContext>.Instance.LookupDirector.RegisterFashion(this.prefab, this.vacItemDefinition, this.gadgetDefinition);
		SRSingleton<SceneContext>.Instance.GameModel.GetPlayerModel().RegisterPotentialAmmo(PlayerState.AmmoMode.DEFAULT, this.prefab);
		SRSingleton<SceneContext>.Instance.GameModel.GetGadgetsModel().RegisterBlueprint(this.gadgetDefinition.id);
	}

	// Token: 0x0400059F RID: 1439
	public GameObject prefab;

	// Token: 0x040005A0 RID: 1440
	public VacItemDefinition vacItemDefinition;

	// Token: 0x040005A1 RID: 1441
	public GadgetDefinition gadgetDefinition;
}
