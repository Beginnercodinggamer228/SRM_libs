using System;
using UnityEngine;

// Token: 0x020003C0 RID: 960
public class DestroyTotemLinker : MonoBehaviour
{
	// Token: 0x06001412 RID: 5138 RVA: 0x0004DBD4 File Offset: 0x0004BDD4
	public void Start()
	{
		TotemLinker componentInChildren = base.GetComponentInChildren<TotemLinker>();
		if (componentInChildren != null)
		{
			Destroyer.Destroy(componentInChildren.gameObject, "DestroyTotemLinker.Start#1");
		}
		TotemLinkerHelper component = base.GetComponent<TotemLinkerHelper>();
		if (component != null)
		{
			Destroyer.Destroy(component, "DestroyTotemLinker.Start#2");
		}
	}
}
