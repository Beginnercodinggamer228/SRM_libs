using System;
using UnityEngine;

// Token: 0x0200006A RID: 106
[RequireComponent(typeof(ParticleSystem))]
public class CFX_AutoDestructShuriken : MonoBehaviour
{
	// Token: 0x060001D7 RID: 471 RVA: 0x0000E2D9 File Offset: 0x0000C4D9
	public void Awake()
	{
		this.particlesRunWhilePaused = base.GetComponent<ParticlesRunWhilePaused>();
	}

	// Token: 0x060001D8 RID: 472 RVA: 0x0000E2E8 File Offset: 0x0000C4E8
	public void OnEnable()
	{
		ParticleSystem.MainModule main = base.GetComponent<ParticleSystem>().main;
		this.endTime = (main.loop ? float.PositiveInfinity : (this.GetTime() + main.duration * 1.5f));
	}

	// Token: 0x060001D9 RID: 473 RVA: 0x0000E32B File Offset: 0x0000C52B
	private float GetTime()
	{
		if (this.particlesRunWhilePaused != null && this.particlesRunWhilePaused.enabled)
		{
			return Time.unscaledTime;
		}
		return Time.time;
	}

	// Token: 0x060001DA RID: 474 RVA: 0x0000E354 File Offset: 0x0000C554
	public void Update()
	{
		float time = this.GetTime();
		if (this.nextCheckTime > time)
		{
			return;
		}
		if (this.endTime > this.GetTime() && base.GetComponent<ParticleSystem>().IsAlive(true))
		{
			this.nextCheckTime = time + 0.5f;
			return;
		}
		if (this.OnlyDeactivate)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (!this.RecycleOnCompletion)
		{
			Destroyer.Destroy(base.gameObject, "CFX_AutoDestructShuriken.Update");
			return;
		}
		if (this.RecycleParent)
		{
			SRSingleton<SceneContext>.Instance.fxPool.Recycle(base.transform.parent.gameObject);
			return;
		}
		SRSingleton<SceneContext>.Instance.fxPool.Recycle(base.gameObject);
	}

	// Token: 0x04000236 RID: 566
	public bool OnlyDeactivate;

	// Token: 0x04000237 RID: 567
	public bool RecycleOnCompletion;

	// Token: 0x04000238 RID: 568
	public bool RecycleParent;

	// Token: 0x04000239 RID: 569
	private float nextCheckTime;

	// Token: 0x0400023A RID: 570
	private float endTime;

	// Token: 0x0400023B RID: 571
	private const float CHECK_DELAY = 0.5f;

	// Token: 0x0400023C RID: 572
	private ParticlesRunWhilePaused particlesRunWhilePaused;

	// Token: 0x0400023D RID: 573
	private const float LIFETIME_SAFETY_MARGIN = 1.5f;
}
