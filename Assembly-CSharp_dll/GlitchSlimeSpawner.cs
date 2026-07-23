using System;
using MonomiPark.SlimeRancher.Serializable.Optional;
using UnityEngine;

// Token: 0x020004F5 RID: 1269
public class GlitchSlimeSpawner : DirectedSlimeSpawner
{
	// Token: 0x06001A9B RID: 6811 RVA: 0x00066FA6 File Offset: 0x000651A6
	public override void Awake()
	{
		base.Awake();
		this.metadata = SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch;
	}

	// Token: 0x06001A9C RID: 6812 RVA: 0x00066FC4 File Offset: 0x000651C4
	protected override void OnActorSpawned(GameObject instance)
	{
		base.OnActorSpawned(instance);
		GlitchSlime component = instance.GetComponent<GlitchSlime>();
		if (component != null && Randoms.SHARED.GetProbability(this.metadata.GetDittoProbability(Identifiable.GetId(instance), this)))
		{
			component.enabled = true;
		}
	}

	// Token: 0x04001A1E RID: 6686
	[Tooltip("If enabled, overrides GlitchMetadata.dittoStandard.probability.")]
	public Float probablityStandard;

	// Token: 0x04001A1F RID: 6687
	[Tooltip("If enabled, overrides GlitchMetadata.dittoLargo.probability.")]
	public Float probablityLargo;

	// Token: 0x04001A20 RID: 6688
	private GlitchMetadata metadata;
}
