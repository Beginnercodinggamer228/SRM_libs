using System;
using UnityEngine;

// Token: 0x0200055E RID: 1374
public class CreditScrollUI : MonoBehaviour
{
	// Token: 0x06001CAE RID: 7342 RVA: 0x00003296 File Offset: 0x00001496
	public void OnEnable()
	{
	}

	// Token: 0x06001CAF RID: 7343 RVA: 0x0006D35C File Offset: 0x0006B55C
	public void OnDisable()
	{
		SRSingleton<GameContext>.Instance != null;
	}
}
