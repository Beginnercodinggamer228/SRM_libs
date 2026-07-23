using System;
using System.Collections.Generic;

// Token: 0x02000261 RID: 609
[Serializable]
public class PediaData : DataModule<PediaData>
{
	// Token: 0x06000CE6 RID: 3302 RVA: 0x00003296 File Offset: 0x00001496
	public static void AssertEquals(PediaData dataA, PediaData dataB)
	{
	}

	// Token: 0x04000BF3 RID: 3059
	public const int CURR_FORMAT_ID = 1;

	// Token: 0x04000BF4 RID: 3060
	public List<string> unlockedIds = new List<string>();

	// Token: 0x04000BF5 RID: 3061
	public List<string> completedTuts = new List<string>();

	// Token: 0x04000BF6 RID: 3062
	public int progressGivenForPediaCount;
}
