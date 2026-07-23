using System;
using UnityEngine;

// Token: 0x02000093 RID: 147
[ExecuteInEditMode]
[AddComponentMenu("SECTR/Audio/SECTR Trigger Source")]
public class SECTR_TriggerSource : SECTR_PointSource
{
	// Token: 0x0600030D RID: 781 RVA: 0x000139AA File Offset: 0x00011BAA
	public SECTR_TriggerSource()
	{
		this.Loop = false;
		this.PlayOnStart = false;
	}

	// Token: 0x0600030E RID: 782 RVA: 0x000139C0 File Offset: 0x00011BC0
	protected override void OnEnable()
	{
		base.OnEnable();
		if (!this.IsPlaying && this.activator)
		{
			this.Play();
		}
	}

	// Token: 0x0600030F RID: 783 RVA: 0x000139E3 File Offset: 0x00011BE3
	private void OnTriggerEnter(Collider other)
	{
		if (this.activator == null)
		{
			this.Play();
			this.activator = other;
		}
	}

	// Token: 0x06000310 RID: 784 RVA: 0x00013A00 File Offset: 0x00011C00
	private void OnTriggerExit(Collider other)
	{
		if (this.activator == other)
		{
			this.Stop(false);
			this.activator = null;
		}
	}

	// Token: 0x0400032C RID: 812
	private Collider activator;
}
