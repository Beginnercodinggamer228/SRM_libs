using System;
using UnityEngine;

// Token: 0x02000078 RID: 120
public interface SECTR_IAudioInstance
{
	// Token: 0x17000035 RID: 53
	// (get) Token: 0x06000225 RID: 549
	int Generation { get; }

	// Token: 0x17000036 RID: 54
	// (get) Token: 0x06000226 RID: 550
	bool Active { get; }

	// Token: 0x17000037 RID: 55
	// (get) Token: 0x06000227 RID: 551
	// (set) Token: 0x06000228 RID: 552
	Vector3 Position { get; set; }

	// Token: 0x17000038 RID: 56
	// (get) Token: 0x06000229 RID: 553
	// (set) Token: 0x0600022A RID: 554
	Vector3 LocalPosition { get; set; }

	// Token: 0x17000039 RID: 57
	// (get) Token: 0x0600022B RID: 555
	// (set) Token: 0x0600022C RID: 556
	float Volume { get; set; }

	// Token: 0x1700003A RID: 58
	// (get) Token: 0x0600022D RID: 557
	// (set) Token: 0x0600022E RID: 558
	float Pitch { get; set; }

	// Token: 0x1700003B RID: 59
	// (get) Token: 0x0600022F RID: 559
	// (set) Token: 0x06000230 RID: 560
	bool Mute { get; set; }

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x06000231 RID: 561
	// (set) Token: 0x06000232 RID: 562
	int TimeSamples { get; set; }

	// Token: 0x1700003D RID: 61
	// (get) Token: 0x06000233 RID: 563
	// (set) Token: 0x06000234 RID: 564
	float TimeSeconds { get; set; }

	// Token: 0x06000235 RID: 565
	void Stop(bool stopImmediately);

	// Token: 0x06000236 RID: 566
	void ForceInfinite();

	// Token: 0x06000237 RID: 567
	void ForceOcclusion(bool occluded);

	// Token: 0x06000238 RID: 568
	void SkipFadeIn();

	// Token: 0x06000239 RID: 569
	void Pause(bool paused);
}
