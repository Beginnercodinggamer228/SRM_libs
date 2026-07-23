using System;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020004CB RID: 1227
public class SlimeLineup : MonoBehaviour
{
	// Token: 0x060019AC RID: 6572 RVA: 0x00064565 File Offset: 0x00062765
	public void ShowSlime(SlimeDefinition slimeDefinition)
	{
		this.CreateSlimePreviews(new List<SlimeDefinition>(new SlimeDefinition[]
		{
			slimeDefinition
		}));
	}

	// Token: 0x060019AD RID: 6573 RVA: 0x0006457C File Offset: 0x0006277C
	public void ShowSlimeAndLargos(SlimeDefinition slimeDefinition)
	{
		List<SlimeDefinition> list = new List<SlimeDefinition>(new SlimeDefinition[]
		{
			slimeDefinition
		});
		list.AddRange(from slime in this.slimeDefinitions.Slimes
		where slime.BaseSlimes.Contains(slimeDefinition)
		select slime);
		this.CreateSlimePreviews(list);
	}

	// Token: 0x060019AE RID: 6574 RVA: 0x000645D4 File Offset: 0x000627D4
	public void ShowAllBaseSlimes()
	{
		this.CreateSlimePreviews((from slime in this.slimeDefinitions.Slimes
		where !slime.IsLargo
		select slime).ToList<SlimeDefinition>());
	}

	// Token: 0x060019AF RID: 6575 RVA: 0x00064610 File Offset: 0x00062810
	public void ShowAllSlimes()
	{
		this.CreateSlimePreviews(new List<SlimeDefinition>(this.slimeDefinitions.Slimes));
	}

	// Token: 0x060019B0 RID: 6576 RVA: 0x00064628 File Offset: 0x00062828
	private void CreateSlimePreviews(List<SlimeDefinition> definitions)
	{
		this.ClearPreviews();
		int num = Mathf.CeilToInt(Mathf.Sqrt((float)definitions.Count));
		float num2 = this.spacing.x * ((float)num / 2f);
		float num3 = this.spacing.y * ((float)num / 2f);
		for (int i = 0; i < definitions.Count; i++)
		{
			Vector3 position = new Vector3((float)(i % num) * this.spacing.x - num2 + base.transform.position.x, base.transform.position.y, (float)(i / num) * this.spacing.y - num3 + base.transform.position.z);
			this.slimePreviews.Add(this.CreatePreviewSlime(definitions[i], position));
		}
	}

	// Token: 0x060019B1 RID: 6577 RVA: 0x00064704 File Offset: 0x00062904
	private GameObject CreatePreviewSlime(SlimeDefinition slimeDefinition, Vector3 position)
	{
		GameObject gameObject = SRSingleton<SceneContext>.Instance.GameModel.InstantiateActor(this.lookupDirector.GetPrefab(slimeDefinition.IdentifiableId), SRSingleton<SceneContext>.Instance.RegionRegistry.GetCurrentRegionSetId(), position, Quaternion.Euler(0f, 180f, 0f), false);
		foreach (Type type in SlimeLineup.componentBlacklist)
		{
			Component[] components = gameObject.GetComponents(type);
			for (int j = 0; j < components.Length; j++)
			{
				UnityEngine.Object.Destroy(components[j]);
			}
		}
		return gameObject;
	}

	// Token: 0x060019B2 RID: 6578 RVA: 0x00064798 File Offset: 0x00062998
	private void ClearPreviews()
	{
		foreach (GameObject obj in this.slimePreviews)
		{
			UnityEngine.Object.Destroy(obj);
		}
		this.slimePreviews.Clear();
	}

	// Token: 0x04001952 RID: 6482
	public LookupDirector lookupDirector;

	// Token: 0x04001953 RID: 6483
	public SlimeDefinitions slimeDefinitions;

	// Token: 0x04001954 RID: 6484
	public Vector2 spacing;

	// Token: 0x04001955 RID: 6485
	private List<GameObject> slimePreviews = new List<GameObject>();

	// Token: 0x04001956 RID: 6486
	private static readonly Type[] componentBlacklist = new Type[]
	{
		typeof(SlimeSubbehaviourPlexer),
		typeof(SlimeSubbehaviour),
		typeof(DestroyOnTouching),
		typeof(DestroyAfterTime),
		typeof(GlitchTarrSterilizeOnWater),
		typeof(GlintController),
		typeof(SlimeHealth),
		typeof(DestroyOutsideHoursOfDay),
		typeof(MaybeCullOnReenable),
		typeof(DamagePlayerOnTouch),
		typeof(FireSlimeIgnition),
		typeof(RegionMember)
	};
}
