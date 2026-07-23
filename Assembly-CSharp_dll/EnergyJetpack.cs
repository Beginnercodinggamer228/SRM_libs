using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x0200029A RID: 666
public class EnergyJetpack : SRBehaviour, EventHandlerRegistrable, PlayerModel.Participant
{
	// Token: 0x06000DFA RID: 3578 RVA: 0x00038BA1 File Offset: 0x00036DA1
	protected virtual void Awake()
	{
		this.playerEvents = base.GetComponent<vp_FPPlayerEventHandler>();
		this.controller = base.GetComponent<vp_FPController>();
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		SRSingleton<SceneContext>.Instance.GameModel.RegisterPlayerParticipant(this);
	}

	// Token: 0x06000DFB RID: 3579 RVA: 0x00038BDB File Offset: 0x00036DDB
	protected virtual void Start()
	{
		this.playerState = SRSingleton<SceneContext>.Instance.PlayerState;
		this.jetpackEnergyThreshold = this.startThreshold * this.energyPerSecond + this.initEnergyUsed;
		this.jetpackLowEnergyThreshold = this.lowEnergyThreshold * this.energyPerSecond;
	}

	// Token: 0x06000DFC RID: 3580 RVA: 0x00038C1A File Offset: 0x00036E1A
	protected virtual void OnEnable()
	{
		if (this.playerEvents != null)
		{
			this.Register(this.playerEvents);
		}
	}

	// Token: 0x06000DFD RID: 3581 RVA: 0x00038C36 File Offset: 0x00036E36
	protected virtual void OnDisable()
	{
		if (this.playerEvents != null)
		{
			this.Unregister(this.playerEvents);
		}
	}

	// Token: 0x06000DFE RID: 3582 RVA: 0x00038C54 File Offset: 0x00036E54
	public void FixedUpdate()
	{
		if (this.playerEvents.Jump.Active && !this.playerEvents.Jetpack.Active && this.playerEvents.Velocity.Get().y <= this.jetpackVelThreshold && this.timeDir.HasReached(this.canKickInJetpackTime))
		{
			this.playerEvents.Jetpack.TryStart(true);
		}
		if (this.playerEvents.Jetpack.Active)
		{
			int currEnergy = this.playerState.GetCurrEnergy();
			this.playerState.SpendEnergy(Time.deltaTime * this.energyPerSecond * this.model.jetpackEfficiency);
			int currEnergy2 = this.playerState.GetCurrEnergy();
			if (currEnergy2 <= 0)
			{
				this.playerEvents.Jetpack.TryStop(true);
			}
			else if (this.jetpackLowEnergyRunCue != null && (float)currEnergy > this.jetpackLowEnergyThreshold * this.model.jetpackEfficiency && (float)currEnergy2 <= this.jetpackLowEnergyThreshold * this.model.jetpackEfficiency)
			{
				this.jetpackAudio.Cue = this.jetpackLowEnergyRunCue;
				this.jetpackAudio.Play();
			}
		}
		if (this.controller.GroundedNonMountain && !this.playerEvents.Jump.Active && !this.playerEvents.Jetpack.Active)
		{
			this.hoverY = float.PositiveInfinity;
		}
		bool active = this.playerEvents.Jetpack.Active;
		if (this.controller.StateEnabled("Jetpack1") != active)
		{
			this.controller.SetState("Jetpack1", active, false, false);
		}
		if (this.playerEvents.Jetpack.Active)
		{
			float num = this.DownwardExtraGravity(this.playerEvents.transform.position.y, this.playerEvents.Velocity.Get().y);
			this.controller.SetTempGravityModifier(num);
			if (this.playerEvents.Velocity.Get().y < 0f && num <= 0f)
			{
				this.controller.AdjustFallSpeed(-this.playerEvents.Velocity.Get().y * 0.003f);
				return;
			}
		}
		else
		{
			this.controller.SetTempGravityModifier(0f);
		}
	}

	// Token: 0x06000DFF RID: 3583 RVA: 0x00038EBC File Offset: 0x000370BC
	protected float DownwardExtraGravity(float y, float yVel)
	{
		float num = y - this.hoverY;
		if (num <= 0f)
		{
			return 0f;
		}
		return num * 0.1f * Mathf.Max(0.5f, yVel);
	}

	// Token: 0x06000E00 RID: 3584 RVA: 0x00038EF4 File Offset: 0x000370F4
	private bool CanStart_Jetpack()
	{
		if (!this.model.hasJetpack)
		{
			return false;
		}
		if ((float)this.playerState.GetCurrEnergy() < this.jetpackEnergyThreshold * this.model.jetpackEfficiency)
		{
			this.jetpackAudio.Cue = this.jetpackNoEnergyCue;
			this.jetpackAudio.Play();
			return false;
		}
		return true;
	}

	// Token: 0x06000E01 RID: 3585 RVA: 0x00038F50 File Offset: 0x00037150
	private void OnStart_Jump()
	{
		this.canKickInJetpackTime = this.timeDir.WorldTime() + 18.0;
		this.hoverY = Mathf.Min(this.hoverY, this.playerEvents.transform.position.y + 7f - 2.2f);
	}

	// Token: 0x06000E02 RID: 3586 RVA: 0x00038FAA File Offset: 0x000371AA
	private void OnStop_Jump()
	{
		this.canKickInJetpackTime = 0.0;
	}

	// Token: 0x06000E03 RID: 3587 RVA: 0x00038FBC File Offset: 0x000371BC
	private void OnStart_Jetpack()
	{
		this.hoverY = Mathf.Min(this.hoverY, this.playerEvents.transform.position.y + 7f - 2.2f);
		this.playerState.SpendEnergy(this.initEnergyUsed * this.model.jetpackEfficiency);
		this.jetpackAudio.Cue = this.jetpackStartCue;
		this.jetpackAudio.Play();
		this.jetpackAudio.Cue = this.jetpackRunCue;
		this.jetpackAudio.Play();
	}

	// Token: 0x06000E04 RID: 3588 RVA: 0x00039050 File Offset: 0x00037250
	private void OnStop_Jetpack()
	{
		this.canKickInJetpackTime = this.timeDir.WorldTime() + 18.0;
		this.jetpackAudio.Cue = this.jetpackEndCue;
		this.jetpackAudio.Play();
	}

	// Token: 0x06000E05 RID: 3589 RVA: 0x0003908C File Offset: 0x0003728C
	public void Register(vp_EventHandler eventHandler)
	{
		eventHandler.RegisterActivity("Jetpack", new vp_Activity.Callback(this.OnStart_Jetpack), new vp_Activity.Callback(this.OnStop_Jetpack), new vp_Activity.Condition(this.CanStart_Jetpack), null, null, null);
		eventHandler.RegisterActivity("Jump", new vp_Activity.Callback(this.OnStart_Jump), new vp_Activity.Callback(this.OnStop_Jump), null, null, null, null);
	}

	// Token: 0x06000E06 RID: 3590 RVA: 0x000390F4 File Offset: 0x000372F4
	public void Unregister(vp_EventHandler eventHandler)
	{
		eventHandler.UnregisterActivity("Jetpack", new vp_Activity.Callback(this.OnStart_Jetpack), new vp_Activity.Callback(this.OnStop_Jetpack), new vp_Activity.Condition(this.CanStart_Jetpack), null, null, null);
		eventHandler.UnregisterActivity("Jump", new vp_Activity.Callback(this.OnStart_Jump), new vp_Activity.Callback(this.OnStop_Jump), null, null, null, null);
	}

	// Token: 0x06000E07 RID: 3591 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(PlayerModel model)
	{
	}

	// Token: 0x06000E08 RID: 3592 RVA: 0x0003915A File Offset: 0x0003735A
	public void SetModel(PlayerModel model)
	{
		this.model = model;
	}

	// Token: 0x06000E09 RID: 3593 RVA: 0x00003296 File Offset: 0x00001496
	public void RegionSetChanged(RegionRegistry.RegionSetId previous, RegionRegistry.RegionSetId current)
	{
	}

	// Token: 0x06000E0A RID: 3594 RVA: 0x00003296 File Offset: 0x00001496
	public void TransformChanged(Vector3 pos, Quaternion rot)
	{
	}

	// Token: 0x06000E0B RID: 3595 RVA: 0x00003296 File Offset: 0x00001496
	public void RegisteredPotentialAmmoChanged(Dictionary<PlayerState.AmmoMode, List<GameObject>> registeredPotentialAmmo)
	{
	}

	// Token: 0x06000E0C RID: 3596 RVA: 0x00003296 File Offset: 0x00001496
	public void KeyAdded()
	{
	}

	// Token: 0x04000D27 RID: 3367
	[Tooltip("Energy used per second of jetpacking.")]
	public float energyPerSecond = 33.3f;

	// Token: 0x04000D28 RID: 3368
	[Tooltip("This much energy is used on initiation of the jetpack.")]
	public float initEnergyUsed = 10f;

	// Token: 0x04000D29 RID: 3369
	[Tooltip("Must have this many seconds of energy available to start a fresh jetpack burst.")]
	public float startThreshold = 0.45f;

	// Token: 0x04000D2A RID: 3370
	[Tooltip("Number of seconds of energy available indicating a low-energy threshold.")]
	public float lowEnergyThreshold;

	// Token: 0x04000D2B RID: 3371
	[Tooltip("The vertical velocity during a jump below which the jetpack kicks in.")]
	public float jetpackVelThreshold = 0.5f;

	// Token: 0x04000D2C RID: 3372
	[Tooltip("Where to play our jetpack audio cue(s).")]
	public SECTR_PointSource jetpackAudio;

	// Token: 0x04000D2D RID: 3373
	[Tooltip("The audio to play on jetpack startup.")]
	public SECTR_AudioCue jetpackStartCue;

	// Token: 0x04000D2E RID: 3374
	[Tooltip("The audio to look during jetpack running.")]
	public SECTR_AudioCue jetpackRunCue;

	// Token: 0x04000D2F RID: 3375
	[Tooltip("The audio to play on jetpack end.")]
	public SECTR_AudioCue jetpackEndCue;

	// Token: 0x04000D30 RID: 3376
	[Tooltip("Audio cue to replace `Jetpack Run Cue` during a low-energy state. (optional)")]
	public SECTR_AudioCue jetpackLowEnergyRunCue;

	// Token: 0x04000D31 RID: 3377
	[Tooltip("Audio cue to play if the jetpack does not successfully start. (optional)")]
	public SECTR_AudioCue jetpackNoEnergyCue;

	// Token: 0x04000D32 RID: 3378
	private vp_FPPlayerEventHandler playerEvents;

	// Token: 0x04000D33 RID: 3379
	private PlayerState playerState;

	// Token: 0x04000D34 RID: 3380
	private float jetpackEnergyThreshold;

	// Token: 0x04000D35 RID: 3381
	private float jetpackLowEnergyThreshold;

	// Token: 0x04000D36 RID: 3382
	private vp_FPController controller;

	// Token: 0x04000D37 RID: 3383
	private TimeDirector timeDir;

	// Token: 0x04000D38 RID: 3384
	private double canKickInJetpackTime;

	// Token: 0x04000D39 RID: 3385
	private float hoverY = float.PositiveInfinity;

	// Token: 0x04000D3A RID: 3386
	private PlayerModel model;

	// Token: 0x04000D3B RID: 3387
	private const float JUMP_TO_JETPACK_PAUSE = 18f;

	// Token: 0x04000D3C RID: 3388
	private const float HOVER_HEIGHT = 7f;

	// Token: 0x04000D3D RID: 3389
	private const float HOVER_SOFTNESS_FUDGE = 2.2f;

	// Token: 0x04000D3E RID: 3390
	private const float GRAV_PER_Y = 0.1f;

	// Token: 0x04000D3F RID: 3391
	private const float FALL_COMPENSATION_PER_FRAME = 0.003f;
}
