using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006AD RID: 1709
public static class UnityWorkarounds
{
	// Token: 0x060023A2 RID: 9122 RVA: 0x0008A145 File Offset: 0x00088345
	public static void SafeRemoveAllNulls<T>(HashSet<T> inputSet) where T : UnityEngine.Object
	{
		inputSet.RemoveWhere((T o) => o == null);
	}
}
