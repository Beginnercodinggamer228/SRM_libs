using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006CA RID: 1738
public class BreakOnImpact : BreakOnImpactBase
{
	// Token: 0x0600242F RID: 9263 RVA: 0x0008B9E8 File Offset: 0x00089BE8
	public override void Awake()
	{
		base.Awake();
		foreach (BreakOnImpact.SpawnOption spawnOption in this.spawnOptions)
		{
			this.spawnWeights[spawnOption] = spawnOption.weight;
		}
	}

	// Token: 0x06002430 RID: 9264 RVA: 0x0008BA4C File Offset: 0x00089C4C
	protected override IEnumerable<GameObject> GetRewardPrefabs()
	{
		int numSpawns = Randoms.SHARED.GetInRange(this.minSpawns, this.maxSpawns);
		int num;
		for (int ii = 0; ii < numSpawns; ii = num)
		{
			BreakOnImpact.SpawnOption spawnOption = Randoms.SHARED.Pick<BreakOnImpact.SpawnOption>(this.spawnWeights, null);
			if (spawnOption != null)
			{
				yield return spawnOption.spawn;
			}
			num = ii + 1;
		}
		foreach (Identifiable.Id id in SRSingleton<SceneContext>.Instance.HolidayDirector.GetCurrOrnament())
		{
			yield return SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(id);
		}
		IEnumerator<Identifiable.Id> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x0400233D RID: 9021
	public int minSpawns = 2;

	// Token: 0x0400233E RID: 9022
	public int maxSpawns = 4;

	// Token: 0x0400233F RID: 9023
	public List<BreakOnImpact.SpawnOption> spawnOptions = new List<BreakOnImpact.SpawnOption>();

	// Token: 0x04002340 RID: 9024
	private Dictionary<BreakOnImpact.SpawnOption, float> spawnWeights = new Dictionary<BreakOnImpact.SpawnOption, float>();

	// Token: 0x020006CB RID: 1739
	[Serializable]
	public class SpawnOption
	{
		// Token: 0x04002341 RID: 9025
		public GameObject spawn;

		// Token: 0x04002342 RID: 9026
		public float weight;
	}
}
