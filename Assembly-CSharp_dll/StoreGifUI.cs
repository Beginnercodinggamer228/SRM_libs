using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000626 RID: 1574
public class StoreGifUI : BaseUI
{
	// Token: 0x0600210C RID: 8460 RVA: 0x0007E65E File Offset: 0x0007C85E
	public void OK()
	{
		this.okBtn.interactable = false;
		this.cancelBtn.interactable = false;
		this.storing = true;
		this.onConfirm();
	}

	// Token: 0x0600210D RID: 8461 RVA: 0x0007E68A File Offset: 0x0007C88A
	public void Cancel()
	{
		Destroyer.Destroy(base.gameObject, "StoreGifUI.Cancel");
	}

	// Token: 0x0600210E RID: 8462 RVA: 0x0007E69C File Offset: 0x0007C89C
	public override void Update()
	{
		base.Update();
		if (this.storing && Time.unscaledTime > this.ellipsisChangeTime)
		{
			base.Status(MessageUtil.Compose("m.ellipsize" + this.ellipsisCount, new string[]
			{
				"m.gif_storing"
			}));
			this.ellipsisCount = (this.ellipsisCount + 1f) % 4f;
			this.ellipsisChangeTime = Time.unscaledTime + 0.5f;
		}
	}

	// Token: 0x0400206C RID: 8300
	public StoreGifUI.OnConfirm onConfirm;

	// Token: 0x0400206D RID: 8301
	public Button okBtn;

	// Token: 0x0400206E RID: 8302
	public Button cancelBtn;

	// Token: 0x0400206F RID: 8303
	public float ellipsisChangeTime;

	// Token: 0x04002070 RID: 8304
	public float ellipsisCount;

	// Token: 0x04002071 RID: 8305
	private bool storing;

	// Token: 0x02000627 RID: 1575
	// (Invoke) Token: 0x06002111 RID: 8465
	public delegate void OnConfirm();
}
