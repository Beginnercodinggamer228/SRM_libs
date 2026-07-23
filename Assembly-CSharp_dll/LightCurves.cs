using System;
using UnityEngine;

// Token: 0x0200005A RID: 90
public class LightCurves : MonoBehaviour
{
	// Token: 0x06000186 RID: 390 RVA: 0x0000B5F1 File Offset: 0x000097F1
	private void Start()
	{
		this.lightSource = base.GetComponent<Light>();
	}

	// Token: 0x06000187 RID: 391 RVA: 0x0000B5FF File Offset: 0x000097FF
	private void OnEnable()
	{
		this.startTime = Time.time;
	}

	// Token: 0x06000188 RID: 392 RVA: 0x0000B60C File Offset: 0x0000980C
	private void Update()
	{
		float num = Time.time - this.startTime;
		if (num <= this.GraphScaleX)
		{
			float intensity = this.LightCurve.Evaluate(num / this.GraphScaleX) * this.GraphScaleY;
			this.lightSource.intensity = intensity;
		}
	}

	// Token: 0x040001B6 RID: 438
	public AnimationCurve LightCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x040001B7 RID: 439
	public float GraphScaleX = 1f;

	// Token: 0x040001B8 RID: 440
	public float GraphScaleY = 1f;

	// Token: 0x040001B9 RID: 441
	private float startTime;

	// Token: 0x040001BA RID: 442
	private Light lightSource;
}
