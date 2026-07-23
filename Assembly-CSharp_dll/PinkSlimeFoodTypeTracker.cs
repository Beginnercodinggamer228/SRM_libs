using System;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000417 RID: 1047
public class PinkSlimeFoodTypeTracker : MonoBehaviour
{
	// Token: 0x060015E1 RID: 5601 RVA: 0x00054E00 File Offset: 0x00053000
	public void Start()
	{
		if (SRSingleton<SceneContext>.Instance != null)
		{
			AchievementsDirector achieveDir = SRSingleton<SceneContext>.Instance.AchievementsDirector;
			RegionMember member = base.GetComponent<RegionMember>();
			SlimeEat component = base.GetComponent<SlimeEat>();
			component.onEat = (SlimeEat.OnEatDelegate)Delegate.Combine(component.onEat, new SlimeEat.OnEatDelegate(delegate(Identifiable.Id eatId)
			{
				if (Identifiable.IsFood(eatId) && CellDirector.IsOnRanch(member))
				{
					achieveDir.AddToStat(AchievementsDirector.EnumStat.PINK_SLIMES_FOOD_TYPES, eatId);
				}
			}));
		}
	}
}
