using System;
using UnityEngine;

// Token: 0x020006EA RID: 1770
public class DirectedSlimeSpawner : DirectedActorSpawner
{
	// Token: 0x060024E7 RID: 9447 RVA: 0x0008DEC1 File Offset: 0x0008C0C1
	public override void Awake()
	{
		base.Awake();
		this.modDir = SRSingleton<SceneContext>.Instance.ModDirector;
		this.lookupDir = SRSingleton<GameContext>.Instance.LookupDirector;
		this.oasis = base.GetComponentInParent<Oasis>();
	}

	// Token: 0x060024E8 RID: 9448 RVA: 0x0008DEF5 File Offset: 0x0008C0F5
	public override bool CanSpawn(float? forHour = null)
	{
		return base.CanSpawn(forHour) && !this.IsOasisFull();
	}

	// Token: 0x060024E9 RID: 9449 RVA: 0x0008DF0B File Offset: 0x0008C10B
	protected override void Register(CellDirector cellDir)
	{
		cellDir.Register(this);
	}

	// Token: 0x060024EA RID: 9450 RVA: 0x0008DF14 File Offset: 0x0008C114
	protected override GameObject MaybeReplacePrefab(GameObject prefab)
	{
		if (Randoms.SHARED.GetProbability(this.modDir.ChanceOfTarrSpawn()))
		{
			return this.lookupDir.GetPrefab(Identifiable.Id.TARR_SLIME);
		}
		return prefab;
	}

	// Token: 0x060024EB RID: 9451 RVA: 0x0008DF3C File Offset: 0x0008C13C
	protected override void SpawnFX(GameObject spawnedObj, Vector3 pos)
	{
		base.SpawnFX(spawnedObj, pos);
		SlimeAppearance.Palette appearancePalette = spawnedObj.GetComponent<SlimeAppearanceApplicator>().GetAppearancePalette();
		RecolorSlimeMaterial[] componentsInChildren = SRBehaviour.SpawnAndPlayFX(this.slimeSpawnFX, spawnedObj.transform.position, spawnedObj.transform.rotation).GetComponentsInChildren<RecolorSlimeMaterial>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SetColors(appearancePalette.Top, appearancePalette.Middle, appearancePalette.Bottom);
		}
	}

	// Token: 0x060024EC RID: 9452 RVA: 0x0008DFAC File Offset: 0x0008C1AC
	private bool IsOasisFull()
	{
		return this.oasis != null && !this.oasis.NeedsMoreSlimes();
	}

	// Token: 0x040023D2 RID: 9170
	[Tooltip("An extra slime effect to play along with each spawn.")]
	public GameObject slimeSpawnFX;

	// Token: 0x040023D3 RID: 9171
	protected ModDirector modDir;

	// Token: 0x040023D4 RID: 9172
	protected LookupDirector lookupDir;

	// Token: 0x040023D5 RID: 9173
	private Oasis oasis;
}
