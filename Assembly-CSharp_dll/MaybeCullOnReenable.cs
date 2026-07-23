using System;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000409 RID: 1033
public class MaybeCullOnReenable : SRBehaviour, ActorModel.Participant
{
	// Token: 0x06001590 RID: 5520 RVA: 0x00054009 File Offset: 0x00052209
	public void Awake()
	{
		this.emotions = base.GetComponent<SlimeEmotions>();
		if (!base.enabled)
		{
			this.model.disabledAtTime = new double?(SRSingleton<SceneContext>.Instance.TimeDirector.HoursFromNowOrStart(0f));
		}
	}

	// Token: 0x06001591 RID: 5521 RVA: 0x00054043 File Offset: 0x00052243
	public void Start()
	{
		if (base.enabled)
		{
			this.DoCullCheck();
		}
	}

	// Token: 0x06001592 RID: 5522 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(ActorModel model)
	{
	}

	// Token: 0x06001593 RID: 5523 RVA: 0x00054053 File Offset: 0x00052253
	public void SetModel(ActorModel model)
	{
		this.model = (SlimeModel)model;
	}

	// Token: 0x06001594 RID: 5524 RVA: 0x00054061 File Offset: 0x00052261
	public void OnDisable()
	{
		if (SRSingleton<SceneContext>.Instance != null && this.model != null)
		{
			this.model.disabledAtTime = new double?(SRSingleton<SceneContext>.Instance.TimeDirector.HoursFromNowOrStart(0f));
		}
	}

	// Token: 0x06001595 RID: 5525 RVA: 0x0005409C File Offset: 0x0005229C
	public void OnEnable()
	{
		this.DoCullCheck();
	}

	// Token: 0x06001596 RID: 5526 RVA: 0x000540A4 File Offset: 0x000522A4
	private void DoCullCheck()
	{
		if (this.model != null)
		{
			if (this.model.disabledAtTime != null && !CellDirector.IsOnRanch(base.GetComponent<RegionMember>()))
			{
				double num = SRSingleton<SceneContext>.Instance.TimeDirector.WorldTime() - this.model.disabledAtTime.Value;
				this.MaybeDestroy((float)(num / 86400.0));
			}
			this.model.disabledAtTime = null;
		}
	}

	// Token: 0x06001597 RID: 5527 RVA: 0x00054120 File Offset: 0x00052320
	private void MaybeDestroy(float daysPassed)
	{
		float p = Mathf.Pow(this.DestroyProbabilityPerDay(), 1f / daysPassed);
		if (Randoms.SHARED.GetProbability(p))
		{
			Destroyer.DestroyActor(base.gameObject, "MaybeCullOnReenable.MaybeDestroy", false);
		}
	}

	// Token: 0x06001598 RID: 5528 RVA: 0x0005415E File Offset: 0x0005235E
	private float DestroyProbabilityPerDay()
	{
		return 0.5f + this.emotions.GetMax() * 0.4f;
	}

	// Token: 0x04001494 RID: 5268
	private SlimeEmotions emotions;

	// Token: 0x04001495 RID: 5269
	private SlimeModel model;
}
