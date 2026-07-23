using System;
using UnityEngine;

// Token: 0x02000628 RID: 1576
public class SwitchOnGamepadDetected : MonoBehaviour
{
	// Token: 0x06002114 RID: 8468 RVA: 0x0007E71C File Offset: 0x0007C91C
	private void Update()
	{
		bool flag = InputDirector.UsingGamepad();
		this.showOnGamepadDetected.SetActive(flag);
		this.showOnGamepadNotDetected.SetActive(!flag);
	}

	// Token: 0x04002072 RID: 8306
	public GameObject showOnGamepadDetected;

	// Token: 0x04002073 RID: 8307
	public GameObject showOnGamepadNotDetected;
}
