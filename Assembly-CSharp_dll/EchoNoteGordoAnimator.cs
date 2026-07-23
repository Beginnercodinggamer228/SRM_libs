using System;

// Token: 0x020001D9 RID: 473
public class EchoNoteGordoAnimator : SRAnimator<EchoNoteGordo>
{
	// Token: 0x060009FB RID: 2555 RVA: 0x0002C1B4 File Offset: 0x0002A3B4
	public void OnAnimationEvent_Popped()
	{
		base.parent.OnAnimationEvent_Popped();
	}
}
