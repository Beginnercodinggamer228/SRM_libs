using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002B1 RID: 689
public class PlayerDeathHandler : SRBehaviour, DeathHandler.Interface, PlayerModel.Participant
{
	// Token: 0x06000EA2 RID: 3746 RVA: 0x0003B1A9 File Offset: 0x000393A9
	public void Awake()
	{
		SRSingleton<SceneContext>.Instance.GameModel.RegisterPlayerParticipant(this);
	}

	// Token: 0x06000EA3 RID: 3747 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(PlayerModel model)
	{
	}

	// Token: 0x06000EA4 RID: 3748 RVA: 0x0003B1BB File Offset: 0x000393BB
	public void SetModel(PlayerModel model)
	{
		this.model = model;
	}

	// Token: 0x06000EA5 RID: 3749 RVA: 0x0003B1C4 File Offset: 0x000393C4
	public void OnDeath(DeathHandler.Source source, GameObject sourceGameObject, string stackTrace)
	{
		PlayerDeathHandler.<>c__DisplayClass16_0 CS$<>8__locals1 = new PlayerDeathHandler.<>c__DisplayClass16_0();
		CS$<>8__locals1.<>4__this = this;
		if (this.deathInProgress)
		{
			return;
		}
		this.deathInProgress = true;
		string eventName = "PlayerDeath";
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("DamageType", source);
		dictionary.Add("DamageObject", AnalyticsUtil.GetEventData(sourceGameObject));
		dictionary.Add("HasInventory", SRSingleton<SceneContext>.Instance.PlayerState.Ammo.Any((Identifiable.Id id) => !Identifiable.IsLiquid(id)));
		AnalyticsUtil.CustomEvent(eventName, dictionary, true);
		RegionRegistry.RegionSetId currentRegionSetId = SRSingleton<SceneContext>.Instance.RegionRegistry.GetCurrentRegionSetId();
		CS$<>8__locals1.deathType = ((source == DeathHandler.Source.EMERGENCY_RETURN) ? PlayerDeathHandler.DeathType.EMERGENCY_RETURN : ((currentRegionSetId == RegionRegistry.RegionSetId.SLIMULATIONS) ? PlayerDeathHandler.DeathType.SLIMULATIONS : PlayerDeathHandler.DeathType.DEFAULT));
		PlayerDeathHandler.DeathType deathType = CS$<>8__locals1.deathType;
		if (deathType == PlayerDeathHandler.DeathType.SLIMULATIONS)
		{
			LockOnDeath deathLocker = base.GetComponent<LockOnDeath>();
			deathLocker.Freeze();
			GlitchTerminalAnimator.OnExit(delegate
			{
				CS$<>8__locals1.<>4__this.StartCoroutine(CS$<>8__locals1.<>4__this.ResetPlayer(CS$<>8__locals1.deathType, 0f, null));
			}, delegate
			{
				deathLocker.Unfreeze();
				CS$<>8__locals1.<>4__this.deathInProgress = false;
			}, base.gameObject.GetInstanceID());
			return;
		}
		TimeDirector timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
		PlayerState playerState = SRSingleton<SceneContext>.Instance.PlayerState;
		GameModeSettings modeSettings = SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings();
		if (modeSettings.hoursTilDawnOnDeath)
		{
			float num = SRSingleton<SceneContext>.Instance.TimeDirector.CurrHour();
			if (num < 10f && num >= 6f)
			{
				SRSingleton<SceneContext>.Instance.AchievementsDirector.AddToStat(AchievementsDirector.IntStat.DEATH_BEFORE_10AM, 1);
			}
		}
		SRSingleton<SceneContext>.Instance.AchievementsDirector.AddToStat(AchievementsDirector.GameIntStat.DEATHS, 1);
		MusicDirector musicDirector = SRSingleton<GameContext>.Instance.MusicDirector;
		musicDirector.RegisterSuppressor(this);
		if (this.screenFadedCue != null)
		{
			SECTR_AudioSource component = base.GetComponent<SECTR_AudioSource>();
			component.Cue = this.screenFadedCue;
			component.Play();
		}
		base.GetComponent<LockOnDeath>().LockUntil(Math.Max(modeSettings.hoursTilDawnOnDeath ? timeDirector.GetNextDawnAfterNextDusk() : timeDirector.HoursFromNow(modeSettings.hoursLostOnDeath), timeDirector.WorldTime() + 390.0), 5f, delegate
		{
			musicDirector.DeregisterSuppressor(CS$<>8__locals1.<>4__this);
			CS$<>8__locals1.<>4__this.deathInProgress = false;
		});
		base.StartCoroutine(this.ResetPlayer(CS$<>8__locals1.deathType, 1f, null));
		base.StartCoroutine(this.DisplayDeathUI(CS$<>8__locals1.deathType));
	}

	// Token: 0x06000EA6 RID: 3750 RVA: 0x0003B439 File Offset: 0x00039639
	public void ResetPlayerLocation(float delayTime, UnityAction onComplete)
	{
		base.StartCoroutine(this.ResetPlayer(PlayerDeathHandler.DeathType.RESET_PLAYER_LOCATION, delayTime, onComplete));
	}

	// Token: 0x06000EA7 RID: 3751 RVA: 0x0003B44B File Offset: 0x0003964B
	private IEnumerator ResetPlayer(PlayerDeathHandler.DeathType deathType, float delayTime = 1f, UnityAction onComplete = null)
	{
		if (delayTime > 0f)
		{
			yield return new WaitForSeconds(delayTime);
		}
		base.gameObject.GetComponentInChildren<WeaponVacuum>().DropAllVacced();
		if (deathType != PlayerDeathHandler.DeathType.SLIMULATIONS)
		{
			WakeUpDestination wakeUpDestination = this.GetWakeUpDestination(deathType);
			vp_FPPlayerEventHandler componentInChildren = base.gameObject.GetComponentInChildren<vp_FPPlayerEventHandler>();
			if (componentInChildren != null)
			{
				componentInChildren.Position.Set(wakeUpDestination.transform.position);
				componentInChildren.Rotation.Set(wakeUpDestination.transform.eulerAngles);
				SRSingleton<SceneContext>.Instance.Player.transform.position = wakeUpDestination.transform.position;
				SRSingleton<SceneContext>.Instance.Player.transform.rotation = wakeUpDestination.transform.rotation;
				this.model.SetCurrRegionSet(wakeUpDestination.GetRegionSetId());
			}
		}
		vp_FPController componentInChildren2 = base.gameObject.GetComponentInChildren<vp_FPController>();
		if (componentInChildren2 != null)
		{
			componentInChildren2.Stop();
		}
		SRSingleton<SceneContext>.Instance.AmbianceDirector.ExitAllLiquid();
		GameModeSettings modeSettings = SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings();
		PlayerState playerState = SRSingleton<SceneContext>.Instance.PlayerState;
		if (modeSettings.pctCurrencyLostOnDeath > 0f)
		{
			playerState.SpendCurrency(Mathf.FloorToInt((float)playerState.GetCurrency() * modeSettings.pctCurrencyLostOnDeath), true);
		}
		playerState.SetHealth(playerState.GetMaxHealth());
		playerState.SetRad(0);
		playerState.SetEnergy(playerState.GetMaxEnergy());
		playerState.SetAmmoMode(PlayerState.AmmoMode.DEFAULT);
		if (deathType != PlayerDeathHandler.DeathType.RESET_PLAYER_LOCATION)
		{
			foreach (KeyValuePair<PlayerState.AmmoMode, Ammo> keyValuePair in playerState.GetAmmoDict())
			{
				if (this.onAmmoWillClear != null)
				{
					this.onAmmoWillClear(keyValuePair.Key, keyValuePair.Value, deathType);
				}
				keyValuePair.Value.Clear();
			}
		}
		if (this.onPlayerDeath != null)
		{
			this.onPlayerDeath(deathType);
		}
		if (onComplete != null)
		{
			onComplete();
		}
		yield break;
	}

	// Token: 0x06000EA8 RID: 3752 RVA: 0x0003B46F File Offset: 0x0003966F
	private IEnumerator DisplayDeathUI(PlayerDeathHandler.DeathType deathType)
	{
		yield return new WaitForSeconds(1f);
		GameObject uiObj = UnityEngine.Object.Instantiate<GameObject>(this.deathUIPrefab);
		TMP_Text promptText = uiObj.transform.Find("PromptText").GetComponent<TMP_Text>();
		TMP_Text subText = uiObj.transform.Find("SubText").GetComponent<TMP_Text>();
		MessageBundle bundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("ui");
		promptText.text = bundle.Get("m.knocked_out1");
		subText.text = bundle.Get("m.knocked_out2");
		promptText.GetComponent<CanvasGroup>().DOFade(1f, 0.5f).SetUpdate(true);
		subText.GetComponent<CanvasGroup>().DOFade(1f, 0.5f).SetUpdate(true);
		TutorialDirector tutDir = SRSingleton<SceneContext>.Instance.TutorialDirector;
		tutDir.SuppressTutorials();
		tutDir.OnPlayerDeath(deathType);
		bool endedEarly = false;
		PlayerState.OnEndGame onEndGameFadeDelegate = delegate()
		{
			Destroyer.Destroy(uiObj, "PlayerDeathHandler.DisplayDeathUI.onEndGameFadeDelegate");
			endedEarly = true;
		};
		SRSingleton<SceneContext>.Instance.PlayerState.onEndGame += onEndGameFadeDelegate;
		yield return new WaitForSeconds(4f);
		if (!endedEarly)
		{
			promptText.GetComponent<CanvasGroup>().DOFade(0f, 0.5f).SetUpdate(true);
			subText.GetComponent<CanvasGroup>().DOFade(0f, 0.5f).SetUpdate(true);
		}
		yield return new WaitForSeconds(0.5f);
		tutDir.UnsuppressTutorials();
		if (!endedEarly)
		{
			Destroyer.Destroy(uiObj, "PlayerDeathHandler.DisplayDeathUI");
		}
		SRSingleton<SceneContext>.Instance.PlayerState.onEndGame -= onEndGameFadeDelegate;
		yield break;
	}

	// Token: 0x06000EA9 RID: 3753 RVA: 0x0003B488 File Offset: 0x00039688
	private WakeUpDestination GetWakeUpDestination(PlayerDeathHandler.DeathType deathType)
	{
		if (deathType == PlayerDeathHandler.DeathType.EMERGENCY_RETURN)
		{
			return SRSingleton<SceneContext>.Instance.GetWakeUpDestination();
		}
		RegionMember component = SRSingleton<SceneContext>.Instance.Player.GetComponent<RegionMember>();
		return SRSingleton<SceneContext>.Instance.GetWakeUpDestination(component);
	}

	// Token: 0x06000EAA RID: 3754 RVA: 0x00003296 File Offset: 0x00001496
	public void RegionSetChanged(RegionRegistry.RegionSetId previous, RegionRegistry.RegionSetId current)
	{
	}

	// Token: 0x06000EAB RID: 3755 RVA: 0x00003296 File Offset: 0x00001496
	public void TransformChanged(Vector3 pos, Quaternion rot)
	{
	}

	// Token: 0x06000EAC RID: 3756 RVA: 0x00003296 File Offset: 0x00001496
	public void RegisteredPotentialAmmoChanged(Dictionary<PlayerState.AmmoMode, List<GameObject>> registeredPotentialAmmo)
	{
	}

	// Token: 0x06000EAD RID: 3757 RVA: 0x00003296 File Offset: 0x00001496
	public void KeyAdded()
	{
	}

	// Token: 0x04000DBB RID: 3515
	public PlayerDeathHandler.OnAmmoWillClear onAmmoWillClear;

	// Token: 0x04000DBC RID: 3516
	public PlayerDeathHandler.OnPlayerDeath onPlayerDeath;

	// Token: 0x04000DBD RID: 3517
	public GameObject deathUIPrefab;

	// Token: 0x04000DBE RID: 3518
	public SECTR_AudioCue screenFadedCue;

	// Token: 0x04000DBF RID: 3519
	public const float DEATH_FADE_TIME = 1f;

	// Token: 0x04000DC0 RID: 3520
	public const float DEATH_FF_DELAY_TIME = 5f;

	// Token: 0x04000DC1 RID: 3521
	public const float DEATH_MSG_FADE = 0.5f;

	// Token: 0x04000DC2 RID: 3522
	private const float MIN_DEATH_TIME = 390f;

	// Token: 0x04000DC3 RID: 3523
	private bool deathInProgress;

	// Token: 0x04000DC4 RID: 3524
	private PlayerModel model;

	// Token: 0x020002B2 RID: 690
	public enum DeathType
	{
		// Token: 0x04000DC6 RID: 3526
		DEFAULT,
		// Token: 0x04000DC7 RID: 3527
		SLIMULATIONS,
		// Token: 0x04000DC8 RID: 3528
		RESET_PLAYER_LOCATION,
		// Token: 0x04000DC9 RID: 3529
		EMERGENCY_RETURN
	}

	// Token: 0x020002B3 RID: 691
	// (Invoke) Token: 0x06000EB0 RID: 3760
	public delegate void OnAmmoWillClear(PlayerState.AmmoMode mode, Ammo ammo, PlayerDeathHandler.DeathType deathType);

	// Token: 0x020002B4 RID: 692
	// (Invoke) Token: 0x06000EB4 RID: 3764
	public delegate void OnPlayerDeath(PlayerDeathHandler.DeathType deathType);
}
