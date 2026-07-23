using System;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x02000750 RID: 1872
public class ProgressDirector : MonoBehaviour, ProgressModel.Participant
{
	// Token: 0x06002715 RID: 10005 RVA: 0x0009481A File Offset: 0x00092A1A
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.mailDir = SRSingleton<SceneContext>.Instance.MailDirector;
		this.playerState = SRSingleton<SceneContext>.Instance.PlayerState;
	}

	// Token: 0x06002716 RID: 10006 RVA: 0x0009484C File Offset: 0x00092A4C
	public void InitForLevel()
	{
		SRSingleton<SceneContext>.Instance.GameModel.RegisterProgress(this);
	}

	// Token: 0x06002717 RID: 10007 RVA: 0x00094860 File Offset: 0x00092A60
	public void InitModel(ProgressModel model)
	{
		model.Reset();
		this.InitDelayedProgressTrackers();
		foreach (ProgressDirector.ProgressTrackerId key in this.delayedProgressTrackerDict.Keys)
		{
			model.delayedProgressTimeDict[key] = double.PositiveInfinity;
		}
	}

	// Token: 0x06002718 RID: 10008 RVA: 0x000948D4 File Offset: 0x00092AD4
	public void SetModel(ProgressModel model)
	{
		this.model = model;
		this.onProgressChanged = (ProgressDirector.OnProgressChanged)Delegate.Combine(this.onProgressChanged, new ProgressDirector.OnProgressChanged(this.CheckProgressUpgrades));
	}

	// Token: 0x06002719 RID: 10009 RVA: 0x00094900 File Offset: 0x00092B00
	public void GameFullyLoaded()
	{
		if (!this.model.HasProgress(ProgressDirector.ProgressType.UNLOCK_LAB_DOCKS_EXTRA) && this.model.HasProgress(ProgressDirector.ProgressType.UNLOCK_LAB) && this.model.HasProgress(ProgressDirector.ProgressType.UNLOCK_DOCKS) && double.IsPositiveInfinity(this.model.GetDelayedProgressTime(ProgressDirector.ProgressTrackerId.TIME_AFTER_LAB_DOCKS)))
		{
			this.AddProgress(ProgressDirector.ProgressType.UNLOCK_LAB_DOCKS_EXTRA);
		}
		if (this.onProgressChanged != null)
		{
			this.onProgressChanged();
		}
		this.CheckTrackers();
	}

	// Token: 0x0600271A RID: 10010 RVA: 0x00094970 File Offset: 0x00092B70
	private void InitDelayedProgressTrackers()
	{
		this.delayedProgressTrackerDict[ProgressDirector.ProgressTrackerId.TIME_AFTER_LAB_DOCKS] = new ProgressDirector.DelayedProgressTracker(this, ProgressDirector.ProgressTrackerId.TIME_AFTER_LAB_DOCKS, null, new ProgressDirector.ProgressType[]
		{
			ProgressDirector.ProgressType.UNLOCK_LAB,
			ProgressDirector.ProgressType.UNLOCK_DOCKS
		}, new int[]
		{
			1,
			1
		}, 120f, delegate(ProgressDirector.ProgressTrackerId id)
		{
			this.SetUniqueProgress(ProgressDirector.ProgressType.UNLOCK_LAB_DOCKS_EXTRA);
		});
		if (!SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().suppressStory)
		{
			this.delayedProgressTrackerDict[ProgressDirector.ProgressTrackerId.CASEY_1] = new ProgressDirector.DelayedProgressTracker(this, ProgressDirector.ProgressTrackerId.CASEY_1, null, ProgressDirector.ProgressType.SLIME_DOORS, 0, 3f, new ProgressDirector.DelayedProgressDelegate(this.SendProgressMail));
			this.delayedProgressTrackerDict[ProgressDirector.ProgressTrackerId.CASEY_2] = new ProgressDirector.DelayedProgressTracker(this, ProgressDirector.ProgressTrackerId.CASEY_2, new MailDirector.Mail(MailDirector.Type.PERSONAL, "casey_1"), ProgressDirector.ProgressType.SLIME_DOORS, 0, 48f, new ProgressDirector.DelayedProgressDelegate(this.SendProgressMail));
			this.delayedProgressTrackerDict[ProgressDirector.ProgressTrackerId.CASEY_3] = new ProgressDirector.DelayedProgressTracker(this, ProgressDirector.ProgressTrackerId.CASEY_3, new MailDirector.Mail(MailDirector.Type.PERSONAL, "casey_2"), ProgressDirector.ProgressType.SLIME_DOORS, 1, 24f, new ProgressDirector.DelayedProgressDelegate(this.SendProgressMail));
			this.delayedProgressTrackerDict[ProgressDirector.ProgressTrackerId.CASEY_4] = new ProgressDirector.DelayedProgressTracker(this, ProgressDirector.ProgressTrackerId.CASEY_4, new MailDirector.Mail(MailDirector.Type.PERSONAL, "casey_3"), ProgressDirector.ProgressType.SLIME_DOORS, 2, 24f, new ProgressDirector.DelayedProgressDelegate(this.SendProgressMail));
			this.delayedProgressTrackerDict[ProgressDirector.ProgressTrackerId.CASEY_5] = new ProgressDirector.DelayedProgressTracker(this, ProgressDirector.ProgressTrackerId.CASEY_5, new MailDirector.Mail(MailDirector.Type.PERSONAL, "casey_4"), ProgressDirector.ProgressType.SLIME_DOORS, 3, 24f, new ProgressDirector.DelayedProgressDelegate(this.SendProgressMail));
			this.delayedProgressTrackerDict[ProgressDirector.ProgressTrackerId.CASEY_6] = new ProgressDirector.DelayedProgressTracker(this, ProgressDirector.ProgressTrackerId.CASEY_6, new MailDirector.Mail(MailDirector.Type.PERSONAL, "casey_5"), ProgressDirector.ProgressType.PLORT_DOOR, 1, 24f, new ProgressDirector.DelayedProgressDelegate(this.SendProgressMail));
			this.delayedProgressTrackerDict[ProgressDirector.ProgressTrackerId.CASEY_7] = new ProgressDirector.DelayedProgressTracker(this, ProgressDirector.ProgressTrackerId.CASEY_7, new MailDirector.Mail(MailDirector.Type.PERSONAL, "casey_6"), ProgressDirector.ProgressType.SLIME_DOORS, 4, 24f, new ProgressDirector.DelayedProgressDelegate(this.SendProgressMail));
			this.delayedProgressTrackerDict[ProgressDirector.ProgressTrackerId.CASEY_8] = new ProgressDirector.DelayedProgressTracker(this, ProgressDirector.ProgressTrackerId.CASEY_8, new MailDirector.Mail(MailDirector.Type.PERSONAL, "casey_7"), ProgressDirector.ProgressType.SLIME_DOORS, 5, 24f, new ProgressDirector.DelayedProgressDelegate(this.SendProgressMail));
			this.delayedProgressTrackerDict[ProgressDirector.ProgressTrackerId.CASEY_9] = new ProgressDirector.DelayedProgressTracker(this, ProgressDirector.ProgressTrackerId.CASEY_9, new MailDirector.Mail(MailDirector.Type.PERSONAL, "casey_8"), ProgressDirector.ProgressType.UNLOCK_DESERT, 1, 24f, new ProgressDirector.DelayedProgressDelegate(this.SendProgressMail));
			this.delayedProgressTrackerDict[ProgressDirector.ProgressTrackerId.CASEY_10] = new ProgressDirector.DelayedProgressTracker(this, ProgressDirector.ProgressTrackerId.CASEY_10, new MailDirector.Mail(MailDirector.Type.PERSONAL, "casey_9"), ProgressDirector.ProgressType.SLIME_DOORS, 5, 24f, new ProgressDirector.DelayedProgressDelegate(this.SendProgressMail));
			this.delayedProgressTrackerDict[ProgressDirector.ProgressTrackerId.CASEY_11] = new ProgressDirector.DelayedProgressTracker(this, ProgressDirector.ProgressTrackerId.CASEY_11, new MailDirector.Mail(MailDirector.Type.PERSONAL, "casey_10"), ProgressDirector.ProgressType.HOBSON_END, 1, 6f, new ProgressDirector.DelayedProgressDelegate(this.SendProgressMail));
			this.delayedProgressTrackerDict[ProgressDirector.ProgressTrackerId.HOBSON_1] = new ProgressDirector.DelayedProgressTracker(this, ProgressDirector.ProgressTrackerId.HOBSON_1, new MailDirector.Mail(MailDirector.Type.PERSONAL, "casey_11"), ProgressDirector.ProgressType.HOBSON_END, 1, 0.167f, new ProgressDirector.DelayedProgressDelegate(this.SendProgressMail));
		}
	}

	// Token: 0x0600271B RID: 10011 RVA: 0x00094C59 File Offset: 0x00092E59
	public void QueueCredits()
	{
		this.readyForCredits = true;
		SRSingleton<SceneContext>.Instance.AchievementsDirector.MaybeUpdateMaxStat(AchievementsDirector.IntStat.FINISH_ADVENTURE, 1);
	}

	// Token: 0x0600271C RID: 10012 RVA: 0x00094C74 File Offset: 0x00092E74
	public void QueueRanchWistfulMusic()
	{
		this.readyForWistfulMusic = true;
	}

	// Token: 0x0600271D RID: 10013 RVA: 0x00094C80 File Offset: 0x00092E80
	public void NoteReturnedToRanch()
	{
		if (this.readyForCredits)
		{
			SRSingleton<GameContext>.Instance.UITemplates.CreateCreditsPrefab(false);
			this.readyForCredits = false;
			return;
		}
		if (this.readyForWistfulMusic)
		{
			SRSingleton<GameContext>.Instance.MusicDirector.SetWistfulRanchMode();
			this.readyForWistfulMusic = false;
		}
	}

	// Token: 0x0600271E RID: 10014 RVA: 0x00094CCC File Offset: 0x00092ECC
	public void SendProgressMail(ProgressDirector.ProgressTrackerId trackerId)
	{
		this.mailDir.SendMailIfExists(MailDirector.Type.PERSONAL, trackerId.ToString().ToLowerInvariant());
	}

	// Token: 0x0600271F RID: 10015 RVA: 0x00094CF0 File Offset: 0x00092EF0
	public void MaybeUnlockOgdenMissions()
	{
		if (SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().enableOgdenMissions && !this.HasProgress(ProgressDirector.ProgressType.UNLOCK_OGDEN_MISSIONS) && this.HasProgress(ProgressDirector.ProgressType.UNLOCK_MOSS) && this.HasProgress(ProgressDirector.ProgressType.UNLOCK_OVERGROWTH))
		{
			this.AddProgress(ProgressDirector.ProgressType.UNLOCK_OGDEN_MISSIONS);
			this.mailDir.SendMail(MailDirector.Type.PERSONAL, "ogden_invite");
		}
	}

	// Token: 0x06002720 RID: 10016 RVA: 0x00094D50 File Offset: 0x00092F50
	public void MaybeUnlockMochiMissions()
	{
		if (SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().enableMochiMissions && !this.HasProgress(ProgressDirector.ProgressType.UNLOCK_MOCHI_MISSIONS) && this.HasProgress(ProgressDirector.ProgressType.UNLOCK_QUARRY) && this.HasProgress(ProgressDirector.ProgressType.UNLOCK_GROTTO))
		{
			this.AddProgress(ProgressDirector.ProgressType.UNLOCK_MOCHI_MISSIONS);
			this.mailDir.SendMail(MailDirector.Type.PERSONAL, "mochi_invite");
		}
	}

	// Token: 0x06002721 RID: 10017 RVA: 0x00094DB0 File Offset: 0x00092FB0
	public void MaybeUnlockViktorMissions()
	{
		if (SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().enableViktorMissions && !this.HasProgress(ProgressDirector.ProgressType.UNLOCK_VIKTOR_MISSIONS) && this.HasProgress(ProgressDirector.ProgressType.UNLOCK_LAB) && this.HasProgress(ProgressDirector.ProgressType.UNLOCK_RUINS) && this.playerState.HasUpgrade(PlayerState.Upgrade.TREASURE_CRACKER_2))
		{
			this.AddProgress(ProgressDirector.ProgressType.UNLOCK_VIKTOR_MISSIONS);
			this.mailDir.SendMail(MailDirector.Type.PERSONAL, "viktor_invite");
		}
	}

	// Token: 0x06002722 RID: 10018 RVA: 0x00094E20 File Offset: 0x00093020
	public void Update()
	{
		foreach (ProgressDirector.DelayedProgressTracker delayedProgressTracker in this.delayedProgressTrackerDict.Values)
		{
			delayedProgressTracker.Update();
		}
	}

	// Token: 0x06002723 RID: 10019 RVA: 0x00094E78 File Offset: 0x00093078
	public bool SetUniqueProgress(ProgressDirector.ProgressType type)
	{
		if (!this.model.progressDict.ContainsKey(type))
		{
			this.model.progressDict[type] = 1;
			this.NoteProgressChanged(type);
			return true;
		}
		return false;
	}

	// Token: 0x06002724 RID: 10020 RVA: 0x00094EAC File Offset: 0x000930AC
	public void SetProgress(ProgressDirector.ProgressType type, int count)
	{
		if (this.model.progressDict.ContainsKey(type) && this.model.progressDict[type] == count)
		{
			return;
		}
		this.model.progressDict[type] = count;
		this.NoteProgressChanged(type);
	}

	// Token: 0x06002725 RID: 10021 RVA: 0x00094EFC File Offset: 0x000930FC
	public void AddProgress(ProgressDirector.ProgressType type)
	{
		if (this.model.progressDict.ContainsKey(type))
		{
			Dictionary<ProgressDirector.ProgressType, int> progressDict = this.model.progressDict;
			int value = progressDict[type] + 1;
			progressDict[type] = value;
		}
		else
		{
			this.model.progressDict[type] = 1;
		}
		this.NoteProgressChanged(type);
	}

	// Token: 0x06002726 RID: 10022 RVA: 0x00094F58 File Offset: 0x00093158
	private void NoteProgressChanged(ProgressDirector.ProgressType type)
	{
		if (type == ProgressDirector.ProgressType.CORPORATE_PARTNER)
		{
			int num = this.model.progressDict[type];
			this.mailDir.SendMailIfExists(MailDirector.Type.PERSONAL, "rewards_level_" + num);
			if (num >= 28)
			{
				this.mailDir.SendMailIfExists(MailDirector.Type.PERSONAL, "rewards_level_max");
			}
		}
		if (type == ProgressDirector.ProgressType.OGDEN_REWARDS)
		{
			if (this.model.progressDict[type] >= 3)
			{
				SRSingleton<SceneContext>.Instance.PediaDirector.MaybeShowPopup(PediaDirector.Id.OGDEN_RETREAT);
			}
		}
		else if (type == ProgressDirector.ProgressType.MOCHI_REWARDS)
		{
			if (this.model.progressDict[type] >= 3)
			{
				SRSingleton<SceneContext>.Instance.PediaDirector.MaybeShowPopup(PediaDirector.Id.MOCHI_MANOR);
			}
		}
		this.CheckTrackers();
		SRSingleton<SceneContext>.Instance.GadgetDirector.OnProgress(type);
		SRSingleton<SceneContext>.Instance.TutorialDirector.OnProgress(type);
		if (type == ProgressDirector.ProgressType.HOBSON_END)
		{
			SRSingleton<SceneContext>.Instance.AchievementsDirector.MaybeUpdateMaxStat(AchievementsDirector.IntStat.FIND_HOBSONS_END, 1);
		}
		if (this.onProgressChanged != null)
		{
			this.onProgressChanged();
		}
	}

	// Token: 0x06002727 RID: 10023 RVA: 0x00095074 File Offset: 0x00093274
	public void CheckTrackers()
	{
		foreach (ProgressDirector.DelayedProgressTracker delayedProgressTracker in this.delayedProgressTrackerDict.Values)
		{
			delayedProgressTracker.CheckReady();
		}
	}

	// Token: 0x06002728 RID: 10024 RVA: 0x000950CC File Offset: 0x000932CC
	public bool HasProgress(ProgressDirector.ProgressType type)
	{
		return this.model.HasProgress(type);
	}

	// Token: 0x06002729 RID: 10025 RVA: 0x000950DA File Offset: 0x000932DA
	public int GetProgress(ProgressDirector.ProgressType type)
	{
		return this.model.GetProgress(type);
	}

	// Token: 0x0600272A RID: 10026 RVA: 0x000950E8 File Offset: 0x000932E8
	public static ProgressDirector.ProgressType GetRancherProgressType(string rancherName)
	{
		return (ProgressDirector.ProgressType)Enum.Parse(typeof(ProgressDirector.ProgressType), "EXCHANGE_" + rancherName.ToUpperInvariant());
	}

	// Token: 0x0600272B RID: 10027 RVA: 0x00095110 File Offset: 0x00093310
	private void CheckProgressUpgrades()
	{
		int progress = this.GetProgress(ProgressDirector.ProgressType.CORPORATE_PARTNER);
		this.CheckVacpackUpgrades(progress);
		this.CheckBlueprintUpgrades(progress);
	}

	// Token: 0x0600272C RID: 10028 RVA: 0x00095138 File Offset: 0x00093338
	private void CheckVacpackUpgrades(int progress)
	{
		if (20 <= progress)
		{
			this.playerState.AddUpgrade(PlayerState.Upgrade.AMMO_1, false);
			this.playerState.AddUpgrade(PlayerState.Upgrade.AMMO_2, false);
			this.playerState.AddUpgrade(PlayerState.Upgrade.AMMO_3, false);
			this.playerState.AddUpgrade(PlayerState.Upgrade.AMMO_4, false);
		}
		if (21 <= progress)
		{
			this.playerState.AddUpgrade(PlayerState.Upgrade.HEALTH_1, false);
			this.playerState.AddUpgrade(PlayerState.Upgrade.HEALTH_2, false);
			this.playerState.AddUpgrade(PlayerState.Upgrade.HEALTH_3, false);
			this.playerState.AddUpgrade(PlayerState.Upgrade.HEALTH_4, false);
		}
		if (19 <= progress)
		{
			this.playerState.AddUpgrade(PlayerState.Upgrade.RUN_EFFICIENCY, false);
			this.playerState.AddUpgrade(PlayerState.Upgrade.RUN_EFFICIENCY_2, false);
		}
		if (22 <= progress)
		{
			this.playerState.AddUpgrade(PlayerState.Upgrade.GOLDEN_SURESHOT, false);
		}
	}

	// Token: 0x0600272D RID: 10029 RVA: 0x000951F0 File Offset: 0x000933F0
	private void CheckBlueprintUpgrades(int progress)
	{
		if (20 <= progress)
		{
			SRSingleton<SceneContext>.Instance.GadgetDirector.AddBlueprint(Gadget.Id.GORDO_SNARE_MASTER);
		}
		if (23 <= progress)
		{
			SRSingleton<SceneContext>.Instance.GadgetDirector.AddBlueprint(Gadget.Id.EXTRACTOR_DRILL_TITAN);
		}
		if (24 <= progress)
		{
			SRSingleton<SceneContext>.Instance.GadgetDirector.AddBlueprint(Gadget.Id.EXTRACTOR_PUMP_ABYSSAL);
		}
		if (25 <= progress)
		{
			SRSingleton<SceneContext>.Instance.GadgetDirector.AddBlueprint(Gadget.Id.EXTRACTOR_APIARY_ROYAL);
		}
		if (11 <= progress)
		{
			SRSingleton<SceneContext>.Instance.GadgetDirector.AddBlueprint(Gadget.Id.MARKET_LINK);
		}
		if (26 <= progress)
		{
			SRSingleton<SceneContext>.Instance.GadgetDirector.AddBlueprint(Gadget.Id.LAMP_GOLD);
		}
		if (27 <= progress)
		{
			SRSingleton<SceneContext>.Instance.GadgetDirector.AddBlueprint(Gadget.Id.WARP_DEPOT_GOLD);
		}
		if (28 <= progress)
		{
			SRSingleton<SceneContext>.Instance.GadgetDirector.AddBlueprint(Gadget.Id.TELEPORTER_GOLD);
		}
	}

	// Token: 0x040026B6 RID: 9910
	public ProgressDirector.OnProgressChanged onProgressChanged;

	// Token: 0x040026B7 RID: 9911
	private TimeDirector timeDir;

	// Token: 0x040026B8 RID: 9912
	private MailDirector mailDir;

	// Token: 0x040026B9 RID: 9913
	private PlayerState playerState;

	// Token: 0x040026BA RID: 9914
	private bool readyForWistfulMusic;

	// Token: 0x040026BB RID: 9915
	private bool readyForCredits;

	// Token: 0x040026BC RID: 9916
	public static ProgressDirector.ProgressTypeComparer progressTypeComparer = new ProgressDirector.ProgressTypeComparer();

	// Token: 0x040026BD RID: 9917
	private ProgressModel model;

	// Token: 0x040026BE RID: 9918
	public static ProgressDirector.ProgressTrackerIdComparer progressTrackerIdComparer = new ProgressDirector.ProgressTrackerIdComparer();

	// Token: 0x040026BF RID: 9919
	private Dictionary<ProgressDirector.ProgressTrackerId, ProgressDirector.DelayedProgressTracker> delayedProgressTrackerDict = new Dictionary<ProgressDirector.ProgressTrackerId, ProgressDirector.DelayedProgressTracker>(ProgressDirector.progressTrackerIdComparer);

	// Token: 0x040026C0 RID: 9920
	private const int CORPORATE_PARTNER_MAX = 28;

	// Token: 0x040026C1 RID: 9921
	private const int OGDEN_REWARD_EXPANSION = 3;

	// Token: 0x040026C2 RID: 9922
	private const int MOCHI_REWARD_EXPANSION = 3;

	// Token: 0x040026C3 RID: 9923
	private const int VIKTOR_REWARD_EXPANSION = 3;

	// Token: 0x040026C4 RID: 9924
	private const int RUN_EFFICIENCY_2_CORPORATE_UNLOCK_PROGRESS_AMOUNT = 19;

	// Token: 0x040026C5 RID: 9925
	private const int AMMO_4_CORPORATE_UNLOCK_PROGRESS_AMOUNT = 20;

	// Token: 0x040026C6 RID: 9926
	private const int HEALTH_4_CORPORATE_UNLOCK_PROGRESS_AMOUNT = 21;

	// Token: 0x040026C7 RID: 9927
	private const int GOLDEN_SURESHOT_CORPORATE_UNLOCK_PROGRESS_AMOUNT = 22;

	// Token: 0x040026C8 RID: 9928
	private const int MARKET_LINK_PROGRESS_AMOUNT = 11;

	// Token: 0x040026C9 RID: 9929
	private const int MASTER_GORDO_SNARE_PROGRESS_AMOUNT = 20;

	// Token: 0x040026CA RID: 9930
	private const int TITAN_DRILL_PROGRESS_AMOUNT = 23;

	// Token: 0x040026CB RID: 9931
	private const int ABYSSAL_PUMP_PROGRESS_AMOUNT = 24;

	// Token: 0x040026CC RID: 9932
	private const int ROYAL_APIARY_PROGRESS_AMOUNT = 25;

	// Token: 0x040026CD RID: 9933
	private const int GOLD_SLIME_LAMP_PROGRESS_AMOUNT = 26;

	// Token: 0x040026CE RID: 9934
	private const int GOLD_WARP_DEPOT_PROGRESS_AMOUNT = 27;

	// Token: 0x040026CF RID: 9935
	private const int GOLD_TELEPORTER_PROGRESS_AMOUNT = 28;

	// Token: 0x02000751 RID: 1873
	// (Invoke) Token: 0x06002732 RID: 10034
	public delegate void OnProgressChanged();

	// Token: 0x02000752 RID: 1874
	public enum ProgressType
	{
		// Token: 0x040026D1 RID: 9937
		NONE = -1,
		// Token: 0x040026D2 RID: 9938
		UNLOCK_QUARRY,
		// Token: 0x040026D3 RID: 9939
		UNLOCK_MOSS,
		// Token: 0x040026D4 RID: 9940
		UNLOCK_DESERT,
		// Token: 0x040026D5 RID: 9941
		UNLOCK_LAB,
		// Token: 0x040026D6 RID: 9942
		UNLOCK_RUINS,
		// Token: 0x040026D7 RID: 9943
		UNLOCK_DOCKS,
		// Token: 0x040026D8 RID: 9944
		UNLOCK_GROTTO,
		// Token: 0x040026D9 RID: 9945
		UNLOCK_OVERGROWTH,
		// Token: 0x040026DA RID: 9946
		UNLOCK_VALLEY,
		// Token: 0x040026DB RID: 9947
		UNLOCK_LAB_DOCKS_EXTRA,
		// Token: 0x040026DC RID: 9948
		UNLOCK_OGDEN_MISSIONS = 200,
		// Token: 0x040026DD RID: 9949
		UNLOCK_WILDS,
		// Token: 0x040026DE RID: 9950
		UNLOCK_MOCHI_MISSIONS,
		// Token: 0x040026DF RID: 9951
		UNLOCK_VIKTOR_MISSIONS = 300,
		// Token: 0x040026E0 RID: 9952
		UNLOCK_SLIMULATIONS,
		// Token: 0x040026E1 RID: 9953
		HOBSON_END = 990,
		// Token: 0x040026E2 RID: 9954
		HOBSON_END_UNLOCK,
		// Token: 0x040026E3 RID: 9955
		SLIME_DOORS = 1000,
		// Token: 0x040026E4 RID: 9956
		PLORT_DOOR,
		// Token: 0x040026E5 RID: 9957
		EXCHANGE_THORA = 2000,
		// Token: 0x040026E6 RID: 9958
		EXCHANGE_VIKTOR,
		// Token: 0x040026E7 RID: 9959
		EXCHANGE_OGDEN,
		// Token: 0x040026E8 RID: 9960
		EXCHANGE_MOCHI,
		// Token: 0x040026E9 RID: 9961
		EXCHANGE_BOB,
		// Token: 0x040026EA RID: 9962
		CORPORATE_PARTNER = 3000,
		// Token: 0x040026EB RID: 9963
		CORPORATE_PARTNER_UNLOCK,
		// Token: 0x040026EC RID: 9964
		OGDEN_REWARDS = 3100,
		// Token: 0x040026ED RID: 9965
		OGDEN_SEEN_FINAL_CHAT,
		// Token: 0x040026EE RID: 9966
		MOCHI_REWARDS,
		// Token: 0x040026EF RID: 9967
		MOCHI_SEEN_FINAL_CHAT,
		// Token: 0x040026F0 RID: 9968
		VIKTOR_REWARDS,
		// Token: 0x040026F1 RID: 9969
		VIKTOR_SEEN_FINAL_CHAT,
		// Token: 0x040026F2 RID: 9970
		ENTER_ZONE_OGDEN_RANCH = 3200,
		// Token: 0x040026F3 RID: 9971
		ENTER_ZONE_MOCHI_RANCH,
		// Token: 0x040026F4 RID: 9972
		ENTER_ZONE_VIKTOR_LAB,
		// Token: 0x040026F5 RID: 9973
		ENTER_ZONE_SLIMULATION
	}

	// Token: 0x02000753 RID: 1875
	public class ProgressTypeComparer : IEqualityComparer<ProgressDirector.ProgressType>
	{
		// Token: 0x06002735 RID: 10037 RVA: 0x00017781 File Offset: 0x00015981
		public bool Equals(ProgressDirector.ProgressType x, ProgressDirector.ProgressType y)
		{
			return x == y;
		}

		// Token: 0x06002736 RID: 10038 RVA: 0x00017787 File Offset: 0x00015987
		public int GetHashCode(ProgressDirector.ProgressType obj)
		{
			return (int)obj;
		}
	}

	// Token: 0x02000754 RID: 1876
	public enum ProgressTrackerId
	{
		// Token: 0x040026F7 RID: 9975
		CASEY_1,
		// Token: 0x040026F8 RID: 9976
		CASEY_2,
		// Token: 0x040026F9 RID: 9977
		CASEY_3,
		// Token: 0x040026FA RID: 9978
		CASEY_4,
		// Token: 0x040026FB RID: 9979
		CASEY_5,
		// Token: 0x040026FC RID: 9980
		CASEY_6,
		// Token: 0x040026FD RID: 9981
		CASEY_7,
		// Token: 0x040026FE RID: 9982
		CASEY_8,
		// Token: 0x040026FF RID: 9983
		CASEY_9,
		// Token: 0x04002700 RID: 9984
		CASEY_10,
		// Token: 0x04002701 RID: 9985
		CASEY_11,
		// Token: 0x04002702 RID: 9986
		HOBSON_1 = 1000,
		// Token: 0x04002703 RID: 9987
		TIME_AFTER_LAB_DOCKS = 2000
	}

	// Token: 0x02000755 RID: 1877
	public class ProgressTrackerIdComparer : IEqualityComparer<ProgressDirector.ProgressTrackerId>
	{
		// Token: 0x06002738 RID: 10040 RVA: 0x00017781 File Offset: 0x00015981
		public bool Equals(ProgressDirector.ProgressTrackerId x, ProgressDirector.ProgressTrackerId y)
		{
			return x == y;
		}

		// Token: 0x06002739 RID: 10041 RVA: 0x00017787 File Offset: 0x00015987
		public int GetHashCode(ProgressDirector.ProgressTrackerId obj)
		{
			return (int)obj;
		}
	}

	// Token: 0x02000756 RID: 1878
	// (Invoke) Token: 0x0600273C RID: 10044
	public delegate void DelayedProgressDelegate(ProgressDirector.ProgressTrackerId trackerId);

	// Token: 0x02000757 RID: 1879
	private class TimedTracker
	{
		// Token: 0x0600273F RID: 10047 RVA: 0x00003296 File Offset: 0x00001496
		public void MaybeComplete()
		{
		}

		// Token: 0x04002704 RID: 9988
		public double unlockAt = double.NaN;
	}

	// Token: 0x02000758 RID: 1880
	public class DelayedProgressTracker
	{
		// Token: 0x06002741 RID: 10049 RVA: 0x00095314 File Offset: 0x00093514
		public DelayedProgressTracker(ProgressDirector progressDir, ProgressDirector.ProgressTrackerId trackerId, MailDirector.Mail hasReadMail, ProgressDirector.ProgressType progressType, int progressCount, float delayHrs, ProgressDirector.DelayedProgressDelegate del) : this(progressDir, trackerId, hasReadMail, new ProgressDirector.ProgressType[]
		{
			progressType
		}, new int[]
		{
			progressCount
		}, delayHrs, del)
		{
		}

		// Token: 0x06002742 RID: 10050 RVA: 0x00095344 File Offset: 0x00093544
		public DelayedProgressTracker(ProgressDirector progressDir, ProgressDirector.ProgressTrackerId trackerId, MailDirector.Mail hasReadMail, ProgressDirector.ProgressType[] progressTypes, int[] progressCounts, float delayHrs, ProgressDirector.DelayedProgressDelegate del)
		{
			this.progressDir = progressDir;
			this.trackerId = trackerId;
			this.requiredReadMail = hasReadMail;
			this.progressTypes = progressTypes;
			this.progressCounts = progressCounts;
			this.delayHrs = delayHrs;
			this.onUnlockDel = del;
		}

		// Token: 0x06002743 RID: 10051 RVA: 0x00095384 File Offset: 0x00093584
		public void Update()
		{
			if (!this.alreadyUnlocked && this.progressDir.timeDir.HasReached(this.progressDir.model.GetDelayedProgressTime(this.trackerId)))
			{
				this.alreadyUnlocked = true;
				this.onUnlockDel(this.trackerId);
			}
		}

		// Token: 0x06002744 RID: 10052 RVA: 0x000953DC File Offset: 0x000935DC
		public void CheckReady()
		{
			if (double.IsPositiveInfinity(this.progressDir.model.GetDelayedProgressTime(this.trackerId)))
			{
				bool flag = this.IsProgressOk();
				bool flag2 = this.requiredReadMail == null || this.progressDir.mailDir.HasReadMail(this.requiredReadMail);
				if (flag && flag2)
				{
					this.progressDir.model.SetDelayedProgressTime(this.trackerId, this.progressDir.timeDir.HoursFromNowOrStart(this.delayHrs));
				}
			}
		}

		// Token: 0x06002745 RID: 10053 RVA: 0x0009545E File Offset: 0x0009365E
		private bool IsProgressOk()
		{
			return !this.progressTypes.Where((ProgressDirector.ProgressType t, int i) => this.progressDir.GetProgress(t) < this.progressCounts[i]).Any<ProgressDirector.ProgressType>();
		}

		// Token: 0x04002705 RID: 9989
		private ProgressDirector.ProgressTrackerId trackerId;

		// Token: 0x04002706 RID: 9990
		private MailDirector.Mail requiredReadMail;

		// Token: 0x04002707 RID: 9991
		private ProgressDirector.ProgressType[] progressTypes;

		// Token: 0x04002708 RID: 9992
		private int[] progressCounts;

		// Token: 0x04002709 RID: 9993
		private ProgressDirector.DelayedProgressDelegate onUnlockDel;

		// Token: 0x0400270A RID: 9994
		private ProgressDirector progressDir;

		// Token: 0x0400270B RID: 9995
		private float delayHrs;

		// Token: 0x0400270C RID: 9996
		private bool alreadyUnlocked;
	}
}
