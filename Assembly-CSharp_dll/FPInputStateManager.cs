using System;

// Token: 0x02000018 RID: 24
public class FPInputStateManager : BaseStateManager<FPInputState, vp_FPInput>
{
	// Token: 0x0600005A RID: 90 RVA: 0x00003EA3 File Offset: 0x000020A3
	public FPInputStateManager(vp_FPInput managedComponent) : base(managedComponent)
	{
		this.CreateStates();
	}

	// Token: 0x0600005B RID: 91 RVA: 0x00003EB2 File Offset: 0x000020B2
	private void CreateStates()
	{
		this.states = new FPInputState[1];
		base.AddState(new FPInputState("Default"), 0);
	}

	// Token: 0x0600005C RID: 92 RVA: 0x00003ED4 File Offset: 0x000020D4
	public override void ApplyState(FPInputState state)
	{
		if (state.MouseLookSensitivity != null)
		{
			this.managedComponent.MouseLookSensitivity = state.MouseLookSensitivity.Value;
		}
		if (state.MouseLookSmoothSteps != null)
		{
			this.managedComponent.MouseLookSmoothSteps = state.MouseLookSmoothSteps.Value;
		}
		if (state.MouseLookAcceleration != null)
		{
			this.managedComponent.MouseLookAcceleration = state.MouseLookAcceleration.Value;
		}
		if (state.MouseLookSmoothWeight != null)
		{
			this.managedComponent.MouseLookSmoothWeight = state.MouseLookSmoothWeight.Value;
		}
		if (state.MouseLookAccelerationThreshold != null)
		{
			this.managedComponent.MouseLookAccelerationThreshold = state.MouseLookAccelerationThreshold.Value;
		}
		if (state.MouseLookInvert != null)
		{
			this.managedComponent.MouseLookInvert = state.MouseLookInvert.Value;
		}
		if (state.MouseCursorForced != null)
		{
			this.managedComponent.MouseCursorForced = state.MouseCursorForced.Value;
		}
		if (state.MouseCursorBlocksMouseLook != null)
		{
			this.managedComponent.MouseCursorBlocksMouseLook = state.MouseCursorBlocksMouseLook.Value;
		}
		if (state.Persist != null)
		{
			this.managedComponent.Persist = state.Persist.Value;
		}
	}
}
