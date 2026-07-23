using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x020006FF RID: 1791
public class FillableAshSource : AshSource, LandPlotModel.Participant
{
	// Token: 0x06002554 RID: 9556 RVA: 0x0008F680 File Offset: 0x0008D880
	public override void Awake()
	{
		AshSource.allAshes.Add(this);
		this.body = base.GetComponent<Rigidbody>();
		this.initPos = base.transform.localPosition;
		this.UpdateAshPosition();
	}

	// Token: 0x06002555 RID: 9557 RVA: 0x0008F6B0 File Offset: 0x0008D8B0
	public void InitModel(LandPlotModel model)
	{
		model.ashUnits = (float)this.maxUnits;
	}

	// Token: 0x06002556 RID: 9558 RVA: 0x0008F6BF File Offset: 0x0008D8BF
	public void SetModel(LandPlotModel model)
	{
		this.plotModel = model;
		this.UpdateAshPosition();
	}

	// Token: 0x06002557 RID: 9559 RVA: 0x0008F6CE File Offset: 0x0008D8CE
	public override bool Available()
	{
		return this.plotModel.ashUnits >= 1f;
	}

	// Token: 0x06002558 RID: 9560 RVA: 0x0008F6E5 File Offset: 0x0008D8E5
	public override void ConsumeAsh()
	{
		this.plotModel.ashUnits = Mathf.Max(this.plotModel.ashUnits - 1f, 0f);
		this.UpdateAshPosition();
	}

	// Token: 0x06002559 RID: 9561 RVA: 0x0008F713 File Offset: 0x0008D913
	public void AddAsh(float amount)
	{
		this.plotModel.ashUnits = Mathf.Min(this.plotModel.ashUnits + amount, (float)this.maxUnits);
		this.UpdateAshPosition();
	}

	// Token: 0x0600255A RID: 9562 RVA: 0x0008F740 File Offset: 0x0008D940
	private void UpdateAshPosition()
	{
		if (this.plotModel != null && base.gameObject.activeInHierarchy)
		{
			Tween tween = this.ashMoveTween;
			if (tween != null)
			{
				tween.Kill(false);
			}
			this.ashMoveTween = this.body.DOMoveY(this.GetAshYPosition(), this.ashFillSpeed, false).SetSpeedBased(true);
		}
	}

	// Token: 0x0600255B RID: 9563 RVA: 0x0008F798 File Offset: 0x0008D998
	private float GetAshYPosition()
	{
		return base.transform.parent.TransformPoint(0f, Mathf.Max(this.initPos.y * (this.plotModel.ashUnits / (float)this.maxUnits), this.minYPos), 0f).y;
	}

	// Token: 0x0600255C RID: 9564 RVA: 0x0008F7EE File Offset: 0x0008D9EE
	public float GetAshSpace()
	{
		return (float)this.maxUnits - this.plotModel.ashUnits;
	}

	// Token: 0x04002441 RID: 9281
	[Tooltip("The maximum number of consumable units this can hold.")]
	public int maxUnits = 20;

	// Token: 0x04002442 RID: 9282
	public float minYPos = 0.05f;

	// Token: 0x04002443 RID: 9283
	private float ashFillSpeed = 0.25f;

	// Token: 0x04002444 RID: 9284
	private Vector3 initPos;

	// Token: 0x04002445 RID: 9285
	private LandPlotModel plotModel;

	// Token: 0x04002446 RID: 9286
	private Tween ashMoveTween;

	// Token: 0x04002447 RID: 9287
	private Rigidbody body;
}
