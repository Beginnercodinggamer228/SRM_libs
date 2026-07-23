using System;
using UnityEngine;

// Token: 0x020001AD RID: 429
[RequireComponent(typeof(DroneStationBattery))]
public class DroneStation : SRBehaviour
{
	// Token: 0x1700011D RID: 285
	// (get) Token: 0x06000905 RID: 2309 RVA: 0x00028DAB File Offset: 0x00026FAB
	// (set) Token: 0x06000906 RID: 2310 RVA: 0x00028DB3 File Offset: 0x00026FB3
	public DroneStationAnimator animator { get; private set; }

	// Token: 0x1700011E RID: 286
	// (get) Token: 0x06000907 RID: 2311 RVA: 0x00028DBC File Offset: 0x00026FBC
	// (set) Token: 0x06000908 RID: 2312 RVA: 0x00028DC4 File Offset: 0x00026FC4
	public DroneStationBattery battery { get; private set; }

	// Token: 0x1700011F RID: 287
	// (get) Token: 0x06000909 RID: 2313 RVA: 0x00028DCD File Offset: 0x00026FCD
	// (set) Token: 0x0600090A RID: 2314 RVA: 0x00028DD5 File Offset: 0x00026FD5
	public DroneGadget gadget { get; private set; }

	// Token: 0x0600090B RID: 2315 RVA: 0x00028DDE File Offset: 0x00026FDE
	public void Awake()
	{
		this.gadget = base.GetComponentInParent<DroneGadget>();
		this.animator = base.GetComponentInChildren<DroneStationAnimator>();
		this.battery = base.GetComponent<DroneStationBattery>();
	}

	// Token: 0x040007A3 RID: 1955
	[Tooltip("Transform guide: resting position/rotation.")]
	public Transform guideRest;
}
