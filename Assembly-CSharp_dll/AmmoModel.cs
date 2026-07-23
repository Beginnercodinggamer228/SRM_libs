using System;

// Token: 0x0200010B RID: 267
public class AmmoModel
{
	// Token: 0x060005E5 RID: 1509 RVA: 0x00021DAC File Offset: 0x0001FFAC
	public void IncreaseUsableSlots(int usableSlots)
	{
		this.usableSlots = Math.Max(this.usableSlots, usableSlots);
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x00021DC0 File Offset: 0x0001FFC0
	public int GetSlotMaxCount(Identifiable.Id id, int index)
	{
		return this.slotMaxCountFunction(id, index);
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x00021DCF File Offset: 0x0001FFCF
	public void SetSlotMaxCountFunction(Func<Identifiable.Id, int, int> slotMaxCountFunction)
	{
		this.slotMaxCountFunction = slotMaxCountFunction;
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x00021DD8 File Offset: 0x0001FFD8
	public void Push(Ammo.Slot[] slots)
	{
		this.slots = slots;
	}

	// Token: 0x060005E9 RID: 1513 RVA: 0x00021DE1 File Offset: 0x0001FFE1
	public void Pull(out Ammo.Slot[] slots)
	{
		slots = this.slots;
	}

	// Token: 0x060005EA RID: 1514 RVA: 0x00021DEC File Offset: 0x0001FFEC
	public void Reset(int numSlots, int initUsableSlots, int[] deprecatedMaxSlotCounts)
	{
		this.Reset(numSlots, initUsableSlots, (Identifiable.Id id, int index) => deprecatedMaxSlotCounts[index]);
	}

	// Token: 0x060005EB RID: 1515 RVA: 0x00021E1A File Offset: 0x0002001A
	public void Reset(int numSlots, int initUsableSlots, Func<Identifiable.Id, int, int> initSlotMaxCountFunction)
	{
		this.slots = new Ammo.Slot[numSlots];
		this.usableSlots = initUsableSlots;
		this.slotMaxCountFunction = initSlotMaxCountFunction;
	}

	// Token: 0x040005B4 RID: 1460
	public Ammo.Slot[] slots;

	// Token: 0x040005B5 RID: 1461
	public int usableSlots;

	// Token: 0x040005B6 RID: 1462
	private Func<Identifiable.Id, int, int> slotMaxCountFunction;
}
