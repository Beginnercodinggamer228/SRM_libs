using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200056D RID: 1389
public class DemoUI : BaseUI
{
	// Token: 0x06001CED RID: 7405 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	protected override bool Closeable()
	{
		return false;
	}

	// Token: 0x06001CEE RID: 7406 RVA: 0x0006E0BD File Offset: 0x0006C2BD
	public void OpenStore()
	{
		Application.OpenURL("http://store.steampowered.com/app/433340/");
		this.Quit();
	}

	// Token: 0x06001CEF RID: 7407 RVA: 0x0006E0CF File Offset: 0x0006C2CF
	public void Quit()
	{
		this.Close();
		SRSingleton<SceneContext>.Instance.OnSessionEnded();
		SceneManager.LoadScene("MainMenu");
	}

	// Token: 0x04001C07 RID: 7175
	private const uint appId = 433340U;

	// Token: 0x04001C08 RID: 7176
	private const string steamStoreUrl = "http://store.steampowered.com/app/433340/";
}
