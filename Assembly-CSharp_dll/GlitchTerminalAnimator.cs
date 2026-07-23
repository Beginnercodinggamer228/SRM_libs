using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.Util.Extensions;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020004FE RID: 1278
public class GlitchTerminalAnimator : SRAnimator, PlayerModel.Participant
{
	// Token: 0x170001F2 RID: 498
	// (get) Token: 0x06001AC0 RID: 6848 RVA: 0x000677C3 File Offset: 0x000659C3
	// (set) Token: 0x06001AC1 RID: 6849 RVA: 0x000677CB File Offset: 0x000659CB
	public GlitchTerminalActivator activator { get; private set; }

	// Token: 0x06001AC2 RID: 6850 RVA: 0x000677D4 File Offset: 0x000659D4
	public override void Awake()
	{
		base.Awake();
		this.activator = base.GetRequiredComponentInChildren<GlitchTerminalActivator>(false);
		SRSingleton<SceneContext>.Instance.GameModel.RegisterPlayerParticipant(this);
	}

	// Token: 0x1400001B RID: 27
	// (add) Token: 0x06001AC3 RID: 6851 RVA: 0x000677FC File Offset: 0x000659FC
	// (remove) Token: 0x06001AC4 RID: 6852 RVA: 0x00067834 File Offset: 0x00065A34
	public event GlitchTerminalAnimator.OnStateChanged onStateEnter;

	// Token: 0x06001AC5 RID: 6853 RVA: 0x00067869 File Offset: 0x00065A69
	public void OnStateEnter(GlitchTerminalAnimatorState.Id id)
	{
		if (this.onStateEnter != null)
		{
			this.onStateEnter(id);
		}
	}

	// Token: 0x06001AC6 RID: 6854 RVA: 0x00067880 File Offset: 0x00065A80
	public void OnEnter(Transform destinationTransform)
	{
		GlitchTerminalAnimator_Player fx = GlitchTerminalAnimator.InstantiatePlayerFX();
		SRSingleton<SceneContext>.Instance.StartCoroutine(this.OnEnter_Coroutine_FX(fx, destinationTransform));
		SRSingleton<SceneContext>.Instance.StartCoroutine(this.OnEnter_Coroutine_Region(fx));
	}

	// Token: 0x06001AC7 RID: 6855 RVA: 0x000678B8 File Offset: 0x00065AB8
	private IEnumerator OnEnter_Coroutine_FX(GlitchTerminalAnimator_Player fx, Transform destinationTransform)
	{
		GlitchMetadata glitch = SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch;
		using (new GlitchTerminalAnimator.TemporaryLockInputMode(base.gameObject.GetInstanceID()))
		{
			using (new GlitchTerminalAnimator.TemporaryReplaceSeaMaterial())
			{
				SECTR_AudioSystem.Play(glitch.animationOnTeleportInCue, fx.transform, Vector3.zero, false, null, false);
				fx.animator.SetTrigger("trigger_enter_slimulation");
				yield return fx.WaitForStateExit(GlitchTerminalAnimator_PlayerState.Id.ENTERING);
				GlitchTerminalAnimator.TeleportTo(destinationTransform, RegionRegistry.RegionSetId.SLIMULATIONS);
				yield return fx.WaitForStateExit(GlitchTerminalAnimator_PlayerState.Id.EXITING);
				Destroyer.Destroy(fx.gameObject, "GlitchTerminalAnimator.OnEnter_Coroutine");
			}
			GlitchTerminalAnimator.TemporaryReplaceSeaMaterial temporaryReplaceSeaMaterial = null;
		}
		GlitchTerminalAnimator.TemporaryLockInputMode temporaryLockInputMode = null;
		yield break;
		yield break;
	}

	// Token: 0x06001AC8 RID: 6856 RVA: 0x000678D5 File Offset: 0x00065AD5
	private IEnumerator OnEnter_Coroutine_Region(GlitchTerminalAnimator_Player fx)
	{
		base.animator.SetBool("state_in_slimulation", true);
		Region region = base.gameObject.GetRequiredComponentInParent(false);
		yield return fx.WaitForAnimationEvent(GlitchTerminalAnimator_Player.AnimationEvent.ENTERING_FULLY_COVERED);
		region.OnRegionSetDeactivated();
		yield break;
	}

	// Token: 0x06001AC9 RID: 6857 RVA: 0x000678EB File Offset: 0x00065AEB
	public static void OnExit(Action onMidpoint, Action onComplete, int sourceObjectId)
	{
		SRSingleton<SceneContext>.Instance.StartCoroutine(GlitchTerminalAnimator.OnExit_Coroutine_FX(onMidpoint, onComplete, sourceObjectId));
	}

	// Token: 0x06001ACA RID: 6858 RVA: 0x00067900 File Offset: 0x00065B00
	private static IEnumerator OnExit_Coroutine_FX(Action onMidpoint, Action onComplete, int sourceObjectId)
	{
		GlitchMetadata glitch = SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch;
		GlitchRegionHelper_Viktor instance = SRSingleton<GlitchRegionHelper_Viktor>.Instance;
		Transform destinationTransform = instance.activator.destinationTransform;
		GlitchTerminalAnimator_Player fx = GlitchTerminalAnimator.InstantiatePlayerFX();
		using (new GlitchTerminalAnimator.TemporaryLockInputMode(sourceObjectId))
		{
			using (new GlitchTerminalAnimator.TemporaryReplaceSeaMaterial())
			{
				SECTR_AudioSystem.Play(glitch.animationOnTeleportOutCue, fx.transform, Vector3.zero, false, null, false);
				fx.animator.SetTrigger("trigger_exit_slimulation");
				yield return fx.WaitForStateExit(GlitchTerminalAnimator_PlayerState.Id.EXITING);
				GlitchTerminalAnimator.TeleportTo(destinationTransform, RegionRegistry.RegionSetId.VIKTOR_LAB);
				if (onMidpoint != null)
				{
					onMidpoint();
				}
				yield return fx.WaitForStateExit(GlitchTerminalAnimator_PlayerState.Id.ENTERING);
				Destroyer.Destroy(fx.gameObject, "GlitchTerminalAnimator.OnExit_Coroutine");
			}
			GlitchTerminalAnimator.TemporaryReplaceSeaMaterial temporaryReplaceSeaMaterial = null;
		}
		GlitchTerminalAnimator.TemporaryLockInputMode temporaryLockInputMode = null;
		if (onComplete != null)
		{
			onComplete();
		}
		yield break;
		yield break;
	}

	// Token: 0x06001ACB RID: 6859 RVA: 0x0006791D File Offset: 0x00065B1D
	public void RegionSetChanged(RegionRegistry.RegionSetId previous, RegionRegistry.RegionSetId current)
	{
		if (current == RegionRegistry.RegionSetId.VIKTOR_LAB)
		{
			base.animator.SetBool("state_in_slimulation", false);
		}
	}

	// Token: 0x06001ACC RID: 6860 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(PlayerModel model)
	{
	}

	// Token: 0x06001ACD RID: 6861 RVA: 0x00003296 File Offset: 0x00001496
	public void SetModel(PlayerModel model)
	{
	}

	// Token: 0x06001ACE RID: 6862 RVA: 0x00003296 File Offset: 0x00001496
	public void TransformChanged(Vector3 position, Quaternion rotation)
	{
	}

	// Token: 0x06001ACF RID: 6863 RVA: 0x00003296 File Offset: 0x00001496
	public void RegisteredPotentialAmmoChanged(Dictionary<PlayerState.AmmoMode, List<GameObject>> ammo)
	{
	}

	// Token: 0x06001AD0 RID: 6864 RVA: 0x00003296 File Offset: 0x00001496
	public void KeyAdded()
	{
	}

	// Token: 0x06001AD1 RID: 6865 RVA: 0x00067934 File Offset: 0x00065B34
	private static void TeleportTo(Transform destinationTransform, RegionRegistry.RegionSetId regionSetId)
	{
		TeleportablePlayer requiredComponent = SRSingleton<SceneContext>.Instance.Player.GetRequiredComponent<TeleportablePlayer>();
		Vector3 position = destinationTransform.position;
		Vector3? rotation = new Vector3?(destinationTransform.rotation.eulerAngles);
		requiredComponent.TeleportTo(position, regionSetId, rotation, true, false);
	}

	// Token: 0x06001AD2 RID: 6866 RVA: 0x00067974 File Offset: 0x00065B74
	private static GlitchTerminalAnimator_Player InstantiatePlayerFX()
	{
		return UnityEngine.Object.Instantiate<GameObject>(SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch.animationFX, SRSingleton<SceneContext>.Instance.Player.transform, false).GetRequiredComponent<GlitchTerminalAnimator_Player>();
	}

	// Token: 0x04001A46 RID: 6726
	public const string STATE_SLEEPING = "state_sleeping";

	// Token: 0x04001A47 RID: 6727
	public const string STATE_IN_SLIMULATION = "state_in_slimulation";

	// Token: 0x020004FF RID: 1279
	// (Invoke) Token: 0x06001AD5 RID: 6869
	public delegate void OnStateChanged(GlitchTerminalAnimatorState.Id id);

	// Token: 0x02000500 RID: 1280
	private class TemporaryLockInputMode : IDisposable
	{
		// Token: 0x06001AD8 RID: 6872 RVA: 0x000679A4 File Offset: 0x00065BA4
		public TemporaryLockInputMode(int inputModeHandle)
		{
			this.inputModeHandle = inputModeHandle;
			SRInput.Instance.SetInputMode(SRInput.InputMode.LOOK_ONLY, inputModeHandle);
		}

		// Token: 0x06001AD9 RID: 6873 RVA: 0x000679BF File Offset: 0x00065BBF
		public void Dispose()
		{
			SRInput.Instance.ClearInputMode(this.inputModeHandle);
		}

		// Token: 0x04001A48 RID: 6728
		private readonly int inputModeHandle;
	}

	// Token: 0x02000501 RID: 1281
	private class TemporaryReplaceSeaMaterial : IDisposable
	{
		// Token: 0x06001ADA RID: 6874 RVA: 0x000679D4 File Offset: 0x00065BD4
		public TemporaryReplaceSeaMaterial()
		{
			GlitchMetadata glitch = SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch;
			GlitchRegionHelper instance = SRSingleton<GlitchRegionHelper>.Instance;
			this.renderer = instance.seaRenderer;
			this.previousMaterial = this.renderer.sharedMaterial;
			this.renderer.sharedMaterial = glitch.animationSeaMaterial;
		}

		// Token: 0x06001ADB RID: 6875 RVA: 0x00067A2B File Offset: 0x00065C2B
		public void Dispose()
		{
			this.renderer.sharedMaterial = this.previousMaterial;
		}

		// Token: 0x04001A49 RID: 6729
		private readonly Renderer renderer;

		// Token: 0x04001A4A RID: 6730
		private readonly Material previousMaterial;
	}
}
