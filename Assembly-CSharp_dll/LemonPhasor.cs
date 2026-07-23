using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000403 RID: 1027
public class LemonPhasor : SRBehaviour
{
	// Token: 0x06001571 RID: 5489 RVA: 0x000535ED File Offset: 0x000517ED
	public void Awake()
	{
		this.region = base.GetComponentInParent<Region>();
		this.spawnResource = base.GetComponent<SpawnResource>();
	}

	// Token: 0x06001572 RID: 5490 RVA: 0x00053608 File Offset: 0x00051808
	public void OnTriggerEnter(Collider col)
	{
		if (col.isTrigger)
		{
			return;
		}
		Identifiable component = col.gameObject.GetComponent<Identifiable>();
		if (component == null || !Identifiable.FRUIT_CLASS.Contains(component.id) || component.id == Identifiable.Id.LEMON_FRUIT || this.handledFruit.Contains(col.gameObject))
		{
			return;
		}
		Joint joint = this.spawnResource.PickRipeResourceJoint();
		if (joint == null)
		{
			return;
		}
		this.handledFruit.Add(col.gameObject);
		((ProduceModel)SRSingleton<SceneContext>.Instance.GameModel.GetActorModel(component.GetActorId())).state = ResourceCycle.State.ROTTEN;
		Destroyer.DestroyActor(joint.connectedBody.gameObject, "LemonPhasor.OnTriggerEnter#1", false);
		GameObject gameObject = SRBehaviour.InstantiateActor(this.lemonPrefab, this.region.setId, joint.transform.position, joint.transform.rotation, false);
		ProduceModel produceModel = (ProduceModel)SRSingleton<SceneContext>.Instance.GameModel.GetActorModel(component.GetActorId());
		ResourceCycle component2 = gameObject.GetComponent<ResourceCycle>();
		produceModel.state = ResourceCycle.State.EDIBLE;
		component2.Eject(gameObject.GetComponent<Rigidbody>());
		if (this.spawnLemonFx != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.spawnLemonFx, joint.transform.position, joint.transform.rotation);
		}
		if (this.phaseoutFruitFx != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.phaseoutFruitFx, col.transform.position, col.transform.rotation);
		}
		Destroyer.DestroyActor(col.gameObject, "LemonPhasor.OnTriggerEnter#2", false);
	}

	// Token: 0x06001573 RID: 5491 RVA: 0x00053793 File Offset: 0x00051993
	public void LateUpdate()
	{
		this.handledFruit.Clear();
	}

	// Token: 0x04001466 RID: 5222
	private SpawnResource spawnResource;

	// Token: 0x04001467 RID: 5223
	private Region region;

	// Token: 0x04001468 RID: 5224
	public GameObject lemonPrefab;

	// Token: 0x04001469 RID: 5225
	public GameObject spawnLemonFx;

	// Token: 0x0400146A RID: 5226
	public GameObject phaseoutFruitFx;

	// Token: 0x0400146B RID: 5227
	private HashSet<GameObject> handledFruit = new HashSet<GameObject>();
}
