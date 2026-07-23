using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003FA RID: 1018
public class GotoPlayer : FindConsumable
{
	// Token: 0x06001544 RID: 5444 RVA: 0x0005298E File Offset: 0x00050B8E
	public override void Awake()
	{
		base.Awake();
		this.chomper = base.GetComponent<Chomper>();
	}

	// Token: 0x06001545 RID: 5445 RVA: 0x000529A2 File Offset: 0x00050BA2
	protected override Dictionary<Identifiable.Id, DriveCalculator> GetSearchIds()
	{
		Dictionary<Identifiable.Id, DriveCalculator> dictionary = new Dictionary<Identifiable.Id, DriveCalculator>();
		dictionary[Identifiable.Id.PLAYER] = new DriveCalculator(this.driver, this.extraDrive, this.minDrive);
		return dictionary;
	}

	// Token: 0x06001546 RID: 5446 RVA: 0x000529C8 File Offset: 0x00050BC8
	public override float Relevancy(bool isGrounded)
	{
		if (!this.shouldGotoPlayer)
		{
			return 0f;
		}
		if (Time.time >= this.modeEndTime)
		{
			if (this.mode == GotoPlayer.Mode.ATTEMPTING)
			{
				this.mode = GotoPlayer.Mode.GIVE_UP;
				this.modeEndTime = Time.time + this.giveUpTime;
				this.emotions.Adjust(SlimeEmotions.Emotion.AGITATION, 0.1f);
			}
			else if (this.mode == GotoPlayer.Mode.GIVE_UP)
			{
				this.mode = GotoPlayer.Mode.AVAIL;
				this.modeEndTime = float.PositiveInfinity;
			}
		}
		if (this.mode == GotoPlayer.Mode.GIVE_UP)
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

	// Token: 0x06001547 RID: 5447 RVA: 0x00052A89 File Offset: 0x00050C89
	public override void Selected()
	{
		this.mode = GotoPlayer.Mode.ATTEMPTING;
		this.modeEndTime = Time.time + this.attemptTime;
	}

	// Token: 0x06001548 RID: 5448 RVA: 0x00052AA4 File Offset: 0x00050CA4
	public override void Action()
	{
		if (this.target != null && !SRSingleton<SceneContext>.Instance.TimeDirector.IsFastForwarding() && !this.chomper.IsChomping())
		{
			base.MoveTowards(SlimeSubbehaviour.GetGotoPos(this.target), base.IsBlocked(this.target, 0, false), ref this.nextLeapAvail, this.DriveToJumpiness(this.currDrive) * this.maxJump);
		}
	}

	// Token: 0x06001549 RID: 5449 RVA: 0x00052B18 File Offset: 0x00050D18
	private float DriveToJumpiness(float drive)
	{
		float num = Mathf.Max(0f, drive - 0.666f) / 0.334f;
		return Mathf.Lerp(0.4f, 1f, num * num);
	}

	// Token: 0x04001424 RID: 5156
	public float maxJump = 12f;

	// Token: 0x04001425 RID: 5157
	public float attemptTime = 10f;

	// Token: 0x04001426 RID: 5158
	public float giveUpTime = 10f;

	// Token: 0x04001427 RID: 5159
	public bool shouldGotoPlayer;

	// Token: 0x04001428 RID: 5160
	public SlimeEmotions.Emotion driver;

	// Token: 0x04001429 RID: 5161
	public float extraDrive;

	// Token: 0x0400142A RID: 5162
	public float minDrive;

	// Token: 0x0400142B RID: 5163
	private GameObject target;

	// Token: 0x0400142C RID: 5164
	private float currDrive;

	// Token: 0x0400142D RID: 5165
	private float nextLeapAvail;

	// Token: 0x0400142E RID: 5166
	private Chomper chomper;

	// Token: 0x0400142F RID: 5167
	private const float AGITATION_PER_GIVE_UP = 0.1f;

	// Token: 0x04001430 RID: 5168
	private GotoPlayer.Mode mode;

	// Token: 0x04001431 RID: 5169
	private float modeEndTime;

	// Token: 0x04001432 RID: 5170
	private const float MAX_VEL_TO_BOUNCE = 0.1f;

	// Token: 0x04001433 RID: 5171
	private const float SQR_MAX_VEL_TO_BOUNCE = 0.010000001f;

	// Token: 0x020003FB RID: 1019
	private enum Mode
	{
		// Token: 0x04001435 RID: 5173
		AVAIL,
		// Token: 0x04001436 RID: 5174
		ATTEMPTING,
		// Token: 0x04001437 RID: 5175
		GIVE_UP
	}
}
