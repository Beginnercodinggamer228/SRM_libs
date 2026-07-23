using System;
using UnityEngine;

// Token: 0x0200001A RID: 26
public class FPWeaponStateManager : BaseStateManager<FPWeaponState, vp_FPWeapon>
{
	// Token: 0x0600005E RID: 94 RVA: 0x0000401C File Offset: 0x0000221C
	public FPWeaponStateManager(vp_FPWeapon managedComponent) : base(managedComponent)
	{
		this.CreateStates();
	}

	// Token: 0x0600005F RID: 95 RVA: 0x0000402C File Offset: 0x0000222C
	private void CreateStates()
	{
		this.states = new FPWeaponState[4];
		base.AddState(new FPWeaponState("Zoom")
		{
			ShakeSpeed = new float?(0.05f),
			ShakeAmplitude = new Vector3?(new Vector3(0.5f, 0f, 0f)),
			PositionOffset = new Vector3?(new Vector3(0f, -0.25f, 0.17f)),
			RotationOffset = new Vector3?(new Vector3(0.4917541f, 0.015994f, 0f)),
			PositionSpringStiffness = new float?(0.055f),
			PositionSpringDamping = new float?(0.45f),
			RotationSpringStiffness = new float?(0.025f),
			RotationSpringDamping = new float?(0.35f),
			RenderingFieldOfView = new int?(35),
			RenderingZoomDamping = new float?(0.2f),
			BobAmplitude = new Vector4?(new Vector4(0.5f, 0.4f, 0.2f, 0.005f)),
			BobRate = new Vector4?(new Vector4(0.8f, -0.4f, 0.4f, 0.4f)),
			BobInputVelocityScale = new float?((float)15),
			PositionWalkSlide = new Vector3?(new Vector3(0.2f, 0.5f, 0.2f))
		}, 0);
		FPWeaponState state = new FPWeaponState("Attack");
		base.AddState(state, 1);
		base.AddState(new FPWeaponState("Run")
		{
			BobAmplitude = new Vector4?(new Vector4(1.5f, 1.2f, 0.6f, 0.015f))
		}, 2);
		base.AddState(new FPWeaponState("Default")
		{
			RenderingZoomDamping = new float?(0.5f),
			RenderingZScale = new int?(1),
			PositionSpringStiffness = new float?(0.01f),
			PositionSpringDamping = new float?(0.25f),
			PositionFallRetract = new int?(1),
			PositionPivotSpringStiffness = new float?(0.1f),
			PositionPivotSpringDamping = new float?(0.5f),
			PositionKneeling = new float?(0.06f),
			PositionKneelingSoftness = new int?(1),
			PositionWalkSlide = new Vector3?(new Vector3(1f, 1f, 1f)),
			PositionPivot = new Vector3?(new Vector3(0f, 0f, -0.2741375f)),
			RotationPivot = new Vector3?(new Vector3(0f, 0f, 0f)),
			PositionInputVelocityScale = new float?(0.5f),
			PositionMaxInputVelocity = new int?(1),
			RotationSpringStiffness = new float?(0.01f),
			RotationSpringDamping = new float?(0.25f),
			RotationPivotSpringStiffness = new float?(0.01f),
			RotationPivotSpringDamping = new float?(0.25f),
			RotationKneeling = new int?(0),
			RotationKneelingSoftness = new int?(1),
			RotationLookSway = new Vector3?(new Vector3(1f, 1f, 0f)),
			RotationStrafeSway = new Vector3?(new Vector3(1f, 1f, -1f)),
			RotationFallSway = new Vector3?(new Vector3(2f, 0f, 0f)),
			RotationSlopeSway = new float?(0.75f),
			RotationInputVelocityScale = new int?(1),
			RotationMaxInputVelocity = new int?(5),
			RetractionDistance = new float?(0.4f),
			RetractionOffset = new Vector2?(new Vector2(0f, 0f)),
			RetractionRelaxSpeed = new float?(0.25f),
			ShakeSpeed = new float?(0.1f),
			ShakeAmplitude = new Vector3?(new Vector3(0.1f, 0f, 0.3f)),
			BobRate = new Vector4?(new Vector4(1.4f, 0.7f, 0.7f, 0.7f)),
			BobAmplitude = new Vector4?(new Vector4(0.5f, 0.4f, 0.2f, 0.005f)),
			BobInputVelocityScale = new float?(3.5f),
			BobMaxInputVelocity = new int?(250),
			BobRequireGroundContact = new bool?(true),
			StepPositionForce = new Vector3?(new Vector3(0f, 0.015f, 0f)),
			StepRotationForce = new Vector3?(new Vector3(1.5f, 0f, 0f)),
			StepSoftness = new int?(5),
			StepMinVelocity = new int?(2),
			StepPositionBalance = new int?(0),
			StepRotationBalance = new int?(0),
			StepForceScale = new float?(0.05f),
			LookDownActive = new bool?(false),
			LookDownYawLimit = new float?((float)60),
			LookDownPositionOffsetMiddle = new Vector3?(new Vector3(0.35f, -0.37f, 0.78f)),
			LookDownPositionOffsetLeft = new Vector3?(new Vector3(0.27f, -0.31f, 0.7f)),
			LookDownPositionOffsetRight = new Vector3?(new Vector3(0.6f, -0.41f, 0.86f)),
			LookDownPositionSpringPower = new float?((float)1),
			LookDownRotationOffsetMiddle = new Vector3?(new Vector3(-3.9f, 2.24f, 4.69f)),
			LookDownRotationOffsetLeft = new Vector3?(new Vector3(-7f, -10.5f, 15.6f)),
			LookDownRotationOffsetRight = new Vector3?(new Vector3(-9.2f, -9.8f, 48.84f)),
			LookDownRotationSpringPower = new float?((float)1),
			AmbientInterval = new Vector2?(new Vector2(0f, 0f)),
			PositionExitOffset = new Vector3?(new Vector3(0f, -1f, 0f)),
			PositionOffset = new Vector3?(new Vector3(0.1493672f, -0.83f, -0.75f)),
			PositionSpring2Stiffness = new float?(0.2f),
			PositionSpring2Damping = new float?(0.5615942f),
			RotationExitOffset = new Vector3?(new Vector3(40f, 0f, 0f)),
			RotationOffset = new Vector3?(new Vector3(-3.158258f, -5f, 0f)),
			RotationSpring2Stiffness = new float?(0.85f),
			RotationSpring2Damping = new float?(0.6f),
			AnimationType = new int?(1),
			AnimationGrip = new int?(1),
			Persist = new bool?(false)
		}, 3);
	}

	// Token: 0x06000060 RID: 96 RVA: 0x00004714 File Offset: 0x00002914
	public override void ApplyState(FPWeaponState state)
	{
		if (state.RenderingZoomDamping != null)
		{
			this.managedComponent.RenderingZoomDamping = state.RenderingZoomDamping.Value;
		}
		if (state.RenderingZScale != null)
		{
			this.managedComponent.RenderingZScale = (float)state.RenderingZScale.Value;
		}
		if (state.PositionOffset != null)
		{
			this.managedComponent.PositionOffset = state.PositionOffset.Value;
		}
		if (state.PositionSpringStiffness != null)
		{
			this.managedComponent.PositionSpringStiffness = state.PositionSpringStiffness.Value;
		}
		if (state.PositionSpringDamping != null)
		{
			this.managedComponent.PositionSpringDamping = state.PositionSpringDamping.Value;
		}
		if (state.PositionFallRetract != null)
		{
			this.managedComponent.PositionFallRetract = (float)state.PositionFallRetract.Value;
		}
		if (state.PositionPivotSpringStiffness != null)
		{
			this.managedComponent.PositionPivotSpringStiffness = state.PositionPivotSpringStiffness.Value;
		}
		if (state.PositionPivotSpringDamping != null)
		{
			this.managedComponent.PositionPivotSpringDamping = state.PositionPivotSpringDamping.Value;
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
		if (state.PositionWalkSlide != null)
		{
			this.managedComponent.PositionWalkSlide = state.PositionWalkSlide.Value;
		}
		if (state.PositionPivot != null)
		{
			this.managedComponent.PositionPivot = state.PositionPivot.Value;
		}
		if (state.RotationPivot != null)
		{
			this.managedComponent.RotationPivot = state.RotationPivot.Value;
		}
		if (state.PositionInputVelocityScale != null)
		{
			this.managedComponent.PositionInputVelocityScale = state.PositionInputVelocityScale.Value;
		}
		if (state.PositionMaxInputVelocity != null)
		{
			this.managedComponent.PositionMaxInputVelocity = (float)state.PositionMaxInputVelocity.Value;
		}
		if (state.RotationOffset != null)
		{
			this.managedComponent.RotationOffset = state.RotationOffset.Value;
		}
		if (state.RotationSpringStiffness != null)
		{
			this.managedComponent.RotationSpringStiffness = state.RotationSpringStiffness.Value;
		}
		if (state.RotationSpringDamping != null)
		{
			this.managedComponent.RotationSpringDamping = state.RotationSpringDamping.Value;
		}
		if (state.RotationPivotSpringStiffness != null)
		{
			this.managedComponent.RotationPivotSpringStiffness = state.RotationPivotSpringStiffness.Value;
		}
		if (state.RotationPivotSpringDamping != null)
		{
			this.managedComponent.RotationPivotSpringDamping = state.RotationPivotSpringDamping.Value;
		}
		if (state.RotationSpring2Stiffness != null)
		{
			this.managedComponent.RotationSpring2Stiffness = state.RotationSpring2Stiffness.Value;
		}
		if (state.RotationSpring2Damping != null)
		{
			this.managedComponent.RotationSpring2Damping = state.RotationSpring2Damping.Value;
		}
		if (state.RotationKneeling != null)
		{
			this.managedComponent.RotationKneeling = (float)state.RotationKneeling.Value;
		}
		if (state.RotationKneelingSoftness != null)
		{
			this.managedComponent.RotationKneelingSoftness = state.RotationKneelingSoftness.Value;
		}
		if (state.RotationLookSway != null)
		{
			this.managedComponent.RotationLookSway = state.RotationLookSway.Value;
		}
		if (state.RotationStrafeSway != null)
		{
			this.managedComponent.RotationStrafeSway = state.RotationStrafeSway.Value;
		}
		if (state.RotationFallSway != null)
		{
			this.managedComponent.RotationFallSway = state.RotationFallSway.Value;
		}
		if (state.RotationSlopeSway != null)
		{
			this.managedComponent.RotationSlopeSway = state.RotationSlopeSway.Value;
		}
		if (state.RotationInputVelocityScale != null)
		{
			this.managedComponent.RotationInputVelocityScale = (float)state.RotationInputVelocityScale.Value;
		}
		if (state.RotationMaxInputVelocity != null)
		{
			this.managedComponent.RotationMaxInputVelocity = (float)state.RotationMaxInputVelocity.Value;
		}
		if (state.RetractionDistance != null)
		{
			this.managedComponent.RetractionDistance = state.RetractionDistance.Value;
		}
		if (state.RetractionOffset != null)
		{
			this.managedComponent.RetractionOffset = state.RetractionOffset.Value;
		}
		if (state.RetractionRelaxSpeed != null)
		{
			this.managedComponent.RetractionRelaxSpeed = state.RetractionRelaxSpeed.Value;
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
		if (state.BobAmplitude != null)
		{
			this.managedComponent.BobAmplitude = state.BobAmplitude.Value;
		}
		if (state.BobInputVelocityScale != null)
		{
			this.managedComponent.BobInputVelocityScale = state.BobInputVelocityScale.Value;
		}
		if (state.BobMaxInputVelocity != null)
		{
			this.managedComponent.BobMaxInputVelocity = (float)state.BobMaxInputVelocity.Value;
		}
		if (state.BobRequireGroundContact != null)
		{
			this.managedComponent.BobRequireGroundContact = state.BobRequireGroundContact.Value;
		}
		if (state.StepPositionForce != null)
		{
			this.managedComponent.StepPositionForce = state.StepPositionForce.Value;
		}
		if (state.StepRotationForce != null)
		{
			this.managedComponent.StepRotationForce = state.StepRotationForce.Value;
		}
		if (state.StepSoftness != null)
		{
			this.managedComponent.StepSoftness = state.StepSoftness.Value;
		}
		if (state.StepMinVelocity != null)
		{
			this.managedComponent.StepMinVelocity = (float)state.StepMinVelocity.Value;
		}
		if (state.StepPositionBalance != null)
		{
			this.managedComponent.StepPositionBalance = (float)state.StepPositionBalance.Value;
		}
		if (state.StepRotationBalance != null)
		{
			this.managedComponent.StepRotationBalance = (float)state.StepRotationBalance.Value;
		}
		if (state.StepForceScale != null)
		{
			this.managedComponent.StepForceScale = state.StepForceScale.Value;
		}
		if (state.AmbientInterval != null)
		{
			this.managedComponent.AmbientInterval = state.AmbientInterval.Value;
		}
		if (state.PositionExitOffset != null)
		{
			this.managedComponent.PositionExitOffset = state.PositionExitOffset.Value;
		}
		if (state.RotationExitOffset != null)
		{
			this.managedComponent.RotationExitOffset = state.RotationExitOffset.Value;
		}
		if (state.LookDownActive != null)
		{
			this.managedComponent.LookDownActive = state.LookDownActive.Value;
		}
		if (state.LookDownYawLimit != null)
		{
			this.managedComponent.LookDownYawLimit = state.LookDownYawLimit.Value;
		}
		if (state.LookDownPositionOffsetMiddle != null)
		{
			this.managedComponent.LookDownPositionOffsetMiddle = state.LookDownPositionOffsetMiddle.Value;
		}
		if (state.LookDownPositionOffsetLeft != null)
		{
			this.managedComponent.LookDownPositionOffsetLeft = state.LookDownPositionOffsetLeft.Value;
		}
		if (state.LookDownPositionOffsetRight != null)
		{
			this.managedComponent.LookDownPositionOffsetRight = state.LookDownPositionOffsetRight.Value;
		}
		if (state.LookDownPositionSpringPower != null)
		{
			this.managedComponent.LookDownPositionSpringPower = state.LookDownPositionSpringPower.Value;
		}
		if (state.LookDownRotationOffsetMiddle != null)
		{
			this.managedComponent.LookDownRotationOffsetMiddle = state.LookDownRotationOffsetMiddle.Value;
		}
		if (state.LookDownRotationOffsetLeft != null)
		{
			this.managedComponent.LookDownRotationOffsetLeft = state.LookDownRotationOffsetLeft.Value;
		}
		if (state.LookDownRotationOffsetRight != null)
		{
			this.managedComponent.LookDownRotationOffsetRight = state.LookDownRotationOffsetRight.Value;
		}
		if (state.LookDownRotationSpringPower != null)
		{
			this.managedComponent.LookDownRotationSpringPower = state.LookDownRotationSpringPower.Value;
		}
		if (state.AnimationType != null)
		{
			this.managedComponent.AnimationType = state.AnimationType.Value;
		}
		if (state.AnimationGrip != null)
		{
			this.managedComponent.AnimationGrip = state.AnimationGrip.Value;
		}
		if (state.Persist != null)
		{
			this.managedComponent.Persist = state.Persist.Value;
		}
	}
}
