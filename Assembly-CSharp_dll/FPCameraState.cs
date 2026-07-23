using System;
using UnityEngine;

// Token: 0x02000013 RID: 19
public class FPCameraState : BaseState
{
	// Token: 0x06000051 RID: 81 RVA: 0x00003267 File Offset: 0x00001467
	public FPCameraState(string name) : base(name)
	{
	}

	// Token: 0x04000031 RID: 49
	public float? RenderingFieldOfView;

	// Token: 0x04000032 RID: 50
	public float? RenderingZoomDamping;

	// Token: 0x04000033 RID: 51
	public Vector3? PositionOffset;

	// Token: 0x04000034 RID: 52
	public float? PositionGroundLimit;

	// Token: 0x04000035 RID: 53
	public float? PositionSpringStiffness;

	// Token: 0x04000036 RID: 54
	public float? PositionSpringDamping;

	// Token: 0x04000037 RID: 55
	public float? PositionSpring2Stiffness;

	// Token: 0x04000038 RID: 56
	public float? PositionSpring2Damping;

	// Token: 0x04000039 RID: 57
	public float? PositionKneeling;

	// Token: 0x0400003A RID: 58
	public int? PositionKneelingSoftness;

	// Token: 0x0400003B RID: 59
	public int? PositionEarthQuakeFactor;

	// Token: 0x0400003C RID: 60
	public Vector2? RotationPitchLimit;

	// Token: 0x0400003D RID: 61
	public Vector2? RotationYawLimit;

	// Token: 0x0400003E RID: 62
	public float? RotationSpringStiffness;

	// Token: 0x0400003F RID: 63
	public float? RotationSpringDamping;

	// Token: 0x04000040 RID: 64
	public float? RotationKneeling;

	// Token: 0x04000041 RID: 65
	public int? RotationKneelingSoftness;

	// Token: 0x04000042 RID: 66
	public float? RotationStrafeRoll;

	// Token: 0x04000043 RID: 67
	public int? RotationEarthQuakeFactor;

	// Token: 0x04000044 RID: 68
	public float? ShakeSpeed;

	// Token: 0x04000045 RID: 69
	public Vector3? ShakeAmplitude;

	// Token: 0x04000046 RID: 70
	public Vector4? BobRate;

	// Token: 0x04000047 RID: 71
	public Vector4? BobAmplitude;

	// Token: 0x04000048 RID: 72
	public int? BobInputVelocityScale;

	// Token: 0x04000049 RID: 73
	public int? BobMaxInputVelocity;

	// Token: 0x0400004A RID: 74
	public bool? BobRequireGroundContact;

	// Token: 0x0400004B RID: 75
	public int? BobStepThreshold;
}
