using System;
using MonomiPark.SlimeRancher.DataModel;

// Token: 0x02000320 RID: 800
public class LinkedSiloStorage : SiloStorage, Gadget.LinkDestroyer, GadgetModel.Participant
{
	// Token: 0x060010EF RID: 4335 RVA: 0x00043BEC File Offset: 0x00041DEC
	public override void Awake()
	{
		this.gadgetId = base.GetComponentInParent<Gadget>().id;
		foreach (GadgetSiteModel gadgetSiteModel in SRSingleton<SceneContext>.Instance.GameModel.AllGadgetSites().Values)
		{
			if (gadgetSiteModel.HasAttached() && gadgetSiteModel.attached.ident == this.gadgetId)
			{
				this.link = gadgetSiteModel.attached.transform.GetComponentInChildren<LinkedSiloStorage>();
				this.link.link = this;
				break;
			}
		}
		base.Awake();
	}

	// Token: 0x060010F0 RID: 4336 RVA: 0x00043C9C File Offset: 0x00041E9C
	public void InitModel(GadgetModel baseModel)
	{
		WarpDepotModel warpDepotModel = (WarpDepotModel)baseModel;
		base.LocalAmmo.InitModel(warpDepotModel.ammo);
		warpDepotModel.isPrimary = true;
	}

	// Token: 0x060010F1 RID: 4337 RVA: 0x00043CC8 File Offset: 0x00041EC8
	public void SetModel(GadgetModel baseModel)
	{
		this.model = (WarpDepotModel)baseModel;
		base.LocalAmmo.SetModel(this.model.ammo);
		if (!this.model.isPrimary && this.link == null)
		{
			this.model.isPrimary = true;
		}
		if (this.link != null && this.link.model != null && this.model.isPrimary == this.link.model.isPrimary)
		{
			this.model.isPrimary = !this.ammo.IsEmpty();
			this.link.model.isPrimary = !this.model.isPrimary;
		}
	}

	// Token: 0x060010F2 RID: 4338 RVA: 0x00043D8D File Offset: 0x00041F8D
	public override Ammo GetRelevantAmmo()
	{
		if (this.model.isPrimary)
		{
			return this.ammo;
		}
		return this.link.ammo;
	}

	// Token: 0x060010F3 RID: 4339 RVA: 0x00043DAE File Offset: 0x00041FAE
	public bool ShouldDestroyPair()
	{
		return this.link != null;
	}

	// Token: 0x060010F4 RID: 4340 RVA: 0x00043DBC File Offset: 0x00041FBC
	public Gadget.LinkDestroyer GetLinked()
	{
		return this.link;
	}

	// Token: 0x04000FE6 RID: 4070
	private Gadget.Id gadgetId;

	// Token: 0x04000FE7 RID: 4071
	private new WarpDepotModel model;

	// Token: 0x04000FE8 RID: 4072
	private LinkedSiloStorage link;
}
