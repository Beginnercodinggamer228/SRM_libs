using System;
using UnityEngine;

// Token: 0x020003E7 RID: 999
public class GoldSlimeFlee : SlimeFlee
{
	// Token: 0x060014CE RID: 5326 RVA: 0x00050FBC File Offset: 0x0004F1BC
	public override void Awake()
	{
		base.Awake();
		this.eat = base.GetComponent<SlimeEat>();
		this.eat.onStartChomp = new Chomper.OnChompStartDelegate(this.OnStartChomp);
		this.eat.onProducePlortsComplete = new SlimeEat.OnProducePlortsCompleteDelegate(this.OnPlortsProduced);
	}

	// Token: 0x060014CF RID: 5327 RVA: 0x00051009 File Offset: 0x0004F209
	public override void OnDestroy()
	{
		this.eat.onStartChomp = null;
		this.eat.onProducePlortsComplete = null;
		this.eat = null;
		base.OnDestroy();
	}

	// Token: 0x060014D0 RID: 5328 RVA: 0x00051030 File Offset: 0x0004F230
	public void OnTriggerEnter(Collider collider)
	{
		if (!base.IsFleeing() && !collider.isTrigger && PhysicsUtil.IsPlayerMainCollider(collider))
		{
			base.StartFleeing(collider.gameObject);
		}
	}

	// Token: 0x060014D1 RID: 5329 RVA: 0x00051056 File Offset: 0x0004F256
	private void OnStartChomp()
	{
		this.currentlyChomping = true;
		this.chompingUntil = this.timeDir.HoursFromNow(0.016666668f);
		this.slimeBody.velocity = Vector3.zero;
		this.slimeBody.angularVelocity = Vector3.zero;
	}

	// Token: 0x060014D2 RID: 5330 RVA: 0x00051095 File Offset: 0x0004F295
	private void OnPlortsProduced()
	{
		this.currentlyChomping = false;
	}

	// Token: 0x060014D3 RID: 5331 RVA: 0x0005109E File Offset: 0x0004F29E
	public override void Action()
	{
		if (!this.currentlyChomping || this.timeDir.HasReached(this.chompingUntil))
		{
			base.Action();
		}
	}

	// Token: 0x040013AE RID: 5038
	private bool currentlyChomping;

	// Token: 0x040013AF RID: 5039
	private double chompingUntil;

	// Token: 0x040013B0 RID: 5040
	private SlimeEat eat;

	// Token: 0x040013B1 RID: 5041
	private const float CHOMPING_MAX_DELAY = 0.016666668f;
}
