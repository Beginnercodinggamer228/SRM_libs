using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

// Token: 0x02000117 RID: 279
public class DestroyAfterTime : RegisteredActorBehaviour, RegistryUpdateable
{
	// Token: 0x06000600 RID: 1536 RVA: 0x00021FAC File Offset: 0x000201AC
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		float num = this.timeDir.CurrHourOrStart();
		bool flag = num < 6f || num > 18f;
		this.deathTime = this.timeDir.HoursFromNowOrStart((flag ? this.nightLifeTimeFactor : 1f) * this.lifeTimeHours);
		this.destroyAfterTimeCondition = base.GetComponent<DestroyAfterTimeCondition>();
		this.hasDestroyAfterTimeCondition = (this.destroyAfterTimeCondition != null);
	}

	// Token: 0x06000601 RID: 1537 RVA: 0x0002202C File Offset: 0x0002022C
	public void RegistryUpdate()
	{
		if (this.timeDir.HasReached(this.deathTime) && !this.destroying && (!this.hasDestroyAfterTimeCondition || this.destroyAfterTimeCondition.ReadyToDestroy()))
		{
			this.destroying = true;
			bool flag = this.timeDir.HasReached(this.deathTime + 3600.0);
			DestroyAfterTimeListener component = base.GetComponent<DestroyAfterTimeListener>();
			if (component != null)
			{
				component.WillDestroyAfterTime();
			}
			if (flag)
			{
				this.DoDestroy("DestroyAfterTime.RegistryUpdate (skippedFX)");
				return;
			}
			if (this.scaleDownOnDestroy)
			{
				base.StartCoroutine(this.ScaleThenDestroy());
			}
			else if (this.fizzleParticlesOnDestroy)
			{
				base.StartCoroutine(this.FizzleThenDestroy());
			}
			else
			{
				this.DoDestroy("DestroyAfterTime.RegistryUpdate");
			}
			if (this.destroyFX != null)
			{
				SRBehaviour.SpawnAndPlayFX(this.destroyFX, base.transform.position, Quaternion.identity);
			}
		}
	}

	// Token: 0x06000602 RID: 1538 RVA: 0x00022115 File Offset: 0x00020315
	private void DoDestroy(string reason)
	{
		if (this.destroyAsActor)
		{
			Destroyer.DestroyActor(base.gameObject, reason, false);
			return;
		}
		Destroyer.Destroy(base.gameObject, reason);
	}

	// Token: 0x06000603 RID: 1539 RVA: 0x00022139 File Offset: 0x00020339
	private IEnumerator FizzleThenDestroy()
	{
		ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Stop();
		}
		yield return new WaitForSeconds(4f);
		this.DoDestroy("DestroyAfterTime.FizzleThenDestroy");
		yield break;
	}

	// Token: 0x06000604 RID: 1540 RVA: 0x00022148 File Offset: 0x00020348
	private IEnumerator ScaleThenDestroy()
	{
		if (this.scaleDownCue != null)
		{
			SECTR_AudioSystem.Play(this.scaleDownCue, base.transform, Vector3.zero, false, null, false);
		}
		this.TweenScaleDownItem(base.gameObject);
		yield return new WaitForSeconds(4f);
		this.DoDestroy("DestroyAfterTime.ScaleThenDestroy");
		yield break;
	}

	// Token: 0x06000605 RID: 1541 RVA: 0x00022157 File Offset: 0x00020357
	private void TweenScaleDownItem(GameObject obj)
	{
		TweenUtil.ScaleOut(obj, 4f, Ease.InQuad);
	}

	// Token: 0x06000606 RID: 1542 RVA: 0x00022166 File Offset: 0x00020366
	public void AdvanceHours(float hours)
	{
		this.deathTime -= (double)(hours * 3600f);
	}

	// Token: 0x06000607 RID: 1543 RVA: 0x00022180 File Offset: 0x00020380
	public void MultiplyRemainingHours(float factor)
	{
		double num = this.deathTime - this.timeDir.WorldTime();
		this.deathTime = this.timeDir.WorldTime() + num * (double)factor;
	}

	// Token: 0x06000608 RID: 1544 RVA: 0x000221B6 File Offset: 0x000203B6
	public void SetDeathTime(double time)
	{
		this.deathTime = time;
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x000221BF File Offset: 0x000203BF
	public void ScaleDownOnDestroy()
	{
		this.scaleDownOnDestroy = true;
	}

	// Token: 0x0600060A RID: 1546 RVA: 0x000221C8 File Offset: 0x000203C8
	public void SetScaleDownCue(SECTR_AudioCue cue)
	{
		this.scaleDownCue = cue;
	}

	// Token: 0x0600060B RID: 1547 RVA: 0x000221D1 File Offset: 0x000203D1
	public void FizzleParticlesOnDestroy()
	{
		this.fizzleParticlesOnDestroy = true;
	}

	// Token: 0x0600060C RID: 1548 RVA: 0x000221DA File Offset: 0x000203DA
	public double GetDeathTime()
	{
		return this.deathTime;
	}

	// Token: 0x040005D0 RID: 1488
	public float lifeTimeHours = 72f;

	// Token: 0x040005D1 RID: 1489
	public float nightLifeTimeFactor = 1f;

	// Token: 0x040005D2 RID: 1490
	public GameObject destroyFX;

	// Token: 0x040005D3 RID: 1491
	public bool destroyAsActor = true;

	// Token: 0x040005D4 RID: 1492
	private TimeDirector timeDir;

	// Token: 0x040005D5 RID: 1493
	private double deathTime;

	// Token: 0x040005D6 RID: 1494
	private bool scaleDownOnDestroy;

	// Token: 0x040005D7 RID: 1495
	private SECTR_AudioCue scaleDownCue;

	// Token: 0x040005D8 RID: 1496
	private bool fizzleParticlesOnDestroy;

	// Token: 0x040005D9 RID: 1497
	private bool destroying;

	// Token: 0x040005DA RID: 1498
	private DestroyAfterTimeCondition destroyAfterTimeCondition;

	// Token: 0x040005DB RID: 1499
	private bool hasDestroyAfterTimeCondition;

	// Token: 0x040005DC RID: 1500
	private const float SCALE_DOWN_TIME = 4f;

	// Token: 0x040005DD RID: 1501
	private const float FIZZLE_TIME = 4f;
}
