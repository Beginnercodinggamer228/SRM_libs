using System;
using UnityEngine;

// Token: 0x02000067 RID: 103
public class CFX_Demo_RandomDir : MonoBehaviour
{
	// Token: 0x060001CF RID: 463 RVA: 0x0000E0EC File Offset: 0x0000C2EC
	private void Start()
	{
		base.transform.eulerAngles = new Vector3(UnityEngine.Random.Range(this.min.x, this.max.x), UnityEngine.Random.Range(this.min.y, this.max.y), UnityEngine.Random.Range(this.min.z, this.max.z));
	}

	// Token: 0x0400022C RID: 556
	public Vector3 min = new Vector3(0f, 0f, 0f);

	// Token: 0x0400022D RID: 557
	public Vector3 max = new Vector3(0f, 360f, 0f);
}
