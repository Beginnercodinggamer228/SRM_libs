using System;
using InControl;
using TMPro;
using UnityEngine;

// Token: 0x02000591 RID: 1425
public class GamepadVisualPanel : MonoBehaviour
{
	// Token: 0x06001D9A RID: 7578 RVA: 0x00070B3C File Offset: 0x0006ED3C
	public void ClearAllGamepadText(MessageBundle uiBundle)
	{
		this.ClearGamepadText(uiBundle, this.aText, false, InputControlType.Action1);
		this.ClearGamepadText(uiBundle, this.bText, false, InputControlType.Action2);
		this.ClearGamepadText(uiBundle, this.xText, false, InputControlType.Action3);
		this.ClearGamepadText(uiBundle, this.yText, false, InputControlType.Action4);
		this.ClearGamepadText(uiBundle, this.ltText, false, InputControlType.LeftTrigger);
		this.ClearGamepadText(uiBundle, this.lbText, false, InputControlType.LeftBumper);
		this.ClearGamepadText(uiBundle, this.rtText, false, InputControlType.RightTrigger);
		this.ClearGamepadText(uiBundle, this.rbText, false, InputControlType.RightBumper);
		this.ClearGamepadText(uiBundle, this.upText, false, InputControlType.DPadUp);
		this.ClearGamepadText(uiBundle, this.rightText, false, InputControlType.DPadRight);
		this.ClearGamepadText(uiBundle, this.downText, false, InputControlType.DPadDown);
		this.ClearGamepadText(uiBundle, this.leftText, false, InputControlType.DPadLeft);
		this.ClearGamepadText(uiBundle, this.backText, false, InputControlType.Back);
		this.ClearGamepadText(uiBundle, this.startText, false, InputControlType.Start);
		this.ClearGamepadText(uiBundle, this.leftStickText, true, InputControlType.LeftStickButton);
		this.ClearGamepadText(uiBundle, this.rightStickText, true, InputControlType.RightStickButton);
	}

	// Token: 0x06001D9B RID: 7579 RVA: 0x00070C48 File Offset: 0x0006EE48
	private void ClearGamepadText(MessageBundle uiBundle, TMP_Text text, bool isStick, InputControlType key)
	{
		if (isStick)
		{
			text.text = uiBundle.Get("m.gamepad_stick", new string[]
			{
				XlateKeyText.XlateKey((key == InputControlType.LeftStickButton) ? "LeftStick" : "RightStick"),
				uiBundle.Get((key == InputControlType.LeftStickButton ^ SRSingleton<GameContext>.Instance.InputDirector.GetSwapSticks()) ? "l.move" : "l.view"),
				uiBundle.Get("l.none"),
				uiBundle.Get(string.Format("l.gamepad_{0}_stick_press_action", (key == InputControlType.LeftStickButton) ? "left" : "right"))
			});
			return;
		}
		if (text == null)
		{
			Log.Error("Test was null!", Array.Empty<object>());
		}
		if (uiBundle == null)
		{
			Log.Error("uiBundle was null!", Array.Empty<object>());
		}
		text.text = uiBundle.Get("m.gamepad_button", new string[]
		{
			XlateKeyText.XlateKey(key),
			uiBundle.Get("l.none")
		});
	}

	// Token: 0x06001D9C RID: 7580 RVA: 0x00070D44 File Offset: 0x0006EF44
	public TMP_Text GetTextForGamepadKey(InputControlType btn)
	{
		switch (btn)
		{
		case InputControlType.DPadUp:
			return this.upText;
		case InputControlType.DPadDown:
			return this.downText;
		case InputControlType.DPadLeft:
			return this.leftText;
		case InputControlType.DPadRight:
			return this.rightText;
		case InputControlType.LeftTrigger:
			return this.ltText;
		case InputControlType.RightTrigger:
			return this.rtText;
		case InputControlType.LeftBumper:
			return this.lbText;
		case InputControlType.RightBumper:
			return this.rbText;
		case InputControlType.Action1:
			return this.aText;
		case InputControlType.Action2:
			return this.bText;
		case InputControlType.Action3:
			return this.xText;
		case InputControlType.Action4:
			return this.yText;
		default:
			switch (btn)
			{
			case InputControlType.Back:
			case InputControlType.Share:
			case InputControlType.View:
				return this.backText;
			case InputControlType.Start:
			case InputControlType.Options:
			case InputControlType.Menu:
				return this.startText;
			}
			return null;
		}
	}

	// Token: 0x06001D9D RID: 7581 RVA: 0x00070E20 File Offset: 0x0006F020
	public TMP_Text GetTextForGamepadStickKey(InputControlType key)
	{
		if (key == InputControlType.LeftStickButton)
		{
			return this.leftStickText;
		}
		if (key != InputControlType.RightStickButton)
		{
			return null;
		}
		return this.rightStickText;
	}

	// Token: 0x04001CAA RID: 7338
	public TMP_Text aText;

	// Token: 0x04001CAB RID: 7339
	public TMP_Text bText;

	// Token: 0x04001CAC RID: 7340
	public TMP_Text xText;

	// Token: 0x04001CAD RID: 7341
	public TMP_Text yText;

	// Token: 0x04001CAE RID: 7342
	public TMP_Text lbText;

	// Token: 0x04001CAF RID: 7343
	public TMP_Text ltText;

	// Token: 0x04001CB0 RID: 7344
	public TMP_Text rbText;

	// Token: 0x04001CB1 RID: 7345
	public TMP_Text rtText;

	// Token: 0x04001CB2 RID: 7346
	public TMP_Text upText;

	// Token: 0x04001CB3 RID: 7347
	public TMP_Text rightText;

	// Token: 0x04001CB4 RID: 7348
	public TMP_Text downText;

	// Token: 0x04001CB5 RID: 7349
	public TMP_Text leftText;

	// Token: 0x04001CB6 RID: 7350
	public TMP_Text backText;

	// Token: 0x04001CB7 RID: 7351
	public TMP_Text startText;

	// Token: 0x04001CB8 RID: 7352
	public TMP_Text leftStickText;

	// Token: 0x04001CB9 RID: 7353
	public TMP_Text rightStickText;
}
