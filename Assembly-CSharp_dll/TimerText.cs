using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200062D RID: 1581
public class TimerText : Text
{
	// Token: 0x0600212D RID: 8493 RVA: 0x0007EE34 File Offset: 0x0007D034
	protected override void Start()
	{
		base.Start();
		if (SRSingleton<SceneContext>.Instance != null)
		{
			this.text = SRSingleton<SceneContext>.Instance.TimeDirector.FormatTimeMinutes(null);
		}
	}

	// Token: 0x0600212E RID: 8494 RVA: 0x0007EE74 File Offset: 0x0007D074
	public void UpdateTimeRemaining(double? secondsRemaining)
	{
		int? num = this.RoundTimeToMinutes(secondsRemaining);
		int? num2 = num;
		int? num3 = this.priorMinutes;
		if (!(num2.GetValueOrDefault() == num3.GetValueOrDefault() & num2 != null == (num3 != null)))
		{
			this.text = SRSingleton<SceneContext>.Instance.TimeDirector.FormatTimeMinutes(num);
			this.priorMinutes = num;
		}
	}

	// Token: 0x0600212F RID: 8495 RVA: 0x0007EED4 File Offset: 0x0007D0D4
	private int? RoundTimeToMinutes(double? timeInSeconds)
	{
		if (timeInSeconds == null)
		{
			return null;
		}
		return new int?(Mathf.CeilToInt((float)timeInSeconds.Value * 0.016666668f));
	}

	// Token: 0x04002086 RID: 8326
	private int? priorMinutes;
}
