using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Script.Util.Extensions
{
	// Token: 0x02000A20 RID: 2592
	public static class TransformExtensions
	{
		// Token: 0x060045D9 RID: 17881 RVA: 0x000CDBD7 File Offset: 0x000CBDD7
		public static string GetHierarchyString(this Transform transform)
		{
			return string.Join("/", (from it in transform.GetHierarchy()
			select it.name).ToArray<string>());
		}

		// Token: 0x060045DA RID: 17882 RVA: 0x000CDC12 File Offset: 0x000CBE12
		public static IEnumerable<Transform> GetHierarchy(this Transform transform)
		{
			return transform.GetAscendents().Reverse<Transform>();
		}

		// Token: 0x060045DB RID: 17883 RVA: 0x000CDC1F File Offset: 0x000CBE1F
		private static IEnumerable<Transform> GetAscendents(this Transform transform)
		{
			Transform current = transform;
			while (current != null)
			{
				yield return current;
				current = current.parent;
			}
			yield break;
		}
	}
}
