using System;
using UnityEngine;

// Token: 0x020005DF RID: 1503
public class PediaUIActivator : UIActivator
{
	// Token: 0x06001F9E RID: 8094 RVA: 0x000786D7 File Offset: 0x000768D7
	public override GameObject Activate()
	{
		return SRSingleton<SceneContext>.Instance.PediaDirector.ShowPedia(this.pediaId);
	}

	// Token: 0x04001ED4 RID: 7892
	public PediaDirector.Id pediaId;
}
