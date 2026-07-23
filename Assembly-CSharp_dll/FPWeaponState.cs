using System;
using UnityEngine;

// Token: 0x02000019 RID: 25
public class FPWeaponState : BaseState
{
	// Token: 0x0600005D RID: 93 RVA: 0x00003267 File Offset: 0x00001467
	public FPWeaponState(string name) : base(name)
	{
	}

	// Token: 0x04000068 RID: 104
	public float? RenderingZoomDamping;

	// Token: 0x04000069 RID: 105
	public int? RenderingFieldOfView;

	// Token: 0x0400006A RID: 106
	public Vector2? RenderingClippingPlanes;

	// Token: 0x0400006B RID: 107
	public int? RenderingZScale;

	// Token: 0x0400006C RID: 108
	public Vector3? PositionOffset;

	// Token: 0x0400006D RID: 109
	public float? PositionSpringStiffness;

	// Token: 0x0400006E RID: 110
	public float? PositionSpringDamping;

	// Token: 0x0400006F RID: 111
	public int? PositionFallRetract;

	// Token: 0x04000070 RID: 112
	public float? PositionPivotSpringStiffness;

	// Token: 0x04000071 RID: 113
	public float? PositionPivotSpringDamping;

	// Token: 0x04000072 RID: 114
	public float? PositionKneeling;

	// Token: 0x04000073 RID: 115
	public int? PositionKneelingSoftness;

	// Token: 0x04000074 RID: 116
	public Vector3? PositionWalkSlide;

	// Token: 0x04000075 RID: 117
	public Vector3? PositionPivot;

	// Token: 0x04000076 RID: 118
	public Vector3? RotationPivot;

	// Token: 0x04000077 RID: 119
	public float? PositionInputVelocityScale;

	// Token: 0x04000078 RID: 120
	public int? PositionMaxInputVelocity;

	// Token: 0x04000079 RID: 121
	public float? RotationSpringStiffness;

	// Token: 0x0400007A RID: 122
	public float? RotationSpringDamping;

	// Token: 0x0400007B RID: 123
	public float? RotationPivotSpringStiffness;

	// Token: 0x0400007C RID: 124
	public float? RotationPivotSpringDamping;

	// Token: 0x0400007D RID: 125
	public int? RotationKneeling;

	// Token: 0x0400007E RID: 126
	public int? RotationKneelingSoftness;

	// Token: 0x0400007F RID: 127
	public Vector3? RotationLookSway;

	// Token: 0x04000080 RID: 128
	public Vector3? RotationStrafeSway;

	// Token: 0x04000081 RID: 129
	public Vector3? RotationFallSway;

	// Token: 0x04000082 RID: 130
	public float? RotationSlopeSway;

	// Token: 0x04000083 RID: 131
	public float? PositionSpring2Stiffness;

	// Token: 0x04000084 RID: 132
	public float? PositionSpring2Damping;

	// Token: 0x04000085 RID: 133
	public Vector3? RotationOffset;

	// Token: 0x04000086 RID: 134
	public float? RotationSpring2Stiffness;

	// Token: 0x04000087 RID: 135
	public float? RotationSpring2Damping;

	// Token: 0x04000088 RID: 136
	public int? RotationInputVelocityScale;

	// Token: 0x04000089 RID: 137
	public int? RotationMaxInputVelocity;

	// Token: 0x0400008A RID: 138
	public float? RetractionDistance;

	// Token: 0x0400008B RID: 139
	public Vector2? RetractionOffset;

	// Token: 0x0400008C RID: 140
	public float? RetractionRelaxSpeed;

	// Token: 0x0400008D RID: 141
	public float? ShakeSpeed;

	// Token: 0x0400008E RID: 142
	public Vector3? ShakeAmplitude;

	// Token: 0x0400008F RID: 143
	public Vector4? BobRate;

	// Token: 0x04000090 RID: 144
	public Vector4? BobAmplitude;

	// Token: 0x04000091 RID: 145
	public float? BobInputVelocityScale;

	// Token: 0x04000092 RID: 146
	public int? BobMaxInputVelocity;

	// Token: 0x04000093 RID: 147
	public bool? BobRequireGroundContact;

	// Token: 0x04000094 RID: 148
	public Vector3? StepPositionForce;

	// Token: 0x04000095 RID: 149
	public Vector3? StepRotationForce;

	// Token: 0x04000096 RID: 150
	public int? StepSoftness;

	// Token: 0x04000097 RID: 151
	public int? StepMinVelocity;

	// Token: 0x04000098 RID: 152
	public int? StepPositionBalance;

	// Token: 0x04000099 RID: 153
	public int? StepRotationBalance;

	// Token: 0x0400009A RID: 154
	public float? StepForceScale;

	// Token: 0x0400009B RID: 155
	public Vector2? AmbientInterval;

	// Token: 0x0400009C RID: 156
	public Vector3? PositionExitOffset;

	// Token: 0x0400009D RID: 157
	public Vector3? RotationExitOffset;

	// Token: 0x0400009E RID: 158
	public bool? LookDownActive;

	// Token: 0x0400009F RID: 159
	public float? LookDownYawLimit;

	// Token: 0x040000A0 RID: 160
	public Vector3? LookDownPositionOffsetMiddle;

	// Token: 0x040000A1 RID: 161
	public Vector3? LookDownPositionOffsetLeft;

	// Token: 0x040000A2 RID: 162
	public Vector3? LookDownPositionOffsetRight;

	// Token: 0x040000A3 RID: 163
	public float? LookDownPositionSpringPower;

	// Token: 0x040000A4 RID: 164
	public Vector3? LookDownRotationOffsetMiddle;

	// Token: 0x040000A5 RID: 165
	public Vector3? LookDownRotationOffsetLeft;

	// Token: 0x040000A6 RID: 166
	public Vector3? LookDownRotationOffsetRight;

	// Token: 0x040000A7 RID: 167
	public float? LookDownRotationSpringPower;

	// Token: 0x040000A8 RID: 168
	public int? AnimationType;

	// Token: 0x040000A9 RID: 169
	public int? AnimationGrip;

	// Token: 0x040000AA RID: 170
	public bool? Persist;
}
