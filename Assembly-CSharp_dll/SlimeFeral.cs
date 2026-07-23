using System;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000488 RID: 1160
public class SlimeFeral : RegisteredActorBehaviour, RegistryUpdateable, ActorModel.Participant
{
	// Token: 0x06001811 RID: 6161 RVA: 0x0005D494 File Offset: 0x0005B694
	public void Awake()
	{
		this.emotions = base.GetComponent<SlimeEmotions>();
		this.member = base.GetComponent<RegionMember>();
		SlimeEat component = base.GetComponent<SlimeEat>();
		component.onEat = (SlimeEat.OnEatDelegate)Delegate.Combine(component.onEat, new SlimeEat.OnEatDelegate(this.DidEat));
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		if (base.GetComponent<Vacuumable>().size == Vacuumable.Size.NORMAL)
		{
			Destroyer.Destroy(this, "SlimeFeral.Awake");
		}
	}

	// Token: 0x06001812 RID: 6162 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(ActorModel model)
	{
	}

	// Token: 0x06001813 RID: 6163 RVA: 0x0005D508 File Offset: 0x0005B708
	public void SetModel(ActorModel model)
	{
		this.model = (SlimeModel)model;
		if (this.model.isFeral)
		{
			this.MakeFeral();
			return;
		}
		this.MakeNotFeral(false);
	}

	// Token: 0x06001814 RID: 6164 RVA: 0x0005D534 File Offset: 0x0005B734
	public void RegistryUpdate()
	{
		if (this.dynamicToFeral && !this.model.isFeral && this.emotions.GetCurr(SlimeEmotions.Emotion.AGITATION) >= 0.999f)
		{
			this.SetFeral();
		}
		if (this.timeDir.HasReached(this.expireAt))
		{
			if (CellDirector.IsOnRanch(this.member) || CellDirector.IsInWilds(this.member))
			{
				this.expireAt += 3600.0;
				return;
			}
			if (this.destroyFX != null)
			{
				SRBehaviour.SpawnAndPlayFX(this.destroyFX, base.transform.position, Quaternion.identity);
			}
			Destroyer.DestroyActor(base.gameObject, "SlimeFeral.RegistryUpdate", false);
		}
	}

	// Token: 0x06001815 RID: 6165 RVA: 0x0005D5EE File Offset: 0x0005B7EE
	public void DidEat(Identifiable.Id id)
	{
		if (this.dynamicFromFeral && this.model.isFeral && id != Identifiable.Id.PLAYER)
		{
			this.ClearFeral(false);
		}
	}

	// Token: 0x06001816 RID: 6166 RVA: 0x0005D611 File Offset: 0x0005B811
	public void SetFeral()
	{
		if (this.model.isFeral)
		{
			return;
		}
		this.MakeFeral();
	}

	// Token: 0x06001817 RID: 6167 RVA: 0x0005D628 File Offset: 0x0005B828
	private void MakeFeral()
	{
		AttackPlayer component = base.GetComponent<AttackPlayer>();
		if (component != null)
		{
			component.shouldAttackPlayer = true;
		}
		GotoPlayer component2 = base.GetComponent<GotoPlayer>();
		if (component2 != null)
		{
			component2.shouldGotoPlayer = true;
		}
		FindConsumable[] components = base.GetComponents<FindConsumable>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].UpdateSearchIds();
		}
		this.aura = UnityEngine.Object.Instantiate<GameObject>(this.auraPrefab);
		this.aura.transform.SetParent(base.transform, false);
		base.GetComponent<SlimeFaceAnimator>().SetFeral();
		this.model.isFeral = true;
		this.expireAt = this.timeDir.HoursFromNowOrStart(this.feralLifetimeHours);
	}

	// Token: 0x06001818 RID: 6168 RVA: 0x0005D6D6 File Offset: 0x0005B8D6
	public void ClearFeral(bool deagitate = false)
	{
		if (!this.model.isFeral)
		{
			return;
		}
		this.MakeNotFeral(deagitate);
	}

	// Token: 0x06001819 RID: 6169 RVA: 0x0005D6F0 File Offset: 0x0005B8F0
	private void MakeNotFeral(bool deagitate)
	{
		SlimeAudio component = base.GetComponent<SlimeAudio>();
		if (component != null)
		{
			component.Play(component.slimeSounds.unferalCue);
		}
		AttackPlayer component2 = base.GetComponent<AttackPlayer>();
		if (component2 != null)
		{
			component2.shouldAttackPlayer = false;
		}
		GotoPlayer component3 = base.GetComponent<GotoPlayer>();
		if (component3 != null)
		{
			component3.shouldGotoPlayer = false;
		}
		FindConsumable[] components = base.GetComponents<FindConsumable>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].UpdateSearchIds();
		}
		Destroyer.Destroy(this.aura, "SlimeFeral.ClearFeral");
		base.GetComponent<SlimeFaceAnimator>().ClearFeral();
		this.model.isFeral = false;
		if (deagitate)
		{
			this.emotions.Adjust(SlimeEmotions.Emotion.AGITATION, -0.5f);
		}
		this.expireAt = double.PositiveInfinity;
	}

	// Token: 0x0600181A RID: 6170 RVA: 0x0005D7B9 File Offset: 0x0005B9B9
	public bool IsFeral()
	{
		return this.model.isFeral;
	}

	// Token: 0x04001779 RID: 6009
	[Tooltip("The aura we use to indicate when a slime has gone feral.")]
	public GameObject auraPrefab;

	// Token: 0x0400177A RID: 6010
	[Tooltip("Whether the feralness of the slime can be made true on the fly")]
	public bool dynamicToFeral;

	// Token: 0x0400177B RID: 6011
	[Tooltip("Whether the feralness of the slime can be made false on the fly")]
	public bool dynamicFromFeral = true;

	// Token: 0x0400177C RID: 6012
	[Tooltip("Hours after which a feral should poof.")]
	public float feralLifetimeHours = 3f;

	// Token: 0x0400177D RID: 6013
	[Tooltip("The FX to play when ferality causes us to poof.")]
	public GameObject destroyFX;

	// Token: 0x0400177E RID: 6014
	private SlimeEmotions emotions;

	// Token: 0x0400177F RID: 6015
	private RegionMember member;

	// Token: 0x04001780 RID: 6016
	private GameObject aura;

	// Token: 0x04001781 RID: 6017
	private TimeDirector timeDir;

	// Token: 0x04001782 RID: 6018
	private SlimeModel model;

	// Token: 0x04001783 RID: 6019
	private double expireAt = double.PositiveInfinity;

	// Token: 0x04001784 RID: 6020
	private const float AGITATION_FERAL_TRIGGER = 0.999f;

	// Token: 0x04001785 RID: 6021
	private const float DEFERAL_AGITATION_ADJUST = -0.5f;
}
