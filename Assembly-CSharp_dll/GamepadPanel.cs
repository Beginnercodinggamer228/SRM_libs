using System;
using System.Collections.Generic;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200058F RID: 1423
public class GamepadPanel : MonoBehaviour
{
	// Token: 0x06001D7B RID: 7547 RVA: 0x00070004 File Offset: 0x0006E204
	public void Awake()
	{
		this.inputDir = SRSingleton<GameContext>.Instance.InputDirector;
		this.ps4SwapSticksToggleLabel.SetActive(false);
		this.swapStickToggleLabel.SetActive(true);
		SRSingleton<GameContext>.Instance.MessageDirector.RegisterBundlesListener(new MessageDirector.BundlesListener(this.OnBundlesAvailable));
	}

	// Token: 0x06001D7C RID: 7548 RVA: 0x00070054 File Offset: 0x0006E254
	public void OnDestroy()
	{
		if (SRSingleton<GameContext>.Instance != null)
		{
			SRSingleton<GameContext>.Instance.MessageDirector.UnregisterBundlesListener(new MessageDirector.BundlesListener(this.OnBundlesAvailable));
		}
	}

	// Token: 0x06001D7D RID: 7549 RVA: 0x0007007E File Offset: 0x0006E27E
	private GamepadVisualPanel GetVisualPanel()
	{
		return this.defaultGamepadVisualPanel;
	}

	// Token: 0x06001D7E RID: 7550 RVA: 0x00070088 File Offset: 0x0006E288
	private void OnBundlesAvailable(MessageDirector msgDir)
	{
		this.initializing = true;
		this.uiBundle = msgDir.GetBundle("ui");
		this.SetupBindings();
		this.disableGamepadToggle.isOn = SRSingleton<GameContext>.Instance.InputDirector.GetDisableGamepad();
		this.SetGamepadControlsInteractable(!this.disableGamepadToggle.isOn);
		this.swapSticksToggle.isOn = SRSingleton<GameContext>.Instance.InputDirector.GetSwapSticks();
		this.invertGamepadLookYToggle.isOn = SRSingleton<GameContext>.Instance.InputDirector.GetInvertGamepadLookY();
		this.lookSensitivityXSlider.value = SRSingleton<GameContext>.Instance.InputDirector.GamepadLookSensitivityX;
		this.lookSensitivityYSlider.value = SRSingleton<GameContext>.Instance.InputDirector.GamepadLookSensitivityY;
		this.deadZoneSlider.value = SRSingleton<GameContext>.Instance.InputDirector.ControllerStickDeadZone * 10f;
		this.disableGamepadToggle.gameObject.SetActive(true);
		this.defaultGamepadBtn.gameObject.SetActive(true);
		this.rightPanel.SetActive(true);
		this.openChangeGamepadSettingsButton.SetActive(false);
		this.initializing = false;
		this.RefreshBindings();
		this.Update();
	}

	// Token: 0x06001D7F RID: 7551 RVA: 0x000701B8 File Offset: 0x0006E3B8
	public void Update()
	{
		bool flag = this.inputDir.UsingSteamController();
		this.standardPanel.SetActive(!flag);
		this.steamPanel.SetActive(flag);
		this.defaultGamepadVisualPanel.gameObject.SetActive(true);
		this.ps4GamepadVisualPanel.gameObject.SetActive(false);
	}

	// Token: 0x06001D80 RID: 7552 RVA: 0x00070210 File Offset: 0x0006E410
	private void SetupBindings()
	{
		Selectable selectable = this.deadZoneSlider;
		for (int i = 0; i < this.bindingsGamepadPanel.transform.childCount; i++)
		{
			Destroyer.Destroy(this.bindingsGamepadPanel.transform.GetChild(i).gameObject, "GamepadPanel.SetupBindings");
		}
		while (this.bindingsGamepadPanel.transform.childCount > 0)
		{
			this.bindingsGamepadPanel.transform.GetChild(0).SetParent(null, false);
		}
		this.CreateGamepadBindingLine("key.shoot", SRInput.Actions.attack);
		this.CreateGamepadBindingLine("key.vac", SRInput.Actions.vac);
		this.CreateGamepadBindingLine("key.burst", SRInput.Actions.burst);
		this.CreateGamepadBindingLine("key.jump", SRInput.Actions.jump);
		this.CreateGamepadBindingLine("key.run", SRInput.Actions.run);
		this.CreateGamepadBindingLine("key.interact", SRInput.Actions.interact);
		this.CreateGamepadBindingLine("key.gadgetMode", SRInput.Actions.toggleGadgetMode);
		this.CreateGamepadBindingLine("key.flashlight", SRInput.Actions.light);
		this.CreateGamepadBindingLine("key.radar", SRInput.Actions.radarToggle);
		this.CreateGamepadBindingLine("key.map", SRInput.Actions.openMap);
		this.CreateGamepadBindingLine("key.slot_1", SRInput.Actions.slot1);
		this.CreateGamepadBindingLine("key.slot_2", SRInput.Actions.slot2);
		this.CreateGamepadBindingLine("key.slot_3", SRInput.Actions.slot3);
		this.CreateGamepadBindingLine("key.slot_4", SRInput.Actions.slot4);
		this.CreateGamepadBindingLine("key.slot_5", SRInput.Actions.slot5);
		this.CreateGamepadBindingLine("key.prev_slot", SRInput.Actions.prevSlot);
		this.CreateGamepadBindingLine("key.next_slot", SRInput.Actions.nextSlot);
		this.CreateGamepadBindingLine("key.reportissue", SRInput.Actions.reportIssue);
		this.CreateGamepadBindingLine("key.screenshot", SRInput.Actions.screenshot);
		this.CreateGamepadBindingLine("key.recordgif", SRInput.Actions.recordGif);
		this.CreateGamepadBindingLine("key.pedia", SRInput.Actions.pedia);
		Button[] componentsInChildren = this.bindingsGamepadPanel.GetComponentsInChildren<Button>(true);
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			Navigation navigation = default(Navigation);
			navigation.mode = Navigation.Mode.Explicit;
			if (j < componentsInChildren.Length - 1)
			{
				navigation.selectOnDown = componentsInChildren[j + 1];
			}
			else
			{
				navigation.selectOnDown = this.defaultGamepadBtn;
				Navigation navigation2 = this.defaultGamepadBtn.navigation;
				navigation2.mode = Navigation.Mode.Explicit;
				navigation2.selectOnUp = componentsInChildren[j];
				this.defaultGamepadBtn.navigation = navigation2;
			}
			if (j > 0)
			{
				navigation.selectOnUp = componentsInChildren[j - 1];
			}
			else
			{
				navigation.selectOnUp = selectable;
				Navigation navigation3 = selectable.navigation;
				navigation3.mode = Navigation.Mode.Explicit;
				navigation3.selectOnDown = componentsInChildren[j];
				selectable.navigation = navigation3;
			}
			navigation.selectOnLeft = this.disableGamepadToggle;
			componentsInChildren[j].navigation = navigation;
		}
	}

	// Token: 0x06001D81 RID: 7553 RVA: 0x0007052D File Offset: 0x0006E72D
	public void ToggleDisableGamepad()
	{
		SRSingleton<GameContext>.Instance.InputDirector.SetDisableGamepad(this.disableGamepadToggle.isOn);
		this.SetGamepadControlsInteractable(!this.disableGamepadToggle.isOn);
	}

	// Token: 0x06001D82 RID: 7554 RVA: 0x0007055D File Offset: 0x0006E75D
	public void ToggleInvertGamepadLookY()
	{
		SRSingleton<GameContext>.Instance.InputDirector.SetInvertGamepadLookY(this.invertGamepadLookYToggle.isOn);
	}

	// Token: 0x06001D83 RID: 7555 RVA: 0x00070579 File Offset: 0x0006E779
	public void ToggleSwapSticks()
	{
		SRSingleton<GameContext>.Instance.InputDirector.SetSwapSticks(this.swapSticksToggle.isOn);
		this.RefreshBindings();
	}

	// Token: 0x06001D84 RID: 7556 RVA: 0x0007059C File Offset: 0x0006E79C
	public void OnLookSensitivityXChanged()
	{
		float value = this.lookSensitivityXSlider.value;
		SRSingleton<GameContext>.Instance.InputDirector.GamepadLookSensitivityX = value;
	}

	// Token: 0x06001D85 RID: 7557 RVA: 0x000705C8 File Offset: 0x0006E7C8
	public void OnLookSensitivityYChanged()
	{
		float value = this.lookSensitivityYSlider.value;
		SRSingleton<GameContext>.Instance.InputDirector.GamepadLookSensitivityY = value;
	}

	// Token: 0x06001D86 RID: 7558 RVA: 0x000705F4 File Offset: 0x0006E7F4
	public void OnDeadZoneChanged()
	{
		float controllerStickDeadZone = Mathf.Clamp(this.deadZoneSlider.value / 10f, 0f, 0.95f);
		SRSingleton<GameContext>.Instance.InputDirector.ControllerStickDeadZone = controllerStickDeadZone;
	}

	// Token: 0x06001D87 RID: 7559 RVA: 0x00070634 File Offset: 0x0006E834
	public void OnOpenChangeSettingsClicked()
	{
		BaseUI component = UnityEngine.Object.Instantiate<GameObject>(this.gamepadSettingsPrefab).GetComponent<BaseUI>();
		this.optionsUi.PreventClosing(true);
		base.gameObject.SetActive(false);
		component.onDestroy = (BaseUI.OnDestroyDelegate)Delegate.Combine(component.onDestroy, new BaseUI.OnDestroyDelegate(delegate()
		{
			base.gameObject.SetActive(true);
			this.optionsUi.PreventClosing(false);
		}));
	}

	// Token: 0x06001D88 RID: 7560 RVA: 0x0007068C File Offset: 0x0006E88C
	public void RefreshBindings()
	{
		if (this.initializing)
		{
			return;
		}
		if (this.uiBundle == null)
		{
			return;
		}
		this.GetVisualPanel().ClearAllGamepadText(this.uiBundle);
		foreach (BindingLineUI bindingLineUI in base.GetComponentsInChildren<BindingLineUI>(true))
		{
			bindingLineUI.Refresh();
			if (bindingLineUI.leftBtnMode == SRInput.ButtonType.GAMEPAD)
			{
				BindingSource binding = SRInput.GetBinding(bindingLineUI.action, bindingLineUI.leftBtnMode);
				this.UpdateGamepadBindingText(binding, bindingLineUI);
			}
		}
		this.GetVisualPanel().GetTextForGamepadKey(InputControlType.Start).text = this.uiBundle.Get("m.gamepad_button_pause", new object[]
		{
			InputControlType.Start
		});
	}

	// Token: 0x06001D89 RID: 7561 RVA: 0x00070734 File Offset: 0x0006E934
	private GameObject CreateGamepadBindingLine(string label, PlayerAction action)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.bindingGamepadLinePrefab);
		gameObject.transform.SetParent(this.bindingsGamepadPanel.transform, false);
		BindingPanel.CreateBindingLine(label, action, gameObject, this.uiBundle, this.labelKeyDict, new BindingLineUI.DisableDelegate(this.DisableGamepads));
		BindingLineUI component = gameObject.GetComponent<BindingLineUI>();
		if (component.leftBtnMode == SRInput.ButtonType.GAMEPAD)
		{
			BindingSource binding = SRInput.GetBinding(component.action, SRInput.ButtonType.GAMEPAD);
			this.UpdateGamepadBindingText(binding, component);
		}
		return gameObject;
	}

	// Token: 0x06001D8A RID: 7562 RVA: 0x000707AC File Offset: 0x0006E9AC
	private void SetGamepadControlsInteractable(bool interactable)
	{
		Button[] componentsInChildren = this.bindingsGamepadPanel.GetComponentsInChildren<Button>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].interactable = interactable;
		}
		this.swapSticksToggle.interactable = interactable;
		this.invertGamepadLookYToggle.interactable = interactable;
		this.lookSensitivityXSlider.interactable = interactable;
		this.lookSensitivityYSlider.interactable = interactable;
		this.deadZoneSlider.interactable = interactable;
	}

	// Token: 0x06001D8B RID: 7563 RVA: 0x0007081C File Offset: 0x0006EA1C
	private void UpdateGamepadBindingText(BindingSource bindingSource, BindingLineUI binding)
	{
		InputControlType inputControlType = (bindingSource == null) ? InputControlType.None : (bindingSource as DeviceBindingSource).Control;
		GamepadVisualPanel visualPanel = this.GetVisualPanel();
		TMP_Text textForGamepadKey = visualPanel.GetTextForGamepadKey(inputControlType);
		TMP_Text textForGamepadStickKey = visualPanel.GetTextForGamepadStickKey(inputControlType);
		if (textForGamepadKey != null)
		{
			textForGamepadKey.text = this.uiBundle.Get("m.gamepad_button", new string[]
			{
				XlateKeyText.XlateKey(inputControlType),
				this.uiBundle.Get(this.labelKeyDict[binding])
			});
			return;
		}
		if (textForGamepadStickKey != null)
		{
			textForGamepadStickKey.text = this.uiBundle.Get("m.gamepad_stick", new string[]
			{
				XlateKeyText.XlateKey((inputControlType == InputControlType.LeftStickButton) ? "LeftStick" : "RightStick"),
				this.uiBundle.Get((inputControlType == InputControlType.LeftStickButton ^ SRSingleton<GameContext>.Instance.InputDirector.GetSwapSticks()) ? "l.move" : "l.view"),
				this.uiBundle.Get(this.labelKeyDict[binding]),
				this.uiBundle.Get(string.Format("l.gamepad_{0}_stick_press_action", (inputControlType == InputControlType.LeftStickButton) ? "left" : "right"))
			});
		}
	}

	// Token: 0x06001D8C RID: 7564 RVA: 0x00070951 File Offset: 0x0006EB51
	private int InputControlTypeToSpriteId(InputControlType btn)
	{
		switch (btn)
		{
		case InputControlType.Action1:
			return 0;
		case InputControlType.Action2:
			return 1;
		case InputControlType.Action3:
			return 2;
		case InputControlType.Action4:
			return 3;
		default:
			return 0;
		}
	}

	// Token: 0x06001D8D RID: 7565 RVA: 0x00070977 File Offset: 0x0006EB77
	private bool DisableGamepads()
	{
		return this.disableGamepadToggle.isOn;
	}

	// Token: 0x06001D8E RID: 7566 RVA: 0x00070984 File Offset: 0x0006EB84
	public void ResetGamepadDefaults()
	{
		SRSingleton<GameContext>.Instance.InputDirector.ResetGamepadDefaults();
		SRSingleton<GameContext>.Instance.InputDirector.SetInvertGamepadLookY(this.invertGamepadLookYToggle.isOn);
		this.RefreshBindings();
	}

	// Token: 0x06001D8F RID: 7567 RVA: 0x000709B5 File Offset: 0x0006EBB5
	public void ShowSteamControllerConfig()
	{
		this.inputDir.ShowSteamControllerConfig();
	}

	// Token: 0x04001C8E RID: 7310
	private bool initializing;

	// Token: 0x04001C8F RID: 7311
	public Toggle disableGamepadToggle;

	// Token: 0x04001C90 RID: 7312
	public Toggle swapSticksToggle;

	// Token: 0x04001C91 RID: 7313
	public Toggle invertGamepadLookYToggle;

	// Token: 0x04001C92 RID: 7314
	public GameObject swapStickToggleLabel;

	// Token: 0x04001C93 RID: 7315
	public GameObject ps4SwapSticksToggleLabel;

	// Token: 0x04001C94 RID: 7316
	public Slider lookSensitivityXSlider;

	// Token: 0x04001C95 RID: 7317
	public Slider lookSensitivityYSlider;

	// Token: 0x04001C96 RID: 7318
	public Slider deadZoneSlider;

	// Token: 0x04001C97 RID: 7319
	public Button defaultGamepadBtn;

	// Token: 0x04001C98 RID: 7320
	public GameObject openChangeGamepadSettingsButton;

	// Token: 0x04001C99 RID: 7321
	private Dictionary<BindingLineUI, string> labelKeyDict = new Dictionary<BindingLineUI, string>();

	// Token: 0x04001C9A RID: 7322
	public GameObject bindingGamepadLinePrefab;

	// Token: 0x04001C9B RID: 7323
	public GameObject bindingsGamepadPanel;

	// Token: 0x04001C9C RID: 7324
	public GameObject rightPanel;

	// Token: 0x04001C9D RID: 7325
	public GameObject gamepadSettingsPrefab;

	// Token: 0x04001C9E RID: 7326
	public OptionsUI optionsUi;

	// Token: 0x04001C9F RID: 7327
	public GamepadVisualPanel defaultGamepadVisualPanel;

	// Token: 0x04001CA0 RID: 7328
	public GamepadVisualPanel ps4GamepadVisualPanel;

	// Token: 0x04001CA1 RID: 7329
	public GameObject standardPanel;

	// Token: 0x04001CA2 RID: 7330
	public GameObject steamPanel;

	// Token: 0x04001CA3 RID: 7331
	private MessageBundle uiBundle;

	// Token: 0x04001CA4 RID: 7332
	private InputDirector inputDir;
}
