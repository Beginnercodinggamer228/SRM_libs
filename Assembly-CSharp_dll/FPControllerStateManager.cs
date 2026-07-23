using System;

// Token: 0x02000016 RID: 22
public class FPControllerStateManager : BaseStateManager<FPControllerState, vp_FPController>
{
	// Token: 0x06000056 RID: 86 RVA: 0x0000383C File Offset: 0x00001A3C
	public FPControllerStateManager(vp_FPController managedComponent) : base(managedComponent)
	{
		this.CreateStates();
	}

	// Token: 0x06000057 RID: 87 RVA: 0x0000384C File Offset: 0x00001A4C
	private void CreateStates()
	{
		this.states = new FPControllerState[9];
		base.AddState(new FPControllerState("Dead")
		{
			PhysicsForceDamping = new float?(0.07f),
			PhysicsGravityModifier = new float?(0.06f),
			PhysicsWallBounce = new float?(1f)
		}, 0);
		base.AddState(new FPControllerState("Freeze")
		{
			MotorAcceleration = new float?(0f),
			MotorJumpForce = new float?(0f),
			MotorAirSpeed = new float?(0f),
			MotorSlopeSpeedUp = new float?(0f),
			MotorSlopeSpeedDown = new float?(0f),
			PhysicsForceDamping = new float?(1f),
			PhysicsPushForce = new float?(0f),
			PhysicsGravityModifier = new float?(0f),
			PhysicsWallBounce = new float?(0f)
		}, 1);
		base.AddState(new FPControllerState("Zoom")
		{
			MotorDamping = new float?(0.17f),
			MotorAcceleration = new float?(0.18f)
		}, 2);
		base.AddState(new FPControllerState("Crouch")
		{
			MotorAcceleration = new float?(0.084f),
			MotorDamping = new float?(0.35f),
			MotorJumpForce = new float?(0f),
			MotorAirSpeed = new float?(0f),
			PhysicsForceDamping = new float?(0.05f),
			PhysicsPushForce = new float?(5f),
			PhysicsGravityModifier = new float?(0.2f),
			PhysicsWallBounce = new float?(0f)
		}, 3);
		base.AddState(new FPControllerState("Run")
		{
			MotorAcceleration = new float?(0.45f),
			MotorAirSpeed = new float?(0.9f)
		}, 4);
		base.AddState(new FPControllerState("Jetpack1")
		{
			PhysicsGravityModifier = new float?(-0.06f)
		}, 5);
		base.AddState(new FPControllerState("Jetpack2")
		{
			PhysicsGravityModifier = new float?(-0.06f)
		}, 6);
		base.AddState(new FPControllerState("Underwater")
		{
			MotorDamping = new float?(0.35f),
			PhysicsGravityModifier = new float?(0.1f),
			MotorJumpForce = new float?(0.09f),
			MotorJumpForceDamping = new float?(0.05f),
			MotorJumpForceHold = new float?(0.0015f)
		}, 7);
		base.AddState(new FPControllerState("Default")
		{
			MotorAcceleration = new float?(0.25f),
			MotorDamping = new float?(0.15f),
			MotorBackwardsSpeed = new float?(0.65f),
			MotorAirSpeed = new float?(0.9f),
			MotorSlopeSpeedUp = new float?(1f),
			MotorSlopeSpeedDown = new float?(1f),
			MotorJumpForce = new float?(0.18f),
			MotorJumpForceDamping = new float?(0.08f),
			MotorJumpForceHold = new float?(0.003f),
			MotorJumpForceHoldDamping = new float?(0.5f),
			PhysicsForceDamping = new float?(0.2f),
			PhysicsPushForce = new float?(5f),
			PhysicsGravityModifier = new float?(0.2f),
			PhysicsSlopeSlideLimit = new float?(60f),
			PhysicsSlopeSlidiness = new float?(0.15f),
			PhysicsWallBounce = new float?(0f),
			PhysicsWallFriction = new float?(0f),
			PhysicsHasCollisionTrigger = new bool?(true)
		}, 8);
	}

	// Token: 0x06000058 RID: 88 RVA: 0x00003C20 File Offset: 0x00001E20
	public override void ApplyState(FPControllerState state)
	{
		if (state.MotorAcceleration != null)
		{
			this.managedComponent.MotorAcceleration = state.MotorAcceleration.Value;
		}
		if (state.MotorBackwardsSpeed != null)
		{
			this.managedComponent.MotorBackwardsSpeed = state.MotorBackwardsSpeed.Value;
		}
		if (state.MotorDamping != null)
		{
			this.managedComponent.MotorDamping = state.MotorDamping.Value;
		}
		if (state.MotorAirSpeed != null)
		{
			this.managedComponent.MotorAirSpeed = state.MotorAirSpeed.Value;
		}
		if (state.MotorSlopeSpeedUp != null)
		{
			this.managedComponent.MotorSlopeSpeedUp = state.MotorSlopeSpeedUp.Value;
		}
		if (state.MotorSlopeSpeedDown != null)
		{
			this.managedComponent.MotorSlopeSpeedDown = state.MotorSlopeSpeedDown.Value;
		}
		if (state.MotorJumpForce != null)
		{
			this.managedComponent.MotorJumpForce = state.MotorJumpForce.Value;
		}
		if (state.MotorJumpForceDamping != null)
		{
			this.managedComponent.MotorJumpForceDamping = state.MotorJumpForceDamping.Value;
		}
		if (state.MotorJumpForceHold != null)
		{
			this.managedComponent.MotorJumpForceHold = state.MotorJumpForceHold.Value;
		}
		if (state.MotorJumpForceHoldDamping != null)
		{
			this.managedComponent.MotorJumpForceHoldDamping = state.MotorJumpForceHoldDamping.Value;
		}
		if (state.PhysicsForceDamping != null)
		{
			this.managedComponent.PhysicsForceDamping = state.PhysicsForceDamping.Value;
		}
		if (state.PhysicsPushForce != null)
		{
			this.managedComponent.PhysicsPushForce = state.PhysicsPushForce.Value;
		}
		if (state.PhysicsGravityModifier != null)
		{
			this.managedComponent.PhysicsGravityModifier = state.PhysicsGravityModifier.Value;
		}
		if (state.PhysicsSlopeSlideLimit != null)
		{
			this.managedComponent.PhysicsSlopeSlideLimit = state.PhysicsSlopeSlideLimit.Value;
		}
		if (state.PhysicsSlopeSlidiness != null)
		{
			this.managedComponent.PhysicsSlopeSlidiness = state.PhysicsSlopeSlidiness.Value;
		}
		if (state.PhysicsWallBounce != null)
		{
			this.managedComponent.PhysicsWallBounce = state.PhysicsWallBounce.Value;
		}
		if (state.PhysicsWallFriction != null)
		{
			this.managedComponent.PhysicsWallFriction = state.PhysicsWallFriction.Value;
		}
		if (state.PhysicsHasCollisionTrigger != null)
		{
			this.managedComponent.PhysicsHasCollisionTrigger = state.PhysicsHasCollisionTrigger.Value;
		}
	}
}
