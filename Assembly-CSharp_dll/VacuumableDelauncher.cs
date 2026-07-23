using System;
using UnityEngine;

// Token: 0x020006B9 RID: 1721
public class VacuumableDelauncher : MonoBehaviour
{
	// Token: 0x060023E6 RID: 9190 RVA: 0x0008AC30 File Offset: 0x00088E30
	public void OnTriggerEnter(Collider col)
	{
		VacDelaunchTrigger component = col.gameObject.GetComponent<VacDelaunchTrigger>();
		if (component != null)
		{
			component.Delaunch();
		}
	}
}
