using System;

// Token: 0x02000616 RID: 1558
public class SetMouseEnabled : SRBehaviour
{
	// Token: 0x060020AC RID: 8364 RVA: 0x0007CE89 File Offset: 0x0007B089
	private void Start()
	{
		vp_Utility.LockCursor = !this.mouseEnabled;
	}

	// Token: 0x04002005 RID: 8197
	public bool mouseEnabled = true;
}
