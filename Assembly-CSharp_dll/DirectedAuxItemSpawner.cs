using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006E7 RID: 1767
public class DirectedAuxItemSpawner : SRBehaviour
{
	// Token: 0x060024DF RID: 9439 RVA: 0x0008DCF4 File Offset: 0x0008BEF4
	public void Start()
	{
		this.cellDir = base.GetComponentInParent<CellDirector>();
		this.zoneDirector = base.GetComponentInParent<ZoneDirector>();
		this.zoneDirector.Register(this);
	}

	// Token: 0x060024E0 RID: 9440 RVA: 0x0008DD1A File Offset: 0x0008BF1A
	public void Spawn(GameObject prefab)
	{
		SRBehaviour.InstantiateActor(prefab, this.zoneDirector.regionSetId, base.transform.position, base.transform.rotation, false);
	}

	// Token: 0x060024E1 RID: 9441 RVA: 0x0008DD48 File Offset: 0x0008BF48
	public void UnspawnIfPresent(IEnumerable<Identifiable.Id> ids)
	{
		List<GameObject> list = new List<GameObject>();
		foreach (Identifiable.Id id in ids)
		{
			this.cellDir.Get(id, ref list);
		}
		foreach (GameObject gameObject in list)
		{
			if ((gameObject.transform.position - base.transform.position).sqrMagnitude < 1.0000001E-06f)
			{
				Destroyer.DestroyActor(gameObject, "DirectedAuxItemSpawner.UnspawnIfPresent", false);
			}
		}
	}

	// Token: 0x040023CD RID: 9165
	private CellDirector cellDir;

	// Token: 0x040023CE RID: 9166
	private ZoneDirector zoneDirector;

	// Token: 0x040023CF RID: 9167
	private const float PRESENT_DIST = 0.001f;

	// Token: 0x040023D0 RID: 9168
	private const float SQR_PRESENT_DIST = 1.0000001E-06f;
}
