using System;
using System.Collections.Generic;
using InControl;
using UnityEngine;

// Token: 0x0200021A RID: 538
public class InputDirector : SRBehaviour
{
	// Token: 0x06000B63 RID: 2915 RVA: 0x0002FFC4 File Offset: 0x0002E1C4
	public void Awake()
	{
		BindingListenOptions listenOptions = SRInput.Actions.ListenOptions;
		listenOptions.OnBindingAdded = (Action<PlayerAction, BindingSource>)Delegate.Combine(listenOptions.OnBindingAdded, new Action<PlayerAction, BindingSource>(this.OnBindingAdded));
		this.input = SRInput.Instance;
		this.InitBindings();
		vp_Utility.SetUsingGamepad(InputDirector.UsingGamepad());
	}

	// Token: 0x06000B64 RID: 2916 RVA: 0x00030017 File Offset: 0x0002E217
	public void OnDestroy()
	{
		BindingListenOptions listenOptions = SRInput.Actions.ListenOptions;
		listenOptions.OnBindingAdded = (Action<PlayerAction, BindingSource>)Delegate.Remove(listenOptions.OnBindingAdded, new Action<PlayerAction, BindingSource>(this.OnBindingAdded));
	}

	// Token: 0x06000B65 RID: 2917 RVA: 0x00030044 File Offset: 0x0002E244
	private void OnBindingAdded(PlayerAction action, BindingSource binding)
	{
		this.NoteKeysChanged();
	}

	// Token: 0x06000B66 RID: 2918 RVA: 0x0003004C File Offset: 0x0002E24C
	public void Update()
	{
		if (SRInput.Actions.reportIssue.WasPressed)
		{
			UnityEngine.Object.Instantiate<GameObject>(this.bugReportPrefab);
		}
		else if (SRInput.Actions.screenshot.WasPressed)
		{
			SRSingleton<GameContext>.Instance.TakeScreenshot();
		}
		else if (SRInput.Actions.recordGif.WasPressed)
		{
			SRSingleton<GameContext>.Instance.TakeGifScreenshot();
		}
		if (Mathf.Abs(Input.GetAxisRaw("mouse x")) > Mathf.Epsilon || Mathf.Abs(Input.GetAxisRaw("mouse y")) > Mathf.Epsilon)
		{
			SRInput.Actions.LastInputType = BindingSourceType.MouseBindingSource;
			SRInput.PauseActions.LastInputType = BindingSourceType.MouseBindingSource;
			SRInput.LookActions.LastInputType = BindingSourceType.MouseBindingSource;
		}
		bool flag = InputDirector.UsingGamepad();
		if (this.oldUsingGamepad != flag)
		{
			this.NoteKeysChanged();
			vp_Utility.SetUsingGamepad(flag);
		}
		this.oldUsingGamepad = flag;
		SRInput.InputMode inputMode = this.input.GetInputMode();
		if (inputMode == SRInput.InputMode.PAUSE)
		{
			if (Time.timeScale != 0f && !Levels.isSpecialNonAlloc())
			{
				this.input.ClearInputMode(base.gameObject.GetInstanceID());
				return;
			}
		}
		else if (inputMode == SRInput.InputMode.DEFAULT && (Time.timeScale == 0f || Levels.isSpecialNonAlloc()))
		{
			this.input.SetInputMode(SRInput.InputMode.PAUSE, base.gameObject.GetInstanceID());
		}
	}

	// Token: 0x06000B67 RID: 2919 RVA: 0x00030187 File Offset: 0x0002E387
	public bool IsProtected(string button)
	{
		return this.protectedButtons.Contains(button);
	}

	// Token: 0x06000B68 RID: 2920 RVA: 0x00030195 File Offset: 0x0002E395
	public bool IsProtected(KeyCode key)
	{
		return this.protectedKeyCodes.Contains(key);
	}

	// Token: 0x06000B69 RID: 2921 RVA: 0x000301A3 File Offset: 0x0002E3A3
	private void InitBindings()
	{
		this.SetDefaultBindings();
	}

	// Token: 0x06000B6A RID: 2922 RVA: 0x000301AC File Offset: 0x0002E3AC
	private void InitializeDefaultBindings()
	{
		if (InputDirector.DEFAULTS == null)
		{
			InputDirector.DEFAULTS = new InputDirector.DefaultBinding[]
			{
				new InputDirector.DefaultBinding(SRInput.Actions.verticalPos, Key.W, Key.UpArrow, InputControlType.LeftStickUp, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.verticalNeg, Key.S, Key.DownArrow, InputControlType.LeftStickDown, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.horizontalPos, Key.D, Key.RightArrow, InputControlType.LeftStickRight, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.horizontalNeg, Key.A, Key.LeftArrow, InputControlType.LeftStickLeft, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.lookYPos, Mouse.PositiveY, Key.None, InputControlType.RightStickDown, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.lookYNeg, Mouse.NegativeY, Key.None, InputControlType.RightStickUp, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.lookXPos, Mouse.PositiveX, Key.None, InputControlType.RightStickRight, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.lookXNeg, Mouse.NegativeX, Key.None, InputControlType.RightStickLeft, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.attack, Mouse.LeftButton, Key.None, InputControlType.RightTrigger, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.vac, Mouse.RightButton, Key.None, InputControlType.LeftTrigger, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.slimeFilter, Key.H, Key.None, InputControlType.None, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.jump, Key.Space, Key.None, InputControlType.Action1, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.run, Key.LeftShift, Key.None, InputControlType.LeftStickButton, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.interact, Key.E, Key.None, InputControlType.Action3, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.accept, Key.Return, Key.PadEnter, InputControlType.None, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.menu, Key.Pause, SRInput.GetDefaultMenuKey(), InputControlType.Start, InputControlType.Options, InputControlType.Menu),
				new InputDirector.DefaultBinding(SRInput.Actions.radarToggle, Key.R, Key.None, InputControlType.RightStickButton, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.openMap, Key.M, Key.None, InputControlType.DPadRight, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.pedia, Key.F1, Key.Slash, InputControlType.DPadUp, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.reportIssue, Key.F2, Key.None, InputControlType.None, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.screenshot, Key.Backslash, Key.None, InputControlType.None, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.recordGif, Key.G, Key.None, InputControlType.None, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.slot1, Key.Key1, Key.None, InputControlType.None, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.slot2, Key.Key2, Key.None, InputControlType.None, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.slot3, Key.Key3, Key.None, InputControlType.None, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.slot4, Key.Key4, Key.None, InputControlType.None, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.slot5, Key.Key5, Key.None, InputControlType.None, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.light, Key.F, Key.None, InputControlType.Action4, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.burst, Mouse.MiddleButton, Key.Q, InputControlType.Action2, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.prevSlot, Mouse.PositiveScrollWheel, Key.None, InputControlType.LeftBumper, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.nextSlot, Mouse.NegativeScrollWheel, Key.None, InputControlType.RightBumper, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.Actions.toggleGadgetMode, Key.T, Key.None, InputControlType.DPadDown, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.PauseActions.submit, Key.Space, Key.None, InputControlType.Action1, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.PauseActions.altSubmit, Key.Space, Key.None, InputControlType.Action3, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.PauseActions.cancel, Key.Escape, Key.None, InputControlType.Action2, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.PauseActions.menuUp, Key.W, Key.UpArrow, InputControlType.DPadUp, InputControlType.LeftStickUp, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.PauseActions.menuDown, Key.S, Key.DownArrow, InputControlType.DPadDown, InputControlType.LeftStickDown, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.PauseActions.menuLeft, Key.A, Key.LeftArrow, InputControlType.DPadLeft, InputControlType.LeftStickLeft, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.PauseActions.menuRight, Key.D, Key.RightArrow, InputControlType.DPadRight, InputControlType.LeftStickRight, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.PauseActions.menuTabLeft, Key.Minus, Key.None, InputControlType.LeftBumper, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.PauseActions.menuTabRight, Key.Equals, Key.None, InputControlType.RightBumper, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.PauseActions.menuScrollUp, Key.PageUp, Key.None, InputControlType.RightStickUp, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.PauseActions.menuScrollDown, Key.PageDown, Key.None, InputControlType.RightStickDown, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.PauseActions.unmenu, Key.Pause, SRInput.GetDefaultMenuKey(), InputControlType.Start, InputControlType.Options, InputControlType.Menu),
				new InputDirector.DefaultBinding(SRInput.PauseActions.closeMap, Key.M, Key.None, InputControlType.None, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.EngageActions.engage, Key.None, Key.None, InputControlType.Menu, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.PauseActions.switchUser, Key.Space, Key.None, InputControlType.Action4, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.LookActions.lookYPos, Mouse.PositiveY, Key.None, InputControlType.RightStickDown, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.LookActions.lookYNeg, Mouse.NegativeY, Key.None, InputControlType.RightStickUp, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.LookActions.lookXPos, Mouse.PositiveX, Key.None, InputControlType.RightStickRight, InputControlType.None, InputControlType.None),
				new InputDirector.DefaultBinding(SRInput.LookActions.lookXNeg, Mouse.NegativeX, Key.None, InputControlType.RightStickLeft, InputControlType.None, InputControlType.None)
			};
		}
		if (InputDirector.EDITOR_DEFAULTS == null)
		{
			InputDirector.EDITOR_DEFAULTS = new InputDirector.DefaultBinding[]
			{
				new InputDirector.DefaultBinding(SRInput.PauseActions.unmenu, Key.Pause, Key.Backquote, InputControlType.Start, InputControlType.Options, InputControlType.Menu)
			};
		}
	}

	// Token: 0x06000B6B RID: 2923 RVA: 0x0003071C File Offset: 0x0002E91C
	private void SetDefaultBindings()
	{
		this.InitializeDefaultBindings();
		BindingListenOptions listenOptions = SRInput.Actions.ListenOptions;
		listenOptions.OnBindingAdded = (Action<PlayerAction, BindingSource>)Delegate.Combine(listenOptions.OnBindingAdded, new Action<PlayerAction, BindingSource>(delegate(PlayerAction action, BindingSource bindingSource)
		{
			SRSingleton<GameContext>.Instance.InputDirector.NoteKeysChanged();
		}));
		InputDirector.DefaultBinding[] array = InputDirector.DEFAULTS;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ApplyDefaultBinding();
		}
		if (Application.isEditor)
		{
			array = InputDirector.EDITOR_DEFAULTS;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ApplyDefaultBinding();
			}
		}
		this.UpdateGamepadStickBindings();
		this.NoteKeysChanged();
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x000307B8 File Offset: 0x0002E9B8
	public void ResetProfile()
	{
		this.mouseLookSensitivity = 0f;
		this.gamepadLookSensitivityX = 0f;
		this.gamepadLookSensitivityY = -0.2f;
		this.invertGamepadLookY = false;
		this.swapSticks = false;
		this.SetDefaultBindings();
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x000307EF File Offset: 0x0002E9EF
	public void ResetKeyMouseDefaults()
	{
		SRInput.Actions.ResetForTypes(new BindingSourceType[]
		{
			BindingSourceType.MouseBindingSource,
			BindingSourceType.KeyBindingSource
		});
	}

	// Token: 0x06000B6E RID: 2926 RVA: 0x00030809 File Offset: 0x0002EA09
	public void ResetGamepadDefaults()
	{
		SRInput.Actions.ResetForTypes(new BindingSourceType[]
		{
			BindingSourceType.DeviceBindingSource
		});
	}

	// Token: 0x06000B6F RID: 2927 RVA: 0x0003081F File Offset: 0x0002EA1F
	public void NoteKeysChanged()
	{
		if (this.onKeysChanged != null)
		{
			this.onKeysChanged();
		}
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x00030834 File Offset: 0x0002EA34
	public static bool UsingGamepad()
	{
		return SRInput.Actions.LastInputType == BindingSourceType.DeviceBindingSource;
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x00030844 File Offset: 0x0002EA44
	public string GetActiveDeviceString(string actionStr, bool isPauseAction)
	{
		bool flag = InputDirector.UsingGamepad();
		PlayerAction action = SRInput.GetAction(actionStr);
		string result;
		if (flag)
		{
			result = this.GetKeyStringForGamepad(action, actionStr);
		}
		else
		{
			result = this.GetKeyStringForMouseKeyboard(action);
		}
		return result;
	}

	// Token: 0x06000B72 RID: 2930 RVA: 0x00030875 File Offset: 0x0002EA75
	public Sprite GetActiveDeviceIcon(string actionStr, bool isPauseAction, out bool iconFound)
	{
		return this.GetDefaultDeviceIcon(actionStr, isPauseAction, out iconFound);
	}

	// Token: 0x06000B73 RID: 2931 RVA: 0x00030880 File Offset: 0x0002EA80
	private Sprite GetDefaultDeviceIcon(string actionStr, bool isPauseAction, out bool iconFound)
	{
		string activeDeviceString = this.GetActiveDeviceString(actionStr, isPauseAction);
		InputDeviceStyle lastDeviceStyle = SRInput.Actions.LastDeviceStyle;
		return SRSingleton<GameContext>.Instance.UITemplates.GetButtonIcon(lastDeviceStyle, activeDeviceString, out iconFound);
	}

	// Token: 0x06000B74 RID: 2932 RVA: 0x000308B4 File Offset: 0x0002EAB4
	private string GetKeyStringForGamepad(PlayerAction action, string actionStr)
	{
		if (action != null)
		{
			BindingSource primGamepadBinding = SRInput.GetPrimGamepadBinding(action);
			if (primGamepadBinding != null && primGamepadBinding is DeviceBindingSource)
			{
				return ((DeviceBindingSource)primGamepadBinding).Control.ToString();
			}
		}
		else
		{
			if (actionStr == (this.swapSticks ? "Look" : "Move"))
			{
				return "LeftStickMove";
			}
			if (actionStr == ((!this.swapSticks) ? "Look" : "Move"))
			{
				return "RightStickMove";
			}
		}
		return null;
	}

	// Token: 0x06000B75 RID: 2933 RVA: 0x0003093C File Offset: 0x0002EB3C
	private string GetKeyStringForMouseKeyboard(PlayerAction action)
	{
		if (action != null)
		{
			BindingSource bindingSource = SRInput.GetPrimKeyBinding(action);
			if (bindingSource == null)
			{
				bindingSource = SRInput.GetSecKeyBinding(action);
			}
			if (bindingSource != null && bindingSource is MouseBindingSource)
			{
				return ((MouseBindingSource)bindingSource).Control.ToString();
			}
			if (bindingSource != null && bindingSource is KeyBindingSource)
			{
				return ((KeyBindingSource)bindingSource).Control.ToString();
			}
		}
		return null;
	}

	// Token: 0x06000B76 RID: 2934 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	public bool UsingSteamController()
	{
		return false;
	}

	// Token: 0x06000B77 RID: 2935 RVA: 0x00003296 File Offset: 0x00001496
	public void ShowSteamControllerConfig()
	{
	}

	// Token: 0x06000B78 RID: 2936 RVA: 0x000309BA File Offset: 0x0002EBBA
	public bool GetSwapSticks()
	{
		return this.swapSticks;
	}

	// Token: 0x06000B79 RID: 2937 RVA: 0x000309C2 File Offset: 0x0002EBC2
	public bool GetDisableGamepad()
	{
		return DeviceBindingSource.DevicesDisabled;
	}

	// Token: 0x06000B7A RID: 2938 RVA: 0x000309C9 File Offset: 0x0002EBC9
	public void SetDisableGamepad(bool disable)
	{
		DeviceBindingSource.DevicesDisabled = disable;
	}

	// Token: 0x06000B7B RID: 2939 RVA: 0x000309D1 File Offset: 0x0002EBD1
	public void SetSwapSticks(bool swap)
	{
		this.swapSticks = swap;
		this.UpdateGamepadStickBindings();
	}

	// Token: 0x06000B7C RID: 2940 RVA: 0x000309E0 File Offset: 0x0002EBE0
	public bool GetInvertGamepadLookY()
	{
		return this.invertGamepadLookY;
	}

	// Token: 0x06000B7D RID: 2941 RVA: 0x000309E8 File Offset: 0x0002EBE8
	public void SetInvertGamepadLookY(bool invert)
	{
		this.invertGamepadLookY = invert;
		this.UpdateGamepadStickBindings();
	}

	// Token: 0x06000B7E RID: 2942 RVA: 0x000309F7 File Offset: 0x0002EBF7
	public bool GetInvertMouseLookY()
	{
		return this.invertMouseLookY;
	}

	// Token: 0x06000B7F RID: 2943 RVA: 0x000309FF File Offset: 0x0002EBFF
	public void SetInvertMouseLookY(bool invert)
	{
		this.invertMouseLookY = invert;
		this.UpdateMouseYAxis();
	}

	// Token: 0x06000B80 RID: 2944 RVA: 0x00030A0E File Offset: 0x0002EC0E
	public bool GetDisableMouseLookSmooth()
	{
		return this.disableMouseLookSmooth;
	}

	// Token: 0x06000B81 RID: 2945 RVA: 0x00030A16 File Offset: 0x0002EC16
	public void SetDisableMouseLookSmooth(bool smooth)
	{
		this.disableMouseLookSmooth = smooth;
	}

	// Token: 0x17000159 RID: 345
	// (get) Token: 0x06000B82 RID: 2946 RVA: 0x00030A1F File Offset: 0x0002EC1F
	// (set) Token: 0x06000B83 RID: 2947 RVA: 0x00030A27 File Offset: 0x0002EC27
	public float ControllerStickDeadZone
	{
		get
		{
			return this.controllerStickDeadZone;
		}
		set
		{
			this.controllerStickDeadZone = value;
		}
	}

	// Token: 0x06000B84 RID: 2948 RVA: 0x00030A30 File Offset: 0x0002EC30
	private void UpdateMouseYAxis()
	{
		this.UpdateMouseYAxis(SRInput.Actions);
		this.UpdateMouseYAxis(SRInput.LookActions);
	}

	// Token: 0x06000B85 RID: 2949 RVA: 0x00030A48 File Offset: 0x0002EC48
	private void UpdateMouseYAxis(SRInput.PlayerLookActions actions)
	{
		actions.lookXNeg.ClearBindingsOfTypes(new BindingSourceType[]
		{
			BindingSourceType.MouseBindingSource
		});
		actions.lookXPos.ClearBindingsOfTypes(new BindingSourceType[]
		{
			BindingSourceType.MouseBindingSource
		});
		actions.lookYNeg.ClearBindingsOfTypes(new BindingSourceType[]
		{
			BindingSourceType.MouseBindingSource
		});
		actions.lookYPos.ClearBindingsOfTypes(new BindingSourceType[]
		{
			BindingSourceType.MouseBindingSource
		});
		actions.lookXNeg.AddBinding(new InputDirector.ScalableMouseBindingSource(Mouse.NegativeX, this.mouseLookSensitivityFactor));
		actions.lookXPos.AddBinding(new InputDirector.ScalableMouseBindingSource(Mouse.PositiveX, this.mouseLookSensitivityFactor));
		actions.lookYNeg.AddBinding(new InputDirector.ScalableMouseBindingSource(this.invertMouseLookY ? Mouse.PositiveY : Mouse.NegativeY, this.mouseLookSensitivityFactor));
		actions.lookYPos.AddBinding(new InputDirector.ScalableMouseBindingSource(this.invertMouseLookY ? Mouse.NegativeY : Mouse.PositiveY, this.mouseLookSensitivityFactor));
	}

	// Token: 0x06000B86 RID: 2950 RVA: 0x00030B20 File Offset: 0x0002ED20
	private void UpdateGamepadStickBindings()
	{
		this.UpdateGamepadStickBindings(SRInput.Actions);
		this.UpdateGamepadStickBindings(SRInput.LookActions);
		SRInput.Actions.horizontalNeg.ClearBindingsOfTypes(new BindingSourceType[]
		{
			BindingSourceType.DeviceBindingSource
		});
		SRInput.Actions.horizontalPos.ClearBindingsOfTypes(new BindingSourceType[]
		{
			BindingSourceType.DeviceBindingSource
		});
		SRInput.Actions.verticalNeg.ClearBindingsOfTypes(new BindingSourceType[]
		{
			BindingSourceType.DeviceBindingSource
		});
		SRInput.Actions.verticalPos.ClearBindingsOfTypes(new BindingSourceType[]
		{
			BindingSourceType.DeviceBindingSource
		});
		if (this.swapSticks)
		{
			SRInput.Actions.horizontalNeg.AddBinding(new DeviceBindingSource(InputControlType.RightStickLeft));
			SRInput.Actions.horizontalPos.AddBinding(new DeviceBindingSource(InputControlType.RightStickRight));
			SRInput.Actions.verticalNeg.AddBinding(new DeviceBindingSource(InputControlType.RightStickDown));
			SRInput.Actions.verticalPos.AddBinding(new DeviceBindingSource(InputControlType.RightStickUp));
			return;
		}
		SRInput.Actions.horizontalNeg.AddBinding(new DeviceBindingSource(InputControlType.LeftStickLeft));
		SRInput.Actions.horizontalPos.AddBinding(new DeviceBindingSource(InputControlType.LeftStickRight));
		SRInput.Actions.verticalNeg.AddBinding(new DeviceBindingSource(InputControlType.LeftStickDown));
		SRInput.Actions.verticalPos.AddBinding(new DeviceBindingSource(InputControlType.LeftStickUp));
	}

	// Token: 0x06000B87 RID: 2951 RVA: 0x00030C64 File Offset: 0x0002EE64
	private void UpdateGamepadStickBindings(SRInput.PlayerLookActions actions)
	{
		actions.lookXNeg.ClearBindingsOfTypes(new BindingSourceType[]
		{
			BindingSourceType.DeviceBindingSource
		});
		actions.lookXPos.ClearBindingsOfTypes(new BindingSourceType[]
		{
			BindingSourceType.DeviceBindingSource
		});
		actions.lookYNeg.ClearBindingsOfTypes(new BindingSourceType[]
		{
			BindingSourceType.DeviceBindingSource
		});
		actions.lookYPos.ClearBindingsOfTypes(new BindingSourceType[]
		{
			BindingSourceType.DeviceBindingSource
		});
		if (this.swapSticks)
		{
			actions.lookXNeg.AddBinding(new DeviceBindingSource(InputControlType.LeftStickLeft));
			actions.lookXPos.AddBinding(new DeviceBindingSource(InputControlType.LeftStickRight));
			actions.lookYNeg.AddBinding(new DeviceBindingSource(this.invertGamepadLookY ? InputControlType.LeftStickUp : InputControlType.LeftStickDown));
			actions.lookYPos.AddBinding(new DeviceBindingSource(this.invertGamepadLookY ? InputControlType.LeftStickDown : InputControlType.LeftStickUp));
			return;
		}
		actions.lookXNeg.AddBinding(new DeviceBindingSource(InputControlType.RightStickLeft));
		actions.lookXPos.AddBinding(new DeviceBindingSource(InputControlType.RightStickRight));
		actions.lookYNeg.AddBinding(new DeviceBindingSource(this.invertGamepadLookY ? InputControlType.RightStickUp : InputControlType.RightStickDown));
		actions.lookYPos.AddBinding(new DeviceBindingSource(this.invertGamepadLookY ? InputControlType.RightStickDown : InputControlType.RightStickUp));
	}

	// Token: 0x1700015A RID: 346
	// (get) Token: 0x06000B88 RID: 2952 RVA: 0x00030D8B File Offset: 0x0002EF8B
	// (set) Token: 0x06000B89 RID: 2953 RVA: 0x00030D93 File Offset: 0x0002EF93
	public float MouseLookSensitivity
	{
		get
		{
			return this.mouseLookSensitivity;
		}
		set
		{
			this.mouseLookSensitivity = value;
			this.NoteNewPlayer(SRSingleton<SceneContext>.Instance.Player);
		}
	}

	// Token: 0x1700015B RID: 347
	// (get) Token: 0x06000B8A RID: 2954 RVA: 0x00030DAC File Offset: 0x0002EFAC
	// (set) Token: 0x06000B8B RID: 2955 RVA: 0x00030DB4 File Offset: 0x0002EFB4
	public float GamepadLookSensitivityX
	{
		get
		{
			return this.gamepadLookSensitivityX;
		}
		set
		{
			this.gamepadLookSensitivityX = value;
			this.NoteNewPlayer(SRSingleton<SceneContext>.Instance.Player);
		}
	}

	// Token: 0x1700015C RID: 348
	// (get) Token: 0x06000B8C RID: 2956 RVA: 0x00030DCD File Offset: 0x0002EFCD
	// (set) Token: 0x06000B8D RID: 2957 RVA: 0x00030DD5 File Offset: 0x0002EFD5
	public float GamepadLookSensitivityY
	{
		get
		{
			return this.gamepadLookSensitivityY;
		}
		set
		{
			this.gamepadLookSensitivityY = value;
			this.NoteNewPlayer(SRSingleton<SceneContext>.Instance.Player);
		}
	}

	// Token: 0x06000B8E RID: 2958 RVA: 0x00030DEE File Offset: 0x0002EFEE
	public void NoteNewPlayer(GameObject player)
	{
		this.SetPlayerMouseSensitivity(player);
		this.SetPlayerGamepadXSensitivity(player);
		this.SetPlayerGamepadYSensitivity(player);
	}

	// Token: 0x06000B8F RID: 2959 RVA: 0x00030E05 File Offset: 0x0002F005
	public float GetGamepadLookSensitivityXFactor()
	{
		return this.gamepadLookSensitivityXFactor;
	}

	// Token: 0x06000B90 RID: 2960 RVA: 0x00030E0D File Offset: 0x0002F00D
	public float GetGamepadLookSensitivityYFactor()
	{
		return this.gamepadLookSensitivityYFactor;
	}

	// Token: 0x06000B91 RID: 2961 RVA: 0x00030E15 File Offset: 0x0002F015
	public void SetGamepadLookSensitivityXFactor(float factor)
	{
		this.gamepadLookSensitivityXFactor = factor;
		this.UpdateGamepadStickBindings();
	}

	// Token: 0x06000B92 RID: 2962 RVA: 0x00030E24 File Offset: 0x0002F024
	public void SetGamepadLookSensitivityYFactor(float factor)
	{
		this.gamepadLookSensitivityYFactor = factor;
		this.UpdateGamepadStickBindings();
	}

	// Token: 0x06000B93 RID: 2963 RVA: 0x00030E33 File Offset: 0x0002F033
	public void SetMouseLookSensitivityFactor(float factor)
	{
		this.mouseLookSensitivityFactor = factor;
		this.UpdateMouseYAxis();
	}

	// Token: 0x06000B94 RID: 2964 RVA: 0x00030E44 File Offset: 0x0002F044
	private void SetPlayerMouseSensitivity(GameObject player)
	{
		if (player != null)
		{
			float num = Mathf.Pow(3f, this.mouseLookSensitivity);
			this.SetMouseLookSensitivityFactor(num);
		}
	}

	// Token: 0x06000B95 RID: 2965 RVA: 0x00030E74 File Offset: 0x0002F074
	private void SetPlayerGamepadXSensitivity(GameObject player)
	{
		if (player != null)
		{
			float num = Mathf.Pow(3f, this.gamepadLookSensitivityX);
			this.SetGamepadLookSensitivityXFactor(num);
		}
	}

	// Token: 0x06000B96 RID: 2966 RVA: 0x00030EA4 File Offset: 0x0002F0A4
	private void SetPlayerGamepadYSensitivity(GameObject player)
	{
		if (player != null)
		{
			float num = Mathf.Pow(3f, this.gamepadLookSensitivityY);
			this.SetGamepadLookSensitivityYFactor(num);
		}
	}

	// Token: 0x04000A88 RID: 2696
	public InputDirector.OnKeysChanged onKeysChanged;

	// Token: 0x04000A89 RID: 2697
	public GameObject bugReportPrefab;

	// Token: 0x04000A8A RID: 2698
	private float mouseLookSensitivity;

	// Token: 0x04000A8B RID: 2699
	private float gamepadLookSensitivityX;

	// Token: 0x04000A8C RID: 2700
	private float gamepadLookSensitivityY = -0.2f;

	// Token: 0x04000A8D RID: 2701
	private float gamepadLookSensitivityXFactor = 1f;

	// Token: 0x04000A8E RID: 2702
	private float gamepadLookSensitivityYFactor = 1f;

	// Token: 0x04000A8F RID: 2703
	private float mouseLookSensitivityFactor = 1f;

	// Token: 0x04000A90 RID: 2704
	private float controllerStickDeadZone;

	// Token: 0x04000A91 RID: 2705
	private bool oldUsingGamepad;

	// Token: 0x04000A92 RID: 2706
	private bool swapSticks;

	// Token: 0x04000A93 RID: 2707
	private bool invertGamepadLookY;

	// Token: 0x04000A94 RID: 2708
	private bool invertMouseLookY;

	// Token: 0x04000A95 RID: 2709
	private bool disableMouseLookSmooth;

	// Token: 0x04000A96 RID: 2710
	private SRInput input;

	// Token: 0x04000A97 RID: 2711
	private HashSet<KeyCode> protectedKeyCodes = new HashSet<KeyCode>();

	// Token: 0x04000A98 RID: 2712
	private HashSet<string> protectedButtons = new HashSet<string>();

	// Token: 0x04000A99 RID: 2713
	private static InputDirector.DefaultBinding[] DEFAULTS;

	// Token: 0x04000A9A RID: 2714
	private static InputDirector.DefaultBinding[] EDITOR_DEFAULTS;

	// Token: 0x0200021B RID: 539
	public class ScalableMouseBindingSource : MouseBindingSource
	{
		// Token: 0x06000B99 RID: 2969 RVA: 0x00030F29 File Offset: 0x0002F129
		public ScalableMouseBindingSource(Mouse control, float sensitivity) : base(control)
		{
			this.sensitivity = sensitivity;
		}

		// Token: 0x06000B9A RID: 2970 RVA: 0x00030F39 File Offset: 0x0002F139
		public override float GetValue(InputDevice device)
		{
			return base.GetValue(device) * this.sensitivity;
		}

		// Token: 0x04000A9B RID: 2715
		private float sensitivity;
	}

	// Token: 0x0200021C RID: 540
	// (Invoke) Token: 0x06000B9C RID: 2972
	public delegate void OnKeysChanged();

	// Token: 0x0200021D RID: 541
	private class DefaultBinding
	{
		// Token: 0x06000B9F RID: 2975 RVA: 0x00030F49 File Offset: 0x0002F149
		public DefaultBinding(PlayerAction bindTo, Key primKey, Key secKey, InputControlType primBtn, InputControlType secBtn = InputControlType.None, InputControlType tertBtn = InputControlType.None)
		{
			this.bindTo = bindTo;
			this.primKey = primKey;
			this.secKey = secKey;
			this.primBtn = primBtn;
			this.secBtn = secBtn;
			this.tertBtn = tertBtn;
		}

		// Token: 0x06000BA0 RID: 2976 RVA: 0x00030F7E File Offset: 0x0002F17E
		public DefaultBinding(PlayerAction bindTo, Mouse primMouse, Key secKey, InputControlType primBtn, InputControlType secBtn = InputControlType.None, InputControlType tertBtn = InputControlType.None)
		{
			this.bindTo = bindTo;
			this.primMouse = primMouse;
			this.secKey = secKey;
			this.primBtn = primBtn;
			this.secBtn = secBtn;
			this.tertBtn = tertBtn;
		}

		// Token: 0x06000BA1 RID: 2977 RVA: 0x00030FB4 File Offset: 0x0002F1B4
		public void ApplyDefaultBinding()
		{
			this.bindTo.ClearBindings();
			if (this.primKey != Key.None)
			{
				this.bindTo.AddDefaultBinding(new Key[]
				{
					this.primKey
				});
			}
			if (this.primMouse != Mouse.None)
			{
				this.bindTo.AddDefaultBinding(this.primMouse);
			}
			if (this.secKey != Key.None)
			{
				this.bindTo.AddDefaultBinding(new Key[]
				{
					this.secKey
				});
			}
			if (this.primBtn != InputControlType.None)
			{
				this.bindTo.AddDefaultBinding(this.primBtn);
			}
			if (this.secBtn != InputControlType.None)
			{
				this.bindTo.AddDefaultBinding(this.secBtn);
			}
			if (this.tertBtn != InputControlType.None)
			{
				this.bindTo.AddDefaultBinding(this.tertBtn);
			}
		}

		// Token: 0x04000A9C RID: 2716
		public PlayerAction bindTo;

		// Token: 0x04000A9D RID: 2717
		public Mouse primMouse;

		// Token: 0x04000A9E RID: 2718
		public Key primKey;

		// Token: 0x04000A9F RID: 2719
		public Key secKey;

		// Token: 0x04000AA0 RID: 2720
		public InputControlType primBtn;

		// Token: 0x04000AA1 RID: 2721
		public InputControlType secBtn;

		// Token: 0x04000AA2 RID: 2722
		public InputControlType tertBtn;
	}
}
