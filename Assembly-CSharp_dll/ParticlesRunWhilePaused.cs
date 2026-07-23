using System;
using UnityEngine;

// Token: 0x020005D8 RID: 1496
public class ParticlesRunWhilePaused : MonoBehaviour
{
	// Token: 0x06001F5A RID: 8026 RVA: 0x00077553 File Offset: 0x00075753
	public void Awake()
	{
		this.uiParticleSystem = base.GetComponent<ParticleSystem>();
	}

	// Token: 0x06001F5B RID: 8027 RVA: 0x00077561 File Offset: 0x00075761
	public void Update()
	{
		if (Time.timeScale < 0.01f)
		{
			this.uiParticleSystem.Simulate(Time.unscaledDeltaTime, false, false);
			this.uiParticleSystem.Play();
		}
	}

	// Token: 0x04001E8D RID: 7821
	private ParticleSystem uiParticleSystem;
}
