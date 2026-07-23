using System;
using UnityEngine;

// Token: 0x020004B2 RID: 1202
public class TotemLinkerHelper : MonoBehaviour
{
	// Token: 0x0600192D RID: 6445 RVA: 0x000622E0 File Offset: 0x000604E0
	public void Start()
	{
		TotemLinker[] componentsInChildren = base.GetComponentsInChildren<TotemLinker>(true);
		if (componentsInChildren != null && componentsInChildren.Length != 0)
		{
			this.totemLinker = componentsInChildren[0];
		}
	}

	// Token: 0x0600192E RID: 6446 RVA: 0x00062305 File Offset: 0x00060505
	public void Update()
	{
		this.totemLinker.UpdateEvenWhenInactive();
	}

	// Token: 0x040018F5 RID: 6389
	private TotemLinker totemLinker;
}
