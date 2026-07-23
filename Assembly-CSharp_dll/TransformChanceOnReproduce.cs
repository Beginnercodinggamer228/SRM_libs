using System;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020007A3 RID: 1955
public class TransformChanceOnReproduce : SRBehaviour
{
	// Token: 0x060028ED RID: 10477 RVA: 0x0009AB5B File Offset: 0x00098D5B
	public void Awake()
	{
		this.regionMember = base.GetComponent<RegionMember>();
	}

	// Token: 0x060028EE RID: 10478 RVA: 0x0009AB6C File Offset: 0x00098D6C
	public void MaybeTransform()
	{
		if (Randoms.SHARED.GetProbability(this.transformChance))
		{
			SRBehaviour.SpawnAndPlayFX(this.transformFX, base.transform.position, base.transform.rotation);
			Destroyer.DestroyActor(base.gameObject, "TransformChanceOnReproduce.MaybeTransform", false);
			SRBehaviour.InstantiateActor(this.targetPrefab, this.regionMember.setId, base.transform.position, base.transform.rotation, false);
		}
	}

	// Token: 0x04002862 RID: 10338
	[Tooltip("Probability we will transform on any given opportunity.")]
	public float transformChance = 0.05f;

	// Token: 0x04002863 RID: 10339
	[Tooltip("What do we transform into.")]
	public GameObject targetPrefab;

	// Token: 0x04002864 RID: 10340
	[Tooltip("Extra particle effect to play on transform.")]
	public GameObject transformFX;

	// Token: 0x04002865 RID: 10341
	private RegionMember regionMember;
}
