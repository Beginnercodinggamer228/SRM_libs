using System;
using TMPro;
using UnityEngine;

// Token: 0x02000552 RID: 1362
public class ConfirmResolutionUI : SRBehaviour
{
	// Token: 0x06001C5D RID: 7261 RVA: 0x0006C487 File Offset: 0x0006A687
	public void Awake()
	{
		this.expireTime = Time.unscaledTime + 15f;
	}

	// Token: 0x06001C5E RID: 7262 RVA: 0x0006C49A File Offset: 0x0006A69A
	public void OK()
	{
		if (this.onConfirm != null)
		{
			this.onConfirm();
		}
		Destroyer.Destroy(base.gameObject, "ConfirmResolutionUI.OK");
	}

	// Token: 0x06001C5F RID: 7263 RVA: 0x0006C4BF File Offset: 0x0006A6BF
	public void Cancel()
	{
		if (this.onCancel != null)
		{
			this.onCancel();
		}
		Destroyer.Destroy(base.gameObject, "ConfirmResolutionUI.Cancel");
	}

	// Token: 0x06001C60 RID: 7264 RVA: 0x0006C4E4 File Offset: 0x0006A6E4
	public void Update()
	{
		float num = this.expireTime - Time.unscaledTime;
		if (SRInput.PauseActions.cancel.WasPressed || num <= 0f)
		{
			this.Cancel();
			return;
		}
		this.countdownText.text = Mathf.FloorToInt(num).ToString();
	}

	// Token: 0x04001B71 RID: 7025
	public TMP_Text countdownText;

	// Token: 0x04001B72 RID: 7026
	public ConfirmResolutionUI.OnCancel onCancel;

	// Token: 0x04001B73 RID: 7027
	public ConfirmResolutionUI.OnConfirm onConfirm;

	// Token: 0x04001B74 RID: 7028
	private float expireTime;

	// Token: 0x04001B75 RID: 7029
	private const float COUNTDOWN_TIME = 15f;

	// Token: 0x02000553 RID: 1363
	// (Invoke) Token: 0x06001C63 RID: 7267
	public delegate void OnCancel();

	// Token: 0x02000554 RID: 1364
	// (Invoke) Token: 0x06001C67 RID: 7271
	public delegate void OnConfirm();
}
