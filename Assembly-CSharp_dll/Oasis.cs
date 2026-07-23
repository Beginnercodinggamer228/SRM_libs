using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x0200073B RID: 1851
public class Oasis : IdHandler, OasisModel.Participant
{
	// Token: 0x060026AC RID: 9900 RVA: 0x0009378C File Offset: 0x0009198C
	public void Awake()
	{
		this.oasisCollider = base.GetComponent<SphereCollider>();
		if (this.oasisCollider != null)
		{
			this.oasisCollider.enabled = false;
		}
		SRSingleton<SceneContext>.Instance.GameModel.RegisterOasis(base.id, base.gameObject);
		this.CreateBoundingSphere();
	}

	// Token: 0x060026AD RID: 9901 RVA: 0x000937E0 File Offset: 0x000919E0
	public void OnDestroy()
	{
		if (SRSingleton<SceneContext>.Instance != null)
		{
			SRSingleton<SceneContext>.Instance.GameModel.UnregisterOasis(base.id);
		}
	}

	// Token: 0x060026AE RID: 9902 RVA: 0x00093804 File Offset: 0x00091A04
	private void CreateBoundingSphere()
	{
		Vector3 position = base.transform.position;
		this.boundingSphere = new BoundingSphere(new Vector4(position.x + this.oasisCollider.center.x, position.y + this.oasisCollider.center.y, position.z + this.oasisCollider.center.z, this.oasisCollider.radius));
	}

	// Token: 0x060026AF RID: 9903 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(OasisModel model)
	{
	}

	// Token: 0x060026B0 RID: 9904 RVA: 0x00093880 File Offset: 0x00091A80
	public void SetModel(OasisModel model)
	{
		this.model = model;
		int childCount = base.transform.childCount;
		this.children = new Transform[childCount];
		for (int i = 0; i < childCount; i++)
		{
			this.children[i] = base.transform.GetChild(i);
			if (!model.isLive)
			{
				this.children[i].gameObject.SetActive(false);
			}
		}
		if (model.isLive)
		{
			this.OnSetLive(true);
		}
	}

	// Token: 0x060026B1 RID: 9905 RVA: 0x000938F6 File Offset: 0x00091AF6
	public void SetLive(bool immediate)
	{
		if (!this.model.isLive)
		{
			this.OnSetLive(immediate);
		}
	}

	// Token: 0x060026B2 RID: 9906 RVA: 0x0009390C File Offset: 0x00091B0C
	private void OnSetLive(bool immediate)
	{
		if (this.oasisCollider != null)
		{
			this.oasisCollider.enabled = true;
			Oasis.oasisSpheres.Add(this.boundingSphere);
		}
		this.model.isLive = true;
		this.TweenScaleChildren(immediate);
		if (!immediate)
		{
			base.StartCoroutine(this.DelayedTriggerAllSpawners());
			SRSingleton<SceneContext>.Instance.AchievementsDirector.AddToStat(AchievementsDirector.IntStat.ACTIVATED_OASES, 1);
		}
		foreach (FireColumn fireColumn in this.suppressColumns)
		{
			if (fireColumn != null)
			{
				fireColumn.NoteInOasis();
			}
		}
	}

	// Token: 0x060026B3 RID: 9907 RVA: 0x000939A0 File Offset: 0x00091BA0
	public bool IsLive()
	{
		return this.model.isLive;
	}

	// Token: 0x060026B4 RID: 9908 RVA: 0x000939AD File Offset: 0x00091BAD
	private IEnumerator DelayedTriggerAllSpawners()
	{
		yield return new WaitForSeconds(this.scaleUpTime);
		this.TriggerAllSpawners();
		yield break;
	}

	// Token: 0x060026B5 RID: 9909 RVA: 0x000939BC File Offset: 0x00091BBC
	private void TriggerAllSpawners()
	{
		DirectedSlimeSpawner[] componentsInChildren = base.GetComponentsInChildren<DirectedSlimeSpawner>();
		List<DirectedSlimeSpawner> list = new List<DirectedSlimeSpawner>();
		foreach (DirectedSlimeSpawner directedSlimeSpawner in componentsInChildren)
		{
			if (directedSlimeSpawner.allowDirectedSpawns)
			{
				list.Add(directedSlimeSpawner);
			}
		}
		foreach (DirectedSlimeSpawner directedSlimeSpawner2 in list)
		{
			base.StartCoroutine(directedSlimeSpawner2.Spawn(this.targetSlimeCount / list.Count, Randoms.SHARED));
		}
		DirectedAnimalSpawner[] componentsInChildren2 = base.GetComponentsInChildren<DirectedAnimalSpawner>();
		List<DirectedAnimalSpawner> list2 = new List<DirectedAnimalSpawner>();
		foreach (DirectedAnimalSpawner directedAnimalSpawner in componentsInChildren2)
		{
			if (directedAnimalSpawner.allowDirectedSpawns)
			{
				list2.Add(directedAnimalSpawner);
			}
		}
		foreach (DirectedAnimalSpawner directedAnimalSpawner2 in list2)
		{
			base.StartCoroutine(directedAnimalSpawner2.Spawn(this.targetAnimalCount / list2.Count, Randoms.SHARED));
		}
	}

	// Token: 0x060026B6 RID: 9910 RVA: 0x00093AE4 File Offset: 0x00091CE4
	public void OnTriggerEnter(Collider col)
	{
		FireBall component = col.GetComponent<FireBall>();
		if (component != null)
		{
			component.Vaporize();
			return;
		}
		Identifiable component2 = col.GetComponent<Identifiable>();
		if (component2 != null)
		{
			if (Identifiable.IsSlime(component2.id))
			{
				this.slimeIdents.Add(component2);
				return;
			}
			if (Identifiable.IsAnimal(component2.id))
			{
				this.animalIdents.Add(component2);
			}
		}
	}

	// Token: 0x060026B7 RID: 9911 RVA: 0x00093B4C File Offset: 0x00091D4C
	public void OnTriggerExit(Collider col)
	{
		Identifiable component = col.GetComponent<Identifiable>();
		if (component != null)
		{
			if (Identifiable.IsSlime(component.id))
			{
				this.slimeIdents.Remove(component);
				return;
			}
			if (Identifiable.IsAnimal(component.id))
			{
				this.animalIdents.Remove(component);
			}
		}
	}

	// Token: 0x060026B8 RID: 9912 RVA: 0x00093B9E File Offset: 0x00091D9E
	public bool NeedsMoreSlimes()
	{
		this.EnsureNoDestroyedIdents();
		return this.slimeIdents.Count < this.targetSlimeCount;
	}

	// Token: 0x060026B9 RID: 9913 RVA: 0x00093BB9 File Offset: 0x00091DB9
	public bool NeedsMoreAnimals()
	{
		this.EnsureNoDestroyedIdents();
		return this.animalIdents.Count < this.targetAnimalCount;
	}

	// Token: 0x060026BA RID: 9914 RVA: 0x00093BD4 File Offset: 0x00091DD4
	private void TweenScaleChildren(bool immediate)
	{
		foreach (Transform transform in this.children)
		{
			transform.gameObject.SetActive(true);
			if (!immediate)
			{
				SpawnResource[] spawners = transform.GetComponentsInChildren<SpawnResource>(true);
				SpawnResource[] spawners2 = spawners;
				for (int j = 0; j < spawners2.Length; j++)
				{
					spawners2[j].RegisterSpawnBlocker();
				}
				TweenUtil.ScaleIn(transform.gameObject, this.scaleUpTime, Ease.OutQuad).OnComplete(delegate
				{
					this.TweenScaleChildren_OnTweenComplete(spawners);
				});
			}
		}
	}

	// Token: 0x060026BB RID: 9915 RVA: 0x00093C6C File Offset: 0x00091E6C
	private void TweenScaleChildren_OnTweenComplete(SpawnResource[] spawners)
	{
		for (int i = 0; i < spawners.Length; i++)
		{
			spawners[i].DeregisterSpawnBlocker();
		}
	}

	// Token: 0x060026BC RID: 9916 RVA: 0x00093C94 File Offset: 0x00091E94
	private void EnsureNoDestroyedIdents()
	{
		this.slimeIdents.RemoveAll((Identifiable ident) => ident == null);
		this.animalIdents.RemoveAll((Identifiable ident) => ident == null);
	}

	// Token: 0x060026BD RID: 9917 RVA: 0x00093CF7 File Offset: 0x00091EF7
	protected override string IdPrefix()
	{
		return "oasis";
	}

	// Token: 0x040025E7 RID: 9703
	public float scaleUpTime = 8f;

	// Token: 0x040025E8 RID: 9704
	public int targetSlimeCount = 20;

	// Token: 0x040025E9 RID: 9705
	public int targetAnimalCount = 5;

	// Token: 0x040025EA RID: 9706
	public FireColumn[] suppressColumns;

	// Token: 0x040025EB RID: 9707
	private Transform[] children;

	// Token: 0x040025EC RID: 9708
	private SphereCollider oasisCollider;

	// Token: 0x040025ED RID: 9709
	private List<Identifiable> slimeIdents = new List<Identifiable>();

	// Token: 0x040025EE RID: 9710
	private List<Identifiable> animalIdents = new List<Identifiable>();

	// Token: 0x040025EF RID: 9711
	private BoundingSphere boundingSphere;

	// Token: 0x040025F0 RID: 9712
	private OasisModel model;

	// Token: 0x040025F1 RID: 9713
	public static ExposedArrayList<BoundingSphere> oasisSpheres = new ExposedArrayList<BoundingSphere>(20);
}
