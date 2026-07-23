using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200012C RID: 300
public class DroneAnimatorState : SRAnimatorState<DroneAnimator>
{
	// Token: 0x0600068C RID: 1676 RVA: 0x000230D4 File Offset: 0x000212D4
	public override void OnStateEnter(Animator animator, AnimatorStateInfo state, int layerIndex)
	{
		base.OnStateEnter(animator, state, layerIndex);
		Drone drone = this.GetDrone(animator);
		Destroyer.Destroy(this.audio, "DroneAnimatorState.OnStateEnter");
		this.audio = drone.SFX(DroneAnimatorState.GetAudioCue(this.id, drone.metadata));
	}

	// Token: 0x0600068D RID: 1677 RVA: 0x0002311F File Offset: 0x0002131F
	public override void OnStateExit(Animator animator, AnimatorStateInfo state, int layerIndex)
	{
		base.OnStateExit(animator, state, layerIndex);
		Destroyer.Destroy(this.audio, "DroneAnimatorState.OnStateExit");
		base.GetAnimatorWrapper(animator).OnStateExit(this.id);
	}

	// Token: 0x0600068E RID: 1678 RVA: 0x0002314C File Offset: 0x0002134C
	public void OnDestroy()
	{
		Destroyer.Destroy(this.audio, "DroneAnimatorState.OnStateExit");
	}

	// Token: 0x0600068F RID: 1679 RVA: 0x0002315E File Offset: 0x0002135E
	private Drone GetDrone(Animator animator)
	{
		if (this.drone == null)
		{
			this.drone = animator.gameObject.GetComponentInParent<Drone>();
		}
		return this.drone;
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x00023188 File Offset: 0x00021388
	private static SECTR_AudioCue GetAudioCue(DroneAnimatorState.Id id, DroneMetadata metadata)
	{
		if (id <= DroneAnimatorState.Id.DEPOSIT_END)
		{
			switch (id)
			{
			case DroneAnimatorState.Id.GATHER_BEGIN:
				return metadata.onGatherBeginCue;
			case DroneAnimatorState.Id.GATHER_LOOP:
				return metadata.onGatherLoopCue;
			case DroneAnimatorState.Id.GATHER_END:
				return metadata.onGatherEndCue;
			default:
				switch (id)
				{
				case DroneAnimatorState.Id.DEPOSIT_BEGIN:
					return metadata.onDepositBeginCue;
				case DroneAnimatorState.Id.DEPOSIT_LOOP:
					return metadata.onDepositLoopCue;
				case DroneAnimatorState.Id.DEPOSIT_END:
					return metadata.onDepositEndCue;
				}
				break;
			}
		}
		else
		{
			switch (id)
			{
			case DroneAnimatorState.Id.REST_BEGIN:
				return metadata.onRestBeginCue;
			case DroneAnimatorState.Id.REST_LOOP:
				return metadata.onRestLoopCue;
			case DroneAnimatorState.Id.REST_END:
				return metadata.onRestEndCue;
			default:
				if (id == DroneAnimatorState.Id.IDLE_CELEBRATE)
				{
					return metadata.onHappyCue;
				}
				if (id == DroneAnimatorState.Id.IDLE_GRUMP)
				{
					return metadata.onGrumpyCue;
				}
				break;
			}
		}
		return null;
	}

	// Token: 0x0400061B RID: 1563
	[Tooltip("Looping state identifier.")]
	public DroneAnimatorState.Id id;

	// Token: 0x0400061C RID: 1564
	private Drone drone;

	// Token: 0x0400061D RID: 1565
	private DroneAudioOnActive audio;

	// Token: 0x0200012D RID: 301
	public enum Id
	{
		// Token: 0x0400061F RID: 1567
		NONE,
		// Token: 0x04000620 RID: 1568
		GATHER_BEGIN = 10,
		// Token: 0x04000621 RID: 1569
		GATHER_LOOP,
		// Token: 0x04000622 RID: 1570
		GATHER_END,
		// Token: 0x04000623 RID: 1571
		DEPOSIT_BEGIN = 20,
		// Token: 0x04000624 RID: 1572
		DEPOSIT_LOOP,
		// Token: 0x04000625 RID: 1573
		DEPOSIT_END,
		// Token: 0x04000626 RID: 1574
		REST_BEGIN = 30,
		// Token: 0x04000627 RID: 1575
		REST_LOOP,
		// Token: 0x04000628 RID: 1576
		REST_END,
		// Token: 0x04000629 RID: 1577
		IDLE_CELEBRATE = 100,
		// Token: 0x0400062A RID: 1578
		IDLE_GRUMP = 200
	}

	// Token: 0x0200012E RID: 302
	public class IdComparer : IEqualityComparer<DroneAnimatorState.Id>
	{
		// Token: 0x06000692 RID: 1682 RVA: 0x00017781 File Offset: 0x00015981
		public bool Equals(DroneAnimatorState.Id a, DroneAnimatorState.Id b)
		{
			return a == b;
		}

		// Token: 0x06000693 RID: 1683 RVA: 0x00017787 File Offset: 0x00015987
		public int GetHashCode(DroneAnimatorState.Id a)
		{
			return (int)a;
		}

		// Token: 0x0400062B RID: 1579
		public static DroneAnimatorState.IdComparer Instance = new DroneAnimatorState.IdComparer();
	}
}
