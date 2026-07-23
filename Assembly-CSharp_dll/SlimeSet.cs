using System;
using UnityEngine;

// Token: 0x02000784 RID: 1924
[Serializable]
public class SlimeSet
{
	// Token: 0x040027D1 RID: 10193
	public SlimeSet.Member[] members;

	// Token: 0x02000785 RID: 1925
	[Serializable]
	public class Member
	{
		// Token: 0x040027D2 RID: 10194
		public GameObject prefab;

		// Token: 0x040027D3 RID: 10195
		public float weight = 1f;
	}
}
