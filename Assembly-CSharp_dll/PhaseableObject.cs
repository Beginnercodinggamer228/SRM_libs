using System;

// Token: 0x02000415 RID: 1045
public abstract class PhaseableObject : SRBehaviour
{
	// Token: 0x060015D7 RID: 5591
	public abstract bool ReadyToPhase();

	// Token: 0x060015D8 RID: 5592
	public abstract void PhaseOut();

	// Token: 0x060015D9 RID: 5593
	public abstract void PhaseIn();
}
