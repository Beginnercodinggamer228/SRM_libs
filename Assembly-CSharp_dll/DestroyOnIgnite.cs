using System;
using UnityEngine;

// Token: 0x020003BA RID: 954
public class DestroyOnIgnite : SRBehaviour, Ignitable
{
	// Token: 0x060013E7 RID: 5095 RVA: 0x0004D171 File Offset: 0x0004B371
	public void Ignite(GameObject igniter)
	{
		if (this.igniteFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.igniteFX);
		}
		Destroyer.DestroyActor(base.gameObject, "DestroyOnIgnite.Ignite", false);
	}

	// Token: 0x040012A2 RID: 4770
	public GameObject igniteFX;
}
