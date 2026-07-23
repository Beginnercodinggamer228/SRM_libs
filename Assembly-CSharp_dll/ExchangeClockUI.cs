using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200057A RID: 1402
public class ExchangeClockUI : MonoBehaviour
{
	// Token: 0x06001D29 RID: 7465 RVA: 0x0006EC41 File Offset: 0x0006CE41
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.exchangeDir = SRSingleton<SceneContext>.Instance.ExchangeDirector;
		this.uiBundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("ui");
	}

	// Token: 0x06001D2A RID: 7466 RVA: 0x0006EC7D File Offset: 0x0006CE7D
	public void Update()
	{
		this.clockText.text = this.FormatTime(this.exchangeDir.GetOfferExpirationTime(this.offerType));
	}

	// Token: 0x06001D2B RID: 7467 RVA: 0x0006ECA4 File Offset: 0x0006CEA4
	private string FormatTime(double? time)
	{
		if (time == null)
		{
			return this.uiBundle.Get("l.time_hours_mins_unset");
		}
		int num = Mathf.FloorToInt((float)time.Value / 60f);
		if (num == this.lastMins)
		{
			return this.lastTimeStr;
		}
		this.lastMins = num;
		this.lastTimeStr = this.timeDir.FormatTime(this.lastMins);
		return this.lastTimeStr;
	}

	// Token: 0x04001C3A RID: 7226
	public Text clockText;

	// Token: 0x04001C3B RID: 7227
	public ExchangeDirector.OfferType offerType;

	// Token: 0x04001C3C RID: 7228
	private TimeDirector timeDir;

	// Token: 0x04001C3D RID: 7229
	private ExchangeDirector exchangeDir;

	// Token: 0x04001C3E RID: 7230
	private MessageBundle uiBundle;

	// Token: 0x04001C3F RID: 7231
	private int lastMins;

	// Token: 0x04001C40 RID: 7232
	private string lastTimeStr;
}
