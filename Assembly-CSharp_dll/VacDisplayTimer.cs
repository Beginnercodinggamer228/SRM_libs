using System;
using UnityEngine;

// Token: 0x020002E0 RID: 736
public class VacDisplayTimer : MonoBehaviour
{
	// Token: 0x06000FB9 RID: 4025 RVA: 0x0003E134 File Offset: 0x0003C334
	public void Awake()
	{
		this.spiral = this.renderer.gameObject.AddComponent<TimerSpiral>();
		this.timer.UpdateTimeRemaining(null);
	}

	// Token: 0x06000FBA RID: 4026 RVA: 0x0003E16C File Offset: 0x0003C36C
	public void Update()
	{
		double? secondsRemaining = (this.source == null) ? null : this.source.GetTimeRemaining();
		double? num = (this.source == null) ? null : this.source.GetWarningTimeSeconds();
		this.timer.UpdateTimeRemaining(secondsRemaining);
		this.spiral.SetWarningThreshold((this.source != null) ? ((secondsRemaining != null && num != null) ? ((float)((secondsRemaining.Value >= num.Value) ? 0 : 1)) : 0.2f) : 0f);
	}

	// Token: 0x06000FBB RID: 4027 RVA: 0x0003E20B File Offset: 0x0003C40B
	public void OnDestroy()
	{
		this.SetTimeSource(null);
	}

	// Token: 0x06000FBC RID: 4028 RVA: 0x0003E214 File Offset: 0x0003C414
	public void SetQuicksilverEnergyGenerator(QuicksilverEnergyGenerator generator)
	{
		if (this.generator != null)
		{
			QuicksilverEnergyGenerator quicksilverEnergyGenerator = this.generator;
			quicksilverEnergyGenerator.onStateChanged = (QuicksilverEnergyGenerator.OnStateChanged)Delegate.Remove(quicksilverEnergyGenerator.onStateChanged, new QuicksilverEnergyGenerator.OnStateChanged(this.OnQuicksilverEnergyGeneratorStateChanged));
		}
		this.SetTimeSource(generator);
		this.generator = generator;
		if (this.generator != null)
		{
			QuicksilverEnergyGenerator quicksilverEnergyGenerator2 = this.generator;
			quicksilverEnergyGenerator2.onStateChanged = (QuicksilverEnergyGenerator.OnStateChanged)Delegate.Combine(quicksilverEnergyGenerator2.onStateChanged, new QuicksilverEnergyGenerator.OnStateChanged(this.OnQuicksilverEnergyGeneratorStateChanged));
			this.OnQuicksilverEnergyGeneratorStateChanged();
		}
	}

	// Token: 0x06000FBD RID: 4029 RVA: 0x0003E29F File Offset: 0x0003C49F
	private void OnQuicksilverEnergyGeneratorStateChanged()
	{
		this.SetTimeSource((this.generator.GetState() == QuicksilverEnergyGenerator.State.ACTIVE || this.generator.GetState() == QuicksilverEnergyGenerator.State.COUNTDOWN) ? this.generator : null);
	}

	// Token: 0x06000FBE RID: 4030 RVA: 0x0003E2CC File Offset: 0x0003C4CC
	public void SetTimeSource(VacDisplayTimer.TimeSource source)
	{
		this.source = source;
		this.spiral.SetTimeSource(source);
	}

	// Token: 0x04000E81 RID: 3713
	[Tooltip("Text object to update with the time.")]
	public TimerText timer;

	// Token: 0x04000E82 RID: 3714
	[Tooltip("Renderer containing the color spiral.")]
	public Renderer renderer;

	// Token: 0x04000E83 RID: 3715
	private QuicksilverEnergyGenerator generator;

	// Token: 0x04000E84 RID: 3716
	private TimerSpiral spiral;

	// Token: 0x04000E85 RID: 3717
	private VacDisplayTimer.TimeSource source;

	// Token: 0x020002E1 RID: 737
	public interface TimeSource
	{
		// Token: 0x06000FC0 RID: 4032
		double? GetTimeRemaining();

		// Token: 0x06000FC1 RID: 4033
		double? GetMaxTimeRemaining();

		// Token: 0x06000FC2 RID: 4034
		double? GetWarningTimeSeconds();
	}
}
