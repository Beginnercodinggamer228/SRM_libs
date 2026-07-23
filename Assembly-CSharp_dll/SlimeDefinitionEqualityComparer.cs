using System;
using System.Collections.Generic;

// Token: 0x02000462 RID: 1122
public class SlimeDefinitionEqualityComparer : IEqualityComparer<SlimeDefinition>
{
	// Token: 0x06001721 RID: 5921 RVA: 0x00057F60 File Offset: 0x00056160
	public bool Equals(SlimeDefinition x, SlimeDefinition y)
	{
		return x == y;
	}

	// Token: 0x06001722 RID: 5922 RVA: 0x00057F69 File Offset: 0x00056169
	public int GetHashCode(SlimeDefinition obj)
	{
		return obj.GetHashCode();
	}

	// Token: 0x04001654 RID: 5716
	public static SlimeDefinitionEqualityComparer Default = new SlimeDefinitionEqualityComparer();
}
