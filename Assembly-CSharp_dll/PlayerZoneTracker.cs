using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.Regions;
using RichPresence;
using UnityEngine;

// Token: 0x020002D3 RID: 723
public class PlayerZoneTracker : MonoBehaviour
{
	// Token: 0x06000F63 RID: 3939 RVA: 0x0003CF6C File Offset: 0x0003B16C
	public void Awake()
	{
		this.pediaDir = SRSingleton<SceneContext>.Instance.PediaDirector;
		this.tutDir = SRSingleton<SceneContext>.Instance.TutorialDirector;
		this.achieveDir = SRSingleton<SceneContext>.Instance.AchievementsDirector;
		this.progressDir = SRSingleton<SceneContext>.Instance.ProgressDirector;
		this.playerState = SRSingleton<SceneContext>.Instance.PlayerState;
		this.musicDir = SRSingleton<GameContext>.Instance.MusicDirector;
		this.richPresenceDir = SRSingleton<GameContext>.Instance.RichPresenceDirector;
		this.member = base.GetComponent<RegionMember>();
		this.member.regionsChanged += this.OnRegionsChanged;
	}

	// Token: 0x06000F64 RID: 3940 RVA: 0x0003D00C File Offset: 0x0003B20C
	public void Start()
	{
		this.OnEntered(this.GetAnyCurrZone(ZoneDirector.Zone.RANCH));
	}

	// Token: 0x06000F65 RID: 3941 RVA: 0x0003D01B File Offset: 0x0003B21B
	public void OnDisable()
	{
		this.member.regionsChanged -= this.OnRegionsChanged;
	}

	// Token: 0x06000F66 RID: 3942 RVA: 0x0003D034 File Offset: 0x0003B234
	public void OnRegionsChanged(List<Region> leftRegions, List<Region> joinRegions)
	{
		HashSet<ZoneDirector.Zone> hashSet = ZoneDirector.Zones(this.member);
		if (this.lastZones != null)
		{
			foreach (ZoneDirector.Zone zone in hashSet)
			{
				if (!this.lastZones.Contains(zone))
				{
					this.OnEntered(zone);
				}
			}
			foreach (ZoneDirector.Zone zone2 in this.lastZones)
			{
				if (!hashSet.Contains(zone2))
				{
					this.OnExited(zone2);
				}
			}
		}
		this.lastZones = hashSet;
		this.musicDir.OnRegionsChanged(this.member);
	}

	// Token: 0x06000F67 RID: 3943 RVA: 0x0003D108 File Offset: 0x0003B308
	private void OnEntered(ZoneDirector.Zone zone)
	{
		switch (zone)
		{
		case ZoneDirector.Zone.REEF:
			this.pediaDir.MaybeShowPopup(PediaDirector.Id.REEF);
			break;
		case ZoneDirector.Zone.QUARRY:
			this.pediaDir.MaybeShowPopup(PediaDirector.Id.QUARRY);
			this.achieveDir.AddToStat(AchievementsDirector.IntStat.VISITED_QUARRY, 1);
			break;
		case ZoneDirector.Zone.MOSS:
			this.pediaDir.MaybeShowPopup(PediaDirector.Id.MOSS);
			this.achieveDir.AddToStat(AchievementsDirector.IntStat.VISITED_MOSS, 1);
			break;
		case ZoneDirector.Zone.DESERT:
			this.pediaDir.MaybeShowPopup(PediaDirector.Id.DESERT);
			this.achieveDir.AddToStat(AchievementsDirector.IntStat.VISITED_DESERT, 1);
			break;
		case ZoneDirector.Zone.RUINS:
			this.pediaDir.MaybeShowPopup(PediaDirector.Id.RUINS);
			this.achieveDir.AddToStat(AchievementsDirector.IntStat.VISITED_RUINS, 1);
			break;
		case ZoneDirector.Zone.WILDS:
			this.pediaDir.MaybeShowPopup(PediaDirector.Id.WILDS);
			break;
		case ZoneDirector.Zone.OGDEN_RANCH:
			this.progressDir.AddProgress(ProgressDirector.ProgressType.ENTER_ZONE_OGDEN_RANCH);
			this.pediaDir.MaybeShowPopup(PediaDirector.Id.OGDEN_RETREAT);
			break;
		case ZoneDirector.Zone.VALLEY:
			this.pediaDir.MaybeShowPopup(PediaDirector.Id.VALLEY);
			this.tutDir.RemoveHidden(TutorialDirector.Id.RACE_START);
			this.tutDir.MaybeShowPopup(TutorialDirector.Id.RACE_START);
			break;
		case ZoneDirector.Zone.MOCHI_RANCH:
			this.progressDir.AddProgress(ProgressDirector.ProgressType.ENTER_ZONE_MOCHI_RANCH);
			this.pediaDir.MaybeShowPopup(PediaDirector.Id.MOCHI_MANOR);
			break;
		case ZoneDirector.Zone.SLIMULATIONS:
			this.pediaDir.MaybeShowPopup(PediaDirector.Id.SLIMULATIONS_WORLD);
			this.tutDir.MaybeShowPopup(TutorialDirector.Id.SLIMULATIONS_START_1);
			this.tutDir.MaybeShowPopup(TutorialDirector.Id.SLIMULATIONS_START_2);
			break;
		case ZoneDirector.Zone.VIKTOR_LAB:
			this.progressDir.AddProgress(ProgressDirector.ProgressType.ENTER_ZONE_VIKTOR_LAB);
			this.pediaDir.MaybeShowPopup(PediaDirector.Id.VIKTOR_LAB);
			break;
		}
		this.playerState.OnEnteredZone(zone);
		this.richPresenceDir.SetRichPresence(new InZoneData(zone));
		AnalyticsUtil.CustomEvent("ZoneEntered", new Dictionary<string, object>
		{
			{
				"ZoneId",
				zone
			}
		}, true);
	}

	// Token: 0x06000F68 RID: 3944 RVA: 0x0003D30A File Offset: 0x0003B50A
	private void OnExited(ZoneDirector.Zone zone)
	{
		this.playerState.OnExitedZone(zone);
	}

	// Token: 0x06000F69 RID: 3945 RVA: 0x0003D318 File Offset: 0x0003B518
	private ZoneDirector.Zone GetAnyCurrZone(ZoneDirector.Zone defaultToZone)
	{
		return Randoms.SHARED.Pick<ZoneDirector.Zone>(ZoneDirector.Zones(this.member), defaultToZone);
	}

	// Token: 0x06000F6A RID: 3946 RVA: 0x0003D330 File Offset: 0x0003B530
	public ZoneDirector.Zone GetCurrentZone()
	{
		if (ZoneDirector.Zones(this.member).Count == 0)
		{
			return ZoneDirector.Zone.NONE;
		}
		ZoneDirector.Zone result = ZoneDirector.Zone.NONE;
		int num = int.MaxValue;
		foreach (ZoneDirector.Zone zone in ZoneDirector.Zones(this.member))
		{
			if (zone < (ZoneDirector.Zone)num)
			{
				result = zone;
				num = (int)zone;
			}
		}
		return result;
	}

	// Token: 0x04000E44 RID: 3652
	private RegionMember member;

	// Token: 0x04000E45 RID: 3653
	private HashSet<ZoneDirector.Zone> lastZones;

	// Token: 0x04000E46 RID: 3654
	private PediaDirector pediaDir;

	// Token: 0x04000E47 RID: 3655
	private TutorialDirector tutDir;

	// Token: 0x04000E48 RID: 3656
	private AchievementsDirector achieveDir;

	// Token: 0x04000E49 RID: 3657
	private MusicDirector musicDir;

	// Token: 0x04000E4A RID: 3658
	private ProgressDirector progressDir;

	// Token: 0x04000E4B RID: 3659
	private PlayerState playerState;

	// Token: 0x04000E4C RID: 3660
	private Director richPresenceDir;
}
