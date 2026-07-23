using System;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

// Token: 0x0200067D RID: 1661
public class DataPrivacyHandler : MonoBehaviour
{
	// Token: 0x06002259 RID: 8793 RVA: 0x00084D32 File Offset: 0x00082F32
	public void OpenDataPrivacyUrl()
	{
		this.sourceButton.interactable = false;
		DataPrivacy.FetchPrivacyUrl(new Action<string>(this.OpenUrl), new Action<string>(this.OnFailure));
	}

	// Token: 0x0600225A RID: 8794 RVA: 0x00084D5D File Offset: 0x00082F5D
	private void OnFailure(string reason)
	{
		this.sourceButton.interactable = true;
		Debug.LogWarning(string.Format("Failed to get data privacy url: {0}", reason));
	}

	// Token: 0x0600225B RID: 8795 RVA: 0x00084D7B File Offset: 0x00082F7B
	private void OpenUrl(string url)
	{
		this.sourceButton.interactable = true;
		this.urlOpened = true;
		Application.OpenURL(url);
	}

	// Token: 0x0600225C RID: 8796 RVA: 0x00084D96 File Offset: 0x00082F96
	private void OnApplicationFocus(bool hasFocus)
	{
		if (hasFocus && this.urlOpened)
		{
			this.urlOpened = false;
			RemoteSettings.ForceUpdate();
		}
	}

	// Token: 0x04002232 RID: 8754
	public Button sourceButton;

	// Token: 0x04002233 RID: 8755
	private bool urlOpened;
}
