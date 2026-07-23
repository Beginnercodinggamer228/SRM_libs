using System;
using UnityEngine;

// Token: 0x02000732 RID: 1842
public class MapDataEntry : SRBehaviour, TechActivator
{
	// Token: 0x0600267C RID: 9852 RVA: 0x000931A9 File Offset: 0x000913A9
	public void Start()
	{
		this.collider = base.GetRequiredComponent<Collider>();
		this.UpdateHologramState();
	}

	// Token: 0x0600267D RID: 9853 RVA: 0x000931C0 File Offset: 0x000913C0
	public void Activate()
	{
		if (this.IsZoneLocked())
		{
			SRSingleton<SceneContext>.Instance.PlayerState.UnlockMap(this.zone);
			this.UpdateHologramState();
			SRSingleton<SceneContext>.Instance.TutorialDirector.OnMapDataGained();
			SRSingleton<Map>.Instance.OpenMap(this.zone);
		}
	}

	// Token: 0x0600267E RID: 9854 RVA: 0x00093210 File Offset: 0x00091410
	private void UpdateHologramState()
	{
		bool flag = this.IsZoneLocked();
		this.hologram.SetActive(flag);
		this.activeFx.SetActive(flag);
		this.collider.enabled = flag;
	}

	// Token: 0x0600267F RID: 9855 RVA: 0x00025E60 File Offset: 0x00024060
	public GameObject GetCustomGuiPrefab()
	{
		return null;
	}

	// Token: 0x06002680 RID: 9856 RVA: 0x00093248 File Offset: 0x00091448
	private bool IsZoneLocked()
	{
		return !SRSingleton<SceneContext>.Instance.PlayerState.HasUnlockedMap(this.zone);
	}

	// Token: 0x040025BD RID: 9661
	[Tooltip("The zone for which we are giving map data.")]
	public ZoneDirector.Zone zone;

	// Token: 0x040025BE RID: 9662
	public GameObject hologram;

	// Token: 0x040025BF RID: 9663
	public GameObject activeFx;

	// Token: 0x040025C0 RID: 9664
	private Collider collider;
}
