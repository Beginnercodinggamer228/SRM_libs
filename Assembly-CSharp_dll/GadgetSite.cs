using System;
using Assets.Script.Util.Extensions;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x02000711 RID: 1809
public class GadgetSite : IdHandler, GadgetSiteModel.Participant
{
	// Token: 0x060025B3 RID: 9651 RVA: 0x00090A39 File Offset: 0x0008EC39
	public void Awake()
	{
		this.gadgetDir = SRSingleton<SceneContext>.Instance.GadgetDirector;
		SRSingleton<SceneContext>.Instance.GameModel.RegisterGadgetSite(base.id, base.gameObject, this);
	}

	// Token: 0x060025B4 RID: 9652 RVA: 0x00090A67 File Offset: 0x0008EC67
	protected override string IdPrefix()
	{
		return "site";
	}

	// Token: 0x060025B5 RID: 9653 RVA: 0x00090A70 File Offset: 0x0008EC70
	public void OnDestroy()
	{
		if (this.attached != null)
		{
			Gadget component = this.attached.GetComponent<Gadget>();
			if (component != null)
			{
				this.gadgetDir.DecrementPlacedGadgetCount(component.id);
				if (SRSingleton<SceneContext>.Instance != null)
				{
					SRSingleton<SceneContext>.Instance.GameModel.UnregisterGadgetSite(base.id);
				}
			}
		}
	}

	// Token: 0x060025B6 RID: 9654 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(GadgetSiteModel model)
	{
	}

	// Token: 0x060025B7 RID: 9655 RVA: 0x00090AD3 File Offset: 0x0008ECD3
	public void SetModel(GadgetSiteModel model)
	{
		this.model = model;
	}

	// Token: 0x060025B8 RID: 9656 RVA: 0x00090ADC File Offset: 0x0008ECDC
	public void SetAttached(GadgetModel gadgetModel)
	{
		if (gadgetModel == null)
		{
			this.attached = null;
			return;
		}
		this.attached = gadgetModel.transform.gameObject;
		Gadget component = this.attached.GetComponent<Gadget>();
		if (component != null)
		{
			this.gadgetDir.IncrementPlacedGadgetCount(component.id);
		}
	}

	// Token: 0x060025B9 RID: 9657 RVA: 0x00090B2C File Offset: 0x0008ED2C
	public virtual void Activate()
	{
		PlaceGadgetUI component = UnityEngine.Object.Instantiate<GameObject>(this.placeGadgetUIPrefab).GetComponent<PlaceGadgetUI>();
		if (component != null)
		{
			component.SetSite(this, this.model);
		}
	}

	// Token: 0x060025BA RID: 9658 RVA: 0x00090B60 File Offset: 0x0008ED60
	public bool HasAttached()
	{
		return this.attached != null;
	}

	// Token: 0x060025BB RID: 9659 RVA: 0x00090B70 File Offset: 0x0008ED70
	public Gadget.Id GetAttachedId()
	{
		if (this.attached == null)
		{
			return Gadget.Id.NONE;
		}
		Gadget component = this.attached.GetComponent<Gadget>();
		if (!(component == null))
		{
			return component.id;
		}
		return Gadget.Id.NONE;
	}

	// Token: 0x060025BC RID: 9660 RVA: 0x00090BAA File Offset: 0x0008EDAA
	public GameObject GetAttached()
	{
		return this.attached;
	}

	// Token: 0x060025BD RID: 9661 RVA: 0x00090BB4 File Offset: 0x0008EDB4
	public void DestroyAttached()
	{
		Gadget component = this.attached.GetComponent<Gadget>();
		if (component != null)
		{
			this.gadgetDir.DecrementPlacedGadgetCount(component.id);
			component.OnUserDestroyed();
		}
		Destroyer.DestroyGadget(base.id, this.attached, "GadgetSite.DestroyAttached");
		this.attached = null;
	}

	// Token: 0x060025BE RID: 9662 RVA: 0x00090C0A File Offset: 0x0008EE0A
	public void DestroyAttachedWithPair()
	{
		GadgetSite componentInParent = ((MonoBehaviour)this.attached.GetComponentInChildren<Gadget.LinkDestroyer>().GetLinked()).GetComponentInParent(true);
		this.DestroyAttached();
		componentInParent.DestroyAttached();
	}

	// Token: 0x060025BF RID: 9663 RVA: 0x00090C32 File Offset: 0x0008EE32
	public void OnRotateCW()
	{
		if (this.attached != null)
		{
			this.attached.GetComponent<Gadget>().AddRotation(GadgetSite.ROT_SPEED * Time.deltaTime);
		}
	}

	// Token: 0x060025C0 RID: 9664 RVA: 0x00090C5D File Offset: 0x0008EE5D
	public void OnRotateCCW()
	{
		if (this.attached != null)
		{
			this.attached.GetComponent<Gadget>().AddRotation(-GadgetSite.ROT_SPEED * Time.deltaTime);
		}
	}

	// Token: 0x060025C1 RID: 9665 RVA: 0x00090C8C File Offset: 0x0008EE8C
	public void RotateToPlayer()
	{
		if (this.attached != null)
		{
			Vector3 vector = SRSingleton<SceneContext>.Instance.Player.transform.position - base.transform.position;
			this.attached.transform.rotation = Quaternion.LookRotation(new Vector3(vector.x, 0f, vector.z), Vector3.up);
		}
	}

	// Token: 0x060025C2 RID: 9666 RVA: 0x00090CFC File Offset: 0x0008EEFC
	public bool DestroysLinkedPairOnRemoval()
	{
		Gadget component = this.attached.GetComponent<Gadget>();
		return component != null && component.DestroysLinkedPairOnRemoval();
	}

	// Token: 0x060025C3 RID: 9667 RVA: 0x00090D28 File Offset: 0x0008EF28
	public bool DestroysOnRemoval()
	{
		Gadget component = this.attached.GetComponent<Gadget>();
		return component != null && component.DestroysOnRemoval();
	}

	// Token: 0x060025C4 RID: 9668 RVA: 0x00090D54 File Offset: 0x0008EF54
	public bool DestroyingWillDestroyContents()
	{
		if (this.attached != null)
		{
			LinkedSiloStorage component = this.attached.GetComponent<LinkedSiloStorage>();
			return component != null && !component.GetRelevantAmmo().IsEmpty();
		}
		return false;
	}

	// Token: 0x04002530 RID: 9520
	public GameObject placeGadgetUIPrefab;

	// Token: 0x04002531 RID: 9521
	private GameObject attached;

	// Token: 0x04002532 RID: 9522
	private GadgetDirector gadgetDir;

	// Token: 0x04002533 RID: 9523
	private GadgetSiteModel model;

	// Token: 0x04002534 RID: 9524
	private static float ROT_SPEED = 360f;
}
