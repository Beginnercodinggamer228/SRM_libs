using System;
using Assets.Script.Util.Extensions;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x020004F4 RID: 1268
public class GlitchSlimeFlee : SlimeFlee, ActorModel.Participant
{
	// Token: 0x06001A91 RID: 6801 RVA: 0x00066CC8 File Offset: 0x00064EC8
	public override void Awake()
	{
		base.Awake();
		this.metadata = SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch;
		this.plexer.activationDelayOverride = new float?(this.metadata.slimeFleeDelay);
		this.vacuumable.onSetLaunched += this.OnSetLaunched;
		base.SetFleeDirection(Quaternion.Euler(0f, (float)Randoms.SHARED.GetInRange(0, 360), 0f) * Vector3.one);
	}

	// Token: 0x06001A92 RID: 6802 RVA: 0x00066D52 File Offset: 0x00064F52
	public void InitModel(ActorModel model)
	{
		GlitchSlimeModel glitchSlimeModel = (GlitchSlimeModel)model;
		glitchSlimeModel.deathTime = this.timeDir.HoursFromNow(this.metadata.slimeLifetime.GetRandom() * 0.016666668f);
		glitchSlimeModel.exposureChance = this.metadata.slimeBaseExposureChance;
	}

	// Token: 0x06001A93 RID: 6803 RVA: 0x00066D91 File Offset: 0x00064F91
	public void SetModel(ActorModel model)
	{
		this.model = (GlitchSlimeModel)model;
	}

	// Token: 0x06001A94 RID: 6804 RVA: 0x00066D9F File Offset: 0x00064F9F
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.vacuumable.onSetLaunched -= this.OnSetLaunched;
	}

	// Token: 0x06001A95 RID: 6805 RVA: 0x00066DC0 File Offset: 0x00064FC0
	public override void Action()
	{
		if (this.timeDir.HasReached(this.model.deathTime))
		{
			if (Randoms.SHARED.GetProbability(this.model.exposureChance))
			{
				this.metadata.slimeExposure.OnExposed(base.gameObject, null, null, null, null, delegate(GameObject instance)
				{
					instance.GetRequiredComponent<GlitchSlimeFlee>().model.exposureChance = this.model.exposureChance * (1f - this.metadata.slimeExposureChanceDegradation);
				});
			}
			SRBehaviour.SpawnAndPlayFX(this.disappearFX, base.transform.position, base.transform.rotation);
			Destroyer.DestroyActor(base.gameObject, "GlitchSlimeFlee.Action", false);
			return;
		}
		if (this.plexer.IsBlocked(null, base.fleeDir.Value, 0, this.requiresForceBlockCheck))
		{
			base.SetFleeDirection(Quaternion.Euler(0f, (float)Randoms.SHARED.GetInRange(90, 270), 0f) * base.fleeDir.Value);
			this.requiresForceBlockCheck = true;
		}
		else
		{
			this.requiresForceBlockCheck = false;
		}
		base.MoveTowards(base.fleeDir.Value);
	}

	// Token: 0x06001A96 RID: 6806 RVA: 0x00066EEC File Offset: 0x000650EC
	public void OnDrawGizmosSelected()
	{
		if (base.fleeDir != null)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(base.transform.position, base.transform.position + base.fleeDir.Value * 1.5f);
		}
	}

	// Token: 0x06001A97 RID: 6807 RVA: 0x00066F4B File Offset: 0x0006514B
	public void DisableExposureChance()
	{
		this.model.exposureChance = 0f;
	}

	// Token: 0x06001A98 RID: 6808 RVA: 0x00066F5D File Offset: 0x0006515D
	private void OnSetLaunched(bool launched)
	{
		if (launched)
		{
			this.DisableExposureChance();
		}
	}

	// Token: 0x04001A1B RID: 6683
	private GlitchSlimeModel model;

	// Token: 0x04001A1C RID: 6684
	private GlitchMetadata metadata;

	// Token: 0x04001A1D RID: 6685
	private bool requiresForceBlockCheck = true;
}
