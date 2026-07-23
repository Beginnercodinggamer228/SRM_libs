using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x020006EE RID: 1774
public class EchoNet : SRBehaviour, GadgetModel.Participant
{
	// Token: 0x06002500 RID: 9472 RVA: 0x0008E21F File Offset: 0x0008C41F
	public void Awake()
	{
		this.lookupDir = SRSingleton<GameContext>.Instance.LookupDirector;
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
	}

	// Token: 0x06002501 RID: 9473 RVA: 0x0008E241 File Offset: 0x0008C441
	public void Start()
	{
		this.zoneDir = base.GetComponentInParent<ZoneDirector>();
		this.cellDir = base.GetComponentInParent<CellDirector>();
		this.MaybeSpawnEchoes();
	}

	// Token: 0x06002502 RID: 9474 RVA: 0x0008E261 File Offset: 0x0008C461
	public void InitModel(GadgetModel model)
	{
		this.ResetSpawnTime((EchoNetModel)model);
	}

	// Token: 0x06002503 RID: 9475 RVA: 0x0008E26F File Offset: 0x0008C46F
	public void SetModel(GadgetModel model)
	{
		this.model = (EchoNetModel)model;
		if (this.zoneDir != null)
		{
			this.MaybeSpawnEchoes();
		}
	}

	// Token: 0x06002504 RID: 9476 RVA: 0x0008E291 File Offset: 0x0008C491
	public void OnEnable()
	{
		if (this.zoneDir == null)
		{
			return;
		}
		this.MaybeSpawnEchoes();
	}

	// Token: 0x06002505 RID: 9477 RVA: 0x0008E2A8 File Offset: 0x0008C4A8
	private void MaybeSpawnEchoes()
	{
		bool flag = this.zoneDir.GetAllAuxItems().Count > 0;
		this.activeVersion.SetActive(flag);
		this.inactiveVersion.SetActive(!flag);
		double num = this.timeDir.WorldTime() - this.model.lastSpawnTime;
		this.ResetSpawnTime(this.model);
		float num2 = (float)((int)Math.Round((double)Randoms.SHARED.GetInRange(this.minSpawnsPerHour, this.maxSpawnsPerHour) * num * 0.00027777778450399637));
		ICollection<Identifiable.Id> allAuxItems = this.zoneDir.GetAllAuxItems();
		if (num2 > 0f && allAuxItems.Count > 0)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (Identifiable.Id id in allAuxItems)
			{
				this.cellDir.Get(id, ref list);
			}
			List<Transform> list2 = new List<Transform>(this.spawnNodes);
			foreach (Transform transform in this.spawnNodes)
			{
				using (List<GameObject>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if ((enumerator2.Current.transform.position - transform.position).sqrMagnitude < 1.0000001E-06f)
						{
							list2.Remove(transform);
							break;
						}
					}
				}
			}
			int num3 = 0;
			while ((float)num3 < num2 && list2.Count > 0)
			{
				Transform node = Randoms.SHARED.Pluck<Transform>(list2, null);
				this.SpawnAt(node);
				num3++;
			}
		}
	}

	// Token: 0x06002506 RID: 9478 RVA: 0x0008E470 File Offset: 0x0008C670
	private void SpawnAt(Transform node)
	{
		Identifiable.Id id = this.zoneDir.PickAuxItem();
		SRBehaviour.InstantiateActor(this.lookupDir.GetPrefab(id), this.zoneDir.regionSetId, node.position, node.rotation, false);
	}

	// Token: 0x06002507 RID: 9479 RVA: 0x0008E4B3 File Offset: 0x0008C6B3
	private void ResetSpawnTime(EchoNetModel model)
	{
		model.lastSpawnTime = this.timeDir.WorldTime();
	}

	// Token: 0x040023DF RID: 9183
	public float minSpawnsPerHour = 0.2f;

	// Token: 0x040023E0 RID: 9184
	public float maxSpawnsPerHour = 0.3333f;

	// Token: 0x040023E1 RID: 9185
	public Transform[] spawnNodes;

	// Token: 0x040023E2 RID: 9186
	public GameObject activeVersion;

	// Token: 0x040023E3 RID: 9187
	public GameObject inactiveVersion;

	// Token: 0x040023E4 RID: 9188
	private CellDirector cellDir;

	// Token: 0x040023E5 RID: 9189
	private ZoneDirector zoneDir;

	// Token: 0x040023E6 RID: 9190
	private LookupDirector lookupDir;

	// Token: 0x040023E7 RID: 9191
	private TimeDirector timeDir;

	// Token: 0x040023E8 RID: 9192
	private EchoNetModel model;

	// Token: 0x040023E9 RID: 9193
	private const float PRESENT_DIST = 0.001f;

	// Token: 0x040023EA RID: 9194
	private const float SQR_PRESENT_DIST = 1.0000001E-06f;
}
