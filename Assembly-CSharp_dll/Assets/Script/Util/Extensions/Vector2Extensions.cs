using System;
using UnityEngine;

namespace Assets.Script.Util.Extensions
{
	// Token: 0x02000A23 RID: 2595
	public static class Vector2Extensions
	{
		// Token: 0x060045E7 RID: 17895 RVA: 0x000CDD1B File Offset: 0x000CBF1B
		public static float GetRandom(this Vector2 range)
		{
			return range.GetRandom(Randoms.SHARED);
		}

		// Token: 0x060045E8 RID: 17896 RVA: 0x000CDD28 File Offset: 0x000CBF28
		public static float GetRandom(this Vector2 range, Randoms random)
		{
			return random.GetInRange(range.x, range.y);
		}

		// Token: 0x060045E9 RID: 17897 RVA: 0x000CDD3C File Offset: 0x000CBF3C
		public static float Lerp(this Vector2 range, float t)
		{
			return Mathf.Lerp(range.x, range.y, t);
		}
	}
}
