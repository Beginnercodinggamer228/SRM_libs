using System;
using Assets.Script.Util.Extensions;
using UnityEngine;

// Token: 0x0200071A RID: 1818
public class GlitchTarrNodeMusic : SECTR_PointSource
{
	// Token: 0x060025F7 RID: 9719 RVA: 0x0009143C File Offset: 0x0008F63C
	protected override void Start()
	{
		base.Start();
		if (Application.isPlaying)
		{
			this.maxVolume = UnityEngine.Random.Range(this.Cue.Volume.x, this.Cue.Volume.y);
			GlitchTarrNode requiredComponentInParent = base.gameObject.GetRequiredComponentInParent(false);
			this.minDistance = 10f * requiredComponentInParent.scale.x;
			this.maxDistance = 20f * requiredComponentInParent.scale.x;
			this.minDistance *= this.minDistance;
			this.maxDistance *= this.maxDistance;
		}
	}

	// Token: 0x060025F8 RID: 9720 RVA: 0x000914E5 File Offset: 0x0008F6E5
	protected void Update()
	{
		if (Application.isPlaying && this.IsPlaying)
		{
			this.instance.Volume = this.maxVolume * this.GetCurrentMultiplier();
		}
	}

	// Token: 0x060025F9 RID: 9721 RVA: 0x00091510 File Offset: 0x0008F710
	private float GetCurrentMultiplier()
	{
		float sqrMagnitude = (SRSingleton<SceneContext>.Instance.Player.transform.position - base.transform.position).sqrMagnitude;
		if (sqrMagnitude <= this.minDistance)
		{
			return 1f;
		}
		if (sqrMagnitude >= this.maxDistance)
		{
			return 0f;
		}
		return 1f - (sqrMagnitude - this.minDistance) / (this.maxDistance - this.minDistance);
	}

	// Token: 0x04002553 RID: 9555
	private const int MIN_DISTANCE = 10;

	// Token: 0x04002554 RID: 9556
	private const int MAX_DISTANCE = 20;

	// Token: 0x04002555 RID: 9557
	private float minDistance;

	// Token: 0x04002556 RID: 9558
	private float maxDistance;

	// Token: 0x04002557 RID: 9559
	private float maxVolume;
}
