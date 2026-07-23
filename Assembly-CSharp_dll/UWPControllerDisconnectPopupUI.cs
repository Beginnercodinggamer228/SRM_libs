using System;
using InControl;

// Token: 0x0200066E RID: 1646
public class UWPControllerDisconnectPopupUI : BaseUI
{
	// Token: 0x06002220 RID: 8736 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	protected override bool Closeable()
	{
		return false;
	}

	// Token: 0x06002221 RID: 8737 RVA: 0x000840CC File Offset: 0x000822CC
	public override void Update()
	{
		base.Update();
		if (InputManager.InputDetected())
		{
			this.Close();
		}
	}
}
