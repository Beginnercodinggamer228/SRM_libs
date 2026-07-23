using System;
using UnityEngine.UI;

// Token: 0x0200037D RID: 893
public class SRToggle : Toggle
{
	// Token: 0x0600126D RID: 4717 RVA: 0x0004935E File Offset: 0x0004755E
	protected override void OnEnable()
	{
		base.OnEnable();
		if (base.group != null)
		{
			((SRToggleGroup)base.group).OnToggleEnable(this);
		}
	}

	// Token: 0x0600126E RID: 4718 RVA: 0x00049385 File Offset: 0x00047585
	protected override void OnDisable()
	{
		if (base.group != null)
		{
			((SRToggleGroup)base.group).OnToggleWillDisable(this);
		}
		base.OnDisable();
	}

	// Token: 0x0600126F RID: 4719 RVA: 0x000493AC File Offset: 0x000475AC
	protected override void OnDestroy()
	{
		if (base.group != null)
		{
			((SRToggleGroup)base.group).OnToggleEnable(this);
		}
		base.OnDestroy();
	}
}
