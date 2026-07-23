using System;
using UnityEngine;

// Token: 0x020003B4 RID: 948
public class DeactivateWhileStealthed : MonoBehaviour
{
	// Token: 0x060013C8 RID: 5064 RVA: 0x0004CC20 File Offset: 0x0004AE20
	public void Start()
	{
		this.particleSys = base.GetComponent<ParticleSystem>();
		this.stealth = base.GetComponentInParent<SlimeStealth>();
	}

	// Token: 0x060013C9 RID: 5065 RVA: 0x0004CC3C File Offset: 0x0004AE3C
	public void Update()
	{
		if (this.stealth != null)
		{
			this.particleSys.emission.enabled = !this.stealth.IsStealthed;
		}
	}

	// Token: 0x04001288 RID: 4744
	private ParticleSystem particleSys;

	// Token: 0x04001289 RID: 4745
	private SlimeStealth stealth;
}
