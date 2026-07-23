using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x020001FB RID: 507
public class GameObjectActorModelIdentifiableIndex
{
	// Token: 0x06000AB3 RID: 2739 RVA: 0x0002DA20 File Offset: 0x0002BC20
	public void Register(GameObject obj, ActorModel actorModel)
	{
		GameObjectActorModelIdentifiableIndex.Entry item = new GameObjectActorModelIdentifiableIndex.Entry(obj, actorModel);
		Identifiable.Id id = item.Id;
		List<GameObjectActorModelIdentifiableIndex.Entry> list;
		this.objects.TryGetValue(id, out list);
		if (list == null)
		{
			list = new List<GameObjectActorModelIdentifiableIndex.Entry>();
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

	// Token: 0x06000AB4 RID: 2740 RVA: 0x0002DABC File Offset: 0x0002BCBC
	public void Deregister(GameObject obj, ActorModel actorModel)
	{
		GameObjectActorModelIdentifiableIndex.Entry item = new GameObjectActorModelIdentifiableIndex.Entry(obj, actorModel);
		Identifiable.Id id = item.Id;
		List<GameObjectActorModelIdentifiableIndex.Entry> list;
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

	// Token: 0x06000AB5 RID: 2741 RVA: 0x0002DB60 File Offset: 0x0002BD60
	public bool IsRegistered(Identifiable.Id id, GameObject gameObject, ActorModel actorModel)
	{
		GameObjectActorModelIdentifiableIndex.Entry item = new GameObjectActorModelIdentifiableIndex.Entry(gameObject, actorModel);
		List<GameObjectActorModelIdentifiableIndex.Entry> list;
		return this.objects.TryGetValue(id, out list) && list != null && list.Contains(item);
	}

	// Token: 0x06000AB6 RID: 2742 RVA: 0x0002DB94 File Offset: 0x0002BD94
	public IList<GameObjectActorModelIdentifiableIndex.Entry> GetObjectsByIdentifiableId(Identifiable.Id id)
	{
		List<GameObjectActorModelIdentifiableIndex.Entry> list;
		if (this.objects.TryGetValue(id, out list) && list != null)
		{
			return list;
		}
		return this.EMPTY_ENTRY_LIST;
	}

	// Token: 0x06000AB7 RID: 2743 RVA: 0x0002DBBC File Offset: 0x0002BDBC
	public IList<GameObjectActorModelIdentifiableIndex.Entry> GetSlimes()
	{
		return this.slimes;
	}

	// Token: 0x06000AB8 RID: 2744 RVA: 0x0002DBC4 File Offset: 0x0002BDC4
	public int GetSlimeCount()
	{
		return this.slimes.Count;
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x0002DBD1 File Offset: 0x0002BDD1
	public IList<GameObjectActorModelIdentifiableIndex.Entry> GetToys()
	{
		return this.toys;
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x0002DBD9 File Offset: 0x0002BDD9
	public int GetToyCount()
	{
		return this.toys.Count;
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x0002DBE6 File Offset: 0x0002BDE6
	public IList<GameObjectActorModelIdentifiableIndex.Entry> GetLargos()
	{
		return this.largos;
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x0002DBEE File Offset: 0x0002BDEE
	public int GetLargoCount()
	{
		return this.largos.Count;
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x0002DBFB File Offset: 0x0002BDFB
	public IList<GameObjectActorModelIdentifiableIndex.Entry> GetAnimals()
	{
		return this.animals;
	}

	// Token: 0x06000ABE RID: 2750 RVA: 0x0002DC03 File Offset: 0x0002BE03
	public int GetAnimalCount()
	{
		return this.animals.Count;
	}

	// Token: 0x06000ABF RID: 2751 RVA: 0x0002DC10 File Offset: 0x0002BE10
	public IEnumerable<GameObjectActorModelIdentifiableIndex.Entry> GetAllRegistered()
	{
		foreach (List<GameObjectActorModelIdentifiableIndex.Entry> entries in this.objects.Values)
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
		Dictionary<Identifiable.Id, List<GameObjectActorModelIdentifiableIndex.Entry>>.ValueCollection.Enumerator enumerator = default(Dictionary<Identifiable.Id, List<GameObjectActorModelIdentifiableIndex.Entry>>.ValueCollection.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x040008B2 RID: 2226
	private readonly List<GameObjectActorModelIdentifiableIndex.Entry> EMPTY_ENTRY_LIST = new List<GameObjectActorModelIdentifiableIndex.Entry>();

	// Token: 0x040008B3 RID: 2227
	private Dictionary<Identifiable.Id, List<GameObjectActorModelIdentifiableIndex.Entry>> objects = new Dictionary<Identifiable.Id, List<GameObjectActorModelIdentifiableIndex.Entry>>(Identifiable.idComparer);

	// Token: 0x040008B4 RID: 2228
	private List<GameObjectActorModelIdentifiableIndex.Entry> slimes = new List<GameObjectActorModelIdentifiableIndex.Entry>();

	// Token: 0x040008B5 RID: 2229
	private List<GameObjectActorModelIdentifiableIndex.Entry> animals = new List<GameObjectActorModelIdentifiableIndex.Entry>();

	// Token: 0x040008B6 RID: 2230
	private List<GameObjectActorModelIdentifiableIndex.Entry> largos = new List<GameObjectActorModelIdentifiableIndex.Entry>();

	// Token: 0x040008B7 RID: 2231
	private List<GameObjectActorModelIdentifiableIndex.Entry> toys = new List<GameObjectActorModelIdentifiableIndex.Entry>();

	// Token: 0x020001FC RID: 508
	public struct Entry : IEquatable<GameObjectActorModelIdentifiableIndex.Entry>
	{
		// Token: 0x17000148 RID: 328
		// (get) Token: 0x06000AC1 RID: 2753 RVA: 0x0002DC7A File Offset: 0x0002BE7A
		public Identifiable.Id Id
		{
			get
			{
				return this.id;
			}
		}

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x06000AC2 RID: 2754 RVA: 0x0002DC82 File Offset: 0x0002BE82
		public GameObject GameObject
		{
			get
			{
				return this.gameObject;
			}
		}

		// Token: 0x06000AC3 RID: 2755 RVA: 0x0002DC8A File Offset: 0x0002BE8A
		public Entry(GameObject gameObject, ActorModel actorModel)
		{
			this.id = actorModel.ident;
			this.gameObject = gameObject;
			this.actorModel = actorModel;
		}

		// Token: 0x06000AC4 RID: 2756 RVA: 0x0002DCA6 File Offset: 0x0002BEA6
		public bool Equals(GameObjectActorModelIdentifiableIndex.Entry other)
		{
			return this.actorModel.actorId == other.actorModel.actorId;
		}

		// Token: 0x06000AC5 RID: 2757 RVA: 0x0002DCC0 File Offset: 0x0002BEC0
		public override int GetHashCode()
		{
			return this.actorModel.actorId.GetHashCode();
		}

		// Token: 0x040008B8 RID: 2232
		private Identifiable.Id id;

		// Token: 0x040008B9 RID: 2233
		private GameObject gameObject;

		// Token: 0x040008BA RID: 2234
		private ActorModel actorModel;
	}
}
