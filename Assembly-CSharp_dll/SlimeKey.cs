using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000783 RID: 1923
public class SlimeKey : SRBehaviour
{
	// Token: 0x06002830 RID: 10288 RVA: 0x000987DC File Offset: 0x000969DC
	public void Awake()
	{
		SlimeKey.allKeys.Add(this);
		this.regionMember = base.GetComponent<RegionMember>();
	}

	// Token: 0x06002831 RID: 10289 RVA: 0x000987F5 File Offset: 0x000969F5
	public bool IsKeyInZone(ZoneDirector.Zone zoneId)
	{
		return this.regionMember.IsInZone(zoneId);
	}

	// Token: 0x06002832 RID: 10290 RVA: 0x00098803 File Offset: 0x00096A03
	public void OnDestroy()
	{
		SlimeKey.allKeys.Remove(this);
		this.regionMember = null;
	}

	// Token: 0x06002833 RID: 10291 RVA: 0x00098818 File Offset: 0x00096A18
	public void OnTriggerEnter(Collider col)
	{
		if (col.gameObject == SRSingleton<SceneContext>.Instance.Player)
		{
			SRSingleton<SceneContext>.Instance.PlayerState.AddKey();
			if (this.pickupFX != null)
			{
				SRBehaviour.SpawnAndPlayFX(this.pickupFX, base.transform.position, base.transform.rotation);
			}
			Destroyer.DestroyActor(base.gameObject, "SlimeKey.OnTriggerEnter", false);
		}
	}

	// Token: 0x040027CE RID: 10190
	public static List<SlimeKey> allKeys = new List<SlimeKey>();

	// Token: 0x040027CF RID: 10191
	public GameObject pickupFX;

	// Token: 0x040027D0 RID: 10192
	private RegionMember regionMember;
}
