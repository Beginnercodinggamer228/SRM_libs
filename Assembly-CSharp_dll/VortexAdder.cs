using System;
using UnityEngine;

// Token: 0x020007CA RID: 1994
public class VortexAdder : MonoBehaviour
{
	// Token: 0x060029BC RID: 10684 RVA: 0x0009CDC7 File Offset: 0x0009AFC7
	public void OnTriggerEnter(Collider col)
	{
		if (this.CanAdd(col.gameObject))
		{
			this.vortexer.Connect(col.gameObject);
		}
	}

	// Token: 0x060029BD RID: 10685 RVA: 0x0009CDE8 File Offset: 0x0009AFE8
	protected virtual bool CanAdd(GameObject gameObj)
	{
		Vacuumable component = gameObj.GetComponent<Vacuumable>();
		return component != null && !component.isCaptive() && component.canCapture();
	}

	// Token: 0x040028F3 RID: 10483
	public ActorVortexer vortexer;
}
