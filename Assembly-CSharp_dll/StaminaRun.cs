using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020002DA RID: 730
public class StaminaRun : SRBehaviour, EventHandlerRegistrable, PlayerModel.Participant
{
	// Token: 0x06000F90 RID: 3984 RVA: 0x0003D75B File Offset: 0x0003B95B
	protected virtual void Awake()
	{
		this.playerEvents = base.GetComponent<vp_FPPlayerEventHandler>();
		this.controller = base.GetComponent<vp_FPController>();
		this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
		SRSingleton<SceneContext>.Instance.GameModel.RegisterPlayerParticipant(this);
	}

	// Token: 0x06000F91 RID: 3985 RVA: 0x0003D795 File Offset: 0x0003B995
	protected virtual void Start()
	{
		this.playerState = SRSingleton<SceneContext>.Instance.PlayerState;
		this.runStaminaThreshold = this.runThreshold * this.runningStaminaPerSecond;
	}

	// Token: 0x06000F92 RID: 3986 RVA: 0x0003D7BA File Offset: 0x0003B9BA
	protected virtual void OnEnable()
	{
		if (this.playerEvents != null)
		{
			this.Register(this.playerEvents);
		}
	}

	// Token: 0x06000F93 RID: 3987 RVA: 0x0003D7D6 File Offset: 0x0003B9D6
	protected virtual void OnDisable()
	{
		if (this.playerEvents != null)
		{
			this.Unregister(this.playerEvents);
		}
	}

	// Token: 0x06000F94 RID: 3988 RVA: 0x0003D7F4 File Offset: 0x0003B9F4
	public void Update()
	{
		if (this.playerEvents.Run.Active)
		{
			bool flag = this.TooSlow();
			if (this.timeDirector.HasReached(this.model.runEnergyDepletionTime) && !flag)
			{
				this.playerState.SpendEnergy(Time.deltaTime * this.runningStaminaPerSecond * this.model.runEfficiency);
			}
			if (!this.CanContinue_Run(1f))
			{
				this.playerEvents.Run.TryStop(true);
			}
		}
	}

	// Token: 0x06000F95 RID: 3989 RVA: 0x0003D877 File Offset: 0x0003BA77
	protected virtual bool CanStart_Run()
	{
		return this.CanContinue_Run(this.runStaminaThreshold);
	}

	// Token: 0x06000F96 RID: 3990 RVA: 0x0003D888 File Offset: 0x0003BA88
	private bool CanContinue_Run(float threshold)
	{
		return (!this.timeDirector.HasReached(this.model.runEnergyDepletionTime) || (float)this.playerState.GetCurrEnergy() >= threshold) && this.controller.Grounded && !this.TooSlow();
	}

	// Token: 0x06000F97 RID: 3991 RVA: 0x0003D8D4 File Offset: 0x0003BAD4
	private bool TooSlow()
	{
		return this.playerEvents.Velocity.Get().sqrMagnitude < 1f;
	}

	// Token: 0x06000F98 RID: 3992 RVA: 0x0003D905 File Offset: 0x0003BB05
	public void Register(vp_EventHandler eventHandler)
	{
		eventHandler.RegisterActivity("Run", null, null, new vp_Activity.Condition(this.CanStart_Run), null, null, null);
	}

	// Token: 0x06000F99 RID: 3993 RVA: 0x0003D924 File Offset: 0x0003BB24
	public void Unregister(vp_EventHandler eventHandler)
	{
		eventHandler.UnregisterActivity("Run", null, null, new vp_Activity.Condition(this.CanStart_Run), null, null, null);
	}

	// Token: 0x06000F9A RID: 3994 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(PlayerModel model)
	{
	}

	// Token: 0x06000F9B RID: 3995 RVA: 0x0003D943 File Offset: 0x0003BB43
	public void SetModel(PlayerModel model)
	{
		this.model = model;
	}

	// Token: 0x06000F9C RID: 3996 RVA: 0x00003296 File Offset: 0x00001496
	public void RegionSetChanged(RegionRegistry.RegionSetId previous, RegionRegistry.RegionSetId current)
	{
	}

	// Token: 0x06000F9D RID: 3997 RVA: 0x00003296 File Offset: 0x00001496
	public void TransformChanged(Vector3 pos, Quaternion rot)
	{
	}

	// Token: 0x06000F9E RID: 3998 RVA: 0x00003296 File Offset: 0x00001496
	public void RegisteredPotentialAmmoChanged(Dictionary<PlayerState.AmmoMode, List<GameObject>> registeredPotentialAmmo)
	{
	}

	// Token: 0x06000F9F RID: 3999 RVA: 0x00003296 File Offset: 0x00001496
	public void KeyAdded()
	{
	}

	// Token: 0x04000E58 RID: 3672
	public float runningStaminaPerSecond = 30f;

	// Token: 0x04000E59 RID: 3673
	public float runThreshold = 1f;

	// Token: 0x04000E5A RID: 3674
	private vp_FPPlayerEventHandler playerEvents;

	// Token: 0x04000E5B RID: 3675
	private PlayerState playerState;

	// Token: 0x04000E5C RID: 3676
	private float runStaminaThreshold;

	// Token: 0x04000E5D RID: 3677
	private vp_FPController controller;

	// Token: 0x04000E5E RID: 3678
	private TimeDirector timeDirector;

	// Token: 0x04000E5F RID: 3679
	private PlayerModel model;

	// Token: 0x04000E60 RID: 3680
	private const float MIN_RUN_VEL = 1f;

	// Token: 0x04000E61 RID: 3681
	private const float SQR_MIN_RUN_VEL = 1f;
}
