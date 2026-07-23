using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000463 RID: 1123
[CreateAssetMenu(menuName = "Slimes/Slime Definitions")]
public class SlimeDefinitions : ScriptableObject
{
	// Token: 0x06001725 RID: 5925 RVA: 0x00059D4A File Offset: 0x00057F4A
	public void OnEnable()
	{
		this.RefreshIndexes();
		this.RefreshDefinitions();
	}

	// Token: 0x06001726 RID: 5926 RVA: 0x00059D58 File Offset: 0x00057F58
	public void RefreshIndexes()
	{
		foreach (SlimeDefinition slimeDefinition in this.Slimes)
		{
			try
			{
				this.slimeDefinitionsByIdentifiable.Add(slimeDefinition.IdentifiableId, slimeDefinition);
				if (slimeDefinition.IsLargo && slimeDefinition.BaseSlimes.Length == 2)
				{
					this.largoDefinitionByBasePlorts.Add(new SlimeDefinitions.PlortPair(slimeDefinition.BaseSlimes[0].Diet.Produces[0], slimeDefinition.BaseSlimes[1].Diet.Produces[0]), slimeDefinition);
					this.largoDefinitionByBaseDefinitions.Add(new SlimeDefinitions.SlimeDefinitionPair(slimeDefinition.BaseSlimes[0], slimeDefinition.BaseSlimes[1]), slimeDefinition);
				}
			}
			catch (Exception ex)
			{
				Log.Error("Exception caught while attempting to process slime.", new object[]
				{
					"name",
					slimeDefinition.Name,
					"Exception",
					ex.Message,
					"Stacktrace",
					ex.StackTrace.ToString()
				});
			}
		}
	}

	// Token: 0x06001727 RID: 5927 RVA: 0x00059E60 File Offset: 0x00058060
	public void RefreshDefinitions()
	{
		foreach (SlimeDefinition slimeDefinition in this.Slimes)
		{
			slimeDefinition.Diet.RefreshEatMap(this, slimeDefinition);
		}
	}

	// Token: 0x06001728 RID: 5928 RVA: 0x00059E94 File Offset: 0x00058094
	public SlimeDefinition GetLargoByPlorts(Identifiable.Id plort1, Identifiable.Id plort2)
	{
		SlimeDefinition result = null;
		this.largoDefinitionByBasePlorts.TryGetValue(new SlimeDefinitions.PlortPair(plort1, plort2), out result);
		return result;
	}

	// Token: 0x06001729 RID: 5929 RVA: 0x00059EBC File Offset: 0x000580BC
	public SlimeDefinition GetLargoByBaseSlimes(SlimeDefinition slimeDefinition1, SlimeDefinition slimeDefinition2)
	{
		SlimeDefinition result = null;
		this.largoDefinitionByBaseDefinitions.TryGetValue(new SlimeDefinitions.SlimeDefinitionPair(slimeDefinition1, slimeDefinition2), out result);
		return result;
	}

	// Token: 0x0600172A RID: 5930 RVA: 0x00059EE4 File Offset: 0x000580E4
	public SlimeDefinition GetSlimeByIdentifiableId(Identifiable.Id id)
	{
		SlimeDefinition result = null;
		this.slimeDefinitionsByIdentifiable.TryGetValue(id, out result);
		return result;
	}

	// Token: 0x04001655 RID: 5717
	public SlimeDefinition[] Slimes;

	// Token: 0x04001656 RID: 5718
	private Dictionary<Identifiable.Id, SlimeDefinition> slimeDefinitionsByIdentifiable = new Dictionary<Identifiable.Id, SlimeDefinition>(Identifiable.idComparer);

	// Token: 0x04001657 RID: 5719
	private Dictionary<SlimeDefinitions.PlortPair, SlimeDefinition> largoDefinitionByBasePlorts = new Dictionary<SlimeDefinitions.PlortPair, SlimeDefinition>(SlimeDefinitions.PlortPair.EqualityComparer.Default);

	// Token: 0x04001658 RID: 5720
	private Dictionary<SlimeDefinitions.SlimeDefinitionPair, SlimeDefinition> largoDefinitionByBaseDefinitions = new Dictionary<SlimeDefinitions.SlimeDefinitionPair, SlimeDefinition>(SlimeDefinitions.SlimeDefinitionPair.EqualityComparer.Default);

	// Token: 0x02000464 RID: 1124
	private struct PlortPair
	{
		// Token: 0x0600172C RID: 5932 RVA: 0x00059F3B File Offset: 0x0005813B
		public PlortPair(Identifiable.Id plort1, Identifiable.Id plort2)
		{
			if (plort1 <= plort2)
			{
				this.Plort1 = plort1;
				this.Plort2 = plort2;
				return;
			}
			this.Plort1 = plort2;
			this.Plort2 = plort1;
		}

		// Token: 0x04001659 RID: 5721
		public Identifiable.Id Plort1;

		// Token: 0x0400165A RID: 5722
		public Identifiable.Id Plort2;

		// Token: 0x02000465 RID: 1125
		public class EqualityComparer : IEqualityComparer<SlimeDefinitions.PlortPair>
		{
			// Token: 0x0600172D RID: 5933 RVA: 0x00059F5E File Offset: 0x0005815E
			public bool Equals(SlimeDefinitions.PlortPair x, SlimeDefinitions.PlortPair y)
			{
				return x.Plort1 == y.Plort1 && x.Plort2 == y.Plort2;
			}

			// Token: 0x0600172E RID: 5934 RVA: 0x00059F7E File Offset: 0x0005817E
			public int GetHashCode(SlimeDefinitions.PlortPair obj)
			{
				return (int)(((int)obj.Plort1 << 5) + (int)obj.Plort1 ^ obj.Plort2);
			}

			// Token: 0x0400165B RID: 5723
			public static SlimeDefinitions.PlortPair.EqualityComparer Default = new SlimeDefinitions.PlortPair.EqualityComparer();
		}
	}

	// Token: 0x02000466 RID: 1126
	private struct SlimeDefinitionPair
	{
		// Token: 0x06001731 RID: 5937 RVA: 0x00059FA2 File Offset: 0x000581A2
		public SlimeDefinitionPair(SlimeDefinition slimeDefinition1, SlimeDefinition slimeDefinition2)
		{
			if (slimeDefinition1.GetHashCode() <= slimeDefinition2.GetHashCode())
			{
				this.SlimeDefinition1 = slimeDefinition1;
				this.SlimeDefinition2 = slimeDefinition2;
				return;
			}
			this.SlimeDefinition1 = slimeDefinition1;
			this.SlimeDefinition2 = slimeDefinition2;
		}

		// Token: 0x0400165C RID: 5724
		public SlimeDefinition SlimeDefinition1;

		// Token: 0x0400165D RID: 5725
		public SlimeDefinition SlimeDefinition2;

		// Token: 0x02000467 RID: 1127
		public class EqualityComparer : IEqualityComparer<SlimeDefinitions.SlimeDefinitionPair>
		{
			// Token: 0x06001732 RID: 5938 RVA: 0x00059FCF File Offset: 0x000581CF
			public bool Equals(SlimeDefinitions.SlimeDefinitionPair x, SlimeDefinitions.SlimeDefinitionPair y)
			{
				return x.SlimeDefinition1 == y.SlimeDefinition1 && x.SlimeDefinition2 == y.SlimeDefinition2;
			}

			// Token: 0x06001733 RID: 5939 RVA: 0x00059FF7 File Offset: 0x000581F7
			public int GetHashCode(SlimeDefinitions.SlimeDefinitionPair obj)
			{
				return (obj.SlimeDefinition1.GetHashCode() << 5) + obj.SlimeDefinition1.GetHashCode() ^ obj.SlimeDefinition2.GetHashCode();
			}

			// Token: 0x0400165E RID: 5726
			public static SlimeDefinitions.SlimeDefinitionPair.EqualityComparer Default = new SlimeDefinitions.SlimeDefinitionPair.EqualityComparer();
		}
	}
}
