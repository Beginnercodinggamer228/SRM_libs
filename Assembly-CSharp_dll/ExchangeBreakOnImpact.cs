using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000301 RID: 769
public class ExchangeBreakOnImpact : SRBehaviour
{
	// Token: 0x06001072 RID: 4210 RVA: 0x00041ABF File Offset: 0x0003FCBF
	public void Awake()
	{
		this.body = base.GetComponent<Rigidbody>();
		this.exchangeDir = SRSingleton<SceneContext>.Instance.ExchangeDirector;
		this.lookupDir = SRSingleton<GameContext>.Instance.LookupDirector;
	}

	// Token: 0x06001073 RID: 4211 RVA: 0x00041AED File Offset: 0x0003FCED
	public void Start()
	{
		if (this.breakOpenOnStart)
		{
			this.BreakOpen();
		}
	}

	// Token: 0x06001074 RID: 4212 RVA: 0x00041B00 File Offset: 0x0003FD00
	public void OnCollisionEnter(Collision col)
	{
		if (!col.collider.isTrigger && !this.body.isKinematic)
		{
			float num = 0f;
			foreach (ContactPoint contactPoint in col.contacts)
			{
				num = Mathf.Max(num, Vector3.Dot(contactPoint.normal, col.relativeVelocity));
			}
			if (num > 0f)
			{
				this.BreakOpen();
			}
		}
	}

	// Token: 0x06001075 RID: 4213 RVA: 0x00041B74 File Offset: 0x0003FD74
	private void BreakOpen()
	{
		if (this.breaking)
		{
			return;
		}
		this.breaking = true;
		SRBehaviour.SpawnAndPlayFX(this.breakFX, base.gameObject.transform.position, base.gameObject.transform.rotation);
		Destroyer.DestroyActor(base.gameObject, "ExchangeBreakOnImpact.BreakOpen", false);
		List<ExchangeDirector.ItemEntry> offerRewards = this.exchangeDir.GetOfferRewards(ExchangeDirector.OfferType.GENERAL);
		RegionRegistry.RegionSetId setId = base.GetComponent<RegionMember>().setId;
		if (offerRewards != null)
		{
			foreach (ExchangeDirector.ItemEntry itemEntry in offerRewards)
			{
				if (itemEntry.specReward != ExchangeDirector.NonIdentReward.NONE)
				{
					this.SpawnSpecReward(itemEntry.specReward);
				}
				else
				{
					GameObject prefab = this.lookupDir.GetPrefab(itemEntry.id);
					for (int i = 0; i < itemEntry.count; i++)
					{
						Vector3 position = base.transform.position + UnityEngine.Random.insideUnitSphere * 1f;
						GameObject gameObject = SRBehaviour.InstantiateActor(prefab, setId, position, Quaternion.identity, false);
						gameObject.transform.DOScale(gameObject.transform.localScale, 0.2f).From(0.01f, true).SetEase(Ease.Linear);
					}
				}
			}
		}
		this.exchangeDir.RewardsDidSpawn(ExchangeDirector.OfferType.GENERAL);
	}

	// Token: 0x06001076 RID: 4214 RVA: 0x00041CDC File Offset: 0x0003FEDC
	private void SpawnSpecReward(ExchangeDirector.NonIdentReward reward)
	{
		if (reward - ExchangeDirector.NonIdentReward.NEWBUCKS_SMALL <= 4)
		{
			int num = ExchangeBreakOnImpact.GetNewbucksRewardValue(reward) / 50;
			for (int i = 0; i < num; i++)
			{
				Vector3 position = base.transform.position + UnityEngine.Random.insideUnitSphere;
				SRBehaviour.SpawnAndPlayFX(this.coinPrefab, position, Quaternion.identity);
			}
			return;
		}
		if (reward != ExchangeDirector.NonIdentReward.TIME_EXTENSION_12H)
		{
			return;
		}
		PlayerState playerState = SRSingleton<SceneContext>.Instance.PlayerState;
		double num2 = (double)((float)ExchangeBreakOnImpact.GetTimeExtensionRewardValue(reward) * 3600f);
		playerState.SetEndGameTime(playerState.GetEndGameTime().Value + num2);
		if (this.timeExtensionFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.timeExtensionFX, base.transform.position, Quaternion.identity);
		}
	}

	// Token: 0x06001077 RID: 4215 RVA: 0x00041D94 File Offset: 0x0003FF94
	public static int GetNewbucksRewardValue(ExchangeDirector.NonIdentReward reward)
	{
		switch (reward)
		{
		case ExchangeDirector.NonIdentReward.NEWBUCKS_SMALL:
			return 250;
		case ExchangeDirector.NonIdentReward.NEWBUCKS_MEDIUM:
			return 500;
		case ExchangeDirector.NonIdentReward.NEWBUCKS_LARGE:
			return 750;
		case ExchangeDirector.NonIdentReward.NEWBUCKS_HUGE:
			return 1000;
		case ExchangeDirector.NonIdentReward.NEWBUCKS_MOCHI:
			return 200;
		default:
			return 0;
		}
	}

	// Token: 0x06001078 RID: 4216 RVA: 0x00041DE2 File Offset: 0x0003FFE2
	public static int GetTimeExtensionRewardValue(ExchangeDirector.NonIdentReward reward)
	{
		if (reward == ExchangeDirector.NonIdentReward.TIME_EXTENSION_12H)
		{
			return 12;
		}
		return 0;
	}

	// Token: 0x04000F34 RID: 3892
	public GameObject breakFX;

	// Token: 0x04000F35 RID: 3893
	public GameObject coinPrefab;

	// Token: 0x04000F36 RID: 3894
	[Tooltip("Prefab spawned when a time extension is granted. (optional)")]
	public GameObject timeExtensionFX;

	// Token: 0x04000F37 RID: 3895
	[HideInInspector]
	public bool breakOpenOnStart = true;

	// Token: 0x04000F38 RID: 3896
	private const float COLLISION_THRESHOLD = 0f;

	// Token: 0x04000F39 RID: 3897
	private Rigidbody body;

	// Token: 0x04000F3A RID: 3898
	private ExchangeDirector exchangeDir;

	// Token: 0x04000F3B RID: 3899
	private LookupDirector lookupDir;

	// Token: 0x04000F3C RID: 3900
	private bool breaking;

	// Token: 0x04000F3D RID: 3901
	private const int COINS_PER_ITEM = 50;

	// Token: 0x04000F3E RID: 3902
	private const float BREAK_SPAWN_RADIUS = 1f;

	// Token: 0x04000F3F RID: 3903
	private const float PRODUCE_SCALE_UP_TIME = 0.2f;
}
