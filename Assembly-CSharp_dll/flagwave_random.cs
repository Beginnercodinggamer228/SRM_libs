using System;
using UnityEngine;

// Token: 0x0200002C RID: 44
public class flagwave_random : MonoBehaviour
{
	// Token: 0x060000A9 RID: 169 RVA: 0x0000582C File Offset: 0x00003A2C
	public void OnEnable()
	{
		Animation component = base.GetComponent<Animation>();
		component["flagwave_loop"].normalizedTime = (float)Randoms.SHARED.GetInRange(0, 1);
		component["flagwave_loop"].normalizedSpeed = Randoms.SHARED.GetInRange(0.16f, 0.18f);
	}
}
