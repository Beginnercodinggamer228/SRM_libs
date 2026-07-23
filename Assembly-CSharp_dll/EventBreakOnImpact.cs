using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x020006F6 RID: 1782
public class EventBreakOnImpact : BreakOnImpactBase
{
	// Token: 0x06002524 RID: 9508 RVA: 0x0008EA5A File Offset: 0x0008CC5A
	protected override IEnumerable<GameObject> GetRewardPrefabs()
	{
		yield return SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(this.PickOrnamentReward());
		yield break;
	}

	// Token: 0x06002525 RID: 9509 RVA: 0x0008EA6C File Offset: 0x0008CC6C
	private Identifiable.Id PickOrnamentReward()
	{
		HolidayModel.EventGordo eventGordo = Randoms.SHARED.Pick<HolidayModel.EventGordo>(SRSingleton<SceneContext>.Instance.GameModel.GetHolidayModel().eventGordos, null);
		if (eventGordo is HolidayModel.EventGordo.Default)
		{
			if (!Randoms.SHARED.GetProbability(HolidayModel.EventGordo.RARE_ORNAMENT_CHANCE))
			{
				return Randoms.SHARED.Pick<Identifiable.Id>(((HolidayModel.EventGordo.Default)eventGordo).commons);
			}
			return Randoms.SHARED.Pick<Identifiable.Id>(HolidayModel.EventGordo.RARE_ORNAMENTS);
		}
		else
		{
			if (eventGordo is HolidayModel.EventGordo.Fixed)
			{
				return ((HolidayModel.EventGordo.Fixed)eventGordo).ornament;
			}
			if (!Randoms.SHARED.GetProbability(HolidayModel.EventGordo.RARE_ORNAMENT_CHANCE))
			{
				return Identifiable.Id.PINK_ORNAMENT;
			}
			return Randoms.SHARED.Pick<Identifiable.Id>(HolidayModel.EventGordo.RARE_ORNAMENTS);
		}
	}
}
