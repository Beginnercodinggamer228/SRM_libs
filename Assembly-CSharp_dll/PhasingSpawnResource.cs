using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000416 RID: 1046
public class PhasingSpawnResource : PhaseableObject
{
	// Token: 0x060015DB RID: 5595 RVA: 0x00054D3C File Offset: 0x00052F3C
	public void Awake()
	{
		this.spawnResource = base.GetComponent<SpawnResource>();
		SpawnResource spawnResource = this.spawnResource;
		spawnResource.onReachedSpawnTime = (UnityAction)Delegate.Combine(spawnResource.onReachedSpawnTime, new UnityAction(delegate()
		{
			this.readyToPhase = true;
		}));
	}

	// Token: 0x060015DC RID: 5596 RVA: 0x00054D71 File Offset: 0x00052F71
	public override void PhaseIn()
	{
		this.spawnResource.RefreshSpawnJointObjectPositions();
		if (base.gameObject.activeInHierarchy)
		{
			SRBehaviour.SpawnAndPlayFX(this.phaseInFx, base.transform.position, base.transform.rotation);
		}
	}

	// Token: 0x060015DD RID: 5597 RVA: 0x00054DAD File Offset: 0x00052FAD
	public override void PhaseOut()
	{
		if (base.gameObject.activeInHierarchy)
		{
			SRBehaviour.SpawnAndPlayFX(this.phaseOutFx, base.transform.position, base.transform.rotation);
		}
		this.readyToPhase = false;
	}

	// Token: 0x060015DE RID: 5598 RVA: 0x00054DE5 File Offset: 0x00052FE5
	public override bool ReadyToPhase()
	{
		return this.readyToPhase;
	}

	// Token: 0x040014C8 RID: 5320
	private SpawnResource spawnResource;

	// Token: 0x040014C9 RID: 5321
	private bool readyToPhase;

	// Token: 0x040014CA RID: 5322
	public GameObject phaseOutFx;

	// Token: 0x040014CB RID: 5323
	public GameObject phaseInFx;
}
