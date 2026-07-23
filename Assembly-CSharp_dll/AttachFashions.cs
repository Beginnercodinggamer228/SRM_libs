using System;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x0200038E RID: 910
public class AttachFashions : SRBehaviour, ActorModel.Participant, GadgetModel.Participant, GordoModel.Participant
{
	// Token: 0x060012F7 RID: 4855 RVA: 0x0004A064 File Offset: 0x00048264
	public void Awake()
	{
		if (!this.gordoMode && Identifiable.IsSlime(Identifiable.GetId(base.gameObject)))
		{
			this.slimeAppearanceApplicator = base.GetRequiredComponent<SlimeAppearanceApplicator>();
		}
	}

	// Token: 0x060012F8 RID: 4856 RVA: 0x0004A08C File Offset: 0x0004828C
	public void SetModel(ActorModel model)
	{
		this.slimeModel = (model as SlimeModel);
		if (this.slimeModel != null)
		{
			this.SetFashions(new List<Identifiable.Id>(this.slimeModel.fashions));
		}
		this.animalModel = (model as AnimalModel);
		if (this.animalModel != null)
		{
			this.SetFashions(new List<Identifiable.Id>(this.animalModel.fashions));
		}
	}

	// Token: 0x060012F9 RID: 4857 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(ActorModel model)
	{
	}

	// Token: 0x060012FA RID: 4858 RVA: 0x0004A0F0 File Offset: 0x000482F0
	public void SetModel(GadgetModel model)
	{
		if (model is SnareModel)
		{
			this.snareModel = (SnareModel)model;
			this.SetFashions(new List<Identifiable.Id>(this.snareModel.fashions));
			return;
		}
		if (model is DroneModel)
		{
			this.droneModel = (DroneModel)model;
			this.SetFashions(new List<Identifiable.Id>(this.droneModel.fashions));
			return;
		}
		Log.Error("Unknown type of gadget model for fashions", Array.Empty<object>());
	}

	// Token: 0x060012FB RID: 4859 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(GadgetModel model)
	{
	}

	// Token: 0x060012FC RID: 4860 RVA: 0x0004A162 File Offset: 0x00048362
	public void SetModel(GordoModel model)
	{
		this.gordoModel = model;
		this.SetFashions(new List<Identifiable.Id>(this.gordoModel.fashions));
	}

	// Token: 0x060012FD RID: 4861 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(GordoModel model)
	{
	}

	// Token: 0x060012FE RID: 4862 RVA: 0x00003296 File Offset: 0x00001496
	public void OnResetEatenCount()
	{
	}

	// Token: 0x060012FF RID: 4863 RVA: 0x0004A184 File Offset: 0x00048384
	public void Attach(Fashion fashion, bool skipFX = false)
	{
		if (this.slotted.ContainsKey(fashion.slot))
		{
			if (this.slimeModel != null)
			{
				this.slimeModel.fashions.Remove(this.slotted[fashion.slot].id);
			}
			else if (this.animalModel != null)
			{
				this.animalModel.fashions.Remove(this.slotted[fashion.slot].id);
			}
			else if (this.snareModel != null)
			{
				this.snareModel.fashions.Remove(this.slotted[fashion.slot].id);
			}
			else if (this.droneModel != null)
			{
				this.droneModel.fashions.Remove(this.slotted[fashion.slot].id);
			}
			else if (this.gordoModel != null)
			{
				this.gordoModel.fashions.Remove(this.slotted[fashion.slot].id);
			}
			Destroyer.Destroy(this.slotted[fashion.slot].gameObj, "AttachFashions.Attach");
		}
		Identifiable component = fashion.GetComponent<Identifiable>();
		Vector3 parentOffset = this.GetParentOffset(component.id);
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(fashion.attachPrefab, parentOffset, Quaternion.identity);
		Transform parentForSlot = this.GetParentForSlot(fashion.slot, component.id);
		gameObject.transform.SetParent(parentForSlot, false);
		this.slotted[fashion.slot] = new AttachFashions.FashionEntry(component.id, gameObject);
		if (this.slimeModel != null)
		{
			this.slimeModel.fashions.Add(component.id);
		}
		else if (this.animalModel != null)
		{
			this.animalModel.fashions.Add(component.id);
		}
		else if (this.snareModel != null)
		{
			this.snareModel.fashions.Add(component.id);
		}
		else if (this.droneModel != null)
		{
			this.droneModel.fashions.Add(component.id);
		}
		else if (this.gordoModel != null)
		{
			this.gordoModel.fashions.Add(component.id);
		}
		if (!skipFX && fashion.attachFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(fashion.attachFX, gameObject.transform.position, gameObject.transform.rotation);
		}
	}

	// Token: 0x06001300 RID: 4864 RVA: 0x0004A3F4 File Offset: 0x000485F4
	public void DetachAll(FashionRemover remover)
	{
		foreach (AttachFashions.FashionEntry fashionEntry in this.slotted.Values)
		{
			Destroyer.Destroy(fashionEntry.gameObj, "AttachFashions.DetachAll");
		}
		if (remover.removeFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(remover.removeFX, remover.transform.position, remover.transform.rotation);
		}
		this.slotted.Clear();
		if (this.slimeModel != null)
		{
			this.slimeModel.fashions.Clear();
			return;
		}
		if (this.animalModel != null)
		{
			this.animalModel.fashions.Clear();
			return;
		}
		if (this.snareModel != null)
		{
			this.snareModel.fashions.Clear();
			return;
		}
		if (this.droneModel != null)
		{
			this.droneModel.fashions.Clear();
			return;
		}
		if (this.gordoModel != null)
		{
			this.gordoModel.fashions.Clear();
		}
	}

	// Token: 0x06001301 RID: 4865 RVA: 0x0004A50C File Offset: 0x0004870C
	private Vector3 GetParentOffset(Identifiable.Id id)
	{
		if (Identifiable.MEAT_CLASS.Contains(Identifiable.GetId(base.gameObject)))
		{
			switch (id)
			{
			case Identifiable.Id.HANDLEBAR_FASHION:
				return new Vector3(0f, -0.127f, 0f);
			case Identifiable.Id.GOOGLY_FASHION:
				return new Vector3(0f, 0f, -0.09f);
			case Identifiable.Id.CUTE_FASHION:
				return new Vector3(0.049f, 0.044f, -0.179f);
			case Identifiable.Id.ROYAL_FASHION:
				return new Vector3(-0.033f, 0f, -0.101f);
			case Identifiable.Id.PIRATEY_FASHION:
				return new Vector3(0f, 0f, -0.16f);
			case Identifiable.Id.HEROIC_FASHION:
				return new Vector3(0f, 0f, -0.16f);
			case Identifiable.Id.SCIFI_FASHION:
				return new Vector3(0f, 0f, -0.16f);
			}
		}
		return Vector3.zero;
	}

	// Token: 0x06001302 RID: 4866 RVA: 0x0004A610 File Offset: 0x00048810
	private Transform GetParentForSlot(Fashion.Slot slot, Identifiable.Id id)
	{
		if (Identifiable.MEAT_CLASS.Contains(Identifiable.GetId(base.gameObject)))
		{
			return this.attachmentFront;
		}
		if (this.slimeAppearanceApplicator != null)
		{
			return this.slimeAppearanceApplicator.GetFashionParent(slot);
		}
		string str = this.gordoMode ? "Vibrating" : "prefab_slimeBase";
		if (slot == Fashion.Slot.TOP)
		{
			return base.transform.Find(str + "/bone_root/bone_slime/bone_core/bone_jiggle_top/bone_skin_top");
		}
		if (slot == Fashion.Slot.FRONT)
		{
			return base.transform.Find(str + "/bone_root/bone_slime/bone_core/bone_jiggle_bac/bone_skin_bac");
		}
		Log.Error("Unhandled fashion slot", new object[]
		{
			"slot",
			slot
		});
		return null;
	}

	// Token: 0x06001303 RID: 4867 RVA: 0x0004A6C4 File Offset: 0x000488C4
	public List<Identifiable.Id> GetAllFashions()
	{
		List<Identifiable.Id> list = new List<Identifiable.Id>();
		foreach (AttachFashions.FashionEntry fashionEntry in this.slotted.Values)
		{
			list.Add(fashionEntry.id);
		}
		return list;
	}

	// Token: 0x06001304 RID: 4868 RVA: 0x0004A728 File Offset: 0x00048928
	public bool HasFashion(Identifiable.Id id)
	{
		return this.slotted.Values.Any((AttachFashions.FashionEntry e) => e.id == id);
	}

	// Token: 0x06001305 RID: 4869 RVA: 0x0004A760 File Offset: 0x00048960
	public void SetFashions(List<Identifiable.Id> ids)
	{
		if (this.slimeModel != null)
		{
			this.slimeModel.fashions.Clear();
		}
		else if (this.animalModel != null)
		{
			this.animalModel.fashions.Clear();
		}
		else if (this.snareModel != null)
		{
			this.snareModel.fashions.Clear();
		}
		else if (this.droneModel != null)
		{
			this.droneModel.fashions.Clear();
		}
		else if (this.gordoModel != null)
		{
			this.gordoModel.fashions.Clear();
		}
		LookupDirector lookupDirector = SRSingleton<GameContext>.Instance.LookupDirector;
		foreach (Identifiable.Id id in ids)
		{
			Fashion component = lookupDirector.GetPrefab(id).GetComponent<Fashion>();
			if (component != null)
			{
				this.Attach(component, true);
			}
		}
	}

	// Token: 0x040011D9 RID: 4569
	[Tooltip("If true, changes the root attachment path based off the gordo structure. (slimes only)")]
	public bool gordoMode;

	// Token: 0x040011DA RID: 4570
	[Tooltip("Transform to attach front fashions to. (not used by slimes)")]
	public Transform attachmentFront;

	// Token: 0x040011DB RID: 4571
	private Dictionary<Fashion.Slot, AttachFashions.FashionEntry> slotted = new Dictionary<Fashion.Slot, AttachFashions.FashionEntry>();

	// Token: 0x040011DC RID: 4572
	private SlimeModel slimeModel;

	// Token: 0x040011DD RID: 4573
	private SnareModel snareModel;

	// Token: 0x040011DE RID: 4574
	private DroneModel droneModel;

	// Token: 0x040011DF RID: 4575
	private GordoModel gordoModel;

	// Token: 0x040011E0 RID: 4576
	private AnimalModel animalModel;

	// Token: 0x040011E1 RID: 4577
	private SlimeAppearanceApplicator slimeAppearanceApplicator;

	// Token: 0x0200038F RID: 911
	private class FashionEntry
	{
		// Token: 0x06001307 RID: 4871 RVA: 0x0004A863 File Offset: 0x00048A63
		public FashionEntry(Identifiable.Id id, GameObject gameObj)
		{
			this.id = id;
			this.gameObj = gameObj;
		}

		// Token: 0x040011E2 RID: 4578
		public Identifiable.Id id;

		// Token: 0x040011E3 RID: 4579
		public GameObject gameObj;
	}
}
