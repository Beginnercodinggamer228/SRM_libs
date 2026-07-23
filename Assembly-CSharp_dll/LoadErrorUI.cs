using System;
using TMPro;
using UnityEngine;

// Token: 0x020005AC RID: 1452
public class LoadErrorUI : BaseUI
{
	// Token: 0x06001E14 RID: 7700 RVA: 0x0007241C File Offset: 0x0007061C
	public static LoadErrorUI OpenLoadErrorUI(LoadErrorUI prefab, string messageKey, bool showContactSupport, string okButtonKey, Action onOkAction, string closeButtonKey, Action onCloseAction)
	{
		LoadErrorUI loadErrorUI = UnityEngine.Object.Instantiate<LoadErrorUI>(prefab);
		if (onOkAction == null)
		{
			loadErrorUI.okButton.SetActive(false);
		}
		else
		{
			loadErrorUI.okButtonText.SetText(loadErrorUI.uiBundle.Xlate(okButtonKey));
		}
		if (showContactSupport)
		{
			loadErrorUI.contactSupportText.SetActive(true);
		}
		else
		{
			loadErrorUI.contactSupportText.SetActive(false);
		}
		loadErrorUI.message.SetText(loadErrorUI.uiBundle.Xlate(messageKey));
		loadErrorUI.closeButtonText.SetText(loadErrorUI.uiBundle.Xlate(closeButtonKey));
		loadErrorUI.onOkAction = onOkAction;
		loadErrorUI.onCloseAction = onCloseAction;
		return loadErrorUI;
	}

	// Token: 0x06001E15 RID: 7701 RVA: 0x000724B6 File Offset: 0x000706B6
	public static void OpenLoadErrorUI(LoadErrorUI prefab, string message, bool showContactSupport, string closeButtonKey, Action onCloseAction)
	{
		LoadErrorUI.OpenLoadErrorUI(prefab, message, showContactSupport, null, null, closeButtonKey, onCloseAction);
	}

	// Token: 0x06001E16 RID: 7702 RVA: 0x000724C6 File Offset: 0x000706C6
	public void OnOk()
	{
		this.onOkAction();
		this.Close();
	}

	// Token: 0x06001E17 RID: 7703 RVA: 0x000724D9 File Offset: 0x000706D9
	public void OnClose()
	{
		this.onCloseAction();
		this.Close();
	}

	// Token: 0x06001E18 RID: 7704 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	protected override bool Closeable()
	{
		return false;
	}

	// Token: 0x04001D34 RID: 7476
	public GameObject okButton;

	// Token: 0x04001D35 RID: 7477
	public GameObject closeButton;

	// Token: 0x04001D36 RID: 7478
	public GameObject contactSupportText;

	// Token: 0x04001D37 RID: 7479
	public TMP_Text message;

	// Token: 0x04001D38 RID: 7480
	public TMP_Text okButtonText;

	// Token: 0x04001D39 RID: 7481
	public TMP_Text closeButtonText;

	// Token: 0x04001D3A RID: 7482
	private Action onOkAction;

	// Token: 0x04001D3B RID: 7483
	private Action onCloseAction;
}
