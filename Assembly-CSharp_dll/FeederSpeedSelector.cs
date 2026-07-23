using System;
using UnityEngine;

// Token: 0x020006FE RID: 1790
public class FeederSpeedSelector : MonoBehaviour, TechActivator
{
	// Token: 0x06002550 RID: 9552 RVA: 0x0008F596 File Offset: 0x0008D796
	public void Awake()
	{
		this.buttonAnimator = base.GetComponentInParent<Animator>();
		this.buttonPressedTriggerId = Animator.StringToHash("ButtonPressed");
	}

	// Token: 0x06002551 RID: 9553 RVA: 0x0008F5B4 File Offset: 0x0008D7B4
	public void Activate()
	{
		if (this.nextAllowedActivationTime <= Time.time)
		{
			this.feeder.StepFeederSpeed();
			if (this.buttonAnimator != null)
			{
				this.buttonAnimator.SetTrigger(this.buttonPressedTriggerId);
			}
			SECTR_AudioCue sectr_AudioCue = this.pressButtonCueNormal;
			switch (this.feeder.GetFeedingCycleSpeed())
			{
			case SlimeFeeder.FeedSpeed.Normal:
				sectr_AudioCue = this.pressButtonCueNormal;
				break;
			case SlimeFeeder.FeedSpeed.Slow:
				sectr_AudioCue = this.pressButtonCueSlow;
				break;
			case SlimeFeeder.FeedSpeed.Fast:
				sectr_AudioCue = this.pressButtonCueFast;
				break;
			default:
				sectr_AudioCue = this.pressButtonCueNormal;
				Log.Error("Invalid feeder speed.", Array.Empty<object>());
				break;
			}
			if (sectr_AudioCue != null)
			{
				SECTR_AudioSystem.Play(sectr_AudioCue, base.transform.position, false);
			}
			this.nextAllowedActivationTime = Time.time + 0.4f;
		}
	}

	// Token: 0x06002552 RID: 9554 RVA: 0x00025E60 File Offset: 0x00024060
	public GameObject GetCustomGuiPrefab()
	{
		return null;
	}

	// Token: 0x04002439 RID: 9273
	public SlimeFeeder feeder;

	// Token: 0x0400243A RID: 9274
	public SECTR_AudioCue pressButtonCueSlow;

	// Token: 0x0400243B RID: 9275
	public SECTR_AudioCue pressButtonCueNormal;

	// Token: 0x0400243C RID: 9276
	public SECTR_AudioCue pressButtonCueFast;

	// Token: 0x0400243D RID: 9277
	private Animator buttonAnimator;

	// Token: 0x0400243E RID: 9278
	private int buttonPressedTriggerId;

	// Token: 0x0400243F RID: 9279
	private const float TIME_BETWEEN_ACTIVATIONS = 0.4f;

	// Token: 0x04002440 RID: 9280
	private float nextAllowedActivationTime;
}
