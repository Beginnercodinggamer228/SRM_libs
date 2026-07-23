using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020007A1 RID: 1953
public class TransformAfterTime : SRBehaviour, ActorModel.Participant
{
	// Token: 0x060028E5 RID: 10469 RVA: 0x0009A97A File Offset: 0x00098B7A
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.regionMember = base.GetComponent<RegionMember>();
		this.lastWorldTime = this.timeDir.HoursFromNowOrStart(0f);
	}

	// Token: 0x060028E6 RID: 10470 RVA: 0x0009A9AE File Offset: 0x00098BAE
	public void InitModel(ActorModel model)
	{
		((AnimalModel)model).transformTime = this.timeDir.HoursFromNowOrStart(this.delayGameHours);
	}

	// Token: 0x060028E7 RID: 10471 RVA: 0x0009A9CC File Offset: 0x00098BCC
	public void SetModel(ActorModel model)
	{
		this.model = (AnimalModel)model;
	}

	// Token: 0x060028E8 RID: 10472 RVA: 0x0009A9DC File Offset: 0x00098BDC
	public void Update()
	{
		if (this.feeders.Count > 0)
		{
			this.model.transformTime -= (this.timeDir.WorldTime() - this.lastWorldTime) * 1.0;
		}
		if (this.timeDir.HasReached(this.model.transformTime) && this.options.Count > 0)
		{
			Dictionary<GameObject, float> dictionary = new Dictionary<GameObject, float>();
			foreach (TransformAfterTime.TransformOpt transformOpt in this.options)
			{
				dictionary[transformOpt.targetPrefab] = transformOpt.weight;
			}
			SRBehaviour.SpawnAndPlayFX(this.transformFX, base.transform.position, base.transform.rotation);
			Destroyer.DestroyActor(base.gameObject, "TransformAfterTime.Update", false);
			SRBehaviour.InstantiateActor(Randoms.SHARED.Pick<GameObject>(dictionary, null), this.regionMember.setId, base.transform.position, base.transform.rotation, false);
		}
		this.lastWorldTime = this.timeDir.WorldTime();
	}

	// Token: 0x060028E9 RID: 10473 RVA: 0x0009AB20 File Offset: 0x00098D20
	public void AddFeeder(FeederRegion feeder)
	{
		this.feeders.Add(feeder);
	}

	// Token: 0x060028EA RID: 10474 RVA: 0x0009AB2E File Offset: 0x00098D2E
	public void RemoveFeeder(FeederRegion feeder)
	{
		this.feeders.Remove(feeder);
	}

	// Token: 0x04002858 RID: 10328
	public float delayGameHours = 6f;

	// Token: 0x04002859 RID: 10329
	public GameObject transformFX;

	// Token: 0x0400285A RID: 10330
	public List<TransformAfterTime.TransformOpt> options;

	// Token: 0x0400285B RID: 10331
	private TimeDirector timeDir;

	// Token: 0x0400285C RID: 10332
	private List<FeederRegion> feeders = new List<FeederRegion>();

	// Token: 0x0400285D RID: 10333
	private double lastWorldTime;

	// Token: 0x0400285E RID: 10334
	private AnimalModel model;

	// Token: 0x0400285F RID: 10335
	private RegionMember regionMember;

	// Token: 0x020007A2 RID: 1954
	[Serializable]
	public class TransformOpt
	{
		// Token: 0x04002860 RID: 10336
		public GameObject targetPrefab;

		// Token: 0x04002861 RID: 10337
		public float weight;
	}
}
