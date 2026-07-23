using System;
using TMPro;
using UnityEngine;

// Token: 0x0200062C RID: 1580
public class TimeLimitV2IntroUI : MonoBehaviour
{
	// Token: 0x0600212A RID: 8490 RVA: 0x0007EDB8 File Offset: 0x0007CFB8
	public void Awake()
	{
		this.time = Time.unscaledTime + 3f;
	}

	// Token: 0x0600212B RID: 8491 RVA: 0x0007EDCC File Offset: 0x0007CFCC
	public void Update()
	{
		this.text.text = string.Format("{0}", Mathf.CeilToInt(this.time - Time.unscaledTime));
		if (Time.unscaledTime >= this.time)
		{
			Destroyer.Destroy(base.gameObject, "TimeLimitV2IntroUI.Update");
		}
	}

	// Token: 0x04002083 RID: 8323
	[Tooltip("Countdown text.")]
	public TMP_Text text;

	// Token: 0x04002084 RID: 8324
	[Tooltip("Duration, in real-time seconds, to countdown.")]
	public float countdown = 3f;

	// Token: 0x04002085 RID: 8325
	private float time;
}
