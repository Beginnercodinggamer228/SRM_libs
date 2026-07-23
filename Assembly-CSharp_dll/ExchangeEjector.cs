using System;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000314 RID: 788
public class ExchangeEjector : SRBehaviour, ExchangeDirector.Awarder
{
	// Token: 0x060010C1 RID: 4289 RVA: 0x00043337 File Offset: 0x00041537
	public void AwardIfType(ExchangeDirector.OfferType offerType)
	{
		if (this.offerType == offerType)
		{
			this.Eject();
		}
	}

	// Token: 0x060010C2 RID: 4290 RVA: 0x00043348 File Offset: 0x00041548
	private void Eject()
	{
		RegionRegistry.RegionSetId setId = base.GetComponentInParent<Region>().setId;
		GameObject gameObject = SRBehaviour.InstantiateActor(this.cratePrefab, setId, base.transform.position, base.transform.rotation, false);
		Rigidbody component = gameObject.GetComponent<Rigidbody>();
		component.isKinematic = false;
		component.AddForce(base.transform.forward * 100f);
		gameObject.GetComponent<ExchangeBreakOnImpact>().breakOpenOnStart = false;
		SRBehaviour.InstantiateDynamic(this.awardFX, this.awardAt.position, this.awardAt.rotation, false);
	}

	// Token: 0x04000FA6 RID: 4006
	public GameObject cratePrefab;

	// Token: 0x04000FA7 RID: 4007
	public ExchangeDirector.OfferType offerType;

	// Token: 0x04000FA8 RID: 4008
	public GameObject awardFX;

	// Token: 0x04000FA9 RID: 4009
	public Transform awardAt;

	// Token: 0x04000FAA RID: 4010
	private const float EJECT_FORCE = 100f;
}
