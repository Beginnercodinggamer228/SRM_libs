using System;
using System.Collections;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000542 RID: 1346
public class BindingLineUI : MonoBehaviour
{
	// Token: 0x06001C0A RID: 7178 RVA: 0x0006B464 File Offset: 0x00069664
	public void Start()
	{
		this.uiBundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("ui");
		this.ui = base.GetComponentInParent<OptionsUI>();
		this.Refresh();
	}

	// Token: 0x06001C0B RID: 7179 RVA: 0x0006B492 File Offset: 0x00069692
	public void BindPrimary()
	{
		this.Bind(this.leftBtnMode);
	}

	// Token: 0x06001C0C RID: 7180 RVA: 0x0006B4A0 File Offset: 0x000696A0
	public void BindSecondary()
	{
		this.Bind(this.rightBtnMode);
	}

	// Token: 0x06001C0D RID: 7181 RVA: 0x0006B4B0 File Offset: 0x000696B0
	public void Bind(SRInput.ButtonType btnMode)
	{
		if (this.isCurrentlyBinding)
		{
			return;
		}
		BindingSource binding = SRInput.GetBinding(this.action, btnMode);
		if (binding != null)
		{
			this.action.ListenForBindingReplacing(binding);
		}
		else
		{
			this.action.ListenForBinding();
		}
		this.EnterBindingState(btnMode);
		this.SetMode(new SRInput.ButtonType?(btnMode));
		this.SetButtonText(this.uiBundle.Get("m.press_key"));
	}

	// Token: 0x06001C0E RID: 7182 RVA: 0x0006B51E File Offset: 0x0006971E
	private void OnBindingRejected(PlayerAction arg1, BindingSource arg2, BindingSourceRejectionType arg3)
	{
		base.StartCoroutine(this.ResetBindingState());
	}

	// Token: 0x06001C0F RID: 7183 RVA: 0x0006B51E File Offset: 0x0006971E
	private void OnBindingAdded(PlayerAction arg1, BindingSource arg2)
	{
		base.StartCoroutine(this.ResetBindingState());
	}

	// Token: 0x06001C10 RID: 7184 RVA: 0x0006B530 File Offset: 0x00069730
	private void EnterBindingState(SRInput.ButtonType btnMode)
	{
		BindingListenOptions listenOptions = SRInput.Actions.ListenOptions;
		listenOptions.OnBindingAdded = (Action<PlayerAction, BindingSource>)Delegate.Combine(listenOptions.OnBindingAdded, new Action<PlayerAction, BindingSource>(this.OnBindingAdded));
		BindingListenOptions listenOptions2 = SRInput.Actions.ListenOptions;
		listenOptions2.OnBindingRejected = (Action<PlayerAction, BindingSource, BindingSourceRejectionType>)Delegate.Combine(listenOptions2.OnBindingRejected, new Action<PlayerAction, BindingSource, BindingSourceRejectionType>(this.OnBindingRejected));
		if (btnMode == SRInput.ButtonType.GAMEPAD || btnMode == SRInput.ButtonType.GAMEPAD_SEC)
		{
			BindingListenOptions listenOptions3 = SRInput.Actions.ListenOptions;
			listenOptions3.OnBindingFound = (Func<PlayerAction, BindingSource, bool>)Delegate.Combine(listenOptions3.OnBindingFound, new Func<PlayerAction, BindingSource, bool>(this.IsGamepadBinding));
		}
		else
		{
			BindingListenOptions listenOptions4 = SRInput.Actions.ListenOptions;
			listenOptions4.OnBindingFound = (Func<PlayerAction, BindingSource, bool>)Delegate.Combine(listenOptions4.OnBindingFound, new Func<PlayerAction, BindingSource, bool>(this.IsKeyboardMouseBinding));
		}
		this.isCurrentlyBinding = true;
		SelectImageForAction componentInChildren = base.gameObject.GetComponentInChildren<SelectImageForAction>();
		if (componentInChildren != null)
		{
			componentInChildren.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001C11 RID: 7185 RVA: 0x0006B61B File Offset: 0x0006981B
	private bool IsGamepadBinding(PlayerAction action, BindingSource binding)
	{
		return binding.BindingSourceType != BindingSourceType.KeyBindingSource && binding.BindingSourceType != BindingSourceType.MouseBindingSource;
	}

	// Token: 0x06001C12 RID: 7186 RVA: 0x0006B634 File Offset: 0x00069834
	private bool IsKeyboardMouseBinding(PlayerAction action, BindingSource binding)
	{
		return binding.BindingSourceType == BindingSourceType.KeyBindingSource || binding.BindingSourceType == BindingSourceType.MouseBindingSource;
	}

	// Token: 0x06001C13 RID: 7187 RVA: 0x0006B64A File Offset: 0x0006984A
	private IEnumerator ResetBindingState()
	{
		BindingListenOptions listenOptions = SRInput.Actions.ListenOptions;
		listenOptions.OnBindingAdded = (Action<PlayerAction, BindingSource>)Delegate.Remove(listenOptions.OnBindingAdded, new Action<PlayerAction, BindingSource>(this.OnBindingAdded));
		BindingListenOptions listenOptions2 = SRInput.Actions.ListenOptions;
		listenOptions2.OnBindingRejected = (Action<PlayerAction, BindingSource, BindingSourceRejectionType>)Delegate.Remove(listenOptions2.OnBindingRejected, new Action<PlayerAction, BindingSource, BindingSourceRejectionType>(this.OnBindingRejected));
		yield return new WaitForEndOfFrame();
		this.isCurrentlyBinding = false;
		SelectImageForAction componentInChildren = base.gameObject.GetComponentInChildren<SelectImageForAction>(true);
		if (componentInChildren != null)
		{
			componentInChildren.gameObject.SetActive(true);
		}
		yield break;
	}

	// Token: 0x06001C14 RID: 7188 RVA: 0x0006B659 File Offset: 0x00069859
	public IEnumerator Delay(UnityAction action)
	{
		yield return new WaitForEndOfFrame();
		action();
		yield break;
	}

	// Token: 0x06001C15 RID: 7189 RVA: 0x0006B668 File Offset: 0x00069868
	public void Refresh()
	{
		BindingLineUI.SetButtonText(this.leftBtn, XlateKeyText.XlateKey(this.GetCurrKey(this.leftBtnMode)));
		if (this.rightBtn != null)
		{
			BindingLineUI.SetButtonText(this.rightBtn, XlateKeyText.XlateKey(this.GetCurrKey(this.rightBtnMode)));
		}
		if (this.mode != null)
		{
			base.StartCoroutine(this.DelayedResetMode());
		}
	}

	// Token: 0x06001C16 RID: 7190 RVA: 0x0006B6D5 File Offset: 0x000698D5
	public IEnumerator DelayedResetMode()
	{
		yield return new WaitForEndOfFrame();
		this.action.StopListeningForBinding();
		this.SetMode(null);
		yield break;
	}

	// Token: 0x06001C17 RID: 7191 RVA: 0x0006B6E4 File Offset: 0x000698E4
	private string GetCurrKey(SRInput.ButtonType mode)
	{
		return SRInput.GetButtonKey(this.action, mode) ?? Key.None.ToString();
	}

	// Token: 0x06001C18 RID: 7192 RVA: 0x0006B710 File Offset: 0x00069910
	private void SetButtonText(string text)
	{
		SRInput.ButtonType? buttonType = this.mode;
		SRInput.ButtonType buttonType2 = this.leftBtnMode;
		BindingLineUI.SetButtonText((buttonType.GetValueOrDefault() == buttonType2 & buttonType != null) ? this.leftBtn : this.rightBtn, text);
	}

	// Token: 0x06001C19 RID: 7193 RVA: 0x0006B754 File Offset: 0x00069954
	private static void SetButtonText(Button btn, string text)
	{
		TMP_Text[] componentsInChildren = btn.GetComponentsInChildren<TMP_Text>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].text = text;
		}
	}

	// Token: 0x06001C1A RID: 7194 RVA: 0x0006B780 File Offset: 0x00069980
	private void SetMode(SRInput.ButtonType? mode)
	{
		this.mode = mode;
		bool flag = mode != null;
		this.ui.PreventClosing(flag);
		TabByMenuKeys.disabledForBinding = flag;
		EventSystem.current.sendNavigationEvents = !flag;
		((SRStandaloneInputModule)EventSystem.current.currentInputModule).processMouseEvents = !flag;
	}

	// Token: 0x04001B23 RID: 6947
	public BindingLineUI.DisableDelegate disableDelegate;

	// Token: 0x04001B24 RID: 6948
	public PlayerAction action;

	// Token: 0x04001B25 RID: 6949
	public Button leftBtn;

	// Token: 0x04001B26 RID: 6950
	public Button rightBtn;

	// Token: 0x04001B27 RID: 6951
	public SRInput.ButtonType leftBtnMode;

	// Token: 0x04001B28 RID: 6952
	public SRInput.ButtonType rightBtnMode = SRInput.ButtonType.SECONDARY;

	// Token: 0x04001B29 RID: 6953
	public MessageBundle uiBundle;

	// Token: 0x04001B2A RID: 6954
	public MessageBundle keysBundle;

	// Token: 0x04001B2B RID: 6955
	private SRInput.ButtonType? mode;

	// Token: 0x04001B2C RID: 6956
	private OptionsUI ui;

	// Token: 0x04001B2D RID: 6957
	private bool isCurrentlyBinding;

	// Token: 0x02000543 RID: 1347
	// (Invoke) Token: 0x06001C1D RID: 7197
	public delegate bool DisableDelegate();
}
