using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200060E RID: 1550
public class SRInputField : InputField
{
	// Token: 0x0600207B RID: 8315 RVA: 0x0007C1BA File Offset: 0x0007A3BA
	protected override void Start()
	{
		base.Start();
	}

	// Token: 0x0600207C RID: 8316 RVA: 0x0007C1C4 File Offset: 0x0007A3C4
	public override void OnUpdateSelected(BaseEventData data)
	{
		bool flag = InputDirector.UsingGamepad();
		if (flag && SRInput.PauseActions.submit)
		{
			this.requestingKeyboard = true;
			data.Use();
			return;
		}
		if (flag && SRInput.PauseActions.cancel)
		{
			this.requestingKeyboard = false;
			data.Use();
			return;
		}
		if (flag && (SRInput.PauseActions.menuDown || SRInput.PauseActions.menuUp))
		{
			return;
		}
		base.OnUpdateSelected(data);
	}

	// Token: 0x0600207D RID: 8317 RVA: 0x0007C248 File Offset: 0x0007A448
	protected override void LateUpdate()
	{
		base.LateUpdate();
		if (this.requestingKeyboard && !this.keyboardActive)
		{
			this.ActivateKeyboard();
			return;
		}
		if (!this.requestingKeyboard && this.keyboardActive)
		{
			this.DeactivateKeyboard();
		}
	}

	// Token: 0x0600207E RID: 8318 RVA: 0x0007C27D File Offset: 0x0007A47D
	private void ActivateKeyboard()
	{
		this.keyboardActive = true;
	}

	// Token: 0x0600207F RID: 8319 RVA: 0x0007C286 File Offset: 0x0007A486
	private void ProcessVirtualKeyboardInput(string inputString)
	{
		if (inputString != null)
		{
			base.text = inputString;
		}
		base.DeactivateInputField();
		this.requestingKeyboard = false;
	}

	// Token: 0x06002080 RID: 8320 RVA: 0x0007C29F File Offset: 0x0007A49F
	private void DeactivateKeyboard()
	{
		this.keyboardActive = false;
	}

	// Token: 0x04001FE7 RID: 8167
	public string descKey;

	// Token: 0x04001FE8 RID: 8168
	private bool requestingKeyboard;

	// Token: 0x04001FE9 RID: 8169
	private bool keyboardActive;
}
