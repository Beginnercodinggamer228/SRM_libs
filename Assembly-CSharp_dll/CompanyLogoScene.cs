using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000550 RID: 1360
public class CompanyLogoScene : MonoBehaviour
{
	// Token: 0x06001C5B RID: 7259 RVA: 0x0006C23C File Offset: 0x0006A43C
	public void Update()
	{
		if (this.currentState == CompanyLogoScene.LogoSceneState.None)
		{
			this.currentState = CompanyLogoScene.LogoSceneState.FirstFrameProcessed;
			return;
		}
		if (this.currentState == CompanyLogoScene.LogoSceneState.FirstFrameProcessed)
		{
			this.currentState = CompanyLogoScene.LogoSceneState.StartedLogoFadeIn;
			this.timeAcc = 0f;
			return;
		}
		if (this.currentState == CompanyLogoScene.LogoSceneState.StartedLogoFadeIn)
		{
			this.timeAcc += Time.deltaTime / this.logoFadeInTime;
			if ((double)this.timeAcc > 1.0)
			{
				this.currentState = CompanyLogoScene.LogoSceneState.LogoWaitTime;
				this.logo.alpha = 1f;
				this.timeAcc = 0f;
				return;
			}
			this.logo.alpha = Mathf.Lerp(0f, 1f, this.timeAcc);
			return;
		}
		else
		{
			if (this.currentState == CompanyLogoScene.LogoSceneState.LogoWaitTime)
			{
				this.timeAcc += Time.deltaTime / this.logoHoldTime;
				if ((double)this.timeAcc > 1.0)
				{
					this.currentState = CompanyLogoScene.LogoSceneState.StartedLogoFadeOut;
					this.timeAcc = 0f;
				}
				return;
			}
			if (this.currentState == CompanyLogoScene.LogoSceneState.StartedLogoFadeOut)
			{
				this.timeAcc += Time.deltaTime / this.logoFadeOutTime;
				if ((double)this.timeAcc > 1.0)
				{
					this.currentState = CompanyLogoScene.LogoSceneState.SplashPreFadeInWait;
					this.logo.alpha = 0f;
					this.timeAcc = 0f;
					return;
				}
				this.logo.alpha = Mathf.Lerp(1f, 0f, this.timeAcc);
				return;
			}
			else
			{
				if (this.currentState == CompanyLogoScene.LogoSceneState.SplashPreFadeInWait)
				{
					this.timeAcc += Time.deltaTime / this.splashFadeInWaitTime;
					if ((double)this.timeAcc > 1.0)
					{
						this.currentState = CompanyLogoScene.LogoSceneState.StartedSplashFadeIn;
						this.timeAcc = 0f;
					}
					return;
				}
				if (this.currentState == CompanyLogoScene.LogoSceneState.StartedSplashFadeIn)
				{
					this.timeAcc += Time.deltaTime / this.splashFadeInTime;
					if ((double)this.timeAcc > 1.0)
					{
						this.currentState = CompanyLogoScene.LogoSceneState.ReadyToStartLoad;
						this.splashBackground.alpha = 1f;
						this.timeAcc = 0f;
						return;
					}
					this.splashBackground.alpha = Mathf.Lerp(0f, 1f, this.timeAcc);
					return;
				}
				else
				{
					if (this.currentState == CompanyLogoScene.LogoSceneState.ReadyToStartLoad)
					{
						this.currentState = CompanyLogoScene.LogoSceneState.Loading;
						SceneManager.LoadScene("StandaloneStart", LoadSceneMode.Single);
						return;
					}
					return;
				}
			}
		}
	}

	// Token: 0x04001B5E RID: 7006
	public CanvasGroup logo;

	// Token: 0x04001B5F RID: 7007
	public CanvasGroup splashBackground;

	// Token: 0x04001B60 RID: 7008
	public float logoFadeInTime;

	// Token: 0x04001B61 RID: 7009
	public float logoHoldTime;

	// Token: 0x04001B62 RID: 7010
	public float logoFadeOutTime;

	// Token: 0x04001B63 RID: 7011
	public float splashFadeInWaitTime;

	// Token: 0x04001B64 RID: 7012
	public float splashFadeInTime;

	// Token: 0x04001B65 RID: 7013
	private float timeAcc;

	// Token: 0x04001B66 RID: 7014
	private CompanyLogoScene.LogoSceneState currentState;

	// Token: 0x02000551 RID: 1361
	private enum LogoSceneState
	{
		// Token: 0x04001B68 RID: 7016
		None,
		// Token: 0x04001B69 RID: 7017
		FirstFrameProcessed,
		// Token: 0x04001B6A RID: 7018
		StartedLogoFadeIn,
		// Token: 0x04001B6B RID: 7019
		LogoWaitTime,
		// Token: 0x04001B6C RID: 7020
		StartedLogoFadeOut,
		// Token: 0x04001B6D RID: 7021
		SplashPreFadeInWait,
		// Token: 0x04001B6E RID: 7022
		StartedSplashFadeIn,
		// Token: 0x04001B6F RID: 7023
		ReadyToStartLoad,
		// Token: 0x04001B70 RID: 7024
		Loading
	}
}
