using System;
using UnityEngine;

// Token: 0x020003B7 RID: 951
public class DervishVortexAdder : VortexAdder
{
	// Token: 0x060013D8 RID: 5080 RVA: 0x0004CF40 File Offset: 0x0004B140
	protected override bool CanAdd(GameObject gameObj)
	{
		if (!base.CanAdd(gameObj))
		{
			return false;
		}
		Identifiable.Id id = Identifiable.GetId(gameObj);
		return Identifiable.IsNonSlimeResource(id) || (this.allowNonDervishSlimes && Identifiable.IsSlime(id) && gameObj.GetComponent<DervishSlimeSpin>() == null);
	}

	// Token: 0x0400129A RID: 4762
	public bool allowNonDervishSlimes;
}
