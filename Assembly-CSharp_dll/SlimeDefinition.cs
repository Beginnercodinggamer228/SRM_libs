using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200045D RID: 1117
[CreateAssetMenu(menuName = "Slimes/Slime Definition")]
public class SlimeDefinition : ScriptableObject
{
	// Token: 0x170001CE RID: 462
	// (get) Token: 0x06001708 RID: 5896 RVA: 0x0005967E File Offset: 0x0005787E
	public IEnumerable<SlimeAppearance> Appearances
	{
		get
		{
			return this.AppearancesDefault.Concat(this.AppearancesDynamic);
		}
	}

	// Token: 0x06001709 RID: 5897 RVA: 0x00059694 File Offset: 0x00057894
	public void LoadDietFromBaseSlimes()
	{
		if (!this.IsLargo || this.BaseSlimes.Length != 2)
		{
			Log.Warning("Can't load diet. Slime is either not a largo or does not have two base slimes.", new object[]
			{
				"name",
				this.Name
			});
			return;
		}
		this.Diet = SlimeDiet.Combine(this.BaseSlimes[0].Diet, this.BaseSlimes[1].Diet);
	}

	// Token: 0x0600170A RID: 5898 RVA: 0x000596FC File Offset: 0x000578FC
	public void LoadFavoriteToysFromBaseSlimes()
	{
		if (!this.IsLargo || this.BaseSlimes.Length != 2)
		{
			Log.Warning("Can't load favorite toys. Slime is either not a largo or does not have two base slimes.", new object[]
			{
				"name",
				this.Name
			});
			return;
		}
		this.FavoriteToys = this.BaseSlimes[0].FavoriteToys.Union(this.BaseSlimes[1].FavoriteToys).ToArray<Identifiable.Id>();
	}

	// Token: 0x0600170B RID: 5899 RVA: 0x00059768 File Offset: 0x00057968
	public void RegisterDynamicAppearance(SlimeAppearance appearance)
	{
		if (!this.AppearancesDynamic.Contains(appearance))
		{
			this.AppearancesDynamic.Add(appearance);
			SRSingleton<SceneContext>.Instance.SlimeAppearanceDirector.RegisterDependentAppearances(this, appearance);
		}
	}

	// Token: 0x0600170C RID: 5900 RVA: 0x00059798 File Offset: 0x00057998
	public SlimeAppearance GetAppearanceForSet(SlimeAppearance.AppearanceSaveSet set)
	{
		return this.Appearances.FirstOrDefault((SlimeAppearance appearance) => appearance.SaveSet == set);
	}

	// Token: 0x04001634 RID: 5684
	public string Name;

	// Token: 0x04001635 RID: 5685
	public Identifiable.Id IdentifiableId;

	// Token: 0x04001636 RID: 5686
	public SlimeDefinition[] BaseSlimes;

	// Token: 0x04001637 RID: 5687
	public bool IsLargo;

	// Token: 0x04001638 RID: 5688
	public bool CanLargofy;

	// Token: 0x04001639 RID: 5689
	public float PrefabScale;

	// Token: 0x0400163A RID: 5690
	[Tooltip("Default slime appearances.")]
	[FormerlySerializedAs("Appearances")]
	public SlimeAppearance[] AppearancesDefault;

	// Token: 0x0400163B RID: 5691
	[NonSerialized]
	public List<SlimeAppearance> AppearancesDynamic = new List<SlimeAppearance>();

	// Token: 0x0400163C RID: 5692
	public SlimeSounds Sounds;

	// Token: 0x0400163D RID: 5693
	public SlimeDiet Diet;

	// Token: 0x0400163E RID: 5694
	public Identifiable.Id[] FavoriteToys;

	// Token: 0x0400163F RID: 5695
	public GameObject BaseModule;

	// Token: 0x04001640 RID: 5696
	public GameObject[] SlimeModules;
}
