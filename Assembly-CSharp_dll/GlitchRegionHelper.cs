using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Util.Extensions;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020004ED RID: 1261
public class GlitchRegionHelper : SRSingleton<GlitchRegionHelper>, AmbianceDirector.TimeOfDay, PlayerModel.Participant
{
	// Token: 0x170001E7 RID: 487
	// (get) Token: 0x06001A53 RID: 6739 RVA: 0x0006646A File Offset: 0x0006466A
	// (set) Token: 0x06001A54 RID: 6740 RVA: 0x00066472 File Offset: 0x00064672
	public GlitchImpostoDirector[] impostoDirectors { get; private set; }

	// Token: 0x170001E8 RID: 488
	// (get) Token: 0x06001A55 RID: 6741 RVA: 0x0006647B File Offset: 0x0006467B
	// (set) Token: 0x06001A56 RID: 6742 RVA: 0x00066483 File Offset: 0x00064683
	public CellDirector[] cellDirectors { get; private set; }

	// Token: 0x170001E9 RID: 489
	// (get) Token: 0x06001A57 RID: 6743 RVA: 0x0006648C File Offset: 0x0006468C
	// (set) Token: 0x06001A58 RID: 6744 RVA: 0x00066494 File Offset: 0x00064694
	public GlitchLiquidSource[] stations { get; private set; }

	// Token: 0x170001EA RID: 490
	// (get) Token: 0x06001A59 RID: 6745 RVA: 0x0006649D File Offset: 0x0006469D
	// (set) Token: 0x06001A5A RID: 6746 RVA: 0x000664A5 File Offset: 0x000646A5
	public GlitchTarrNode[] nodes { get; private set; }

	// Token: 0x170001EB RID: 491
	// (get) Token: 0x06001A5B RID: 6747 RVA: 0x000664AE File Offset: 0x000646AE
	// (set) Token: 0x06001A5C RID: 6748 RVA: 0x000664B6 File Offset: 0x000646B6
	public GlitchBreadcrumbNetwork breadcrumbs { get; private set; }

	// Token: 0x170001EC RID: 492
	// (get) Token: 0x06001A5D RID: 6749 RVA: 0x000664BF File Offset: 0x000646BF
	// (set) Token: 0x06001A5E RID: 6750 RVA: 0x000664C7 File Offset: 0x000646C7
	public Dictionary<string, GlitchTeleportDestination> destinationsDict { get; private set; }

	// Token: 0x170001ED RID: 493
	// (get) Token: 0x06001A5F RID: 6751 RVA: 0x000664D0 File Offset: 0x000646D0
	public IEnumerable<GlitchTeleportDestination> destinations
	{
		get
		{
			return this.destinationsDict.Values;
		}
	}

	// Token: 0x06001A60 RID: 6752 RVA: 0x000664E0 File Offset: 0x000646E0
	public override void Awake()
	{
		base.Awake();
		SRSingleton<SceneContext>.Instance.GameModel.RegisterPlayerParticipant(this);
		SRSingleton<SceneContext>.Instance.RegionRegistry.ManageWithRegionSet(base.gameObject, RegionRegistry.RegionSetId.SLIMULATIONS);
		ZoneDirector componentInParent = base.GetComponentInParent<ZoneDirector>();
		this.impostoDirectors = componentInParent.GetComponentsInChildren<GlitchImpostoDirector>(true);
		this.cellDirectors = componentInParent.GetComponentsInChildren<CellDirector>(true);
		this.stations = componentInParent.GetComponentsInChildren<GlitchLiquidSource>(true);
		this.nodes = componentInParent.GetComponentsInChildren<GlitchTarrNode>(true);
		this.breadcrumbs = componentInParent.GetRequiredComponentInChildren<GlitchBreadcrumbNetwork>(true);
		this.destinationsDict = componentInParent.GetComponentsInChildren<GlitchTeleportDestination>(true).ToDictionary((GlitchTeleportDestination d) => d.id, (GlitchTeleportDestination d) => d);
		this.breadcrumbs.OnGlitchRegionLoaded();
	}

	// Token: 0x06001A61 RID: 6753 RVA: 0x000665BC File Offset: 0x000647BC
	public override void OnDestroy()
	{
		base.OnDestroy();
		if (SRSingleton<SceneContext>.Instance != null && SRSingleton<SceneContext>.Instance.RegionRegistry != null)
		{
			SRSingleton<SceneContext>.Instance.RegionRegistry.ReleaseFromRegionSet(base.gameObject, RegionRegistry.RegionSetId.SLIMULATIONS);
		}
	}

	// Token: 0x06001A62 RID: 6754 RVA: 0x000665F9 File Offset: 0x000647F9
	public void OnEnable()
	{
		SRSingleton<SceneContext>.Instance.AmbianceDirector.Register(this);
	}

	// Token: 0x06001A63 RID: 6755 RVA: 0x0006660B File Offset: 0x0006480B
	public void OnDisable()
	{
		if (SRSingleton<SceneContext>.Instance != null && SRSingleton<SceneContext>.Instance.AmbianceDirector != null)
		{
			SRSingleton<SceneContext>.Instance.AmbianceDirector.Deregister(this);
		}
	}

	// Token: 0x06001A64 RID: 6756 RVA: 0x0006663C File Offset: 0x0006483C
	public void RegionSetChanged(RegionRegistry.RegionSetId previous, RegionRegistry.RegionSetId current)
	{
		if (current == RegionRegistry.RegionSetId.SLIMULATIONS)
		{
			TimeDirector timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
			GlitchMetadata glitch = SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch;
			PlayerState playerState = SRSingleton<SceneContext>.Instance.PlayerState;
			SRSingleton<DynamicObjectContainer>.Instance.DestroyChildren(delegate(GameObject go)
			{
				RegionMember component = go.GetComponent<RegionMember>();
				return component != null && component.IsInRegion(RegionRegistry.RegionSetId.SLIMULATIONS);
			}, "GlitchRegionHelper.RegionSetChanged");
			GlitchImpostoDirector[] impostoDirectors = this.impostoDirectors;
			for (int i = 0; i < impostoDirectors.Length; i++)
			{
				impostoDirectors[i].ResetImpostos();
			}
			CellDirector[] cellDirectors = this.cellDirectors;
			for (int i = 0; i < cellDirectors.Length; i++)
			{
				cellDirectors[i].ForceCheckSpawn();
			}
			GlitchLiquidSource[] stations = this.stations;
			for (int i = 0; i < stations.Length; i++)
			{
				stations[i].ResetLiquidState();
			}
			List<GlitchTarrNode.Group> list = Enum.GetValues(typeof(GlitchTarrNode.Group)).Cast<GlitchTarrNode.Group>().ToList<GlitchTarrNode.Group>();
			list.Sort(from it in new GlitchRegionHelper.GlitchTarrNodeGroupComparer()
			orderby Randoms.SHARED.GetInt()
			select it);
			foreach (GlitchTarrNode glitchTarrNode in this.nodes)
			{
				glitchTarrNode.ResetNode(timeDirector.WorldTime() + (double)((glitch.tarrNodeActivationDelay + (float)(list.IndexOf(glitchTarrNode.activationGroup) + list.Count * glitchTarrNode.activationIndex) * glitch.tarrNodeActivationDelayPerNode) * 3600f));
			}
			foreach (GlitchTeleportDestination glitchTeleportDestination in this.destinations)
			{
				glitchTeleportDestination.Reset(null);
			}
			Randoms.SHARED.Pick<GlitchTeleportDestination>(from e in this.destinations
			where e.isPotentialExitDestination
			select e, null).Reset(new double?(timeDirector.HoursFromNow(glitch.teleportActivationDelay.GetRandom())));
			playerState.Ammo.Replace(Identifiable.Id.GLITCH_BUG_REPORT, Identifiable.Id.GLITCH_SLIME);
		}
		if (previous == RegionRegistry.RegionSetId.SLIMULATIONS)
		{
			PlayerState player = SRSingleton<SceneContext>.Instance.PlayerState;
			foreach (GlitchTeleportDestination glitchTeleportDestination2 in this.destinations)
			{
				glitchTeleportDestination2.Reset(new double?(0.0));
			}
			SRSingleton<DynamicObjectContainer>.Instance.DestroyChildren(delegate(GameObject go)
			{
				RegionMember component = go.GetComponent<RegionMember>();
				return component != null && component.IsInRegion(RegionRegistry.RegionSetId.SLIMULATIONS);
			}, "GlitchRegionHelper.RegionSetChanged");
			Destroyer.Destroy(this.exitHudInstance, "GlitchRegionHelper.RegionSetChanged");
			player.Ammo.Replace(Identifiable.Id.GLITCH_SLIME, Identifiable.Id.GLITCH_BUG_REPORT);
			player.Ammo.Clear((int ii) => player.Ammo.GetSlotName(ii) != Identifiable.Id.GLITCH_BUG_REPORT);
		}
	}

	// Token: 0x06001A65 RID: 6757 RVA: 0x00066954 File Offset: 0x00064B54
	public void OnExitTeleporterBecameActive()
	{
		GlitchMetadata glitch = SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch;
		Destroyer.Destroy(this.exitHudInstance, "GlitchRegionHelper.OnExitTeleporterBecameActive");
		this.exitHudInstance = UnityEngine.Object.Instantiate<GameObject>(glitch.teleportHudPrefab, SRSingleton<HudUI>.Instance.uiContainer.transform);
		this.exitHudInstance.StartCoroutine(GlitchRegionHelper.OnExitTeleporterBecameActive_Coroutine(this.exitHudInstance));
	}

	// Token: 0x06001A66 RID: 6758 RVA: 0x000669B7 File Offset: 0x00064BB7
	private static IEnumerator OnExitTeleporterBecameActive_Coroutine(GameObject instance)
	{
		GlitchMetadata glitch = SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch;
		yield return new WaitForSeconds(glitch.teleportHudLifetime);
		instance.GetRequiredComponent<Animator>().SetBool("state_active", false);
		yield break;
	}

	// Token: 0x06001A67 RID: 6759 RVA: 0x000669C6 File Offset: 0x00064BC6
	public float GetCurrentDayFraction_Position()
	{
		return SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch.ambianceTimeOfDay;
	}

	// Token: 0x06001A68 RID: 6760 RVA: 0x000669C6 File Offset: 0x00064BC6
	public float GetCurrentDayFraction_Color()
	{
		return SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch.ambianceTimeOfDay;
	}

	// Token: 0x06001A69 RID: 6761 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(PlayerModel model)
	{
	}

	// Token: 0x06001A6A RID: 6762 RVA: 0x00003296 File Offset: 0x00001496
	public void SetModel(PlayerModel model)
	{
	}

	// Token: 0x06001A6B RID: 6763 RVA: 0x00003296 File Offset: 0x00001496
	public void TransformChanged(Vector3 position, Quaternion rotation)
	{
	}

	// Token: 0x06001A6C RID: 6764 RVA: 0x00003296 File Offset: 0x00001496
	public void RegisteredPotentialAmmoChanged(Dictionary<PlayerState.AmmoMode, List<GameObject>> ammo)
	{
	}

	// Token: 0x06001A6D RID: 6765 RVA: 0x00003296 File Offset: 0x00001496
	public void KeyAdded()
	{
	}

	// Token: 0x04001A04 RID: 6660
	[Tooltip("Renderer to update the material on death in SLIMULATIONS region.")]
	public Renderer seaRenderer;

	// Token: 0x04001A0B RID: 6667
	private GameObject exitHudInstance;

	// Token: 0x020004EE RID: 1262
	private class GlitchTarrNodeGroupComparer : SRComparer<GlitchTarrNode.Group>
	{
	}
}
