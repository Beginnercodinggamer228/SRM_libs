using System;
using UnityEngine;

// Token: 0x0200013E RID: 318
public class DroneGadgetInteractor : MonoBehaviour, GadgetInteractor
{
	// Token: 0x170000F4 RID: 244
	// (get) Token: 0x060006E9 RID: 1769 RVA: 0x00023D22 File Offset: 0x00021F22
	// (set) Token: 0x060006EA RID: 1770 RVA: 0x00023D2A File Offset: 0x00021F2A
	public DroneGadget gadget { get; private set; }

	// Token: 0x060006EB RID: 1771 RVA: 0x00023D33 File Offset: 0x00021F33
	public void Awake()
	{
		this.gadget = base.GetComponentInParent<DroneGadget>();
	}

	// Token: 0x060006EC RID: 1772 RVA: 0x00013CC5 File Offset: 0x00011EC5
	public bool CanInteract()
	{
		return true;
	}

	// Token: 0x060006ED RID: 1773 RVA: 0x00023D41 File Offset: 0x00021F41
	public void OnInteract()
	{
		this.gadget.drone.InstantiateDroneUI();
	}
}
