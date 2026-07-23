using System;
using UnityEngine;

// Token: 0x020002FC RID: 764
public class DecorizerStorageActivator : SRBehaviour, TechActivator
{
	// Token: 0x06001060 RID: 4192 RVA: 0x000416E8 File Offset: 0x0003F8E8
	public void Awake()
	{
		this.buttonAnimator = base.GetComponentInParent<Animator>();
		this.buttonAnimation = Animator.StringToHash("ButtonPressed");
		this.storage = base.GetComponentInParent<DecorizerStorage>();
	}

	// Token: 0x06001061 RID: 4193 RVA: 0x00041714 File Offset: 0x0003F914
	public void Activate()
	{
		if (this.nextActivationTime < Time.time)
		{
			this.nextActivationTime = Time.time + 0.4f;
			this.buttonAnimator.SetTrigger(this.buttonAnimation);
			UnityEngine.Object.Instantiate<GameObject>(SRSingleton<GameContext>.Instance.UITemplates.decorizerUIPrefab).GetComponent<DecorizerUI>().storage = this.storage;
		}
	}

	// Token: 0x06001062 RID: 4194 RVA: 0x00025E60 File Offset: 0x00024060
	public GameObject GetCustomGuiPrefab()
	{
		return null;
	}

	// Token: 0x04000F1F RID: 3871
	private Animator buttonAnimator;

	// Token: 0x04000F20 RID: 3872
	private int buttonAnimation;

	// Token: 0x04000F21 RID: 3873
	private const float TIME_BETWEEN_ACTIVATIONS = 0.4f;

	// Token: 0x04000F22 RID: 3874
	private float nextActivationTime;

	// Token: 0x04000F23 RID: 3875
	private DecorizerStorage storage;
}
