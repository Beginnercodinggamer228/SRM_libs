using System;
using UnityEngine;

// Token: 0x0200043E RID: 1086
public class SlimeAnimationEventHandler : MonoBehaviour
{
	// Token: 0x06001684 RID: 5764 RVA: 0x00057B67 File Offset: 0x00055D67
	public void Awake()
	{
		this.sfAnimator = base.GetComponent<SlimeFaceAnimator>();
	}

	// Token: 0x06001685 RID: 5765 RVA: 0x00057B75 File Offset: 0x00055D75
	public void TriggerAweFace()
	{
		this.TriggerFace("triggerAwe");
	}

	// Token: 0x06001686 RID: 5766 RVA: 0x00057B82 File Offset: 0x00055D82
	public void TriggerAlarmFace()
	{
		this.TriggerFace("triggerAlarm");
	}

	// Token: 0x06001687 RID: 5767 RVA: 0x00057B8F File Offset: 0x00055D8F
	public void TriggerWinceFace()
	{
		this.TriggerFace("triggerWince");
	}

	// Token: 0x06001688 RID: 5768 RVA: 0x00057B9C File Offset: 0x00055D9C
	public void TriggerMinorWinceFace()
	{
		this.TriggerFace("triggerMinorWince");
	}

	// Token: 0x06001689 RID: 5769 RVA: 0x00057BA9 File Offset: 0x00055DA9
	public void TriggerAttackTelegraphFace()
	{
		this.TriggerFace("triggerAttackTelegraph");
	}

	// Token: 0x0600168A RID: 5770 RVA: 0x00057BB6 File Offset: 0x00055DB6
	public void TriggerChompOpenFace()
	{
		this.TriggerFace("triggerChompOpen");
	}

	// Token: 0x0600168B RID: 5771 RVA: 0x00057BC3 File Offset: 0x00055DC3
	public void TriggerChompOpenQuickFace()
	{
		this.TriggerFace("triggerChompOpenQuick");
	}

	// Token: 0x0600168C RID: 5772 RVA: 0x00057BD0 File Offset: 0x00055DD0
	public void TriggerChompClosedFace()
	{
		this.TriggerFace("triggerChompClosed");
	}

	// Token: 0x0600168D RID: 5773 RVA: 0x00057BDD File Offset: 0x00055DDD
	public void TriggerInvokeFace()
	{
		this.TriggerFace("triggerConcentrate");
	}

	// Token: 0x0600168E RID: 5774 RVA: 0x00057BEA File Offset: 0x00055DEA
	public void TriggerGrimaceFace()
	{
		this.TriggerFace("triggerGrimace");
	}

	// Token: 0x0600168F RID: 5775 RVA: 0x00057BF7 File Offset: 0x00055DF7
	public void TriggerFriedFace()
	{
		this.TriggerFace("triggerFried");
	}

	// Token: 0x06001690 RID: 5776 RVA: 0x00057C04 File Offset: 0x00055E04
	private void TriggerFace(string faceTrigger)
	{
		this.sfAnimator.SetTrigger(faceTrigger);
	}

	// Token: 0x040015B7 RID: 5559
	private SlimeFaceAnimator sfAnimator;
}
