using System;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x0200041D RID: 1053
public class PollenCloudController : SRBehaviour
{
	// Token: 0x060015F4 RID: 5620 RVA: 0x000552BC File Offset: 0x000534BC
	public void Awake()
	{
		this.emotions = base.GetComponent<SlimeEmotions>();
		this.regionMember = base.GetComponent<RegionMember>();
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.growthFactor = 1f / (1f - this.startGrowthAgitation);
		this.pctGrowthPerGameSec = this.pctGrowthPerGameHour * 0.00027777778f;
	}

	// Token: 0x060015F5 RID: 5621 RVA: 0x0005531B File Offset: 0x0005351B
	public void Start()
	{
		this.cloud = base.GetComponentInChildren<PollenCloudMarker>(true);
	}

	// Token: 0x060015F6 RID: 5622 RVA: 0x0005532C File Offset: 0x0005352C
	public void Update()
	{
		float num = this.ScaleForAgitation(this.emotions.GetCurr(SlimeEmotions.Emotion.AGITATION));
		float num2 = this.cloud.gameObject.activeSelf ? (this.cloud.transform.localScale.x / this.maxCloudScale) : 0f;
		if (num2 > num)
		{
			num2 = Mathf.Max(num, num2 - (float)(this.timeDir.DeltaWorldTime() * (double)this.pctGrowthPerGameSec));
		}
		else if (num2 < num)
		{
			num2 = Mathf.Min(num, num2 + (float)(this.timeDir.DeltaWorldTime() * (double)this.pctGrowthPerGameSec));
		}
		if (num2 >= 0.95f)
		{
			SRBehaviour.InstantiateActor(this.cloudActorPrefab, this.regionMember.setId, base.transform.position, base.transform.rotation, false).GetComponent<Rigidbody>().velocity = base.transform.forward * 1f;
			num2 = 0f;
		}
		this.cloud.transform.localScale = Vector3.one * (this.maxCloudScale * num2);
		this.cloud.gameObject.SetActive(num2 > 0f);
	}

	// Token: 0x060015F7 RID: 5623 RVA: 0x0005545A File Offset: 0x0005365A
	private float ScaleForAgitation(float agitation)
	{
		return Math.Max(0f, (agitation - this.startGrowthAgitation) * this.growthFactor);
	}

	// Token: 0x040014E5 RID: 5349
	public float pctGrowthPerGameHour = 1f;

	// Token: 0x040014E6 RID: 5350
	public float startGrowthAgitation = 0.75f;

	// Token: 0x040014E7 RID: 5351
	public float maxCloudScale = 5f;

	// Token: 0x040014E8 RID: 5352
	public GameObject cloudActorPrefab;

	// Token: 0x040014E9 RID: 5353
	private PollenCloudMarker cloud;

	// Token: 0x040014EA RID: 5354
	private SlimeEmotions emotions;

	// Token: 0x040014EB RID: 5355
	private RegionMember regionMember;

	// Token: 0x040014EC RID: 5356
	private TimeDirector timeDir;

	// Token: 0x040014ED RID: 5357
	private float growthFactor;

	// Token: 0x040014EE RID: 5358
	private float pctGrowthPerGameSec;

	// Token: 0x040014EF RID: 5359
	private const float CLOUD_SPEED = 1f;

	// Token: 0x040014F0 RID: 5360
	private const float RELEASE_CUTOFF = 0.95f;
}
