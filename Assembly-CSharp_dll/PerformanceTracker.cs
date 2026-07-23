using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

// Token: 0x02000254 RID: 596
public class PerformanceTracker : MonoBehaviour
{
	// Token: 0x06000CC8 RID: 3272 RVA: 0x000349B4 File Offset: 0x00032BB4
	public void Update()
	{
		this.frameCount += 1f;
		this.dt += Time.deltaTime;
		if (this.dt > this.updateRate)
		{
			this.fps = this.frameCount / this.dt;
			this.frameCount = 0f;
			this.dt -= this.updateRate;
			this.fpsSum += (double)this.fps;
			this.fpsCount++;
		}
		this.maxHeapSize = Math.Max(this.maxHeapSize, Profiler.usedHeapSizeLong);
	}

	// Token: 0x06000CC9 RID: 3273 RVA: 0x00034A5C File Offset: 0x00032C5C
	public void OnApplicationQuit()
	{
		if (this.fpsCount > 0)
		{
			AnalyticsUtil.CustomEvent("PerfSummary", new Dictionary<string, object>
			{
				{
					"meanFps",
					(int)Math.Round(this.fpsSum / (double)this.fpsCount)
				},
				{
					"maxMem",
					this.maxHeapSize / 1048576L
				}
			}, false);
		}
	}

	// Token: 0x04000B95 RID: 2965
	private float frameCount;

	// Token: 0x04000B96 RID: 2966
	private float dt;

	// Token: 0x04000B97 RID: 2967
	private float fps;

	// Token: 0x04000B98 RID: 2968
	private float updateRate = 1f;

	// Token: 0x04000B99 RID: 2969
	private double fpsSum;

	// Token: 0x04000B9A RID: 2970
	private int fpsCount;

	// Token: 0x04000B9B RID: 2971
	private long maxHeapSize;

	// Token: 0x04000B9C RID: 2972
	private const int BYTES_PER_MEG = 1048576;
}
