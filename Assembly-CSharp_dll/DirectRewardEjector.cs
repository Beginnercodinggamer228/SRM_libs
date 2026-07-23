using System;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020002FF RID: 767
public class DirectRewardEjector : SRBehaviour, ExchangeDirector.Awarder
{
	// Token: 0x06001068 RID: 4200 RVA: 0x00041843 File Offset: 0x0003FA43
	public void Start()
	{
		this.rewardIsActor = (this.rewardPrefab.GetComponent<Identifiable>() != null);
	}

	// Token: 0x06001069 RID: 4201 RVA: 0x0004185C File Offset: 0x0003FA5C
	public void AwardIfType(ExchangeDirector.OfferType offerType)
	{
		if (this.offerType == offerType)
		{
			this.Eject();
		}
	}

	// Token: 0x0600106A RID: 4202 RVA: 0x00041870 File Offset: 0x0003FA70
	public void Eject()
	{
		RegionRegistry.RegionSetId setId = base.GetComponentInParent<Region>().setId;
		for (int i = 0; i < this.rewardCount; i++)
		{
			Rigidbody component = (this.rewardIsActor ? SRBehaviour.InstantiateActor(this.rewardPrefab, setId, base.transform.position, base.transform.rotation, false) : SRBehaviour.InstantiateDynamic(this.rewardPrefab, base.transform.position, base.transform.rotation, false)).GetComponent<Rigidbody>();
			if (component != null)
			{
				component.isKinematic = false;
				component.AddForce(base.transform.forward * 60f);
			}
		}
		SRSingleton<SceneContext>.Instance.ExchangeDirector.RewardsDidSpawn(this.offerType);
		SECTR_AudioSystem.Play(this.onEjectCue, base.transform.position, false);
	}

	// Token: 0x04000F27 RID: 3879
	public GameObject rewardPrefab;

	// Token: 0x04000F28 RID: 3880
	public int rewardCount = 1;

	// Token: 0x04000F29 RID: 3881
	public ExchangeDirector.OfferType offerType;

	// Token: 0x04000F2A RID: 3882
	[Tooltip("SFX played when the reward is ejected. (optional)")]
	public SECTR_AudioCue onEjectCue;

	// Token: 0x04000F2B RID: 3883
	private bool rewardIsActor;

	// Token: 0x04000F2C RID: 3884
	private const float EJECT_FORCE = 60f;
}
