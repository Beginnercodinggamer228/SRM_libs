using System;
using UnityEngine;

// Token: 0x020003F7 RID: 1015
public class GotoAsh : FindConsumable
{
	// Token: 0x06001535 RID: 5429 RVA: 0x000525C6 File Offset: 0x000507C6
	public override void Start()
	{
		base.Start();
		this.nonAsh = base.GetComponent<DestroyOnTouching>();
	}

	// Token: 0x06001536 RID: 5430 RVA: 0x000525DA File Offset: 0x000507DA
	public override float Relevancy(bool isGrounded)
	{
		if (this.nonAsh == null)
		{
			return 0f;
		}
		this.FindNearestAshLoc();
		if (this.tgtLoc == null)
		{
			return 0f;
		}
		return 1f - this.nonAsh.PctTimeToDestruct();
	}

	// Token: 0x06001537 RID: 5431 RVA: 0x00003296 File Offset: 0x00001496
	public override void Selected()
	{
	}

	// Token: 0x06001538 RID: 5432 RVA: 0x0005261C File Offset: 0x0005081C
	private void FindNearestAshLoc()
	{
		float num = 900f;
		AshSource ashSource = null;
		foreach (AshSource ashSource2 in AshSource.allAshes)
		{
			float sqrMagnitude = (ashSource2.transform.position - base.transform.position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				ashSource = ashSource2;
				num = sqrMagnitude;
			}
		}
		if (ashSource != null)
		{
			this.tgtLoc = new Vector3?(ashSource.transform.position);
			return;
		}
		this.tgtLoc = null;
	}

	// Token: 0x06001539 RID: 5433 RVA: 0x000526CC File Offset: 0x000508CC
	public override void Deselected()
	{
		base.Deselected();
	}

	// Token: 0x0600153A RID: 5434 RVA: 0x000526D4 File Offset: 0x000508D4
	public override void Action()
	{
		if (this.tgtLoc != null && base.IsGrounded())
		{
			float positiveInfinity = float.PositiveInfinity;
			base.MoveTowards(this.tgtLoc.Value, false, ref positiveInfinity, 0f);
		}
	}

	// Token: 0x0400140C RID: 5132
	private DestroyOnTouching nonAsh;

	// Token: 0x0400140D RID: 5133
	private Vector3? tgtLoc;

	// Token: 0x0400140E RID: 5134
	private const float GAME_MINS_THRESHOLD = 20f;

	// Token: 0x0400140F RID: 5135
	private const float SEARCH_RAD = 30f;

	// Token: 0x04001410 RID: 5136
	private const float SEARCH_RAD_SQR = 900f;
}
