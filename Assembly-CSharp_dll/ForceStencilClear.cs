using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001EA RID: 490
public class ForceStencilClear : MonoBehaviour
{
	// Token: 0x06000A4A RID: 2634 RVA: 0x0002C948 File Offset: 0x0002AB48
	public void OnPreRender()
	{
		GL.Clear(true, false, Color.black);
	}

	// Token: 0x06000A4B RID: 2635 RVA: 0x0002C956 File Offset: 0x0002AB56
	public void RegisterEnabler(GameObject enabler)
	{
		if (this.enablers == null)
		{
			this.enablers = new HashSet<GameObject>();
		}
		this.enablers.Add(enabler);
		base.enabled = true;
	}

	// Token: 0x06000A4C RID: 2636 RVA: 0x0002C97F File Offset: 0x0002AB7F
	public void DeregisterEnabler(GameObject enabler)
	{
		if (this.enablers != null && this.enablers.Remove(enabler) && this.enablers.Count == 0)
		{
			base.enabled = false;
		}
	}

	// Token: 0x0400085F RID: 2143
	private HashSet<GameObject> enablers;
}
