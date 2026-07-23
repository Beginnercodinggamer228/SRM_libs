using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000267 RID: 615
[Serializable]
public class WorldData : DataModule<WorldData>
{
	// Token: 0x06000CF7 RID: 3319 RVA: 0x00003296 File Offset: 0x00001496
	public static void AssertEquals(WorldData dataA, WorldData dataB)
	{
	}

	// Token: 0x06000CF8 RID: 3320 RVA: 0x00035744 File Offset: 0x00033944
	private static string PrintResourceSpawnerWater(Dictionary<Vector3, WorldData.ResourceWater> resourceSpawnerWater)
	{
		string text = "ResourceSpawnerWater: ";
		foreach (KeyValuePair<Vector3, WorldData.ResourceWater> keyValuePair in resourceSpawnerWater)
		{
			text = string.Concat(new object[]
			{
				text,
				keyValuePair.Key,
				":",
				keyValuePair.Value.spawn,
				":",
				keyValuePair.Value.water,
				","
			});
		}
		return text;
	}

	// Token: 0x04000C20 RID: 3104
	public const int CURR_FORMAT_ID = 6;

	// Token: 0x04000C21 RID: 3105
	public float worldTime;

	// Token: 0x04000C22 RID: 3106
	public float econSeed;

	// Token: 0x04000C23 RID: 3107
	public Dictionary<Identifiable.Id, float> econSaturations;

	// Token: 0x04000C24 RID: 3108
	public Dictionary<Vector3, WorldData.ResourceWater> resourceSpawnerWater;

	// Token: 0x04000C25 RID: 3109
	public Dictionary<Vector3, float> spawnerTriggerTimes;

	// Token: 0x04000C26 RID: 3110
	public Dictionary<string, bool> teleportNodeActivations;

	// Token: 0x04000C27 RID: 3111
	public Dictionary<Vector3, float> animalSpawnerTimes;

	// Token: 0x04000C28 RID: 3112
	public ExchangeDirector.Offer offer;

	// Token: 0x04000C29 RID: 3113
	public float dailyOfferCreateTime;

	// Token: 0x04000C2A RID: 3114
	public string lastRancherOfferId;

	// Token: 0x04000C2B RID: 3115
	public Dictionary<Vector3, float> liquidSourceUnits;

	// Token: 0x04000C2C RID: 3116
	public AmbianceDirector.Weather weather;

	// Token: 0x04000C2D RID: 3117
	public float weatherUntil;

	// Token: 0x04000C2E RID: 3118
	public Dictionary<Vector3, int> gordoEatenCounts;

	// Token: 0x04000C2F RID: 3119
	private const float MAX_DIST_MATCH = 5f;

	// Token: 0x04000C30 RID: 3120
	private const float MAX_DIST_MATCH_SQR = 25f;

	// Token: 0x04000C31 RID: 3121
	private const float MAX_DIST_CLOSE_MATCH = 0.1f;

	// Token: 0x04000C32 RID: 3122
	private const float MAX_DIST_CLOSE_MATCH_SQR = 0.010000001f;

	// Token: 0x02000268 RID: 616
	[Serializable]
	public class ResourceWater : IEquatable<WorldData.ResourceWater>
	{
		// Token: 0x06000CFA RID: 3322 RVA: 0x000357FC File Offset: 0x000339FC
		public ResourceWater(float spawn, float water)
		{
			this.spawn = spawn;
			this.water = water;
		}

		// Token: 0x06000CFB RID: 3323 RVA: 0x00035812 File Offset: 0x00033A12
		public bool Equals(WorldData.ResourceWater that)
		{
			return this.spawn == that.spawn && this.water == that.water;
		}

		// Token: 0x06000CFC RID: 3324 RVA: 0x00035832 File Offset: 0x00033A32
		public override bool Equals(object o)
		{
			return o is WorldData.ResourceWater && this.Equals((WorldData.ResourceWater)o);
		}

		// Token: 0x06000CFD RID: 3325 RVA: 0x0003584A File Offset: 0x00033A4A
		public override int GetHashCode()
		{
			return this.spawn.GetHashCode() ^ this.water.GetHashCode();
		}

		// Token: 0x04000C33 RID: 3123
		public float spawn;

		// Token: 0x04000C34 RID: 3124
		public float water;
	}
}
