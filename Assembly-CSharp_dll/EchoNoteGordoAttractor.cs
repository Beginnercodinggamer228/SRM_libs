using System;
using UnityEngine;

// Token: 0x020001DC RID: 476
public class EchoNoteGordoAttractor : Attractor
{
	// Token: 0x06000A00 RID: 2560 RVA: 0x0002C215 File Offset: 0x0002A415
	public void Awake()
	{
		base.SetAweFactor(this.attractionFactor);
	}

	// Token: 0x06000A01 RID: 2561 RVA: 0x00013CC5 File Offset: 0x00011EC5
	public override bool CauseMoveTowards()
	{
		return true;
	}

	// Token: 0x04000848 RID: 2120
	[Tooltip("Factor applied to the slimes to determine aweness.")]
	[Range(0f, 1f)]
	public float attractionFactor;
}
