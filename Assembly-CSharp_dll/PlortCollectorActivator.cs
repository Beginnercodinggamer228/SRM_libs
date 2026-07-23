using System;
using UnityEngine;

// Token: 0x0200074A RID: 1866
public class PlortCollectorActivator : MonoBehaviour, TechActivator
{
	// Token: 0x060026FF RID: 9983 RVA: 0x000944C0 File Offset: 0x000926C0
	public void Awake()
	{
		this.buttonAnimator = base.GetComponentInParent<Animator>();
		this.buttonPressedTriggerId = Animator.StringToHash("ButtonPressed");
	}

	// Token: 0x06002700 RID: 9984 RVA: 0x000944E0 File Offset: 0x000926E0
	public void Activate()
	{
		if (this.nextAllowedActivationTime <= Time.time)
		{
			this.collector.StartCollection();
			if (this.buttonAnimator != null)
			{
				this.buttonAnimator.SetTrigger(this.buttonPressedTriggerId);
			}
			if (this.pressButtonCue != null)
			{
				SECTR_AudioSystem.Play(this.pressButtonCue, base.transform.position, false);
			}
			this.nextAllowedActivationTime = Time.time + 0.4f;
		}
	}

	// Token: 0x06002701 RID: 9985 RVA: 0x00025E60 File Offset: 0x00024060
	public GameObject GetCustomGuiPrefab()
	{
		return null;
	}

	// Token: 0x0400269E RID: 9886
	public PlortCollector collector;

	// Token: 0x0400269F RID: 9887
	public SECTR_AudioCue pressButtonCue;

	// Token: 0x040026A0 RID: 9888
	private Animator buttonAnimator;

	// Token: 0x040026A1 RID: 9889
	private int buttonPressedTriggerId;

	// Token: 0x040026A2 RID: 9890
	private const float TIME_BETWEEN_ACTIVATIONS = 0.4f;

	// Token: 0x040026A3 RID: 9891
	private float nextAllowedActivationTime;
}
