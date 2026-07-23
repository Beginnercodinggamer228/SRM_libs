using System;
using UnityEngine;

// Token: 0x02000209 RID: 521
public class HideOnStart : SRBehaviour
{
	// Token: 0x06000B02 RID: 2818 RVA: 0x0002E9A1 File Offset: 0x0002CBA1
	private void Start()
	{
		base.GetComponent<Renderer>().enabled = false;
	}
}
