using System;

// Token: 0x02000112 RID: 274
public class DebugDateProvider : IDateProvider
{
	// Token: 0x060005F7 RID: 1527 RVA: 0x00021F1D File Offset: 0x0002011D
	public DebugDateProvider(DateTime date)
	{
		this.date = new DateTime(date.Year, date.Month, date.Day);
	}

	// Token: 0x060005F8 RID: 1528 RVA: 0x00021F45 File Offset: 0x00020145
	public DateTime GetToday()
	{
		return this.date;
	}

	// Token: 0x040005C7 RID: 1479
	private DateTime date;
}
