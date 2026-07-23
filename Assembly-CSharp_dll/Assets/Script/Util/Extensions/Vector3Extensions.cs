using System;
using UnityEngine;

namespace Assets.Script.Util.Extensions
{
	// Token: 0x02000A24 RID: 2596
	public static class Vector3Extensions
	{
		// Token: 0x060045EA RID: 17898 RVA: 0x000CDD50 File Offset: 0x000CBF50
		public static bool IsNaN(this Vector3 instance)
		{
			return float.IsNaN(instance.x) || float.IsNaN(instance.y) || float.IsNaN(instance.z);
		}
	}
}
