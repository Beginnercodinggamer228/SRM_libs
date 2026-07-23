using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000256 RID: 598
[Serializable]
public class ActorsData : DataModule<ActorsData>
{
	// Token: 0x06000CCC RID: 3276 RVA: 0x00034B0A File Offset: 0x00032D0A
	public ActorsData.ActorData[] GetActors()
	{
		return this.actors;
	}

	// Token: 0x06000CCD RID: 3277 RVA: 0x00034B14 File Offset: 0x00032D14
	public static void AssertEquals(ActorsData dataA, ActorsData dataB)
	{
		TestUtil.Vector3Comparer vector3Comparer = new TestUtil.Vector3Comparer(0.1f, false);
		if (dataA.actors.Length == dataB.actors.Length)
		{
			List<ActorsData.ActorData> list = new List<ActorsData.ActorData>(dataB.actors);
			foreach (ActorsData.ActorData actorData in dataA.actors)
			{
				foreach (ActorsData.ActorData actorData2 in list)
				{
					if (vector3Comparer.Equals(actorData.pos, actorData2.pos) && actorData.id == actorData2.id)
					{
						ActorsData.AssertEqualActors(actorData, actorData2);
						list.Remove(actorData2);
						break;
					}
				}
			}
		}
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x00003296 File Offset: 0x00001496
	private static void AssertEqualActors(ActorsData.ActorData actorA, ActorsData.ActorData actorB)
	{
	}

	// Token: 0x04000BA2 RID: 2978
	public const int CURR_FORMAT_ID = 1;

	// Token: 0x04000BA3 RID: 2979
	private ActorsData.ActorData[] actors;

	// Token: 0x02000257 RID: 599
	[Serializable]
	public class ActorData
	{
		// Token: 0x04000BA4 RID: 2980
		public Vector3 pos;

		// Token: 0x04000BA5 RID: 2981
		public Vector3 rot;

		// Token: 0x04000BA6 RID: 2982
		public Identifiable.Id id;

		// Token: 0x04000BA7 RID: 2983
		public SlimeEmotionData emotions;

		// Token: 0x04000BA8 RID: 2984
		public float transformTime;

		// Token: 0x04000BA9 RID: 2985
		public float reproduceTime;

		// Token: 0x04000BAA RID: 2986
		public ResourceCycle.CycleData cycleData;

		// Token: 0x04000BAB RID: 2987
		public float? disabledAtTime;
	}
}
