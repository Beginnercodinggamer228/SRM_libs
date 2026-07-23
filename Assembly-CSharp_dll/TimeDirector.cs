using System;
using System.Collections;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x02000798 RID: 1944
public class TimeDirector : SRBehaviour, WorldModel.Participant
{
	// Token: 0x14000023 RID: 35
	// (add) Token: 0x0600288E RID: 10382 RVA: 0x00099B54 File Offset: 0x00097D54
	// (remove) Token: 0x0600288F RID: 10383 RVA: 0x00099B8C File Offset: 0x00097D8C
	public event TimeDirector.OnFastForwardChanged onFastForwardChanged;

	// Token: 0x06002890 RID: 10384 RVA: 0x00099BC4 File Offset: 0x00097DC4
	public void Awake()
	{
		if (this.pauserCount == 0)
		{
			Time.timeScale = 1f;
		}
		this.timeFactor = 86400f / this.secsPerGameDay;
		this.ffTimeFactor = 86400f / this.ffSecsPerGameDay;
		this.input = UnityEngine.Object.FindObjectOfType<vp_FPInput>();
		SRSingleton<GameContext>.Instance.MessageDirector.RegisterBundlesListener(new MessageDirector.BundlesListener(this.InitBundles));
		SRSingleton<SceneContext>.Instance.GameModel.RegisterWorldParticipant(this);
	}

	// Token: 0x06002891 RID: 10385 RVA: 0x00099C3D File Offset: 0x00097E3D
	public void InitModel(WorldModel worldModel)
	{
		worldModel.fastForwardUntil = null;
		worldModel.pauseWorldTime = false;
		this.ResetToStartTime(worldModel);
	}

	// Token: 0x06002892 RID: 10386 RVA: 0x00099C59 File Offset: 0x00097E59
	public void SetModel(WorldModel worldModel)
	{
		this.worldModel = worldModel;
	}

	// Token: 0x06002893 RID: 10387 RVA: 0x00099C62 File Offset: 0x00097E62
	private void ResetToStartTime(WorldModel worldModel)
	{
		worldModel.worldTime = 32400.0;
		worldModel.lastWorldTime = new double?(worldModel.worldTime);
	}

	// Token: 0x06002894 RID: 10388 RVA: 0x00099C84 File Offset: 0x00097E84
	public void InitBundles(MessageDirector msgDir)
	{
		this.uiBundle = msgDir.GetBundle("ui");
		if (this.uiBundle == null)
		{
			return;
		}
		this.dayFormatString = this.uiBundle.Get("m.day");
	}

	// Token: 0x06002895 RID: 10389 RVA: 0x00099CB6 File Offset: 0x00097EB6
	public void OnUnpause(TimeDirector.OnUnpauseDelegate del)
	{
		if (Time.timeScale != 0f)
		{
			del();
			return;
		}
		TimeDirector.onUnpauseDelegate = (TimeDirector.OnUnpauseDelegate)Delegate.Combine(TimeDirector.onUnpauseDelegate, del);
	}

	// Token: 0x06002896 RID: 10390 RVA: 0x00099CE0 File Offset: 0x00097EE0
	public void ClearOnUnpause(TimeDirector.OnUnpauseDelegate del)
	{
		if (TimeDirector.onUnpauseDelegate != null)
		{
			TimeDirector.onUnpauseDelegate = (TimeDirector.OnUnpauseDelegate)Delegate.Remove(TimeDirector.onUnpauseDelegate, del);
		}
	}

	// Token: 0x06002897 RID: 10391 RVA: 0x00099CFE File Offset: 0x00097EFE
	public bool ExactlyOnePauser()
	{
		return this.pauserCount == 1;
	}

	// Token: 0x06002898 RID: 10392 RVA: 0x00099D09 File Offset: 0x00097F09
	public bool IsFastForwarding()
	{
		return this.worldModel.fastForwardUntil != null;
	}

	// Token: 0x06002899 RID: 10393 RVA: 0x00099D1B File Offset: 0x00097F1B
	public bool HasPauser()
	{
		return this.pauserCount > 0;
	}

	// Token: 0x0600289A RID: 10394 RVA: 0x00099D26 File Offset: 0x00097F26
	public void InitForLevel()
	{
		if (this.pauserCount == 0)
		{
			this.SetCursorInUnpausedState();
		}
	}

	// Token: 0x0600289B RID: 10395 RVA: 0x00099D36 File Offset: 0x00097F36
	public void OnApplicationFocus(bool focused)
	{
		if (focused && this.pauserCount == 0)
		{
			this.SetCursorInUnpausedState();
		}
	}

	// Token: 0x0600289C RID: 10396 RVA: 0x00099D49 File Offset: 0x00097F49
	private void SetCursorInUnpausedState()
	{
		if (Levels.isSpecial())
		{
			this.EnableCursor(this.input);
			return;
		}
		this.DisableCursor(this.input);
	}

	// Token: 0x0600289D RID: 10397 RVA: 0x00099D6C File Offset: 0x00097F6C
	public void Update()
	{
		if (Time.timeScale != 0f && !Levels.isSpecialNonAlloc() && !this.worldModel.pauseWorldTime)
		{
			if (this.worldModel.fastForwardUntil != null)
			{
				this.worldModel.worldTime += (double)(Time.deltaTime * this.ffTimeFactor);
				if (this.worldModel.worldTime >= this.worldModel.fastForwardUntil.Value)
				{
					this.worldModel.worldTime = this.worldModel.fastForwardUntil.Value;
					this.worldModel.fastForwardUntil = null;
					if (this.onFastForwardChanged != null)
					{
						this.onFastForwardChanged(false);
						return;
					}
				}
			}
			else
			{
				this.worldModel.worldTime += (double)(Time.deltaTime * this.timeFactor);
			}
		}
	}

	// Token: 0x0600289E RID: 10398 RVA: 0x00099E50 File Offset: 0x00098050
	public void LateUpdate()
	{
		for (int i = this.passedTimeDelegates.Count - 1; i >= 0; i--)
		{
			if (this.OnPassedTime(this.passedTimeDelegates[i].time))
			{
				this.passedTimeDelegates[i].action();
				this.passedTimeDelegates.RemoveAt(i);
			}
		}
		this.worldModel.lastWorldTime = new double?(this.worldModel.worldTime);
	}

	// Token: 0x0600289F RID: 10399 RVA: 0x00099ECB File Offset: 0x000980CB
	public double DeltaWorldTime()
	{
		if (this.worldModel.lastWorldTime != null)
		{
			return this.worldModel.worldTime - this.worldModel.lastWorldTime.Value;
		}
		return 0.0;
	}

	// Token: 0x060028A0 RID: 10400 RVA: 0x00099F08 File Offset: 0x00098108
	public void Pause(bool pauseSFX = true, bool pauseSpecialScenes = false)
	{
		int num = ((this.pauserCount > 0 && !Levels.isSpecial()) || this.specialPauserCount > 0) ? 1 : 0;
		this.pauserCount++;
		if (pauseSpecialScenes)
		{
			this.specialPauserCount++;
		}
		bool flag = (this.pauserCount > 0 && !Levels.isSpecial()) || this.specialPauserCount > 0;
		if (num == 0 && flag)
		{
			this.savedTimeScale = Time.timeScale;
			Time.timeScale = 0f;
		}
		if (this.pauserCount > 0)
		{
			this.EnableCursor(this.input);
		}
	}

	// Token: 0x060028A1 RID: 10401 RVA: 0x00099FA0 File Offset: 0x000981A0
	public void DisableCursor(vp_FPInput input)
	{
		if (input != null)
		{
			input.MouseCursorForced = false;
		}
		vp_Utility.LockCursor = true;
	}

	// Token: 0x060028A2 RID: 10402 RVA: 0x00099FB8 File Offset: 0x000981B8
	public void EnableCursor(vp_FPInput input)
	{
		if (input != null)
		{
			input.MouseCursorForced = true;
		}
		vp_Utility.LockCursor = false;
	}

	// Token: 0x060028A3 RID: 10403 RVA: 0x00099FD0 File Offset: 0x000981D0
	public void Unpause(bool unpauseSFX = true, bool pauseSpecialScenes = false)
	{
		base.StartCoroutine(this.DelayedUnpause(unpauseSFX, pauseSpecialScenes));
	}

	// Token: 0x060028A4 RID: 10404 RVA: 0x00099FE1 File Offset: 0x000981E1
	private IEnumerator DelayedUnpause(bool unpauseSFX, bool pauseSpecialScenes = false)
	{
		yield return new WaitForEndOfFrame();
		bool flag = (this.pauserCount > 0 && !Levels.isSpecial()) || this.specialPauserCount > 0;
		this.pauserCount--;
		if (pauseSpecialScenes)
		{
			this.specialPauserCount--;
		}
		bool flag2 = (this.pauserCount > 0 && !Levels.isSpecial()) || this.specialPauserCount > 0;
		if (this.pauserCount < 0)
		{
			Log.Warning("Unpause() called while already unpaused.", Array.Empty<object>());
		}
		else if (flag && !flag2)
		{
			Time.timeScale = this.savedTimeScale;
			if (TimeDirector.onUnpauseDelegate != null)
			{
				TimeDirector.onUnpauseDelegate();
				TimeDirector.onUnpauseDelegate = null;
			}
		}
		if (this.pauserCount <= 0)
		{
			this.DisableCursor(this.input);
		}
		yield break;
	}

	// Token: 0x060028A5 RID: 10405 RVA: 0x00099FFE File Offset: 0x000981FE
	public double WorldTime()
	{
		return this.worldModel.worldTime;
	}

	// Token: 0x060028A6 RID: 10406 RVA: 0x0009A00B File Offset: 0x0009820B
	public double HoursFromNow(float hours)
	{
		return this.worldModel.worldTime + (double)(hours * 3600f);
	}

	// Token: 0x060028A7 RID: 10407 RVA: 0x0009A021 File Offset: 0x00098221
	public static double HoursFromTime(float hours, double time)
	{
		return time + (double)(hours * 3600f);
	}

	// Token: 0x060028A8 RID: 10408 RVA: 0x0009A02D File Offset: 0x0009822D
	public double HoursFromNowOrStart(float hours)
	{
		return Math.Max((this.worldModel == null) ? 0.0 : this.worldModel.worldTime, 32400.0) + (double)(hours * 3600f);
	}

	// Token: 0x060028A9 RID: 10409 RVA: 0x0009A064 File Offset: 0x00098264
	public bool HasReached(double targetWorldTime)
	{
		return TimeUtil.HasReached(this.worldModel.worldTime, targetWorldTime);
	}

	// Token: 0x060028AA RID: 10410 RVA: 0x0009A077 File Offset: 0x00098277
	public double TimeSince(double time)
	{
		return this.worldModel.worldTime - time;
	}

	// Token: 0x060028AB RID: 10411 RVA: 0x0009A086 File Offset: 0x00098286
	public double HoursUntil(double targetWorldTime)
	{
		return (targetWorldTime - this.worldModel.worldTime) * 0.00027777778450399637;
	}

	// Token: 0x060028AC RID: 10412 RVA: 0x0009A09F File Offset: 0x0009829F
	public float CurrDayFraction()
	{
		return TimeDirector.DayFraction(this.worldModel.worldTime);
	}

	// Token: 0x060028AD RID: 10413 RVA: 0x0009A0B1 File Offset: 0x000982B1
	public static float DayFraction(double time)
	{
		return (float)(time % 86400.0) * 1.1574074E-05f;
	}

	// Token: 0x060028AE RID: 10414 RVA: 0x0009A0C5 File Offset: 0x000982C5
	public int CurrDay()
	{
		return 1 + (int)Math.Floor(this.worldModel.worldTime * 1.1574074051168282E-05);
	}

	// Token: 0x060028AF RID: 10415 RVA: 0x0009A0E4 File Offset: 0x000982E4
	public int CurrDayAfterHour(float hour)
	{
		return 1 + (int)Math.Floor((this.worldModel.worldTime - (double)(hour * 3600f)) * 1.1574074051168282E-05);
	}

	// Token: 0x060028B0 RID: 10416 RVA: 0x0009A10C File Offset: 0x0009830C
	public float CurrHour()
	{
		return this.CurrDayFraction() * 24f;
	}

	// Token: 0x060028B1 RID: 10417 RVA: 0x0009A11A File Offset: 0x0009831A
	public float CurrHourOrStart()
	{
		return (float)(Math.Max(this.worldModel.worldTime, 32400.0) % 86400.0) * 0.00027777778f;
	}

	// Token: 0x060028B2 RID: 10418 RVA: 0x0009A146 File Offset: 0x00098346
	public int CurrTime()
	{
		return (int)Math.Floor((double)(this.CurrDayFraction() * 1440f));
	}

	// Token: 0x060028B3 RID: 10419 RVA: 0x0009A15C File Offset: 0x0009835C
	public string CurrDayString()
	{
		int num = this.CurrDay();
		return string.Format(this.dayFormatString, num);
	}

	// Token: 0x060028B4 RID: 10420 RVA: 0x0009A181 File Offset: 0x00098381
	public string CurrTimeString()
	{
		return this.FormatTime(this.CurrTime());
	}

	// Token: 0x060028B5 RID: 10421 RVA: 0x0009A190 File Offset: 0x00098390
	public Sprite CurrTimeIcon()
	{
		double num = (double)this.CurrDayFraction();
		if (num < 0.20000000298023224 || num > 0.800000011920929)
		{
			return this.nightSprite;
		}
		if (num > 0.30000001192092896 && num < 0.699999988079071)
		{
			return this.daySprite;
		}
		if (num > 0.5)
		{
			return this.duskSprite;
		}
		return this.dawnSprite;
	}

	// Token: 0x060028B6 RID: 10422 RVA: 0x0009A1FC File Offset: 0x000983FC
	public double GetNextHour(float hour)
	{
		return this.GetHourAfter(0, hour);
	}

	// Token: 0x060028B7 RID: 10423 RVA: 0x0009A208 File Offset: 0x00098408
	public double GetNextHourAtLeastHalfDay(float hour)
	{
		float num = hour - this.CurrHourOrStart();
		if (num < 0f)
		{
			num += 24f;
		}
		return this.GetHourAfter((num < 12f) ? 1 : 0, hour);
	}

	// Token: 0x060028B8 RID: 10424 RVA: 0x0009A241 File Offset: 0x00098441
	public double GetNextDawn()
	{
		return this.GetHourAfter(0, 6f);
	}

	// Token: 0x060028B9 RID: 10425 RVA: 0x0009A250 File Offset: 0x00098450
	public double GetNextDawnAfterNextDusk()
	{
		float num = this.CurrHour();
		return this.GetHourAfter((num >= 6f && num <= 18f) ? 0 : 1, 6f);
	}

	// Token: 0x060028BA RID: 10426 RVA: 0x0009A283 File Offset: 0x00098483
	public double GetHourAfter(int fullDays, float hour)
	{
		return TimeDirector.GetHourAfter(this.worldModel.worldTime, fullDays, hour);
	}

	// Token: 0x060028BB RID: 10427 RVA: 0x0009A298 File Offset: 0x00098498
	public static double GetHourAfter(double fromTime, int fullDays, float hour)
	{
		float num = hour / 24f;
		float num2 = TimeDirector.DayFraction(fromTime);
		float num3;
		if (num2 < num)
		{
			num3 = num - num2 + (float)fullDays;
		}
		else
		{
			num3 = num - num2 + (float)fullDays + 1f;
		}
		return fromTime + (double)(num3 * 86400f);
	}

	// Token: 0x060028BC RID: 10428 RVA: 0x0009A2DE File Offset: 0x000984DE
	public void FastForwardTo(double fastForwardUntil)
	{
		this.worldModel.fastForwardUntil = new double?((double)((long)(fastForwardUntil + 0.5)));
		if (this.onFastForwardChanged != null)
		{
			this.onFastForwardChanged(true);
		}
	}

	// Token: 0x060028BD RID: 10429 RVA: 0x0009A311 File Offset: 0x00098511
	public bool IsAtStart()
	{
		return this.worldModel != null && this.worldModel.worldTime == 32400.0;
	}

	// Token: 0x060028BE RID: 10430 RVA: 0x0009A333 File Offset: 0x00098533
	public static double HoursFromStart(double hours)
	{
		return (9.0 + hours) * 3600.0;
	}

	// Token: 0x060028BF RID: 10431 RVA: 0x0009A34C File Offset: 0x0009854C
	public bool OnPassedHour(float hour)
	{
		if (this.worldModel.lastWorldTime != null)
		{
			double? lastWorldTime = this.worldModel.lastWorldTime;
			double worldTime = this.worldModel.worldTime;
			if (!(lastWorldTime.GetValueOrDefault() >= worldTime & lastWorldTime != null))
			{
				double num = this.worldModel.worldTime * 0.00027777778450399637 % 24.0;
				double num2 = this.worldModel.lastWorldTime.Value * 0.00027777778450399637 % 24.0;
				return (num >= (double)hour && num2 < (double)hour) || (num < num2 && (num2 < (double)hour || (double)hour <= num));
			}
		}
		return false;
	}

	// Token: 0x060028C0 RID: 10432 RVA: 0x0009A404 File Offset: 0x00098604
	public bool OnPassedTime(double worldTime)
	{
		if (this.worldModel.lastWorldTime != null)
		{
			double? lastWorldTime = this.worldModel.lastWorldTime;
			if (lastWorldTime.GetValueOrDefault() < worldTime & lastWorldTime != null)
			{
				return worldTime <= this.worldModel.worldTime;
			}
		}
		return false;
	}

	// Token: 0x060028C1 RID: 10433 RVA: 0x0009A458 File Offset: 0x00098658
	public void OnPassedTime(double time, Action action)
	{
		if (this.HasReached(time))
		{
			action();
			return;
		}
		this.AddPassedTimeDelegate(time, action);
	}

	// Token: 0x060028C2 RID: 10434 RVA: 0x0009A472 File Offset: 0x00098672
	public void AddPassedTimeDelegate(double time, Action action)
	{
		this.passedTimeDelegates.Add(new TimeDirector.PassedTimeDelegate
		{
			time = time,
			action = action
		});
	}

	// Token: 0x060028C3 RID: 10435 RVA: 0x0009A494 File Offset: 0x00098694
	public void RemovePassedTimeDelegate(Action action)
	{
		for (int i = this.passedTimeDelegates.Count - 1; i >= 0; i--)
		{
			if (this.passedTimeDelegates[i].action == action)
			{
				this.passedTimeDelegates.RemoveAt(i);
			}
		}
	}

	// Token: 0x060028C4 RID: 10436 RVA: 0x0009A4E0 File Offset: 0x000986E0
	public string FormatTime(int totalMins)
	{
		int val = totalMins / 60;
		int val2 = totalMins % 60;
		return this.uiBundle.Get("l.time_hours_mins", new string[]
		{
			StringUtil.Pad(val, 2),
			StringUtil.Pad(val2, 2)
		});
	}

	// Token: 0x060028C5 RID: 10437 RVA: 0x0009A521 File Offset: 0x00098721
	public string FormatTimeMinutes(int? minutes)
	{
		if (minutes != null)
		{
			return this.FormatTime(minutes.Value);
		}
		return this.uiBundle.Get("l.time_hours_mins_unset");
	}

	// Token: 0x060028C6 RID: 10438 RVA: 0x0009A54C File Offset: 0x0009874C
	public string FormatTimeSeconds(double? seconds)
	{
		if (seconds != null)
		{
			int totalMins = Mathf.CeilToInt((float)seconds.Value * 0.016666668f);
			return this.FormatTime(totalMins);
		}
		return this.uiBundle.Get("l.time_hours_mins_unset");
	}

	// Token: 0x0400282C RID: 10284
	public float secsPerGameDay = 1440f;

	// Token: 0x0400282D RID: 10285
	public float ffSecsPerGameDay = 5f;

	// Token: 0x0400282E RID: 10286
	public const float START_HOUR = 9f;

	// Token: 0x0400282F RID: 10287
	public Sprite daySprite;

	// Token: 0x04002830 RID: 10288
	public Sprite nightSprite;

	// Token: 0x04002831 RID: 10289
	public Sprite dawnSprite;

	// Token: 0x04002832 RID: 10290
	public Sprite duskSprite;

	// Token: 0x04002833 RID: 10291
	private float timeFactor;

	// Token: 0x04002834 RID: 10292
	private float ffTimeFactor;

	// Token: 0x04002835 RID: 10293
	private MessageBundle uiBundle;

	// Token: 0x04002836 RID: 10294
	private float savedTimeScale;

	// Token: 0x04002837 RID: 10295
	private int pauserCount;

	// Token: 0x04002838 RID: 10296
	private int specialPauserCount;

	// Token: 0x04002839 RID: 10297
	private static TimeDirector.OnUnpauseDelegate onUnpauseDelegate;

	// Token: 0x0400283A RID: 10298
	private vp_FPInput input;

	// Token: 0x0400283B RID: 10299
	public const float SECS_PER_MIN = 60f;

	// Token: 0x0400283C RID: 10300
	public const float MINS_PER_HOUR = 60f;

	// Token: 0x0400283D RID: 10301
	public const float HOURS_PER_DAY = 24f;

	// Token: 0x0400283E RID: 10302
	public const float MINS_PER_DAY = 1440f;

	// Token: 0x0400283F RID: 10303
	public const float SECS_PER_DAY = 86400f;

	// Token: 0x04002840 RID: 10304
	public const float SECS_PER_HOUR = 3600f;

	// Token: 0x04002841 RID: 10305
	public const float MINS_PER_SEC = 0.016666668f;

	// Token: 0x04002842 RID: 10306
	public const float HOURS_PER_SEC = 0.00027777778f;

	// Token: 0x04002843 RID: 10307
	public const float HOURS_PER_MIN = 0.016666668f;

	// Token: 0x04002844 RID: 10308
	public const float DAYS_PER_SEC = 1.1574074E-05f;

	// Token: 0x04002845 RID: 10309
	private string dayFormatString;

	// Token: 0x04002846 RID: 10310
	private WorldModel worldModel;

	// Token: 0x04002847 RID: 10311
	private List<TimeDirector.PassedTimeDelegate> passedTimeDelegates = new List<TimeDirector.PassedTimeDelegate>();

	// Token: 0x02000799 RID: 1945
	// (Invoke) Token: 0x060028C9 RID: 10441
	public delegate void OnUnpauseDelegate();

	// Token: 0x0200079A RID: 1946
	// (Invoke) Token: 0x060028CD RID: 10445
	public delegate void OnFastForwardChanged(bool isFastForwarding);

	// Token: 0x0200079B RID: 1947
	private class PassedTimeDelegate
	{
		// Token: 0x04002848 RID: 10312
		public double time;

		// Token: 0x04002849 RID: 10313
		public Action action;
	}
}
