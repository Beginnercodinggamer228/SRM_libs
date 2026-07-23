using System;

// Token: 0x02000567 RID: 1383
public class DecorizerSlotUI : StorageSlotUI
{
	// Token: 0x06001CCF RID: 7375 RVA: 0x0006DC3C File Offset: 0x0006BE3C
	public override void Awake()
	{
		base.Awake();
		this.storage = base.GetComponentInParent<DecorizerStorage>();
	}

	// Token: 0x06001CD0 RID: 7376 RVA: 0x0006DC50 File Offset: 0x0006BE50
	protected override Identifiable.Id GetCurrentId()
	{
		return this.storage.selected;
	}

	// Token: 0x06001CD1 RID: 7377 RVA: 0x0006DC5D File Offset: 0x0006BE5D
	protected override int GetCurrentCount()
	{
		return this.storage.GetCount();
	}

	// Token: 0x04001BE6 RID: 7142
	private DecorizerStorage storage;
}
