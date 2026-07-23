using System;
using UnityEngine;

// Token: 0x02000017 RID: 23
public class FPInputState : BaseState
{
	// Token: 0x06000059 RID: 89 RVA: 0x00003267 File Offset: 0x00001467
	public FPInputState(string name) : base(name)
	{
	}

	// Token: 0x0400005F RID: 95
	public Vector2? MouseLookSensitivity;

	// Token: 0x04000060 RID: 96
	public int? MouseLookSmoothSteps;

	// Token: 0x04000061 RID: 97
	public bool? MouseLookAcceleration;

	// Token: 0x04000062 RID: 98
	public float? MouseLookSmoothWeight;

	// Token: 0x04000063 RID: 99
	public float? MouseLookAccelerationThreshold;

	// Token: 0x04000064 RID: 100
	public bool? MouseLookInvert;

	// Token: 0x04000065 RID: 101
	public bool? MouseCursorForced;

	// Token: 0x04000066 RID: 102
	public bool? MouseCursorBlocksMouseLook;

	// Token: 0x04000067 RID: 103
	public bool? Persist;
}
