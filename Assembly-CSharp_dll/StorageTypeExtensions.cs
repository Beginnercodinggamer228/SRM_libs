using System;
using System.Collections.Generic;

// Token: 0x02000343 RID: 835
public static class StorageTypeExtensions
{
	// Token: 0x06001187 RID: 4487 RVA: 0x0004667D File Offset: 0x0004487D
	public static bool Contains(this SiloStorage.StorageType type, Identifiable.Id id)
	{
		return type.GetContents().Contains(id);
	}

	// Token: 0x06001188 RID: 4488 RVA: 0x0004668C File Offset: 0x0004488C
	public static HashSet<Identifiable.Id> GetContents(this SiloStorage.StorageType type)
	{
		HashSet<Identifiable.Id> hashSet;
		if (StorageTypeExtensions.getContentsCache.TryGetValue(type, out hashSet))
		{
			return hashSet;
		}
		hashSet = new HashSet<Identifiable.Id>(Identifiable.idComparer);
		switch (type)
		{
		case SiloStorage.StorageType.NON_SLIMES:
			hashSet.UnionWith(Identifiable.NON_SLIMES_CLASS);
			hashSet.UnionWith(Identifiable.ORNAMENT_CLASS);
			hashSet.UnionWith(Identifiable.ECHO_CLASS);
			hashSet.UnionWith(Identifiable.ECHO_NOTE_CLASS);
			break;
		case SiloStorage.StorageType.PLORT:
			hashSet.UnionWith(Identifiable.PLORT_CLASS);
			break;
		case SiloStorage.StorageType.FOOD:
			hashSet.UnionWith(Identifiable.FOOD_CLASS);
			hashSet.UnionWith(Identifiable.CHICK_CLASS);
			break;
		case SiloStorage.StorageType.CRAFTING:
			hashSet.UnionWith(Identifiable.PLORT_CLASS);
			hashSet.UnionWith(Identifiable.CRAFT_CLASS);
			break;
		case SiloStorage.StorageType.ELDER:
			hashSet.Add(Identifiable.Id.ELDER_HEN);
			hashSet.Add(Identifiable.Id.ELDER_ROOSTER);
			break;
		default:
			throw new ArgumentException(string.Format("Failed to get contents for storage type. [type={0}]", type));
		}
		hashSet.Remove(Identifiable.Id.QUICKSILVER_PLORT);
		return StorageTypeExtensions.getContentsCache[type] = hashSet;
	}

	// Token: 0x040010C1 RID: 4289
	private static Dictionary<SiloStorage.StorageType, HashSet<Identifiable.Id>> getContentsCache = new Dictionary<SiloStorage.StorageType, HashSet<Identifiable.Id>>(SiloStorage.StorageTypeComparer.Instance);
}
