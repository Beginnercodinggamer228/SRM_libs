using System;
using UnityEngine;

// Token: 0x02000700 RID: 1792
public class FireBall : SRBehaviour
{
	// Token: 0x0600255E RID: 9566 RVA: 0x0008F829 File Offset: 0x0008DA29
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.timeToDie = this.timeDir.HoursFromNow(this.hoursToLive);
	}

	// Token: 0x0600255F RID: 9567 RVA: 0x0008F852 File Offset: 0x0008DA52
	public void OnEnable()
	{
		this.Reset();
	}

	// Token: 0x06002560 RID: 9568 RVA: 0x0008F85A File Offset: 0x0008DA5A
	private void Reset()
	{
		this.timeToDie = this.timeDir.HoursFromNow(this.hoursToLive);
		SECTR_AudioSystem.Play(this.spawnCue, base.transform.position, false);
		this.numBounces = 0;
		this.defused = false;
	}

	// Token: 0x06002561 RID: 9569 RVA: 0x0008F899 File Offset: 0x0008DA99
	public void Update()
	{
		if (this.HasBouncedTooMuch() || this.timeDir.HasReached(this.timeToDie))
		{
			this.OnExpire();
			base.RequestDestroy("FireBall.Update");
		}
	}

	// Token: 0x06002562 RID: 9570 RVA: 0x0008F8C7 File Offset: 0x0008DAC7
	private bool HasBouncedTooMuch()
	{
		return this.bouncesToDie > 0 && this.numBounces >= this.bouncesToDie;
	}

	// Token: 0x06002563 RID: 9571 RVA: 0x0008F8E3 File Offset: 0x0008DAE3
	protected virtual void OnExpire()
	{
		if (this.expireFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.expireFX, base.transform.position, base.transform.rotation);
		}
	}

	// Token: 0x06002564 RID: 9572 RVA: 0x0008F918 File Offset: 0x0008DB18
	public void OnCollisionEnter(Collision col)
	{
		if (base.name.Contains("Fireball"))
		{
			Log.Warning("Bounced!", new object[]
			{
				"this.name",
				base.name,
				"col.name",
				col.gameObject.name
			});
		}
		this.numBounces++;
		Ignitable component = col.gameObject.GetComponent<Ignitable>();
		SECTR_AudioSystem.Play(this.bounceCue, base.transform.position, false);
		if (component != null)
		{
			component.Ignite(base.gameObject);
		}
	}

	// Token: 0x06002565 RID: 9573 RVA: 0x0008F9B0 File Offset: 0x0008DBB0
	public void Vaporize()
	{
		this.DefuseAndDestroy();
		SECTR_AudioSystem.Play(this.shatterCue, base.transform.position, false);
		if (this.vaporizeFx != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.vaporizeFx, base.gameObject.transform.position, Quaternion.identity);
		}
	}

	// Token: 0x06002566 RID: 9574 RVA: 0x0008FA0A File Offset: 0x0008DC0A
	public void DefuseAndDestroy()
	{
		this.defused = true;
		base.RequestDestroy("FireBall.DefuseAndDestroy");
	}

	// Token: 0x04002448 RID: 9288
	public int bouncesToDie = 3;

	// Token: 0x04002449 RID: 9289
	public float hoursToLive = 0.1f;

	// Token: 0x0400244A RID: 9290
	private int numBounces;

	// Token: 0x0400244B RID: 9291
	private double timeToDie;

	// Token: 0x0400244C RID: 9292
	protected bool defused;

	// Token: 0x0400244D RID: 9293
	public GameObject vaporizeFx;

	// Token: 0x0400244E RID: 9294
	public GameObject expireFX;

	// Token: 0x0400244F RID: 9295
	public SECTR_AudioCue spawnCue;

	// Token: 0x04002450 RID: 9296
	public SECTR_AudioCue bounceCue;

	// Token: 0x04002451 RID: 9297
	public SECTR_AudioCue shatterCue;

	// Token: 0x04002452 RID: 9298
	private TimeDirector timeDir;
}
