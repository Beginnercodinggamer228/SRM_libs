using System;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x0200015A RID: 346
[RequireComponent(typeof(Drone))]
public class DroneNoClip : SRBehaviour, GadgetModel.Participant
{
	// Token: 0x06000775 RID: 1909 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(GadgetModel model)
	{
	}

	// Token: 0x06000776 RID: 1910 RVA: 0x00025711 File Offset: 0x00023911
	public void SetModel(GadgetModel model)
	{
		this.model = (DroneModel)model;
	}

	// Token: 0x06000777 RID: 1911 RVA: 0x0002571F File Offset: 0x0002391F
	public void OnEnable()
	{
		base.gameObject.layer = 14;
		this.model.noClip = true;
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x0002573A File Offset: 0x0002393A
	public void OnDisable()
	{
		base.gameObject.layer = 20;
		this.model.noClip = false;
	}

	// Token: 0x040006DF RID: 1759
	private DroneModel model;
}
