using System;
using UnityEngine;

// Token: 0x0200045A RID: 1114
public interface SlimeAppearanceObjectProvider
{
	// Token: 0x060016FE RID: 5886
	SlimeAppearanceObject Get(SlimeAppearanceObject appearanceObjectPrefab, GameObject targetParent);

	// Token: 0x060016FF RID: 5887
	void Put(SlimeAppearanceObject appearanceObjectPrefab, SlimeAppearanceObject appearanceObject);
}
