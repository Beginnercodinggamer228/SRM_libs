using System;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x02000270 RID: 624
public class AchievementsDirector : MonoBehaviour, GameAchievesModel.Participant, ProfileAchievesModel.Participant
{
	// Token: 0x06000D11 RID: 3345 RVA: 0x0003596B File Offset: 0x00033B6B
	public void Awake()
	{
		this.availableAchievementCountDivisor = 1f / (float)Enum.GetNames(typeof(AchievementsDirector.Achievement)).Length;
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.gameModel = SRSingleton<SceneContext>.Instance.GameModel;
	}

	// Token: 0x06000D12 RID: 3346 RVA: 0x000359AB File Offset: 0x00033BAB
	public void InitForLevel()
	{
		this.gameModel.RegisterProfileAchievements(this);
		this.gameModel.RegisterGameAchievements(this);
	}

	// Token: 0x06000D13 RID: 3347 RVA: 0x000359C5 File Offset: 0x00033BC5
	public void InitModel(GameAchievesModel gameAchievesModel)
	{
		gameAchievesModel.Reset();
	}

	// Token: 0x06000D14 RID: 3348 RVA: 0x000359CD File Offset: 0x00033BCD
	public void SetModel(GameAchievesModel gameAchievesModel)
	{
		this.gameAchievesModel = gameAchievesModel;
		if (this.gameAchievesModel != null && this.profileAchievesModel != null)
		{
			this.InitTrackers();
		}
	}

	// Token: 0x06000D15 RID: 3349 RVA: 0x000359EC File Offset: 0x00033BEC
	public void InitModel(ProfileAchievesModel profileAchievesModel)
	{
		profileAchievesModel.Reset();
	}

	// Token: 0x06000D16 RID: 3350 RVA: 0x000359F4 File Offset: 0x00033BF4
	public void SetModel(ProfileAchievesModel profileAchievesModel)
	{
		this.profileAchievesModel = profileAchievesModel;
		if (this.gameAchievesModel != null && this.profileAchievesModel != null)
		{
			this.InitTrackers();
		}
	}

	// Token: 0x06000D17 RID: 3351 RVA: 0x00035A13 File Offset: 0x00033C13
	public static void SyncAchievements(ProfileAchievesModel profileAchievesModel)
	{
		Log.Debug("Syncing achievements", new object[]
		{
			"count",
			profileAchievesModel.earnedAchievements.Count
		});
	}

	// Token: 0x06000D18 RID: 3352 RVA: 0x00035A40 File Offset: 0x00033C40
	public void ResetProfile()
	{
		this.profileAchievesModel.Reset();
		this.InitTrackers();
	}

	// Token: 0x06000D19 RID: 3353 RVA: 0x00035A54 File Offset: 0x00033C54
	public void Update()
	{
		foreach (AchievementsDirector.Updatable updatable in this.updatables)
		{
			updatable.Update();
		}
	}

	// Token: 0x06000D1A RID: 3354 RVA: 0x00035AA4 File Offset: 0x00033CA4
	public void LateUpdate()
	{
		if (this.postUpdateAchievementChecks.Any<AchievementsDirector.Achievement>() && !Levels.isSpecialNonAlloc())
		{
			foreach (AchievementsDirector.Achievement achievement in this.postUpdateAchievementChecks)
			{
				if (this.HasAchievement(achievement) || this.trackers[achievement].Reached())
				{
					this.AwardAchievement(achievement);
				}
			}
			this.postUpdateAchievementChecks.Clear();
		}
	}

	// Token: 0x06000D1B RID: 3355 RVA: 0x00035B34 File Offset: 0x00033D34
	private float CalculateGameProgress(int earnedAchievements)
	{
		return (float)earnedAchievements * this.availableAchievementCountDivisor;
	}

	// Token: 0x06000D1C RID: 3356 RVA: 0x00035B3F File Offset: 0x00033D3F
	private void RegisterUpdatable(AchievementsDirector.Updatable updatable)
	{
		this.updatables.Add(updatable);
	}

	// Token: 0x06000D1D RID: 3357 RVA: 0x00035B50 File Offset: 0x00033D50
	private bool AllowStatUpdate<T>(Dictionary<PlayerState.GameMode, HashSet<T>> gameModeStats, T statId)
	{
		HashSet<T> hashSet;
		return !gameModeStats.TryGetValue(SRSingleton<SceneContext>.Instance.GameModel.currGameMode, out hashSet) || hashSet.Contains(statId);
	}

	// Token: 0x06000D1E RID: 3358 RVA: 0x00035B80 File Offset: 0x00033D80
	public void AddToStat(AchievementsDirector.IntStat stat, int amount)
	{
		if (this.AllowStatUpdate<AchievementsDirector.IntStat>(this.GAME_MODE_INT_STATS, stat))
		{
			if (!this.profileAchievesModel.intStatDict.ContainsKey(stat))
			{
				this.profileAchievesModel.intStatDict[stat] = amount;
			}
			else
			{
				Dictionary<AchievementsDirector.IntStat, int> intStatDict = this.profileAchievesModel.intStatDict;
				intStatDict[stat] += amount;
			}
			this.CheckAchievements(stat);
		}
	}

	// Token: 0x06000D1F RID: 3359 RVA: 0x00035BE8 File Offset: 0x00033DE8
	public void ResetStat(AchievementsDirector.IntStat stat)
	{
		if (this.AllowStatUpdate<AchievementsDirector.IntStat>(this.GAME_MODE_INT_STATS, stat))
		{
			this.profileAchievesModel.intStatDict[stat] = 0;
		}
	}

	// Token: 0x06000D20 RID: 3360 RVA: 0x00035C0C File Offset: 0x00033E0C
	public void MaybeUpdateMaxStat(AchievementsDirector.IntStat stat, int val)
	{
		if (this.AllowStatUpdate<AchievementsDirector.IntStat>(this.GAME_MODE_INT_STATS, stat))
		{
			if (!this.profileAchievesModel.intStatDict.ContainsKey(stat))
			{
				this.profileAchievesModel.intStatDict[stat] = val;
			}
			else if (val > this.profileAchievesModel.intStatDict[stat])
			{
				this.profileAchievesModel.intStatDict[stat] = val;
			}
			this.CheckAchievements(stat);
		}
	}

	// Token: 0x06000D21 RID: 3361 RVA: 0x00035C7C File Offset: 0x00033E7C
	public int? GetStat(AchievementsDirector.IntStat stat)
	{
		int value;
		if (this.profileAchievesModel.intStatDict.TryGetValue(stat, out value))
		{
			return new int?(value);
		}
		return null;
	}

	// Token: 0x06000D22 RID: 3362 RVA: 0x00035CB0 File Offset: 0x00033EB0
	public void AddToStat(AchievementsDirector.EnumStat stat, Enum val)
	{
		if (this.AllowStatUpdate<AchievementsDirector.EnumStat>(this.GAME_MODE_ENUM_STATS, stat))
		{
			HashSet<Enum> hashSet;
			if (this.profileAchievesModel.enumStatDict.ContainsKey(stat))
			{
				hashSet = this.profileAchievesModel.enumStatDict[stat];
			}
			else
			{
				hashSet = new HashSet<Enum>();
				this.profileAchievesModel.enumStatDict[stat] = hashSet;
			}
			hashSet.Add(val);
			this.CheckAchievements(stat);
		}
	}

	// Token: 0x06000D23 RID: 3363 RVA: 0x00035D1A File Offset: 0x00033F1A
	public void SetStat(AchievementsDirector.BoolStat stat)
	{
		if (this.AllowStatUpdate<AchievementsDirector.BoolStat>(this.GAME_MODE_BOOL_STATS, stat))
		{
			this.profileAchievesModel.boolStatDict[stat] = true;
			this.CheckAchievements(stat);
		}
	}

	// Token: 0x06000D24 RID: 3364 RVA: 0x00035D44 File Offset: 0x00033F44
	public void SetStat(AchievementsDirector.GameFloatStat stat, float val)
	{
		this.gameAchievesModel.gameFloatStatDict[stat] = val;
		this.CheckAchievements(stat);
	}

	// Token: 0x06000D25 RID: 3365 RVA: 0x00035D5F File Offset: 0x00033F5F
	public void SetStat(AchievementsDirector.GameDoubleStat stat, double val)
	{
		this.gameAchievesModel.gameDoubleStatDict[stat] = val;
		this.CheckAchievements(stat);
	}

	// Token: 0x06000D26 RID: 3366 RVA: 0x00035D7C File Offset: 0x00033F7C
	public void AddToStat(AchievementsDirector.GameIntStat stat, int amt)
	{
		int num = this.gameAchievesModel.gameIntStatDict.ContainsKey(stat) ? this.gameAchievesModel.gameIntStatDict[stat] : 0;
		this.gameAchievesModel.gameIntStatDict[stat] = num + amt;
		SRSingleton<SceneContext>.Instance.PlayerState.OnGameIntStatChanged(stat, this.gameAchievesModel.gameIntStatDict[stat]);
		this.CheckAchievements(stat);
	}

	// Token: 0x06000D27 RID: 3367 RVA: 0x00035DED File Offset: 0x00033FED
	public int GetGameIntStat(AchievementsDirector.GameIntStat stat)
	{
		return this.gameAchievesModel.gameIntStatDict.Get(stat);
	}

	// Token: 0x06000D28 RID: 3368 RVA: 0x00035E00 File Offset: 0x00034000
	public void AddToStat(AchievementsDirector.GameIdDictStat stat, Identifiable.Id id, int amt)
	{
		Dictionary<Identifiable.Id, int> dictionary;
		if (this.gameAchievesModel.gameIdDictStatDict.ContainsKey(stat))
		{
			dictionary = this.gameAchievesModel.gameIdDictStatDict[stat];
		}
		else
		{
			dictionary = new Dictionary<Identifiable.Id, int>(Identifiable.idComparer);
			this.gameAchievesModel.gameIdDictStatDict[stat] = dictionary;
		}
		int num = dictionary.ContainsKey(id) ? dictionary[id] : 0;
		dictionary[id] = num + amt;
		this.CheckAchievements(stat);
	}

	// Token: 0x06000D29 RID: 3369 RVA: 0x00035E78 File Offset: 0x00034078
	public Dictionary<Identifiable.Id, int> GetGameIdDictStat(AchievementsDirector.GameIdDictStat stat)
	{
		Dictionary<Identifiable.Id, int> dictionary = this.gameAchievesModel.gameIdDictStatDict.Get(stat);
		if (dictionary == null)
		{
			return new Dictionary<Identifiable.Id, int>(Identifiable.idComparer);
		}
		return new Dictionary<Identifiable.Id, int>(dictionary, Identifiable.idComparer);
	}

	// Token: 0x06000D2A RID: 3370 RVA: 0x00035EB0 File Offset: 0x000340B0
	private void InitTrackers()
	{
		this.updatables.Clear();
		this.trackers[AchievementsDirector.Achievement.SELL_PLORTS_A] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.SELL_PLORTS_A, AchievementsDirector.IntStat.PLORTS_SOLD, 100);
		this.trackers[AchievementsDirector.Achievement.SELL_PLORTS_B] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.SELL_PLORTS_B, AchievementsDirector.IntStat.PLORTS_SOLD, 500);
		this.trackers[AchievementsDirector.Achievement.SELL_PLORTS_C] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.SELL_PLORTS_C, AchievementsDirector.IntStat.PLORTS_SOLD, 1000);
		this.trackers[AchievementsDirector.Achievement.SELL_PLORTS_D] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.SELL_PLORTS_D, AchievementsDirector.IntStat.PLORTS_SOLD, 2500);
		this.trackers[AchievementsDirector.Achievement.SELL_PLORTS_E] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.SELL_PLORTS_E, AchievementsDirector.IntStat.PLORTS_SOLD, 5000);
		this.trackers[AchievementsDirector.Achievement.DAY_CURRENCY] = new AchievementsDirector.DailyCountTracker(this, AchievementsDirector.Achievement.DAY_CURRENCY, AchievementsDirector.IntStat.DAY_CURRENCY, 5000);
		this.trackers[AchievementsDirector.Achievement.EARN_CURRENCY_A] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.EARN_CURRENCY_A, AchievementsDirector.IntStat.CURRENCY, 5000);
		this.trackers[AchievementsDirector.Achievement.EARN_CURRENCY_B] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.EARN_CURRENCY_B, AchievementsDirector.IntStat.CURRENCY, 25000);
		this.trackers[AchievementsDirector.Achievement.EARN_CURRENCY_C] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.EARN_CURRENCY_C, AchievementsDirector.IntStat.CURRENCY, 100000);
		this.trackers[AchievementsDirector.Achievement.FEED_SLIMES_CHICKENS] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.FEED_SLIMES_CHICKENS, AchievementsDirector.IntStat.CHICKENS_FED_SLIMES, 100);
		this.trackers[AchievementsDirector.Achievement.PINK_SLIMES_FOOD_TYPES] = new AchievementsDirector.CountEnumsTracker(this, AchievementsDirector.Achievement.PINK_SLIMES_FOOD_TYPES, AchievementsDirector.EnumStat.PINK_SLIMES_FOOD_TYPES, 10);
		this.trackers[AchievementsDirector.Achievement.FEED_AIRBORNE] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.FEED_AIRBORNE, AchievementsDirector.IntStat.FED_AIRBORNE, 1);
		this.trackers[AchievementsDirector.Achievement.FEED_FAVORITES] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.FEED_FAVORITES, AchievementsDirector.IntStat.FED_FAVORITE, 50);
		this.trackers[AchievementsDirector.Achievement.AWAY_FROM_RANCH] = new AchievementsDirector.SimpleTracker(this, AchievementsDirector.Achievement.AWAY_FROM_RANCH, () => this.gameAchievesModel.gameDoubleStatDict.ContainsKey(AchievementsDirector.GameDoubleStat.LAST_LEFT_RANCH) && this.gameAchievesModel.gameDoubleStatDict.ContainsKey(AchievementsDirector.GameDoubleStat.LAST_ENTERED_RANCH) && this.gameAchievesModel.gameDoubleStatDict[AchievementsDirector.GameDoubleStat.LAST_ENTERED_RANCH] - this.gameAchievesModel.gameDoubleStatDict[AchievementsDirector.GameDoubleStat.LAST_LEFT_RANCH] >= 86400.0, new AchievementsDirector.GameDoubleStat[]
		{
			AchievementsDirector.GameDoubleStat.LAST_LEFT_RANCH,
			AchievementsDirector.GameDoubleStat.LAST_ENTERED_RANCH
		});
		this.trackers[AchievementsDirector.Achievement.AWAKE_UNTIL_MORNING] = new AchievementsDirector.UpdatableSimpleTracker(this, AchievementsDirector.Achievement.AWAKE_UNTIL_MORNING, () => this.gameAchievesModel.gameDoubleStatDict.ContainsKey(AchievementsDirector.GameDoubleStat.LAST_SLEPT) && this.gameAchievesModel.gameDoubleStatDict.ContainsKey(AchievementsDirector.GameDoubleStat.LAST_AWOKE) && this.gameAchievesModel.gameDoubleStatDict[AchievementsDirector.GameDoubleStat.LAST_AWOKE] > this.gameAchievesModel.gameDoubleStatDict[AchievementsDirector.GameDoubleStat.LAST_SLEPT] && this.gameAchievesModel.gameDoubleStatDict[AchievementsDirector.GameDoubleStat.LAST_AWOKE] < this.timeDir.GetHourAfter(-2, 6f) + 3600.0, delegate()
		{
			if (this.timeDir.OnPassedHour(6f))
			{
				this.CheckAchievement(AchievementsDirector.Achievement.AWAKE_UNTIL_MORNING);
			}
		}, new AchievementsDirector.GameDoubleStat[]
		{
			AchievementsDirector.GameDoubleStat.LAST_SLEPT,
			AchievementsDirector.GameDoubleStat.LAST_AWOKE
		});
		this.trackers[AchievementsDirector.Achievement.KNOCKOUT_MORNING] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.KNOCKOUT_MORNING, AchievementsDirector.IntStat.DEATH_BEFORE_10AM, 1);
		this.trackers[AchievementsDirector.Achievement.FRUIT_TREE_TYPES] = new AchievementsDirector.SimpleTracker(this, AchievementsDirector.Achievement.FRUIT_TREE_TYPES, () => SRSingleton<SceneContext>.Instance.GameModel.GetRanchResourceTypes(Identifiable.FRUIT_CLASS).Count >= 3, Array.Empty<AchievementsDirector.GameDoubleStat>());
		this.trackers[AchievementsDirector.Achievement.VEGGIE_PATCH_TYPES] = new AchievementsDirector.SimpleTracker(this, AchievementsDirector.Achievement.VEGGIE_PATCH_TYPES, () => SRSingleton<SceneContext>.Instance.GameModel.GetRanchResourceTypes(Identifiable.VEGGIE_CLASS).Count >= 3, Array.Empty<AchievementsDirector.GameDoubleStat>());
		this.trackers[AchievementsDirector.Achievement.DISCOVERED_QUARRY] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.DISCOVERED_QUARRY, AchievementsDirector.IntStat.VISITED_QUARRY, 1);
		this.trackers[AchievementsDirector.Achievement.DISCOVERED_MOSS] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.DISCOVERED_MOSS, AchievementsDirector.IntStat.VISITED_MOSS, 1);
		this.trackers[AchievementsDirector.Achievement.DISCOVERED_DESERT] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.DISCOVERED_DESERT, AchievementsDirector.IntStat.VISITED_DESERT, 1);
		this.trackers[AchievementsDirector.Achievement.DISCOVERED_RUINS] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.DISCOVERED_RUINS, AchievementsDirector.IntStat.VISITED_RUINS, 1);
		this.trackers[AchievementsDirector.Achievement.BURST_GORDO] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.BURST_GORDO, AchievementsDirector.IntStat.BURST_GORDOS, 1);
		this.trackers[AchievementsDirector.Achievement.OPEN_SLIME_GATE] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.OPEN_SLIME_GATE, AchievementsDirector.IntStat.OPENED_SLIME_GATES, 1);
		this.trackers[AchievementsDirector.Achievement.INCINERATE_ELDER_CHICKEN] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.INCINERATE_ELDER_CHICKEN, AchievementsDirector.IntStat.INCINERATED_ELDER_CHICKENS, 1);
		this.trackers[AchievementsDirector.Achievement.INCINERATE_CHICK] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.INCINERATE_CHICK, AchievementsDirector.IntStat.INCINERATED_CHICKS, 1);
		this.trackers[AchievementsDirector.Achievement.FILLED_SILO] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.FILLED_SILO, AchievementsDirector.IntStat.FILLED_SILO, 1);
		this.trackers[AchievementsDirector.Achievement.RANCH_UPGRADED_STORAGE] = new AchievementsDirector.SimpleTracker(this, AchievementsDirector.Achievement.RANCH_UPGRADED_STORAGE, () => this.gameModel.IncludesFullyUpgradedCorralCoopAndSilo(), Array.Empty<AchievementsDirector.GameDoubleStat>());
		this.trackers[AchievementsDirector.Achievement.FULFILL_EXCHANGE_EARLY] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.FULFILL_EXCHANGE_EARLY, AchievementsDirector.IntStat.FULFILL_EXCHANGE_EARLY, 1);
		this.trackers[AchievementsDirector.Achievement.DAY_COLLECT_PLORTS] = new AchievementsDirector.DailyCountTracker(this, AchievementsDirector.Achievement.DAY_COLLECT_PLORTS, AchievementsDirector.IntStat.DAY_COLLECT_PLORTS, 50);
		this.trackers[AchievementsDirector.Achievement.GOLD_SLIME_TRIPLE_PLORT] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.GOLD_SLIME_TRIPLE_PLORT, AchievementsDirector.IntStat.GOLD_SLIME_TRIPLE_PLORT, 1);
		this.trackers[AchievementsDirector.Achievement.EXTENDED_RAD_EXPOSURE] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.EXTENDED_RAD_EXPOSURE, AchievementsDirector.IntStat.EXTENDED_RAD_EXPOSURE, 15);
		this.trackers[AchievementsDirector.Achievement.EXTENDED_TARR_HOLD] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.EXTENDED_TARR_HOLD, AchievementsDirector.IntStat.EXTENDED_TARR_HOLD, 15);
		this.trackers[AchievementsDirector.Achievement.TABBY_HEADBUTT] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.TABBY_HEADBUTT, AchievementsDirector.IntStat.TABBY_HEADBUTT, 1);
		this.trackers[AchievementsDirector.Achievement.LAUNCHED_BOOM_EXPLODE] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.LAUNCHED_BOOM_EXPLODE, AchievementsDirector.IntStat.LAUNCHED_BOOM_EXPLODE, 1);
		this.trackers[AchievementsDirector.Achievement.MANY_SLIMES_IN_VAC] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.MANY_SLIMES_IN_VAC, AchievementsDirector.IntStat.SLIMES_IN_VAC, 15);
		this.trackers[AchievementsDirector.Achievement.CORRAL_SLIME_TYPES] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.CORRAL_SLIME_TYPES, AchievementsDirector.IntStat.CORRAL_SLIME_TYPES, 6);
		this.trackers[AchievementsDirector.Achievement.CORRAL_LARGO_TYPES] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.CORRAL_LARGO_TYPES, AchievementsDirector.IntStat.CORRAL_LARGO_TYPES, 3);
		this.trackers[AchievementsDirector.Achievement.POND_SLIME_TYPES] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.POND_SLIME_TYPES, AchievementsDirector.IntStat.POND_SLIME_TYPES, 5);
		this.trackers[AchievementsDirector.Achievement.RANCH_LARGO_TYPES] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.RANCH_LARGO_TYPES, AchievementsDirector.IntStat.RANCH_LARGO_TYPES, 10);
		this.trackers[AchievementsDirector.Achievement.ENTERED_CORRAL_SLIMES] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.ENTERED_CORRAL_SLIMES, AchievementsDirector.IntStat.ENTERED_CORRAL_SLIMES, 40);
		this.trackers[AchievementsDirector.Achievement.TIME_LIMIT_CURRENCY_A] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.TIME_LIMIT_CURRENCY_A, AchievementsDirector.IntStat.TIME_LIMIT_V2_CURRENCY, 10000);
		this.trackers[AchievementsDirector.Achievement.TIME_LIMIT_CURRENCY_B] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.TIME_LIMIT_CURRENCY_B, AchievementsDirector.IntStat.TIME_LIMIT_V2_CURRENCY, 35000);
		this.trackers[AchievementsDirector.Achievement.TIME_LIMIT_CURRENCY_C] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.TIME_LIMIT_CURRENCY_C, AchievementsDirector.IntStat.TIME_LIMIT_V2_CURRENCY, 75000);
		this.trackers[AchievementsDirector.Achievement.FABRICATE_GADGETS_A] = new AchievementsDirector.GameCountTracker(this, AchievementsDirector.Achievement.FABRICATE_GADGETS_A, AchievementsDirector.GameIntStat.FABRICATED_GADGETS, 1);
		this.trackers[AchievementsDirector.Achievement.FABRICATE_GADGETS_B] = new AchievementsDirector.GameCountTracker(this, AchievementsDirector.Achievement.FABRICATE_GADGETS_B, AchievementsDirector.GameIntStat.FABRICATED_GADGETS, 35);
		this.trackers[AchievementsDirector.Achievement.FABRICATE_GADGETS_C] = new AchievementsDirector.GameCountTracker(this, AchievementsDirector.Achievement.FABRICATE_GADGETS_C, AchievementsDirector.GameIntStat.FABRICATED_GADGETS, 100);
		this.trackers[AchievementsDirector.Achievement.SLIMEBALL_SCORE] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.SLIMEBALL_SCORE, AchievementsDirector.IntStat.SLIMEBALL_SCORE, 50);
		this.trackers[AchievementsDirector.Achievement.SLIME_STAGE_TARR] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.SLIME_STAGE_TARR, AchievementsDirector.IntStat.SLIME_STAGE_TARRS, 1);
		this.trackers[AchievementsDirector.Achievement.JOIN_REWARDS_CLUB] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.JOIN_REWARDS_CLUB, AchievementsDirector.IntStat.REWARD_LEVELS, 1);
		this.trackers[AchievementsDirector.Achievement.USE_CHROMAS] = new AchievementsDirector.CountEnumsTracker(this, AchievementsDirector.Achievement.USE_CHROMAS, AchievementsDirector.EnumStat.USE_CHROMAS, 3);
		this.trackers[AchievementsDirector.Achievement.COLLECT_SLIME_TOYS] = new AchievementsDirector.CountEnumsTracker(this, AchievementsDirector.Achievement.COLLECT_SLIME_TOYS, AchievementsDirector.EnumStat.SLIME_TOYS_BOUGHT, 10);
		this.trackers[AchievementsDirector.Achievement.SNARE_HUNTER_GORDO] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.SNARE_HUNTER_GORDO, AchievementsDirector.IntStat.SNARED_HUNTER_GORDOS, 1);
		this.trackers[AchievementsDirector.Achievement.ACTIVATE_OASIS] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.ACTIVATE_OASIS, AchievementsDirector.IntStat.ACTIVATED_OASES, 1);
		this.trackers[AchievementsDirector.Achievement.COMPLETE_SLIMEPEDIA] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.COMPLETE_SLIMEPEDIA, AchievementsDirector.IntStat.COMPLETED_SLIMEPEDIA, 1);
		this.trackers[AchievementsDirector.Achievement.FIND_HOBSONS_END] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.FIND_HOBSONS_END, AchievementsDirector.IntStat.FIND_HOBSONS_END, 1);
		this.trackers[AchievementsDirector.Achievement.FINISH_ADVENTURE] = new AchievementsDirector.CountTracker(this, AchievementsDirector.Achievement.FINISH_ADVENTURE, AchievementsDirector.IntStat.FINISH_ADVENTURE, 1);
	}

	// Token: 0x06000D2B RID: 3371 RVA: 0x000364B9 File Offset: 0x000346B9
	public void CheckAchievement(AchievementsDirector.Achievement achievement)
	{
		this.CheckAchievements(Enumerable.Repeat<AchievementsDirector.Achievement>(achievement, 1));
	}

	// Token: 0x06000D2C RID: 3372 RVA: 0x000364C8 File Offset: 0x000346C8
	private void CheckAchievements(IEnumerable<AchievementsDirector.Achievement> achievements)
	{
		this.postUpdateAchievementChecks.UnionWith(achievements);
	}

	// Token: 0x06000D2D RID: 3373 RVA: 0x000364D8 File Offset: 0x000346D8
	private void CheckAchievements(AchievementsDirector.BoolStat stat)
	{
		this.CheckAchievements(from p in this.trackers
		where p.Value.IsTracking(stat)
		select p.Key);
	}

	// Token: 0x06000D2E RID: 3374 RVA: 0x00036534 File Offset: 0x00034734
	private void CheckAchievements(AchievementsDirector.IntStat stat)
	{
		this.CheckAchievements(from p in this.trackers
		where p.Value.IsTracking(stat)
		select p.Key);
	}

	// Token: 0x06000D2F RID: 3375 RVA: 0x00036590 File Offset: 0x00034790
	private void CheckAchievements(AchievementsDirector.EnumStat stat)
	{
		this.CheckAchievements(from p in this.trackers
		where p.Value.IsTracking(stat)
		select p.Key);
	}

	// Token: 0x06000D30 RID: 3376 RVA: 0x000365EC File Offset: 0x000347EC
	private void CheckAchievements(AchievementsDirector.GameFloatStat stat)
	{
		this.CheckAchievements(from p in this.trackers
		where p.Value.IsTracking(stat)
		select p.Key);
	}

	// Token: 0x06000D31 RID: 3377 RVA: 0x00036648 File Offset: 0x00034848
	private void CheckAchievements(AchievementsDirector.GameDoubleStat stat)
	{
		this.CheckAchievements(from p in this.trackers
		where p.Value.IsTracking(stat)
		select p.Key);
	}

	// Token: 0x06000D32 RID: 3378 RVA: 0x000366A4 File Offset: 0x000348A4
	private void CheckAchievements(AchievementsDirector.GameIntStat stat)
	{
		this.CheckAchievements(from p in this.trackers
		where p.Value.IsTracking(stat)
		select p.Key);
	}

	// Token: 0x06000D33 RID: 3379 RVA: 0x00036700 File Offset: 0x00034900
	private void CheckAchievements(AchievementsDirector.GameIdDictStat stat)
	{
		this.CheckAchievements(from p in this.trackers
		where p.Value.IsTracking(stat)
		select p.Key);
	}

	// Token: 0x06000D34 RID: 3380 RVA: 0x0003675C File Offset: 0x0003495C
	private bool AwardAchievement(AchievementsDirector.Achievement achievement)
	{
		HashSet<AchievementsDirector.Achievement> hashSet;
		if (this.GAME_MODE_ACHIEVEMENTS.TryGetValue(SRSingleton<SceneContext>.Instance.GameModel.currGameMode, out hashSet) && !hashSet.Contains(achievement))
		{
			return false;
		}
		bool flag = this.profileAchievesModel.earnedAchievements.Add(achievement);
		if (flag)
		{
			this.MaybeShowPopup(achievement);
			AnalyticsUtil.CustomEvent("Achievement", new Dictionary<string, object>
			{
				{
					"id",
					achievement.ToString()
				}
			}, true);
		}
		return flag;
	}

	// Token: 0x06000D35 RID: 3381 RVA: 0x000367D5 File Offset: 0x000349D5
	private void MaybeShowPopup(AchievementsDirector.Achievement achievement)
	{
		this.popupQueue.Enqueue(achievement);
		this.MaybePopupNext();
	}

	// Token: 0x06000D36 RID: 3382 RVA: 0x000367E9 File Offset: 0x000349E9
	public void RegisterSuppressor()
	{
		this.suppressors++;
	}

	// Token: 0x06000D37 RID: 3383 RVA: 0x000367F9 File Offset: 0x000349F9
	public void UnregisterSuppressor()
	{
		this.suppressors--;
		if (this.suppressors <= 0)
		{
			this.MaybePopupNext();
		}
	}

	// Token: 0x06000D38 RID: 3384 RVA: 0x00036818 File Offset: 0x00034A18
	private void MaybePopupNext()
	{
		if (this.popupQueue.Count > 0 && this.currPopup == null && this.suppressors <= 0)
		{
			AchievementsDirector.Achievement idEntry = this.popupQueue.Dequeue();
			UnityEngine.Object.Instantiate<GameObject>(this.achievementAwardUIPrefab).GetComponent<AchievementAwardUI>().Init(idEntry);
		}
	}

	// Token: 0x06000D39 RID: 3385 RVA: 0x0003686C File Offset: 0x00034A6C
	public void OnApplicationQuit()
	{
		this.quitting = true;
	}

	// Token: 0x06000D3A RID: 3386 RVA: 0x00036875 File Offset: 0x00034A75
	public void PopupActivated(AchievementAwardUI popup)
	{
		if (this.currPopup != null)
		{
			Log.Warning("Popup arrived with already-active popup.", Array.Empty<object>());
		}
		this.currPopup = popup;
	}

	// Token: 0x06000D3B RID: 3387 RVA: 0x0003689C File Offset: 0x00034A9C
	public void PopupDeactivated(AchievementAwardUI popup)
	{
		if (this.currPopup == popup && !this.quitting)
		{
			this.currPopup = null;
			this.timeDir.OnUnpause(new TimeDirector.OnUnpauseDelegate(this.OnUnpause));
			return;
		}
		Log.Warning("Popup deactivated, but wasn't current popup.", Array.Empty<object>());
	}

	// Token: 0x06000D3C RID: 3388 RVA: 0x000368ED File Offset: 0x00034AED
	public void OnDestroy()
	{
		this.timeDir.ClearOnUnpause(new TimeDirector.OnUnpauseDelegate(this.OnUnpause));
	}

	// Token: 0x06000D3D RID: 3389 RVA: 0x00036906 File Offset: 0x00034B06
	public void OnUnpause()
	{
		this.MaybePopupNext();
	}

	// Token: 0x06000D3E RID: 3390 RVA: 0x0003690E File Offset: 0x00034B0E
	public bool HasAchievement(AchievementsDirector.Achievement achievement)
	{
		return this.profileAchievesModel.earnedAchievements.Contains(achievement);
	}

	// Token: 0x06000D3F RID: 3391 RVA: 0x00036921 File Offset: 0x00034B21
	public void GetProgress(AchievementsDirector.Achievement achievement, out int progress, out int outOf)
	{
		if (!this.trackers.ContainsKey(achievement))
		{
			progress = 0;
			outOf = 1;
			return;
		}
		this.trackers[achievement].GetProgress(out progress, out outOf);
	}

	// Token: 0x06000D40 RID: 3392 RVA: 0x0003694B File Offset: 0x00034B4B
	public void GetOverallProgress(out int progress, out int outOf)
	{
		progress = this.profileAchievesModel.earnedAchievements.Count;
		outOf = Enum.GetValues(typeof(AchievementsDirector.Achievement)).Length;
	}

	// Token: 0x06000D41 RID: 3393 RVA: 0x00036978 File Offset: 0x00034B78
	public Sprite GetAchievementImage(string achievementKey, AchievementsDirector.Achievement achieve)
	{
		Sprite sprite = Resources.Load("Achievements/" + achievementKey, typeof(Sprite)) as Sprite;
		if (!(sprite == null))
		{
			return sprite;
		}
		if (this.TIER_1.Contains(achieve))
		{
			return this.tier1DefaultIcon;
		}
		if (this.TIER_2.Contains(achieve))
		{
			return this.tier2DefaultIcon;
		}
		if (this.TIER_3.Contains(achieve))
		{
			return this.tier3DefaultIcon;
		}
		return this.tier1DefaultIcon;
	}

	// Token: 0x06000D42 RID: 3394 RVA: 0x000369F8 File Offset: 0x00034BF8
	protected bool HasMissingTieredAchieves()
	{
		foreach (object obj in Enum.GetValues(typeof(AchievementsDirector.Achievement)))
		{
			AchievementsDirector.Achievement achievement = (AchievementsDirector.Achievement)obj;
			if (!this.TIER_1.Contains(achievement) && !this.TIER_2.Contains(achievement) && !this.TIER_3.Contains(achievement))
			{
				Log.Error("Missing achieve tier: " + achievement, Array.Empty<object>());
				return true;
			}
		}
		return false;
	}

	// Token: 0x04000C41 RID: 3137
	public GameObject achievementAwardUIPrefab;

	// Token: 0x04000C42 RID: 3138
	public GameObject achievementsPanelPrefab;

	// Token: 0x04000C43 RID: 3139
	public Sprite tier1DefaultIcon;

	// Token: 0x04000C44 RID: 3140
	public Sprite tier2DefaultIcon;

	// Token: 0x04000C45 RID: 3141
	public Sprite tier3DefaultIcon;

	// Token: 0x04000C46 RID: 3142
	private float availableAchievementCountDivisor = 1f;

	// Token: 0x04000C47 RID: 3143
	public HashSet<AchievementsDirector.Achievement> TIER_1 = new HashSet<AchievementsDirector.Achievement>
	{
		AchievementsDirector.Achievement.SELL_PLORTS_A,
		AchievementsDirector.Achievement.SELL_PLORTS_B,
		AchievementsDirector.Achievement.FEED_SLIMES_CHICKENS,
		AchievementsDirector.Achievement.FRUIT_TREE_TYPES,
		AchievementsDirector.Achievement.VEGGIE_PATCH_TYPES,
		AchievementsDirector.Achievement.EARN_CURRENCY_A,
		AchievementsDirector.Achievement.AWAKE_UNTIL_MORNING,
		AchievementsDirector.Achievement.KNOCKOUT_MORNING,
		AchievementsDirector.Achievement.AWAY_FROM_RANCH,
		AchievementsDirector.Achievement.FEED_AIRBORNE,
		AchievementsDirector.Achievement.PINK_SLIMES_FOOD_TYPES,
		AchievementsDirector.Achievement.TABBY_HEADBUTT,
		AchievementsDirector.Achievement.INCINERATE_ELDER_CHICKEN,
		AchievementsDirector.Achievement.FEED_FAVORITES,
		AchievementsDirector.Achievement.FILLED_SILO,
		AchievementsDirector.Achievement.INCINERATE_CHICK,
		AchievementsDirector.Achievement.TIME_LIMIT_CURRENCY_A,
		AchievementsDirector.Achievement.FABRICATE_GADGETS_A,
		AchievementsDirector.Achievement.SLIME_STAGE_TARR,
		AchievementsDirector.Achievement.JOIN_REWARDS_CLUB,
		AchievementsDirector.Achievement.USE_CHROMAS
	};

	// Token: 0x04000C48 RID: 3144
	public HashSet<AchievementsDirector.Achievement> TIER_2 = new HashSet<AchievementsDirector.Achievement>
	{
		AchievementsDirector.Achievement.SELL_PLORTS_C,
		AchievementsDirector.Achievement.SELL_PLORTS_D,
		AchievementsDirector.Achievement.EARN_CURRENCY_B,
		AchievementsDirector.Achievement.LAUNCHED_BOOM_EXPLODE,
		AchievementsDirector.Achievement.MANY_SLIMES_IN_VAC,
		AchievementsDirector.Achievement.DISCOVERED_QUARRY,
		AchievementsDirector.Achievement.DISCOVERED_MOSS,
		AchievementsDirector.Achievement.DISCOVERED_RUINS,
		AchievementsDirector.Achievement.OPEN_SLIME_GATE,
		AchievementsDirector.Achievement.BURST_GORDO,
		AchievementsDirector.Achievement.EXTENDED_RAD_EXPOSURE,
		AchievementsDirector.Achievement.DAY_COLLECT_PLORTS,
		AchievementsDirector.Achievement.FULFILL_EXCHANGE_EARLY,
		AchievementsDirector.Achievement.RANCH_UPGRADED_STORAGE,
		AchievementsDirector.Achievement.ENTERED_CORRAL_SLIMES,
		AchievementsDirector.Achievement.POND_SLIME_TYPES,
		AchievementsDirector.Achievement.CORRAL_SLIME_TYPES,
		AchievementsDirector.Achievement.CORRAL_LARGO_TYPES,
		AchievementsDirector.Achievement.TIME_LIMIT_CURRENCY_B,
		AchievementsDirector.Achievement.FABRICATE_GADGETS_B,
		AchievementsDirector.Achievement.COLLECT_SLIME_TOYS,
		AchievementsDirector.Achievement.SNARE_HUNTER_GORDO,
		AchievementsDirector.Achievement.ACTIVATE_OASIS
	};

	// Token: 0x04000C49 RID: 3145
	public HashSet<AchievementsDirector.Achievement> TIER_3 = new HashSet<AchievementsDirector.Achievement>
	{
		AchievementsDirector.Achievement.SELL_PLORTS_E,
		AchievementsDirector.Achievement.EARN_CURRENCY_C,
		AchievementsDirector.Achievement.DAY_CURRENCY,
		AchievementsDirector.Achievement.DISCOVERED_DESERT,
		AchievementsDirector.Achievement.EXTENDED_TARR_HOLD,
		AchievementsDirector.Achievement.GOLD_SLIME_TRIPLE_PLORT,
		AchievementsDirector.Achievement.RANCH_LARGO_TYPES,
		AchievementsDirector.Achievement.TIME_LIMIT_CURRENCY_C,
		AchievementsDirector.Achievement.FABRICATE_GADGETS_C,
		AchievementsDirector.Achievement.SLIMEBALL_SCORE,
		AchievementsDirector.Achievement.FIND_HOBSONS_END,
		AchievementsDirector.Achievement.FINISH_ADVENTURE,
		AchievementsDirector.Achievement.COMPLETE_SLIMEPEDIA
	};

	// Token: 0x04000C4A RID: 3146
	private Dictionary<PlayerState.GameMode, HashSet<AchievementsDirector.Achievement>> GAME_MODE_ACHIEVEMENTS = new Dictionary<PlayerState.GameMode, HashSet<AchievementsDirector.Achievement>>(PlayerState.GameModeComparer.Instance)
	{
		{
			PlayerState.GameMode.TIME_LIMIT_V2,
			new HashSet<AchievementsDirector.Achievement>(AchievementsDirector.AchievementComparer.Instance)
			{
				AchievementsDirector.Achievement.TIME_LIMIT_CURRENCY_A,
				AchievementsDirector.Achievement.TIME_LIMIT_CURRENCY_B,
				AchievementsDirector.Achievement.TIME_LIMIT_CURRENCY_C
			}
		}
	};

	// Token: 0x04000C4B RID: 3147
	private Dictionary<PlayerState.GameMode, HashSet<AchievementsDirector.IntStat>> GAME_MODE_INT_STATS = new Dictionary<PlayerState.GameMode, HashSet<AchievementsDirector.IntStat>>(PlayerState.GameModeComparer.Instance)
	{
		{
			PlayerState.GameMode.TIME_LIMIT_V2,
			new HashSet<AchievementsDirector.IntStat>
			{
				AchievementsDirector.IntStat.TIME_LIMIT_V2_CURRENCY
			}
		}
	};

	// Token: 0x04000C4C RID: 3148
	private Dictionary<PlayerState.GameMode, HashSet<AchievementsDirector.BoolStat>> GAME_MODE_BOOL_STATS = new Dictionary<PlayerState.GameMode, HashSet<AchievementsDirector.BoolStat>>(PlayerState.GameModeComparer.Instance)
	{
		{
			PlayerState.GameMode.TIME_LIMIT_V2,
			new HashSet<AchievementsDirector.BoolStat>()
		}
	};

	// Token: 0x04000C4D RID: 3149
	private Dictionary<PlayerState.GameMode, HashSet<AchievementsDirector.EnumStat>> GAME_MODE_ENUM_STATS = new Dictionary<PlayerState.GameMode, HashSet<AchievementsDirector.EnumStat>>(PlayerState.GameModeComparer.Instance)
	{
		{
			PlayerState.GameMode.TIME_LIMIT_V2,
			new HashSet<AchievementsDirector.EnumStat>()
		}
	};

	// Token: 0x04000C4E RID: 3150
	private Dictionary<AchievementsDirector.Achievement, AchievementsDirector.Tracker> trackers = new Dictionary<AchievementsDirector.Achievement, AchievementsDirector.Tracker>();

	// Token: 0x04000C4F RID: 3151
	private TimeDirector timeDir;

	// Token: 0x04000C50 RID: 3152
	private GameModel gameModel;

	// Token: 0x04000C51 RID: 3153
	private AchievementAwardUI currPopup;

	// Token: 0x04000C52 RID: 3154
	private bool quitting;

	// Token: 0x04000C53 RID: 3155
	private int suppressors;

	// Token: 0x04000C54 RID: 3156
	private Queue<AchievementsDirector.Achievement> popupQueue = new Queue<AchievementsDirector.Achievement>();

	// Token: 0x04000C55 RID: 3157
	private List<AchievementsDirector.Updatable> updatables = new List<AchievementsDirector.Updatable>();

	// Token: 0x04000C56 RID: 3158
	private HashSet<AchievementsDirector.Achievement> postUpdateAchievementChecks = new HashSet<AchievementsDirector.Achievement>();

	// Token: 0x04000C57 RID: 3159
	private ProfileAchievesModel profileAchievesModel;

	// Token: 0x04000C58 RID: 3160
	private GameAchievesModel gameAchievesModel;

	// Token: 0x02000271 RID: 625
	public enum Achievement
	{
		// Token: 0x04000C5A RID: 3162
		SELL_PLORTS_A,
		// Token: 0x04000C5B RID: 3163
		SELL_PLORTS_B,
		// Token: 0x04000C5C RID: 3164
		SELL_PLORTS_C,
		// Token: 0x04000C5D RID: 3165
		SELL_PLORTS_D,
		// Token: 0x04000C5E RID: 3166
		SELL_PLORTS_E,
		// Token: 0x04000C5F RID: 3167
		FEED_SLIMES_CHICKENS,
		// Token: 0x04000C60 RID: 3168
		FRUIT_TREE_TYPES,
		// Token: 0x04000C61 RID: 3169
		VEGGIE_PATCH_TYPES,
		// Token: 0x04000C62 RID: 3170
		EARN_CURRENCY_A,
		// Token: 0x04000C63 RID: 3171
		EARN_CURRENCY_B,
		// Token: 0x04000C64 RID: 3172
		EARN_CURRENCY_C,
		// Token: 0x04000C65 RID: 3173
		DAY_CURRENCY,
		// Token: 0x04000C66 RID: 3174
		AWAKE_UNTIL_MORNING,
		// Token: 0x04000C67 RID: 3175
		KNOCKOUT_MORNING,
		// Token: 0x04000C68 RID: 3176
		AWAY_FROM_RANCH,
		// Token: 0x04000C69 RID: 3177
		PINK_SLIMES_FOOD_TYPES,
		// Token: 0x04000C6A RID: 3178
		FEED_AIRBORNE,
		// Token: 0x04000C6B RID: 3179
		DISCOVERED_QUARRY,
		// Token: 0x04000C6C RID: 3180
		DISCOVERED_MOSS,
		// Token: 0x04000C6D RID: 3181
		DISCOVERED_DESERT,
		// Token: 0x04000C6E RID: 3182
		BURST_GORDO,
		// Token: 0x04000C6F RID: 3183
		OPEN_SLIME_GATE,
		// Token: 0x04000C70 RID: 3184
		INCINERATE_ELDER_CHICKEN,
		// Token: 0x04000C71 RID: 3185
		FEED_FAVORITES,
		// Token: 0x04000C72 RID: 3186
		FILLED_SILO,
		// Token: 0x04000C73 RID: 3187
		RANCH_UPGRADED_STORAGE,
		// Token: 0x04000C74 RID: 3188
		FULFILL_EXCHANGE_EARLY,
		// Token: 0x04000C75 RID: 3189
		DAY_COLLECT_PLORTS,
		// Token: 0x04000C76 RID: 3190
		GOLD_SLIME_TRIPLE_PLORT,
		// Token: 0x04000C77 RID: 3191
		EXTENDED_RAD_EXPOSURE,
		// Token: 0x04000C78 RID: 3192
		EXTENDED_TARR_HOLD,
		// Token: 0x04000C79 RID: 3193
		TABBY_HEADBUTT,
		// Token: 0x04000C7A RID: 3194
		LAUNCHED_BOOM_EXPLODE,
		// Token: 0x04000C7B RID: 3195
		MANY_SLIMES_IN_VAC,
		// Token: 0x04000C7C RID: 3196
		CORRAL_SLIME_TYPES,
		// Token: 0x04000C7D RID: 3197
		CORRAL_LARGO_TYPES,
		// Token: 0x04000C7E RID: 3198
		POND_SLIME_TYPES,
		// Token: 0x04000C7F RID: 3199
		RANCH_LARGO_TYPES,
		// Token: 0x04000C80 RID: 3200
		ENTERED_CORRAL_SLIMES,
		// Token: 0x04000C81 RID: 3201
		INCINERATE_CHICK,
		// Token: 0x04000C82 RID: 3202
		TIME_LIMIT_CURRENCY_A,
		// Token: 0x04000C83 RID: 3203
		TIME_LIMIT_CURRENCY_B,
		// Token: 0x04000C84 RID: 3204
		TIME_LIMIT_CURRENCY_C,
		// Token: 0x04000C85 RID: 3205
		DISCOVERED_RUINS,
		// Token: 0x04000C86 RID: 3206
		FABRICATE_GADGETS_A,
		// Token: 0x04000C87 RID: 3207
		FABRICATE_GADGETS_B,
		// Token: 0x04000C88 RID: 3208
		FABRICATE_GADGETS_C,
		// Token: 0x04000C89 RID: 3209
		SLIME_STAGE_TARR,
		// Token: 0x04000C8A RID: 3210
		SLIMEBALL_SCORE,
		// Token: 0x04000C8B RID: 3211
		JOIN_REWARDS_CLUB,
		// Token: 0x04000C8C RID: 3212
		USE_CHROMAS,
		// Token: 0x04000C8D RID: 3213
		COLLECT_SLIME_TOYS,
		// Token: 0x04000C8E RID: 3214
		FIND_HOBSONS_END,
		// Token: 0x04000C8F RID: 3215
		SNARE_HUNTER_GORDO,
		// Token: 0x04000C90 RID: 3216
		ACTIVATE_OASIS,
		// Token: 0x04000C91 RID: 3217
		FINISH_ADVENTURE,
		// Token: 0x04000C92 RID: 3218
		COMPLETE_SLIMEPEDIA
	}

	// Token: 0x02000272 RID: 626
	public class AchievementComparer : IEqualityComparer<AchievementsDirector.Achievement>
	{
		// Token: 0x06000D48 RID: 3400 RVA: 0x00017781 File Offset: 0x00015981
		public bool Equals(AchievementsDirector.Achievement a, AchievementsDirector.Achievement b)
		{
			return a == b;
		}

		// Token: 0x06000D49 RID: 3401 RVA: 0x00017787 File Offset: 0x00015987
		public int GetHashCode(AchievementsDirector.Achievement a)
		{
			return (int)a;
		}

		// Token: 0x04000C93 RID: 3219
		public static AchievementsDirector.AchievementComparer Instance = new AchievementsDirector.AchievementComparer();
	}

	// Token: 0x02000273 RID: 627
	public enum BoolStat
	{

	}

	// Token: 0x02000274 RID: 628
	public enum IntStat
	{
		// Token: 0x04000C96 RID: 3222
		PLORTS_SOLD,
		// Token: 0x04000C97 RID: 3223
		CHICKENS_FED_SLIMES,
		// Token: 0x04000C98 RID: 3224
		DAY_CURRENCY,
		// Token: 0x04000C99 RID: 3225
		CURRENCY,
		// Token: 0x04000C9A RID: 3226
		DEATH_BEFORE_10AM,
		// Token: 0x04000C9B RID: 3227
		FED_AIRBORNE,
		// Token: 0x04000C9C RID: 3228
		VISITED_QUARRY,
		// Token: 0x04000C9D RID: 3229
		VISITED_MOSS,
		// Token: 0x04000C9E RID: 3230
		VISITED_DESERT,
		// Token: 0x04000C9F RID: 3231
		BURST_GORDOS,
		// Token: 0x04000CA0 RID: 3232
		OPENED_SLIME_GATES,
		// Token: 0x04000CA1 RID: 3233
		INCINERATED_ELDER_CHICKENS,
		// Token: 0x04000CA2 RID: 3234
		FED_FAVORITE,
		// Token: 0x04000CA3 RID: 3235
		FILLED_SILO,
		// Token: 0x04000CA4 RID: 3236
		FULFILL_EXCHANGE_EARLY,
		// Token: 0x04000CA5 RID: 3237
		DAY_COLLECT_PLORTS,
		// Token: 0x04000CA6 RID: 3238
		GOLD_SLIME_TRIPLE_PLORT,
		// Token: 0x04000CA7 RID: 3239
		EXTENDED_RAD_EXPOSURE,
		// Token: 0x04000CA8 RID: 3240
		EXTENDED_TARR_HOLD,
		// Token: 0x04000CA9 RID: 3241
		TABBY_HEADBUTT,
		// Token: 0x04000CAA RID: 3242
		LAUNCHED_BOOM_EXPLODE,
		// Token: 0x04000CAB RID: 3243
		SLIMES_IN_VAC,
		// Token: 0x04000CAC RID: 3244
		CORRAL_SLIME_TYPES,
		// Token: 0x04000CAD RID: 3245
		CORRAL_LARGO_TYPES,
		// Token: 0x04000CAE RID: 3246
		POND_SLIME_TYPES,
		// Token: 0x04000CAF RID: 3247
		RANCH_LARGO_TYPES,
		// Token: 0x04000CB0 RID: 3248
		ENTERED_CORRAL_SLIMES,
		// Token: 0x04000CB1 RID: 3249
		INCINERATED_CHICKS,
		// Token: 0x04000CB2 RID: 3250
		[Obsolete("use TIME_LIMIT_V2_CURRENCY", true)]
		TIME_LIMIT_CURRENCY,
		// Token: 0x04000CB3 RID: 3251
		SLIMEBALL_SCORE,
		// Token: 0x04000CB4 RID: 3252
		SLIME_STAGE_TARRS,
		// Token: 0x04000CB5 RID: 3253
		VISITED_RUINS,
		// Token: 0x04000CB6 RID: 3254
		REWARD_LEVELS,
		// Token: 0x04000CB7 RID: 3255
		SNARED_HUNTER_GORDOS,
		// Token: 0x04000CB8 RID: 3256
		ACTIVATED_OASES,
		// Token: 0x04000CB9 RID: 3257
		COMPLETED_SLIMEPEDIA,
		// Token: 0x04000CBA RID: 3258
		FIND_HOBSONS_END,
		// Token: 0x04000CBB RID: 3259
		FINISH_ADVENTURE,
		// Token: 0x04000CBC RID: 3260
		TIME_LIMIT_V2_CURRENCY
	}

	// Token: 0x02000275 RID: 629
	public enum EnumStat
	{
		// Token: 0x04000CBE RID: 3262
		PINK_SLIMES_FOOD_TYPES,
		// Token: 0x04000CBF RID: 3263
		RANCH_FRUIT_TYPES,
		// Token: 0x04000CC0 RID: 3264
		RANCH_VEGGIE_TYPES,
		// Token: 0x04000CC1 RID: 3265
		SLIME_TOYS_BOUGHT,
		// Token: 0x04000CC2 RID: 3266
		USE_CHROMAS
	}

	// Token: 0x02000276 RID: 630
	public enum GameFloatStat
	{

	}

	// Token: 0x02000277 RID: 631
	public enum GameDoubleStat
	{
		// Token: 0x04000CC5 RID: 3269
		LAST_LEFT_RANCH,
		// Token: 0x04000CC6 RID: 3270
		LAST_ENTERED_RANCH,
		// Token: 0x04000CC7 RID: 3271
		LAST_SLEPT,
		// Token: 0x04000CC8 RID: 3272
		LAST_AWOKE
	}

	// Token: 0x02000278 RID: 632
	public enum GameIntStat
	{
		// Token: 0x04000CCA RID: 3274
		DEATHS,
		// Token: 0x04000CCB RID: 3275
		UPGRADES_PURCHASED,
		// Token: 0x04000CCC RID: 3276
		CURRENCY_SPENT,
		// Token: 0x04000CCD RID: 3277
		FABRICATED_GADGETS
	}

	// Token: 0x02000279 RID: 633
	public enum GameIdDictStat
	{
		// Token: 0x04000CCF RID: 3279
		PLORT_TYPES_SOLD
	}

	// Token: 0x0200027A RID: 634
	public interface Updatable
	{
		// Token: 0x06000D4C RID: 3404
		void Update();
	}

	// Token: 0x0200027B RID: 635
	public abstract class Tracker
	{
		// Token: 0x17000175 RID: 373
		// (get) Token: 0x06000D4D RID: 3405 RVA: 0x00036EC1 File Offset: 0x000350C1
		// (set) Token: 0x06000D4E RID: 3406 RVA: 0x00036EC9 File Offset: 0x000350C9
		public AchievementsDirector dir { get; private set; }

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x06000D4F RID: 3407 RVA: 0x00036ED2 File Offset: 0x000350D2
		// (set) Token: 0x06000D50 RID: 3408 RVA: 0x00036EDA File Offset: 0x000350DA
		public AchievementsDirector.Achievement achievement { get; private set; }

		// Token: 0x06000D51 RID: 3409 RVA: 0x00036EE3 File Offset: 0x000350E3
		public Tracker(AchievementsDirector dir, AchievementsDirector.Achievement achievement)
		{
			this.dir = dir;
			this.achievement = achievement;
		}

		// Token: 0x06000D52 RID: 3410
		public abstract bool Reached();

		// Token: 0x06000D53 RID: 3411
		public abstract void GetProgress(out int progress, out int outOf);

		// Token: 0x06000D54 RID: 3412 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
		public virtual bool IsTracking(AchievementsDirector.BoolStat stat)
		{
			return false;
		}

		// Token: 0x06000D55 RID: 3413 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
		public virtual bool IsTracking(AchievementsDirector.IntStat stat)
		{
			return false;
		}

		// Token: 0x06000D56 RID: 3414 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
		public virtual bool IsTracking(AchievementsDirector.EnumStat stat)
		{
			return false;
		}

		// Token: 0x06000D57 RID: 3415 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
		public virtual bool IsTracking(AchievementsDirector.GameFloatStat stat)
		{
			return false;
		}

		// Token: 0x06000D58 RID: 3416 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
		public virtual bool IsTracking(AchievementsDirector.GameDoubleStat stat)
		{
			return false;
		}

		// Token: 0x06000D59 RID: 3417 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
		public virtual bool IsTracking(AchievementsDirector.GameIntStat stat)
		{
			return false;
		}

		// Token: 0x06000D5A RID: 3418 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
		public virtual bool IsTracking(AchievementsDirector.GameIdDictStat stat)
		{
			return false;
		}
	}

	// Token: 0x0200027C RID: 636
	public class BoolTracker : AchievementsDirector.Tracker
	{
		// Token: 0x06000D5B RID: 3419 RVA: 0x00036EF9 File Offset: 0x000350F9
		public BoolTracker(AchievementsDirector dir, AchievementsDirector.Achievement achievement, AchievementsDirector.BoolStat stat) : base(dir, achievement)
		{
			this.stat = stat;
		}

		// Token: 0x06000D5C RID: 3420 RVA: 0x00036F0A File Offset: 0x0003510A
		public override bool Reached()
		{
			return base.dir.profileAchievesModel.boolStatDict.ContainsKey(this.stat) && base.dir.profileAchievesModel.boolStatDict[this.stat];
		}

		// Token: 0x06000D5D RID: 3421 RVA: 0x00036F48 File Offset: 0x00035148
		public override void GetProgress(out int progress, out int outOf)
		{
			progress = ((base.dir.profileAchievesModel.boolStatDict.ContainsKey(this.stat) && base.dir.profileAchievesModel.boolStatDict[this.stat]) ? 1 : 0);
			outOf = 1;
		}

		// Token: 0x06000D5E RID: 3422 RVA: 0x00036F98 File Offset: 0x00035198
		public override bool IsTracking(AchievementsDirector.BoolStat stat)
		{
			return this.stat == stat;
		}

		// Token: 0x04000CD2 RID: 3282
		protected AchievementsDirector.BoolStat stat;
	}

	// Token: 0x0200027D RID: 637
	public class CountTracker : AchievementsDirector.Tracker
	{
		// Token: 0x06000D5F RID: 3423 RVA: 0x00036FA3 File Offset: 0x000351A3
		public CountTracker(AchievementsDirector dir, AchievementsDirector.Achievement achievement, AchievementsDirector.IntStat stat, int count) : base(dir, achievement)
		{
			this.count = count;
			this.stat = stat;
		}

		// Token: 0x06000D60 RID: 3424 RVA: 0x00036FBC File Offset: 0x000351BC
		public override bool Reached()
		{
			return base.dir.profileAchievesModel.intStatDict.ContainsKey(this.stat) && base.dir.profileAchievesModel.intStatDict[this.stat] >= this.count;
		}

		// Token: 0x06000D61 RID: 3425 RVA: 0x00037010 File Offset: 0x00035210
		public override void GetProgress(out int progress, out int outOf)
		{
			progress = (base.dir.profileAchievesModel.intStatDict.ContainsKey(this.stat) ? Math.Min(this.count, base.dir.profileAchievesModel.intStatDict[this.stat]) : 0);
			outOf = this.count;
		}

		// Token: 0x06000D62 RID: 3426 RVA: 0x0003706D File Offset: 0x0003526D
		public override bool IsTracking(AchievementsDirector.IntStat stat)
		{
			return this.stat == stat;
		}

		// Token: 0x04000CD3 RID: 3283
		protected int count;

		// Token: 0x04000CD4 RID: 3284
		protected AchievementsDirector.IntStat stat;
	}

	// Token: 0x0200027E RID: 638
	public class GameCountTracker : AchievementsDirector.Tracker
	{
		// Token: 0x06000D63 RID: 3427 RVA: 0x00037078 File Offset: 0x00035278
		public GameCountTracker(AchievementsDirector dir, AchievementsDirector.Achievement achievement, AchievementsDirector.GameIntStat stat, int count) : base(dir, achievement)
		{
			this.count = count;
			this.stat = stat;
		}

		// Token: 0x06000D64 RID: 3428 RVA: 0x00037094 File Offset: 0x00035294
		public override bool Reached()
		{
			return base.dir.gameAchievesModel.gameIntStatDict.ContainsKey(this.stat) && base.dir.gameAchievesModel.gameIntStatDict[this.stat] >= this.count;
		}

		// Token: 0x06000D65 RID: 3429 RVA: 0x000370E8 File Offset: 0x000352E8
		public override void GetProgress(out int progress, out int outOf)
		{
			progress = (base.dir.gameAchievesModel.gameIntStatDict.ContainsKey(this.stat) ? Math.Min(this.count, base.dir.gameAchievesModel.gameIntStatDict[this.stat]) : 0);
			outOf = this.count;
		}

		// Token: 0x06000D66 RID: 3430 RVA: 0x00037145 File Offset: 0x00035345
		public override bool IsTracking(AchievementsDirector.GameIntStat stat)
		{
			return this.stat == stat;
		}

		// Token: 0x04000CD5 RID: 3285
		protected int count;

		// Token: 0x04000CD6 RID: 3286
		protected AchievementsDirector.GameIntStat stat;
	}

	// Token: 0x0200027F RID: 639
	public class DailyCountTracker : AchievementsDirector.CountTracker, AchievementsDirector.Updatable
	{
		// Token: 0x06000D67 RID: 3431 RVA: 0x00037150 File Offset: 0x00035350
		public DailyCountTracker(AchievementsDirector dir, AchievementsDirector.Achievement achievement, AchievementsDirector.IntStat stat, int count) : base(dir, achievement, stat, count)
		{
			dir.RegisterUpdatable(this);
		}

		// Token: 0x06000D68 RID: 3432 RVA: 0x00037164 File Offset: 0x00035364
		public void Update()
		{
			int num = base.dir.timeDir.CurrDay();
			if (num > this.lastDay)
			{
				base.dir.ResetStat(this.stat);
			}
			this.lastDay = num;
		}

		// Token: 0x04000CD7 RID: 3287
		private int lastDay;
	}

	// Token: 0x02000280 RID: 640
	public class CountEnumsTracker : AchievementsDirector.Tracker
	{
		// Token: 0x06000D69 RID: 3433 RVA: 0x000371A3 File Offset: 0x000353A3
		public CountEnumsTracker(AchievementsDirector dir, AchievementsDirector.Achievement achievement, AchievementsDirector.EnumStat stat, int count) : base(dir, achievement)
		{
			this.count = count;
			this.stat = stat;
		}

		// Token: 0x06000D6A RID: 3434 RVA: 0x000371BC File Offset: 0x000353BC
		public override bool Reached()
		{
			return base.dir.profileAchievesModel.enumStatDict.ContainsKey(this.stat) && base.dir.profileAchievesModel.enumStatDict[this.stat].Count >= this.count;
		}

		// Token: 0x06000D6B RID: 3435 RVA: 0x00037214 File Offset: 0x00035414
		public override void GetProgress(out int progress, out int outOf)
		{
			progress = (base.dir.profileAchievesModel.enumStatDict.ContainsKey(this.stat) ? Math.Min(this.count, base.dir.profileAchievesModel.enumStatDict[this.stat].Count) : 0);
			outOf = this.count;
		}

		// Token: 0x06000D6C RID: 3436 RVA: 0x00037276 File Offset: 0x00035476
		public override bool IsTracking(AchievementsDirector.EnumStat stat)
		{
			return this.stat == stat;
		}

		// Token: 0x04000CD8 RID: 3288
		protected int count;

		// Token: 0x04000CD9 RID: 3289
		protected AchievementsDirector.EnumStat stat;
	}

	// Token: 0x02000281 RID: 641
	public class SimpleTracker : AchievementsDirector.Tracker
	{
		// Token: 0x06000D6D RID: 3437 RVA: 0x00037281 File Offset: 0x00035481
		public SimpleTracker(AchievementsDirector dir, AchievementsDirector.Achievement achievement, AchievementsDirector.SimpleTracker.ReachedDelegate reachedDel, params AchievementsDirector.GameDoubleStat[] stats) : base(dir, achievement)
		{
			this.reachedDel = reachedDel;
			this.stats = stats;
		}

		// Token: 0x06000D6E RID: 3438 RVA: 0x0003729A File Offset: 0x0003549A
		public override bool Reached()
		{
			return this.reachedDel();
		}

		// Token: 0x06000D6F RID: 3439 RVA: 0x000372A7 File Offset: 0x000354A7
		public override void GetProgress(out int progress, out int outOf)
		{
			progress = (base.dir.profileAchievesModel.earnedAchievements.Contains(base.achievement) ? 1 : 0);
			outOf = 1;
		}

		// Token: 0x06000D70 RID: 3440 RVA: 0x000372D0 File Offset: 0x000354D0
		public override bool IsTracking(AchievementsDirector.GameDoubleStat stat)
		{
			AchievementsDirector.GameDoubleStat[] array = this.stats;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == stat)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04000CDA RID: 3290
		private AchievementsDirector.SimpleTracker.ReachedDelegate reachedDel;

		// Token: 0x04000CDB RID: 3291
		private AchievementsDirector.GameDoubleStat[] stats;

		// Token: 0x02000282 RID: 642
		// (Invoke) Token: 0x06000D72 RID: 3442
		public delegate bool ReachedDelegate();
	}

	// Token: 0x02000283 RID: 643
	public class UpdatableSimpleTracker : AchievementsDirector.SimpleTracker, AchievementsDirector.Updatable
	{
		// Token: 0x06000D75 RID: 3445 RVA: 0x000372FB File Offset: 0x000354FB
		public UpdatableSimpleTracker(AchievementsDirector dir, AchievementsDirector.Achievement achievement, AchievementsDirector.SimpleTracker.ReachedDelegate reachedDel, AchievementsDirector.UpdatableSimpleTracker.UpdateDelegate updateDel, params AchievementsDirector.GameDoubleStat[] stats) : base(dir, achievement, reachedDel, stats)
		{
			dir.RegisterUpdatable(this);
			this.updateDel = updateDel;
		}

		// Token: 0x06000D76 RID: 3446 RVA: 0x00037317 File Offset: 0x00035517
		public void Update()
		{
			this.updateDel();
		}

		// Token: 0x04000CDC RID: 3292
		private AchievementsDirector.UpdatableSimpleTracker.UpdateDelegate updateDel;

		// Token: 0x02000284 RID: 644
		// (Invoke) Token: 0x06000D78 RID: 3448
		public delegate void UpdateDelegate();
	}
}
