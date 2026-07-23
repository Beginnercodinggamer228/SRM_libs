using System;
using System.Collections.Generic;

// Token: 0x0200045B RID: 1115
public class SlimeAppearanceObjectComparer : IEqualityComparer<SlimeAppearanceObject>
{
	// Token: 0x06001700 RID: 5888 RVA: 0x00059600 File Offset: 0x00057800
	public bool Equals(SlimeAppearanceObject id1, SlimeAppearanceObject id2)
	{
		return id1.GetInstanceID() == id2.GetInstanceID();
	}

	// Token: 0x06001701 RID: 5889 RVA: 0x00057F69 File Offset: 0x00056169
	public int GetHashCode(SlimeAppearanceObject obj)
	{
		return obj.GetHashCode();
	}
}
