using System;
using UnityEngine;

// Token: 0x02000423 RID: 1059
public class PuddleSlimeScoot : SlimeSubbehaviour
{
	// Token: 0x0600160B RID: 5643 RVA: 0x00055734 File Offset: 0x00053934
	public override void Awake()
	{
		base.Awake();
		Collider[] components = base.GetComponents<Collider>();
		int num = 0;
		while (num < components.Length && components[num].isTrigger)
		{
			num++;
		}
	}

	// Token: 0x0600160C RID: 5644 RVA: 0x0004BA2F File Offset: 0x00049C2F
	public override void Start()
	{
		base.Start();
	}

	// Token: 0x0600160D RID: 5645 RVA: 0x00055766 File Offset: 0x00053966
	public override bool Forbids(SlimeSubbehaviour toMaybeForbid)
	{
		return toMaybeForbid is SlimeRandomMove;
	}

	// Token: 0x0600160E RID: 5646 RVA: 0x0004BA37 File Offset: 0x00049C37
	public override float Relevancy(bool isGrounded)
	{
		return 0.2f;
	}

	// Token: 0x0600160F RID: 5647 RVA: 0x00055771 File Offset: 0x00053971
	public override void Selected()
	{
		this.SelectMode();
	}

	// Token: 0x06001610 RID: 5648 RVA: 0x000526CC File Offset: 0x000508CC
	public override void Deselected()
	{
		base.Deselected();
	}

	// Token: 0x06001611 RID: 5649 RVA: 0x0005577C File Offset: 0x0005397C
	private void SelectMode()
	{
		if (this.canRicochet && base.IsBlocked(null, 0, false))
		{
			this.mode = PuddleSlimeScoot.Mode.RICOCHET;
			this.ricochetDir = -base.transform.forward;
		}
		else
		{
			this.mode = (Randoms.SHARED.GetProbability(0.2f) ? PuddleSlimeScoot.Mode.TURN : PuddleSlimeScoot.Mode.SCOOT);
		}
		this.nextModeChoice = Time.fixedTime + 1f;
		if (this.mode == PuddleSlimeScoot.Mode.TURN)
		{
			this.turnTorque = (float)(Randoms.SHARED.GetBoolean() ? 1 : -1) * Randoms.SHARED.GetInRange(1f, 2f);
			return;
		}
		this.turnTorque = 0f;
	}

	// Token: 0x06001612 RID: 5650 RVA: 0x00055828 File Offset: 0x00053A28
	public override void Action()
	{
		if (Time.fixedTime >= this.nextModeChoice)
		{
			this.SelectMode();
		}
		if (base.IsGrounded())
		{
			Rigidbody component = base.GetComponent<Rigidbody>();
			if (this.mode == PuddleSlimeScoot.Mode.RICOCHET)
			{
				base.RotateTowards(this.ricochetDir, 5f, 1f);
				component.AddForce(this.ricochetDir * (80f * this.straightlineForceFactor * component.mass * Time.fixedDeltaTime));
				Vector3 position = base.transform.position + Vector3.down * (0.5f * base.transform.localScale.y);
				component.AddForceAtPosition(this.ricochetDir * (240f * this.straightlineForceFactor * component.mass * Time.fixedDeltaTime), position);
				return;
			}
			if (this.mode == PuddleSlimeScoot.Mode.TURN)
			{
				float num = base.IsFloating() ? 0.2f : 1f;
				component.AddForce(base.transform.forward * (-80f * num * component.mass * Time.fixedDeltaTime));
				Vector3 position2 = base.transform.position + Vector3.down * (0.5f * base.transform.localScale.y);
				component.AddForceAtPosition(base.transform.forward * (-240f * num * component.mass * Time.fixedDeltaTime), position2);
				component.AddTorque(0f, this.turnTorque * Time.fixedDeltaTime, 0f);
				return;
			}
			component.AddForce(base.transform.forward * (this.straightlineForceFactor * 80f * component.mass * Time.fixedDeltaTime));
			Vector3 position3 = base.transform.position + Vector3.down * (0.5f * base.transform.localScale.y);
			component.AddForceAtPosition(base.transform.forward * (this.straightlineForceFactor * 240f * component.mass * Time.fixedDeltaTime), position3);
		}
	}

	// Token: 0x040014FA RID: 5370
	public float straightlineForceFactor = 1f;

	// Token: 0x040014FB RID: 5371
	public bool canRicochet;

	// Token: 0x040014FC RID: 5372
	private PuddleSlimeScoot.Mode mode;

	// Token: 0x040014FD RID: 5373
	private float turnTorque;

	// Token: 0x040014FE RID: 5374
	private float nextModeChoice;

	// Token: 0x040014FF RID: 5375
	private Vector3 ricochetDir;

	// Token: 0x04001500 RID: 5376
	private const float TURN_PROB = 0.2f;

	// Token: 0x04001501 RID: 5377
	private const float MIN_TURN_TORQUE = 1f;

	// Token: 0x04001502 RID: 5378
	private const float MAX_TURN_TORQUE = 2f;

	// Token: 0x02000424 RID: 1060
	private enum Mode
	{
		// Token: 0x04001504 RID: 5380
		SCOOT,
		// Token: 0x04001505 RID: 5381
		TURN,
		// Token: 0x04001506 RID: 5382
		RICOCHET
	}
}
