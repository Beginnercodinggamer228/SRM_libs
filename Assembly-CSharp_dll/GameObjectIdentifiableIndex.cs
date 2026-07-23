using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001FE RID: 510
public class GameObjectIdentifiableIndex
{
	// Token: 0x06000ACF RID: 2767 RVA: 0x0002DEB4 File Offset: 0x0002C0B4
	public void Register(Identifiable.Id id, GameObject obj)
	{
		GameObjectIdentifiableIndex.Entry item = new GameObjectIdentifiableIndex.Entry(id, obj);
		List<GameObjectIdentifiableIndex.Entry> list;
		this.objects.TryGetValue(id, out list);
		if (list == null)
		{
			list = new List<GameObjectIdentifiableIndex.Entry>();
			this.objects[id] = list;
		}
		list.Add(item);
		if (Identifiable.IsSlime(id))
		{
			this.slimes.Add(item);
		}
		if (Identifiable.IsAnimal(id))
		{
			this.animals.Add(item);
		}
		if (Identifiable.IsLargo(id))
		{
			this.largos.Add(item);
		}
		if (Identifiable.IsToy(id))
		{
			this.toys.Add(item);
		}
	}

	// Token: 0x06000AD0 RID: 2768 RVA: 0x0002DF48 File Offset: 0x0002C148
	public void Deregister(Identifiable.Id id, GameObject obj)
	{
		GameObjectIdentifiableIndex.Entry item = new GameObjectIdentifiableIndex.Entry(id, obj);
		List<GameObjectIdentifiableIndex.Entry> list;
		this.objects.TryGetValue(id, out list);
		if (list != null)
		{
			list.Remove(item);
			if (list.Count <= 0)
			{
				this.objects[id] = null;
			}
		}
		if (Identifiable.IsSlime(id))
		{
			this.slimes.Remove(item);
		}
		if (Identifiable.IsAnimal(id))
		{
			this.animals.Remove(item);
		}
		if (Identifiable.IsLargo(id))
		{
			this.largos.Remove(item);
		}
		if (Identifiable.IsToy(id))
		{
			this.toys.Remove(item);
		}
	}

	// Token: 0x06000AD1 RID: 2769 RVA: 0x0002DFE4 File Offset: 0x0002C1E4
	public bool IsRegistered(Identifiable.Id id, GameObject gameObject)
	{
		GameObjectIdentifiableIndex.Entry item = new GameObjectIdentifiableIndex.Entry(id, gameObject);
		List<GameObjectIdentifiableIndex.Entry> list;
		return this.objects.TryGetValue(id, out list) && list != null && list.Contains(item);
	}

	// Token: 0x06000AD2 RID: 2770 RVA: 0x0002E018 File Offset: 0x0002C218
	public IList<GameObjectIdentifiableIndex.Entry> GetObjectsByIdentifiableId(Identifiable.Id id)
	{
		List<GameObjectIdentifiableIndex.Entry> list;
		if (this.objects.TryGetValue(id, out list) && list != null)
		{
			return list;
		}
		return this.EMPTY_ENTRY_LIST;
	}

	// Token: 0x06000AD3 RID: 2771 RVA: 0x0002E040 File Offset: 0x0002C240
	public IList<GameObjectIdentifiableIndex.Entry> GetSlimes()
	{
		return this.slimes;
	}

	// Token: 0x06000AD4 RID: 2772 RVA: 0x0002E048 File Offset: 0x0002C248
	public int GetSlimeCount()
	{
		return this.slimes.Count;
	}

	// Token: 0x06000AD5 RID: 2773 RVA: 0x0002E055 File Offset: 0x0002C255
	public IList<GameObjectIdentifiableIndex.Entry> GetToys()
	{
		return this.toys;
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x0002E05D File Offset: 0x0002C25D
	public int GetToyCount()
	{
		return this.toys.Count;
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x0002E06A File Offset: 0x0002C26A
	public IList<GameObjectIdentifiableIndex.Entry> GetLargos()
	{
		return this.largos;
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x0002E072 File Offset: 0x0002C272
	public int GetLargoCount()
	{
		return this.largos.Count;
	}

	// Token: 0x06000AD9 RID: 2777 RVA: 0x0002E07F File Offset: 0x0002C27F
	public IList<GameObjectIdentifiableIndex.Entry> GetAnimals()
	{
		return this.animals;
	}

	// Token: 0x06000ADA RID: 2778 RVA: 0x0002E087 File Offset: 0x0002C287
	public int GetAnimalCount()
	{
		return this.animals.Count;
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x0002E094 File Offset: 0x0002C294
	public IEnumerable<GameObjectIdentifiableIndex.Entry> GetAllRegistered()
	{
		foreach (List<GameObjectIdentifiableIndex.Entry> entries in this.objects.Values)
		{
			if (entries != null)
			{
				int num;
				for (int ii = 0; ii < entries.Count; ii = num)
				{
					yield return entries[ii];
					num = ii + 1;
				}
			}
			entries = null;
		}
		Dictionary<Identifiable.Id, List<GameObjectIdentifiableIndex.Entry>>.ValueCollection.Enumerator enumerator = default(Dictionary<Identifiable.Id, List<GameObjectIdentifiableIndex.Entry>>.ValueCollection.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x040008C2 RID: 2242
	private readonly List<GameObjectIdentifiableIndex.Entry> EMPTY_ENTRY_LIST = new List<GameObjectIdentifiableIndex.Entry>();

	// Token: 0x040008C3 RID: 2243
	private Dictionary<Identifiable.Id, List<GameObjectIdentifiableIndex.Entry>> objects = new Dictionary<Identifiable.Id, List<GameObjectIdentifiableIndex.Entry>>(Identifiable.idComparer);

	// Token: 0x040008C4 RID: 2244
	private List<GameObjectIdentifiableIndex.Entry> slimes = new List<GameObjectIdentifiableIndex.Entry>();

	// Token: 0x040008C5 RID: 2245
	private List<GameObjectIdentifiableIndex.Entry> animals = new List<GameObjectIdentifiableIndex.Entry>();

	// Token: 0x040008C6 RID: 2246
	private List<GameObjectIdentifiableIndex.Entry> largos = new List<GameObjectIdentifiableIndex.Entry>();

	// Token: 0x040008C7 RID: 2247
	private List<GameObjectIdentifiableIndex.Entry> toys = new List<GameObjectIdentifiableIndex.Entry>();

	// Token: 0x020001FF RID: 511
	public struct Entry : IEquatable<GameObjectIdentifiableIndex.Entry>
	{
		// Token: 0x06000ADD RID: 2781 RVA: 0x0002E0FE File Offset: 0x0002C2FE
		public Entry(Identifiable.Id id, GameObject gameObject)
		{
			this.id = id;
			this.gameObject = gameObject;
			this.instanceId = gameObject.GetInstanceID();
		}

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x06000ADE RID: 2782 RVA: 0x0002E11A File Offset: 0x0002C31A
		public Identifiable.Id Id
		{
			get
			{
				return this.id;
			}
		}

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x06000ADF RID: 2783 RVA: 0x0002E122 File Offset: 0x0002C322
		public GameObject GameObject
		{
			get
			{
				return this.gameObject;
			}
		}

		// Token: 0x06000AE0 RID: 2784 RVA: 0x0002E12A File Offset: 0x0002C32A
		public bool Equals(GameObjectIdentifiableIndex.Entry other)
		{
			return this.id == other.id && this.instanceId == other.instanceId;
		}

		// Token: 0x040008C8 RID: 2248
		private Identifiable.Id id;

		// Token: 0x040008C9 RID: 2249
		private GameObject gameObject;

		// Token: 0x040008CA RID: 2250
		private int instanceId;
	}
}
