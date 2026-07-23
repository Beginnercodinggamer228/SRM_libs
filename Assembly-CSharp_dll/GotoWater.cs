using System;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x020003FC RID: 1020
public class GotoWater : FindConsumable
{
	// Token: 0x0600154B RID: 5451 RVA: 0x00052B78 File Offset: 0x00050D78
	public override void Start()
	{
		base.Start();
		this.nonWater = base.GetComponent<DestroyOnTouching>();
	}

	// Token: 0x0600154C RID: 5452 RVA: 0x00052B8C File Offset: 0x00050D8C
	public override float Relevancy(bool isGrounded)
	{
		if (this.nonWater == null)
		{
			return 0f;
		}
		this.FindNearestWaterLoc();
		if (this.tgtLoc == null)
		{
			return 0f;
		}
		return 1f - this.nonWater.PctTimeToDestruct();
	}

	// Token: 0x0600154D RID: 5453 RVA: 0x00003296 File Offset: 0x00001496
	public override void Selected()
	{
	}

	// Token: 0x0600154E RID: 5454 RVA: 0x00052BCC File Offset: 0x00050DCC
	private void FindNearestWaterLoc()
	{
		float num = 900f;
		LiquidSourceModel liquidSourceModel = null;
		foreach (LiquidSourceModel liquidSourceModel2 in SRSingleton<SceneContext>.Instance.GameModel.LiquidSources.Instances)
		{
			float sqrMagnitude = (liquidSourceModel2.pos - base.transform.position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				liquidSourceModel = liquidSourceModel2;
				num = sqrMagnitude;
			}
		}
		if (liquidSourceModel != null)
		{
			this.tgtLoc = new Vector3?(liquidSourceModel.pos);
			return;
		}
		this.tgtLoc = null;
	}

	// Token: 0x0600154F RID: 5455 RVA: 0x000526CC File Offset: 0x000508CC
	public override void Deselected()
	{
		base.Deselected();
	}

	// Token: 0x06001550 RID: 5456 RVA: 0x00052C74 File Offset: 0x00050E74
	public override void Action()
	{
		if (this.tgtLoc != null && base.IsGrounded())
		{
			float positiveInfinity = float.PositiveInfinity;
			base.MoveTowards(this.tgtLoc.Value, false, ref positiveInfinity, 0f);
		}
	}

	// Token: 0x04001438 RID: 5176
	private DestroyOnTouching nonWater;

	// Token: 0x04001439 RID: 5177
	private Vector3? tgtLoc;

	// Token: 0x0400143A RID: 5178
	private const float GAME_MINS_THRESHOLD = 20f;

	// Token: 0x0400143B RID: 5179
	private const float SEARCH_RAD = 30f;

	// Token: 0x0400143C RID: 5180
	private const float SEARCH_RAD_SQR = 900f;
}
