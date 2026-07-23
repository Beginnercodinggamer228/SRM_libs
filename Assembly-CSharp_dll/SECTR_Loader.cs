using System;
using UnityEngine;

// Token: 0x020000B1 RID: 177
public abstract class SECTR_Loader : MonoBehaviour
{
	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x06000416 RID: 1046
	public abstract bool Loaded { get; }

	// Token: 0x06000417 RID: 1047 RVA: 0x00018D4C File Offset: 0x00016F4C
	protected void LockSelf(bool lockSelf)
	{
		if (lockSelf != this.locked)
		{
			Rigidbody[] componentsInChildren = base.GetComponentsInChildren<Rigidbody>();
			int num = componentsInChildren.Length;
			for (int i = 0; i < num; i++)
			{
				Rigidbody rigidbody = componentsInChildren[i];
				if (lockSelf)
				{
					rigidbody.Sleep();
				}
				else
				{
					rigidbody.WakeUp();
				}
			}
			Collider[] componentsInChildren2 = base.GetComponentsInChildren<Collider>();
			int num2 = componentsInChildren2.Length;
			for (int j = 0; j < num2; j++)
			{
				componentsInChildren2[j].enabled = !lockSelf;
			}
			this.locked = lockSelf;
		}
	}

	// Token: 0x040003FD RID: 1021
	protected bool locked;
}
