using System;

// Token: 0x02000681 RID: 1665
internal class LRUCacheItem<K, V>
{
	// Token: 0x0600226C RID: 8812 RVA: 0x0008504F File Offset: 0x0008324F
	public LRUCacheItem(K k, V v)
	{
		this.key = k;
		this.internalValue = new WeakReference(v);
	}

	// Token: 0x17000232 RID: 562
	// (get) Token: 0x0600226D RID: 8813 RVA: 0x0008506F File Offset: 0x0008326F
	public V value
	{
		get
		{
			return (V)((object)this.internalValue.Target);
		}
	}

	// Token: 0x0400223B RID: 8763
	public K key;

	// Token: 0x0400223C RID: 8764
	private readonly WeakReference internalValue;
}
