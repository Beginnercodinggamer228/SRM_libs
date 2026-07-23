using System;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020006E8 RID: 1768
public class DirectedCrateSpawner : SRBehaviour
{
	// Token: 0x060024E3 RID: 9443 RVA: 0x0008DE10 File Offset: 0x0008C010
	public void Start()
	{
		this.zoneDirector = base.GetComponentInParent<ZoneDirector>();
		this.zoneDirector.Register(this);
	}

	// Token: 0x060024E4 RID: 9444 RVA: 0x0008DE2C File Offset: 0x0008C02C
	public GameObject Spawn(GameObject zoneCratePrefab)
	{
		RegionRegistry.RegionSetId regionSetId = this.zoneDirector.regionSetId;
		if (SRSingleton<SceneContext>.Instance.GameModel.GetHolidayModel().eventGordos.Any<HolidayModel.EventGordo>() && Randoms.SHARED.GetProbability(HolidayModel.EventGordo.CRATE_CHANCE))
		{
			return SRBehaviour.InstantiateActor(SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(HolidayModel.EventGordo.CRATE), regionSetId, base.transform.position, base.transform.rotation, false);
		}
		return SRBehaviour.InstantiateActor(zoneCratePrefab, regionSetId, base.transform.position, base.transform.rotation, false);
	}

	// Token: 0x040023D1 RID: 9169
	private ZoneDirector zoneDirector;
}
