using System;
using System.Linq;
using UnityEngine;

// Token: 0x020004FA RID: 1274
public class GlitchTerminalActivator : SRBehaviour, TechActivator
{
	// Token: 0x06001AB5 RID: 6837 RVA: 0x00067500 File Offset: 0x00065700
	public void Awake()
	{
		this.progressDirector = SRSingleton<SceneContext>.Instance.ProgressDirector;
		this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.metadata = SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch;
		this.animator = base.GetRequiredComponentInParent<GlitchTerminalAnimator>(false);
		this.buttonAnimator = base.GetRequiredComponentInParent<Animator>(false);
		this.buttonAnimationId = Animator.StringToHash("ButtonPressed");
	}

	// Token: 0x06001AB6 RID: 6838 RVA: 0x0006756C File Offset: 0x0006576C
	public void Start()
	{
		this.destinations = (from id in this.destinationIds
		select SRSingleton<GlitchRegionHelper>.Instance.destinationsDict[id]).ToArray<GlitchTeleportDestination>();
	}

	// Token: 0x06001AB7 RID: 6839 RVA: 0x000675A4 File Offset: 0x000657A4
	public void Activate()
	{
		if (this.onButtonPressedFXInstance != null)
		{
			return;
		}
		bool flag = this.GetLinkState() == GlitchTerminalActivator.LinkState.ACTIVE;
		SECTR_AudioSystem.Play(flag ? this.onButtonPressedSuccessCue : this.onButtonPressedFailureCue, base.transform.position, false);
		this.buttonAnimator.SetTrigger(this.buttonAnimationId);
		if (flag && this.onButtonPressedSuccessFX != null)
		{
			this.onButtonPressedFXInstance = SRBehaviour.SpawnAndPlayFX(this.onButtonPressedSuccessFX, base.transform.position, Quaternion.identity);
		}
		else if (!flag && this.onButtonPressedFailureFX != null)
		{
			this.onButtonPressedFXInstance = SRBehaviour.SpawnAndPlayFX(this.onButtonPressedFailureFX, base.transform.position, Quaternion.identity);
		}
		if (!flag)
		{
			return;
		}
		if (!this.progressDirector.HasProgress(ProgressDirector.ProgressType.ENTER_ZONE_SLIMULATION))
		{
			this.progressDirector.SetProgress(ProgressDirector.ProgressType.ENTER_ZONE_SLIMULATION, this.timeDirector.CurrDay());
		}
		GlitchTeleportDestination glitchTeleportDestination = this.destinations[(this.timeDirector.CurrDay() - this.progressDirector.GetProgress(ProgressDirector.ProgressType.ENTER_ZONE_SLIMULATION)) % this.destinations.Length];
		glitchTeleportDestination.isPotentialExitDestination = false;
		this.animator.OnEnter(glitchTeleportDestination.destinationTransform);
	}

	// Token: 0x06001AB8 RID: 6840 RVA: 0x000676DC File Offset: 0x000658DC
	public GameObject GetCustomGuiPrefab()
	{
		switch (this.GetLinkState())
		{
		case GlitchTerminalActivator.LinkState.INACTIVE_PROGRESS:
			return this.metadata.activatorGuiProgress;
		case GlitchTerminalActivator.LinkState.INACTIVE_AMMO:
			return this.metadata.activatorGuiAmmo;
		case GlitchTerminalActivator.LinkState.PRE_ACTIVE:
			return this.metadata.activatorGuiPreActive;
		default:
			return null;
		}
	}

	// Token: 0x06001AB9 RID: 6841 RVA: 0x0006772C File Offset: 0x0006592C
	public GlitchTerminalActivator.LinkState GetLinkState()
	{
		if (!this.progressDirector.HasProgress(ProgressDirector.ProgressType.UNLOCK_SLIMULATIONS))
		{
			return GlitchTerminalActivator.LinkState.INACTIVE_PROGRESS;
		}
		Ammo ammo = SRSingleton<SceneContext>.Instance.PlayerState.Ammo;
		if (Enumerable.Range(0, 4).Any((int ii) => ammo.GetSlotCount(ii) > 0))
		{
			return GlitchTerminalActivator.LinkState.INACTIVE_AMMO;
		}
		if (SRInput.Instance.GetInputMode() == SRInput.InputMode.DEFAULT)
		{
			return GlitchTerminalActivator.LinkState.ACTIVE;
		}
		return GlitchTerminalActivator.LinkState.PRE_ACTIVE;
	}

	// Token: 0x04001A2E RID: 6702
	[Tooltip("Teleport destination transform.")]
	public Transform destinationTransform;

	// Token: 0x04001A2F RID: 6703
	[Tooltip("List of GlitchTeleportDestination ids in SLIMULATIONS representing potential entrance teleporters.")]
	public string[] destinationIds;

	// Token: 0x04001A30 RID: 6704
	[Tooltip("FX played on successful button press.")]
	public GameObject onButtonPressedSuccessFX;

	// Token: 0x04001A31 RID: 6705
	[Tooltip("SFX cue on successful button press.")]
	public SECTR_AudioCue onButtonPressedSuccessCue;

	// Token: 0x04001A32 RID: 6706
	[Tooltip("FX played on unsuccessful button press.")]
	public GameObject onButtonPressedFailureFX;

	// Token: 0x04001A33 RID: 6707
	[Tooltip("SFX cue on unsuccessful button press.")]
	public SECTR_AudioCue onButtonPressedFailureCue;

	// Token: 0x04001A34 RID: 6708
	private ProgressDirector progressDirector;

	// Token: 0x04001A35 RID: 6709
	private TimeDirector timeDirector;

	// Token: 0x04001A36 RID: 6710
	private GlitchMetadata metadata;

	// Token: 0x04001A37 RID: 6711
	private GlitchTerminalAnimator animator;

	// Token: 0x04001A38 RID: 6712
	private GlitchTeleportDestination[] destinations;

	// Token: 0x04001A39 RID: 6713
	private Animator buttonAnimator;

	// Token: 0x04001A3A RID: 6714
	private int buttonAnimationId;

	// Token: 0x04001A3B RID: 6715
	private GameObject onButtonPressedFXInstance;

	// Token: 0x020004FB RID: 1275
	public enum LinkState
	{
		// Token: 0x04001A3D RID: 6717
		INACTIVE_PROGRESS,
		// Token: 0x04001A3E RID: 6718
		INACTIVE_AMMO,
		// Token: 0x04001A3F RID: 6719
		PRE_ACTIVE,
		// Token: 0x04001A40 RID: 6720
		ACTIVE
	}
}
