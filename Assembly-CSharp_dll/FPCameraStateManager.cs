using System;
using UnityEngine;

// Token: 0x02000014 RID: 20
public class FPCameraStateManager : BaseStateManager<FPCameraState, vp_FPCamera>
{
	// Token: 0x06000052 RID: 82 RVA: 0x00003298 File Offset: 0x00001498
	public FPCameraStateManager(vp_FPCamera managedComponent) : base(managedComponent)
	{
		this.CreateStates();
	}

	// Token: 0x06000053 RID: 83 RVA: 0x000032A8 File Offset: 0x000014A8
	private void CreateStates()
	{
		this.states = new FPCameraState[1];
		base.AddState(new FPCameraState("Default")
		{
			RenderingFieldOfView = new float?(60f),
			RenderingZoomDamping = new float?(0.2f),
			PositionOffset = new Vector3?(new Vector3(0f, 1.75f, 0.1f)),
			PositionGroundLimit = new float?(0.1f),
			PositionSpringStiffness = new float?(0.01f),
			PositionSpringDamping = new float?(0.25f),
			PositionSpring2Stiffness = new float?(0.95f),
			PositionSpring2Damping = new float?(0.25f),
			PositionKneeling = new float?(0.025f),
			PositionKneelingSoftness = new int?(1),
			RotationPitchLimit = new Vector2?(new Vector2(90f, -90f)),
			RotationYawLimit = new Vector2?(new Vector2(-360f, 360f)),
			RotationSpringStiffness = new float?(0.01f),
			RotationSpringDamping = new float?(0.25f),
			RotationKneeling = new float?(0.025f),
			RotationKneelingSoftness = new int?(1),
			RotationStrafeRoll = new float?(0.01f),
			ShakeSpeed = new float?(0f),
			ShakeAmplitude = new Vector3?(new Vector3(10f, 10f, 0f)),
			BobRate = new Vector4?(new Vector4(0f, 1.4f, 0f, 0.7f)),
			BobAmplitude = new Vector4?(new Vector4(0f, 0.25f, 0f, 0.5f)),
			BobInputVelocityScale = new int?(1),
			BobMaxInputVelocity = new int?(100),
			BobStepThreshold = new int?(10)
		}, 0);
	}

	// Token: 0x06000054 RID: 84 RVA: 0x0000349C File Offset: 0x0000169C
	public override void ApplyState(FPCameraState state)
	{
		if (state.RenderingFieldOfView != null)
		{
			this.managedComponent.RenderingFieldOfView = state.RenderingFieldOfView.Value;
		}
		if (state.RenderingZoomDamping != null)
		{
			this.managedComponent.RenderingZoomDamping = state.RenderingZoomDamping.Value;
		}
		if (state.PositionOffset != null)
		{
			this.managedComponent.PositionOffset = state.PositionOffset.Value;
		}
		if (state.PositionGroundLimit != null)
		{
			this.managedComponent.PositionGroundLimit = state.PositionGroundLimit.Value;
		}
		if (state.PositionSpringStiffness != null)
		{
			this.managedComponent.PositionSpringStiffness = state.PositionSpringStiffness.Value;
		}
		if (state.PositionSpringDamping != null)
		{
			this.managedComponent.PositionSpringDamping = state.PositionSpringDamping.Value;
		}
		if (state.PositionSpring2Stiffness != null)
		{
			this.managedComponent.PositionSpring2Stiffness = state.PositionSpring2Stiffness.Value;
		}
		if (state.PositionSpring2Damping != null)
		{
			this.managedComponent.PositionSpring2Damping = state.PositionSpring2Damping.Value;
		}
		if (state.PositionKneeling != null)
		{
			this.managedComponent.PositionKneeling = state.PositionKneeling.Value;
		}
		if (state.PositionKneelingSoftness != null)
		{
			this.managedComponent.PositionKneelingSoftness = state.PositionKneelingSoftness.Value;
		}
		if (state.PositionEarthQuakeFactor != null)
		{
			this.managedComponent.PositionEarthQuakeFactor = (float)state.PositionEarthQuakeFactor.Value;
		}
		if (state.RotationPitchLimit != null)
		{
			this.managedComponent.RotationPitchLimit = state.RotationPitchLimit.Value;
		}
		if (state.RotationYawLimit != null)
		{
			this.managedComponent.RotationYawLimit = state.RotationYawLimit.Value;
		}
		if (state.RotationSpringStiffness != null)
		{
			this.managedComponent.RotationSpringStiffness = state.RotationSpringStiffness.Value;
		}
		if (state.RotationSpringDamping != null)
		{
			this.managedComponent.RotationSpringDamping = state.RotationSpringDamping.Value;
		}
		if (state.RotationKneeling != null)
		{
			this.managedComponent.RotationKneeling = state.RotationKneeling.Value;
		}
		if (state.RotationKneelingSoftness != null)
		{
			this.managedComponent.RotationKneelingSoftness = state.RotationKneelingSoftness.Value;
		}
		if (state.RotationStrafeRoll != null)
		{
			this.managedComponent.RotationStrafeRoll = state.RotationStrafeRoll.Value;
		}
		if (state.RotationEarthQuakeFactor != null)
		{
			this.managedComponent.RotationEarthQuakeFactor = (float)state.RotationEarthQuakeFactor.Value;
		}
		if (state.ShakeSpeed != null)
		{
			this.managedComponent.ShakeSpeed = state.ShakeSpeed.Value;
		}
		if (state.ShakeAmplitude != null)
		{
			this.managedComponent.ShakeAmplitude = state.ShakeAmplitude.Value;
		}
		if (state.BobRate != null)
		{
			this.managedComponent.BobRate = state.BobRate.Value;
		}
		if (state.BobInputVelocityScale != null)
		{
			this.managedComponent.BobInputVelocityScale = (float)state.BobInputVelocityScale.Value;
		}
		if (state.BobMaxInputVelocity != null)
		{
			this.managedComponent.BobMaxInputVelocity = (float)state.BobMaxInputVelocity.Value;
		}
		if (state.BobRequireGroundContact != null)
		{
			this.managedComponent.BobRequireGroundContact = state.BobRequireGroundContact.Value;
		}
		if (state.BobStepThreshold != null)
		{
			this.managedComponent.BobStepThreshold = (float)state.BobStepThreshold.Value;
		}
	}
}
