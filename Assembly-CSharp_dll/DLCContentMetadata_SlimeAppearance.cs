using System;
using UnityEngine;

// Token: 0x02000104 RID: 260
[CreateAssetMenu(fileName = "DLC", menuName = "DLC/Content/Slime Appearance Metadata")]
public class DLCContentMetadata_SlimeAppearance : DLCContentMetadata
{
	// Token: 0x060005D1 RID: 1489 RVA: 0x00021BE3 File Offset: 0x0001FDE3
	public override void Register()
	{
		this.definition.RegisterDynamicAppearance(this.appearance);
	}

	// Token: 0x040005A3 RID: 1443
	[Tooltip("SlimeDefinition to add the appearance to.")]
	public SlimeDefinition definition;

	// Token: 0x040005A4 RID: 1444
	[Tooltip("SlimeAppearance to add to the definition.")]
	public SlimeAppearance appearance;
}
