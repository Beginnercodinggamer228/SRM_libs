using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000226 RID: 550
public abstract class ListAsset<T> : ScriptableObject, IEnumerable<T>, IEnumerable
{
	// Token: 0x1700015D RID: 349
	public T this[int index]
	{
		get
		{
			return this.items[index];
		}
	}

	// Token: 0x1700015E RID: 350
	// (get) Token: 0x06000BD2 RID: 3026 RVA: 0x0003178E File Offset: 0x0002F98E
	public int Count
	{
		get
		{
			return this.items.Count;
		}
	}

	// Token: 0x06000BD3 RID: 3027 RVA: 0x0003179B File Offset: 0x0002F99B
	public IEnumerator<T> GetEnumerator()
	{
		return this.items.GetEnumerator();
	}

	// Token: 0x06000BD4 RID: 3028 RVA: 0x000317AD File Offset: 0x0002F9AD
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	// Token: 0x04000ABC RID: 2748
	[SerializeField]
	private List<T> items = new List<T>();
}
