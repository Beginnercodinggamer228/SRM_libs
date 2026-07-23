using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000405 RID: 1029
public class LuckySlimeProduceCoins : CollidableActorBehaviour, Collidable
{
	// Token: 0x0600157D RID: 5501 RVA: 0x0005397A File Offset: 0x00051B7A
	public override void Start()
	{
		base.Start();
		this.flee = base.GetComponent<LuckySlimeFlee>();
		this.slimeAudio = base.GetComponent<SlimeAudio>();
		this.sfAnimator = base.GetComponent<SlimeFaceAnimator>();
		this.slimeEat = base.GetComponent<SlimeEat>();
	}

	// Token: 0x0600157E RID: 5502 RVA: 0x000539B4 File Offset: 0x00051BB4
	public void ProcessCollisionEnter(Collision collision)
	{
		Identifiable component = collision.gameObject.GetComponent<Identifiable>();
		if (component != null && Identifiable.IsAnimal(component.id) && !this.colliders.Contains(collision.gameObject))
		{
			float num = float.NegativeInfinity;
			foreach (ContactPoint contactPoint in collision.contacts)
			{
				float num2 = Vector3.Dot(contactPoint.normal, collision.relativeVelocity);
				if (num2 > num)
				{
					num = num2;
				}
			}
			if (num > 0.02f)
			{
				this.ProduceCoins(collision.gameObject);
				if (this.flee != null)
				{
					this.flee.StartFleeing(SRSingleton<SceneContext>.Instance.Player);
				}
				this.colliders.Add(collision.gameObject);
				Identifiable component2 = base.GetComponent<Identifiable>();
				SRSingleton<SceneContext>.Instance.PediaDirector.MaybeShowPopup(component2.id);
			}
		}
	}

	// Token: 0x0600157F RID: 5503 RVA: 0x00003296 File Offset: 0x00001496
	public void ProcessCollisionExit(Collision col)
	{
	}

	// Token: 0x06001580 RID: 5504 RVA: 0x00053AA6 File Offset: 0x00051CA6
	private IEnumerator DropCoinsAndJumpDelayed()
	{
		yield return new WaitForSeconds(0.35f);
		base.GetComponent<Rigidbody>().AddForce(new Vector3(Randoms.SHARED.GetFloat(225f), 450f, Randoms.SHARED.GetFloat(225f)));
		this.slimeAudio.Play(this.slimeAudio.slimeSounds.jumpCue);
		this.slimeAudio.Play(this.slimeAudio.slimeSounds.voiceJumpCue);
		int num;
		for (int ii = 0; ii < this.coinPrefabsLastHit; ii = num)
		{
			SRBehaviour.InstantiateDynamic(this.coinsPrefab, base.transform.position, base.transform.rotation, false);
			yield return new WaitForSeconds(0.1f);
			num = ii + 1;
		}
		yield break;
	}

	// Token: 0x06001581 RID: 5505 RVA: 0x00053AB8 File Offset: 0x00051CB8
	private void ProduceCoins(GameObject triggerer)
	{
		if (this.slimeEat.MaybeSpinAndChomp(triggerer, true))
		{
			if (this.coinPrefabsLastHit == 0)
			{
				this.coinPrefabsLastHit = Randoms.SHARED.GetInRange(2, 3);
			}
			else
			{
				this.coinPrefabsLastHit = Math.Min(6, this.coinPrefabsLastHit * 2);
			}
			base.StartCoroutine(this.DropCoinsAndJumpDelayed());
			this.sfAnimator.SetTrigger("triggerWince");
			this.coinSetsProduced++;
		}
	}

	// Token: 0x04001473 RID: 5235
	public GameObject coinsPrefab;

	// Token: 0x04001474 RID: 5236
	private LuckySlimeFlee flee;

	// Token: 0x04001475 RID: 5237
	private HashSet<GameObject> colliders = new HashSet<GameObject>();

	// Token: 0x04001476 RID: 5238
	private SlimeAudio slimeAudio;

	// Token: 0x04001477 RID: 5239
	private SlimeFaceAnimator sfAnimator;

	// Token: 0x04001478 RID: 5240
	private SlimeEat slimeEat;

	// Token: 0x04001479 RID: 5241
	private int coinSetsProduced;

	// Token: 0x0400147A RID: 5242
	private int coinPrefabsLastHit;

	// Token: 0x0400147B RID: 5243
	private const float HIT_THRESHOLD = 0.02f;

	// Token: 0x0400147C RID: 5244
	private const float JUMP_ON_HIT_VERT_FORCE = 450f;

	// Token: 0x0400147D RID: 5245
	private const float JUMP_ON_HIT_MAX_HORIZ_FORCE = 225f;

	// Token: 0x0400147E RID: 5246
	private const int MIN_INIT_COIN_PREFABS = 2;

	// Token: 0x0400147F RID: 5247
	private const int MAX_INIT_COIN_PREFABS = 2;

	// Token: 0x04001480 RID: 5248
	private const int MAX_TOTAL_COIN_PREFABS = 6;

	// Token: 0x04001481 RID: 5249
	private const float DELAY_BETWEEN_COINS = 0.1f;

	// Token: 0x04001482 RID: 5250
	private const float ADDL_DELAY = 0.1f;
}
