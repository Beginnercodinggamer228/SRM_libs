using System;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x02000517 RID: 1303
public class SpawnerTrigger : MonoBehaviour, SpawnerTriggerModel.Participant
{
	// Token: 0x06001B2D RID: 6957 RVA: 0x0006866B File Offset: 0x0006686B
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		SRSingleton<SceneContext>.Instance.GameModel.RegisterSpawnerTrigger(this);
	}

	// Token: 0x06001B2E RID: 6958 RVA: 0x0006868D File Offset: 0x0006688D
	public void InitModel(SpawnerTriggerModel model)
	{
		model.pos = base.transform.position;
	}

	// Token: 0x06001B2F RID: 6959 RVA: 0x000686A0 File Offset: 0x000668A0
	public void SetModel(SpawnerTriggerModel model)
	{
		this.model = model;
	}

	// Token: 0x06001B30 RID: 6960 RVA: 0x000686AC File Offset: 0x000668AC
	public void OnTriggerEnter(Collider collider)
	{
		if (collider.isTrigger)
		{
			return;
		}
		if (this.timeDir.HasReached(this.model.nextTriggerTime))
		{
			Identifiable componentInParent = collider.gameObject.GetComponentInParent<Identifiable>();
			if (componentInParent != null && componentInParent.id == Identifiable.Id.PLAYER && this.spawner.CanSpawnSomething())
			{
				if (Randoms.SHARED.GetProbability(this.chanceOfTrigger))
				{
					float num = (this.spawner is DirectedSlimeSpawner) ? SRSingleton<SceneContext>.Instance.ModDirector.SlimeCountFactor() : 1f;
					base.StartCoroutine(this.spawner.Spawn(Mathf.RoundToInt((float)Randoms.SHARED.GetInRange(this.minSpawn, this.maxSpawn + 1) * num), Randoms.SHARED));
				}
				this.model.nextTriggerTime = this.timeDir.HoursFromNow(Randoms.SHARED.GetInRange(0.5f, 1.5f) * this.avgGameHoursBetweenTrigger);
			}
		}
	}

	// Token: 0x04001A95 RID: 6805
	public DirectedActorSpawner spawner;

	// Token: 0x04001A96 RID: 6806
	[Tooltip("Minimum number of items/slimes to spawn at a time.")]
	public int minSpawn = 3;

	// Token: 0x04001A97 RID: 6807
	[Tooltip("Maximum number of items/slimes to spawn at a time.")]
	public int maxSpawn = 5;

	// Token: 0x04001A98 RID: 6808
	[Tooltip("Average cooldown between triggers.")]
	public float avgGameHoursBetweenTrigger = 2f;

	// Token: 0x04001A99 RID: 6809
	[Tooltip("Chance the trigger will spawn slimes. Even if it doesn't, cooldown will reset.")]
	public float chanceOfTrigger = 1f;

	// Token: 0x04001A9A RID: 6810
	private TimeDirector timeDir;

	// Token: 0x04001A9B RID: 6811
	private SpawnerTriggerModel model;
}
