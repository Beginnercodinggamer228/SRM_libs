using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000733 RID: 1843
public class ModDirector : MonoBehaviour
{
	// Token: 0x06002682 RID: 9858 RVA: 0x00093264 File Offset: 0x00091464
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		foreach (ModDirector.Mod item in this.initMods)
		{
			this.activeMods.Add(item);
		}
	}

	// Token: 0x06002683 RID: 9859 RVA: 0x000932A6 File Offset: 0x000914A6
	public void InitForLevel()
	{
		this.NotifyModsChanged();
	}

	// Token: 0x06002684 RID: 9860 RVA: 0x000932AE File Offset: 0x000914AE
	public void ActivateMod(ModDirector.Mod mod)
	{
		if (!this.activeMods.Contains(mod))
		{
			this.activeMods.Add(mod);
			this.NotifyModsChanged();
		}
	}

	// Token: 0x06002685 RID: 9861 RVA: 0x000932D0 File Offset: 0x000914D0
	public void DeactivateMod(ModDirector.Mod mod)
	{
		if (this.activeMods.Contains(mod))
		{
			this.activeMods.Remove(mod);
			this.NotifyModsChanged();
		}
	}

	// Token: 0x06002686 RID: 9862 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	public bool IsModActive(ModDirector.Mod mod)
	{
		return false;
	}

	// Token: 0x06002687 RID: 9863 RVA: 0x000932F3 File Offset: 0x000914F3
	public void RegisterModsListener(ModDirector.ModsListener listener)
	{
		this.modsListeners = (ModDirector.ModsListener)Delegate.Combine(this.modsListeners, listener);
		listener();
	}

	// Token: 0x06002688 RID: 9864 RVA: 0x00093312 File Offset: 0x00091512
	public void UnregisterModsListener(ModDirector.ModsListener listener)
	{
		this.modsListeners = (ModDirector.ModsListener)Delegate.Remove(this.modsListeners, listener);
	}

	// Token: 0x06002689 RID: 9865 RVA: 0x00027ECC File Offset: 0x000260CC
	public float SlimeCountFactor()
	{
		return 1f;
	}

	// Token: 0x0600268A RID: 9866 RVA: 0x00027ECC File Offset: 0x000260CC
	public float SlimeHungerFactor()
	{
		return 1f;
	}

	// Token: 0x0600268B RID: 9867 RVA: 0x0004B32F File Offset: 0x0004952F
	public float ChanceOfTarrSpawn()
	{
		return 0f;
	}

	// Token: 0x0600268C RID: 9868 RVA: 0x0009332C File Offset: 0x0009152C
	private bool IsNight()
	{
		float num = this.timeDir.CurrDayFraction();
		return num < 0.25f || num > 0.75f;
	}

	// Token: 0x0600268D RID: 9869 RVA: 0x0004B32F File Offset: 0x0004952F
	public float ChanceRandomPlort()
	{
		return 0f;
	}

	// Token: 0x0600268E RID: 9870 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	public bool PlortsUnstable()
	{
		return false;
	}

	// Token: 0x0600268F RID: 9871 RVA: 0x00027ECC File Offset: 0x000260CC
	public float PlortPriceFactor(Identifiable.Id plortId)
	{
		return 1f;
	}

	// Token: 0x06002690 RID: 9872 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	public bool VampiricChickens()
	{
		return false;
	}

	// Token: 0x06002691 RID: 9873 RVA: 0x00093357 File Offset: 0x00091557
	private void NotifyModsChanged()
	{
		if (this.modsListeners != null)
		{
			this.modsListeners();
		}
	}

	// Token: 0x040025C1 RID: 9665
	private ModDirector.ModsListener modsListeners;

	// Token: 0x040025C2 RID: 9666
	[Tooltip("Chance of a tarr slime spawning in place of a normal slime at night during the mod.")]
	public float nightTarrChance = 0.05f;

	// Token: 0x040025C3 RID: 9667
	[Tooltip("Chance of a slime producing a random plort instead of its normal one during the mod.")]
	public float randomPlortChance = 0.1f;

	// Token: 0x040025C4 RID: 9668
	[Tooltip("Factor by which we increase the number of slimes spawned during the mod.")]
	public float increasedSlimeSpawnsFactor = 1.3f;

	// Token: 0x040025C5 RID: 9669
	[Tooltip("Factor by which slimes' hunger increases during the mod.")]
	public float hungerFactor = 2f;

	// Token: 0x040025C6 RID: 9670
	[Tooltip("Any mods we should activate immediately on startup, for testing")]
	public ModDirector.Mod[] initMods;

	// Token: 0x040025C7 RID: 9671
	private List<ModDirector.Mod> activeMods = new List<ModDirector.Mod>();

	// Token: 0x040025C8 RID: 9672
	private TimeDirector timeDir;

	// Token: 0x02000734 RID: 1844
	// (Invoke) Token: 0x06002694 RID: 9876
	public delegate void ModsListener();

	// Token: 0x02000735 RID: 1845
	public enum Mod
	{
		// Token: 0x040025CA RID: 9674
		VAMPIRIC_CHICKENS,
		// Token: 0x040025CB RID: 9675
		NIGHT_TARR_SLIMES,
		// Token: 0x040025CC RID: 9676
		RANDOM_PLORTS,
		// Token: 0x040025CD RID: 9677
		INCREASED_SLIME_SPAWNS,
		// Token: 0x040025CE RID: 9678
		SLIME_HUNGER,
		// Token: 0x040025CF RID: 9679
		UNSTABLE_PLORTS
	}
}
