using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x0200023F RID: 575
public class QuicksilverEnergyGenerator : IdHandler, QuicksilverEnergyGeneratorModel.Participant, VacDisplayTimer.TimeSource
{
	// Token: 0x06000C63 RID: 3171 RVA: 0x0003335A File Offset: 0x0003155A
	public void Awake()
	{
		this.tutDirector = SRSingleton<SceneContext>.Instance.TutorialDirector;
		this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
		SRSingleton<SceneContext>.Instance.GameModel.RegisterGenerator(base.id, this);
	}

	// Token: 0x06000C64 RID: 3172 RVA: 0x00033394 File Offset: 0x00031594
	public void Start()
	{
		QuicksilverEnergyGenerator.allGenerators.Add(this);
		if (SRSingleton<SceneContext>.Instance != null)
		{
			this.deathHandler = SRSingleton<SceneContext>.Instance.Player.GetComponent<PlayerDeathHandler>();
			PlayerDeathHandler playerDeathHandler = this.deathHandler;
			playerDeathHandler.onPlayerDeath = (PlayerDeathHandler.OnPlayerDeath)Delegate.Combine(playerDeathHandler.onPlayerDeath, new PlayerDeathHandler.OnPlayerDeath(this.OnPlayerDeath));
		}
	}

	// Token: 0x06000C65 RID: 3173 RVA: 0x000333F8 File Offset: 0x000315F8
	public void OnDestroy()
	{
		QuicksilverEnergyGenerator.allGenerators.Remove(this);
		if (this.deathHandler != null)
		{
			PlayerDeathHandler playerDeathHandler = this.deathHandler;
			playerDeathHandler.onPlayerDeath = (PlayerDeathHandler.OnPlayerDeath)Delegate.Remove(playerDeathHandler.onPlayerDeath, new PlayerDeathHandler.OnPlayerDeath(this.OnPlayerDeath));
		}
		Destroyer.Destroy(this.countdownUI, "QuicksilverEnergyGenerator.OnDestroy");
	}

	// Token: 0x06000C66 RID: 3174 RVA: 0x00033456 File Offset: 0x00031656
	public void InitModel(QuicksilverEnergyGeneratorModel model)
	{
		model.state = QuicksilverEnergyGenerator.State.INACTIVE;
		model.timer = null;
	}

	// Token: 0x06000C67 RID: 3175 RVA: 0x0003346C File Offset: 0x0003166C
	public void SetModel(QuicksilverEnergyGeneratorModel model)
	{
		this.model = model;
		if (this.model.state == QuicksilverEnergyGenerator.State.ACTIVE || this.model.state == QuicksilverEnergyGenerator.State.COUNTDOWN)
		{
			this.SetState(QuicksilverEnergyGenerator.State.COOLDOWN, false);
			return;
		}
		double? timer = model.timer;
		this.SetState(this.model.state, false);
		this.model.timer = timer;
	}

	// Token: 0x06000C68 RID: 3176 RVA: 0x000334CC File Offset: 0x000316CC
	public bool Activate()
	{
		if (this.model.state == QuicksilverEnergyGenerator.State.INACTIVE)
		{
			this.tutDirector.MaybeShowPopup(TutorialDirector.Id.RACE_GENERATOR);
			this.tutDirector.OnQuicksilverRaceActivated();
			SRSingleton<GameContext>.Instance.MusicDirector.SetValleyRaceMode(true);
			this.onStateChanged = (QuicksilverEnergyGenerator.OnStateChanged)Delegate.Combine(this.onStateChanged, new QuicksilverEnergyGenerator.OnStateChanged(this.DisableRaceMusicOnStateChanged));
			this.SetState(QuicksilverEnergyGenerator.State.COUNTDOWN, true);
			return true;
		}
		return false;
	}

	// Token: 0x06000C69 RID: 3177 RVA: 0x0003353C File Offset: 0x0003173C
	private void DisableRaceMusicOnStateChanged()
	{
		if (this.model.state == QuicksilverEnergyGenerator.State.COOLDOWN || this.model.state == QuicksilverEnergyGenerator.State.INACTIVE)
		{
			SRSingleton<GameContext>.Instance.MusicDirector.SetValleyRaceMode(false);
			this.onStateChanged = (QuicksilverEnergyGenerator.OnStateChanged)Delegate.Remove(this.onStateChanged, new QuicksilverEnergyGenerator.OnStateChanged(this.DisableRaceMusicOnStateChanged));
		}
	}

	// Token: 0x06000C6A RID: 3178 RVA: 0x00033598 File Offset: 0x00031798
	public bool ExtendActiveDuration(float hours)
	{
		if (this.model.state == QuicksilverEnergyGenerator.State.ACTIVE)
		{
			this.tutDirector.MaybeShowPopup(TutorialDirector.Id.RACE_CHECKPOINT);
			this.model.timer = new double?(TimeDirector.HoursFromTime(hours, this.model.timer.Value));
			return true;
		}
		return false;
	}

	// Token: 0x06000C6B RID: 3179 RVA: 0x000335EC File Offset: 0x000317EC
	public void Update()
	{
		if (this.model.timer != null && this.timeDirector.HasReached(this.model.timer.Value))
		{
			this.SetState((this.model.state + 1) % (QuicksilverEnergyGenerator.State)Enum.GetNames(typeof(QuicksilverEnergyGenerator.State)).Length, true);
		}
	}

	// Token: 0x06000C6C RID: 3180 RVA: 0x0003364E File Offset: 0x0003184E
	public QuicksilverEnergyGenerator.State GetState()
	{
		return this.model.state;
	}

	// Token: 0x06000C6D RID: 3181 RVA: 0x0003365C File Offset: 0x0003185C
	private void SetState(QuicksilverEnergyGenerator.State state, bool enableSFX)
	{
		Destroyer.Destroy(this.countdownUI, "QuicksilverEnergyGenerator.SetState");
		this.model.state = state;
		if (this.model.state == QuicksilverEnergyGenerator.State.COUNTDOWN)
		{
			this.model.timer = new double?(this.timeDirector.HoursFromNow(this.countdownMinutes * 0.016666668f));
			if (enableSFX)
			{
				SECTR_AudioSystem.Play(this.onCountdownCue, base.transform.position, false);
			}
			SRSingleton<SceneContext>.Instance.Player.GetComponentInChildren<WeaponVacuum>().GetComponentInChildren<VacDisplayTimer>().SetQuicksilverEnergyGenerator(this);
			this.countdownUI = UnityEngine.Object.Instantiate<GameObject>(this.countdownUIPrefab);
			this.countdownUI.GetComponent<HUDCountdownUI>().SetCountdownTime((double)this.countdownMinutes);
		}
		else if (this.model.state == QuicksilverEnergyGenerator.State.ACTIVE)
		{
			this.model.timer = new double?(this.timeDirector.HoursFromNow(this.activeHours));
		}
		else if (this.model.state == QuicksilverEnergyGenerator.State.COOLDOWN)
		{
			this.model.timer = new double?(this.timeDirector.HoursFromNow(this.cooldownHours));
			if (enableSFX)
			{
				SECTR_AudioSystem.Play(this.onCooldownCue, base.transform.position, false);
				SECTR_AudioSystem.Play(this.onCooldownCue2D, Vector3.zero, false);
			}
		}
		else
		{
			this.model.timer = null;
			if (enableSFX)
			{
				SECTR_AudioSystem.Play(this.onInactiveCue, base.transform.position, false);
			}
		}
		if (this.inactiveFX != null)
		{
			this.inactiveFX.SetActive(this.model.state == QuicksilverEnergyGenerator.State.INACTIVE);
		}
		if (this.activeFX != null)
		{
			this.activeFX.SetActive(this.model.state == QuicksilverEnergyGenerator.State.ACTIVE);
		}
		if (this.cooldownFX != null)
		{
			this.cooldownFX.SetActive(this.model.state == QuicksilverEnergyGenerator.State.COOLDOWN);
		}
		if (this.onStateChanged != null)
		{
			this.onStateChanged();
		}
	}

	// Token: 0x06000C6E RID: 3182 RVA: 0x00033865 File Offset: 0x00031A65
	private void OnPlayerDeath(PlayerDeathHandler.DeathType deathType)
	{
		if (this.model.state == QuicksilverEnergyGenerator.State.ACTIVE || this.model.state == QuicksilverEnergyGenerator.State.COUNTDOWN)
		{
			this.SetState(QuicksilverEnergyGenerator.State.COOLDOWN, false);
		}
	}

	// Token: 0x06000C6F RID: 3183 RVA: 0x0003388C File Offset: 0x00031A8C
	public double? GetTimeRemaining()
	{
		if (this.model.timer != null)
		{
			return new double?(this.model.timer.Value - this.timeDirector.WorldTime());
		}
		return null;
	}

	// Token: 0x06000C70 RID: 3184 RVA: 0x000338D8 File Offset: 0x00031AD8
	public double? GetMaxTimeRemaining()
	{
		switch (this.model.state)
		{
		case QuicksilverEnergyGenerator.State.COUNTDOWN:
			return new double?((double)(this.countdownMinutes * 60f));
		case QuicksilverEnergyGenerator.State.ACTIVE:
			return new double?((double)(this.activeHours * 3600f));
		case QuicksilverEnergyGenerator.State.COOLDOWN:
			return new double?((double)(this.cooldownHours * 3600f));
		default:
			return null;
		}
	}

	// Token: 0x06000C71 RID: 3185 RVA: 0x0003394C File Offset: 0x00031B4C
	public double? GetWarningTimeSeconds()
	{
		return null;
	}

	// Token: 0x06000C72 RID: 3186 RVA: 0x00033962 File Offset: 0x00031B62
	protected override string IdPrefix()
	{
		return "qseg";
	}

	// Token: 0x04000B43 RID: 2883
	[Tooltip("Duration, in game minutes, that the game will countdown until the race begins.")]
	public float countdownMinutes;

	// Token: 0x04000B44 RID: 2884
	[Tooltip("Duration, in game hours, that the generator will stay active.")]
	public float activeHours;

	// Token: 0x04000B45 RID: 2885
	[Tooltip("Duration, in game hours, that the generator will cooldown.")]
	public float cooldownHours;

	// Token: 0x04000B46 RID: 2886
	[Tooltip("FX to display when the generator is inactive. (optional)")]
	public GameObject inactiveFX;

	// Token: 0x04000B47 RID: 2887
	[Tooltip("FX to display when the generator is active. (optional)")]
	public GameObject activeFX;

	// Token: 0x04000B48 RID: 2888
	[Tooltip("FX to display when the generator is cooling down. (optional)")]
	public GameObject cooldownFX;

	// Token: 0x04000B49 RID: 2889
	[Tooltip("CountdownUI prefab.")]
	public GameObject countdownUIPrefab;

	// Token: 0x04000B4A RID: 2890
	private GameObject countdownUI;

	// Token: 0x04000B4B RID: 2891
	[Tooltip("SFX played when the generator is ready to be activated. (optional)")]
	public SECTR_AudioCue onInactiveCue;

	// Token: 0x04000B4C RID: 2892
	[Tooltip("SFX played when the countdown timer begins. (optional)")]
	public SECTR_AudioCue onCountdownCue;

	// Token: 0x04000B4D RID: 2893
	[Tooltip("SFX played when the cooldown timer begins. (optional)")]
	public SECTR_AudioCue onCooldownCue;

	// Token: 0x04000B4E RID: 2894
	[Tooltip("SFX played when the cooldown timer begins. (2D, optional)")]
	public SECTR_AudioCue onCooldownCue2D;

	// Token: 0x04000B4F RID: 2895
	public static List<QuicksilverEnergyGenerator> allGenerators = new List<QuicksilverEnergyGenerator>();

	// Token: 0x04000B50 RID: 2896
	public QuicksilverEnergyGenerator.OnStateChanged onStateChanged;

	// Token: 0x04000B51 RID: 2897
	private TimeDirector timeDirector;

	// Token: 0x04000B52 RID: 2898
	private PlayerDeathHandler deathHandler;

	// Token: 0x04000B53 RID: 2899
	private TutorialDirector tutDirector;

	// Token: 0x04000B54 RID: 2900
	private QuicksilverEnergyGeneratorModel model;

	// Token: 0x02000240 RID: 576
	public enum State
	{
		// Token: 0x04000B56 RID: 2902
		INACTIVE,
		// Token: 0x04000B57 RID: 2903
		COUNTDOWN,
		// Token: 0x04000B58 RID: 2904
		ACTIVE,
		// Token: 0x04000B59 RID: 2905
		COOLDOWN
	}

	// Token: 0x02000241 RID: 577
	public class StateComparer : IEqualityComparer<QuicksilverEnergyGenerator.State>
	{
		// Token: 0x06000C75 RID: 3189 RVA: 0x00017781 File Offset: 0x00015981
		public bool Equals(QuicksilverEnergyGenerator.State a, QuicksilverEnergyGenerator.State b)
		{
			return a == b;
		}

		// Token: 0x06000C76 RID: 3190 RVA: 0x00017787 File Offset: 0x00015987
		public int GetHashCode(QuicksilverEnergyGenerator.State state)
		{
			return (int)state;
		}

		// Token: 0x04000B5A RID: 2906
		public static QuicksilverEnergyGenerator.StateComparer Instance = new QuicksilverEnergyGenerator.StateComparer();
	}

	// Token: 0x02000242 RID: 578
	// (Invoke) Token: 0x06000C7A RID: 3194
	public delegate void OnStateChanged();
}
