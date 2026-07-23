using System;
using UnityEngine;

// Token: 0x020001B7 RID: 439
public class DroneSubbehaviourDumpAmmo : DroneSubbehaviour
{
	// Token: 0x0600094D RID: 2381 RVA: 0x00029A37 File Offset: 0x00027C37
	public override bool Relevancy()
	{
		return !this.drone.ammo.IsEmpty();
	}

	// Token: 0x0600094E RID: 2382 RVA: 0x00029A50 File Offset: 0x00027C50
	public override void Selected()
	{
		base.Selected();
		this.drone.movement.rigidbody.isKinematic = true;
		this.drone.movement.rigidbody.velocity = Vector3.zero;
		this.drone.movement.rigidbody.angularVelocity = Vector3.zero;
		this.state = DroneSubbehaviourDumpAmmo.State.ANIMATE;
		this.drone.animator.SetAnimation(DroneAnimator.Id.IDLE_GRUMP);
		this.drone.animator.OnStateExit(DroneAnimatorState.Id.IDLE_GRUMP, delegate()
		{
			this.drone.animator.SetAnimation(DroneAnimator.Id.IDLE);
			this.state = DroneSubbehaviourDumpAmmo.State.ELEVATE;
		});
	}

	// Token: 0x0600094F RID: 2383 RVA: 0x00029AE6 File Offset: 0x00027CE6
	public override void Deselected()
	{
		base.Deselected();
		this.drone.animator.SetAnimation(DroneAnimator.Id.IDLE);
		this.drone.movement.rigidbody.isKinematic = false;
	}

	// Token: 0x06000950 RID: 2384 RVA: 0x00029B18 File Offset: 0x00027D18
	public override void Action()
	{
		if (this.state == DroneSubbehaviourDumpAmmo.State.ELEVATE)
		{
			if (Physics.Raycast(this.drone.transform.position, Vector3.down, 3f))
			{
				Vector3 position = this.drone.transform.position + Vector3.up;
				this.drone.movement.MoveTowards(position);
			}
			else
			{
				this.state = DroneSubbehaviourDumpAmmo.State.DUMP;
				this.time = 0.0;
			}
		}
		if (this.state == DroneSubbehaviourDumpAmmo.State.DUMP && base.OnAction_DumpAmmo(ref this.time) && this.drone.ammo.IsEmpty())
		{
			base.plexer.ForceRethink(0f);
			if (this.destructive)
			{
				if (this.drone.metadata.onTeleportFX != null)
				{
					SRBehaviour.SpawnAndPlayFX(this.drone.metadata.onTeleportFX, this.drone.transform.position, this.drone.transform.rotation);
				}
				Destroyer.Destroy(this.drone.gameObject, "DroneSubbehaviourDumpAmmo.Destructive.Action");
			}
		}
	}

	// Token: 0x040007D1 RID: 2001
	[HideInInspector]
	public bool destructive;

	// Token: 0x040007D2 RID: 2002
	private DroneSubbehaviourDumpAmmo.State state;

	// Token: 0x040007D3 RID: 2003
	private double time;

	// Token: 0x020001B8 RID: 440
	private enum State
	{
		// Token: 0x040007D5 RID: 2005
		ANIMATE,
		// Token: 0x040007D6 RID: 2006
		ELEVATE,
		// Token: 0x040007D7 RID: 2007
		DUMP
	}
}
