using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000623 RID: 1571
public class StandaloneStartScreen : MonoBehaviour
{
	// Token: 0x060020FD RID: 8445 RVA: 0x0007E25C File Offset: 0x0007C45C
	public void Update()
	{
		if (!this.pastFirstFrame)
		{
			this.pastFirstFrame = true;
			return;
		}
		if (!this.isLoading)
		{
			this.isLoading = true;
			SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
		}
	}

	// Token: 0x04002053 RID: 8275
	private bool isLoading;

	// Token: 0x04002054 RID: 8276
	private bool pastFirstFrame;
}
