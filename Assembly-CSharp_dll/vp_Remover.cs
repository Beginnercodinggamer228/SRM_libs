using System;
using UnityEngine;

// Token: 0x0200085D RID: 2141
public class vp_Remover : MonoBehaviour
{
	// Token: 0x06002D2D RID: 11565 RVA: 0x000ABA11 File Offset: 0x000A9C11
	private void OnEnable()
	{
		vp_Timer.In(Mathf.Max(this.LifeTime, 0.1f), delegate()
		{
			vp_Utility.Destroy(base.gameObject);
		}, this.m_DestroyTimer);
	}

	// Token: 0x06002D2E RID: 11566 RVA: 0x000ABA3A File Offset: 0x000A9C3A
	private void OnDisable()
	{
		this.m_DestroyTimer.Cancel();
	}

	// Token: 0x04002B3D RID: 11069
	public float LifeTime = 10f;

	// Token: 0x04002B3E RID: 11070
	protected vp_Timer.Handle m_DestroyTimer = new vp_Timer.Handle();
}
