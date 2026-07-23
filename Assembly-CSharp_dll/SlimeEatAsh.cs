using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000470 RID: 1136
public class SlimeEatAsh : SRBehaviour
{
	// Token: 0x06001777 RID: 6007 RVA: 0x0005B211 File Offset: 0x00059411
	public void Awake()
	{
		this.emotions = base.GetComponent<SlimeEmotions>();
		this.slimeEat = base.GetComponent<SlimeEat>();
		this.slimeAudio = base.GetComponent<SlimeAudio>();
		this.regionMember = base.GetComponent<RegionMember>();
		this.ResetEatClock();
	}

	// Token: 0x06001778 RID: 6008 RVA: 0x0005B24C File Offset: 0x0005944C
	public void Update()
	{
		if (this.ashes.Count > 0 && Time.time >= this.nextChompTime && this.emotions.GetCurr(SlimeEmotions.Emotion.HUNGER) > this.slimeEat.minDriveToEat)
		{
			AshSource ashSource = Randoms.SHARED.Pick<AshSource>(this.ashes, null);
			if (ashSource.Available())
			{
				ashSource.ConsumeAsh();
				base.StartCoroutine(this.ProduceAfterDelay(1, this.plort, 2f));
				this.OnEat(SlimeEmotions.Emotion.HUNGER, Identifiable.Id.NONE);
			}
		}
	}

	// Token: 0x06001779 RID: 6009 RVA: 0x0005B2D0 File Offset: 0x000594D0
	public void OnCollisionEnter(Collision col)
	{
		AshSource component = col.gameObject.GetComponent<AshSource>();
		if (component != null)
		{
			this.ashes.Add(component);
		}
	}

	// Token: 0x0600177A RID: 6010 RVA: 0x0005B300 File Offset: 0x00059500
	public void OnCollisionExit(Collision col)
	{
		AshSource component = col.gameObject.GetComponent<AshSource>();
		if (component != null)
		{
			this.ashes.Remove(component);
		}
	}

	// Token: 0x0600177B RID: 6011 RVA: 0x0005B32F File Offset: 0x0005952F
	public void ResetEatClock()
	{
		this.nextChompTime = Time.time + this.eatRate;
	}

	// Token: 0x0600177C RID: 6012 RVA: 0x0005B343 File Offset: 0x00059543
	private void OnEat(SlimeEmotions.Emotion driver, Identifiable.Id otherId)
	{
		this.ResetEatClock();
		this.emotions.Adjust(driver, -this.slimeEat.drivePerEat);
		if (otherId != Identifiable.Id.PLAYER)
		{
			this.emotions.Adjust(SlimeEmotions.Emotion.AGITATION, -this.slimeEat.agitationPerEat);
		}
	}

	// Token: 0x0600177D RID: 6013 RVA: 0x0005B382 File Offset: 0x00059582
	private IEnumerator ProduceAfterDelay(int count, GameObject produces, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (base.gameObject != null)
		{
			for (int i = 0; i < count; i++)
			{
				Vector3 position = base.transform.TransformPoint(SlimeEatAsh.LOCAL_PRODUCE_LOC);
				Vector3 velocity = base.transform.TransformVector(SlimeEatAsh.LOCAL_PRODUCE_VEL);
				if (this.produceFX != null)
				{
					SRBehaviour.SpawnAndPlayFX(this.produceFX, position, base.transform.rotation);
				}
				GameObject gameObject = SRBehaviour.InstantiateActor(produces, this.regionMember.setId, position, base.transform.rotation, false);
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.velocity = velocity;
				}
				PlortInvulnerability component2 = gameObject.GetComponent<PlortInvulnerability>();
				if (component2 != null)
				{
					component2.GoInvulnerable();
				}
				gameObject.transform.DOScale(gameObject.transform.localScale, 0.5f).From(0.001f, true);
			}
			this.slimeAudio.Play(this.slimeAudio.slimeSounds.plortCue);
		}
		yield break;
	}

	// Token: 0x040016A3 RID: 5795
	[Tooltip("The plort to produce when we eat.")]
	public GameObject plort;

	// Token: 0x040016A4 RID: 5796
	public GameObject produceFX;

	// Token: 0x040016A5 RID: 5797
	public float eatRate = 3f;

	// Token: 0x040016A6 RID: 5798
	private SlimeEmotions emotions;

	// Token: 0x040016A7 RID: 5799
	private SlimeEat slimeEat;

	// Token: 0x040016A8 RID: 5800
	private RegionMember regionMember;

	// Token: 0x040016A9 RID: 5801
	private float nextChompTime;

	// Token: 0x040016AA RID: 5802
	private HashSet<AshSource> ashes = new HashSet<AshSource>();

	// Token: 0x040016AB RID: 5803
	private SlimeAudio slimeAudio;

	// Token: 0x040016AC RID: 5804
	private static readonly Vector3 LOCAL_PRODUCE_LOC = new Vector3(0f, 0.5f, 0f);

	// Token: 0x040016AD RID: 5805
	private static readonly Vector3 LOCAL_PRODUCE_VEL = new Vector3(0f, 1f, 0f);

	// Token: 0x040016AE RID: 5806
	private const float PRODUCE_SCALE_UP_TIME = 0.5f;
}
