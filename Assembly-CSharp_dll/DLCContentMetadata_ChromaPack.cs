using System;
using UnityEngine;

// Token: 0x02000101 RID: 257
[CreateAssetMenu(fileName = "DLC", menuName = "DLC/Content/Chroma Pack Metadata")]
public class DLCContentMetadata_ChromaPack : DLCContentMetadata
{
	// Token: 0x060005CB RID: 1483 RVA: 0x00021B43 File Offset: 0x0001FD43
	public override void Register()
	{
		SRSingleton<SceneContext>.Instance.RanchDirector.RegisterPalette(this.paletteEntry);
	}

	// Token: 0x0400059E RID: 1438
	public RanchDirector.PaletteEntry paletteEntry;
}
