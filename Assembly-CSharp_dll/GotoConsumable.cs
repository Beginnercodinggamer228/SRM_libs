using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003F8 RID: 1016
public class GotoConsumable : FindConsumable
{
	// Token: 0x0600153C RID: 5436 RVA: 0x00052715 File Offset: 0x00050915
	public override void Awake()
	{
		base.Awake();
		this.eat = base.GetComponent<SlimeEat>();
		this.fastForwardLayerMask = LayerMask.GetMask(new string[]
		{
			"Actor",
			"ActorEchoes",
			"ActorIgnorePlayer"
		});
	}

	// Token: 0x0600153D RID: 5437 RVA: 0x00052754 File Offset: 0x00050954
	public override float Relevancy(bool isGrounded)
	{
		if (Time.time >= this.modeEndTime)
		{
			if (this.mode == GotoConsumable.Mode.ATTEMPTING)
			{
				this.mode = GotoConsumable.Mode.GIVE_UP;
				this.modeEndTime = Time.time + this.giveUpTime;
				this.emotions.Adjust(SlimeEmotions.Emotion.AGITATION, 0.1f);
			}
			else if (this.mode == GotoConsumable.Mode.GIVE_UP)
			{
				this.mode = GotoConsumable.Mode.AVAIL;
				this.modeEndTime = float.PositiveInfinity;
			}
		}
		if (this.mode == GotoConsumable.Mode.GIVE_UP)
		{
			return 0f;
		}
		this.target = base.FindNearestConsumable(out this.currDrive);
		if (!(this.target == null))
		{
			return this.currDrive * this.currDrive * 0.95f;
		}
		return 0f;
	}

	// Token: 0x0600153E RID: 5438 RVA: 0x00052807 File Offset: 0x00050A07
	public override void Selected()
	{
		this.mode = GotoConsumable.Mode.ATTEMPTING;
		this.modeEndTime = Time.time + this.attemptTime;
		base.GetComponent<SlimeFaceAnimator>().SetSeekingFood(true);
	}

	// Token: 0x0600153F RID: 5439 RVA: 0x0005282E File Offset: 0x00050A2E
	public override void Deselected()
	{
		base.Deselected();
		base.GetComponent<SlimeFaceAnimator>().SetSeekingFood(false);
	}

	// Token: 0x06001540 RID: 5440 RVA: 0x00052844 File Offset: 0x00050A44
	public override void Action()
	{
		if (this.target != null)
		{
			if (SRSingleton<SceneContext>.Instance.TimeDirector.IsFastForwarding() && CellDirector.IsOnRanch(this.member))
			{
				if (!base.IsBlocked(this.target, this.fastForwardLayerMask, true))
				{
					Identifiable.Id id = Identifiable.GetId(this.target);
					if (id != Identifiable.Id.PLAYER)
					{
						this.eat.EatImmediate(this.target, id, this.eat.GetProducedIds(id, GotoConsumable.reuseIdList), GotoConsumable.alreadyCollectedList, false);
						return;
					}
				}
			}
			else if (!this.eat.IsChomping())
			{
				base.MoveTowards(SlimeSubbehaviour.GetGotoPos(this.target), base.IsBlocked(this.target, 0, false), ref this.nextLeapAvail, this.DriveToJumpiness(this.currDrive) * this.maxJump);
			}
		}
	}

	// Token: 0x06001541 RID: 5441 RVA: 0x00052918 File Offset: 0x00050B18
	private float DriveToJumpiness(float drive)
	{
		float num = Mathf.Max(0f, drive - 0.666f) / 0.334f;
		return Mathf.Lerp(0.4f, 1f, num * num);
	}

	// Token: 0x04001411 RID: 5137
	public float maxJump = 12f;

	// Token: 0x04001412 RID: 5138
	public float attemptTime = 10f;

	// Token: 0x04001413 RID: 5139
	public float giveUpTime = 10f;

	// Token: 0x04001414 RID: 5140
	private GameObject target;

	// Token: 0x04001415 RID: 5141
	private float currDrive;

	// Token: 0x04001416 RID: 5142
	private float nextLeapAvail;

	// Token: 0x04001417 RID: 5143
	private SlimeEat eat;

	// Token: 0x04001418 RID: 5144
	private const float AGITATION_PER_GIVE_UP = 0.1f;

	// Token: 0x04001419 RID: 5145
	private int fastForwardLayerMask;

	// Token: 0x0400141A RID: 5146
	private GotoConsumable.Mode mode;

	// Token: 0x0400141B RID: 5147
	private float modeEndTime;

	// Token: 0x0400141C RID: 5148
	private static List<Identifiable.Id> reuseIdList = new List<Identifiable.Id>();

	// Token: 0x0400141D RID: 5149
	private static readonly List<Identifiable.Id> alreadyCollectedList = new List<Identifiable.Id>();

	// Token: 0x0400141E RID: 5150
	private const float MAX_VEL_TO_BOUNCE = 0.1f;

	// Token: 0x0400141F RID: 5151
	private const float SQR_MAX_VEL_TO_BOUNCE = 0.010000001f;

	// Token: 0x020003F9 RID: 1017
	private enum Mode
	{
		// Token: 0x04001421 RID: 5153
		AVAIL,
		// Token: 0x04001422 RID: 5154
		ATTEMPTING,
		// Token: 0x04001423 RID: 5155
		GIVE_UP
	}
}
