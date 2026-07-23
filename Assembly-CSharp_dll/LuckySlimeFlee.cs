using System;
using UnityEngine;

// Token: 0x02000404 RID: 1028
public class LuckySlimeFlee : SlimeSubbehaviour
{
	// Token: 0x06001575 RID: 5493 RVA: 0x000537B3 File Offset: 0x000519B3
	public override void Awake()
	{
		base.Awake();
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
	}

	// Token: 0x06001576 RID: 5494 RVA: 0x000537CC File Offset: 0x000519CC
	public void OnTriggerEnter(Collider collider)
	{
		Identifiable component = collider.gameObject.GetComponent<Identifiable>();
		if (this.fleeProximityDisappearTime == null && this.fleeTriggeredDisappearTime == null && component != null && component.id == Identifiable.Id.PLAYER)
		{
			this.Flee(collider.gameObject, false);
		}
	}

	// Token: 0x06001577 RID: 5495 RVA: 0x0005381F File Offset: 0x00051A1F
	public void StartFleeing(GameObject fleeFrom)
	{
		if (this.fleeTriggeredDisappearTime == null)
		{
			this.Flee(fleeFrom, true);
		}
	}

	// Token: 0x06001578 RID: 5496 RVA: 0x00053838 File Offset: 0x00051A38
	private void Flee(GameObject fleeFrom, bool triggered)
	{
		if (triggered)
		{
			this.fleeTriggeredDisappearTime = new double?(this.timeDir.WorldTime() + 600.0);
			this.fleeProximityDisappearTime = null;
		}
		else
		{
			this.fleeProximityDisappearTime = new double?(this.timeDir.WorldTime() + 600.0);
		}
		this.plexer.ForceRethink();
	}

	// Token: 0x06001579 RID: 5497 RVA: 0x000538A1 File Offset: 0x00051AA1
	public override float Relevancy(bool isGrounded)
	{
		if (this.fleeProximityDisappearTime != null || this.fleeTriggeredDisappearTime != null)
		{
			return 1f;
		}
		return 0f;
	}

	// Token: 0x0600157A RID: 5498 RVA: 0x00003296 File Offset: 0x00001496
	public override void Selected()
	{
	}

	// Token: 0x0600157B RID: 5499 RVA: 0x000538C8 File Offset: 0x00051AC8
	public override void Action()
	{
		if ((this.fleeProximityDisappearTime != null && this.timeDir.HasReached(this.fleeProximityDisappearTime.Value)) || (this.fleeTriggeredDisappearTime != null && this.timeDir.HasReached(this.fleeTriggeredDisappearTime.Value)))
		{
			SRBehaviour.SpawnAndPlayFX(this.disappearFX, base.transform.position, base.transform.rotation);
			if (this.disappearCue != null)
			{
				SECTR_AudioSystem.Play(this.disappearCue, base.transform.position, false);
			}
			Destroyer.DestroyActor(base.gameObject, "LuckySlimeFlee.Action", false);
		}
	}

	// Token: 0x0400146C RID: 5228
	public GameObject disappearFX;

	// Token: 0x0400146D RID: 5229
	public SECTR_AudioCue disappearCue;

	// Token: 0x0400146E RID: 5230
	private double? fleeProximityDisappearTime;

	// Token: 0x0400146F RID: 5231
	private double? fleeTriggeredDisappearTime;

	// Token: 0x04001470 RID: 5232
	private TimeDirector timeDir;

	// Token: 0x04001471 RID: 5233
	private const float FLEE_PROXIMITY_WORLD_TIME = 600f;

	// Token: 0x04001472 RID: 5234
	private const float FLEE_TRIGGERED_WORLD_TIME = 600f;
}
