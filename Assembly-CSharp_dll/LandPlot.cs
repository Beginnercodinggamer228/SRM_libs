using System;
using System.Collections.Generic;
using Assets.Script.Util.Extensions;
using DG.Tweening;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x0200031A RID: 794
public class LandPlot : SRBehaviour, LandPlotModel.Participant
{
	// Token: 0x060010D6 RID: 4310 RVA: 0x0004380C File Offset: 0x00041A0C
	public void InitModel(LandPlotModel model)
	{
		model.typeId = this.typeId;
	}

	// Token: 0x060010D7 RID: 4311 RVA: 0x0004381C File Offset: 0x00041A1C
	public void SetModel(LandPlotModel model)
	{
		this.model = model;
		this.ApplyUpgrades(model.upgrades);
		if (model.attachedId != SpawnResource.Id.NONE)
		{
			GameObject toAttach = UnityEngine.Object.Instantiate<GameObject>(SRSingleton<GameContext>.Instance.LookupDirector.GetResourcePrefab(model.attachedId), base.transform.position, base.transform.rotation);
			this.Attach(toAttach, true, false, null);
		}
	}

	// Token: 0x060010D8 RID: 4312 RVA: 0x0004387F File Offset: 0x00041A7F
	public void Awake()
	{
		this.achievementsDirector = SRSingleton<SceneContext>.Instance.AchievementsDirector;
	}

	// Token: 0x060010D9 RID: 4313 RVA: 0x00043891 File Offset: 0x00041A91
	public void Start()
	{
		this.droneNetwork = base.GetComponentInParent<DroneNetwork>();
		this.droneNetwork.Register(this);
	}

	// Token: 0x060010DA RID: 4314 RVA: 0x000438AB File Offset: 0x00041AAB
	public void OnDestroy()
	{
		if (this.droneNetwork != null)
		{
			this.droneNetwork.Deregister(this);
			this.droneNetwork = null;
		}
	}

	// Token: 0x060010DB RID: 4315 RVA: 0x000438D0 File Offset: 0x00041AD0
	public void Attach(GameObject toAttach, bool immediate, bool isReplacement, SECTR_AudioCue scaleUpCue = null)
	{
		toAttach.transform.SetParent(base.transform, true);
		this.attached = toAttach;
		SpawnResource component = this.attached.GetComponent<SpawnResource>();
		this.model.attachedId = ((component == null) ? SpawnResource.Id.NONE : component.id);
		this.model.attachedResourceId = component.GetPrimarySpawnId();
		if (!immediate)
		{
			SECTR_AudioSystem.Play(scaleUpCue, toAttach.transform, Vector3.zero, false, null, false);
			this.TweenScaleUpItem(toAttach, isReplacement);
		}
		if (this.typeId == LandPlot.Id.GARDEN)
		{
			Identifiable.Id attachedCropId = this.GetAttachedCropId();
			if (Identifiable.IsFruit(attachedCropId))
			{
				this.achievementsDirector.CheckAchievement(AchievementsDirector.Achievement.FRUIT_TREE_TYPES);
				return;
			}
			if (Identifiable.IsVeggie(attachedCropId))
			{
				this.achievementsDirector.CheckAchievement(AchievementsDirector.Achievement.VEGGIE_PATCH_TYPES);
			}
		}
	}

	// Token: 0x060010DC RID: 4316 RVA: 0x00043994 File Offset: 0x00041B94
	private void TweenScaleUpItem(GameObject toAttach, bool isReplacement)
	{
		SpawnResource[] spawners = toAttach.GetComponentsInChildren<SpawnResource>(true);
		TweenCallback <>9__0;
		foreach (ScaleMarker scaleMarker in toAttach.GetComponentsInChildren<ScaleMarker>())
		{
			if (!isReplacement || !scaleMarker.doNotScaleAsReplacement)
			{
				SpawnResource[] spawners2 = spawners;
				for (int j = 0; j < spawners2.Length; j++)
				{
					spawners2[j].RegisterSpawnBlocker();
				}
				Tweener t = TweenUtil.ScaleIn(scaleMarker.gameObject, 5f, Ease.OutQuad);
				TweenCallback action;
				if ((action = <>9__0) == null)
				{
					action = (<>9__0 = delegate()
					{
						this.TweenScaleUpItem_OnTweenComplete(spawners);
					});
				}
				t.OnComplete(action);
			}
		}
	}

	// Token: 0x060010DD RID: 4317 RVA: 0x00043A3C File Offset: 0x00041C3C
	private void TweenScaleUpItem_OnTweenComplete(SpawnResource[] spawners)
	{
		for (int i = 0; i < spawners.Length; i++)
		{
			spawners[i].DeregisterSpawnBlocker();
		}
	}

	// Token: 0x060010DE RID: 4318 RVA: 0x00043A61 File Offset: 0x00041C61
	public bool HasAttached()
	{
		return this.attached != null;
	}

	// Token: 0x060010DF RID: 4319 RVA: 0x00043A6F File Offset: 0x00041C6F
	public void DestroyAttached()
	{
		Destroyer.Destroy(this.attached, "LandPlot.DestroyAttached");
		this.attached = null;
		this.model.attachedId = SpawnResource.Id.NONE;
	}

	// Token: 0x060010E0 RID: 4320 RVA: 0x00043A94 File Offset: 0x00041C94
	public void AddUpgrade(LandPlot.Upgrade upgrade)
	{
		this.model.upgrades.Add(upgrade);
		this.achievementsDirector.CheckAchievement(AchievementsDirector.Achievement.RANCH_UPGRADED_STORAGE);
		this.achievementsDirector.AddToStat(AchievementsDirector.GameIntStat.UPGRADES_PURCHASED, 1);
		this.ApplyUpgrades(upgrade.ToEnumerable<LandPlot.Upgrade>());
	}

	// Token: 0x060010E1 RID: 4321 RVA: 0x00043ACE File Offset: 0x00041CCE
	public bool HasUpgrade(LandPlot.Upgrade upgrade)
	{
		return this.model.HasUpgrade(upgrade);
	}

	// Token: 0x060010E2 RID: 4322 RVA: 0x00043ADC File Offset: 0x00041CDC
	private void ApplyUpgrades(IEnumerable<LandPlot.Upgrade> upgrades)
	{
		foreach (PlotUpgrader plotUpgrader in base.GetComponents<PlotUpgrader>())
		{
			foreach (LandPlot.Upgrade upgrade in upgrades)
			{
				plotUpgrader.Apply(upgrade);
			}
		}
		if (this.droneNetwork != null)
		{
			this.droneNetwork.OnUpgradesChanged(this);
		}
	}

	// Token: 0x060010E3 RID: 4323 RVA: 0x00043B5C File Offset: 0x00041D5C
	public Identifiable.Id GetAttachedCropId()
	{
		if (this.attached != null)
		{
			return this.attached.GetComponent<SpawnResource>().GetPrimarySpawnId();
		}
		return Identifiable.Id.NONE;
	}

	// Token: 0x060010E4 RID: 4324 RVA: 0x00043B80 File Offset: 0x00041D80
	public double GetAttachedDeathTime()
	{
		if (this.attached != null)
		{
			DestroyAfterTime component = this.attached.GetComponent<DestroyAfterTime>();
			if (component != null)
			{
				return component.GetDeathTime();
			}
		}
		return 0.0;
	}

	// Token: 0x04000FBF RID: 4031
	public static LandPlot.IdComparer idComparer = new LandPlot.IdComparer();

	// Token: 0x04000FC0 RID: 4032
	public static LandPlot.UpgradeComparer upgradeComparer = new LandPlot.UpgradeComparer();

	// Token: 0x04000FC1 RID: 4033
	public LandPlot.Id typeId;

	// Token: 0x04000FC2 RID: 4034
	public const float attachScaleUpTime = 5f;

	// Token: 0x04000FC3 RID: 4035
	private LandPlotModel model;

	// Token: 0x04000FC4 RID: 4036
	private GameObject attached;

	// Token: 0x04000FC5 RID: 4037
	private DroneNetwork droneNetwork;

	// Token: 0x04000FC6 RID: 4038
	private AchievementsDirector achievementsDirector;

	// Token: 0x0200031B RID: 795
	public enum Id
	{
		// Token: 0x04000FC8 RID: 4040
		NONE,
		// Token: 0x04000FC9 RID: 4041
		EMPTY,
		// Token: 0x04000FCA RID: 4042
		CORRAL,
		// Token: 0x04000FCB RID: 4043
		COOP,
		// Token: 0x04000FCC RID: 4044
		GARDEN,
		// Token: 0x04000FCD RID: 4045
		SILO,
		// Token: 0x04000FCE RID: 4046
		POND,
		// Token: 0x04000FCF RID: 4047
		INCINERATOR
	}

	// Token: 0x0200031C RID: 796
	public class IdComparer : IEqualityComparer<LandPlot.Id>
	{
		// Token: 0x060010E7 RID: 4327 RVA: 0x00017781 File Offset: 0x00015981
		public bool Equals(LandPlot.Id id1, LandPlot.Id id2)
		{
			return id1 == id2;
		}

		// Token: 0x060010E8 RID: 4328 RVA: 0x00017787 File Offset: 0x00015987
		public int GetHashCode(LandPlot.Id obj)
		{
			return (int)obj;
		}
	}

	// Token: 0x0200031D RID: 797
	public enum Upgrade
	{
		// Token: 0x04000FD1 RID: 4049
		NONE,
		// Token: 0x04000FD2 RID: 4050
		WALLS,
		// Token: 0x04000FD3 RID: 4051
		MUSIC_BOX,
		// Token: 0x04000FD4 RID: 4052
		STORAGE2,
		// Token: 0x04000FD5 RID: 4053
		STORAGE3,
		// Token: 0x04000FD6 RID: 4054
		STORAGE4,
		// Token: 0x04000FD7 RID: 4055
		SOIL,
		// Token: 0x04000FD8 RID: 4056
		SPRINKLER,
		// Token: 0x04000FD9 RID: 4057
		SCARESLIME,
		// Token: 0x04000FDA RID: 4058
		FEEDER,
		// Token: 0x04000FDB RID: 4059
		VITAMIZER,
		// Token: 0x04000FDC RID: 4060
		AIR_NET,
		// Token: 0x04000FDD RID: 4061
		PLORT_COLLECTOR,
		// Token: 0x04000FDE RID: 4062
		SOLAR_SHIELD,
		// Token: 0x04000FDF RID: 4063
		ASH_TROUGH,
		// Token: 0x04000FE0 RID: 4064
		MIRACLE_MIX,
		// Token: 0x04000FE1 RID: 4065
		DELUXE_GARDEN,
		// Token: 0x04000FE2 RID: 4066
		DELUXE_COOP
	}

	// Token: 0x0200031E RID: 798
	public class UpgradeComparer : IEqualityComparer<LandPlot.Upgrade>
	{
		// Token: 0x060010EA RID: 4330 RVA: 0x00017781 File Offset: 0x00015981
		public bool Equals(LandPlot.Upgrade a, LandPlot.Upgrade b)
		{
			return a == b;
		}

		// Token: 0x060010EB RID: 4331 RVA: 0x00017787 File Offset: 0x00015987
		public int GetHashCode(LandPlot.Upgrade obj)
		{
			return (int)obj;
		}
	}
}
