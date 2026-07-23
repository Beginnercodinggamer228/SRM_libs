using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200045F RID: 1119
[Serializable]
public class SlimeDiet
{
	// Token: 0x06001710 RID: 5904 RVA: 0x000597EC File Offset: 0x000579EC
	public static SlimeDiet Combine(SlimeDiet diet1, SlimeDiet diet2)
	{
		return new SlimeDiet
		{
			MajorFoodGroups = diet1.MajorFoodGroups.Union(diet2.MajorFoodGroups).ToArray<SlimeEat.FoodGroup>(),
			Favorites = diet1.Favorites.Union(diet2.Favorites).ToArray<Identifiable.Id>(),
			AdditionalFoods = diet1.AdditionalFoods.Union(diet2.AdditionalFoods).ToArray<Identifiable.Id>(),
			Produces = diet1.Produces.Union(diet2.Produces).ToArray<Identifiable.Id>(),
			FavoriteProductionCount = diet1.FavoriteProductionCount
		};
	}

	// Token: 0x06001711 RID: 5905 RVA: 0x0005987C File Offset: 0x00057A7C
	public IEnumerable<Identifiable.Id> GetDietIdentifiableIds()
	{
		return new HashSet<Identifiable.Id>(this.MajorFoodGroups.SelectMany((SlimeEat.FoodGroup group) => SlimeEat.GetFoodGroupIds(group)).Concat(this.AdditionalFoods)).AsEnumerable<Identifiable.Id>();
	}

	// Token: 0x06001712 RID: 5906 RVA: 0x000598C8 File Offset: 0x00057AC8
	public void RefreshEatMap(SlimeDefinitions definitions, SlimeDefinition definition)
	{
		this.EatMap = new List<SlimeDiet.EatMapEntry>();
		foreach (Identifiable.Id id3 in this.GetDietIdentifiableIds())
		{
			SlimeEmotions.Emotion driver = (id3 != Identifiable.Id.SPICY_TOFU) ? SlimeEmotions.Emotion.HUNGER : SlimeEmotions.Emotion.NONE;
			foreach (Identifiable.Id producesId in this.Produces)
			{
				SlimeDiet.EatMapEntry item = new SlimeDiet.EatMapEntry
				{
					eats = id3,
					producesId = producesId,
					isFavorite = this.Favorites.Contains(id3),
					favoriteProductionCount = this.FavoriteProductionCount,
					driver = driver,
					minDrive = 0f,
					extraDrive = 0f,
					becomesId = Identifiable.Id.NONE
				};
				this.EatMap.Add(item);
			}
		}
		if (this.ProducePlorts() && (definition.IsLargo || definition.CanLargofy))
		{
			foreach (Identifiable.Id id2 in from id in SlimeEat.GetFoodGroupIds(SlimeEat.FoodGroup.PLORTS)
			where id != Identifiable.Id.QUICKSILVER_PLORT
			select id)
			{
				if (!this.Produces.Contains(id2))
				{
					Identifiable.Id becomesId;
					if (definition.IsLargo)
					{
						becomesId = Identifiable.Id.TARR_SLIME;
					}
					else
					{
						SlimeDefinition largoByPlorts = definitions.GetLargoByPlorts(id2, this.Produces[0]);
						if (largoByPlorts == null)
						{
							continue;
						}
						becomesId = largoByPlorts.IdentifiableId;
					}
					SlimeDiet.EatMapEntry item2 = new SlimeDiet.EatMapEntry
					{
						eats = id2,
						producesId = Identifiable.Id.NONE,
						isFavorite = false,
						favoriteProductionCount = this.FavoriteProductionCount,
						driver = SlimeEmotions.Emotion.AGITATION,
						minDrive = 0.5f,
						extraDrive = 0f,
						becomesId = becomesId
					};
					this.EatMap.Add(item2);
				}
			}
		}
	}

	// Token: 0x06001713 RID: 5907 RVA: 0x00059AD4 File Offset: 0x00057CD4
	public void AddEatMapEntries(Identifiable.Id id, IList<SlimeDiet.EatMapEntry> targetEntries)
	{
		for (int i = 0; i < this.EatMap.Count; i++)
		{
			if (this.EatMap[i].eats == id)
			{
				targetEntries.Add(this.EatMap[i]);
			}
		}
	}

	// Token: 0x06001714 RID: 5908 RVA: 0x00059B1D File Offset: 0x00057D1D
	private bool ProducePlorts()
	{
		return this.Produces.Count((Identifiable.Id id) => Identifiable.IsPlort(id)) > 0;
	}

	// Token: 0x06001715 RID: 5909 RVA: 0x00059B4C File Offset: 0x00057D4C
	public static string GetFoodCategoryMsg(Identifiable.Id id)
	{
		if (Array.IndexOf<Identifiable.Id>(SlimeEat.GetFoodGroupIds(SlimeEat.FoodGroup.VEGGIES), id) != -1)
		{
			return "m.foodgroup.veggies";
		}
		if (Array.IndexOf<Identifiable.Id>(SlimeEat.GetFoodGroupIds(SlimeEat.FoodGroup.FRUIT), id) != -1)
		{
			return "m.foodgroup.fruit";
		}
		if (Array.IndexOf<Identifiable.Id>(SlimeEat.GetFoodGroupIds(SlimeEat.FoodGroup.MEAT), id) != -1)
		{
			return "m.foodgroup.meat";
		}
		if (Array.IndexOf<Identifiable.Id>(SlimeEat.GetFoodGroupIds(SlimeEat.FoodGroup.GINGER), id) != -1)
		{
			return "m.foodgroup.ginger";
		}
		return "m.foodgroup.none";
	}

	// Token: 0x06001716 RID: 5910 RVA: 0x00059BB2 File Offset: 0x00057DB2
	public string GetDirectFoodGroupsMsg()
	{
		return this.GetGroupsMsg(this.MajorFoodGroups);
	}

	// Token: 0x06001717 RID: 5911 RVA: 0x00059BC0 File Offset: 0x00057DC0
	public string GetModulesFoodGroupsMsg()
	{
		HashSet<SlimeEat.FoodGroup> hashSet = new HashSet<SlimeEat.FoodGroup>();
		foreach (SlimeEat.FoodGroup item in this.MajorFoodGroups)
		{
			hashSet.Add(item);
		}
		return this.GetGroupsMsg(hashSet);
	}

	// Token: 0x06001718 RID: 5912 RVA: 0x00059BFC File Offset: 0x00057DFC
	private string GetGroupsMsg(ICollection<SlimeEat.FoodGroup> groups)
	{
		switch (groups.Count)
		{
		case 0:
			return "m.foodgroup.none";
		case 1:
		{
			string str = Enum.GetName(typeof(SlimeEat.FoodGroup), groups.First<SlimeEat.FoodGroup>()).ToLowerInvariant();
			return "m.foodgroup." + str;
		}
		case 3:
			return "m.foodgroup.all";
		}
		string[] array = new string[groups.Count];
		int num = 0;
		foreach (SlimeEat.FoodGroup foodGroup in groups)
		{
			string str2 = Enum.GetName(typeof(SlimeEat.FoodGroup), foodGroup).ToLowerInvariant();
			array[num++] = "m.foodgroup." + str2;
		}
		return MessageUtil.Compose("m.andlist" + groups.Count, array);
	}

	// Token: 0x04001642 RID: 5698
	public SlimeEat.FoodGroup[] MajorFoodGroups;

	// Token: 0x04001643 RID: 5699
	public Identifiable.Id[] Favorites;

	// Token: 0x04001644 RID: 5700
	public Identifiable.Id[] AdditionalFoods;

	// Token: 0x04001645 RID: 5701
	public Identifiable.Id[] Produces;

	// Token: 0x04001646 RID: 5702
	public int FavoriteProductionCount;

	// Token: 0x04001647 RID: 5703
	[HideInInspector]
	public List<SlimeDiet.EatMapEntry> EatMap;

	// Token: 0x02000460 RID: 1120
	public class EatMapEntry
	{
		// Token: 0x0600171A RID: 5914 RVA: 0x00059CF4 File Offset: 0x00057EF4
		public int NumToProduce()
		{
			if (!this.isFavorite)
			{
				return 1;
			}
			return this.favoriteProductionCount;
		}

		// Token: 0x04001648 RID: 5704
		public Identifiable.Id eats;

		// Token: 0x04001649 RID: 5705
		public bool isFavorite;

		// Token: 0x0400164A RID: 5706
		public int favoriteProductionCount = 2;

		// Token: 0x0400164B RID: 5707
		public Identifiable.Id producesId;

		// Token: 0x0400164C RID: 5708
		public Identifiable.Id becomesId;

		// Token: 0x0400164D RID: 5709
		public SlimeEmotions.Emotion driver;

		// Token: 0x0400164E RID: 5710
		public float extraDrive;

		// Token: 0x0400164F RID: 5711
		public float minDrive;
	}
}
