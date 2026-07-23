using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x02000340 RID: 832
public class SiloStorage : MonoBehaviour, LandPlotModel.Participant
{
	// Token: 0x1700018D RID: 397
	// (get) Token: 0x06001174 RID: 4468 RVA: 0x00046507 File Offset: 0x00044707
	public Ammo LocalAmmo
	{
		get
		{
			return this.ammo;
		}
	}

	// Token: 0x06001175 RID: 4469 RVA: 0x0004650F File Offset: 0x0004470F
	public virtual void Awake()
	{
		this.InitAmmo();
	}

	// Token: 0x06001176 RID: 4470 RVA: 0x00046518 File Offset: 0x00044718
	private void InitAmmo()
	{
		if (this.ammo == null)
		{
			this.ammo = new Ammo(this.type.GetContents(), this.numSlots, this.numSlots, new Predicate<Identifiable.Id>[this.numSlots], (Identifiable.Id id, int index) => this.maxAmmo);
		}
	}

	// Token: 0x06001177 RID: 4471 RVA: 0x00046566 File Offset: 0x00044766
	public void InitModel(LandPlotModel model)
	{
		this.InitAmmo();
		model.siloAmmo[this.type] = new AmmoModel();
		this.LocalAmmo.InitModel(model.siloAmmo[this.type]);
	}

	// Token: 0x06001178 RID: 4472 RVA: 0x000465A0 File Offset: 0x000447A0
	public void SetModel(LandPlotModel model)
	{
		this.model = model;
		this.LocalAmmo.SetModel(model.siloAmmo[this.type]);
	}

	// Token: 0x06001179 RID: 4473 RVA: 0x00046507 File Offset: 0x00044707
	public virtual Ammo GetRelevantAmmo()
	{
		return this.ammo;
	}

	// Token: 0x0600117A RID: 4474 RVA: 0x000465C5 File Offset: 0x000447C5
	public Identifiable.Id GetSlotIdentifiable(int slotIdx)
	{
		return this.GetRelevantAmmo().GetSlotName(slotIdx);
	}

	// Token: 0x0600117B RID: 4475 RVA: 0x000465D3 File Offset: 0x000447D3
	public int GetSlotCount(int slotIdx)
	{
		return this.GetRelevantAmmo().GetSlotCount(slotIdx);
	}

	// Token: 0x0600117C RID: 4476 RVA: 0x000465E1 File Offset: 0x000447E1
	public bool MaybeAddIdentifiable(Identifiable.Id id)
	{
		bool result = this.GetRelevantAmmo().MaybeAddToSlot(id, null);
		this.OnAdded();
		return result;
	}

	// Token: 0x0600117D RID: 4477 RVA: 0x000465F6 File Offset: 0x000447F6
	public bool MaybeAddIdentifiable(Identifiable.Id id, int slotIdx, int count = 1, bool overflow = false)
	{
		bool result = this.GetRelevantAmmo().MaybeAddToSpecificSlot(id, null, slotIdx, count, overflow);
		this.OnAdded();
		return result;
	}

	// Token: 0x0600117E RID: 4478 RVA: 0x0004660F File Offset: 0x0004480F
	public bool CanAccept(Identifiable.Id id)
	{
		return this.GetRelevantAmmo().CouldAddToSlot(id);
	}

	// Token: 0x0600117F RID: 4479 RVA: 0x0004661D File Offset: 0x0004481D
	public bool CanAccept(Identifiable.Id id, int slotIdx, bool overflow)
	{
		return this.GetRelevantAmmo().CouldAddToSlot(id, slotIdx, overflow);
	}

	// Token: 0x06001180 RID: 4480 RVA: 0x0004662D File Offset: 0x0004482D
	private void OnAdded()
	{
		if (this.GetRelevantAmmo().HasFullSlots(12, 50))
		{
			SRSingleton<SceneContext>.Instance.AchievementsDirector.AddToStat(AchievementsDirector.IntStat.FILLED_SILO, 1);
		}
	}

	// Token: 0x040010B3 RID: 4275
	public SiloStorage.StorageType type;

	// Token: 0x040010B4 RID: 4276
	public int numSlots = 4;

	// Token: 0x040010B5 RID: 4277
	public int maxAmmo = 100;

	// Token: 0x040010B6 RID: 4278
	protected LandPlotModel model;

	// Token: 0x040010B7 RID: 4279
	protected Ammo ammo;

	// Token: 0x040010B8 RID: 4280
	private const int SILO_FULL_ACHIEVE_SLOTS = 12;

	// Token: 0x040010B9 RID: 4281
	private const int SILO_FULL_ACHIEVE_AMOUNT = 50;

	// Token: 0x02000341 RID: 833
	public enum StorageType
	{
		// Token: 0x040010BB RID: 4283
		NON_SLIMES,
		// Token: 0x040010BC RID: 4284
		PLORT,
		// Token: 0x040010BD RID: 4285
		FOOD,
		// Token: 0x040010BE RID: 4286
		CRAFTING,
		// Token: 0x040010BF RID: 4287
		ELDER
	}

	// Token: 0x02000342 RID: 834
	public class StorageTypeComparer : IEqualityComparer<SiloStorage.StorageType>
	{
		// Token: 0x06001183 RID: 4483 RVA: 0x00017781 File Offset: 0x00015981
		public bool Equals(SiloStorage.StorageType a, SiloStorage.StorageType b)
		{
			return a == b;
		}

		// Token: 0x06001184 RID: 4484 RVA: 0x00017787 File Offset: 0x00015987
		public int GetHashCode(SiloStorage.StorageType a)
		{
			return (int)a;
		}

		// Token: 0x040010C0 RID: 4288
		public static SiloStorage.StorageTypeComparer Instance = new SiloStorage.StorageTypeComparer();
	}
}
