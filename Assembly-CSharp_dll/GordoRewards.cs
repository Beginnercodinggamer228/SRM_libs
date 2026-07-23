using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003F3 RID: 1011
public class GordoRewards : GordoRewardsBase
{
	// Token: 0x06001522 RID: 5410 RVA: 0x00051FD4 File Offset: 0x000501D4
	protected override IEnumerable<GameObject> SelectActiveRewardPrefabs()
	{
		PlayerState.GameMode currGameMode = SRSingleton<SceneContext>.Instance.GameModel.currGameMode;
		foreach (GordoRewards.RewardOverride rewardOverride in this.rewardOverrides)
		{
			if (rewardOverride.gameMode == currGameMode)
			{
				return rewardOverride.rewardPrefabs;
			}
		}
		return this.rewardPrefabs;
	}

	// Token: 0x040013F6 RID: 5110
	[Tooltip("The default rewards to provide on popping the gordo")]
	public GameObject[] rewardPrefabs;

	// Token: 0x040013F7 RID: 5111
	[Tooltip("A set of overrides for different game modes on popping the gordo")]
	public GordoRewards.RewardOverride[] rewardOverrides;

	// Token: 0x020003F4 RID: 1012
	[Serializable]
	public class RewardOverride
	{
		// Token: 0x040013F8 RID: 5112
		public PlayerState.GameMode gameMode;

		// Token: 0x040013F9 RID: 5113
		public GameObject[] rewardPrefabs;
	}
}
