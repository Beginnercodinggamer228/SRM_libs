using System;
using UnityEngine;

// Token: 0x020001B9 RID: 441
public class DroneSubbehaviourIdle : DroneSubbehaviour
{
	// Token: 0x06000953 RID: 2387 RVA: 0x00029C58 File Offset: 0x00027E58
	public override bool Relevancy()
	{
		return this.drone.station.battery.HasAny() && this.timeDirector.HasReached(this.cooldown) && Randoms.SHARED.GetProbability(0.1f);
	}

	// Token: 0x06000954 RID: 2388 RVA: 0x00029CA8 File Offset: 0x00027EA8
	public override void Selected()
	{
		base.Selected();
		this.drone.movement.rigidbody.isKinematic = true;
		this.rotation = new Quaternion?(Quaternion.LookRotation(SRSingleton<SceneContext>.Instance.Player.transform.position - this.drone.transform.position));
		this.cooldown = this.timeDirector.HoursFromNow(3f);
	}

	// Token: 0x06000955 RID: 2389 RVA: 0x00029AE6 File Offset: 0x00027CE6
	public override void Deselected()
	{
		base.Deselected();
		this.drone.animator.SetAnimation(DroneAnimator.Id.IDLE);
		this.drone.movement.rigidbody.isKinematic = false;
	}

	// Token: 0x06000956 RID: 2390 RVA: 0x00029D20 File Offset: 0x00027F20
	public override void Action()
	{
		if (this.rotation != null && this.drone.movement.RotateTowards(this.rotation.Value))
		{
			this.drone.animator.SetAnimation(DroneAnimator.Id.IDLE_CELEBRATE);
			this.drone.animator.OnStateExit(DroneAnimatorState.Id.IDLE_CELEBRATE, delegate()
			{
				base.plexer.ForceRethink(0f);
			});
			this.rotation = null;
		}
	}

	// Token: 0x040007D8 RID: 2008
	private double cooldown;

	// Token: 0x040007D9 RID: 2009
	private Quaternion? rotation;
}
