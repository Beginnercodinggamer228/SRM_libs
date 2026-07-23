using System;

// Token: 0x02000714 RID: 1812
public class GlitchStorageUI : StorageSlotUI
{
	// Token: 0x060025DC RID: 9692 RVA: 0x000910F7 File Offset: 0x0008F2F7
	public override void Awake()
	{
		base.Awake();
		this.storage = base.GetComponentInParent<GlitchStorage>();
	}

	// Token: 0x060025DD RID: 9693 RVA: 0x0009110B File Offset: 0x0008F30B
	protected override Identifiable.Id GetCurrentId()
	{
		return this.storage.selected;
	}

	// Token: 0x060025DE RID: 9694 RVA: 0x00091118 File Offset: 0x0008F318
	protected override int GetCurrentCount()
	{
		return this.storage.count;
	}

	// Token: 0x04002540 RID: 9536
	private GlitchStorage storage;
}
