using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000367 RID: 871
public static class SRBehaviourStatic
{
	// Token: 0x06001207 RID: 4615 RVA: 0x00047A55 File Offset: 0x00045C55
	public static I GetInterfaceComponent<I>(this GameObject obj) where I : class
	{
		return obj.GetComponent(typeof(I)) as I;
	}

	// Token: 0x06001208 RID: 4616 RVA: 0x00047A71 File Offset: 0x00045C71
	public static I GetInterfaceComponentInParent<I>(this GameObject obj) where I : class
	{
		return obj.GetComponentInParent(typeof(I)) as I;
	}

	// Token: 0x06001209 RID: 4617 RVA: 0x00047A90 File Offset: 0x00045C90
	public static V Get<K, V>(this Dictionary<K, V> dict, K key)
	{
		V result;
		dict.TryGetValue(key, out result);
		return result;
	}
}
