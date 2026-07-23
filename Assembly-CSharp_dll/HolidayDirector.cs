using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x0200020A RID: 522
public class HolidayDirector : MonoBehaviour, HolidayModel.Participant
{
	// Token: 0x06000B04 RID: 2820 RVA: 0x0002E9B0 File Offset: 0x0002CBB0
	public void InitForLevel()
	{
		SRSingleton<SceneContext>.Instance.GameModel.RegisterHoliday(this);
		SceneContext.onSceneLoaded = (SceneContext.SceneLoadDelegate)Delegate.Combine(SceneContext.onSceneLoaded, new SceneContext.SceneLoadDelegate(this.OnSceneLoaded_EventGordos));
		SceneContext.onSceneLoaded = (SceneContext.SceneLoadDelegate)Delegate.Combine(SceneContext.onSceneLoaded, new SceneContext.SceneLoadDelegate(this.OnSceneLoaded_EchoNoteGordo));
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x0002EA10 File Offset: 0x0002CC10
	public void Awake()
	{
		foreach (HolidayDirector.OrnamentEntry ornamentEntry in this.ornaments)
		{
			ornamentEntry.Init();
			this.ornamentDict[ornamentEntry.date] = ornamentEntry;
		}
	}

	// Token: 0x06000B06 RID: 2822 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(HolidayModel model)
	{
	}

	// Token: 0x06000B07 RID: 2823 RVA: 0x0002EA74 File Offset: 0x0002CC74
	public void SetModel(HolidayModel model)
	{
		this.model = model;
	}

	// Token: 0x06000B08 RID: 2824 RVA: 0x0002EA7D File Offset: 0x0002CC7D
	public IEnumerable<Identifiable.Id> GetCurrOrnament()
	{
		if (DateTime.Today.Year == 2017)
		{
			HolidayDirector.MonthAndDay key = new HolidayDirector.MonthAndDay(DateTime.Today.Month, DateTime.Today.Day);
			if (this.ornamentDict.ContainsKey(key))
			{
				yield return Randoms.SHARED.Pick<Identifiable.Id>(this.ornamentDict[key].weightDict, Identifiable.Id.NONE);
			}
		}
		yield break;
	}

	// Token: 0x06000B09 RID: 2825 RVA: 0x0002EA90 File Offset: 0x0002CC90
	private void OnSceneLoaded_EventGordos(SceneContext ctx)
	{
		SceneContext.onSceneLoaded = (SceneContext.SceneLoadDelegate)Delegate.Remove(SceneContext.onSceneLoaded, new SceneContext.SceneLoadDelegate(this.OnSceneLoaded_EventGordos));
		if (Levels.isSpecial() || !ctx.GameModeConfig.GetModeSettings().enableEventGordos)
		{
			this.model.eventGordos.Clear();
			return;
		}
		IDateProvider dateProvider = SRSingleton<SystemContext>.Instance.DateProvider;
		DateTime currentDate = dateProvider.GetToday();
		Log.Debug("Current System Date For Events", new object[]
		{
			"Date",
			currentDate.ToString("yyyy-MM-dd")
		});
		bool flag = false;
		int num = this.model.eventGordos.RemoveWhere((HolidayModel.EventGordo e) => !e.IsLiveAsOf(currentDate));
		flag |= (num > 0);
		IEnumerable<HolidayModel.EventGordo> instances = HolidayModel.EventGordo.INSTANCES;
		Func<HolidayModel.EventGordo, bool> <>9__1;
		Func<HolidayModel.EventGordo, bool> predicate;
		if ((predicate = <>9__1) == null)
		{
			predicate = (<>9__1 = ((HolidayModel.EventGordo e) => e.IsLiveAsOf(currentDate)));
		}
		foreach (HolidayModel.EventGordo eventGordo in instances.Where(predicate))
		{
			GordoModel gordoModel = SRSingleton<SceneContext>.Instance.GameModel.GetGordoModel(eventGordo.objectId);
			if (gordoModel == null)
			{
				Log.Error("Failed to active EventGordo.", new object[]
				{
					"event",
					eventGordo
				});
				SentrySdk.CaptureMessage("Failed to active EventGordo!");
			}
			else
			{
				bool flag2 = this.model.eventGordos.Add(eventGordo);
				gordoModel.EventGordoActivate(flag2);
				flag = (flag || flag2);
			}
		}
		if (flag)
		{
			base.StartCoroutine(this.ResetCratesAfterFrame());
		}
	}

	// Token: 0x06000B0A RID: 2826 RVA: 0x0002EC30 File Offset: 0x0002CE30
	private IEnumerator ResetCratesAfterFrame()
	{
		yield return new WaitForEndOfFrame();
		ZoneDirector.Zone currentZone = SRSingleton<SceneContext>.Instance.Player.GetComponent<PlayerZoneTracker>().GetCurrentZone();
		ZoneDirector zoneDirector = ZoneDirector.zones.Get(currentZone);
		if (zoneDirector != null)
		{
			zoneDirector.ResetCrates();
		}
		yield break;
	}

	// Token: 0x06000B0B RID: 2827 RVA: 0x0002EC38 File Offset: 0x0002CE38
	private void OnSceneLoaded_EchoNoteGordo(SceneContext ctx)
	{
		SceneContext.onSceneLoaded = (SceneContext.SceneLoadDelegate)Delegate.Remove(SceneContext.onSceneLoaded, new SceneContext.SceneLoadDelegate(this.OnSceneLoaded_EchoNoteGordo));
		if (Levels.isSpecial() || !ctx.GameModeConfig.GetModeSettings().enableEchoNoteGordos)
		{
			this.model.eventEchoNoteGordos.Clear();
			return;
		}
		IDateProvider dateProvider = SRSingleton<SystemContext>.Instance.DateProvider;
		DateTime currentDate = dateProvider.GetToday();
		Log.Debug("Current System Date For Wiggly Events", new object[]
		{
			"Date",
			currentDate.ToString("yyyy-MM-dd")
		});
		this.model.eventEchoNoteGordos.RemoveWhere((HolidayModel.EventEchoNoteGordo e) => !e.IsLiveAsOf(currentDate));
		IEnumerable<HolidayModel.EventEchoNoteGordo> instances = HolidayModel.EventEchoNoteGordo.INSTANCES;
		Func<HolidayModel.EventEchoNoteGordo, bool> <>9__1;
		Func<HolidayModel.EventEchoNoteGordo, bool> predicate;
		if ((predicate = <>9__1) == null)
		{
			predicate = (<>9__1 = ((HolidayModel.EventEchoNoteGordo e) => e.IsLiveAsOf(currentDate)));
		}
		foreach (HolidayModel.EventEchoNoteGordo eventEchoNoteGordo in instances.Where(predicate))
		{
			EchoNoteGordoModel echoNoteGordoModel = SRSingleton<SceneContext>.Instance.GameModel.GetEchoNoteGordoModel(eventEchoNoteGordo.objectId);
			if (echoNoteGordoModel == null)
			{
				Log.Error("Failed to active EchoNoteGordo.", new object[]
				{
					"id",
					eventEchoNoteGordo.objectId
				});
				SentrySdk.CaptureMessage("Failed to active EchoNoteGordo!");
			}
			else
			{
				bool isFirstActivation = this.model.eventEchoNoteGordos.Add(eventEchoNoteGordo);
				echoNoteGordoModel.Activate(isFirstActivation);
			}
		}
	}

	// Token: 0x040008F6 RID: 2294
	public List<HolidayDirector.OrnamentEntry> ornaments = new List<HolidayDirector.OrnamentEntry>();

	// Token: 0x040008F7 RID: 2295
	private Dictionary<HolidayDirector.MonthAndDay, HolidayDirector.OrnamentEntry> ornamentDict = new Dictionary<HolidayDirector.MonthAndDay, HolidayDirector.OrnamentEntry>();

	// Token: 0x040008F8 RID: 2296
	private HolidayModel model;

	// Token: 0x0200020B RID: 523
	[Serializable]
	public class MonthAndDay : IEquatable<HolidayDirector.MonthAndDay>
	{
		// Token: 0x06000B0D RID: 2829 RVA: 0x0002EDD6 File Offset: 0x0002CFD6
		public MonthAndDay(int month, int day)
		{
			this.month = month;
			this.day = day;
		}

		// Token: 0x06000B0E RID: 2830 RVA: 0x0002EDEC File Offset: 0x0002CFEC
		public bool Equals(HolidayDirector.MonthAndDay other)
		{
			return this.month == other.month && this.day == other.day;
		}

		// Token: 0x06000B0F RID: 2831 RVA: 0x0002EE0C File Offset: 0x0002D00C
		public override int GetHashCode()
		{
			return this.month << 8 ^ this.day;
		}

		// Token: 0x040008F9 RID: 2297
		public int month;

		// Token: 0x040008FA RID: 2298
		public int day;
	}

	// Token: 0x0200020C RID: 524
	[Serializable]
	public class OrnamentEntry
	{
		// Token: 0x06000B10 RID: 2832 RVA: 0x0002EE20 File Offset: 0x0002D020
		public void Init()
		{
			foreach (HolidayDirector.OrnamentEntry.WeightEntry weightEntry in this.weights)
			{
				this.weightDict[weightEntry.id] = weightEntry.weight;
			}
		}

		// Token: 0x040008FB RID: 2299
		public HolidayDirector.MonthAndDay date;

		// Token: 0x040008FC RID: 2300
		public List<HolidayDirector.OrnamentEntry.WeightEntry> weights;

		// Token: 0x040008FD RID: 2301
		public Dictionary<Identifiable.Id, float> weightDict = new Dictionary<Identifiable.Id, float>(Identifiable.idComparer);

		// Token: 0x0200020D RID: 525
		[Serializable]
		public class WeightEntry
		{
			// Token: 0x040008FE RID: 2302
			public float weight;

			// Token: 0x040008FF RID: 2303
			public Identifiable.Id id;
		}
	}
}
