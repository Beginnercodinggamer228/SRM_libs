using System;
using System.Collections.Generic;
using Assets.Script.Util.Extensions;
using UnityEngine;

// Token: 0x020004D7 RID: 1239
public class GlitchCellDirector : CellDirector
{
	// Token: 0x060019E7 RID: 6631 RVA: 0x000651A2 File Offset: 0x000633A2
	public override void Awake()
	{
		base.Awake();
		this.metadata = SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch;
		this.tarrSpawners = new List<GlitchTarrNodeSpawner>();
	}

	// Token: 0x060019E8 RID: 6632 RVA: 0x000651CC File Offset: 0x000633CC
	protected override void UpdateToTime(double worldTime)
	{
		base.UpdateToTime(worldTime);
		if (TimeUtil.HasReached(worldTime, this.tarrNextSpawn))
		{
			if (this.tarrSpawners.Count > 0 && this.NeedsMoreTarrs())
			{
				GlitchTarrNodeSpawner glitchTarrNodeSpawner = this.rand.Pick<GlitchTarrNodeSpawner>(this.tarrSpawners, delegate(GlitchTarrNodeSpawner it)
				{
					if (!it.CanSpawn(null))
					{
						return 0f;
					}
					return it.directedSpawnWeight;
				}, null);
				if (glitchTarrNodeSpawner != null)
				{
					base.StartCoroutine(glitchTarrNodeSpawner.Spawn(Mathf.RoundToInt(this.tarrSpawnCount.GetRandom(this.rand)), this.rand));
				}
			}
			this.tarrNextSpawn = worldTime + (double)(this.metadata.tarrSpawnerThrottleTime * 60f);
		}
	}

	// Token: 0x060019E9 RID: 6633 RVA: 0x00065285 File Offset: 0x00063485
	public override void ForceCheckSpawn()
	{
		base.ForceCheckSpawn();
		this.tarrNextSpawn = 0.0;
	}

	// Token: 0x060019EA RID: 6634 RVA: 0x0006529C File Offset: 0x0006349C
	public void Register(GlitchTarrNodeSpawner spawner)
	{
		this.tarrSpawners.Add(spawner);
	}

	// Token: 0x060019EB RID: 6635 RVA: 0x00013CC5 File Offset: 0x00011EC5
	protected override bool CanSpawnSlimes()
	{
		return true;
	}

	// Token: 0x060019EC RID: 6636 RVA: 0x000652AA File Offset: 0x000634AA
	private bool NeedsMoreTarrs()
	{
		return this.tarrSlimeCount < this.targetTarrCount;
	}

	// Token: 0x04001983 RID: 6531
	[Header("Tarr Properties")]
	[Tooltip("Target number of tarr slimes to be in the cell.")]
	public int targetTarrCount;

	// Token: 0x04001984 RID: 6532
	[Tooltip("Number of tarr slime to spawn per spawn. (random range)")]
	public Vector2 tarrSpawnCount;

	// Token: 0x04001985 RID: 6533
	[Tooltip("Tarr activation major group.")]
	public GlitchTarrNode.Group tarrActivationGroup;

	// Token: 0x04001986 RID: 6534
	private GlitchMetadata metadata;

	// Token: 0x04001987 RID: 6535
	private List<GlitchTarrNodeSpawner> tarrSpawners;

	// Token: 0x04001988 RID: 6536
	private double tarrNextSpawn;
}
