using System;
using TMPro;
using UnityEngine;

// Token: 0x02000595 RID: 1429
public class HUDCountdownUI : MonoBehaviour
{
	// Token: 0x06001DBA RID: 7610 RVA: 0x0007141D File Offset: 0x0006F61D
	public void Awake()
	{
		this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001DBB RID: 7611 RVA: 0x0007143C File Offset: 0x0006F63C
	public void Update()
	{
		int num = Mathf.CeilToInt((float)(this.time - this.timeDirector.WorldTime()) % 3600f / 60f);
		this.text.text = string.Format("{0}", num);
		base.gameObject.SetActive(num >= 0);
	}

	// Token: 0x06001DBC RID: 7612 RVA: 0x0007149B File Offset: 0x0006F69B
	public void SetCountdownTime(double minutes)
	{
		this.time = this.timeDirector.HoursFromNow((float)minutes * 0.016666668f);
		base.gameObject.SetActive(true);
	}

	// Token: 0x04001CCF RID: 7375
	[Tooltip("Countdown text.")]
	public TMP_Text text;

	// Token: 0x04001CD0 RID: 7376
	private TimeDirector timeDirector;

	// Token: 0x04001CD1 RID: 7377
	private double time;
}
