using System;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x0200029E RID: 670
public class GadgetDirector : MonoBehaviour, GadgetsModel.Participant
{
	// Token: 0x06000E1C RID: 3612 RVA: 0x00039394 File Offset: 0x00037594
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.progressDir = SRSingleton<SceneContext>.Instance.ProgressDirector;
		this.popupDir = SRSingleton<SceneContext>.Instance.PopupDirector;
		this.tutorialDir = SRSingleton<SceneContext>.Instance.TutorialDirector;
	}

	// Token: 0x06000E1D RID: 3613 RVA: 0x000393E1 File Offset: 0x000375E1
	public void InitForLevel()
	{
		SRSingleton<SceneContext>.Instance.GameModel.RegisterGadgets(this);
	}

	// Token: 0x06000E1E RID: 3614 RVA: 0x000393F4 File Offset: 0x000375F4
	public void InitModel(GadgetsModel model)
	{
		this.blueprintLocks.Clear();
		model.Reset();
		this.InitBlueprintLocks();
		foreach (Gadget.Id key in this.blueprintLocks.Keys)
		{
			model.blueprintLockData[key] = new GadgetDirector.BlueprintLockData(false, double.PositiveInfinity);
		}
	}

	// Token: 0x06000E1F RID: 3615 RVA: 0x00039478 File Offset: 0x00037678
	public void SetModel(GadgetsModel model)
	{
		this.model = model;
		this.ClearUnnecessaryBlueprintLockers();
		this.CheckAllBlueprintLockers();
	}

	// Token: 0x06000E20 RID: 3616 RVA: 0x00039490 File Offset: 0x00037690
	private void ClearUnnecessaryBlueprintLockers()
	{
		foreach (Gadget.Id key in this.model.availBlueprints)
		{
			this.model.blueprintLockData.Remove(key);
			this.blueprintLocks.Remove(key);
		}
	}

	// Token: 0x06000E21 RID: 3617 RVA: 0x00039500 File Offset: 0x00037700
	private void InitBlueprintLocks()
	{
		if (SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().blueprintsEnabled)
		{
			this.blueprintLocks[Gadget.Id.EXTRACTOR_DRILL_ADVANCED] = this.CreateBasicLock(Gadget.Id.EXTRACTOR_DRILL_ADVANCED, Gadget.Id.NONE, ProgressDirector.ProgressType.UNLOCK_LAB, 126f);
			this.blueprintLocks[Gadget.Id.EXTRACTOR_PUMP_ADVANCED] = this.CreateBasicLock(Gadget.Id.EXTRACTOR_PUMP_ADVANCED, Gadget.Id.EXTRACTOR_PUMP_NOVICE, 120);
			this.blueprintLocks[Gadget.Id.EXTRACTOR_APIARY_ADVANCED] = this.CreateBasicLock(Gadget.Id.EXTRACTOR_APIARY_ADVANCED, Gadget.Id.EXTRACTOR_APIARY_NOVICE, 120);
			Dictionary<Gadget.Id, GadgetDirector.BlueprintLocker> dictionary = this.blueprintLocks;
			Gadget.Id key = Gadget.Id.TAMING_BELL;
			Gadget.Id id = Gadget.Id.TAMING_BELL;
			Gadget.Id[] waitForBlueprints = new Gadget.Id[]
			{
				Gadget.Id.MED_STATION
			};
			ProgressDirector.ProgressType[] array = new ProgressDirector.ProgressType[2];
			array[0] = ProgressDirector.ProgressType.UNLOCK_MOSS;
			dictionary[key] = this.CreateBasicLock(id, waitForBlueprints, array, 72f);
			this.blueprintLocks[Gadget.Id.REFINERY_LINK] = this.CreateBasicLock(Gadget.Id.REFINERY_LINK, new Gadget.Id[]
			{
				Gadget.Id.EXTRACTOR_DRILL_ADVANCED,
				Gadget.Id.EXTRACTOR_PUMP_ADVANCED,
				Gadget.Id.EXTRACTOR_APIARY_ADVANCED
			}, null, 24f);
			this.blueprintLocks[Gadget.Id.TELEPORTER_BLUE] = this.CreateBasicLock(Gadget.Id.TELEPORTER_BLUE, Gadget.Id.TELEPORTER_PINK, ProgressDirector.ProgressType.UNLOCK_QUARRY, 48f);
			this.blueprintLocks[Gadget.Id.TELEPORTER_GREY] = this.CreateBasicLock(Gadget.Id.TELEPORTER_GREY, Gadget.Id.TELEPORTER_PINK, ProgressDirector.ProgressType.UNLOCK_MOSS, 48f);
			this.blueprintLocks[Gadget.Id.TELEPORTER_BUTTERSCOTCH] = this.CreateBasicLock(Gadget.Id.TELEPORTER_BUTTERSCOTCH, Gadget.Id.TELEPORTER_PINK, ProgressDirector.ProgressType.UNLOCK_DESERT, 0.5f);
			this.blueprintLocks[Gadget.Id.WARP_DEPOT_BLUE] = this.CreateBasicLock(Gadget.Id.WARP_DEPOT_BLUE, Gadget.Id.WARP_DEPOT_PINK, ProgressDirector.ProgressType.UNLOCK_QUARRY, 48f);
			this.blueprintLocks[Gadget.Id.WARP_DEPOT_GREY] = this.CreateBasicLock(Gadget.Id.WARP_DEPOT_GREY, Gadget.Id.WARP_DEPOT_PINK, ProgressDirector.ProgressType.UNLOCK_MOSS, 48f);
			this.blueprintLocks[Gadget.Id.WARP_DEPOT_BUTTERSCOTCH] = this.CreateBasicLock(Gadget.Id.WARP_DEPOT_BUTTERSCOTCH, Gadget.Id.WARP_DEPOT_PINK, ProgressDirector.ProgressType.UNLOCK_DESERT, 0.5f);
			this.blueprintLocks[Gadget.Id.LAMP_BLUE] = this.CreateBasicLock(Gadget.Id.LAMP_BLUE, Gadget.Id.LAMP_PINK, 48);
			this.blueprintLocks[Gadget.Id.LAMP_GREY] = this.CreateBasicLock(Gadget.Id.LAMP_GREY, Gadget.Id.LAMP_BLUE, 48);
			this.blueprintLocks[Gadget.Id.FASHION_POD_CLIP_ON] = this.CreateBasicLock(Gadget.Id.FASHION_POD_CLIP_ON, Gadget.Id.SLIME_STAGE, 48);
			this.blueprintLocks[Gadget.Id.FASHION_POD_GOOGLY] = this.CreateBasicLock(Gadget.Id.FASHION_POD_GOOGLY, Gadget.Id.SLIME_HOOP, 72);
			this.blueprintLocks[Gadget.Id.FASHION_POD_REMOVER] = this.CreateAnyFashionLock(Gadget.Id.FASHION_POD_REMOVER, 24f);
			this.blueprintLocks[Gadget.Id.CHICKEN_CLONER] = new GadgetDirector.BlueprintLocker.ViktorProgress(this, Gadget.Id.CHICKEN_CLONER, 1);
			this.blueprintLocks[Gadget.Id.DRONE_ADVANCED] = new GadgetDirector.BlueprintLocker.ViktorProgress(this, Gadget.Id.DRONE_ADVANCED, 2);
		}
	}

	// Token: 0x06000E22 RID: 3618 RVA: 0x000397A2 File Offset: 0x000379A2
	public GadgetDirector.BlueprintLocker CreateBasicLock(Gadget.Id id, Gadget.Id waitForBlueprint, int delayHrs)
	{
		Gadget.Id[] waitForBlueprints;
		if (waitForBlueprint != Gadget.Id.NONE)
		{
			(waitForBlueprints = new Gadget.Id[1])[0] = waitForBlueprint;
		}
		else
		{
			waitForBlueprints = null;
		}
		return this.CreateBasicLock(id, waitForBlueprints, null, (float)delayHrs);
	}

	// Token: 0x06000E23 RID: 3619 RVA: 0x000397BE File Offset: 0x000379BE
	public GadgetDirector.BlueprintLocker CreateBasicLock(Gadget.Id id, Gadget.Id waitForBlueprint, ProgressDirector.ProgressType waitForProgress, float delayHrs)
	{
		Gadget.Id[] waitForBlueprints;
		if (waitForBlueprint != Gadget.Id.NONE)
		{
			(waitForBlueprints = new Gadget.Id[1])[0] = waitForBlueprint;
		}
		else
		{
			waitForBlueprints = null;
		}
		return this.CreateBasicLock(id, waitForBlueprints, new ProgressDirector.ProgressType[]
		{
			waitForProgress
		}, delayHrs);
	}

	// Token: 0x06000E24 RID: 3620 RVA: 0x000397E4 File Offset: 0x000379E4
	public GadgetDirector.BlueprintLocker CreateBasicLock(Gadget.Id id, Gadget.Id[] waitForBlueprints, ProgressDirector.ProgressType[] waitForProgress, float delayHrs)
	{
		return new GadgetDirector.BlueprintLocker(this, id, () => this.ShouldUnlock(waitForBlueprints, waitForProgress), delayHrs);
	}

	// Token: 0x06000E25 RID: 3621 RVA: 0x00039821 File Offset: 0x00037A21
	public GadgetDirector.BlueprintLocker CreateAnyFashionLock(Gadget.Id id, float delayHrs)
	{
		return new GadgetDirector.BlueprintLocker(this, id, () => this.HasAnyFashionGadget(), delayHrs);
	}

	// Token: 0x06000E26 RID: 3622 RVA: 0x00039838 File Offset: 0x00037A38
	private bool HasAnyFashionGadget()
	{
		foreach (Gadget.Id blueprint in Gadget.ALL_FASHIONS)
		{
			if (this.HasBlueprint(blueprint))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000E27 RID: 3623 RVA: 0x00039894 File Offset: 0x00037A94
	private bool ShouldUnlock(Gadget.Id[] waitForBlueprints, ProgressDirector.ProgressType[] waitForProgress)
	{
		if (waitForBlueprints != null)
		{
			foreach (Gadget.Id blueprint in waitForBlueprints)
			{
				if (!this.HasBlueprint(blueprint))
				{
					return false;
				}
			}
		}
		if (waitForProgress != null)
		{
			foreach (ProgressDirector.ProgressType type in waitForProgress)
			{
				if (!this.progressDir.HasProgress(type))
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x06000E28 RID: 3624 RVA: 0x000398F0 File Offset: 0x00037AF0
	public void Update()
	{
		if (SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().blueprintsEnabled)
		{
			foreach (KeyValuePair<Gadget.Id, GadgetDirector.BlueprintLocker> keyValuePair in this.blueprintLocks)
			{
				if (keyValuePair.Value.ReachedUnlockTime())
				{
					this.toRemove.Add(keyValuePair.Key);
					if (!this.model.availBlueprints.Contains(keyValuePair.Key))
					{
						this.model.availBlueprints.Add(keyValuePair.Key);
						if (keyValuePair.Value.ShowBlueprintAvailablePopup())
						{
							this.popupDir.QueueForPopup(new GadgetDirector.AvailBlueprintPopupCreator(keyValuePair.Key));
							this.popupDir.MaybePopupNext();
						}
					}
				}
			}
			foreach (Gadget.Id key in this.toRemove)
			{
				this.blueprintLocks.Remove(key);
				this.model.blueprintLockData.Remove(key);
			}
			this.toRemove.Clear();
		}
		if (this.timeDir.OnPassedHour(5f) && SRSingleton<LockOnDeath>.Instance != null && SRSingleton<LockOnDeath>.Instance.Locked())
		{
			this.CheckGordoSnares();
		}
	}

	// Token: 0x06000E29 RID: 3625 RVA: 0x00039A74 File Offset: 0x00037C74
	public void CheckGordoSnares()
	{
		foreach (GordoSnare gordoSnare in GordoSnare.AllGordoSnares)
		{
			if (gordoSnare.IsBaited())
			{
				gordoSnare.SnareGordo();
			}
		}
	}

	// Token: 0x06000E2A RID: 3626 RVA: 0x00039AD0 File Offset: 0x00037CD0
	public void OnProgress(ProgressDirector.ProgressType type)
	{
		this.CheckAllBlueprintLockers();
	}

	// Token: 0x06000E2B RID: 3627 RVA: 0x00039AD8 File Offset: 0x00037CD8
	public bool AddBlueprint(Gadget.Id blueprint)
	{
		if (SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().blueprintsEnabled && !this.model.blueprints.Contains(blueprint))
		{
			this.model.blueprints.Add(blueprint);
			this.CheckAllBlueprintLockers();
			return true;
		}
		return false;
	}

	// Token: 0x06000E2C RID: 3628 RVA: 0x00039B2C File Offset: 0x00037D2C
	public void CheckAllBlueprintLockers()
	{
		foreach (KeyValuePair<Gadget.Id, GadgetDirector.BlueprintLocker> keyValuePair in this.blueprintLocks)
		{
			if (keyValuePair.Value.CheckUnlockCondition())
			{
				keyValuePair.Value.Unlock();
			}
		}
	}

	// Token: 0x06000E2D RID: 3629 RVA: 0x00039B94 File Offset: 0x00037D94
	public bool IsBlueprintUnlocked(Gadget.Id blueprint)
	{
		return !this.HasBlueprint(blueprint) && (this.model.availBlueprints.Contains(blueprint) || this.model.registeredBlueprints.Contains(blueprint));
	}

	// Token: 0x06000E2E RID: 3630 RVA: 0x00039BC7 File Offset: 0x00037DC7
	public bool HasBlueprint(Gadget.Id blueprint)
	{
		return this.model.blueprints.Contains(blueprint);
	}

	// Token: 0x06000E2F RID: 3631 RVA: 0x00039BDC File Offset: 0x00037DDC
	public bool CanAddGadget(GadgetDefinition gadgetDefinition)
	{
		int buyCountLimit = gadgetDefinition.buyCountLimit;
		List<Gadget.Id> list = new List<Gadget.Id>
		{
			gadgetDefinition.id
		};
		return buyCountLimit <= 0 || this.GetGadgetCount(list) + this.GetPlacedCount(list) < buyCountLimit;
	}

	// Token: 0x06000E30 RID: 3632 RVA: 0x00039C1A File Offset: 0x00037E1A
	public bool CanPlaceGadget(GadgetSite site, Gadget.Id gadget)
	{
		return this.GetPlacementError(site, gadget) == null;
	}

	// Token: 0x06000E31 RID: 3633 RVA: 0x00039C28 File Offset: 0x00037E28
	public GadgetDirector.PlacementError GetPlacementError(GadgetSite site, Gadget.Id gadget)
	{
		GadgetDefinition gadgetDefinition = SRSingleton<GameContext>.Instance.LookupDirector.GetGadgetDefinition(gadget);
		if (gadget != Gadget.Id.DRONE && gadget != Gadget.Id.DRONE_ADVANCED)
		{
			if (gadgetDefinition.countLimit > 0)
			{
				List<Gadget.Id> gadgetsToCountIds = gadgetDefinition.GetGadgetsToCountIds();
				if (this.GetPlacedCount(gadgetsToCountIds) >= gadgetDefinition.countLimit)
				{
					string text = gadgetDefinition.id.ToString();
					if (text.StartsWith("EXTRACTOR_DRILL_"))
					{
						return new GadgetDirector.PlacementError
						{
							message = "w.limit_reached_drill",
							button = "b.limit_reached"
						};
					}
					if (text.StartsWith("EXTRACTOR_PUMP_"))
					{
						return new GadgetDirector.PlacementError
						{
							message = "w.limit_reached_pump",
							button = "b.limit_reached"
						};
					}
					if (text.StartsWith("EXTRACTOR_APIARY_"))
					{
						return new GadgetDirector.PlacementError
						{
							message = "w.limit_reached_apiary",
							button = "b.limit_reached"
						};
					}
					return new GadgetDirector.PlacementError
					{
						message = "w.limit_reached_drill",
						button = "b.limit_reached"
					};
				}
			}
			return null;
		}
		DroneNetwork droneNetwork = DroneNetwork.Find(site.gameObject);
		if (droneNetwork == null)
		{
			return new GadgetDirector.PlacementError
			{
				message = "w.not_on_ranch_drone",
				button = "b.place"
			};
		}
		if (droneNetwork.Drones.Count<Drone>() >= gadgetDefinition.countLimit)
		{
			return new GadgetDirector.PlacementError
			{
				message = "w.limit_reached_drone",
				button = "b.limit_reached"
			};
		}
		return null;
	}

	// Token: 0x06000E32 RID: 3634 RVA: 0x00039D88 File Offset: 0x00037F88
	public void AddGadget(Gadget.Id gadget)
	{
		if (this.model.gadgets.ContainsKey(gadget))
		{
			Dictionary<Gadget.Id, int> gadgets = this.model.gadgets;
			int value = gadgets[gadget] + 1;
			gadgets[gadget] = value;
			return;
		}
		this.model.gadgets[gadget] = 1;
	}

	// Token: 0x06000E33 RID: 3635 RVA: 0x00039DDC File Offset: 0x00037FDC
	public void SpendGadget(Gadget.Id gadget)
	{
		if (this.model.gadgets.ContainsKey(gadget))
		{
			if (this.model.gadgets[gadget] > 1)
			{
				Dictionary<Gadget.Id, int> gadgets = this.model.gadgets;
				int value = gadgets[gadget] - 1;
				gadgets[gadget] = value;
				return;
			}
			this.model.gadgets.Remove(gadget);
		}
	}

	// Token: 0x06000E34 RID: 3636 RVA: 0x00039E44 File Offset: 0x00038044
	public bool RemoveGadget(Gadget.Id gadget)
	{
		if (this.model.gadgets.ContainsKey(gadget) && this.model.gadgets[gadget] >= 1)
		{
			Dictionary<Gadget.Id, int> gadgets = this.model.gadgets;
			int value = gadgets[gadget] - 1;
			gadgets[gadget] = value;
			if (this.model.gadgets[gadget] == 0)
			{
				this.model.gadgets.Remove(gadget);
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000E35 RID: 3637 RVA: 0x00039EC0 File Offset: 0x000380C0
	public int GetGadgetCount(List<Gadget.Id> gadgets)
	{
		int num = 0;
		foreach (Gadget.Id gadget in gadgets)
		{
			num += this.GetGadgetCount(gadget);
		}
		return num;
	}

	// Token: 0x06000E36 RID: 3638 RVA: 0x00039F14 File Offset: 0x00038114
	public int GetGadgetCount(Gadget.Id gadget)
	{
		if (this.model.gadgets.ContainsKey(gadget))
		{
			return this.model.gadgets[gadget];
		}
		return 0;
	}

	// Token: 0x06000E37 RID: 3639 RVA: 0x00039F3C File Offset: 0x0003813C
	public static bool IsRefineryResource(Identifiable.Id id)
	{
		return Identifiable.PLORT_CLASS.Contains(id) || Identifiable.CRAFT_CLASS.Contains(id);
	}

	// Token: 0x06000E38 RID: 3640 RVA: 0x00039F58 File Offset: 0x00038158
	public int GetRefinerySpaceAvailable(Identifiable.Id id)
	{
		if (!GadgetDirector.IsRefineryResource(id))
		{
			return 0;
		}
		return Mathf.Max(0, 999 - this.GetRefineryCount(id));
	}

	// Token: 0x06000E39 RID: 3641 RVA: 0x00039F77 File Offset: 0x00038177
	public bool HasRefinerySpaceAvailable(Identifiable.Id id, int count = 1)
	{
		return this.GetRefinerySpaceAvailable(id) >= count;
	}

	// Token: 0x06000E3A RID: 3642 RVA: 0x00039F86 File Offset: 0x00038186
	public bool AddToRefinery(Identifiable.Id id)
	{
		return this.AddToRefinery(id, 1) > 0;
	}

	// Token: 0x06000E3B RID: 3643 RVA: 0x00039F93 File Offset: 0x00038193
	public int AddToRefinery(Identifiable.Id id, int count)
	{
		return this.AddToRefinery(id, count, false);
	}

	// Token: 0x06000E3C RID: 3644 RVA: 0x00039FA0 File Offset: 0x000381A0
	public int AddToRefinery(Identifiable.Id id, int count, bool overflow)
	{
		int num = GadgetDirector.IsRefineryResource(id) ? (overflow ? count : Mathf.Min(count, this.GetRefinerySpaceAvailable(id))) : 0;
		if (num > 0)
		{
			this.model.craftMatCounts[id] = this.GetRefineryCount(id) + num;
			this.tutorialDir.OnRefineryAdded();
		}
		return num;
	}

	// Token: 0x06000E3D RID: 3645 RVA: 0x00039FF8 File Offset: 0x000381F8
	public bool HasInRefinery(GadgetDefinition.CraftCost[] costs)
	{
		foreach (GadgetDefinition.CraftCost craftCost in costs)
		{
			if (this.model.craftMatCounts.Get(craftCost.id) < craftCost.amount)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000E3E RID: 3646 RVA: 0x0003A03A File Offset: 0x0003823A
	public int GetRefineryCount(Identifiable.Id id)
	{
		return this.model.craftMatCounts.Get(id);
	}

	// Token: 0x06000E3F RID: 3647 RVA: 0x0003A050 File Offset: 0x00038250
	public bool TryToSpendFromRefinery(GadgetDefinition.CraftCost[] costs)
	{
		if (!this.HasInRefinery(costs))
		{
			return false;
		}
		foreach (GadgetDefinition.CraftCost craftCost in costs)
		{
			if (craftCost.amount > 0)
			{
				Dictionary<Identifiable.Id, int> craftMatCounts = this.model.craftMatCounts;
				Identifiable.Id id = craftCost.id;
				craftMatCounts[id] -= craftCost.amount;
			}
		}
		return true;
	}

	// Token: 0x06000E40 RID: 3648 RVA: 0x0003A0B4 File Offset: 0x000382B4
	public void IncrementPlacedGadgetCount(Gadget.Id id)
	{
		int num = this.model.placedGadgetCounts.Get(id);
		this.model.placedGadgetCounts[id] = num + 1;
	}

	// Token: 0x06000E41 RID: 3649 RVA: 0x0003A0E8 File Offset: 0x000382E8
	public void DecrementPlacedGadgetCount(Gadget.Id id)
	{
		if (this.model.placedGadgetCounts.ContainsKey(id))
		{
			Dictionary<Gadget.Id, int> placedGadgetCounts = this.model.placedGadgetCounts;
			int value = placedGadgetCounts[id] - 1;
			placedGadgetCounts[id] = value;
		}
	}

	// Token: 0x06000E42 RID: 3650 RVA: 0x0003A128 File Offset: 0x00038328
	public int GetPlacedCount(List<Gadget.Id> gadgetCountIds)
	{
		int num = 0;
		foreach (Gadget.Id key in gadgetCountIds)
		{
			num += this.model.placedGadgetCounts.Get(key);
		}
		return num;
	}

	// Token: 0x04000D48 RID: 3400
	public GameObject gadgetPopupPrefab;

	// Token: 0x04000D49 RID: 3401
	public GameObject availBlueprintPopupPrefab;

	// Token: 0x04000D4A RID: 3402
	public GameObject waitForChargeupPrefab;

	// Token: 0x04000D4B RID: 3403
	public const int REFINERY_MAX = 999;

	// Token: 0x04000D4C RID: 3404
	private TimeDirector timeDir;

	// Token: 0x04000D4D RID: 3405
	private ProgressDirector progressDir;

	// Token: 0x04000D4E RID: 3406
	private PopupDirector popupDir;

	// Token: 0x04000D4F RID: 3407
	private TutorialDirector tutorialDir;

	// Token: 0x04000D50 RID: 3408
	private GadgetsModel model;

	// Token: 0x04000D51 RID: 3409
	private List<Gadget.Id> toRemove = new List<Gadget.Id>();

	// Token: 0x04000D52 RID: 3410
	public Dictionary<Gadget.Id, GadgetDirector.BlueprintLocker> blueprintLocks = new Dictionary<Gadget.Id, GadgetDirector.BlueprintLocker>(Gadget.idComparer);

	// Token: 0x0200029F RID: 671
	// (Invoke) Token: 0x06000E46 RID: 3654
	public delegate bool UnlockCondition();

	// Token: 0x020002A0 RID: 672
	public class BlueprintLockData
	{
		// Token: 0x06000E49 RID: 3657 RVA: 0x0003A1B3 File Offset: 0x000383B3
		public BlueprintLockData(bool timedLock, double lockedUntil)
		{
			this.timedLock = timedLock;
			this.lockedUntil = lockedUntil;
		}

		// Token: 0x06000E4A RID: 3658 RVA: 0x000053FC File Offset: 0x000035FC
		public BlueprintLockData()
		{
		}

		// Token: 0x04000D53 RID: 3411
		public bool timedLock;

		// Token: 0x04000D54 RID: 3412
		public double lockedUntil;
	}

	// Token: 0x020002A1 RID: 673
	public class BlueprintLocker
	{
		// Token: 0x06000E4B RID: 3659 RVA: 0x0003A1C9 File Offset: 0x000383C9
		public BlueprintLocker(GadgetDirector gadgetDir, Gadget.Id id, GadgetDirector.UnlockCondition unlockCondition, float unlockDelayHrs)
		{
			this.gadgetDir = gadgetDir;
			this.id = id;
			this.unlockCondition = unlockCondition;
			this.unlockDelayHrs = unlockDelayHrs;
		}

		// Token: 0x06000E4C RID: 3660 RVA: 0x0003A1EE File Offset: 0x000383EE
		public bool CheckUnlockCondition()
		{
			return !this.gadgetDir.model.IsTimedLock(this.id) && this.unlockCondition();
		}

		// Token: 0x06000E4D RID: 3661 RVA: 0x0003A218 File Offset: 0x00038418
		public bool ReachedUnlockTime()
		{
			return this.gadgetDir.model.IsTimedLock(this.id) && this.gadgetDir.timeDir.HasReached(this.gadgetDir.model.GetLockedUntil(this.id));
		}

		// Token: 0x06000E4E RID: 3662 RVA: 0x0003A265 File Offset: 0x00038465
		public virtual void Unlock()
		{
			this.gadgetDir.model.UnlockAt(this.id, this.gadgetDir.timeDir.HoursFromNow(this.unlockDelayHrs));
		}

		// Token: 0x06000E4F RID: 3663 RVA: 0x00013CC5 File Offset: 0x00011EC5
		public virtual bool ShowBlueprintAvailablePopup()
		{
			return true;
		}

		// Token: 0x04000D55 RID: 3413
		protected readonly GadgetDirector gadgetDir;

		// Token: 0x04000D56 RID: 3414
		protected readonly GadgetDirector.UnlockCondition unlockCondition;

		// Token: 0x04000D57 RID: 3415
		protected readonly float unlockDelayHrs;

		// Token: 0x04000D58 RID: 3416
		protected readonly Gadget.Id id;

		// Token: 0x020002A2 RID: 674
		public class ViktorProgress : GadgetDirector.BlueprintLocker
		{
			// Token: 0x06000E50 RID: 3664 RVA: 0x0003A294 File Offset: 0x00038494
			public ViktorProgress(GadgetDirector director, Gadget.Id id, int count) : base(director, id, () => SRSingleton<SceneContext>.Instance.ProgressDirector.GetProgress(ProgressDirector.ProgressType.VIKTOR_REWARDS) >= count, 0f)
			{
			}

			// Token: 0x06000E51 RID: 3665 RVA: 0x0003A2C7 File Offset: 0x000384C7
			public override void Unlock()
			{
				base.Unlock();
				this.gadgetDir.AddBlueprint(this.id);
			}

			// Token: 0x06000E52 RID: 3666 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
			public override bool ShowBlueprintAvailablePopup()
			{
				return false;
			}
		}
	}

	// Token: 0x020002A4 RID: 676
	private class AvailBlueprintPopupCreator : PopupDirector.PopupCreator
	{
		// Token: 0x06000E55 RID: 3669 RVA: 0x0003A302 File Offset: 0x00038502
		public AvailBlueprintPopupCreator(Gadget.Id id)
		{
			this.id = id;
		}

		// Token: 0x06000E56 RID: 3670 RVA: 0x0003A311 File Offset: 0x00038511
		public override void Create()
		{
			AvailBlueprintPopupUI.CreateAvailBlueprintPopup(SRSingleton<GameContext>.Instance.LookupDirector.GetGadgetDefinition(this.id));
		}

		// Token: 0x06000E57 RID: 3671 RVA: 0x0003A32E File Offset: 0x0003852E
		public override bool Equals(object other)
		{
			return other is GadgetDirector.AvailBlueprintPopupCreator && ((GadgetDirector.AvailBlueprintPopupCreator)other).id == this.id;
		}

		// Token: 0x06000E58 RID: 3672 RVA: 0x0003A34D File Offset: 0x0003854D
		public override int GetHashCode()
		{
			return this.id.GetHashCode();
		}

		// Token: 0x06000E59 RID: 3673 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
		public override bool ShouldClear()
		{
			return false;
		}

		// Token: 0x04000D5A RID: 3418
		private Gadget.Id id;
	}

	// Token: 0x020002A5 RID: 677
	public class PlacementError
	{
		// Token: 0x04000D5B RID: 3419
		public string message;

		// Token: 0x04000D5C RID: 3420
		public string button;
	}
}
