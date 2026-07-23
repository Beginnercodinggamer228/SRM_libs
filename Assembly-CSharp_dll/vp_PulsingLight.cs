using System;
using UnityEngine;

// Token: 0x02000850 RID: 2128
public class vp_PulsingLight : MonoBehaviour
{
	// Token: 0x06002CDA RID: 11482 RVA: 0x000A941B File Offset: 0x000A761B
	private void Start()
	{
		this.m_Light = base.GetComponent<Light>();
	}

	// Token: 0x06002CDB RID: 11483 RVA: 0x000A942C File Offset: 0x000A762C
	private void Update()
	{
		if (this.m_Light == null)
		{
			return;
		}
		this.m_Light.intensity = this.m_MinIntensity + Mathf.Abs(Mathf.Cos(Time.time * this.m_Rate) * (this.m_MaxIntensity - this.m_MinIntensity));
	}

	// Token: 0x04002AC1 RID: 10945
	private Light m_Light;

	// Token: 0x04002AC2 RID: 10946
	public float m_MinIntensity = 2f;

	// Token: 0x04002AC3 RID: 10947
	public float m_MaxIntensity = 5f;

	// Token: 0x04002AC4 RID: 10948
	public float m_Rate = 1f;
}
