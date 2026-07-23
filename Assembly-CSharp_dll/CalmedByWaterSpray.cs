using System;
using UnityEngine;

// Token: 0x020003A2 RID: 930
public class CalmedByWaterSpray : MonoBehaviour, LiquidConsumer
{
	// Token: 0x0600136C RID: 4972 RVA: 0x0004B948 File Offset: 0x00049B48
	public void Awake()
	{
		this.emotions = base.GetComponent<SlimeEmotions>();
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
	}

	// Token: 0x0600136D RID: 4973 RVA: 0x0004B966 File Offset: 0x00049B66
	public void AddLiquid(Identifiable.Id liquidId, float units)
	{
		if (Identifiable.IsWater(liquidId))
		{
			this.emotions.Adjust(SlimeEmotions.Emotion.AGITATION, -this.agitationReduction * units);
			this.calmedUntil = this.timeDir.HoursFromNow(this.calmedHours);
		}
	}

	// Token: 0x0600136E RID: 4974 RVA: 0x0004B99D File Offset: 0x00049B9D
	public bool IsCalmed()
	{
		return !this.timeDir.HasReached(this.calmedUntil);
	}

	// Token: 0x04001226 RID: 4646
	public float agitationReduction = 0.1f;

	// Token: 0x04001227 RID: 4647
	public float calmedHours = 0.3333f;

	// Token: 0x04001228 RID: 4648
	private double calmedUntil;

	// Token: 0x04001229 RID: 4649
	private SlimeEmotions emotions;

	// Token: 0x0400122A RID: 4650
	private TimeDirector timeDir;
}
