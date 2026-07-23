using System;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.Persist;
using UnityEngine;

// Token: 0x0200028F RID: 655
public class Ammo
{
	// Token: 0x17000177 RID: 375
	// (get) Token: 0x06000DA2 RID: 3490 RVA: 0x000376FB File Offset: 0x000358FB
	private Ammo.Slot[] Slots
	{
		get
		{
			return this.ammoModel.slots;
		}
	}

	// Token: 0x06000DA3 RID: 3491 RVA: 0x00037708 File Offset: 0x00035908
	public Ammo(HashSet<Identifiable.Id> potentialAmmo, int numSlots, int usableSlots, Predicate<Identifiable.Id>[] slotPreds, Func<Identifiable.Id, int, int> slotMaxCountFunction)
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.lookupDir = SRSingleton<GameContext>.Instance.LookupDirector;
		this.potentialAmmo = potentialAmmo;
		this.numSlots = numSlots;
		this.slotPreds = slotPreds;
		this.initUsableSlots = usableSlots;
		this.initSlotMaxCountFunction = slotMaxCountFunction;
	}

	// Token: 0x06000DA4 RID: 3492 RVA: 0x0003776F File Offset: 0x0003596F
	public void InitModel(AmmoModel model)
	{
		model.Reset(this.numSlots, this.initUsableSlots, this.initSlotMaxCountFunction);
	}

	// Token: 0x06000DA5 RID: 3493 RVA: 0x00037789 File Offset: 0x00035989
	public void SetModel(AmmoModel model)
	{
		this.ValidateAndAdjustSlots(ref model.slots);
		this.ammoModel = model;
	}

	// Token: 0x06000DA6 RID: 3494 RVA: 0x0003779E File Offset: 0x0003599E
	public Predicate<Identifiable.Id> GetSlotPredicate(int index)
	{
		return this.slotPreds[index];
	}

	// Token: 0x06000DA7 RID: 3495 RVA: 0x000377A8 File Offset: 0x000359A8
	public bool SetAmmoSlot(int idx)
	{
		if (this.selectedAmmoIdx != idx && idx < this.ammoModel.usableSlots)
		{
			this.selectedAmmoIdx = idx;
			return true;
		}
		return false;
	}

	// Token: 0x06000DA8 RID: 3496 RVA: 0x000377CB File Offset: 0x000359CB
	public void NextAmmoSlot()
	{
		this.selectedAmmoIdx = (this.selectedAmmoIdx + 1) % this.ammoModel.usableSlots;
	}

	// Token: 0x06000DA9 RID: 3497 RVA: 0x000377E7 File Offset: 0x000359E7
	public void PrevAmmoSlot()
	{
		this.selectedAmmoIdx = (this.selectedAmmoIdx + this.ammoModel.usableSlots - 1) % this.ammoModel.usableSlots;
	}

	// Token: 0x06000DAA RID: 3498 RVA: 0x0003780F File Offset: 0x00035A0F
	public Identifiable.Id GetSlotName(int slotIdx)
	{
		if (this.Slots[slotIdx] == null)
		{
			return Identifiable.Id.NONE;
		}
		return this.AdjustId(this.Slots[slotIdx].id);
	}

	// Token: 0x06000DAB RID: 3499 RVA: 0x00037830 File Offset: 0x00035A30
	public SlimeEmotionData GetSlimeEmotionData(int slotIdx)
	{
		if (this.Slots[slotIdx] == null)
		{
			return new SlimeEmotionData();
		}
		return this.Slots[slotIdx].emotions;
	}

	// Token: 0x06000DAC RID: 3500 RVA: 0x00037850 File Offset: 0x00035A50
	public int GetCount(Identifiable.Id id)
	{
		for (int i = 0; i < this.GetUsableSlotCount(); i++)
		{
			if (this.Slots[i] != null && this.Slots[i].id == id)
			{
				return this.Slots[i].count;
			}
		}
		return 0;
	}

	// Token: 0x06000DAD RID: 3501 RVA: 0x00037897 File Offset: 0x00035A97
	public int GetSlotCount(int slotIdx)
	{
		if (this.Slots[slotIdx] == null)
		{
			return 0;
		}
		return this.Slots[slotIdx].count;
	}

	// Token: 0x06000DAE RID: 3502 RVA: 0x000378B2 File Offset: 0x00035AB2
	public int GetSelectedAmmoIdx()
	{
		return this.selectedAmmoIdx;
	}

	// Token: 0x06000DAF RID: 3503 RVA: 0x000378BC File Offset: 0x00035ABC
	public int? GetAmmoIdx(Identifiable.Id id)
	{
		for (int i = 0; i < this.GetUsableSlotCount(); i++)
		{
			if (this.Slots[i] != null && this.Slots[i].id == id)
			{
				return new int?(i);
			}
		}
		return null;
	}

	// Token: 0x06000DB0 RID: 3504 RVA: 0x00037904 File Offset: 0x00035B04
	public Identifiable.Id GetSelectedId()
	{
		Ammo.Slot slot = this.Slots[this.selectedAmmoIdx];
		if (slot != null)
		{
			return this.AdjustId(slot.id);
		}
		return Identifiable.Id.NONE;
	}

	// Token: 0x06000DB1 RID: 3505 RVA: 0x00037930 File Offset: 0x00035B30
	public void RegisterPotentialAmmo(GameObject prefab)
	{
		this.potentialAmmo.Add(Identifiable.GetId(prefab));
	}

	// Token: 0x06000DB2 RID: 3506 RVA: 0x00037944 File Offset: 0x00035B44
	public int GetSlotMaxCount(int index)
	{
		return this.GetSlotMaxCount(this.GetSlotName(index), index);
	}

	// Token: 0x06000DB3 RID: 3507 RVA: 0x00037954 File Offset: 0x00035B54
	public int GetSlotMaxCount(Identifiable.Id id, int index)
	{
		return this.ammoModel.GetSlotMaxCount(id, index);
	}

	// Token: 0x06000DB4 RID: 3508 RVA: 0x00037963 File Offset: 0x00035B63
	private Identifiable.Id AdjustId(Identifiable.Id id)
	{
		if (Identifiable.IsWater(id) && !this.timeDir.HasReached(this.waterIsMagicUntil))
		{
			return Identifiable.Id.MAGIC_WATER_LIQUID;
		}
		return id;
	}

	// Token: 0x06000DB5 RID: 3509 RVA: 0x00037984 File Offset: 0x00035B84
	public GameObject GetSelectedStored()
	{
		Ammo.Slot slot = this.Slots[this.selectedAmmoIdx];
		if (slot == null)
		{
			return null;
		}
		return this.lookupDir.GetPrefab(this.AdjustId(slot.id));
	}

	// Token: 0x06000DB6 RID: 3510 RVA: 0x000379BB File Offset: 0x00035BBB
	public Dictionary<SlimeEmotions.Emotion, float> GetSelectedEmotions()
	{
		return this.Slots[this.selectedAmmoIdx].emotions;
	}

	// Token: 0x06000DB7 RID: 3511 RVA: 0x000379CF File Offset: 0x00035BCF
	public bool HasSelectedAmmo()
	{
		return this.Slots[this.selectedAmmoIdx] != null && this.Slots[this.selectedAmmoIdx].count > 0;
	}

	// Token: 0x06000DB8 RID: 3512 RVA: 0x000379F8 File Offset: 0x00035BF8
	public void DecrementSelectedAmmo(int amount = 1)
	{
		if (this.Slots[this.selectedAmmoIdx] != null)
		{
			if (Identifiable.IsWater(this.Slots[this.selectedAmmoIdx].id) && !this.timeDir.HasReached(this.waterIsMagicUntil))
			{
				return;
			}
			this.Slots[this.selectedAmmoIdx].count = Math.Max(this.Slots[this.selectedAmmoIdx].count - amount, 0);
			if (this.Slots[this.selectedAmmoIdx].count == 0)
			{
				this.Slots[this.selectedAmmoIdx] = null;
			}
		}
	}

	// Token: 0x06000DB9 RID: 3513 RVA: 0x00037A8F File Offset: 0x00035C8F
	public void Clear()
	{
		this.Clear((int i) => true);
	}

	// Token: 0x06000DBA RID: 3514 RVA: 0x00037AB8 File Offset: 0x00035CB8
	public void Clear(Predicate<int> predicate)
	{
		for (int i = 0; i < this.Slots.Length; i++)
		{
			if (predicate(i))
			{
				this.Clear(i);
			}
		}
	}

	// Token: 0x06000DBB RID: 3515 RVA: 0x00037AE8 File Offset: 0x00035CE8
	public void ClearSelected()
	{
		this.Clear(this.selectedAmmoIdx);
	}

	// Token: 0x06000DBC RID: 3516 RVA: 0x00037AF6 File Offset: 0x00035CF6
	public void Clear(int index)
	{
		if (Identifiable.IsWater(this.GetSlotName(index)))
		{
			this.waterIsMagicUntil = double.NegativeInfinity;
		}
		this.Slots[index] = null;
	}

	// Token: 0x06000DBD RID: 3517 RVA: 0x00037B20 File Offset: 0x00035D20
	public bool ReplaceWithQuicksilverAmmo(Identifiable.Id id, int count)
	{
		if (!this.potentialAmmo.Contains(id))
		{
			return false;
		}
		int? num = null;
		int? num2 = null;
		int i = 0;
		while (i < this.Slots.Length)
		{
			if (this.Slots[i] != null && this.Slots[i].id == id)
			{
				if (this.Slots[i].count < this.GetSlotMaxCount(id, i))
				{
					this.Slots[i] = new Ammo.Slot(id, Mathf.Min(this.GetSlotMaxCount(id, i), this.Slots[i].count + count));
					return true;
				}
				return false;
			}
			else
			{
				if ((this.slotPreds[i] == null || this.slotPreds[i](id)) && (num2 == null || this.GetSlotCount(i) < num2.Value))
				{
					num2 = new int?(this.GetSlotCount(i));
					num = new int?(i);
				}
				i++;
			}
		}
		if (num != null)
		{
			this.Slots[num.Value] = new Ammo.Slot(id, Mathf.Min(this.GetSlotMaxCount(id, num.Value), count));
			return true;
		}
		return false;
	}

	// Token: 0x06000DBE RID: 3518 RVA: 0x00037C44 File Offset: 0x00035E44
	public bool Contains(Identifiable.Id id)
	{
		foreach (Ammo.Slot slot in this.Slots)
		{
			if (slot != null && slot.id == id)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000DBF RID: 3519 RVA: 0x00037C7C File Offset: 0x00035E7C
	public void Decrement(Identifiable.Id id, int count = 1)
	{
		for (int i = 0; i < this.Slots.Length; i++)
		{
			Ammo.Slot slot = this.Slots[i];
			if (slot != null && slot.id == id)
			{
				this.Decrement(i, count);
				return;
			}
		}
		throw new InvalidOperationException("Cannot decrement ammo we don't have: " + id);
	}

	// Token: 0x06000DC0 RID: 3520 RVA: 0x00037CCF File Offset: 0x00035ECF
	public void Decrement(int index, int count = 1)
	{
		this.Slots[index].count -= count;
		if (this.Slots[index].count <= 0)
		{
			this.Slots[index] = null;
		}
	}

	// Token: 0x06000DC1 RID: 3521 RVA: 0x00037D00 File Offset: 0x00035F00
	public bool IsEmpty()
	{
		for (int i = 0; i < this.Slots.Length; i++)
		{
			Ammo.Slot slot = this.Slots[i];
			if (slot != null && slot.count > 0)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000DC2 RID: 3522 RVA: 0x00037D38 File Offset: 0x00035F38
	public bool Any(Predicate<Identifiable.Id> predicate)
	{
		for (int i = 0; i < this.GetUsableSlotCount(); i++)
		{
			if (this.Slots[i] != null && this.Slots[i].count > 0 && predicate(this.Slots[i].id))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000DC3 RID: 3523 RVA: 0x00037D88 File Offset: 0x00035F88
	public double GetRemainingWaterIsMagicMins()
	{
		double num = this.timeDir.HoursUntil(this.waterIsMagicUntil);
		if (num >= 0.0)
		{
			return num * 60.0;
		}
		return 0.0;
	}

	// Token: 0x06000DC4 RID: 3524 RVA: 0x00037DC8 File Offset: 0x00035FC8
	public float GetFullness(int index)
	{
		return Mathf.Min(1f, (float)this.GetSlotCount(index) / (float)this.GetSlotMaxCount(index));
	}

	// Token: 0x06000DC5 RID: 3525 RVA: 0x00037DE5 File Offset: 0x00035FE5
	public float GetSelectedFullness()
	{
		return this.GetFullness(this.selectedAmmoIdx);
	}

	// Token: 0x06000DC6 RID: 3526 RVA: 0x00037DF3 File Offset: 0x00035FF3
	public bool MaybeAddToSpecificSlot(Identifiable identifiable, int slotIdx)
	{
		return this.MaybeAddToSpecificSlot(identifiable.id, identifiable, slotIdx);
	}

	// Token: 0x06000DC7 RID: 3527 RVA: 0x00037E03 File Offset: 0x00036003
	public bool MaybeAddToSpecificSlot(Identifiable.Id id, Identifiable identifiable, int slotIdx)
	{
		return this.MaybeAddToSpecificSlot(id, identifiable, slotIdx, this.GetAmountFilledPerVac(id, slotIdx));
	}

	// Token: 0x06000DC8 RID: 3528 RVA: 0x00037E16 File Offset: 0x00036016
	public bool MaybeAddToSpecificSlot(Identifiable.Id id, Identifiable identifiable, int slotIdx, int count)
	{
		return this.MaybeAddToSpecificSlot(id, identifiable, slotIdx, count, false);
	}

	// Token: 0x06000DC9 RID: 3529 RVA: 0x00037E24 File Offset: 0x00036024
	public bool MaybeAddToSpecificSlot(Identifiable.Id id, Identifiable identifiable, int slotIdx, int count, bool overflow)
	{
		if (this.Slots[slotIdx] == null)
		{
			if ((this.slotPreds[slotIdx] != null && !this.slotPreds[slotIdx](id)) || !this.potentialAmmo.Contains(id))
			{
				return false;
			}
			this.Slots[slotIdx] = new Ammo.Slot(id, 0);
		}
		if (this.Slots[slotIdx].id != id)
		{
			return false;
		}
		if (!overflow && this.Slots[slotIdx].count + count > this.GetSlotMaxCount(id, slotIdx))
		{
			return false;
		}
		this.Slots[slotIdx].count += count;
		if (identifiable != null)
		{
			SlimeEmotions component = identifiable.GetComponent<SlimeEmotions>();
			if (component != null)
			{
				this.Slots[slotIdx].AverageIn(component);
			}
		}
		return true;
	}

	// Token: 0x06000DCA RID: 3530 RVA: 0x00037EE4 File Offset: 0x000360E4
	public bool CouldAddToSlot(Identifiable.Id id)
	{
		return this.CouldAddToSlot(id, 0, this.GetUsableSlotCount() - 1, false);
	}

	// Token: 0x06000DCB RID: 3531 RVA: 0x00037EF7 File Offset: 0x000360F7
	public bool CouldAddToSlot(Identifiable.Id id, int slotIdx, bool overflow)
	{
		return this.CouldAddToSlot(id, slotIdx, slotIdx, overflow);
	}

	// Token: 0x06000DCC RID: 3532 RVA: 0x00037F04 File Offset: 0x00036104
	private bool CouldAddToSlot(Identifiable.Id id, int indexMin, int indexMax, bool overflow)
	{
		if (id == Identifiable.Id.MAGIC_WATER_LIQUID)
		{
			id = Identifiable.Id.WATER_LIQUID;
		}
		if (!this.potentialAmmo.Contains(id))
		{
			return false;
		}
		for (int i = indexMin; i <= indexMax; i++)
		{
			if (this.Slots[i] == null)
			{
				if (this.slotPreds[i] == null || this.slotPreds[i](id))
				{
					return true;
				}
			}
			else if (this.Slots[i].id == id && (overflow || this.Slots[i].count < this.GetSlotMaxCount(id, i)))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000DCD RID: 3533 RVA: 0x00037F8C File Offset: 0x0003618C
	public bool MaybeAddToSlot(Identifiable.Id id, Identifiable identifiable)
	{
		bool flag = id == Identifiable.Id.MAGIC_WATER_LIQUID;
		if (flag)
		{
			id = Identifiable.Id.WATER_LIQUID;
		}
		bool flag2 = false;
		bool flag3 = false;
		for (int i = 0; i < this.ammoModel.usableSlots; i++)
		{
			if (this.Slots[i] != null && this.Slots[i].id == id)
			{
				int slotMaxCount = this.GetSlotMaxCount(id, i);
				if (flag)
				{
					this.Slots[i].count = slotMaxCount;
					this.waterIsMagicUntil = this.timeDir.HoursFromNow(0.5f);
				}
				else if (this.Slots[i].count >= slotMaxCount)
				{
					flag3 = true;
				}
				else
				{
					this.Slots[i].count = Mathf.Min(slotMaxCount, this.Slots[i].count + this.GetAmountFilledPerVac(id, i));
					SlimeEmotions slimeEmotions = (identifiable == null) ? null : identifiable.GetComponent<SlimeEmotions>();
					if (slimeEmotions != null)
					{
						this.Slots[i].AverageIn(slimeEmotions);
					}
				}
				flag2 = true;
				break;
			}
		}
		if (!flag2)
		{
			int num = 0;
			while (num < this.ammoModel.usableSlots && !flag2)
			{
				if ((this.slotPreds[num] == null || this.slotPreds[num](id)) && this.Slots[num] == null && this.potentialAmmo.Contains(id))
				{
					SlimeEmotions slimeEmotions2 = (identifiable == null) ? null : identifiable.GetComponent<SlimeEmotions>();
					if (flag)
					{
						this.Slots[num] = new Ammo.Slot(id, this.GetSlotMaxCount(id, num));
						this.waterIsMagicUntil = this.timeDir.HoursFromNow(0.5f);
					}
					else
					{
						this.Slots[num] = new Ammo.Slot(id, this.GetAmountFilledPerVac(id, num));
					}
					if (slimeEmotions2 != null)
					{
						this.Slots[num].AverageIn(slimeEmotions2);
					}
					flag2 = true;
				}
				num++;
			}
		}
		return flag2 && !flag3;
	}

	// Token: 0x06000DCE RID: 3534 RVA: 0x00038172 File Offset: 0x00036372
	private int GetAmountFilledPerVac(Identifiable.Id id, int index)
	{
		if (id == Identifiable.Id.WATER_LIQUID)
		{
			return 5;
		}
		if (id == Identifiable.Id.GLITCH_DEBUG_SPRAY_LIQUID)
		{
			return SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch.debugSprayAmmoPerStation;
		}
		return 1;
	}

	// Token: 0x06000DCF RID: 3535 RVA: 0x0003819C File Offset: 0x0003639C
	public bool Replace(int index, Identifiable.Id id)
	{
		Ammo.Slot slot = this.Slots[index];
		double num = this.waterIsMagicUntil;
		this.Clear(index);
		if (this.MaybeAddToSpecificSlot(id, null, index))
		{
			return true;
		}
		this.Slots[index] = slot;
		this.waterIsMagicUntil = num;
		return false;
	}

	// Token: 0x06000DD0 RID: 3536 RVA: 0x000381E0 File Offset: 0x000363E0
	public bool Replace(Identifiable.Id previous, Identifiable.Id next)
	{
		for (int i = 0; i < this.Slots.Length; i++)
		{
			if (this.GetSlotName(i) == previous)
			{
				this.Slots[i] = new Ammo.Slot(next, Mathf.Min(this.GetSlotCount(i), this.GetSlotMaxCount(next, i)));
				if (Identifiable.IsSlime(this.Slots[i].id))
				{
					this.Slots[i].emotions = new SlimeEmotionData();
					this.Slots[i].emotions[SlimeEmotions.Emotion.AGITATION] = 0f;
					this.Slots[i].emotions[SlimeEmotions.Emotion.HUNGER] = 0.5f;
					this.Slots[i].emotions[SlimeEmotions.Emotion.FEAR] = 0f;
				}
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000DD1 RID: 3537 RVA: 0x000382A8 File Offset: 0x000364A8
	public bool HasFullSlots(int numSlots, int fullToAmount)
	{
		if (this.ammoModel.usableSlots < numSlots)
		{
			return false;
		}
		for (int i = 0; i < this.ammoModel.usableSlots; i++)
		{
			if (this.GetSlotCount(i) < fullToAmount)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000DD2 RID: 3538 RVA: 0x000382E8 File Offset: 0x000364E8
	public List<AmmoDataV02> ToSerializable()
	{
		return (from ii in Enumerable.Range(0, this.Slots.Length)
		select new AmmoDataV02
		{
			id = this.GetSlotName(ii),
			emotionData = new SlimeEmotionDataV02
			{
				emotionData = this.GetSlimeEmotionData(ii)
			},
			count = this.GetSlotCount(ii)
		}).ToList<AmmoDataV02>();
	}

	// Token: 0x06000DD3 RID: 3539 RVA: 0x00038310 File Offset: 0x00036510
	public void FromSerializable(List<AmmoDataV02> data)
	{
		if (data.Count > 0 && data.Count != this.Slots.Length)
		{
			Log.Warning("Unserializing ammo slot length differs, ignoring extra.", Array.Empty<object>());
		}
		int num = 0;
		while (num < this.Slots.Length && num < data.Count)
		{
			if (data[num] != null && data[num].id != Identifiable.Id.NONE)
			{
				if (this.slotPreds[num] != null && !this.slotPreds[num](data[num].id))
				{
					Debug.Log("Unserializing ammo slot contained no-longer-legal ID, ignoring: " + data[num].id);
				}
				else if (!this.potentialAmmo.Contains(data[num].id))
				{
					Log.Warning("Unserializing ammo slot contained invalid ammo id: " + data[num].id, Array.Empty<object>());
				}
				else if (this.lookupDir.GetPrefab(data[num].id) == null)
				{
					Log.Warning("Found unknown ammo ID: " + data[num].id, Array.Empty<object>());
				}
				else
				{
					this.Slots[num] = new Ammo.Slot(data[num].id, data[num].count);
					this.Slots[num].emotions = new SlimeEmotionData();
					foreach (KeyValuePair<SlimeEmotions.Emotion, float> keyValuePair in data[num].emotionData.emotionData)
					{
						this.Slots[num].emotions.Add(keyValuePair.Key, keyValuePair.Value);
					}
				}
			}
			num++;
		}
	}

	// Token: 0x06000DD4 RID: 3540 RVA: 0x000384FC File Offset: 0x000366FC
	private void ValidateAndAdjustSlots(ref Ammo.Slot[] slots)
	{
		if (slots.Length != this.slotPreds.Length)
		{
			Log.Warning("Unserializing ammo slot length different than expected, ignoring extra and/or padding.", new object[]
			{
				"slots",
				slots.Length,
				"preds",
				this.slotPreds.Length
			});
			Ammo.Slot[] array = slots;
			slots = new Ammo.Slot[this.slotPreds.Length];
			for (int i = 0; i < this.slotPreds.Length; i++)
			{
				if (i < array.Length)
				{
					slots[i] = array[i];
				}
				else
				{
					slots[i] = null;
				}
			}
		}
		for (int j = 0; j < this.slotPreds.Length; j++)
		{
			if (slots[j] == null || slots[j].id == Identifiable.Id.NONE)
			{
				slots[j] = null;
			}
			else if (this.slotPreds[j] != null && !this.slotPreds[j](slots[j].id))
			{
				Debug.Log("Unserialized ammo slot contained no-longer-legal ID, ignoring: " + slots[j].id);
				slots[j] = null;
			}
			else if (!this.potentialAmmo.Contains(slots[j].id))
			{
				Log.Warning("Unserializing ammo slot contained invalid ammo id: " + slots[j].id, Array.Empty<object>());
				slots[j] = null;
			}
		}
	}

	// Token: 0x06000DD5 RID: 3541 RVA: 0x00038644 File Offset: 0x00036844
	public void IncreaseUsableSlots(int usableSlots)
	{
		this.ammoModel.IncreaseUsableSlots(usableSlots);
		Debug.Log(string.Concat(new object[]
		{
			"MST Increased slots: ",
			this.ammoModel.usableSlots,
			" set: ",
			usableSlots
		}));
	}

	// Token: 0x06000DD6 RID: 3542 RVA: 0x00038699 File Offset: 0x00036899
	public int GetUsableSlotCount()
	{
		return this.ammoModel.usableSlots;
	}

	// Token: 0x04000D00 RID: 3328
	private AmmoModel ammoModel;

	// Token: 0x04000D01 RID: 3329
	private int selectedAmmoIdx;

	// Token: 0x04000D02 RID: 3330
	private readonly HashSet<Identifiable.Id> potentialAmmo;

	// Token: 0x04000D03 RID: 3331
	private int numSlots;

	// Token: 0x04000D04 RID: 3332
	private int initUsableSlots;

	// Token: 0x04000D05 RID: 3333
	private Func<Identifiable.Id, int, int> initSlotMaxCountFunction;

	// Token: 0x04000D06 RID: 3334
	private Predicate<Identifiable.Id>[] slotPreds;

	// Token: 0x04000D07 RID: 3335
	private TimeDirector timeDir;

	// Token: 0x04000D08 RID: 3336
	private LookupDirector lookupDir;

	// Token: 0x04000D09 RID: 3337
	private double waterIsMagicUntil = double.NegativeInfinity;

	// Token: 0x04000D0A RID: 3338
	private const float MAGIC_WATER_LIFETIME = 0.5f;

	// Token: 0x02000290 RID: 656
	public class Slot
	{
		// Token: 0x06000DD8 RID: 3544 RVA: 0x000386DF File Offset: 0x000368DF
		public Slot(Identifiable.Id id, int count)
		{
			this.id = id;
			this.count = count;
		}

		// Token: 0x06000DD9 RID: 3545 RVA: 0x000386F5 File Offset: 0x000368F5
		public void AverageIn(SlimeEmotions emotions)
		{
			if (this.emotions == null)
			{
				this.emotions = new SlimeEmotionData(emotions);
				return;
			}
			this.emotions.AverageIn(emotions, 1f / (float)this.count);
		}

		// Token: 0x04000D0B RID: 3339
		public readonly Identifiable.Id id;

		// Token: 0x04000D0C RID: 3340
		public int count;

		// Token: 0x04000D0D RID: 3341
		public SlimeEmotionData emotions;
	}

	// Token: 0x02000291 RID: 657
	[Serializable]
	public class AmmoData
	{
		// Token: 0x06000DDA RID: 3546 RVA: 0x00038725 File Offset: 0x00036925
		public AmmoData(Identifiable.Id id, int count, SlimeEmotionData emotionData)
		{
			this.id = id;
			this.count = count;
			this.emotionData = emotionData;
		}

		// Token: 0x06000DDB RID: 3547 RVA: 0x00038744 File Offset: 0x00036944
		public override bool Equals(object o)
		{
			if (!(o is Ammo.AmmoData))
			{
				return false;
			}
			Ammo.AmmoData ammoData = (Ammo.AmmoData)o;
			if (this.id != ammoData.id || this.count != ammoData.count)
			{
				return false;
			}
			if (this.emotionData != null)
			{
				return this.emotionData.Equals(ammoData.emotionData);
			}
			return ammoData.emotionData == null;
		}

		// Token: 0x06000DDC RID: 3548 RVA: 0x000387A3 File Offset: 0x000369A3
		public override int GetHashCode()
		{
			return this.id.GetHashCode() ^ this.count.GetHashCode() ^ ((this.emotionData == null) ? 0 : this.emotionData.GetHashCode());
		}

		// Token: 0x04000D0E RID: 3342
		public Identifiable.Id id;

		// Token: 0x04000D0F RID: 3343
		public int count;

		// Token: 0x04000D10 RID: 3344
		public SlimeEmotionData emotionData;
	}
}
