using System;
using System.Collections.Generic;

namespace Assets.Script.Util.Extensions
{
	// Token: 0x02000A1E RID: 2590
	public static class LinqExtensions
	{
		// Token: 0x060045CE RID: 17870 RVA: 0x000CDAF7 File Offset: 0x000CBCF7
		public static IEnumerable<T> ToEnumerable<T>(this T item)
		{
			yield return item;
			yield break;
		}

		// Token: 0x060045CF RID: 17871 RVA: 0x000CDB07 File Offset: 0x000CBD07
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items)
		{
			return new HashSet<T>(items);
		}

		// Token: 0x060045D0 RID: 17872 RVA: 0x000CDB0F File Offset: 0x000CBD0F
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items, IEqualityComparer<T> comparer)
		{
			return new HashSet<T>(items, comparer);
		}
	}
}
