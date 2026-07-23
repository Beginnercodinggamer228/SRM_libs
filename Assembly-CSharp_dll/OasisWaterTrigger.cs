using System;
using UnityEngine;

// Token: 0x0200073F RID: 1855
public class OasisWaterTrigger : SRBehaviour, LiquidConsumer
{
	// Token: 0x060026CC RID: 9932 RVA: 0x00093DD0 File Offset: 0x00091FD0
	public void Start()
	{
		bool flag = this.oasisToScale.IsLive();
		if (this.indicatorObj != null)
		{
			this.indicatorObj.SetActive(flag);
		}
		if (this.indicatorReplacesObj != null)
		{
			this.indicatorReplacesObj.SetActive(!flag);
		}
		this.hasAlreadyActivated = flag;
	}

	// Token: 0x060026CD RID: 9933 RVA: 0x00093E28 File Offset: 0x00092028
	public void AddLiquid(Identifiable.Id liquidId, float units)
	{
		if (this.oasisToScale != null && liquidId == Identifiable.Id.MAGIC_WATER_LIQUID && !this.hasAlreadyActivated)
		{
			this.oasisToScale.SetLive(false);
			if (this.scaleCue != null)
			{
				SECTR_AudioSystem.Play(this.scaleCue, base.transform.position, false);
			}
			if (this.scaleFX != null)
			{
				SRBehaviour.InstantiateDynamic(this.scaleFX, base.transform.position, base.transform.rotation, false);
			}
			if (this.indicatorObj != null)
			{
				this.indicatorObj.SetActive(true);
			}
			if (this.indicatorReplacesObj != null)
			{
				this.indicatorReplacesObj.SetActive(false);
			}
			this.hasAlreadyActivated = true;
		}
	}

	// Token: 0x040025FA RID: 9722
	public Oasis oasisToScale;

	// Token: 0x040025FB RID: 9723
	public SECTR_AudioCue scaleCue;

	// Token: 0x040025FC RID: 9724
	public GameObject scaleFX;

	// Token: 0x040025FD RID: 9725
	public GameObject indicatorObj;

	// Token: 0x040025FE RID: 9726
	public GameObject indicatorReplacesObj;

	// Token: 0x040025FF RID: 9727
	private bool hasAlreadyActivated;
}
