using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020007AB RID: 1963
public class TutorialDirector : SRBehaviour, TutorialsModel.Participant
{
	// Token: 0x06002921 RID: 10529 RVA: 0x0009B460 File Offset: 0x00099660
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.optionsDir = SRSingleton<GameContext>.Instance.OptionsDirector;
		this.progressDir = SRSingleton<SceneContext>.Instance.ProgressDirector;
		foreach (TutorialDirector.IdEntry idEntry in this.entries)
		{
			this.entryDict[idEntry.id] = idEntry;
		}
		this.InitTrackers();
		this.InitDependencies();
	}

	// Token: 0x06002922 RID: 10530 RVA: 0x0009B4D4 File Offset: 0x000996D4
	public void InitForLevel()
	{
		SRSingleton<SceneContext>.Instance.GameModel.RegisterTutorials(this);
		this.popupQueue.Clear();
		this.InitTrackers();
		this.InitDependencies();
		SceneContext.onSceneLoaded = (SceneContext.SceneLoadDelegate)Delegate.Combine(SceneContext.onSceneLoaded, new SceneContext.SceneLoadDelegate(delegate(SceneContext ctx)
		{
			this.MaybeShowStatusTutorials();
		}));
	}

	// Token: 0x06002923 RID: 10531 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(TutorialsModel tutModel)
	{
	}

	// Token: 0x06002924 RID: 10532 RVA: 0x0009B528 File Offset: 0x00099728
	public void SetModel(TutorialsModel tutModel)
	{
		this.tutModel = tutModel;
	}

	// Token: 0x06002925 RID: 10533 RVA: 0x0009B534 File Offset: 0x00099734
	public void Update()
	{
		if (this.currTracker != null)
		{
			this.currTracker.Update();
			if (this.currTracker.MarkCompleted())
			{
				this.currPopup.CompletedAction();
			}
			if (this.currTracker.ShouldEnd())
			{
				this.currPopup.Complete();
				return;
			}
			if (this.currTracker.ShouldHide())
			{
				this.currPopup.Hide();
			}
		}
	}

	// Token: 0x06002926 RID: 10534 RVA: 0x0009B5A0 File Offset: 0x000997A0
	public void OnVac(Identifiable.Id id)
	{
		if (this.currTracker is TutorialDirector.VacTutorial)
		{
			((TutorialDirector.VacTutorial)this.currTracker).OnVac();
		}
		if (Identifiable.IsSlime(id))
		{
			this.MaybeShowPopup(TutorialDirector.Id.SHOOTING);
			return;
		}
		if (Identifiable.IsFood(id))
		{
			this.MaybeShowPopup(TutorialDirector.Id.FOOD);
			return;
		}
		if (Identifiable.IsPlort(id))
		{
			this.MaybeShowPopup(TutorialDirector.Id.PLORT);
		}
	}

	// Token: 0x06002927 RID: 10535 RVA: 0x0009B5F9 File Offset: 0x000997F9
	public void OnProgress(ProgressDirector.ProgressType type)
	{
		if (type == ProgressDirector.ProgressType.UNLOCK_LAB)
		{
			this.MaybeShowScienceTutorial();
		}
	}

	// Token: 0x06002928 RID: 10536 RVA: 0x0009B605 File Offset: 0x00099805
	public void OnShoot(Identifiable.Id id)
	{
		if (this.currTracker != null)
		{
			this.currTracker.OnShot(id);
		}
	}

	// Token: 0x06002929 RID: 10537 RVA: 0x0009B61B File Offset: 0x0009981B
	public void OnDelaunchedSlime()
	{
		if (this.currTracker is TutorialDirector.SlimeShootingTutorial)
		{
			((TutorialDirector.SlimeShootingTutorial)this.currTracker).OnDelaunchedSlime();
		}
	}

	// Token: 0x0600292A RID: 10538 RVA: 0x0009B63C File Offset: 0x0009983C
	public void OnEnteredRanch()
	{
		if (this.currTracker is TutorialDirector.RanchOnlyTutorial)
		{
			((TutorialDirector.RanchOnlyTutorial)this.currTracker).EnteredRanch();
		}
		List<TutorialDirector.Id> list = new List<TutorialDirector.Id>(this.hidden);
		this.hidden.Clear();
		foreach (TutorialDirector.Id id in list)
		{
			this.MaybeShowPopup(id);
		}
	}

	// Token: 0x0600292B RID: 10539 RVA: 0x0009B6BC File Offset: 0x000998BC
	public void OnLeftRanch()
	{
		if (this.currTracker is TutorialDirector.RanchOnlyTutorial)
		{
			((TutorialDirector.RanchOnlyTutorial)this.currTracker).LeftRanch();
		}
	}

	// Token: 0x0600292C RID: 10540 RVA: 0x0009B6DB File Offset: 0x000998DB
	public void RemoveHidden(TutorialDirector.Id id)
	{
		this.hidden.Remove(id);
	}

	// Token: 0x0600292D RID: 10541 RVA: 0x0009B6EA File Offset: 0x000998EA
	public void SetInMarketArea(bool inArea)
	{
		this.inMarketArea = inArea;
		if (this.inMarketArea)
		{
			this.MaybeShowPopup(TutorialDirector.Id.MARKET);
		}
	}

	// Token: 0x0600292E RID: 10542 RVA: 0x0009B702 File Offset: 0x00099902
	public void SetInBarnArea(bool inArea)
	{
		this.inBarnArea = inArea;
		if (this.inBarnArea)
		{
			this.MaybeShowPopup(TutorialDirector.Id.SCIENCE_SUMMARY);
			this.MaybeShowPopup(TutorialDirector.Id.SCIENCE_REFINERY);
			this.MaybeShowPopup(TutorialDirector.Id.SCIENCE_BUILDER_SHOP);
			this.MaybeShowPopup(TutorialDirector.Id.SCIENCE_FABRICATOR);
			this.MaybeShowPopup(TutorialDirector.Id.SCIENCE_GADGET_MODE);
		}
	}

	// Token: 0x0600292F RID: 10543 RVA: 0x0009B73B File Offset: 0x0009993B
	public void OnPlortSold()
	{
		if (this.currTracker is TutorialDirector.MarketTutorial)
		{
			((TutorialDirector.MarketTutorial)this.currTracker).OnPlortSold();
		}
	}

	// Token: 0x06002930 RID: 10544 RVA: 0x0009B75A File Offset: 0x0009995A
	public void OnPlanted()
	{
		if (this.currTracker is TutorialDirector.GardenTutorial)
		{
			((TutorialDirector.GardenTutorial)this.currTracker).OnPlanted();
		}
	}

	// Token: 0x06002931 RID: 10545 RVA: 0x0009B779 File Offset: 0x00099979
	public void OnPlayerDeath(PlayerDeathHandler.DeathType deathType)
	{
		if (deathType == PlayerDeathHandler.DeathType.DEFAULT)
		{
			this.MaybeShowPopup(TutorialDirector.Id.DEATH);
		}
	}

	// Token: 0x06002932 RID: 10546 RVA: 0x0009B785 File Offset: 0x00099985
	public void OnRefineryAdded()
	{
		if (this.currTracker is TutorialDirector.RefineryTutorial)
		{
			((TutorialDirector.RefineryTutorial)this.currTracker).OnRefineryAdded();
		}
	}

	// Token: 0x06002933 RID: 10547 RVA: 0x0009B7A4 File Offset: 0x000999A4
	public void OnGadgetModeActivated()
	{
		if (this.currTracker is TutorialDirector.GadgetModeTutorial)
		{
			((TutorialDirector.GadgetModeTutorial)this.currTracker).OnGadgetModeActivated();
		}
		this.MaybeShowPopup(TutorialDirector.Id.SCIENCE_SITES);
		this.MaybeShowPopup(TutorialDirector.Id.SCIENCE_PLACE_SITE);
	}

	// Token: 0x06002934 RID: 10548 RVA: 0x0009B7D3 File Offset: 0x000999D3
	public void OnLiquidSlotGained()
	{
		this.MaybeShowPopup(TutorialDirector.Id.WATER);
	}

	// Token: 0x06002935 RID: 10549 RVA: 0x0009B7DD File Offset: 0x000999DD
	public void OnMapDataGained()
	{
		this.MaybeShowPopup(TutorialDirector.Id.MAP);
	}

	// Token: 0x06002936 RID: 10550 RVA: 0x0009B7E7 File Offset: 0x000999E7
	public void OnFabricatorOpen()
	{
		if (this.currTracker is TutorialDirector.FabricatorTutorial)
		{
			((TutorialDirector.FabricatorTutorial)this.currTracker).OnFabricatorOpen();
		}
	}

	// Token: 0x06002937 RID: 10551 RVA: 0x0009B806 File Offset: 0x00099A06
	public void OnBuilderShopOpen()
	{
		if (this.currTracker is TutorialDirector.BuilderShopTutorial)
		{
			((TutorialDirector.BuilderShopTutorial)this.currTracker).OnBuilderShopOpen();
		}
	}

	// Token: 0x06002938 RID: 10552 RVA: 0x0009B825 File Offset: 0x00099A25
	public void OnPlaceGadgetOpen()
	{
		if (this.currTracker is TutorialDirector.PlaceGadgetTutorial)
		{
			((TutorialDirector.PlaceGadgetTutorial)this.currTracker).OnPlaceGadgetOpen();
		}
	}

	// Token: 0x06002939 RID: 10553 RVA: 0x0009B844 File Offset: 0x00099A44
	public void OnMailRead(MailDirector.Mail mail)
	{
		if (mail.key == "ogden_invite")
		{
			this.MaybeShowPopup(TutorialDirector.Id.ENTER_ZONE_OGDEN_RANCH);
			return;
		}
		if (mail.key == "mochi_invite")
		{
			this.MaybeShowPopup(TutorialDirector.Id.ENTER_ZONE_MOCHI_RANCH);
			return;
		}
		if (mail.key == "viktor_invite")
		{
			this.MaybeShowPopup(TutorialDirector.Id.ENTER_ZONE_VIKTOR_LAB);
		}
	}

	// Token: 0x0600293A RID: 10554 RVA: 0x0009B8A1 File Offset: 0x00099AA1
	public void OnQuicksilverRaceActivated()
	{
		if (this.currTracker is TutorialDirector.QuicksilverRaceTutorial)
		{
			((TutorialDirector.QuicksilverRaceTutorial)this.currTracker).OnQuicksilverRaceActivated();
		}
	}

	// Token: 0x0600293B RID: 10555 RVA: 0x0009B8C0 File Offset: 0x00099AC0
	public void SuppressTutorials()
	{
		this.suppressors++;
	}

	// Token: 0x0600293C RID: 10556 RVA: 0x0009B8D0 File Offset: 0x00099AD0
	public void UnsuppressTutorials()
	{
		this.suppressors--;
		if (this.suppressors <= 0)
		{
			this.MaybePopupNext();
		}
	}

	// Token: 0x0600293D RID: 10557 RVA: 0x0009B8F0 File Offset: 0x00099AF0
	private void InitTrackers()
	{
		this.trackers.Clear();
		this.trackers[TutorialDirector.Id.BASIC_MOVEMENT] = new TutorialDirector.MoveTutorial();
		this.trackers[TutorialDirector.Id.JUMPING] = new TutorialDirector.JumpTutorial();
		this.trackers[TutorialDirector.Id.VACCING] = new TutorialDirector.VacTutorial();
		this.trackers[TutorialDirector.Id.SHOOTING] = new TutorialDirector.SlimeShootingTutorial();
		this.trackers[TutorialDirector.Id.FOOD] = new TutorialDirector.WaitForShotTutorial(Identifiable.FOOD_CLASS);
		this.trackers[TutorialDirector.Id.PLORT] = new TutorialDirector.PlortTutorial(this);
		this.trackers[TutorialDirector.Id.MARKET] = new TutorialDirector.MarketTutorial();
		this.trackers[TutorialDirector.Id.DEATH] = new TutorialDirector.TimeOnlyTutorial(20f);
		this.trackers[TutorialDirector.Id.GARDEN] = new TutorialDirector.GardenTutorial();
		this.trackers[TutorialDirector.Id.EXPLORE] = new TutorialDirector.TimeOnlyTutorial(20f);
		this.trackers[TutorialDirector.Id.LARGO] = new TutorialDirector.TimeOnlyTutorial(20f);
		this.trackers[TutorialDirector.Id.SCIENCE_BARN] = new TutorialDirector.BarnTutorial(this);
		this.trackers[TutorialDirector.Id.SCIENCE_SUMMARY] = new TutorialDirector.TimeOnlyTutorial(20f);
		this.trackers[TutorialDirector.Id.SCIENCE_REFINERY] = new TutorialDirector.RefineryTutorial();
		this.trackers[TutorialDirector.Id.SCIENCE_BUILDER_SHOP] = new TutorialDirector.BuilderShopTutorial();
		this.trackers[TutorialDirector.Id.SCIENCE_FABRICATOR] = new TutorialDirector.FabricatorTutorial();
		this.trackers[TutorialDirector.Id.SCIENCE_GADGET_MODE] = new TutorialDirector.GadgetModeTutorial();
		this.trackers[TutorialDirector.Id.SCIENCE_SITES] = new TutorialDirector.TimeOnlyTutorial(20f);
		this.trackers[TutorialDirector.Id.SCIENCE_PLACE_SITE] = new TutorialDirector.PlaceGadgetTutorial();
		this.trackers[TutorialDirector.Id.SCIENCE_WRAPUP] = new TutorialDirector.TimeOnlyTutorial(20f);
		this.trackers[TutorialDirector.Id.MAP] = new TutorialDirector.MapTutorial(20f);
		this.trackers[TutorialDirector.Id.WATER] = new TutorialDirector.TimeOnlyTutorial(20f);
		this.trackers[TutorialDirector.Id.ENTER_ZONE_OGDEN_RANCH] = new TutorialDirector.HasProgressTutorial(TutorialDirector.Id.ENTER_ZONE_OGDEN_RANCH, ProgressDirector.ProgressType.ENTER_ZONE_OGDEN_RANCH);
		this.trackers[TutorialDirector.Id.WILDS_SLIMEPEDIA] = new TutorialDirector.TimeOnlyTutorial(10f);
		this.trackers[TutorialDirector.Id.ENTER_ZONE_MOCHI_RANCH] = new TutorialDirector.HasProgressTutorial(TutorialDirector.Id.ENTER_ZONE_MOCHI_RANCH, ProgressDirector.ProgressType.ENTER_ZONE_MOCHI_RANCH);
		this.trackers[TutorialDirector.Id.VALLEY_SLIMEPEDIA] = new TutorialDirector.TimeOnlyTutorial(10f);
		this.trackers[TutorialDirector.Id.RACE_START] = new TutorialDirector.QuicksilverRaceTutorial();
		this.trackers[TutorialDirector.Id.RACE_GENERATOR] = new TutorialDirector.TimeOnlyTutorial(10f);
		this.trackers[TutorialDirector.Id.RACE_CHECKPOINT] = new TutorialDirector.TimeOnlyTutorial(10f);
		this.trackers[TutorialDirector.Id.RACE_PULSESHOT] = new TutorialDirector.TimeOnlyTutorial(10f);
		this.trackers[TutorialDirector.Id.RACE_POWERUP] = new TutorialDirector.TimeOnlyTutorial(10f);
		this.trackers[TutorialDirector.Id.RACE_ENERGYBOOST] = new TutorialDirector.TimeOnlyTutorial(10f);
		this.trackers[TutorialDirector.Id.ENTER_ZONE_VIKTOR_LAB] = new TutorialDirector.HasProgressTutorial(TutorialDirector.Id.ENTER_ZONE_VIKTOR_LAB, ProgressDirector.ProgressType.ENTER_ZONE_VIKTOR_LAB);
		this.trackers[TutorialDirector.Id.SLIMULATIONS_SLIMEPEDIA] = new TutorialDirector.TimeOnlyTutorial(10f);
		this.trackers[TutorialDirector.Id.SLIMULATIONS_START_1] = new TutorialDirector.TimeOnlyTutorial(10f);
		this.trackers[TutorialDirector.Id.SLIMULATIONS_START_2] = new TutorialDirector.TimeOnlyTutorial(10f);
		this.trackers[TutorialDirector.Id.SLIMULATIONS_DEBUG_SPRAY] = new TutorialDirector.TimeOnlyTutorial(10f);
		this.trackers[TutorialDirector.Id.SLIMULATIONS_DAMAGE] = new TutorialDirector.TimeOnlyTutorial(10f);
		this.trackers[TutorialDirector.Id.SLIMULATIONS_EXIT_AVAILABLE] = new TutorialDirector.TimeOnlyTutorial(10f);
		this.trackers[TutorialDirector.Id.MODE_TIME_LIMIT] = new TutorialDirector.TimeOnlyTutorial(20f);
	}

	// Token: 0x0600293E RID: 10558 RVA: 0x0009BC5C File Offset: 0x00099E5C
	private void InitDependencies()
	{
		this.dependencies.Clear();
		this.dependencies[TutorialDirector.Id.MARKET] = new TutorialDirector.Id[]
		{
			TutorialDirector.Id.PLORT
		};
		this.dependencies[TutorialDirector.Id.SCIENCE_SUMMARY] = new TutorialDirector.Id[]
		{
			TutorialDirector.Id.SCIENCE_BARN
		};
		this.dependencies[TutorialDirector.Id.SCIENCE_REFINERY] = new TutorialDirector.Id[]
		{
			TutorialDirector.Id.SCIENCE_BARN
		};
		this.dependencies[TutorialDirector.Id.SCIENCE_BUILDER_SHOP] = new TutorialDirector.Id[]
		{
			TutorialDirector.Id.SCIENCE_BARN
		};
		this.dependencies[TutorialDirector.Id.SCIENCE_FABRICATOR] = new TutorialDirector.Id[]
		{
			TutorialDirector.Id.SCIENCE_BARN
		};
		this.dependencies[TutorialDirector.Id.SCIENCE_GADGET_MODE] = new TutorialDirector.Id[]
		{
			TutorialDirector.Id.SCIENCE_BARN
		};
		this.dependencies[TutorialDirector.Id.SCIENCE_SITES] = new TutorialDirector.Id[]
		{
			TutorialDirector.Id.SCIENCE_BARN
		};
		this.dependencies[TutorialDirector.Id.SCIENCE_PLACE_SITE] = new TutorialDirector.Id[]
		{
			TutorialDirector.Id.SCIENCE_BARN
		};
	}

	// Token: 0x0600293F RID: 10559 RVA: 0x0009BD32 File Offset: 0x00099F32
	public void MaybeShowStatusTutorials()
	{
		if (!this.IsCompleted(TutorialDirector.Id.VACCING))
		{
			this.MaybeShowInitTutorial();
			return;
		}
		if (!this.IsCompleted(TutorialDirector.Id.SCIENCE_BARN))
		{
			this.MaybeShowScienceTutorial();
		}
	}

	// Token: 0x06002940 RID: 10560 RVA: 0x0009BD54 File Offset: 0x00099F54
	private void MaybeShowInitTutorial()
	{
		if (!Levels.isSpecial())
		{
			this.MaybeShowPopup(TutorialDirector.Id.BASIC_MOVEMENT);
			this.MaybeShowPopup(TutorialDirector.Id.JUMPING);
			this.MaybeShowPopup(TutorialDirector.Id.VACCING);
			if (SRSingleton<SceneContext>.Instance.GameModel.currGameMode == PlayerState.GameMode.TIME_LIMIT_V2)
			{
				this.MaybeShowPopup(TutorialDirector.Id.MODE_TIME_LIMIT);
			}
		}
	}

	// Token: 0x06002941 RID: 10561 RVA: 0x0009BD8C File Offset: 0x00099F8C
	private void MaybeShowScienceTutorial()
	{
		if (this.progressDir.HasProgress(ProgressDirector.ProgressType.UNLOCK_LAB))
		{
			this.MaybeShowPopup(TutorialDirector.Id.SCIENCE_BARN);
		}
	}

	// Token: 0x06002942 RID: 10562 RVA: 0x0009BDA4 File Offset: 0x00099FA4
	public TutorialDirector.IdEntry Get(TutorialDirector.Id id)
	{
		return this.entryDict[id];
	}

	// Token: 0x06002943 RID: 10563 RVA: 0x0009BDB4 File Offset: 0x00099FB4
	public List<TutorialDirector.Id> GetPopupQueue()
	{
		List<TutorialDirector.Id> list = new List<TutorialDirector.Id>(this.popupQueue.Values);
		if (this.currTracker != null)
		{
			foreach (KeyValuePair<TutorialDirector.Id, TutorialDirector.Tutorial> keyValuePair in this.trackers)
			{
				if (this.currTracker == keyValuePair.Value)
				{
					list.Insert(0, keyValuePair.Key);
					break;
				}
			}
		}
		return list;
	}

	// Token: 0x06002944 RID: 10564 RVA: 0x0009BE3C File Offset: 0x0009A03C
	public void SetPopupQueue(IEnumerable<TutorialDirector.Id> ids)
	{
		base.StartCoroutine(this.SetPopupQueueCoroutine(ids));
	}

	// Token: 0x06002945 RID: 10565 RVA: 0x0009BE4C File Offset: 0x0009A04C
	private IEnumerator SetPopupQueueCoroutine(IEnumerable<TutorialDirector.Id> ids)
	{
		yield return new WaitForEndOfFrame();
		using (IEnumerator<TutorialDirector.Id> enumerator = ids.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				TutorialDirector.Id id = enumerator.Current;
				this.MaybeShowPopup(id);
			}
			yield break;
		}
		yield break;
	}

	// Token: 0x06002946 RID: 10566 RVA: 0x0009BE64 File Offset: 0x0009A064
	private int Priority(TutorialDirector.Id id)
	{
		int num = Array.IndexOf<TutorialDirector.Id>(TutorialDirector.ID_PRIORITIES, id);
		if (num == -1)
		{
			return int.MaxValue;
		}
		return num;
	}

	// Token: 0x06002947 RID: 10567 RVA: 0x0009BE88 File Offset: 0x0009A088
	public void MaybeShowPopup(TutorialDirector.Id id)
	{
		if (this.ShouldPopupDisplay(id))
		{
			this.popupQueue.Add(this.Priority(id), id);
			this.MaybePopupNext();
		}
	}

	// Token: 0x06002948 RID: 10568 RVA: 0x0009BEAC File Offset: 0x0009A0AC
	private bool ShouldPopupDisplay(TutorialDirector.Id id)
	{
		return !Levels.isSpecial() && this.IsEnabled(id) && (!SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().assumeExperiencedUser || TutorialDirector.EXPERIENCED_TUTS.Contains(id)) && !this.IsCompleted(id) && !this.IsHidden(id) && !this.popupQueue.ContainsValue(id) && (this.currPopup == null || this.currPopup.GetId() != id) && this.DependenciesSatisfied(id);
	}

	// Token: 0x06002949 RID: 10569 RVA: 0x0009BF31 File Offset: 0x0009A131
	public bool IsCompleted(TutorialDirector.Id tut)
	{
		return this.tutModel.completedIds.Contains(tut);
	}

	// Token: 0x0600294A RID: 10570 RVA: 0x0009BF44 File Offset: 0x0009A144
	public bool IsEnabled(TutorialDirector.Id id)
	{
		OptionsDirector.EnabledTutorials enabledTutorials = this.optionsDir.enabledTutorials;
		if (enabledTutorials != OptionsDirector.EnabledTutorials.ADVANCED_ONLY)
		{
			return enabledTutorials != OptionsDirector.EnabledTutorials.NONE || TutorialDirector.FORCED_TUTS.Contains(id);
		}
		return TutorialDirector.ADVANCED_TUTS.Contains(id) || TutorialDirector.FORCED_TUTS.Contains(id);
	}

	// Token: 0x0600294B RID: 10571 RVA: 0x0009BF8F File Offset: 0x0009A18F
	public bool IsCompletedOrDisabled(TutorialDirector.Id id)
	{
		return this.IsCompleted(id) || !this.IsEnabled(id);
	}

	// Token: 0x0600294C RID: 10572 RVA: 0x0009BFA6 File Offset: 0x0009A1A6
	private bool IsHidden(TutorialDirector.Id tut)
	{
		return this.hidden.Contains(tut);
	}

	// Token: 0x0600294D RID: 10573 RVA: 0x0009BFB4 File Offset: 0x0009A1B4
	public void OnApplicationQuit()
	{
		this.quitting = true;
	}

	// Token: 0x0600294E RID: 10574 RVA: 0x0009BFC0 File Offset: 0x0009A1C0
	public void PopupDeactivated(TutorialPopupUI popup, bool doComplete)
	{
		if (this.currPopup == popup && !this.quitting)
		{
			if (doComplete)
			{
				this.Complete(this.currPopup.GetId());
			}
			else
			{
				this.Hide(popup.GetId());
			}
			this.currPopup = null;
			this.currTracker.End();
			this.currTracker = null;
			this.timeDir.OnUnpause(new TimeDirector.OnUnpauseDelegate(this.OnUnpause));
			return;
		}
		Log.Warning("Popup deactivated, but wasn't current popup.", Array.Empty<object>());
	}

	// Token: 0x0600294F RID: 10575 RVA: 0x0009C045 File Offset: 0x0009A245
	private void Hide(TutorialDirector.Id id)
	{
		this.hidden.Add(id);
	}

	// Token: 0x06002950 RID: 10576 RVA: 0x0009C054 File Offset: 0x0009A254
	public void OnDestroy()
	{
		this.timeDir.ClearOnUnpause(new TimeDirector.OnUnpauseDelegate(this.OnUnpause));
	}

	// Token: 0x06002951 RID: 10577 RVA: 0x0009C06D File Offset: 0x0009A26D
	public void OnUnpause()
	{
		this.MaybeShowStatusTutorials();
		this.MaybePopupNext();
	}

	// Token: 0x06002952 RID: 10578 RVA: 0x0009C07C File Offset: 0x0009A27C
	private bool DependenciesSatisfied(TutorialDirector.Id id)
	{
		if (this.dependencies.ContainsKey(id))
		{
			foreach (TutorialDirector.Id id2 in this.dependencies[id])
			{
				if (!this.IsCompleted(id2) && (this.currPopup == null || this.currPopup.GetId() != id2))
				{
					return false;
				}
			}
			return true;
		}
		return true;
	}

	// Token: 0x06002953 RID: 10579 RVA: 0x0009C0E0 File Offset: 0x0009A2E0
	private void MaybePopupNext()
	{
		if (SRSingleton<SceneContext>.Instance != null && this.popupQueue.Count > 0 && this.currPopup == null && this.suppressors <= 0)
		{
			TutorialDirector.Id id = this.popupQueue.Values[0];
			this.popupQueue.RemoveAt(0);
			if (this.ShouldPopupDisplay(id))
			{
				if (this.trackers[id].HideInsteadOfPopup())
				{
					this.Hide(id);
					return;
				}
				GameObject gameObject = TutorialPopupUI.CreateTutorialPopup(this.Get(id));
				this.currPopup = gameObject.GetComponent<TutorialPopupUI>();
				this.currTracker = this.trackers[id];
				this.currTracker.Start();
			}
		}
	}

	// Token: 0x06002954 RID: 10580 RVA: 0x0009C1A0 File Offset: 0x0009A3A0
	public bool MarkTutorialCompleted(TutorialDirector.Id id)
	{
		if (this.tutModel.completedIds.Add(id))
		{
			AnalyticsUtil.CustomEvent("TutorialComplete", new Dictionary<string, object>
			{
				{
					"id",
					id.ToString()
				}
			}, true);
			return true;
		}
		return false;
	}

	// Token: 0x06002955 RID: 10581 RVA: 0x0009C1E0 File Offset: 0x0009A3E0
	private void Complete(TutorialDirector.Id id)
	{
		if (this.MarkTutorialCompleted(id))
		{
			bool flag = true;
			foreach (TutorialDirector.Id item in TutorialDirector.PRE_EXPLORE_TUTS)
			{
				if (!this.tutModel.completedIds.Contains(item))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				this.MaybeShowPopup(TutorialDirector.Id.EXPLORE);
			}
			bool flag2 = true;
			foreach (TutorialDirector.Id item2 in TutorialDirector.PRE_SCIENCE_WRAPUP_TUTS)
			{
				if (!this.tutModel.completedIds.Contains(item2))
				{
					flag2 = false;
					break;
				}
			}
			if (flag2)
			{
				this.MaybeShowPopup(TutorialDirector.Id.SCIENCE_WRAPUP);
			}
		}
	}

	// Token: 0x04002891 RID: 10385
	private static readonly TutorialDirector.Id[] ID_PRIORITIES = new TutorialDirector.Id[]
	{
		TutorialDirector.Id.DEATH,
		TutorialDirector.Id.BASIC_MOVEMENT,
		TutorialDirector.Id.JUMPING,
		TutorialDirector.Id.VACCING,
		TutorialDirector.Id.SHOOTING,
		TutorialDirector.Id.FOOD,
		TutorialDirector.Id.PLORT,
		TutorialDirector.Id.MARKET,
		TutorialDirector.Id.GARDEN,
		TutorialDirector.Id.EXPLORE,
		TutorialDirector.Id.SCIENCE_BARN,
		TutorialDirector.Id.SCIENCE_SUMMARY,
		TutorialDirector.Id.SCIENCE_REFINERY,
		TutorialDirector.Id.SCIENCE_BUILDER_SHOP,
		TutorialDirector.Id.SCIENCE_FABRICATOR,
		TutorialDirector.Id.SCIENCE_GADGET_MODE,
		TutorialDirector.Id.SCIENCE_SITES,
		TutorialDirector.Id.SCIENCE_PLACE_SITE,
		TutorialDirector.Id.SCIENCE_WRAPUP,
		TutorialDirector.Id.LARGO,
		TutorialDirector.Id.MAP,
		TutorialDirector.Id.WATER,
		TutorialDirector.Id.MODE_TIME_LIMIT,
		TutorialDirector.Id.ENTER_ZONE_OGDEN_RANCH,
		TutorialDirector.Id.WILDS_SLIMEPEDIA,
		TutorialDirector.Id.ENTER_ZONE_MOCHI_RANCH,
		TutorialDirector.Id.VALLEY_SLIMEPEDIA,
		TutorialDirector.Id.RACE_START,
		TutorialDirector.Id.RACE_GENERATOR,
		TutorialDirector.Id.RACE_CHECKPOINT,
		TutorialDirector.Id.RACE_PULSESHOT,
		TutorialDirector.Id.RACE_POWERUP,
		TutorialDirector.Id.RACE_ENERGYBOOST,
		TutorialDirector.Id.ENTER_ZONE_VIKTOR_LAB,
		TutorialDirector.Id.SLIMULATIONS_SLIMEPEDIA,
		TutorialDirector.Id.SLIMULATIONS_START_1,
		TutorialDirector.Id.SLIMULATIONS_START_2,
		TutorialDirector.Id.SLIMULATIONS_DEBUG_SPRAY,
		TutorialDirector.Id.SLIMULATIONS_DAMAGE,
		TutorialDirector.Id.SLIMULATIONS_EXIT_AVAILABLE,
		TutorialDirector.Id.APPEARANCE_UI
	};

	// Token: 0x04002892 RID: 10386
	private static readonly TutorialDirector.Id[] PRE_EXPLORE_TUTS = new TutorialDirector.Id[]
	{
		TutorialDirector.Id.BASIC_MOVEMENT,
		TutorialDirector.Id.JUMPING,
		TutorialDirector.Id.VACCING,
		TutorialDirector.Id.SHOOTING,
		TutorialDirector.Id.FOOD,
		TutorialDirector.Id.PLORT,
		TutorialDirector.Id.MARKET
	};

	// Token: 0x04002893 RID: 10387
	private static readonly TutorialDirector.Id[] PRE_SCIENCE_WRAPUP_TUTS = new TutorialDirector.Id[]
	{
		TutorialDirector.Id.SCIENCE_BARN,
		TutorialDirector.Id.SCIENCE_SUMMARY,
		TutorialDirector.Id.SCIENCE_REFINERY,
		TutorialDirector.Id.SCIENCE_BUILDER_SHOP,
		TutorialDirector.Id.SCIENCE_FABRICATOR,
		TutorialDirector.Id.SCIENCE_GADGET_MODE,
		TutorialDirector.Id.SCIENCE_SITES,
		TutorialDirector.Id.SCIENCE_PLACE_SITE
	};

	// Token: 0x04002894 RID: 10388
	private static HashSet<TutorialDirector.Id> EXPERIENCED_TUTS = new HashSet<TutorialDirector.Id>(TutorialDirector.IdComparer.Instance)
	{
		TutorialDirector.Id.MODE_TIME_LIMIT
	};

	// Token: 0x04002895 RID: 10389
	private static HashSet<TutorialDirector.Id> ADVANCED_TUTS = new HashSet<TutorialDirector.Id>(TutorialDirector.IdComparer.Instance)
	{
		TutorialDirector.Id.SCIENCE_BARN,
		TutorialDirector.Id.SCIENCE_SUMMARY,
		TutorialDirector.Id.SCIENCE_REFINERY,
		TutorialDirector.Id.SCIENCE_BUILDER_SHOP,
		TutorialDirector.Id.SCIENCE_FABRICATOR,
		TutorialDirector.Id.SCIENCE_GADGET_MODE,
		TutorialDirector.Id.SCIENCE_SITES,
		TutorialDirector.Id.SCIENCE_PLACE_SITE,
		TutorialDirector.Id.SCIENCE_WRAPUP,
		TutorialDirector.Id.ENTER_ZONE_OGDEN_RANCH,
		TutorialDirector.Id.WILDS_SLIMEPEDIA,
		TutorialDirector.Id.ENTER_ZONE_MOCHI_RANCH,
		TutorialDirector.Id.VALLEY_SLIMEPEDIA,
		TutorialDirector.Id.RACE_START,
		TutorialDirector.Id.RACE_GENERATOR,
		TutorialDirector.Id.RACE_CHECKPOINT,
		TutorialDirector.Id.RACE_PULSESHOT,
		TutorialDirector.Id.RACE_POWERUP,
		TutorialDirector.Id.RACE_ENERGYBOOST,
		TutorialDirector.Id.ENTER_ZONE_VIKTOR_LAB,
		TutorialDirector.Id.SLIMULATIONS_SLIMEPEDIA,
		TutorialDirector.Id.SLIMULATIONS_START_1,
		TutorialDirector.Id.SLIMULATIONS_START_2,
		TutorialDirector.Id.SLIMULATIONS_DEBUG_SPRAY,
		TutorialDirector.Id.SLIMULATIONS_DAMAGE,
		TutorialDirector.Id.SLIMULATIONS_EXIT_AVAILABLE
	};

	// Token: 0x04002896 RID: 10390
	private static HashSet<TutorialDirector.Id> FORCED_TUTS = new HashSet<TutorialDirector.Id>(TutorialDirector.IdComparer.Instance)
	{
		TutorialDirector.Id.APPEARANCE_UI
	};

	// Token: 0x04002897 RID: 10391
	private const float CLOSE_AFTER_ACTION_DELAY = 6f;

	// Token: 0x04002898 RID: 10392
	private const float WAIT_FOR_SHORT_READ_DELAY = 10f;

	// Token: 0x04002899 RID: 10393
	private const float WAIT_FOR_READ_DELAY = 20f;

	// Token: 0x0400289A RID: 10394
	private Dictionary<TutorialDirector.Id, TutorialDirector.Tutorial> trackers = new Dictionary<TutorialDirector.Id, TutorialDirector.Tutorial>();

	// Token: 0x0400289B RID: 10395
	private Dictionary<TutorialDirector.Id, TutorialDirector.Id[]> dependencies = new Dictionary<TutorialDirector.Id, TutorialDirector.Id[]>();

	// Token: 0x0400289C RID: 10396
	public GameObject tutorialPopupPrefab;

	// Token: 0x0400289D RID: 10397
	public TutorialDirector.IdEntry[] entries;

	// Token: 0x0400289E RID: 10398
	private Dictionary<TutorialDirector.Id, TutorialDirector.IdEntry> entryDict = new Dictionary<TutorialDirector.Id, TutorialDirector.IdEntry>();

	// Token: 0x0400289F RID: 10399
	private HashSet<TutorialDirector.Id> hidden = new HashSet<TutorialDirector.Id>();

	// Token: 0x040028A0 RID: 10400
	private bool quitting;

	// Token: 0x040028A1 RID: 10401
	private TutorialPopupUI currPopup;

	// Token: 0x040028A2 RID: 10402
	private TutorialDirector.Tutorial currTracker;

	// Token: 0x040028A3 RID: 10403
	private SortedList<int, TutorialDirector.Id> popupQueue = new SortedList<int, TutorialDirector.Id>();

	// Token: 0x040028A4 RID: 10404
	private TimeDirector timeDir;

	// Token: 0x040028A5 RID: 10405
	private OptionsDirector optionsDir;

	// Token: 0x040028A6 RID: 10406
	private ProgressDirector progressDir;

	// Token: 0x040028A7 RID: 10407
	private bool inMarketArea;

	// Token: 0x040028A8 RID: 10408
	private bool inBarnArea;

	// Token: 0x040028A9 RID: 10409
	private int suppressors;

	// Token: 0x040028AA RID: 10410
	private TutorialsModel tutModel;

	// Token: 0x020007AC RID: 1964
	public enum Id
	{
		// Token: 0x040028AC RID: 10412
		BASIC_MOVEMENT,
		// Token: 0x040028AD RID: 10413
		JUMPING,
		// Token: 0x040028AE RID: 10414
		VACCING,
		// Token: 0x040028AF RID: 10415
		SHOOTING,
		// Token: 0x040028B0 RID: 10416
		FOOD,
		// Token: 0x040028B1 RID: 10417
		PLORT,
		// Token: 0x040028B2 RID: 10418
		MARKET,
		// Token: 0x040028B3 RID: 10419
		DEATH,
		// Token: 0x040028B4 RID: 10420
		GARDEN,
		// Token: 0x040028B5 RID: 10421
		EXPLORE,
		// Token: 0x040028B6 RID: 10422
		LARGO,
		// Token: 0x040028B7 RID: 10423
		SCIENCE_BARN,
		// Token: 0x040028B8 RID: 10424
		SCIENCE_SUMMARY,
		// Token: 0x040028B9 RID: 10425
		SCIENCE_REFINERY,
		// Token: 0x040028BA RID: 10426
		SCIENCE_BUILDER_SHOP,
		// Token: 0x040028BB RID: 10427
		SCIENCE_FABRICATOR,
		// Token: 0x040028BC RID: 10428
		SCIENCE_GADGET_MODE,
		// Token: 0x040028BD RID: 10429
		SCIENCE_SITES,
		// Token: 0x040028BE RID: 10430
		SCIENCE_PLACE_SITE,
		// Token: 0x040028BF RID: 10431
		SCIENCE_WRAPUP,
		// Token: 0x040028C0 RID: 10432
		MAP,
		// Token: 0x040028C1 RID: 10433
		WATER,
		// Token: 0x040028C2 RID: 10434
		ENTER_ZONE_OGDEN_RANCH,
		// Token: 0x040028C3 RID: 10435
		ENTER_ZONE_MOCHI_RANCH,
		// Token: 0x040028C4 RID: 10436
		RACE_START,
		// Token: 0x040028C5 RID: 10437
		RACE_GENERATOR,
		// Token: 0x040028C6 RID: 10438
		RACE_CHECKPOINT,
		// Token: 0x040028C7 RID: 10439
		RACE_PULSESHOT,
		// Token: 0x040028C8 RID: 10440
		RACE_POWERUP,
		// Token: 0x040028C9 RID: 10441
		RACE_ENERGYBOOST,
		// Token: 0x040028CA RID: 10442
		MODE_TIME_LIMIT,
		// Token: 0x040028CB RID: 10443
		ENTER_ZONE_VIKTOR_LAB,
		// Token: 0x040028CC RID: 10444
		SLIMULATIONS_SLIMEPEDIA,
		// Token: 0x040028CD RID: 10445
		SLIMULATIONS_START_1,
		// Token: 0x040028CE RID: 10446
		SLIMULATIONS_START_2,
		// Token: 0x040028CF RID: 10447
		SLIMULATIONS_DEBUG_SPRAY,
		// Token: 0x040028D0 RID: 10448
		SLIMULATIONS_DAMAGE,
		// Token: 0x040028D1 RID: 10449
		SLIMULATIONS_EXIT_AVAILABLE,
		// Token: 0x040028D2 RID: 10450
		WILDS_SLIMEPEDIA,
		// Token: 0x040028D3 RID: 10451
		VALLEY_SLIMEPEDIA,
		// Token: 0x040028D4 RID: 10452
		APPEARANCE_UI
	}

	// Token: 0x020007AD RID: 1965
	public class IdComparer : IEqualityComparer<TutorialDirector.Id>
	{
		// Token: 0x06002959 RID: 10585 RVA: 0x00017781 File Offset: 0x00015981
		public bool Equals(TutorialDirector.Id a, TutorialDirector.Id b)
		{
			return a == b;
		}

		// Token: 0x0600295A RID: 10586 RVA: 0x00017787 File Offset: 0x00015987
		public int GetHashCode(TutorialDirector.Id a)
		{
			return (int)a;
		}

		// Token: 0x040028D5 RID: 10453
		public static TutorialDirector.IdComparer Instance = new TutorialDirector.IdComparer();
	}

	// Token: 0x020007AE RID: 1966
	[Serializable]
	public class IdEntry
	{
		// Token: 0x040028D6 RID: 10454
		public TutorialDirector.Id id;

		// Token: 0x040028D7 RID: 10455
		public Sprite[] images;

		// Token: 0x040028D8 RID: 10456
		[Tooltip("Additional x/y offset added to the image's initial position.")]
		public Vector3 imageOffset = Vector3.zero;

		// Token: 0x040028D9 RID: 10457
		[Tooltip("Absolute scale of the image.")]
		public Vector3 imageScale = Vector3.one;
	}

	// Token: 0x020007AF RID: 1967
	private interface Tutorial
	{
		// Token: 0x0600295E RID: 10590
		void Start();

		// Token: 0x0600295F RID: 10591
		void Update();

		// Token: 0x06002960 RID: 10592
		bool ShouldEnd();

		// Token: 0x06002961 RID: 10593
		bool ShouldHide();

		// Token: 0x06002962 RID: 10594
		bool HideInsteadOfPopup();

		// Token: 0x06002963 RID: 10595
		bool MarkCompleted();

		// Token: 0x06002964 RID: 10596
		void End();

		// Token: 0x06002965 RID: 10597
		void OnShot(Identifiable.Id id);
	}

	// Token: 0x020007B0 RID: 1968
	private class BaseTutorial : TutorialDirector.Tutorial
	{
		// Token: 0x06002966 RID: 10598 RVA: 0x00003296 File Offset: 0x00001496
		public virtual void Start()
		{
		}

		// Token: 0x06002967 RID: 10599 RVA: 0x00003296 File Offset: 0x00001496
		public virtual void Update()
		{
		}

		// Token: 0x06002968 RID: 10600 RVA: 0x0009C45F File Offset: 0x0009A65F
		public virtual bool ShouldEnd()
		{
			return Time.time >= this.endTime;
		}

		// Token: 0x06002969 RID: 10601 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
		public virtual bool ShouldHide()
		{
			return false;
		}

		// Token: 0x0600296A RID: 10602 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
		public virtual bool HideInsteadOfPopup()
		{
			return false;
		}

		// Token: 0x0600296B RID: 10603 RVA: 0x0009C471 File Offset: 0x0009A671
		public virtual bool MarkCompleted()
		{
			return !float.IsPositiveInfinity(this.endTime);
		}

		// Token: 0x0600296C RID: 10604 RVA: 0x00003296 File Offset: 0x00001496
		public virtual void End()
		{
		}

		// Token: 0x0600296D RID: 10605 RVA: 0x00003296 File Offset: 0x00001496
		public virtual void OnShot(Identifiable.Id id)
		{
		}

		// Token: 0x040028DA RID: 10458
		protected float endTime = float.PositiveInfinity;
	}

	// Token: 0x020007B1 RID: 1969
	private class RanchOnlyTutorial : TutorialDirector.BaseTutorial
	{
		// Token: 0x0600296F RID: 10607 RVA: 0x0009C494 File Offset: 0x0009A694
		public override void Start()
		{
			if (!this.HideInsteadOfPopup())
			{
				this.hideTime = float.PositiveInfinity;
				return;
			}
			this.hideTime = Time.time + 10f;
		}

		// Token: 0x06002970 RID: 10608 RVA: 0x0009C4BB File Offset: 0x0009A6BB
		public override bool ShouldHide()
		{
			return Time.time >= this.hideTime;
		}

		// Token: 0x06002971 RID: 10609 RVA: 0x0009C4CD File Offset: 0x0009A6CD
		public override bool HideInsteadOfPopup()
		{
			return !CellDirector.IsOnHomeRanch(SRSingleton<SceneContext>.Instance.Player.GetComponent<RegionMember>());
		}

		// Token: 0x06002972 RID: 10610 RVA: 0x0009C4E6 File Offset: 0x0009A6E6
		public void LeftRanch()
		{
			this.hideTime = Time.time + 10f;
		}

		// Token: 0x06002973 RID: 10611 RVA: 0x0009C4F9 File Offset: 0x0009A6F9
		public void EnteredRanch()
		{
			this.hideTime = float.PositiveInfinity;
		}

		// Token: 0x040028DB RID: 10459
		private float hideTime = float.PositiveInfinity;

		// Token: 0x040028DC RID: 10460
		private const float HIDE_DELAY = 10f;
	}

	// Token: 0x020007B2 RID: 1970
	private class WaitForShotTutorial : TutorialDirector.BaseTutorial
	{
		// Token: 0x06002975 RID: 10613 RVA: 0x0009C519 File Offset: 0x0009A719
		public WaitForShotTutorial(HashSet<Identifiable.Id> ids)
		{
			this.waitForIds = ids;
		}

		// Token: 0x06002976 RID: 10614 RVA: 0x0009C528 File Offset: 0x0009A728
		public override void OnShot(Identifiable.Id id)
		{
			if (this.waitForIds.Contains(id))
			{
				this.endTime = Time.time + 6f;
			}
		}

		// Token: 0x040028DD RID: 10461
		private HashSet<Identifiable.Id> waitForIds;
	}

	// Token: 0x020007B3 RID: 1971
	private class TimeOnlyTutorial : TutorialDirector.BaseTutorial
	{
		// Token: 0x06002977 RID: 10615 RVA: 0x0009C549 File Offset: 0x0009A749
		public TimeOnlyTutorial(float timeToLive)
		{
			this.timeToLive = timeToLive;
		}

		// Token: 0x06002978 RID: 10616 RVA: 0x0009C558 File Offset: 0x0009A758
		public override void Start()
		{
			this.endTime = Time.time + this.timeToLive;
		}

		// Token: 0x06002979 RID: 10617 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
		public override bool MarkCompleted()
		{
			return false;
		}

		// Token: 0x040028DE RID: 10462
		private float timeToLive;
	}

	// Token: 0x020007B4 RID: 1972
	private class MoveTutorial : TutorialDirector.BaseTutorial
	{
		// Token: 0x0600297A RID: 10618 RVA: 0x0009C56C File Offset: 0x0009A76C
		public override void Update()
		{
			if (float.IsPositiveInfinity(this.endTime) && (SRInput.Actions.vertical.RawValue != 0f || SRInput.Actions.horizontal.RawValue != 0f))
			{
				this.endTime = Time.time + 6f;
			}
		}
	}

	// Token: 0x020007B5 RID: 1973
	private class JumpTutorial : TutorialDirector.BaseTutorial
	{
		// Token: 0x0600297D RID: 10621 RVA: 0x0009C5CB File Offset: 0x0009A7CB
		public override void Update()
		{
			if (float.IsPositiveInfinity(this.endTime) && SRInput.Actions.jump.IsPressed)
			{
				this.endTime = Time.time + 6f;
			}
		}
	}

	// Token: 0x020007B6 RID: 1974
	private class VacTutorial : TutorialDirector.BaseTutorial
	{
		// Token: 0x0600297F RID: 10623 RVA: 0x0009C5FC File Offset: 0x0009A7FC
		public void OnVac()
		{
			if (float.IsPositiveInfinity(this.endTime))
			{
				this.endTime = Time.time + 6f;
			}
		}
	}

	// Token: 0x020007B7 RID: 1975
	private class MapTutorial : TutorialDirector.BaseTutorial
	{
		// Token: 0x06002980 RID: 10624 RVA: 0x0009C61C File Offset: 0x0009A81C
		public MapTutorial(float tutorialTimeout)
		{
			this.tutorialTimeout = tutorialTimeout;
		}

		// Token: 0x06002981 RID: 10625 RVA: 0x0009C62B File Offset: 0x0009A82B
		public override void Start()
		{
			this.endTime = Time.time + this.tutorialTimeout;
		}

		// Token: 0x06002982 RID: 10626 RVA: 0x0009C63F File Offset: 0x0009A83F
		public override void Update()
		{
			base.Update();
			if (SRInput.Actions.openMap.IsPressed)
			{
				this.mapOpened = true;
				this.endTime = Time.time + 6f;
			}
		}

		// Token: 0x06002983 RID: 10627 RVA: 0x0009C670 File Offset: 0x0009A870
		public override bool MarkCompleted()
		{
			return this.mapOpened;
		}

		// Token: 0x040028DF RID: 10463
		private bool mapOpened;

		// Token: 0x040028E0 RID: 10464
		private float tutorialTimeout;
	}

	// Token: 0x020007B8 RID: 1976
	private abstract class MarkerTutorial : TutorialDirector.RanchOnlyTutorial
	{
		// Token: 0x06002984 RID: 10628 RVA: 0x0009C678 File Offset: 0x0009A878
		public MarkerTutorial(TutorialDirector.Id areaId)
		{
			this.areaId = areaId;
		}

		// Token: 0x06002985 RID: 10629 RVA: 0x0009C688 File Offset: 0x0009A888
		public override void Start()
		{
			base.Start();
			foreach (TutorialRadar tutorialRadar in TutorialRadar.allRadars)
			{
				if (tutorialRadar.tutorialId == this.areaId)
				{
					tutorialRadar.gameObject.GetComponent<RadarTrackedObject>().enabled = true;
				}
			}
		}

		// Token: 0x06002986 RID: 10630 RVA: 0x0009C6F8 File Offset: 0x0009A8F8
		public override void End()
		{
			base.End();
			foreach (TutorialRadar tutorialRadar in TutorialRadar.allRadars)
			{
				if (tutorialRadar.tutorialId == this.areaId)
				{
					tutorialRadar.gameObject.GetComponent<RadarTrackedObject>().enabled = false;
				}
			}
		}

		// Token: 0x040028E1 RID: 10465
		private TutorialDirector.Id areaId;
	}

	// Token: 0x020007B9 RID: 1977
	private class PlortTutorial : TutorialDirector.MarkerTutorial
	{
		// Token: 0x06002987 RID: 10631 RVA: 0x0009C768 File Offset: 0x0009A968
		public PlortTutorial(TutorialDirector dir) : base(TutorialDirector.Id.PLORT)
		{
			this.dir = dir;
		}

		// Token: 0x06002988 RID: 10632 RVA: 0x0009C778 File Offset: 0x0009A978
		public override bool ShouldEnd()
		{
			return this.dir.inMarketArea;
		}

		// Token: 0x040028E2 RID: 10466
		protected TutorialDirector dir;
	}

	// Token: 0x020007BA RID: 1978
	private class BarnTutorial : TutorialDirector.MarkerTutorial
	{
		// Token: 0x06002989 RID: 10633 RVA: 0x0009C785 File Offset: 0x0009A985
		public BarnTutorial(TutorialDirector dir) : base(TutorialDirector.Id.SCIENCE_BARN)
		{
			this.dir = dir;
		}

		// Token: 0x0600298A RID: 10634 RVA: 0x0009C796 File Offset: 0x0009A996
		public override bool ShouldEnd()
		{
			return this.dir.inBarnArea;
		}

		// Token: 0x040028E3 RID: 10467
		protected TutorialDirector dir;
	}

	// Token: 0x020007BB RID: 1979
	private class MarketTutorial : TutorialDirector.RanchOnlyTutorial
	{
		// Token: 0x0600298B RID: 10635 RVA: 0x0009C7A3 File Offset: 0x0009A9A3
		public void OnPlortSold()
		{
			this.endTime = Time.time + 6f;
		}
	}

	// Token: 0x020007BC RID: 1980
	private class RefineryTutorial : TutorialDirector.MarkerTutorial
	{
		// Token: 0x0600298D RID: 10637 RVA: 0x0009C7BE File Offset: 0x0009A9BE
		public RefineryTutorial() : base(TutorialDirector.Id.SCIENCE_REFINERY)
		{
		}

		// Token: 0x0600298E RID: 10638 RVA: 0x0009C7A3 File Offset: 0x0009A9A3
		public void OnRefineryAdded()
		{
			this.endTime = Time.time + 6f;
		}
	}

	// Token: 0x020007BD RID: 1981
	private class GadgetModeTutorial : TutorialDirector.BaseTutorial
	{
		// Token: 0x0600298F RID: 10639 RVA: 0x0009C7A3 File Offset: 0x0009A9A3
		public void OnGadgetModeActivated()
		{
			this.endTime = Time.time + 6f;
		}
	}

	// Token: 0x020007BE RID: 1982
	private class FabricatorTutorial : TutorialDirector.MarkerTutorial
	{
		// Token: 0x06002991 RID: 10641 RVA: 0x0009C7C8 File Offset: 0x0009A9C8
		public FabricatorTutorial() : base(TutorialDirector.Id.SCIENCE_FABRICATOR)
		{
		}

		// Token: 0x06002992 RID: 10642 RVA: 0x0009C7A3 File Offset: 0x0009A9A3
		public void OnFabricatorOpen()
		{
			this.endTime = Time.time + 6f;
		}
	}

	// Token: 0x020007BF RID: 1983
	private class BuilderShopTutorial : TutorialDirector.MarkerTutorial
	{
		// Token: 0x06002993 RID: 10643 RVA: 0x0009C7D2 File Offset: 0x0009A9D2
		public BuilderShopTutorial() : base(TutorialDirector.Id.SCIENCE_BUILDER_SHOP)
		{
		}

		// Token: 0x06002994 RID: 10644 RVA: 0x0009C7A3 File Offset: 0x0009A9A3
		public void OnBuilderShopOpen()
		{
			this.endTime = Time.time + 6f;
		}
	}

	// Token: 0x020007C0 RID: 1984
	private class PlaceGadgetTutorial : TutorialDirector.BaseTutorial
	{
		// Token: 0x06002996 RID: 10646 RVA: 0x0009C7A3 File Offset: 0x0009A9A3
		public void OnPlaceGadgetOpen()
		{
			this.endTime = Time.time + 6f;
		}
	}

	// Token: 0x020007C1 RID: 1985
	private class GardenTutorial : TutorialDirector.RanchOnlyTutorial
	{
		// Token: 0x06002998 RID: 10648 RVA: 0x0009C7DC File Offset: 0x0009A9DC
		public void OnPlanted()
		{
			this.endTime = Time.time + 6f;
			foreach (TutorialRadar tutorialRadar in TutorialRadar.allRadars)
			{
				if (tutorialRadar.tutorialId == TutorialDirector.Id.GARDEN)
				{
					tutorialRadar.gameObject.GetComponent<RadarTrackedObject>().enabled = false;
				}
			}
		}

		// Token: 0x06002999 RID: 10649 RVA: 0x0009C854 File Offset: 0x0009AA54
		public override void Start()
		{
			base.Start();
			foreach (TutorialRadar tutorialRadar in TutorialRadar.allRadars)
			{
				if (tutorialRadar.tutorialId == TutorialDirector.Id.GARDEN)
				{
					tutorialRadar.gameObject.GetComponent<RadarTrackedObject>().enabled = true;
				}
			}
			if (SRSingleton<SceneContext>.Instance.GameModel.AllLandPlots().Values.Any((LandPlotModel p) => p.typeId == LandPlot.Id.GARDEN && p.HasAttached()))
			{
				this.OnPlanted();
			}
		}

		// Token: 0x0600299A RID: 10650 RVA: 0x0009C900 File Offset: 0x0009AB00
		public override void End()
		{
			base.End();
			foreach (TutorialRadar tutorialRadar in TutorialRadar.allRadars)
			{
				if (tutorialRadar.tutorialId == TutorialDirector.Id.GARDEN)
				{
					tutorialRadar.gameObject.GetComponent<RadarTrackedObject>().enabled = false;
				}
			}
		}
	}

	// Token: 0x020007C3 RID: 1987
	private class SlimeShootingTutorial : TutorialDirector.RanchOnlyTutorial
	{
		// Token: 0x0600299F RID: 10655 RVA: 0x0009C7A3 File Offset: 0x0009A9A3
		public void OnDelaunchedSlime()
		{
			this.endTime = Time.time + 6f;
		}

		// Token: 0x060029A0 RID: 10656 RVA: 0x0009C98C File Offset: 0x0009AB8C
		public override void Start()
		{
			base.Start();
			foreach (TutorialRadar tutorialRadar in TutorialRadar.allRadars)
			{
				if (tutorialRadar.tutorialId == TutorialDirector.Id.SHOOTING)
				{
					tutorialRadar.gameObject.GetComponent<RadarTrackedObject>().enabled = true;
				}
			}
		}

		// Token: 0x060029A1 RID: 10657 RVA: 0x0009C9F8 File Offset: 0x0009ABF8
		public override void End()
		{
			base.End();
			foreach (TutorialRadar tutorialRadar in TutorialRadar.allRadars)
			{
				if (tutorialRadar.tutorialId == TutorialDirector.Id.SHOOTING)
				{
					tutorialRadar.gameObject.GetComponent<RadarTrackedObject>().enabled = false;
				}
			}
		}
	}

	// Token: 0x020007C4 RID: 1988
	private class HasProgressTutorial : TutorialDirector.MarkerTutorial
	{
		// Token: 0x060029A2 RID: 10658 RVA: 0x0009CA64 File Offset: 0x0009AC64
		public HasProgressTutorial(TutorialDirector.Id id, ProgressDirector.ProgressType progress) : base(id)
		{
			this.progress = progress;
		}

		// Token: 0x060029A3 RID: 10659 RVA: 0x0009CA74 File Offset: 0x0009AC74
		public override bool ShouldEnd()
		{
			return SRSingleton<SceneContext>.Instance.ProgressDirector.HasProgress(this.progress);
		}

		// Token: 0x040028E6 RID: 10470
		private ProgressDirector.ProgressType progress;
	}

	// Token: 0x020007C5 RID: 1989
	private class QuicksilverRaceTutorial : TutorialDirector.BaseTutorial
	{
		// Token: 0x060029A5 RID: 10661 RVA: 0x0009CA8C File Offset: 0x0009AC8C
		public override void Start()
		{
			base.Start();
			if (this.marker != null)
			{
				this.marker.enabled = false;
				this.marker = null;
			}
			if (this.IsEnergyGeneratorActivated())
			{
				this.isActivated = true;
				return;
			}
			Vector3 position = SRSingleton<SceneContext>.Instance.Player.transform.position;
			float num = float.MaxValue;
			foreach (TutorialRadar tutorialRadar in TutorialRadar.allRadars)
			{
				if (tutorialRadar.tutorialId == TutorialDirector.Id.RACE_START)
				{
					float sqrMagnitude = (position - tutorialRadar.gameObject.transform.position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						this.marker = tutorialRadar.GetComponent<RadarTrackedObject>();
						num = sqrMagnitude;
					}
				}
			}
			if (this.marker != null)
			{
				this.marker.enabled = true;
			}
		}

		// Token: 0x060029A6 RID: 10662 RVA: 0x0009CB84 File Offset: 0x0009AD84
		public override bool ShouldEnd()
		{
			return base.ShouldEnd() || this.isActivated;
		}

		// Token: 0x060029A7 RID: 10663 RVA: 0x0009CB98 File Offset: 0x0009AD98
		private bool IsEnergyGeneratorActivated()
		{
			using (List<QuicksilverEnergyGenerator>.Enumerator enumerator = QuicksilverEnergyGenerator.allGenerators.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.GetState() != QuicksilverEnergyGenerator.State.INACTIVE)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060029A8 RID: 10664 RVA: 0x0009CBF0 File Offset: 0x0009ADF0
		public override void End()
		{
			base.End();
			if (this.marker != null)
			{
				this.marker.enabled = false;
				this.marker = null;
			}
		}

		// Token: 0x060029A9 RID: 10665 RVA: 0x0009CC19 File Offset: 0x0009AE19
		public override bool ShouldHide()
		{
			return !SRSingleton<SceneContext>.Instance.Player.GetComponent<RegionMember>().IsInZone(ZoneDirector.Zone.VALLEY);
		}

		// Token: 0x060029AA RID: 10666 RVA: 0x0009CC34 File Offset: 0x0009AE34
		public void OnQuicksilverRaceActivated()
		{
			this.isActivated = true;
		}

		// Token: 0x040028E7 RID: 10471
		private bool isActivated;

		// Token: 0x040028E8 RID: 10472
		private RadarTrackedObject marker;
	}
}
