using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x02000303 RID: 771
public class ExchangeDirector : SRBehaviour, WorldModel.Participant
{
	// Token: 0x0600107E RID: 4222 RVA: 0x00041EAD File Offset: 0x000400AD
	public bool HasPendingOffers(ExchangeDirector.OfferType offerType)
	{
		return this.worldModel != null && offerType == ExchangeDirector.OfferType.GENERAL && this.worldModel.pendingOfferRancherIds.Count > 0;
	}

	// Token: 0x0600107F RID: 4223 RVA: 0x00041ED4 File Offset: 0x000400D4
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.progressDir = SRSingleton<SceneContext>.Instance.ProgressDirector;
		this.mailDir = SRSingleton<SceneContext>.Instance.MailDirector;
		this.tutorialDir = SRSingleton<SceneContext>.Instance.TutorialDirector;
		this.pediaDir = SRSingleton<SceneContext>.Instance.PediaDirector;
		foreach (ExchangeDirector.ValueEntry valueEntry in this.values)
		{
			this.valueDict[valueEntry.id] = valueEntry.value;
		}
		foreach (ExchangeDirector.NonIdentEntry nonIdentEntry in this.nonIdentRewards)
		{
			this.nonIdentRewardDict[nonIdentEntry.reward] = nonIdentEntry.icon;
		}
		this.ConfigureOfferGenerators();
	}

	// Token: 0x06001080 RID: 4224 RVA: 0x00041F9C File Offset: 0x0004019C
	public void Start()
	{
		if (this.progressDir.HasProgress(ProgressDirector.ProgressType.UNLOCK_WILDS))
		{
			this.pediaDir.Unlock(new PediaDirector.Id[]
			{
				PediaDirector.Id.WILDS_TUTORIAL
			});
		}
		if (this.progressDir.HasProgress(ProgressDirector.ProgressType.UNLOCK_VALLEY))
		{
			this.pediaDir.Unlock(new PediaDirector.Id[]
			{
				PediaDirector.Id.VALLEY_TUTORIAL
			});
		}
	}

	// Token: 0x06001081 RID: 4225 RVA: 0x00041FFB File Offset: 0x000401FB
	public void InitForLevel()
	{
		SRSingleton<SceneContext>.Instance.GameModel.RegisterWorldParticipant(this);
	}

	// Token: 0x06001082 RID: 4226 RVA: 0x00042010 File Offset: 0x00040210
	public void InitModel(WorldModel worldModel)
	{
		worldModel.currOffers.Clear();
		worldModel.lastOfferRancherIds.Clear();
		worldModel.pendingOfferRancherIds.Clear();
		int fullDays = SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().exchangeStartDay - 1;
		worldModel.nextDailyOfferCreateTime = TimeDirector.GetHourAfter(0.0, fullDays, 12.083333f);
	}

	// Token: 0x06001083 RID: 4227 RVA: 0x0004206F File Offset: 0x0004026F
	public void SetModel(WorldModel worldModel)
	{
		this.worldModel = worldModel;
	}

	// Token: 0x06001084 RID: 4228 RVA: 0x00042078 File Offset: 0x00040278
	private void PrepareNextDailyOffer()
	{
		this.SetupPendingOfferRanchers();
		this.worldModel.lastOfferRancherIds.Clear();
		this.worldModel.lastOfferRancherIds.AddRange(this.worldModel.pendingOfferRancherIds);
	}

	// Token: 0x06001085 RID: 4229 RVA: 0x000420AC File Offset: 0x000402AC
	private double GetNextDailyOfferCreateTime()
	{
		if (SRSingleton<SceneContext>.Instance.GameModel.currGameMode == PlayerState.GameMode.TIME_LIMIT_V2 && this.timeDir.CurrDay() == 1)
		{
			return this.timeDir.GetHourAfter(1, 12.083333f);
		}
		return this.timeDir.GetNextHour(12.083333f);
	}

	// Token: 0x06001086 RID: 4230 RVA: 0x000420FC File Offset: 0x000402FC
	public void Update()
	{
		ExchangeDirector.Offer offer = this.worldModel.currOffers.ContainsKey(ExchangeDirector.OfferType.GENERAL) ? this.worldModel.currOffers[ExchangeDirector.OfferType.GENERAL] : null;
		if (offer != null && this.timeDir.HasReached(offer.expireTime))
		{
			this.ClearOffer(ExchangeDirector.OfferType.GENERAL);
		}
		if (offer == null && this.timeDir.HasReached(this.worldModel.nextDailyOfferCreateTime))
		{
			this.worldModel.nextDailyOfferCreateTime = this.GetNextDailyOfferCreateTime();
			this.PrepareNextDailyOffer();
			this.OfferDidChange();
		}
		if (!this.worldModel.currOffers.ContainsKey(ExchangeDirector.OfferType.OGDEN_RECUR) && (this.worldModel.currOffers.ContainsKey(ExchangeDirector.OfferType.OGDEN) || (float)this.progressDir.GetProgress(ProgressDirector.ProgressType.OGDEN_REWARDS) >= 3f))
		{
			this.worldModel.currOffers[ExchangeDirector.OfferType.OGDEN_RECUR] = this.CreateOgdenRecurOffer();
			this.OfferDidChange();
		}
		if (!this.worldModel.currOffers.ContainsKey(ExchangeDirector.OfferType.MOCHI_RECUR) && (this.worldModel.currOffers.ContainsKey(ExchangeDirector.OfferType.MOCHI) || (float)this.progressDir.GetProgress(ProgressDirector.ProgressType.MOCHI_REWARDS) >= 3f))
		{
			this.worldModel.currOffers[ExchangeDirector.OfferType.MOCHI_RECUR] = this.CreateMochiRecurOffer();
			this.OfferDidChange();
		}
		if (!this.worldModel.currOffers.ContainsKey(ExchangeDirector.OfferType.VIKTOR_RECUR) && (this.worldModel.currOffers.ContainsKey(ExchangeDirector.OfferType.VIKTOR) || (float)this.progressDir.GetProgress(ProgressDirector.ProgressType.VIKTOR_REWARDS) >= 3f))
		{
			this.worldModel.currOffers[ExchangeDirector.OfferType.VIKTOR_RECUR] = this.CreateViktorRecurOffer();
			this.OfferDidChange();
		}
	}

	// Token: 0x06001087 RID: 4231 RVA: 0x00042298 File Offset: 0x00040498
	public bool MaybeStartNext(ExchangeDirector.OfferType offerType)
	{
		if (this.worldModel.currOffers.ContainsKey(offerType))
		{
			return false;
		}
		ExchangeDirector.ProgressOfferEntry progressEntry = this.GetProgressEntry(offerType);
		if (progressEntry != null && !this.worldModel.currOffers.ContainsKey(progressEntry.specialOfferType) && this.progressDir.GetProgress(progressEntry.progressType) < progressEntry.rewardLevels.Length)
		{
			this.worldModel.currOffers[progressEntry.specialOfferType] = this.CreateProgressOffer(progressEntry.specialOfferType, progressEntry.progressType, progressEntry.rewardLevels);
			this.OfferDidChange();
			return this.CreateRancherChatUI(offerType, true);
		}
		return false;
	}

	// Token: 0x06001088 RID: 4232 RVA: 0x00042338 File Offset: 0x00040538
	private void SetupPendingOfferRanchers()
	{
		this.worldModel.pendingOfferRancherIds.Clear();
		List<string> list = new List<string>();
		foreach (ExchangeDirector.Rancher rancher in this.ranchers)
		{
			ProgressDirector.ProgressType rancherProgressType = ProgressDirector.GetRancherProgressType(rancher.name);
			if (!this.progressDir.HasProgress(rancherProgressType))
			{
				this.worldModel.pendingOfferRancherIds.Add(rancher.name);
				this.mailDir.SendMail(MailDirector.Type.EXCHANGE, "exchangeintro_" + rancher.name);
				return;
			}
			if (!this.worldModel.lastOfferRancherIds.Contains(rancher.name))
			{
				list.Add(rancher.name);
			}
		}
		if (list.Count < 2)
		{
			Log.Error("Somehow do not have enough available ranchers to choose from for exchange offers.", Array.Empty<object>());
			return;
		}
		this.worldModel.pendingOfferRancherIds.Add(Randoms.SHARED.Pluck<string>(list, null));
		this.worldModel.pendingOfferRancherIds.Add(Randoms.SHARED.Pluck<string>(list, null));
	}

	// Token: 0x06001089 RID: 4233 RVA: 0x0004243C File Offset: 0x0004063C
	public double? GetOfferExpirationTime(ExchangeDirector.OfferType type)
	{
		if (this.worldModel.currOffers.ContainsKey(type))
		{
			return new double?(this.worldModel.currOffers[type].expireTime - this.timeDir.WorldTime());
		}
		return null;
	}

	// Token: 0x0600108A RID: 4234 RVA: 0x0004248D File Offset: 0x0004068D
	public List<ExchangeDirector.RequestedItemEntry> GetOfferRequests(ExchangeDirector.OfferType type)
	{
		if (this.worldModel == null)
		{
			return null;
		}
		if (this.worldModel.currOffers.ContainsKey(type))
		{
			return this.worldModel.currOffers[type].requests;
		}
		return null;
	}

	// Token: 0x0600108B RID: 4235 RVA: 0x000424C4 File Offset: 0x000406C4
	public List<ExchangeDirector.ItemEntry> GetOfferRewards(ExchangeDirector.OfferType type)
	{
		if (this.worldModel == null)
		{
			return null;
		}
		if (this.worldModel.currOffers.ContainsKey(type))
		{
			return this.worldModel.currOffers[type].rewards;
		}
		return null;
	}

	// Token: 0x0600108C RID: 4236 RVA: 0x000424FB File Offset: 0x000406FB
	public string GetOfferId(ExchangeDirector.OfferType type)
	{
		if (this.worldModel == null)
		{
			return null;
		}
		if (this.worldModel.currOffers.ContainsKey(type))
		{
			return this.worldModel.currOffers[type].offerId;
		}
		return null;
	}

	// Token: 0x0600108D RID: 4237 RVA: 0x00042534 File Offset: 0x00040734
	private ExchangeDirector.ProgressOfferEntry GetProgressEntry(ExchangeDirector.OfferType type)
	{
		foreach (ExchangeDirector.ProgressOfferEntry progressOfferEntry in this.progressOffers)
		{
			if (progressOfferEntry.specialOfferType == type)
			{
				return progressOfferEntry;
			}
		}
		return null;
	}

	// Token: 0x0600108E RID: 4238 RVA: 0x00042568 File Offset: 0x00040768
	public bool TryToAcceptNewOffer()
	{
		if (this.worldModel.pendingOfferRancherIds.Count == 0)
		{
			return false;
		}
		if (this.worldModel.pendingOfferRancherIds.Count == 1)
		{
			this.SelectDailyOffer(this.worldModel.pendingOfferRancherIds[0], true);
			return false;
		}
		SRSingleton<GameContext>.Instance.UITemplates.CreateRancherChoiceUI(this.worldModel.pendingOfferRancherIds);
		return true;
	}

	// Token: 0x0600108F RID: 4239 RVA: 0x000425D4 File Offset: 0x000407D4
	public bool SelectDailyOffer(string rancherId, bool isFirstOffer)
	{
		if (this.worldModel.currOffers.ContainsKey(ExchangeDirector.OfferType.GENERAL))
		{
			return false;
		}
		ExchangeDirector.Offer offer = this.CreateDailyOffer(rancherId, isFirstOffer);
		if (offer == null)
		{
			return false;
		}
		this.worldModel.currOffers[ExchangeDirector.OfferType.GENERAL] = offer;
		ProgressDirector.ProgressType rancherProgressType = ProgressDirector.GetRancherProgressType(offer.rancherId);
		this.progressDir.AddProgress(rancherProgressType);
		this.worldModel.pendingOfferRancherIds.Clear();
		this.OfferDidChange();
		return true;
	}

	// Token: 0x06001090 RID: 4240 RVA: 0x00042645 File Offset: 0x00040845
	public Sprite GetRancherImage(string rancherId)
	{
		return this.GetRancher(rancherId).defaultImg;
	}

	// Token: 0x06001091 RID: 4241 RVA: 0x00042653 File Offset: 0x00040853
	public Sprite GetRancherIcon(string rancherId)
	{
		return this.GetRancher(rancherId).icon;
	}

	// Token: 0x06001092 RID: 4242 RVA: 0x00042664 File Offset: 0x00040864
	private ExchangeDirector.Rancher GetRancher(string rancherId)
	{
		foreach (ExchangeDirector.Rancher rancher in this.ranchers)
		{
			if (rancher.name == rancherId)
			{
				return rancher;
			}
		}
		return null;
	}

	// Token: 0x06001093 RID: 4243 RVA: 0x0004269B File Offset: 0x0004089B
	public string GetOfferRancherId(ExchangeDirector.OfferType type)
	{
		if (this.worldModel.currOffers.ContainsKey(type))
		{
			return this.worldModel.currOffers[type].rancherId;
		}
		return null;
	}

	// Token: 0x06001094 RID: 4244 RVA: 0x000426C8 File Offset: 0x000408C8
	public void RewardsDidSpawn(ExchangeDirector.OfferType type)
	{
		this.ClearOffer(type);
	}

	// Token: 0x06001095 RID: 4245 RVA: 0x000426D1 File Offset: 0x000408D1
	public bool TryAccept(ExchangeDirector.OfferType type, Identifiable.Id id, ExchangeDirector.Awarder[] awarders)
	{
		if (this.worldModel.currOffers.ContainsKey(type) && this.worldModel.currOffers[type].TryAccept(id, awarders, type))
		{
			this.OfferDidChange();
			return true;
		}
		return false;
	}

	// Token: 0x06001096 RID: 4246 RVA: 0x0004270A File Offset: 0x0004090A
	public int GetCountForValue(Identifiable.Id id, int value)
	{
		if (this.valueDict.ContainsKey(id))
		{
			return Mathf.RoundToInt((float)value / this.valueDict[id]);
		}
		return 0;
	}

	// Token: 0x06001097 RID: 4247 RVA: 0x00042730 File Offset: 0x00040930
	public Sprite GetSpecRewardIcon(ExchangeDirector.NonIdentReward specReward)
	{
		return this.nonIdentRewardDict[specReward];
	}

	// Token: 0x06001098 RID: 4248 RVA: 0x0004273E File Offset: 0x0004093E
	private void ClearOffer(ExchangeDirector.OfferType type)
	{
		this.worldModel.currOffers.Remove(type);
		this.OfferDidChange();
	}

	// Token: 0x06001099 RID: 4249 RVA: 0x00042758 File Offset: 0x00040958
	private ExchangeDirector.Offer CreateOgdenRecurOffer()
	{
		return new ExchangeDirector.Offer("m.offer.ogden_recur", "ogden", double.PositiveInfinity, double.NegativeInfinity, new List<ExchangeDirector.RequestedItemEntry>
		{
			new ExchangeDirector.RequestedItemEntry(Identifiable.Id.KOOKADOBA_FRUIT, this.ogdenRecurAmount, 0)
		}, new List<ExchangeDirector.ItemEntry>
		{
			new ExchangeDirector.ItemEntry(Identifiable.Id.SPICY_TOFU, 1)
		});
	}

	// Token: 0x0600109A RID: 4250 RVA: 0x000427BC File Offset: 0x000409BC
	private ExchangeDirector.Offer CreateMochiRecurOffer()
	{
		return new ExchangeDirector.Offer("m.offer.mochi_recur", "mochi", double.PositiveInfinity, double.NegativeInfinity, new List<ExchangeDirector.RequestedItemEntry>
		{
			new ExchangeDirector.RequestedItemEntry(Identifiable.Id.QUICKSILVER_PLORT, this.mochiRecurAmount, 0)
		}, new List<ExchangeDirector.ItemEntry>
		{
			new ExchangeDirector.ItemEntry(ExchangeDirector.NonIdentReward.NEWBUCKS_MOCHI)
		});
	}

	// Token: 0x0600109B RID: 4251 RVA: 0x00042820 File Offset: 0x00040A20
	private ExchangeDirector.Offer CreateViktorRecurOffer()
	{
		return new ExchangeDirector.Offer("m.offer.viktor_recur", "viktor", double.PositiveInfinity, double.NegativeInfinity, new List<ExchangeDirector.RequestedItemEntry>
		{
			new ExchangeDirector.RequestedItemEntry(Identifiable.Id.GLITCH_BUG_REPORT, this.viktorRecurAmount, 0)
		}, new List<ExchangeDirector.ItemEntry>
		{
			new ExchangeDirector.ItemEntry(Identifiable.Id.MANIFOLD_CUBE_CRAFT, 1)
		});
	}

	// Token: 0x0600109C RID: 4252 RVA: 0x00042884 File Offset: 0x00040A84
	private ExchangeDirector.Offer CreateProgressOffer(ExchangeDirector.OfferType offerType, ProgressDirector.ProgressType progressType, ExchangeDirector.RewardLevel[] rewardLevels)
	{
		int num = this.progressDir.GetProgress(progressType) + 1;
		ExchangeDirector.RewardLevel rewardLevel = rewardLevels[num - 1];
		string offerId = string.Concat(new object[]
		{
			"m.offer.",
			progressType.ToString().ToLowerInvariant(),
			"_level",
			num
		});
		List<ExchangeDirector.RequestedItemEntry> list = new List<ExchangeDirector.RequestedItemEntry>();
		list.Add(new ExchangeDirector.RequestedItemEntry(rewardLevel.requestedItem, rewardLevel.count, 0));
		List<ExchangeDirector.ItemEntry> list2 = new List<ExchangeDirector.ItemEntry>();
		list2.Add(new ExchangeDirector.ItemEntry(rewardLevel.reward));
		return new ExchangeDirector.Offer(offerId, offerType.ToString().ToLowerInvariant(), double.PositiveInfinity, double.NegativeInfinity, list, list2);
	}

	// Token: 0x0600109D RID: 4253 RVA: 0x00042940 File Offset: 0x00040B40
	private ExchangeDirector.Offer CreateDailyOffer(string rancherId, bool isFirstOffer)
	{
		int num = 10;
		if (SRSingleton<SceneContext>.Instance.GameModel.currGameMode != PlayerState.GameMode.TIME_LIMIT_V2)
		{
			return this.offerGenerators[rancherId].Generate(this, this.CreateWhiteList(), this.timeDir.GetNextHourAtLeastHalfDay(12f), this.timeDir.HoursFromNow(2f), num, isFirstOffer, SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().exchangeRewardsGoldPlorts);
		}
		List<Identifiable.Id> whitelist = this.CreateRushModeWhiteList(this.timeDir.CurrDay());
		List<ExchangeDirector.RequestedItemEntry> list = null;
		while (list == null && num > 0)
		{
			list = this.offerGenerators[rancherId].GenerateRequestList(this, whitelist);
			num--;
		}
		if (list == null)
		{
			return null;
		}
		string offerId = string.Format("m.offer_{0}.{1}", this.offerGenerators[rancherId].GetRandomBlurb(), rancherId);
		List<ExchangeDirector.ItemEntry> rewards = new List<ExchangeDirector.ItemEntry>
		{
			new ExchangeDirector.ItemEntry(ExchangeDirector.NonIdentReward.TIME_EXTENSION_12H),
			new ExchangeDirector.ItemEntry(Identifiable.Id.GINGER_VEGGIE, 6)
		};
		return new ExchangeDirector.Offer(offerId, rancherId, this.timeDir.GetNextHourAtLeastHalfDay(12f), this.timeDir.HoursFromNow(2f), list, rewards);
	}

	// Token: 0x0600109E RID: 4254 RVA: 0x00042A60 File Offset: 0x00040C60
	private List<Identifiable.Id> CreateRushModeWhiteList(int day)
	{
		HashSet<ProgressDirector.ProgressType> hashSet = new HashSet<ProgressDirector.ProgressType>(ProgressDirector.progressTypeComparer);
		if (day >= 2)
		{
			hashSet.Add(ProgressDirector.ProgressType.UNLOCK_QUARRY);
			hashSet.Add(ProgressDirector.ProgressType.UNLOCK_MOSS);
		}
		if (day >= 3)
		{
			hashSet.Add(ProgressDirector.ProgressType.UNLOCK_DESERT);
			hashSet.Add(ProgressDirector.ProgressType.UNLOCK_RUINS);
		}
		List<Identifiable.Id> list = new List<Identifiable.Id>(this.initUnlocked);
		foreach (ExchangeDirector.UnlockList unlockList in this.unlockLists)
		{
			if (hashSet.Contains(unlockList.unlock))
			{
				list.AddRange(unlockList.ids);
			}
		}
		return list;
	}

	// Token: 0x0600109F RID: 4255 RVA: 0x00042AE4 File Offset: 0x00040CE4
	private List<Identifiable.Id> CreateWhiteList()
	{
		List<Identifiable.Id> list = new List<Identifiable.Id>();
		list.AddRange(this.initUnlocked);
		foreach (ExchangeDirector.UnlockList unlockList in this.unlockLists)
		{
			if (this.progressDir.HasProgress(unlockList.unlock))
			{
				list.AddRange(unlockList.ids);
			}
		}
		return list;
	}

	// Token: 0x060010A0 RID: 4256 RVA: 0x00042B3C File Offset: 0x00040D3C
	private void OfferDidChange()
	{
		if (this.onOfferChanged != null)
		{
			this.onOfferChanged();
		}
	}

	// Token: 0x060010A1 RID: 4257 RVA: 0x00042B54 File Offset: 0x00040D54
	private void ConfigureOfferGenerators()
	{
		this.offerGenerators.Clear();
		foreach (ExchangeDirector.Rancher rancher in this.ranchers)
		{
			List<Identifiable.Id> list = new List<Identifiable.Id>();
			foreach (ExchangeDirector.Category key in rancher.requestCategories)
			{
				list.AddRange(this.catDict[key]);
			}
			list.AddRange(rancher.indivRequests);
			List<Identifiable.Id> list2 = new List<Identifiable.Id>();
			foreach (ExchangeDirector.Category key2 in rancher.rewardCategories)
			{
				list2.AddRange(this.catDict[key2]);
			}
			list2.AddRange(rancher.indivRewards);
			List<Identifiable.Id> list3 = new List<Identifiable.Id>();
			foreach (ExchangeDirector.Category key3 in rancher.rareRewardCategories)
			{
				list3.AddRange(this.catDict[key3]);
			}
			list3.AddRange(rancher.indivRareRewards);
			this.offerGenerators[rancher.name] = new OfferGenerator(rancher.name, rancher.numBlurbs, list.ToArray(), list2.ToArray(), list3.ToArray());
		}
	}

	// Token: 0x060010A2 RID: 4258 RVA: 0x00042C9C File Offset: 0x00040E9C
	public bool IsOffline(ExchangeDirector.OfferType offerType)
	{
		return this.GetOfferRequests(offerType) == null && !this.HasPendingOffers(offerType);
	}

	// Token: 0x060010A3 RID: 4259 RVA: 0x00042CB4 File Offset: 0x00040EB4
	public bool CreateRancherChatUI(ExchangeDirector.OfferType offerType, bool intro)
	{
		RancherChatMetadata rancherChatMetadata = this.GetRancherChatMetadata(offerType, intro);
		if (rancherChatMetadata == null)
		{
			return false;
		}
		RancherChatUI.Instantiate(rancherChatMetadata).onDestroy = delegate()
		{
			if (SRSingleton<SceneContext>.Instance != null)
			{
				switch (offerType)
				{
				case ExchangeDirector.OfferType.OGDEN:
					if (this.progressDir.SetUniqueProgress(ProgressDirector.ProgressType.UNLOCK_WILDS))
					{
						this.tutorialDir.MaybeShowPopup(TutorialDirector.Id.WILDS_SLIMEPEDIA);
						this.pediaDir.Unlock(new PediaDirector.Id[]
						{
							PediaDirector.Id.WILDS_TUTORIAL
						});
						return;
					}
					break;
				case ExchangeDirector.OfferType.OGDEN_RECUR:
				case ExchangeDirector.OfferType.MOCHI_RECUR:
					break;
				case ExchangeDirector.OfferType.MOCHI:
					if (this.progressDir.SetUniqueProgress(ProgressDirector.ProgressType.UNLOCK_VALLEY))
					{
						this.tutorialDir.MaybeShowPopup(TutorialDirector.Id.VALLEY_SLIMEPEDIA);
						this.pediaDir.Unlock(new PediaDirector.Id[]
						{
							PediaDirector.Id.VALLEY_TUTORIAL
						});
						return;
					}
					break;
				case ExchangeDirector.OfferType.VIKTOR:
					if (this.progressDir.SetUniqueProgress(ProgressDirector.ProgressType.UNLOCK_SLIMULATIONS))
					{
						this.tutorialDir.MaybeShowPopup(TutorialDirector.Id.SLIMULATIONS_SLIMEPEDIA);
						this.pediaDir.Unlock(new PediaDirector.Id[]
						{
							PediaDirector.Id.SLIMULATIONS_TUTORIAL
						});
					}
					break;
				default:
					return;
				}
			}
		};
		return true;
	}

	// Token: 0x060010A4 RID: 4260 RVA: 0x00042D08 File Offset: 0x00040F08
	private RancherChatMetadata GetRancherChatMetadata(ExchangeDirector.OfferType offerType, bool intro)
	{
		switch (offerType)
		{
		case ExchangeDirector.OfferType.OGDEN_RECUR:
		{
			ExchangeDirector.ProgressOfferEntry progressEntry = this.GetProgressEntry(ExchangeDirector.OfferType.OGDEN);
			intro = this.progressDir.SetUniqueProgress(ProgressDirector.ProgressType.OGDEN_SEEN_FINAL_CHAT);
			if (!intro)
			{
				return progressEntry.rancherChatEndRepeat;
			}
			return progressEntry.rancherChatEndIntro;
		}
		case ExchangeDirector.OfferType.MOCHI_RECUR:
		{
			ExchangeDirector.ProgressOfferEntry progressEntry2 = this.GetProgressEntry(ExchangeDirector.OfferType.MOCHI);
			intro = this.progressDir.SetUniqueProgress(ProgressDirector.ProgressType.MOCHI_SEEN_FINAL_CHAT);
			if (!intro)
			{
				return progressEntry2.rancherChatEndRepeat;
			}
			return progressEntry2.rancherChatEndIntro;
		}
		case ExchangeDirector.OfferType.VIKTOR_RECUR:
		{
			ExchangeDirector.ProgressOfferEntry progressEntry3 = this.GetProgressEntry(ExchangeDirector.OfferType.VIKTOR);
			intro = this.progressDir.SetUniqueProgress(ProgressDirector.ProgressType.VIKTOR_SEEN_FINAL_CHAT);
			if (!intro)
			{
				return progressEntry3.rancherChatEndRepeat;
			}
			return progressEntry3.rancherChatEndIntro;
		}
		}
		ExchangeDirector.ProgressOfferEntry progressEntry4 = this.GetProgressEntry(offerType);
		if (progressEntry4 != null)
		{
			int progress = this.progressDir.GetProgress(progressEntry4.progressType);
			if (progress < progressEntry4.rewardLevels.Length)
			{
				ExchangeDirector.RewardLevel rewardLevel = progressEntry4.rewardLevels[progress];
				if (!intro)
				{
					return rewardLevel.rancherChatRepeat;
				}
				return rewardLevel.rancherChatIntro;
			}
		}
		if (this.worldModel.currOffers.ContainsKey(offerType))
		{
			ExchangeDirector.Offer offer = this.worldModel.currOffers[offerType];
			return this.CreateRancherChatMetadata(offer.rancherId, offer.offerId);
		}
		return null;
	}

	// Token: 0x060010A5 RID: 4261 RVA: 0x00042E3C File Offset: 0x0004103C
	public RancherChatMetadata CreateRancherChatMetadata(string rancherId, string message)
	{
		RancherChatMetadata rancherChatMetadata = ScriptableObject.CreateInstance<RancherChatMetadata>();
		rancherChatMetadata.entries = new RancherChatMetadata.Entry[]
		{
			new RancherChatMetadata.Entry
			{
				rancherName = (RancherChatMetadata.Entry.RancherName)Enum.Parse(typeof(RancherChatMetadata.Entry.RancherName), rancherId.ToUpperInvariant()),
				rancherImage = this.GetRancherImage(rancherId),
				messageBackground = this.GetRancher(rancherId).chatBackground,
				messageText = message
			}
		};
		return rancherChatMetadata;
	}

	// Token: 0x04000F43 RID: 3907
	public ExchangeDirector.OnOfferChanged onOfferChanged;

	// Token: 0x04000F44 RID: 3908
	[Tooltip("Values to be used in generating offers.")]
	public ExchangeDirector.ValueEntry[] values;

	// Token: 0x04000F45 RID: 3909
	public ExchangeDirector.ProgressOfferEntry[] progressOffers;

	// Token: 0x04000F46 RID: 3910
	private Dictionary<ExchangeDirector.Category, Identifiable.Id[]> catDict = new Dictionary<ExchangeDirector.Category, Identifiable.Id[]>
	{
		{
			ExchangeDirector.Category.FRUIT,
			new List<Identifiable.Id>(Identifiable.FRUIT_CLASS).ToArray()
		},
		{
			ExchangeDirector.Category.VEGGIES,
			new List<Identifiable.Id>(Identifiable.VEGGIE_CLASS).ToArray()
		},
		{
			ExchangeDirector.Category.MEAT,
			new List<Identifiable.Id>(Identifiable.MEAT_CLASS).ToArray()
		},
		{
			ExchangeDirector.Category.PLORTS,
			new List<Identifiable.Id>(Identifiable.PLORT_CLASS).ToArray()
		},
		{
			ExchangeDirector.Category.SLIMES,
			new List<Identifiable.Id>(Identifiable.SLIME_CLASS).ToArray()
		},
		{
			ExchangeDirector.Category.CRAFT_MATS,
			new List<Identifiable.Id>(Identifiable.CRAFT_CLASS).ToArray()
		}
	};

	// Token: 0x04000F47 RID: 3911
	[Tooltip("The ranchers and what they request/reward the player with.")]
	public ExchangeDirector.Rancher[] ranchers;

	// Token: 0x04000F48 RID: 3912
	public Identifiable.Id[] initUnlocked;

	// Token: 0x04000F49 RID: 3913
	public ExchangeDirector.UnlockList[] unlockLists;

	// Token: 0x04000F4A RID: 3914
	public ExchangeDirector.NonIdentEntry[] nonIdentRewards;

	// Token: 0x04000F4B RID: 3915
	public int ogdenRecurAmount = 3;

	// Token: 0x04000F4C RID: 3916
	public int mochiRecurAmount = 5;

	// Token: 0x04000F4D RID: 3917
	public int viktorRecurAmount = 5;

	// Token: 0x04000F4E RID: 3918
	private Dictionary<ExchangeDirector.NonIdentReward, Sprite> nonIdentRewardDict = new Dictionary<ExchangeDirector.NonIdentReward, Sprite>();

	// Token: 0x04000F4F RID: 3919
	private Dictionary<Identifiable.Id, float> valueDict = new Dictionary<Identifiable.Id, float>(Identifiable.idComparer);

	// Token: 0x04000F50 RID: 3920
	private TimeDirector timeDir;

	// Token: 0x04000F51 RID: 3921
	private ProgressDirector progressDir;

	// Token: 0x04000F52 RID: 3922
	private MailDirector mailDir;

	// Token: 0x04000F53 RID: 3923
	private PediaDirector pediaDir;

	// Token: 0x04000F54 RID: 3924
	private TutorialDirector tutorialDir;

	// Token: 0x04000F55 RID: 3925
	private Dictionary<string, OfferGenerator> offerGenerators = new Dictionary<string, OfferGenerator>();

	// Token: 0x04000F56 RID: 3926
	private WorldModel worldModel;

	// Token: 0x04000F57 RID: 3927
	private const float HOURS_BETWEEN_OFFERS = 0.083333336f;

	// Token: 0x04000F58 RID: 3928
	private const float OFFER_END_HOUR = 12f;

	// Token: 0x04000F59 RID: 3929
	private const float OFFER_HOUR = 12.083333f;

	// Token: 0x04000F5A RID: 3930
	private const float DAYS_PER_DAILY_LEVEL = 3f;

	// Token: 0x04000F5B RID: 3931
	private const float OGDEN_LEVELS = 3f;

	// Token: 0x04000F5C RID: 3932
	private const float MOCHI_LEVELS = 3f;

	// Token: 0x04000F5D RID: 3933
	private const float VIKTOR_LEVELS = 3f;

	// Token: 0x04000F5E RID: 3934
	private const float EARLY_EXCHANGE_HOURS = 2f;

	// Token: 0x02000304 RID: 772
	// (Invoke) Token: 0x060010A8 RID: 4264
	public delegate void OnAwakeDelegate(ExchangeDirector exchangeDir);

	// Token: 0x02000305 RID: 773
	// (Invoke) Token: 0x060010AC RID: 4268
	public delegate void OnOfferChanged();

	// Token: 0x02000306 RID: 774
	public interface Awarder
	{
		// Token: 0x060010AF RID: 4271
		void AwardIfType(ExchangeDirector.OfferType offerType);
	}

	// Token: 0x02000307 RID: 775
	public enum OfferType
	{
		// Token: 0x04000F60 RID: 3936
		GENERAL,
		// Token: 0x04000F61 RID: 3937
		OGDEN,
		// Token: 0x04000F62 RID: 3938
		OGDEN_RECUR,
		// Token: 0x04000F63 RID: 3939
		MOCHI,
		// Token: 0x04000F64 RID: 3940
		MOCHI_RECUR,
		// Token: 0x04000F65 RID: 3941
		VIKTOR,
		// Token: 0x04000F66 RID: 3942
		VIKTOR_RECUR
	}

	// Token: 0x02000308 RID: 776
	[Serializable]
	public class ValueEntry
	{
		// Token: 0x04000F67 RID: 3943
		public Identifiable.Id id;

		// Token: 0x04000F68 RID: 3944
		public float value;
	}

	// Token: 0x02000309 RID: 777
	[Serializable]
	public class Rancher
	{
		// Token: 0x04000F69 RID: 3945
		public string name;

		// Token: 0x04000F6A RID: 3946
		public Sprite defaultImg;

		// Token: 0x04000F6B RID: 3947
		public Sprite icon;

		// Token: 0x04000F6C RID: 3948
		public Material chatBackground;

		// Token: 0x04000F6D RID: 3949
		public int numBlurbs;

		// Token: 0x04000F6E RID: 3950
		public ExchangeDirector.Category[] requestCategories;

		// Token: 0x04000F6F RID: 3951
		public Identifiable.Id[] indivRequests;

		// Token: 0x04000F70 RID: 3952
		public ExchangeDirector.Category[] rewardCategories;

		// Token: 0x04000F71 RID: 3953
		public Identifiable.Id[] indivRewards;

		// Token: 0x04000F72 RID: 3954
		public ExchangeDirector.Category[] rareRewardCategories;

		// Token: 0x04000F73 RID: 3955
		public Identifiable.Id[] indivRareRewards;
	}

	// Token: 0x0200030A RID: 778
	[Serializable]
	public class RewardLevel
	{
		// Token: 0x04000F74 RID: 3956
		public ExchangeDirector.NonIdentReward reward;

		// Token: 0x04000F75 RID: 3957
		public Identifiable.Id requestedItem;

		// Token: 0x04000F76 RID: 3958
		public int count;

		// Token: 0x04000F77 RID: 3959
		public RancherChatMetadata rancherChatIntro;

		// Token: 0x04000F78 RID: 3960
		public RancherChatMetadata rancherChatRepeat;
	}

	// Token: 0x0200030B RID: 779
	[Serializable]
	public class ProgressOfferEntry
	{
		// Token: 0x04000F79 RID: 3961
		public ExchangeDirector.OfferType specialOfferType;

		// Token: 0x04000F7A RID: 3962
		public ProgressDirector.ProgressType progressType;

		// Token: 0x04000F7B RID: 3963
		public ExchangeDirector.RewardLevel[] rewardLevels;

		// Token: 0x04000F7C RID: 3964
		public RancherChatMetadata rancherChatEndIntro;

		// Token: 0x04000F7D RID: 3965
		public RancherChatMetadata rancherChatEndRepeat;
	}

	// Token: 0x0200030C RID: 780
	public enum Category
	{
		// Token: 0x04000F7F RID: 3967
		FRUIT,
		// Token: 0x04000F80 RID: 3968
		VEGGIES,
		// Token: 0x04000F81 RID: 3969
		MEAT,
		// Token: 0x04000F82 RID: 3970
		PLORTS,
		// Token: 0x04000F83 RID: 3971
		SLIMES,
		// Token: 0x04000F84 RID: 3972
		CRAFT_MATS
	}

	// Token: 0x0200030D RID: 781
	[Serializable]
	public class UnlockList
	{
		// Token: 0x04000F85 RID: 3973
		public ProgressDirector.ProgressType unlock;

		// Token: 0x04000F86 RID: 3974
		public Identifiable.Id[] ids;
	}

	// Token: 0x0200030E RID: 782
	[Serializable]
	public class RequestedItemEntry : ExchangeDirector.ItemEntry
	{
		// Token: 0x060010B5 RID: 4277 RVA: 0x00042F89 File Offset: 0x00041189
		public RequestedItemEntry(Identifiable.Id id, int count, int progress, ExchangeDirector.NonIdentReward specReward) : base(id, count, specReward)
		{
			this.progress = progress;
		}

		// Token: 0x060010B6 RID: 4278 RVA: 0x00042F9C File Offset: 0x0004119C
		public RequestedItemEntry(Identifiable.Id id, int count, int progress) : base(id, count)
		{
			this.progress = progress;
		}

		// Token: 0x060010B7 RID: 4279 RVA: 0x00042FAD File Offset: 0x000411AD
		public bool IsComplete()
		{
			return this.progress >= this.count;
		}

		// Token: 0x04000F87 RID: 3975
		public int progress;
	}

	// Token: 0x0200030F RID: 783
	public enum NonIdentReward
	{
		// Token: 0x04000F89 RID: 3977
		NONE,
		// Token: 0x04000F8A RID: 3978
		OGDEN_MIX = 100,
		// Token: 0x04000F8B RID: 3979
		OGDEN_GARDEN,
		// Token: 0x04000F8C RID: 3980
		OGDEN_RANCH,
		// Token: 0x04000F8D RID: 3981
		MOCHI_EXTRA_MILE = 200,
		// Token: 0x04000F8E RID: 3982
		MOCHI_COOP,
		// Token: 0x04000F8F RID: 3983
		MOCHI_RANCH,
		// Token: 0x04000F90 RID: 3984
		VIKTOR_CHICKEN_CLONER = 300,
		// Token: 0x04000F91 RID: 3985
		VIKTOR_DELUXE_DRONES,
		// Token: 0x04000F92 RID: 3986
		VIKTOR_RANCH,
		// Token: 0x04000F93 RID: 3987
		NEWBUCKS_SMALL = 10000,
		// Token: 0x04000F94 RID: 3988
		NEWBUCKS_MEDIUM,
		// Token: 0x04000F95 RID: 3989
		NEWBUCKS_LARGE,
		// Token: 0x04000F96 RID: 3990
		NEWBUCKS_HUGE,
		// Token: 0x04000F97 RID: 3991
		NEWBUCKS_MOCHI,
		// Token: 0x04000F98 RID: 3992
		TIME_EXTENSION_12H = 20000
	}

	// Token: 0x02000310 RID: 784
	[Serializable]
	public class NonIdentEntry
	{
		// Token: 0x04000F99 RID: 3993
		public ExchangeDirector.NonIdentReward reward;

		// Token: 0x04000F9A RID: 3994
		public Sprite icon;
	}

	// Token: 0x02000311 RID: 785
	[Serializable]
	public class ItemEntry
	{
		// Token: 0x060010B9 RID: 4281 RVA: 0x00042FC0 File Offset: 0x000411C0
		public ItemEntry(Identifiable.Id id, int count, ExchangeDirector.NonIdentReward specReward)
		{
			this.id = id;
			this.specReward = specReward;
			this.count = count;
		}

		// Token: 0x060010BA RID: 4282 RVA: 0x00042FDD File Offset: 0x000411DD
		public ItemEntry(Identifiable.Id id, int count)
		{
			this.id = id;
			this.specReward = ExchangeDirector.NonIdentReward.NONE;
			this.count = count;
		}

		// Token: 0x060010BB RID: 4283 RVA: 0x00042FFA File Offset: 0x000411FA
		public ItemEntry(ExchangeDirector.NonIdentReward specReward)
		{
			this.id = Identifiable.Id.NONE;
			this.specReward = specReward;
			this.count = 1;
		}

		// Token: 0x04000F9B RID: 3995
		public Identifiable.Id id;

		// Token: 0x04000F9C RID: 3996
		public ExchangeDirector.NonIdentReward specReward;

		// Token: 0x04000F9D RID: 3997
		public int count;
	}

	// Token: 0x02000312 RID: 786
	[Serializable]
	public class Offer
	{
		// Token: 0x060010BC RID: 4284 RVA: 0x00043017 File Offset: 0x00041217
		public Offer(string offerId, string rancherId, double expireTime, double earlyExchangeTime, List<ExchangeDirector.RequestedItemEntry> requests, List<ExchangeDirector.ItemEntry> rewards)
		{
			this.offerId = offerId;
			this.rancherId = rancherId;
			this.expireTime = expireTime;
			this.earlyExchangeTime = earlyExchangeTime;
			this.requests = requests;
			this.rewards = rewards;
		}

		// Token: 0x060010BD RID: 4285 RVA: 0x0004304C File Offset: 0x0004124C
		public bool TryAccept(Identifiable.Id id, ExchangeDirector.Awarder[] awarders, ExchangeDirector.OfferType offerType)
		{
			foreach (ExchangeDirector.RequestedItemEntry requestedItemEntry in this.requests)
			{
				if (requestedItemEntry.id == id && !requestedItemEntry.IsComplete())
				{
					requestedItemEntry.progress++;
					if (this.IsComplete())
					{
						for (int i = 0; i < awarders.Length; i++)
						{
							awarders[i].AwardIfType(offerType);
						}
						if (offerType == ExchangeDirector.OfferType.GENERAL && !SRSingleton<SceneContext>.Instance.TimeDirector.HasReached(this.earlyExchangeTime))
						{
							SRSingleton<SceneContext>.Instance.AchievementsDirector.AddToStat(AchievementsDirector.IntStat.FULFILL_EXCHANGE_EARLY, 1);
						}
						if (offerType == ExchangeDirector.OfferType.GENERAL)
						{
							if (this.rancherId == "ogden")
							{
								SRSingleton<SceneContext>.Instance.ProgressDirector.MaybeUnlockOgdenMissions();
							}
							else if (this.rancherId == "mochi")
							{
								SRSingleton<SceneContext>.Instance.ProgressDirector.MaybeUnlockMochiMissions();
							}
							else if (this.rancherId == "viktor")
							{
								SRSingleton<SceneContext>.Instance.ProgressDirector.MaybeUnlockViktorMissions();
							}
						}
						AnalyticsUtil.CustomEvent("ExchangeOfferComplete", new Dictionary<string, object>
						{
							{
								"RancherId",
								this.rancherId
							},
							{
								"ExchangeId",
								this.offerId
							}
						}, true);
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x060010BE RID: 4286 RVA: 0x000431C4 File Offset: 0x000413C4
		public bool IsComplete()
		{
			using (List<ExchangeDirector.RequestedItemEntry>.Enumerator enumerator = this.requests.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.IsComplete())
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x04000F9E RID: 3998
		public List<ExchangeDirector.RequestedItemEntry> requests;

		// Token: 0x04000F9F RID: 3999
		public List<ExchangeDirector.ItemEntry> rewards;

		// Token: 0x04000FA0 RID: 4000
		public double expireTime;

		// Token: 0x04000FA1 RID: 4001
		public double earlyExchangeTime;

		// Token: 0x04000FA2 RID: 4002
		public string rancherId;

		// Token: 0x04000FA3 RID: 4003
		public string offerId;
	}
}
