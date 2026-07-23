using System;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020004F3 RID: 1267
public class GlitchSlime : SRBehaviour, LiquidConsumer, ActorModel.Participant
{
	// Token: 0x06001A87 RID: 6791 RVA: 0x00066B78 File Offset: 0x00064D78
	public void Awake()
	{
		this.regionMember = base.GetComponent<RegionMember>();
		this.id = Identifiable.GetId(base.gameObject);
		base.enabled = false;
	}

	// Token: 0x06001A88 RID: 6792 RVA: 0x00066B9E File Offset: 0x00064D9E
	public void InitModel(ActorModel model)
	{
		((SlimeModel)model).isGlitch = false;
	}

	// Token: 0x06001A89 RID: 6793 RVA: 0x00066BAC File Offset: 0x00064DAC
	public void SetModel(ActorModel model)
	{
		this.model = (SlimeModel)model;
		base.enabled = this.model.isGlitch;
	}

	// Token: 0x06001A8A RID: 6794 RVA: 0x00066BCC File Offset: 0x00064DCC
	public void Start()
	{
		this.model.isGlitch = true;
		base.GetRequiredComponent<SlimeFaceAnimator>().SetGlitch();
		Vacuumable requiredComponent = base.GetRequiredComponent<Vacuumable>();
		requiredComponent.onSetHeld += delegate(bool b)
		{
			this.OnExposed(null);
		};
		requiredComponent.consume += delegate()
		{
			this.OnExposed(null);
		};
		SlimeHealth requiredComponent2 = base.GetRequiredComponent<SlimeHealth>();
		requiredComponent2.onDamage = (SlimeHealth.OnDamage)Delegate.Combine(requiredComponent2.onDamage, new SlimeHealth.OnDamage(delegate(GameObject s)
		{
			this.OnExposed(s);
		}));
	}

	// Token: 0x06001A8B RID: 6795 RVA: 0x00066C40 File Offset: 0x00064E40
	public void AddLiquid(Identifiable.Id id, float units)
	{
		if (base.enabled && id == Identifiable.Id.GLITCH_DEBUG_SPRAY_LIQUID)
		{
			this.OnExposed(null);
		}
	}

	// Token: 0x06001A8C RID: 6796 RVA: 0x00066C5C File Offset: 0x00064E5C
	private void OnExposed(GameObject source = null)
	{
		Destroyer.DestroyActor(base.gameObject, "GlitchSlime.OnExposed", false);
		SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch.GetDittoExposureMetadata(this.id).OnExposed(base.gameObject, null, null, null, source, null);
	}

	// Token: 0x04001A18 RID: 6680
	private SlimeModel model;

	// Token: 0x04001A19 RID: 6681
	private RegionMember regionMember;

	// Token: 0x04001A1A RID: 6682
	private Identifiable.Id id;
}
