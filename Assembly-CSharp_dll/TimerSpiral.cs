using System;
using UnityEngine;

// Token: 0x0200052D RID: 1325
public class TimerSpiral : MonoBehaviour
{
	// Token: 0x06001B94 RID: 7060 RVA: 0x0006985B File Offset: 0x00067A5B
	public void Awake()
	{
		this.renderer = base.GetComponent<Renderer>();
		this.renderer.material.SetFloat("_Timer", 0f);
		this.ratio = 0.0;
	}

	// Token: 0x06001B95 RID: 7061 RVA: 0x00069894 File Offset: 0x00067A94
	public void SetTimeSource(VacDisplayTimer.TimeSource source)
	{
		this.source = source;
		double? num = (source != null) ? source.GetTimeRemaining() : null;
		double? num2 = (source != null) ? source.GetMaxTimeRemaining() : null;
		this.ratio = ((num2 == null) ? 0.0 : (0.5 / num2.Value));
		this.renderer.material.SetFloat("_Timer", (num != null) ? ((float)(this.ratio * num.Value)) : 0f);
	}

	// Token: 0x06001B96 RID: 7062 RVA: 0x00069931 File Offset: 0x00067B31
	public void SetWarningThreshold(float percentage)
	{
		this.renderer.material.SetFloat("_WarningThreshold", percentage);
	}

	// Token: 0x06001B97 RID: 7063 RVA: 0x0006994C File Offset: 0x00067B4C
	public void Update()
	{
		if (this.ratio > 0.0 && this.source != null)
		{
			float @float = this.renderer.material.GetFloat("_Timer");
			float num = Mathf.Clamp01((float)(this.ratio * this.source.GetTimeRemaining().Value));
			this.renderer.material.SetFloat("_Timer", @float + Mathf.Max(-0.004f, Mathf.Min(0.004f, num - @float)));
		}
	}

	// Token: 0x06001B98 RID: 7064 RVA: 0x000699D7 File Offset: 0x00067BD7
	public void OnDestroy()
	{
		this.renderer.material.SetFloat("_Timer", 0f);
	}

	// Token: 0x04001AC1 RID: 6849
	private const float MAX_CHANGE_PER_FRAME = 0.004f;

	// Token: 0x04001AC2 RID: 6850
	private VacDisplayTimer.TimeSource source;

	// Token: 0x04001AC3 RID: 6851
	private Renderer renderer;

	// Token: 0x04001AC4 RID: 6852
	private double ratio;
}
