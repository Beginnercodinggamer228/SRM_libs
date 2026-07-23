using System;
using Assets.Script.Util.Extensions;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020004F6 RID: 1270
public class GlitchTarrSterilizeOnWater : TarrSterilizeOnWater, DestroyAfterTimeListener
{
	// Token: 0x06001A9E RID: 6814 RVA: 0x00067018 File Offset: 0x00065218
	public override void Awake()
	{
		base.Awake();
		this.regionMember = base.GetComponent<RegionMember>();
		GlitchMetadata glitch = SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch;
		this.multiplyChance = glitch.tarrBaseMultiplyChance;
	}

	// Token: 0x06001A9F RID: 6815 RVA: 0x00067054 File Offset: 0x00065254
	public override void Start()
	{
		base.Start();
		GlitchMetadata glitch = SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch;
		this.destroyer.SetDeathTime(this.timeDir.HoursFromNow(glitch.tarrLifetime.GetRandom() * 0.016666668f));
	}

	// Token: 0x06001AA0 RID: 6816 RVA: 0x000670A0 File Offset: 0x000652A0
	public void WillDestroyAfterTime()
	{
		if (!this.sterilized && Randoms.SHARED.GetProbability(this.multiplyChance))
		{
			LookupDirector lookupDirector = SRSingleton<GameContext>.Instance.LookupDirector;
			GlitchMetadata glitch = SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch;
			GameObject prefab = lookupDirector.GetPrefab(Identifiable.Id.GLITCH_TARR_SLIME);
			for (int i = 0; i < Mathf.RoundToInt(glitch.tarrMultiplyCount.GetRandom()); i++)
			{
				GameObject gameObject = SRBehaviour.InstantiateActor(prefab, this.regionMember.setId, base.gameObject.transform.position + UnityEngine.Random.insideUnitSphere * 2f, Quaternion.identity, false);
				gameObject.GetComponent<GlitchTarrSterilizeOnWater>().multiplyChance = this.multiplyChance * (1f - glitch.tarrMultiplyChanceDegradation);
				gameObject.GetComponent<Rigidbody>().velocity = (Quaternion.Euler(new Vector2(-45f, 30f).GetRandom(), new Vector2(0f, 360f).GetRandom(), 0f) * gameObject.transform.forward).normalized * 15f;
				float fromValue = gameObject.transform.localScale.x * 0.2f;
				gameObject.transform.DOScale(gameObject.transform.localScale, 0.2f).From(fromValue, true).SetEase(Ease.Linear);
			}
		}
	}

	// Token: 0x04001A21 RID: 6689
	private RegionMember regionMember;

	// Token: 0x04001A22 RID: 6690
	private float multiplyChance;
}
