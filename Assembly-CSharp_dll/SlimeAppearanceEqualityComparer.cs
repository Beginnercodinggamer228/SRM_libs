using System;
using System.Collections.Generic;

// Token: 0x02000447 RID: 1095
public class SlimeAppearanceEqualityComparer : IEqualityComparer<SlimeAppearance>
{
	// Token: 0x060016A3 RID: 5795 RVA: 0x00057F60 File Offset: 0x00056160
	public bool Equals(SlimeAppearance x, SlimeAppearance y)
	{
		return x == y;
	}

	// Token: 0x060016A4 RID: 5796 RVA: 0x00057F69 File Offset: 0x00056169
	public int GetHashCode(SlimeAppearance obj)
	{
		return obj.GetHashCode();
	}

	// Token: 0x040015EC RID: 5612
	public static SlimeAppearanceEqualityComparer Default = new SlimeAppearanceEqualityComparer();
}
