using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000472 RID: 1138
public class SlimeEatTrigger : RegisteredActorBehaviour, RegistryUpdateable
{
	// Token: 0x06001786 RID: 6022 RVA: 0x0005B570 File Offset: 0x00059770
	public void Awake()
	{
		this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.eat = base.GetComponentInParent<SlimeEat>();
		SlimeEat slimeEat = this.eat;
		slimeEat.onFinishChompSuccess = (SlimeEat.OnFinishChompSuccessDelegate)Delegate.Combine(slimeEat.onFinishChompSuccess, new SlimeEat.OnFinishChompSuccessDelegate(this.OnFinishChompSuccess));
		this.attack = base.GetComponentInParent<AttackPlayer>();
		if (this.attack != null)
		{
			AttackPlayer attackPlayer = this.attack;
			attackPlayer.onFinishChompSuccess = (AttackPlayer.OnFinishChompSuccessDelegate)Delegate.Combine(attackPlayer.onFinishChompSuccess, new AttackPlayer.OnFinishChompSuccessDelegate(this.OnFinishChompSuccess));
		}
	}

	// Token: 0x06001787 RID: 6023 RVA: 0x0005B604 File Offset: 0x00059804
	public override void OnDestroy()
	{
		base.OnDestroy();
		SlimeEat slimeEat = this.eat;
		slimeEat.onFinishChompSuccess = (SlimeEat.OnFinishChompSuccessDelegate)Delegate.Remove(slimeEat.onFinishChompSuccess, new SlimeEat.OnFinishChompSuccessDelegate(this.OnFinishChompSuccess));
		if (this.attack != null)
		{
			AttackPlayer attackPlayer = this.attack;
			attackPlayer.onFinishChompSuccess = (AttackPlayer.OnFinishChompSuccessDelegate)Delegate.Remove(attackPlayer.onFinishChompSuccess, new AttackPlayer.OnFinishChompSuccessDelegate(this.OnFinishChompSuccess));
		}
	}

	// Token: 0x06001788 RID: 6024 RVA: 0x0005B674 File Offset: 0x00059874
	public void RegistryUpdate()
	{
		for (int i = this.targets.Count - 1; i >= 0; i--)
		{
			SlimeEatTrigger.EatTarget eatTarget = this.targets[i];
			if (eatTarget.gameObject == null)
			{
				this.targets.RemoveAt(i);
			}
			else if (this.timeDirector.HasReached(eatTarget.time))
			{
				if (!eatTarget.isColliding)
				{
					this.targets.RemoveAt(i);
				}
				else if (this.eat.MaybeChomp(eatTarget.gameObject) || (this.attack != null && this.attack.MaybeChomp(eatTarget.gameObject)))
				{
					break;
				}
			}
		}
	}

	// Token: 0x06001789 RID: 6025 RVA: 0x0005B725 File Offset: 0x00059925
	public void OnTriggerEnter(Collider collider)
	{
		this.SetColliding(collider, true);
	}

	// Token: 0x0600178A RID: 6026 RVA: 0x0005B72F File Offset: 0x0005992F
	public void OnTriggerExit(Collider collider)
	{
		this.SetColliding(collider, false);
		this.eat.CancelChomp(collider.gameObject);
		if (this.attack != null)
		{
			this.attack.CancelChomp(collider.gameObject);
		}
	}

	// Token: 0x0600178B RID: 6027 RVA: 0x0005B76C File Offset: 0x0005996C
	private void SetColliding(Collider collider, bool colliding)
	{
		if (!this.eat.DoesEat(collider.gameObject) && (this.attack == null || !this.attack.DoesAttack(collider.gameObject)))
		{
			return;
		}
		SlimeEatTrigger.EatTarget eatTarget = this.FindTarget(collider.gameObject);
		if (eatTarget != null)
		{
			eatTarget.isColliding = colliding;
			return;
		}
		if (colliding)
		{
			this.targets.Insert(0, new SlimeEatTrigger.EatTarget
			{
				gameObject = collider.gameObject,
				time = this.timeDirector.WorldTime(),
				isColliding = true
			});
		}
	}

	// Token: 0x0600178C RID: 6028 RVA: 0x0005B800 File Offset: 0x00059A00
	private SlimeEatTrigger.EatTarget FindTarget(GameObject gameObject)
	{
		return this.targets.FirstOrDefault((SlimeEatTrigger.EatTarget t) => t.gameObject == gameObject);
	}

	// Token: 0x0600178D RID: 6029 RVA: 0x0005B834 File Offset: 0x00059A34
	private void OnFinishChompSuccess(GameObject gameObject)
	{
		SlimeEatTrigger.EatTarget eatTarget = this.FindTarget(gameObject);
		if (eatTarget != null)
		{
			eatTarget.time = this.timeDirector.HoursFromNow(0.050000004f);
		}
	}

	// Token: 0x040016B5 RID: 5813
	private const float NEXT_CHOMP_COOLDOWN = 0.050000004f;

	// Token: 0x040016B6 RID: 5814
	private TimeDirector timeDirector;

	// Token: 0x040016B7 RID: 5815
	private SlimeEat eat;

	// Token: 0x040016B8 RID: 5816
	private AttackPlayer attack;

	// Token: 0x040016B9 RID: 5817
	private List<SlimeEatTrigger.EatTarget> targets = new List<SlimeEatTrigger.EatTarget>();

	// Token: 0x02000473 RID: 1139
	private class EatTarget
	{
		// Token: 0x040016BA RID: 5818
		public GameObject gameObject;

		// Token: 0x040016BB RID: 5819
		public bool isColliding;

		// Token: 0x040016BC RID: 5820
		public double time;
	}
}
