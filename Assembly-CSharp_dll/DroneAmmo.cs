using System;

// Token: 0x02000126 RID: 294
public class DroneAmmo : Ammo
{
	// Token: 0x0600066C RID: 1644 RVA: 0x00022CC0 File Offset: 0x00020EC0
	public DroneAmmo() : base(SRSingleton<SceneContext>.Instance.PlayerState.GetPotentialAmmo(), 1, 1, new Predicate<Identifiable.Id>[1], (Identifiable.Id id, int index) => 50)
	{
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x00022D00 File Offset: 0x00020F00
	public Identifiable.Id Pop()
	{
		Identifiable.Id slotName = this.GetSlotName();
		base.Decrement(slotName, 1);
		return slotName;
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x00022D1D File Offset: 0x00020F1D
	public Identifiable.Id GetSlotName()
	{
		return base.GetSlotName(0);
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x00022D26 File Offset: 0x00020F26
	public bool MaybeAddToSlot(Identifiable.Id id)
	{
		return base.MaybeAddToSpecificSlot(id, null, 0);
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x00022D31 File Offset: 0x00020F31
	public new bool IsEmpty()
	{
		return this.GetSlotCount() <= 0;
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x00022D3F File Offset: 0x00020F3F
	public bool IsFull()
	{
		return this.GetSlotCount() >= this.GetSlotMaxCount();
	}

	// Token: 0x06000672 RID: 1650 RVA: 0x00022D52 File Offset: 0x00020F52
	public int GetSlotCount()
	{
		return base.GetSlotCount(0);
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x00022D5B File Offset: 0x00020F5B
	public int GetSlotMaxCount()
	{
		return base.GetSlotMaxCount(0);
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x00022D64 File Offset: 0x00020F64
	public new bool CouldAddToSlot(Identifiable.Id id)
	{
		return base.CouldAddToSlot(id, 0, false);
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x00022D6F File Offset: 0x00020F6F
	public bool Any()
	{
		return this.GetSlotCount() > 0;
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x00022D31 File Offset: 0x00020F31
	public bool None()
	{
		return this.GetSlotCount() <= 0;
	}

	// Token: 0x04000607 RID: 1543
	public const int MAX_COUNT = 50;
}
