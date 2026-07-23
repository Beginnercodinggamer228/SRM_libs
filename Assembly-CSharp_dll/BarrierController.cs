using System;
using UnityEngine;

// Token: 0x020006C8 RID: 1736
public class BarrierController : MonoBehaviour
{
	// Token: 0x0600242B RID: 9259 RVA: 0x0008B994 File Offset: 0x00089B94
	public void SetIsOpen(bool isOpen)
	{
		if (this.barrier == null)
		{
			this.barrier = base.transform.Find("Barrier").gameObject;
		}
		if (this.barrier != null)
		{
			this.barrier.SetActive(!isOpen);
		}
	}

	// Token: 0x0400233B RID: 9019
	private GameObject barrier;

	// Token: 0x0400233C RID: 9020
	private const string BARRIER_NAME = "Barrier";
}
