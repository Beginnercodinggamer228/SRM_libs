using System;
using System.Collections.Generic;

// Token: 0x02000680 RID: 1664
public class LRUCache<K, V>
{
	// Token: 0x06002266 RID: 8806 RVA: 0x00084F15 File Offset: 0x00083115
	public LRUCache(int capacity)
	{
		this.capacity = capacity;
	}

	// Token: 0x06002267 RID: 8807 RVA: 0x00084F3A File Offset: 0x0008313A
	public bool contains(K key)
	{
		return this.cacheMap.ContainsKey(key);
	}

	// Token: 0x06002268 RID: 8808 RVA: 0x00084F48 File Offset: 0x00083148
	public V get(K key)
	{
		LinkedListNode<LRUCacheItem<K, V>> linkedListNode;
		if (this.cacheMap.TryGetValue(key, out linkedListNode))
		{
			V value = linkedListNode.Value.value;
			this.lruList.Remove(linkedListNode);
			this.lruList.AddLast(linkedListNode);
			return value;
		}
		throw new KeyNotFoundException();
	}

	// Token: 0x06002269 RID: 8809 RVA: 0x00084F90 File Offset: 0x00083190
	public void put(K key, V val)
	{
		if (this.cacheMap.Count >= this.capacity)
		{
			this.removeFirst();
		}
		LinkedListNode<LRUCacheItem<K, V>> linkedListNode = new LinkedListNode<LRUCacheItem<K, V>>(new LRUCacheItem<K, V>(key, val));
		this.lruList.AddLast(linkedListNode);
		this.cacheMap.Add(key, linkedListNode);
	}

	// Token: 0x0600226A RID: 8810 RVA: 0x00084FDC File Offset: 0x000831DC
	public void clear(K key)
	{
		LinkedListNode<LRUCacheItem<K, V>> node;
		if (this.cacheMap.TryGetValue(key, out node))
		{
			this.lruList.Remove(node);
			this.cacheMap.Remove(key);
		}
	}

	// Token: 0x0600226B RID: 8811 RVA: 0x00085014 File Offset: 0x00083214
	protected void removeFirst()
	{
		LinkedListNode<LRUCacheItem<K, V>> first = this.lruList.First;
		this.lruList.RemoveFirst();
		this.cacheMap.Remove(first.Value.key);
	}

	// Token: 0x04002238 RID: 8760
	private int capacity;

	// Token: 0x04002239 RID: 8761
	private Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>> cacheMap = new Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>>();

	// Token: 0x0400223A RID: 8762
	private LinkedList<LRUCacheItem<K, V>> lruList = new LinkedList<LRUCacheItem<K, V>>();
}
