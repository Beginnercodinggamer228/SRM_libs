using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200015E RID: 350
public abstract class DroneProgram : DroneSubbehaviour
{
	// Token: 0x06000781 RID: 1921 RVA: 0x00025854 File Offset: 0x00023A54
	public override void Selected()
	{
		base.Selected();
		this.drone.animator.SetAnimation(DroneAnimator.Id.MOVE);
		this.noClipPreviousPosition = this.drone.transform.position;
		this.noClipTime = 10f;
		this.state = DroneProgram.State.PATHING;
	}

	// Token: 0x06000782 RID: 1922 RVA: 0x000258A0 File Offset: 0x00023AA0
	public override void Deselected()
	{
		base.Deselected();
		this.drone.animator.SetAnimation(DroneAnimator.Id.IDLE);
		this.drone.movement.rigidbody.isKinematic = false;
		this.drone.upright.enabled = true;
		this.existingPath = null;
	}

	// Token: 0x06000783 RID: 1923 RVA: 0x000258F4 File Offset: 0x00023AF4
	public void OnDrawGizmos()
	{
		if (this.existingPath != null)
		{
			Gizmos.color = Color.blue;
			Vector3 from = this.drone.transform.position;
			foreach (Vector3 vector in this.existingPath)
			{
				Gizmos.DrawLine(from, vector);
				from = vector;
			}
		}
	}

	// Token: 0x06000784 RID: 1924 RVA: 0x0002596C File Offset: 0x00023B6C
	public sealed override void Action()
	{
		if (this.state == DroneProgram.State.COMPLETE || this.CanCancel())
		{
			base.plexer.ForceRethink(0f);
			return;
		}
		if ((this.state == DroneProgram.State.PATHING || this.state == DroneProgram.State.PATHING_ARRIVAL) && !this.drone.noClip.enabled)
		{
			this.noClipTime -= Time.fixedDeltaTime;
			if (this.noClipTime <= 0f)
			{
				float sqrMagnitude = (this.noClipPreviousPosition - this.drone.transform.position).sqrMagnitude;
				this.noClipPreviousPosition = this.drone.transform.position;
				this.noClipTime = ((this.state == DroneProgram.State.PATHING_ARRIVAL) ? 2f : 10f);
				this.drone.noClip.enabled = (sqrMagnitude <= 0.1f);
			}
		}
		if (this.state == DroneProgram.State.PATHING)
		{
			if (this.existingPath == null)
			{
				if (!this.timeDirector.HasReached(this.drone.network.pathingThrottleUntil))
				{
					return;
				}
				this.drone.network.pathingThrottleUntil = this.timeDirector.HoursFromNow(0.033333335f);
				this.GeneratePath(this.GetSubnetwork(), this.GetTargetOrientations(), this.GetTargetPosition());
			}
			if (this.existingPath == null)
			{
				base.plexer.ForceRethink(0f);
				return;
			}
			Vector3? vector = (this.existingPath.Count > 0) ? new Vector3?(this.existingPath.Peek()) : null;
			if (vector == null || (vector.Value - this.drone.transform.position).sqrMagnitude <= 1f)
			{
				if (this.existingPath.Count <= 1)
				{
					if ((this.GetTargetPosition() - this.previousTargetPosition).sqrMagnitude >= 1f)
					{
						this.existingPath = null;
						return;
					}
					this.state = DroneProgram.State.PATHING_ARRIVAL;
					this.noClipTime = Mathf.Min(this.noClipTime, 2f);
					this.drone.movement.rigidbody.velocity = Vector3.zero;
					this.drone.movement.rigidbody.angularVelocity = Vector3.zero;
					this.drone.upright.enabled = false;
					this.existingPath = null;
				}
				else
				{
					this.drone.noClip.enabled = false;
					this.existingPath.Dequeue();
				}
			}
			else
			{
				this.drone.movement.PathTowards(vector.Value);
			}
		}
		if (this.state == DroneProgram.State.PATHING_ARRIVAL && this.drone.movement.MoveTowards(this.arrivalOrient.pos) && this.drone.movement.RotateTowards(this.arrivalOrient.rot))
		{
			this.drone.movement.rigidbody.isKinematic = true;
			this.OnReachedDestination();
			this.state = DroneProgram.State.PATHING_ARRIVED;
			return;
		}
		if (this.state == DroneProgram.State.PATHING_ARRIVED)
		{
			Action action = delegate()
			{
				this.state = DroneProgram.State.ACTION_LOOP_FIRST;
			};
			if (this.animationStateBegin != DroneAnimatorState.Id.NONE)
			{
				this.state = DroneProgram.State.ACTION_PRE;
				this.drone.animator.SetAnimation(this.animation);
				this.drone.animator.OnStateExit(this.animationStateBegin, action);
			}
			else
			{
				this.drone.animator.SetAnimation(this.animation);
				action();
			}
		}
		if (this.state == DroneProgram.State.ACTION_LOOP_FIRST)
		{
			this.state = DroneProgram.State.ACTION_LOOP;
			this.OnFirstAction();
		}
		if (this.state == DroneProgram.State.ACTION_LOOP && this.OnAction())
		{
			Action action2 = delegate()
			{
				this.state = DroneProgram.State.COMPLETE;
			};
			if (this.animationStateEnd != DroneAnimatorState.Id.NONE)
			{
				this.state = DroneProgram.State.ACTION_POST;
				this.drone.animator.SetAnimation(DroneAnimator.Id.IDLE);
				this.drone.animator.OnStateExit(this.animationStateEnd, action2);
				return;
			}
			this.drone.animator.SetAnimation(DroneAnimator.Id.IDLE);
			action2();
		}
	}

	// Token: 0x06000785 RID: 1925 RVA: 0x00025D74 File Offset: 0x00023F74
	protected bool GeneratePath(GardenDroneSubnetwork subnetwork, IEnumerable<DroneProgram.Orientation> orientations, Vector3 position)
	{
		Vector3 position2 = this.drone.transform.position;
		List<PathingNetwork> list = new List<PathingNetwork>();
		if (subnetwork != null)
		{
			list.Add(subnetwork);
		}
		list.Add(this.drone.network);
		foreach (DroneProgram.Orientation orientation in orientations)
		{
			foreach (PathingNetwork pathingNetwork in list)
			{
				if ((this.existingPath = pathingNetwork.GeneratePath(position2, orientation.pos)) != null)
				{
					this.arrivalOrient = orientation;
					this.previousTargetPosition = position;
					return true;
				}
			}
		}
		this.OnPathGenerationFailed();
		return false;
	}

	// Token: 0x06000786 RID: 1926 RVA: 0x00025E60 File Offset: 0x00024060
	protected virtual GardenDroneSubnetwork GetSubnetwork()
	{
		return null;
	}

	// Token: 0x17000103 RID: 259
	// (get) Token: 0x06000787 RID: 1927
	protected abstract DroneAnimator.Id animation { get; }

	// Token: 0x17000104 RID: 260
	// (get) Token: 0x06000788 RID: 1928
	protected abstract DroneAnimatorState.Id animationStateBegin { get; }

	// Token: 0x17000105 RID: 261
	// (get) Token: 0x06000789 RID: 1929
	protected abstract DroneAnimatorState.Id animationStateEnd { get; }

	// Token: 0x0600078A RID: 1930
	protected abstract IEnumerable<DroneProgram.Orientation> GetTargetOrientations();

	// Token: 0x0600078B RID: 1931
	protected abstract Vector3 GetTargetPosition();

	// Token: 0x0600078C RID: 1932 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	protected virtual bool CanCancel()
	{
		return false;
	}

	// Token: 0x0600078D RID: 1933 RVA: 0x00003296 File Offset: 0x00001496
	protected virtual void OnReachedDestination()
	{
	}

	// Token: 0x0600078E RID: 1934 RVA: 0x00003296 File Offset: 0x00001496
	protected virtual void OnFirstAction()
	{
	}

	// Token: 0x0600078F RID: 1935 RVA: 0x00013CC5 File Offset: 0x00011EC5
	protected virtual bool OnAction()
	{
		return true;
	}

	// Token: 0x06000790 RID: 1936 RVA: 0x00003296 File Offset: 0x00001496
	protected virtual void OnPathGenerationFailed()
	{
	}

	// Token: 0x040006E1 RID: 1761
	private const float ARRIVE_RAD = 1f;

	// Token: 0x040006E2 RID: 1762
	private const float NO_CLIP_PERIOD = 10f;

	// Token: 0x040006E3 RID: 1763
	private const float ARRIVAL_NO_CLIP_PERIOD = 2f;

	// Token: 0x040006E4 RID: 1764
	private DroneProgram.State state;

	// Token: 0x040006E5 RID: 1765
	private Queue<Vector3> existingPath;

	// Token: 0x040006E6 RID: 1766
	private DroneProgram.Orientation arrivalOrient;

	// Token: 0x040006E7 RID: 1767
	private Vector3 previousTargetPosition;

	// Token: 0x040006E8 RID: 1768
	private Vector3 noClipPreviousPosition;

	// Token: 0x040006E9 RID: 1769
	private float noClipTime;

	// Token: 0x0200015F RID: 351
	public class Orientation
	{
		// Token: 0x06000794 RID: 1940 RVA: 0x000053FC File Offset: 0x000035FC
		public Orientation()
		{
		}

		// Token: 0x06000795 RID: 1941 RVA: 0x00025E7D File Offset: 0x0002407D
		public Orientation(Vector3 pos, Quaternion rot)
		{
			this.pos = pos;
			this.rot = rot;
		}

		// Token: 0x040006EA RID: 1770
		public Vector3 pos;

		// Token: 0x040006EB RID: 1771
		public Quaternion rot;
	}

	// Token: 0x02000160 RID: 352
	private enum State
	{
		// Token: 0x040006ED RID: 1773
		PATHING,
		// Token: 0x040006EE RID: 1774
		PATHING_ARRIVAL,
		// Token: 0x040006EF RID: 1775
		PATHING_ARRIVED,
		// Token: 0x040006F0 RID: 1776
		ACTION_PRE,
		// Token: 0x040006F1 RID: 1777
		ACTION_LOOP_FIRST,
		// Token: 0x040006F2 RID: 1778
		ACTION_LOOP,
		// Token: 0x040006F3 RID: 1779
		ACTION_POST,
		// Token: 0x040006F4 RID: 1780
		COMPLETE
	}
}
