using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002A8 RID: 680
public class LockOnDeath : SRSingleton<LockOnDeath>
{
	// Token: 0x06000E63 RID: 3683 RVA: 0x0003A4BC File Offset: 0x000386BC
	public override void Awake()
	{
		base.Awake();
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.achieveDir = SRSingleton<SceneContext>.Instance.AchievementsDirector;
		this.input = SRInput.Instance;
		this.fpController = base.GetComponentInChildren<vp_FPController>();
		this.fpInput = base.GetComponentInChildren<vp_FPInput>();
		this.fpCamera = base.GetComponentInChildren<vp_FPCamera>();
		this.fpEvents = base.GetComponentInChildren<vp_FPPlayerEventHandler>();
		this.vacuum = base.GetComponentInChildren<WeaponVacuum>();
		SRSingleton<SceneContext>.Instance.PlayerState.onEndGame += delegate()
		{
			this.unlockWorldTime = this.timeDir.WorldTime();
		};
	}

	// Token: 0x06000E64 RID: 3684 RVA: 0x0003A554 File Offset: 0x00038754
	public void LockUntil(double unlockWorldTime, float delayTime, UnityAction onComplete = null)
	{
		this.achieveDir.SetStat(AchievementsDirector.GameDoubleStat.LAST_SLEPT, this.timeDir.WorldTime());
		this.unlockWorldTime = unlockWorldTime;
		this.locked = true;
		this.Freeze();
		if (this.onLockChanged != null)
		{
			this.onLockChanged(true);
		}
		this.onLockComplete = onComplete;
		base.StartCoroutine(this.DelayedFastForward(delayTime));
	}

	// Token: 0x06000E65 RID: 3685 RVA: 0x0003A5B5 File Offset: 0x000387B5
	private IEnumerator DelayedFastForward(float delayTime)
	{
		if (delayTime > 0f)
		{
			yield return new WaitForSeconds(delayTime);
		}
		this.timeDir.FastForwardTo(this.unlockWorldTime);
		if (this.timePassingInstance.Active)
		{
			this.timePassingInstance.Stop(true);
		}
		if (this.timePassingCue != null)
		{
			this.timePassingInstance = SECTR_AudioSystem.Play(this.timePassingCue, Vector3.zero, true);
		}
		yield break;
	}

	// Token: 0x06000E66 RID: 3686 RVA: 0x0003A5CC File Offset: 0x000387CC
	public void Update()
	{
		if (this.locked && this.timeDir.HasReached(this.unlockWorldTime))
		{
			this.locked = false;
			this.Unfreeze();
			if (this.onLockChanged != null)
			{
				this.onLockChanged(false);
			}
			this.timePassingInstance.Stop(false);
			if (this.playerWakeCue != null)
			{
				SECTR_AudioSource component = base.GetComponent<SECTR_AudioSource>();
				component.Cue = this.playerWakeCue;
				component.Play();
			}
			SRSingleton<GameContext>.Instance.AutoSaveDirector.SaveAllNow();
			this.achieveDir.SetStat(AchievementsDirector.GameDoubleStat.LAST_AWOKE, this.timeDir.WorldTime());
			if (this.onLockComplete != null)
			{
				this.onLockComplete();
			}
		}
	}

	// Token: 0x06000E67 RID: 3687 RVA: 0x0003A688 File Offset: 0x00038888
	public void Freeze()
	{
		if (this.fpController != null)
		{
			this.fpController.SetState("Freeze", true, false, false);
			this.fpController.Stop();
		}
		if (this.fpCamera != null)
		{
			this.fpCamera.SetState("Freeze", true, false, false);
		}
		this.fpEvents.Jump.Stop(0f);
		this.fpEvents.Jetpack.Stop(0f);
		this.vacuum.enabled = false;
		SECTR_AudioSystem.MuteNonUISFX(true);
		this.previousInputMode = this.input.GetInputMode();
		this.input.SetInputMode(SRInput.InputMode.NONE, base.gameObject.GetInstanceID());
	}

	// Token: 0x06000E68 RID: 3688 RVA: 0x0003A748 File Offset: 0x00038948
	public void Unfreeze()
	{
		if (this.fpController != null)
		{
			this.fpController.SetState("Freeze", false, false, false);
		}
		if (this.fpCamera != null)
		{
			this.fpCamera.SetState("Freeze", false, false, false);
		}
		this.vacuum.enabled = true;
		SECTR_AudioSystem.MuteNonUISFX(false);
		this.input.ClearInputMode(base.gameObject.GetInstanceID());
	}

	// Token: 0x06000E69 RID: 3689 RVA: 0x0003A7BF File Offset: 0x000389BF
	public bool Locked()
	{
		return this.locked;
	}

	// Token: 0x04000D7A RID: 3450
	public LockOnDeath.OnLockChanged onLockChanged;

	// Token: 0x04000D7B RID: 3451
	public SECTR_AudioCue playerWakeCue;

	// Token: 0x04000D7C RID: 3452
	public SECTR_AudioCue timePassingCue;

	// Token: 0x04000D7D RID: 3453
	private bool locked;

	// Token: 0x04000D7E RID: 3454
	private double unlockWorldTime;

	// Token: 0x04000D7F RID: 3455
	private TimeDirector timeDir;

	// Token: 0x04000D80 RID: 3456
	private AchievementsDirector achieveDir;

	// Token: 0x04000D81 RID: 3457
	private vp_FPController fpController;

	// Token: 0x04000D82 RID: 3458
	private vp_FPInput fpInput;

	// Token: 0x04000D83 RID: 3459
	private vp_FPCamera fpCamera;

	// Token: 0x04000D84 RID: 3460
	private vp_FPPlayerEventHandler fpEvents;

	// Token: 0x04000D85 RID: 3461
	private WeaponVacuum vacuum;

	// Token: 0x04000D86 RID: 3462
	private SECTR_AudioCueInstance timePassingInstance;

	// Token: 0x04000D87 RID: 3463
	private UnityAction onLockComplete;

	// Token: 0x04000D88 RID: 3464
	private SRInput input;

	// Token: 0x04000D89 RID: 3465
	private SRInput.InputMode previousInputMode;

	// Token: 0x020002A9 RID: 681
	// (Invoke) Token: 0x06000E6D RID: 3693
	public delegate void OnLockChanged(bool locked);
}
