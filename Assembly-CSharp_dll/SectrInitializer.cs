using System;
using UnityEngine;

// Token: 0x0200077C RID: 1916
public class SectrInitializer : MonoBehaviour
{
	// Token: 0x0600281A RID: 10266 RVA: 0x00097F90 File Offset: 0x00096190
	public void Start()
	{
		SECTR_Chunk component = base.GetComponent<SECTR_Chunk>();
		component.SetRoot(this.sectorRoot);
		component.CheckReferences();
	}

	// Token: 0x040027A5 RID: 10149
	public GameObject sectorRoot;
}
