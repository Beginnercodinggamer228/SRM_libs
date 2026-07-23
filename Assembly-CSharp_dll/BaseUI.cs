using System;
using System.Collections;
using TMPro;
using UnityEngine;

// Token: 0x0200053F RID: 1343
public class BaseUI : SRBehaviour
{
	// Token: 0x06001BF0 RID: 7152 RVA: 0x0006B1D4 File Offset: 0x000693D4
	public virtual void Awake()
	{
		SRSingleton<GameContext>.Instance.MessageDirector.RegisterBundlesListener(new MessageDirector.BundlesListener(this.OnBundlesAvailable));
		if (this.statusArea != null)
		{
			this.defaultStatusColor = this.statusArea.color;
			this.statusArea.text = "";
		}
	}

	// Token: 0x06001BF1 RID: 7153 RVA: 0x0006B22C File Offset: 0x0006942C
	public virtual void OnBundlesAvailable(MessageDirector msgDir)
	{
		this.uiBundle = msgDir.GetBundle("ui");
	}

	// Token: 0x06001BF2 RID: 7154 RVA: 0x0006B23F File Offset: 0x0006943F
	public virtual void OnDestroy()
	{
		if (SRSingleton<GameContext>.Instance != null)
		{
			SRSingleton<GameContext>.Instance.MessageDirector.UnregisterBundlesListener(new MessageDirector.BundlesListener(this.OnBundlesAvailable));
		}
		if (this.onDestroy != null)
		{
			this.onDestroy();
		}
	}

	// Token: 0x06001BF3 RID: 7155 RVA: 0x0006B27D File Offset: 0x0006947D
	public virtual void Close()
	{
		Destroyer.Destroy(base.gameObject, "BaseUI.Close");
	}

	// Token: 0x06001BF4 RID: 7156 RVA: 0x0006B28F File Offset: 0x0006948F
	public void Status(string statusMsg)
	{
		this.SetStatusAndSetupClear(this.uiBundle.Xlate(statusMsg), this.defaultStatusColor, 5f);
	}

	// Token: 0x06001BF5 RID: 7157 RVA: 0x0006B2B0 File Offset: 0x000694B0
	public void Error(string errorMsg, bool neverClear = false)
	{
		if (this.statusArea != null)
		{
			if (neverClear)
			{
				this.SetStatus(this.uiBundle.Xlate(errorMsg), Color.yellow);
				return;
			}
			this.SetStatusAndSetupClear(this.uiBundle.Xlate(errorMsg), Color.yellow, 5f);
		}
	}

	// Token: 0x06001BF6 RID: 7158 RVA: 0x0006B302 File Offset: 0x00069502
	public void ClearStatus()
	{
		this.statusArea.color = this.defaultStatusColor;
		this.statusArea.text = "";
	}

	// Token: 0x06001BF7 RID: 7159 RVA: 0x0006B325 File Offset: 0x00069525
	private void SetStatus(string text, Color color)
	{
		Debug.Log("MST Setting status: " + text);
		this.statusArea.color = color;
		this.statusArea.text = text;
	}

	// Token: 0x06001BF8 RID: 7160 RVA: 0x0006B34F File Offset: 0x0006954F
	private void SetStatusAndSetupClear(string text, Color color, float clearTime)
	{
		this.SetStatus(text, color);
		this.statusClearTime = Time.unscaledTime + clearTime;
		base.StartCoroutine(this.TimedClearStatus(clearTime));
	}

	// Token: 0x06001BF9 RID: 7161 RVA: 0x0006B374 File Offset: 0x00069574
	private IEnumerator TimedClearStatus(float seconds)
	{
		yield return new WaitForSecondsRealtime(seconds);
		if (this.statusClearTime <= Time.unscaledTime)
		{
			this.ClearStatus();
		}
		yield break;
	}

	// Token: 0x06001BFA RID: 7162 RVA: 0x0006B38A File Offset: 0x0006958A
	protected virtual bool Closeable()
	{
		return !SRSingleton<SceneContext>.Instance.PediaDirector.IsPediaOpen();
	}

	// Token: 0x06001BFB RID: 7163 RVA: 0x0006B39E File Offset: 0x0006959E
	public virtual void Update()
	{
		if (SRInput.PauseActions.cancel.WasPressed)
		{
			this.OnCancelPressed();
		}
	}

	// Token: 0x06001BFC RID: 7164 RVA: 0x0006B3B7 File Offset: 0x000695B7
	public void Play(SECTR_AudioCue cue)
	{
		SECTR_AudioSystem.Play(cue, Vector3.zero, false);
	}

	// Token: 0x06001BFD RID: 7165 RVA: 0x0006B3C6 File Offset: 0x000695C6
	public void PlayErrorCue()
	{
		this.Play(SRSingleton<GameContext>.Instance.UITemplates.errorCue);
	}

	// Token: 0x06001BFE RID: 7166 RVA: 0x0006B3DD File Offset: 0x000695DD
	protected virtual void OnCancelPressed()
	{
		if (this.Closeable())
		{
			this.Close();
		}
	}

	// Token: 0x04001B18 RID: 6936
	public BaseUI.OnDestroyDelegate onDestroy;

	// Token: 0x04001B19 RID: 6937
	public TMP_Text statusArea;

	// Token: 0x04001B1A RID: 6938
	private Color defaultStatusColor;

	// Token: 0x04001B1B RID: 6939
	protected MessageBundle uiBundle;

	// Token: 0x04001B1C RID: 6940
	protected const string ERR_INSUF_COINS = "e.insuf_coins";

	// Token: 0x04001B1D RID: 6941
	private const float STATUS_AREA_CLEAR_TIME = 5f;

	// Token: 0x04001B1E RID: 6942
	private float statusClearTime;

	// Token: 0x02000540 RID: 1344
	// (Invoke) Token: 0x06001C01 RID: 7169
	public delegate void OnDestroyDelegate();
}
