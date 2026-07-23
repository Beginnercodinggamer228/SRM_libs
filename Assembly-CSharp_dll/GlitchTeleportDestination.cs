using System;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020004F7 RID: 1271
public class GlitchTeleportDestination : SRBehaviour, GlitchTeleportDestinationModel.Participant
{
	// Token: 0x1400001A RID: 26
	// (add) Token: 0x06001AA2 RID: 6818 RVA: 0x00067218 File Offset: 0x00065418
	// (remove) Token: 0x06001AA3 RID: 6819 RVA: 0x00067250 File Offset: 0x00065450
	public event GlitchTeleportDestination.OnExitTeleporterBecameActiveDelegate onExitTeleporterBecameActive;

	// Token: 0x170001F0 RID: 496
	// (get) Token: 0x06001AA4 RID: 6820 RVA: 0x00067285 File Offset: 0x00065485
	public string id
	{
		get
		{
			return base.GetRequiredComponent<IdHandler>().id;
		}
	}

	// Token: 0x170001F1 RID: 497
	// (get) Token: 0x06001AA5 RID: 6821 RVA: 0x00067292 File Offset: 0x00065492
	// (set) Token: 0x06001AA6 RID: 6822 RVA: 0x0006729A File Offset: 0x0006549A
	public bool isPotentialExitDestination { get; set; }

	// Token: 0x06001AA7 RID: 6823 RVA: 0x000672A4 File Offset: 0x000654A4
	public void Awake()
	{
		this.tutorialDirector = SRSingleton<SceneContext>.Instance.TutorialDirector;
		this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.isPotentialExitDestination = true;
		this.exitActiveFX.SetActive(false);
		SRSingleton<SceneContext>.Instance.GameModel.Glitch.Register(this);
	}

	// Token: 0x06001AA8 RID: 6824 RVA: 0x000672F9 File Offset: 0x000654F9
	public void InitModel(GlitchTeleportDestinationModel model)
	{
		model.activationTime = null;
	}

	// Token: 0x06001AA9 RID: 6825 RVA: 0x00067307 File Offset: 0x00065507
	public void SetModel(GlitchTeleportDestinationModel model)
	{
		this.model = model;
		if (SRSingleton<SceneContext>.Instance.GameModel.GetPlayerModel().currRegionSetId == RegionRegistry.RegionSetId.SLIMULATIONS)
		{
			this.Reset(this.model.activationTime);
		}
	}

	// Token: 0x06001AAA RID: 6826 RVA: 0x00067338 File Offset: 0x00065538
	public void OnDestroy()
	{
		if (SRSingleton<SceneContext>.Instance != null)
		{
			SRSingleton<SceneContext>.Instance.GameModel.Glitch.Unregister(this);
			this.timeDirector.RemovePassedTimeDelegate(new Action(this.OnExitTeleporterBecameActive));
		}
	}

	// Token: 0x06001AAB RID: 6827 RVA: 0x00067373 File Offset: 0x00065573
	public void OnTriggerEnter(Collider collider)
	{
		if (this.IsLinkActive() && PhysicsUtil.IsPlayerMainCollider(collider))
		{
			GlitchTerminalAnimator.OnExit(null, null, base.gameObject.GetInstanceID());
		}
	}

	// Token: 0x06001AAC RID: 6828 RVA: 0x00067397 File Offset: 0x00065597
	public bool IsLinkActive()
	{
		return this.GetCurrentState() == GlitchTeleportDestination.State.ACTIVATED;
	}

	// Token: 0x06001AAD RID: 6829 RVA: 0x000673A4 File Offset: 0x000655A4
	public void Reset(double? activationTime)
	{
		this.model.activationTime = activationTime;
		GlitchTeleportDestination.State currentState = this.GetCurrentState();
		this.exitActiveFX.SetActive(currentState == GlitchTeleportDestination.State.ACTIVATED);
		bool isPotentialExitDestination = this.isPotentialExitDestination;
		double num = this.timeDirector.WorldTime();
		double? activationTime2 = this.model.activationTime;
		this.isPotentialExitDestination = (isPotentialExitDestination | (num >= activationTime2.GetValueOrDefault() & activationTime2 != null));
		this.timeDirector.RemovePassedTimeDelegate(new Action(this.OnExitTeleporterBecameActive));
		if (currentState == GlitchTeleportDestination.State.PREACTIVATED)
		{
			this.timeDirector.AddPassedTimeDelegate(this.model.activationTime.Value, new Action(this.OnExitTeleporterBecameActive));
			return;
		}
		if (currentState == GlitchTeleportDestination.State.ACTIVATED)
		{
			this.OnExitTeleporterBecameActive();
		}
	}

	// Token: 0x06001AAE RID: 6830 RVA: 0x00067458 File Offset: 0x00065658
	private void OnExitTeleporterBecameActive()
	{
		SRSingleton<GlitchRegionHelper>.Instance.OnExitTeleporterBecameActive();
		this.exitActiveFX.SetActive(true);
		this.tutorialDirector.MaybeShowPopup(TutorialDirector.Id.SLIMULATIONS_EXIT_AVAILABLE);
		if (this.onExitTeleporterBecameActive != null)
		{
			this.onExitTeleporterBecameActive(this);
		}
	}

	// Token: 0x06001AAF RID: 6831 RVA: 0x00067494 File Offset: 0x00065694
	private GlitchTeleportDestination.State GetCurrentState()
	{
		double num = this.timeDirector.WorldTime();
		double? activationTime = this.model.activationTime;
		if (num < activationTime.GetValueOrDefault() & activationTime != null)
		{
			return GlitchTeleportDestination.State.PREACTIVATED;
		}
		double num2 = this.timeDirector.WorldTime();
		activationTime = this.model.activationTime;
		if (!(num2 >= activationTime.GetValueOrDefault() & activationTime != null))
		{
			return GlitchTeleportDestination.State.DEACTIVATED;
		}
		return GlitchTeleportDestination.State.ACTIVATED;
	}

	// Token: 0x04001A23 RID: 6691
	[Tooltip("Teleport destination transform.")]
	public Transform destinationTransform;

	// Token: 0x04001A24 RID: 6692
	[Tooltip("FX parent when the destination is activated.")]
	public GameObject exitActiveFX;

	// Token: 0x04001A27 RID: 6695
	private GlitchTeleportDestinationModel model;

	// Token: 0x04001A28 RID: 6696
	private TutorialDirector tutorialDirector;

	// Token: 0x04001A29 RID: 6697
	private TimeDirector timeDirector;

	// Token: 0x020004F8 RID: 1272
	// (Invoke) Token: 0x06001AB2 RID: 6834
	public delegate void OnExitTeleporterBecameActiveDelegate(GlitchTeleportDestination destination);

	// Token: 0x020004F9 RID: 1273
	private enum State
	{
		// Token: 0x04001A2B RID: 6699
		DEACTIVATED,
		// Token: 0x04001A2C RID: 6700
		PREACTIVATED,
		// Token: 0x04001A2D RID: 6701
		ACTIVATED
	}
}
