using System;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x020003C7 RID: 967
public class EventGordoRewards : GordoRewardsBase
{
	// Token: 0x0600142D RID: 5165 RVA: 0x0004E090 File Offset: 0x0004C290
	protected override IEnumerable<GameObject> SelectActiveRewardPrefabs()
	{
		LookupDirector lookupDirector = SRSingleton<GameContext>.Instance.LookupDirector;
		List<GameObject> list = new List<GameObject>();
		list.Add(lookupDirector.GetPrefab(this.PickOrnamentReward()));
		list.AddRange(Enumerable.Repeat<GameObject>(lookupDirector.GetPrefab(HolidayModel.EventGordo.CRATE), this.cratesToSpawn));
		return list;
	}

	// Token: 0x0600142E RID: 5166 RVA: 0x0004E0DC File Offset: 0x0004C2DC
	protected override void OnInstantiatedReward(GameObject instance)
	{
		base.OnInstantiatedReward(instance);
		if (this.slimeFashion != null)
		{
			AttachFashions component = instance.GetComponent<AttachFashions>();
			if (component != null)
			{
				component.Attach(this.slimeFashion, true);
			}
		}
	}

	// Token: 0x0600142F RID: 5167 RVA: 0x0004E11C File Offset: 0x0004C31C
	private Identifiable.Id PickOrnamentReward()
	{
		string id = base.GetComponent<IdHandler>().id;
		HolidayModel.EventGordo eventGordo = SRSingleton<SceneContext>.Instance.GameModel.GetHolidayModel().eventGordos.FirstOrDefault((HolidayModel.EventGordo e) => e.objectId == id);
		if (eventGordo is HolidayModel.EventGordo.Fixed)
		{
			return ((HolidayModel.EventGordo.Fixed)eventGordo).ornament;
		}
		return Randoms.SHARED.Pick<Identifiable.Id>(HolidayModel.EventGordo.RARE_ORNAMENTS);
	}

	// Token: 0x040012DE RID: 4830
	[Tooltip("Fashion to attach to spawned slimes. (optional)")]
	public Fashion slimeFashion;

	// Token: 0x040012DF RID: 4831
	[Tooltip("Number of EventGordo crates to spawn on break.")]
	public int cratesToSpawn;
}
