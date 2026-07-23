using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x0200071D RID: 1821
public class GordoSnare : SRBehaviour, GadgetModel.Participant
{
	// Token: 0x06002602 RID: 9730 RVA: 0x0009166A File Offset: 0x0008F86A
	public void Awake()
	{
		GordoSnare.AllGordoSnares.Add(this);
	}

	// Token: 0x06002603 RID: 9731 RVA: 0x00091677 File Offset: 0x0008F877
	public void OnDestroy()
	{
		GordoSnare.AllGordoSnares.Remove(this);
	}

	// Token: 0x06002604 RID: 9732 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(GadgetModel model)
	{
	}

	// Token: 0x06002605 RID: 9733 RVA: 0x00091688 File Offset: 0x0008F888
	public void SetModel(GadgetModel model)
	{
		this.model = (SnareModel)model;
		if (this.model.baitTypeId != Identifiable.Id.NONE)
		{
			this.AttachBait(this.model.baitTypeId);
			return;
		}
		if (this.model.gordoTypeId != Identifiable.Id.NONE)
		{
			this.SnareGordo(this.model.gordoTypeId);
		}
	}

	// Token: 0x06002606 RID: 9734 RVA: 0x000916E0 File Offset: 0x0008F8E0
	public void OnTriggerEnter(Collider col)
	{
		if (!col.isTrigger && this.bait == null && !this.isSnared)
		{
			Identifiable component = col.GetComponent<Identifiable>();
			if (component != null && Identifiable.IsFood(component.id))
			{
				if (this.baitAttachedFx != null)
				{
					SRBehaviour.SpawnAndPlayFX(this.baitAttachedFx, base.gameObject);
				}
				Destroyer.DestroyActor(col.gameObject, "GordoSnare.OnTriggerEnter", false);
				this.AttachBait(component.id);
			}
		}
	}

	// Token: 0x06002607 RID: 9735 RVA: 0x00091765 File Offset: 0x0008F965
	public bool HasSnaredGordo()
	{
		return this.isSnared;
	}

	// Token: 0x06002608 RID: 9736 RVA: 0x0009176D File Offset: 0x0008F96D
	public bool IsBaited()
	{
		return this.model.baitTypeId > Identifiable.Id.NONE;
	}

	// Token: 0x06002609 RID: 9737 RVA: 0x00091780 File Offset: 0x0008F980
	public bool SnareGordo()
	{
		if (!this.IsBaited() || this.HasSnaredGordo())
		{
			return false;
		}
		Identifiable.Id gordoIdForBait = this.GetGordoIdForBait();
		this.SnareGordo(gordoIdForBait);
		if (gordoIdForBait == Identifiable.Id.HUNTER_GORDO)
		{
			SRSingleton<SceneContext>.Instance.AchievementsDirector.AddToStat(AchievementsDirector.IntStat.SNARED_HUNTER_GORDOS, 1);
		}
		return true;
	}

	// Token: 0x0600260A RID: 9738 RVA: 0x000917C8 File Offset: 0x0008F9C8
	private void SnareGordo(Identifiable.Id id)
	{
		this.model.gordoTypeId = id;
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(SRSingleton<GameContext>.Instance.LookupDirector.GetGordo(id), base.gameObject.transform);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		GadgetModel.Participant[] components = gameObject.GetComponents<GadgetModel.Participant>();
		GadgetModel.Participant[] array = components;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].InitModel(this.model);
		}
		array = components;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetModel(this.model);
		}
		this.isSnared = true;
		this.ClearBait();
	}

	// Token: 0x0600260B RID: 9739 RVA: 0x00091870 File Offset: 0x0008FA70
	private Identifiable.Id GetGordoIdForBait()
	{
		Dictionary<Identifiable.Id, float> dictionary = new Dictionary<Identifiable.Id, float>(Identifiable.idComparer);
		Identifiable.Id id = Identifiable.Id.NONE;
		List<Identifiable.Id> list = new List<Identifiable.Id>();
		foreach (GameObject gameObject in SRSingleton<GameContext>.Instance.LookupDirector.GordoEntries)
		{
			GordoEat component = gameObject.GetComponent<GordoEat>();
			GordoIdentifiable component2 = component.GetComponent<GordoIdentifiable>();
			if (component2.id != Identifiable.Id.PINK_GORDO)
			{
				SlimeDiet diet = component.slimeDefinition.Diet;
				List<SlimeDiet.EatMapEntry> list2 = new List<SlimeDiet.EatMapEntry>();
				diet.AddEatMapEntries(this.model.baitTypeId, list2);
				SlimeDiet.EatMapEntry eatMapEntry = (list2.Count > 0) ? list2[0] : null;
				bool flag = false;
				for (int i = 0; i < component2.nativeZones.Length; i++)
				{
					if (ZoneDirector.HasAccessToZone(component2.nativeZones[i]))
					{
						flag = true;
					}
				}
				if (flag && eatMapEntry != null)
				{
					if (eatMapEntry.isFavorite)
					{
						Log.Debug("Found favorite", new object[]
						{
							"gordo",
							component2.id,
							"hasAccess",
							flag
						});
						id = component2.id;
					}
					else
					{
						Log.Debug("Adding potential", new object[]
						{
							"gordo",
							component2.id,
							"hasAccess",
							flag
						});
						list.Add(component2.id);
					}
				}
			}
		}
		if (list.Count > 0)
		{
			float value = (float)(this.foodTypeSnareWeight / list.Count);
			for (int j = 0; j < list.Count; j++)
			{
				dictionary.Add(list[j], value);
			}
		}
		if (id != Identifiable.Id.NONE)
		{
			dictionary.Add(id, (float)this.favoredFoodSnareWeight);
		}
		dictionary.Add(Identifiable.Id.PINK_GORDO, (float)this.pinkSnareWeight);
		return Randoms.SHARED.Pick<Identifiable.Id>(dictionary, Identifiable.Id.PINK_GORDO);
	}

	// Token: 0x0600260C RID: 9740 RVA: 0x00091A80 File Offset: 0x0008FC80
	private void AttachBait(Identifiable.Id id)
	{
		this.ClearBait();
		this.model.baitTypeId = id;
		GameObject prefab = SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(id);
		this.bait = UnityEngine.Object.Instantiate<GameObject>(prefab, base.transform);
		this.bait.transform.position = this.baitPosition.transform.position;
		this.bait.transform.rotation = Quaternion.identity;
		this.RemoveComponents<Collider>(this.bait);
		this.RemoveComponent<DragFloatReactor>(this.bait);
		this.RemoveComponent<Rigidbody>(this.bait);
		this.RemoveComponent<KeepUpright>(this.bait);
		this.RemoveComponent<DontGoThroughThings>(this.bait);
		this.RemoveComponent<SECTR_PointSource>(this.bait);
		this.RemoveComponent<RegionMember>(this.bait);
		this.RemoveComponent<ChickenRandomMove>(this.bait);
		this.RemoveComponent<ChickenVampirism>(this.bait);
		this.RemoveComponent<PlaySoundOnHit>(this.bait);
		this.RemoveComponent<ResourceCycle>(this.bait);
		this.RemoveComponent<Reproduce>(this.bait);
		this.RemoveComponent<SlimeSubbehaviourPlexer>(this.bait);
		Animator componentInChildren = this.bait.GetComponentInChildren<Animator>();
		if (componentInChildren != null)
		{
			componentInChildren.SetBool("grounded", true);
		}
	}

	// Token: 0x0600260D RID: 9741 RVA: 0x00091BB4 File Offset: 0x0008FDB4
	public void Destroy()
	{
		Gadget componentInParent = base.GetComponentInParent<Gadget>();
		if (componentInParent != null)
		{
			componentInParent.DestroyGadget();
		}
	}

	// Token: 0x0600260E RID: 9742 RVA: 0x00091BD7 File Offset: 0x0008FDD7
	private void ClearBait()
	{
		if (this.bait != null)
		{
			this.model.baitTypeId = Identifiable.Id.NONE;
			Destroyer.Destroy(this.bait, 0f, "GordoSnare.ClearBait", true, false);
		}
	}

	// Token: 0x0600260F RID: 9743 RVA: 0x00091C0C File Offset: 0x0008FE0C
	private void RemoveComponent<T>(GameObject gameObject) where T : Component
	{
		T component = gameObject.GetComponent<T>();
		if (component != null)
		{
			Destroyer.Destroy(component, "GordoSnare.RemoveComponent");
		}
	}

	// Token: 0x06002610 RID: 9744 RVA: 0x00091C40 File Offset: 0x0008FE40
	private void RemoveComponents<T>(GameObject gameObject) where T : Component
	{
		T[] components = gameObject.GetComponents<T>();
		for (int i = 0; i < components.Length; i++)
		{
			Destroyer.Destroy(components[i], "GordoSnare.RemoveComponents");
		}
	}

	// Token: 0x06002611 RID: 9745 RVA: 0x00091C78 File Offset: 0x0008FE78
	private void RemoveComponentInChildren<T>(GameObject gameObject) where T : Component
	{
		T componentInChildren = gameObject.GetComponentInChildren<T>();
		if (componentInChildren != null)
		{
			Destroyer.Destroy(componentInChildren, "GordoSnare.RemoveComponentInChildren");
		}
	}

	// Token: 0x06002612 RID: 9746 RVA: 0x00091CAC File Offset: 0x0008FEAC
	private void RemoveComponentsInChildren<T>(GameObject gameObject) where T : Component
	{
		T[] componentsInChildren = gameObject.GetComponentsInChildren<T>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Destroyer.Destroy(componentsInChildren[i], "GordoSnare.RemoveComponentsInChildren");
		}
	}

	// Token: 0x06002613 RID: 9747 RVA: 0x00091CE4 File Offset: 0x0008FEE4
	public static bool HasSnaredGordo(GadgetSite site)
	{
		if (site.HasAttached())
		{
			Gadget.Id attachedId = site.GetAttachedId();
			if (attachedId == Gadget.Id.GORDO_SNARE_NOVICE || attachedId == Gadget.Id.GORDO_SNARE_ADVANCED || attachedId == Gadget.Id.GORDO_SNARE_MASTER)
			{
				GordoSnare componentInChildren = site.GetAttached().GetComponentInChildren<GordoSnare>();
				if (componentInChildren != null)
				{
					return componentInChildren.HasSnaredGordo();
				}
			}
		}
		return false;
	}

	// Token: 0x0400255B RID: 9563
	public static List<GordoSnare> AllGordoSnares = new List<GordoSnare>();

	// Token: 0x0400255C RID: 9564
	private bool isSnared;

	// Token: 0x0400255D RID: 9565
	public GameObject baitPosition;

	// Token: 0x0400255E RID: 9566
	public GameObject bait;

	// Token: 0x0400255F RID: 9567
	public GameObject baitAttachedFx;

	// Token: 0x04002560 RID: 9568
	public int pinkSnareWeight;

	// Token: 0x04002561 RID: 9569
	public int foodTypeSnareWeight;

	// Token: 0x04002562 RID: 9570
	public int favoredFoodSnareWeight;

	// Token: 0x04002563 RID: 9571
	private SnareModel model;
}
