using System;
using UnityEngine;

// Token: 0x02000617 RID: 1559
public class SiloSlotUI : StorageSlotUI
{
	// Token: 0x060020AE RID: 8366 RVA: 0x0007CEA8 File Offset: 0x0007B0A8
	public override void Awake()
	{
		base.Awake();
		this.storage = base.GetComponentInParent<SiloStorage>();
	}

	// Token: 0x060020AF RID: 8367 RVA: 0x0007CEBC File Offset: 0x0007B0BC
	protected override Identifiable.Id GetCurrentId()
	{
		return this.storage.GetSlotIdentifiable(this.slotIdx);
	}

	// Token: 0x060020B0 RID: 8368 RVA: 0x0007CECF File Offset: 0x0007B0CF
	protected override int GetCurrentCount()
	{
		return this.storage.GetSlotCount(this.slotIdx);
	}

	// Token: 0x04002006 RID: 8198
	[Tooltip("Silo slot index.")]
	public int slotIdx;

	// Token: 0x04002007 RID: 8199
	private SiloStorage storage;
}
