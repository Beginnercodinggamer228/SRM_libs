using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x02000684 RID: 1668
public class Lookup<K, V> : IEnumerable
{
	// Token: 0x06002274 RID: 8820 RVA: 0x00085105 File Offset: 0x00083305
	public Lookup(IEqualityComparer<K> keyComparer)
	{
		this.dictionary = new Dictionary<K, V>(keyComparer);
		this.dictionaryReversed = new Dictionary<V, K>();
	}

	// Token: 0x06002275 RID: 8821 RVA: 0x00085124 File Offset: 0x00083324
	public void Add(K key, V value)
	{
		this.dictionary.Add(key, value);
		this.dictionaryReversed.Add(value, key);
	}

	// Token: 0x17000233 RID: 563
	// (get) Token: 0x06002276 RID: 8822 RVA: 0x00085140 File Offset: 0x00083340
	public IEnumerable<K> Keys
	{
		get
		{
			return this.dictionary.Keys;
		}
	}

	// Token: 0x06002277 RID: 8823 RVA: 0x0008514D File Offset: 0x0008334D
	public IEnumerator GetEnumerator()
	{
		return this.dictionary.GetEnumerator();
	}

	// Token: 0x06002278 RID: 8824 RVA: 0x0008515F File Offset: 0x0008335F
	public bool TryGetValue(K key, out V value)
	{
		return this.dictionary.TryGetValue(key, out value);
	}

	// Token: 0x06002279 RID: 8825 RVA: 0x0008516E File Offset: 0x0008336E
	public bool TryGetValue(V key, out K value)
	{
		return this.dictionaryReversed.TryGetValue(key, out value);
	}

	// Token: 0x0400223E RID: 8766
	private Dictionary<K, V> dictionary;

	// Token: 0x0400223F RID: 8767
	private Dictionary<V, K> dictionaryReversed;
}
