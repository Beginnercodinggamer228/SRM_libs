using System;
using UnityEngine;

// Token: 0x020006C7 RID: 1735
public class Attractor : SRBehaviour
{
	// Token: 0x06002425 RID: 9253 RVA: 0x0008B92C File Offset: 0x00089B2C
	public virtual void OnTriggerEnter(Collider col)
	{
		AweTowardsAttractors component = col.GetComponent<AweTowardsAttractors>();
		if (component != null)
		{
			component.RegisterAttractor(this);
		}
	}

	// Token: 0x06002426 RID: 9254 RVA: 0x0008B950 File Offset: 0x00089B50
	public virtual void OnTriggerExit(Collider col)
	{
		AweTowardsAttractors component = col.GetComponent<AweTowardsAttractors>();
		if (component != null)
		{
			component.UnregisterAttractor(this);
		}
	}

	// Token: 0x06002427 RID: 9255 RVA: 0x0008B974 File Offset: 0x00089B74
	public void SetAweFactor(float aweFactor)
	{
		this.aweFactor = aweFactor;
	}

	// Token: 0x06002428 RID: 9256 RVA: 0x0008B97D File Offset: 0x00089B7D
	public virtual float AweFactor(GameObject slime)
	{
		if (!base.isActiveAndEnabled)
		{
			return 0f;
		}
		return this.aweFactor;
	}

	// Token: 0x06002429 RID: 9257 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	public virtual bool CauseMoveTowards()
	{
		return false;
	}

	// Token: 0x0400233A RID: 9018
	private float aweFactor;
}
