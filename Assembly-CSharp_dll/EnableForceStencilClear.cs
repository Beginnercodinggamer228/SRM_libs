using System;
using UnityEngine;

// Token: 0x020001E4 RID: 484
public class EnableForceStencilClear : MonoBehaviour
{
	// Token: 0x06000A1E RID: 2590 RVA: 0x0002C2FD File Offset: 0x0002A4FD
	public void Awake()
	{
		this.instance = Camera.main.GetComponent<ForceStencilClear>();
	}

	// Token: 0x06000A1F RID: 2591 RVA: 0x0002C30F File Offset: 0x0002A50F
	public void OnEnable()
	{
		if (this.instance != null)
		{
			this.instance.RegisterEnabler(base.gameObject);
		}
	}

	// Token: 0x06000A20 RID: 2592 RVA: 0x0002C330 File Offset: 0x0002A530
	public void OnDisable()
	{
		if (this.instance != null)
		{
			this.instance.DeregisterEnabler(base.gameObject);
		}
	}

	// Token: 0x04000853 RID: 2131
	private ForceStencilClear instance;
}
