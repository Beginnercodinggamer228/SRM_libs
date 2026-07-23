using System;

// Token: 0x02000015 RID: 21
public class FPControllerState : BaseState
{
	// Token: 0x06000055 RID: 85 RVA: 0x00003267 File Offset: 0x00001467
	public FPControllerState(string name) : base(name)
	{
	}

	// Token: 0x0400004C RID: 76
	public float? MotorAcceleration;

	// Token: 0x0400004D RID: 77
	public float? MotorBackwardsSpeed;

	// Token: 0x0400004E RID: 78
	public float? MotorDamping;

	// Token: 0x0400004F RID: 79
	public float? MotorAirSpeed;

	// Token: 0x04000050 RID: 80
	public float? MotorSlopeSpeedUp;

	// Token: 0x04000051 RID: 81
	public float? MotorSlopeSpeedDown;

	// Token: 0x04000052 RID: 82
	public bool? MotorFreeFly;

	// Token: 0x04000053 RID: 83
	public float? MotorJumpForce;

	// Token: 0x04000054 RID: 84
	public float? MotorJumpForceDamping;

	// Token: 0x04000055 RID: 85
	public float? MotorJumpForceHold;

	// Token: 0x04000056 RID: 86
	public float? MotorJumpForceHoldDamping;

	// Token: 0x04000057 RID: 87
	public float? PhysicsForceDamping;

	// Token: 0x04000058 RID: 88
	public float? PhysicsPushForce;

	// Token: 0x04000059 RID: 89
	public float? PhysicsGravityModifier;

	// Token: 0x0400005A RID: 90
	public float? PhysicsSlopeSlideLimit;

	// Token: 0x0400005B RID: 91
	public float? PhysicsSlopeSlidiness;

	// Token: 0x0400005C RID: 92
	public float? PhysicsWallBounce;

	// Token: 0x0400005D RID: 93
	public float? PhysicsWallFriction;

	// Token: 0x0400005E RID: 94
	public bool? PhysicsHasCollisionTrigger;
}
