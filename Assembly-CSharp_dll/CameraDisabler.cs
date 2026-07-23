using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000293 RID: 659
public class CameraDisabler : MonoBehaviour
{
	// Token: 0x06000DE0 RID: 3552 RVA: 0x000387E8 File Offset: 0x000369E8
	public void Start()
	{
		this.uiOnlyMask = LayerMask.GetMask(new string[]
		{
			"UI"
		});
		this.cam = base.GetComponent<Camera>();
		this.standardMask = this.cam.cullingMask;
	}

	// Token: 0x06000DE1 RID: 3553 RVA: 0x00038835 File Offset: 0x00036A35
	public void AddBlocker(Component comp)
	{
		this.blockers.Add(comp);
		if (this.blockers.Count > 0)
		{
			this.cam.cullingMask = this.uiOnlyMask;
		}
	}

	// Token: 0x06000DE2 RID: 3554 RVA: 0x00038868 File Offset: 0x00036A68
	public void RemoveBlocker(Component comp)
	{
		this.blockers.Remove(comp);
		if (this.blockers.Count <= 0 && this.cam != null)
		{
			this.cam.cullingMask = this.standardMask;
		}
	}

	// Token: 0x04000D13 RID: 3347
	private Camera cam;

	// Token: 0x04000D14 RID: 3348
	private List<Component> blockers = new List<Component>();

	// Token: 0x04000D15 RID: 3349
	private LayerMask standardMask;

	// Token: 0x04000D16 RID: 3350
	private LayerMask uiOnlyMask;
}
