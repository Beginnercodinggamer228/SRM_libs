using System;

// Token: 0x02000555 RID: 1365
public class ConfirmUI : BaseUI
{
	// Token: 0x06001C6A RID: 7274 RVA: 0x0006C537 File Offset: 0x0006A737
	public void OK()
	{
		Destroyer.Destroy(base.gameObject, "ConfirmUI.OK");
		this.onConfirm();
	}

	// Token: 0x06001C6B RID: 7275 RVA: 0x0006C554 File Offset: 0x0006A754
	public void Cancel()
	{
		Destroyer.Destroy(base.gameObject, "ConfirmUI.Cancel");
	}

	// Token: 0x04001B76 RID: 7030
	public ConfirmUI.OnConfirm onConfirm;

	// Token: 0x02000556 RID: 1366
	// (Invoke) Token: 0x06001C6E RID: 7278
	public delegate void OnConfirm();
}
