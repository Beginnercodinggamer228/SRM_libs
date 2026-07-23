using System;

// Token: 0x02000579 RID: 1401
public class ErrorUI : SRBehaviour
{
	// Token: 0x06001D27 RID: 7463 RVA: 0x0006EC2F File Offset: 0x0006CE2F
	public void Close()
	{
		Destroyer.Destroy(base.gameObject, "ErrorUI.Close");
	}
}
