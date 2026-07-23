using System;
using UnityEngine;

// Token: 0x02000057 RID: 87
public class DemoReactivator : MonoBehaviour
{
	// Token: 0x0600017B RID: 379 RVA: 0x0000B3F2 File Offset: 0x000095F2
	private void Start()
	{
		base.InvokeRepeating("Reactivate", this.TimeDelayToReactivate, this.TimeDelayToReactivate);
	}

	// Token: 0x0600017C RID: 380 RVA: 0x0000B40B File Offset: 0x0000960B
	private void Reactivate()
	{
		base.gameObject.SetActive(false);
		base.gameObject.SetActive(true);
	}

	// Token: 0x040001AE RID: 430
	public float TimeDelayToReactivate = 3f;
}
