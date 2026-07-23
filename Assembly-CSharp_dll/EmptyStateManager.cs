using System;

// Token: 0x02000012 RID: 18
public class EmptyStateManager<VComp> : BaseStateManager<EmptyState, VComp> where VComp : vp_Component
{
	// Token: 0x0600004F RID: 79 RVA: 0x00003270 File Offset: 0x00001470
	public EmptyStateManager(VComp managedComponent) : base(managedComponent)
	{
		this.states = new EmptyState[1];
		base.AddState(new EmptyState("Default"), 0);
	}

	// Token: 0x06000050 RID: 80 RVA: 0x00003296 File Offset: 0x00001496
	public override void ApplyState(EmptyState state)
	{
	}
}
