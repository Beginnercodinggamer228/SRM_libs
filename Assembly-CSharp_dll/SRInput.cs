using System;
using System.Collections.ObjectModel;
using InControl;
using MonomiPark.SlimeRancher.Persist;
using MonomiPark.SlimeRancher.Utility;
using UnityEngine;

// Token: 0x0200036A RID: 874
public class SRInput
{
	// Token: 0x06001220 RID: 4640 RVA: 0x00047E60 File Offset: 0x00046060
	private SRInput()
	{
		this.PROTECTED_KEYS = new Key[]
		{
			Key.Pause,
			SRInput.GetDefaultMenuKey()
		};
		this.PROTECTED_CONTROLS = new InputControlType[]
		{
			InputControlType.Start,
			InputControlType.Menu
		};
		this.actions = new SRInput.PlayerActions(this);
		this.pauseActions = new SRInput.PlayerPauseActions();
		this.engageActions = new SRInput.PlayerEngageActions();
		this.lookActions = new SRInput.PlayerLookActions();
		this.SetInputMode(SRInput.InputMode.DEFAULT, 0);
		this.PROTECTED_ACTIONS = new PlayerAction[]
		{
			this.actions.menu,
			this.pauseActions.unmenu
		};
	}

	// Token: 0x17000191 RID: 401
	// (get) Token: 0x06001221 RID: 4641 RVA: 0x00047F0D File Offset: 0x0004610D
	public static SRInput Instance
	{
		get
		{
			if (SRInput.instance == null && Application.isPlaying)
			{
				SRInput.instance = new SRInput();
			}
			return SRInput.instance;
		}
	}

	// Token: 0x17000192 RID: 402
	// (get) Token: 0x06001222 RID: 4642 RVA: 0x00047F2C File Offset: 0x0004612C
	public static SRInput.PlayerActions Actions
	{
		get
		{
			return SRInput.Instance.actions;
		}
	}

	// Token: 0x17000193 RID: 403
	// (get) Token: 0x06001223 RID: 4643 RVA: 0x00047F38 File Offset: 0x00046138
	public static SRInput.PlayerPauseActions PauseActions
	{
		get
		{
			return SRInput.Instance.pauseActions;
		}
	}

	// Token: 0x17000194 RID: 404
	// (get) Token: 0x06001224 RID: 4644 RVA: 0x00047F44 File Offset: 0x00046144
	public static SRInput.PlayerEngageActions EngageActions
	{
		get
		{
			return SRInput.Instance.engageActions;
		}
	}

	// Token: 0x17000195 RID: 405
	// (get) Token: 0x06001225 RID: 4645 RVA: 0x00047F50 File Offset: 0x00046150
	public static SRInput.PlayerLookActions LookActions
	{
		get
		{
			return SRInput.Instance.lookActions;
		}
	}

	// Token: 0x06001226 RID: 4646 RVA: 0x00047F5C File Offset: 0x0004615C
	public static BindingSource GetPrimKeyBinding(PlayerAction action)
	{
		ReadOnlyCollection<BindingSource> readOnlyCollection = action.BindingsOfTypes(new BindingSourceType[]
		{
			BindingSourceType.KeyBindingSource,
			BindingSourceType.MouseBindingSource
		});
		if (readOnlyCollection.Count >= 1)
		{
			return readOnlyCollection[0];
		}
		return null;
	}

	// Token: 0x06001227 RID: 4647 RVA: 0x00047F90 File Offset: 0x00046190
	public static BindingSource GetSecKeyBinding(PlayerAction action)
	{
		ReadOnlyCollection<BindingSource> readOnlyCollection = action.BindingsOfTypes(new BindingSourceType[]
		{
			BindingSourceType.KeyBindingSource,
			BindingSourceType.MouseBindingSource
		});
		if (readOnlyCollection.Count >= 2)
		{
			return readOnlyCollection[1];
		}
		return null;
	}

	// Token: 0x06001228 RID: 4648 RVA: 0x00047FC4 File Offset: 0x000461C4
	public static BindingSource GetPrimGamepadBinding(PlayerAction action)
	{
		ReadOnlyCollection<BindingSource> readOnlyCollection = action.BindingsOfTypes(new BindingSourceType[]
		{
			BindingSourceType.DeviceBindingSource
		});
		if (readOnlyCollection.Count >= 1)
		{
			return readOnlyCollection[0];
		}
		return null;
	}

	// Token: 0x06001229 RID: 4649 RVA: 0x00047FF4 File Offset: 0x000461F4
	public static BindingSource GetSecGamepadBinding(PlayerAction action)
	{
		ReadOnlyCollection<BindingSource> readOnlyCollection = action.BindingsOfTypes(new BindingSourceType[]
		{
			BindingSourceType.DeviceBindingSource
		});
		if (readOnlyCollection.Count >= 2)
		{
			return readOnlyCollection[1];
		}
		return null;
	}

	// Token: 0x0600122A RID: 4650 RVA: 0x00048024 File Offset: 0x00046224
	public static BindingSource GetBinding(PlayerAction action, SRInput.ButtonType type)
	{
		switch (type)
		{
		case SRInput.ButtonType.PRIMARY:
			return SRInput.GetPrimKeyBinding(action);
		case SRInput.ButtonType.SECONDARY:
			return SRInput.GetSecKeyBinding(action);
		case SRInput.ButtonType.GAMEPAD:
			return SRInput.GetPrimGamepadBinding(action);
		case SRInput.ButtonType.GAMEPAD_SEC:
			return SRInput.GetSecGamepadBinding(action);
		default:
			return null;
		}
	}

	// Token: 0x0600122B RID: 4651 RVA: 0x0004805C File Offset: 0x0004625C
	public static string GetButtonKey(PlayerAction action, SRInput.ButtonType type)
	{
		BindingSource binding = SRInput.GetBinding(action, type);
		if (binding is KeyBindingSource)
		{
			return (binding as KeyBindingSource).Control.GetInclude(0).ToString();
		}
		if (binding is MouseBindingSource)
		{
			return (binding as MouseBindingSource).Control.ToString();
		}
		if (binding is DeviceBindingSource)
		{
			return (binding as DeviceBindingSource).Control.ToString();
		}
		return null;
	}

	// Token: 0x0600122C RID: 4652 RVA: 0x000480E4 File Offset: 0x000462E4
	public static bool IsProtected(PlayerAction action)
	{
		PlayerAction[] protected_ACTIONS = SRInput.Instance.PROTECTED_ACTIONS;
		for (int i = 0; i < protected_ACTIONS.Length; i++)
		{
			if (protected_ACTIONS[i] == action)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600122D RID: 4653 RVA: 0x00048114 File Offset: 0x00046314
	public static bool IsProtected(params Key[] keys)
	{
		foreach (Key key in keys)
		{
			Key[] protected_KEYS = SRInput.Instance.PROTECTED_KEYS;
			for (int j = 0; j < protected_KEYS.Length; j++)
			{
				if (protected_KEYS[j] == key)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600122E RID: 4654 RVA: 0x0004815C File Offset: 0x0004635C
	public static PlayerAction GetAction(string actionName)
	{
		PlayerAction playerAction = SRInput.Actions.Get(actionName);
		if (playerAction == null)
		{
			playerAction = SRInput.PauseActions.Get(actionName);
		}
		return playerAction;
	}

	// Token: 0x0600122F RID: 4655 RVA: 0x00048185 File Offset: 0x00046385
	public static Key GetDefaultMenuKey()
	{
		if (!Application.isEditor)
		{
			return Key.Escape;
		}
		return Key.Backquote;
	}

	// Token: 0x06001230 RID: 4656 RVA: 0x00048194 File Offset: 0x00046394
	public void SetInputMode(SRInput.InputMode mode, int handle)
	{
		if (this.inputModeStack.Push(mode, handle))
		{
			this.SetInputMode(mode);
			return;
		}
		Log.Error("Failed to set input mode!", new object[]
		{
			"mode",
			mode,
			"handle",
			handle
		});
	}

	// Token: 0x06001231 RID: 4657 RVA: 0x000481EC File Offset: 0x000463EC
	public void ClearInputMode(int handle)
	{
		this.inputModeStack.Pop(handle);
		SRInput.InputMode inputMode = this.inputModeStack.Peek();
		if (inputMode != this.GetInputMode())
		{
			this.SetInputMode(inputMode);
		}
	}

	// Token: 0x06001232 RID: 4658 RVA: 0x00048224 File Offset: 0x00046424
	private void SetInputMode(SRInput.InputMode mode)
	{
		this.actions.Enabled = (mode == SRInput.InputMode.DEFAULT);
		this.pauseActions.Enabled = (mode == SRInput.InputMode.PAUSE);
		this.engageActions.Enabled = (mode == SRInput.InputMode.ENGAGEMENT);
		this.lookActions.Enabled = (mode == SRInput.InputMode.LOOK_ONLY);
		Log.Debug("Setting input mode", new object[]
		{
			"mode",
			mode
		});
	}

	// Token: 0x06001233 RID: 4659 RVA: 0x0004828E File Offset: 0x0004648E
	public SRInput.InputMode GetInputMode()
	{
		if (this.actions.Enabled)
		{
			return SRInput.InputMode.DEFAULT;
		}
		if (this.pauseActions.Enabled)
		{
			return SRInput.InputMode.PAUSE;
		}
		if (this.engageActions.Enabled)
		{
			return SRInput.InputMode.ENGAGEMENT;
		}
		if (!this.lookActions.Enabled)
		{
			return SRInput.InputMode.NONE;
		}
		return SRInput.InputMode.LOOK_ONLY;
	}

	// Token: 0x06001234 RID: 4660 RVA: 0x000482CD File Offset: 0x000464CD
	public Vector2 GetMouseLook()
	{
		return SRInput.Actions.GetMouseLook() + SRInput.LookActions.GetMouseLook();
	}

	// Token: 0x06001235 RID: 4661 RVA: 0x000482E8 File Offset: 0x000464E8
	public Vector2 GetMouseLookRaw()
	{
		return SRInput.Actions.GetMouseLookRaw() + SRInput.LookActions.GetMouseLookRaw();
	}

	// Token: 0x06001236 RID: 4662 RVA: 0x00048304 File Offset: 0x00046504
	public static BindingV01 ToBinding(PlayerAction action)
	{
		BindingV01 bindingV = new BindingV01();
		bindingV.action = action.Name;
		BindingSource primKeyBinding = SRInput.GetPrimKeyBinding(action);
		if (primKeyBinding is KeyBindingSource)
		{
			bindingV.primKey = (int)((KeyBindingSource)primKeyBinding).Control.GetInclude(0);
		}
		else if (primKeyBinding is MouseBindingSource)
		{
			bindingV.primMouse = (int)((MouseBindingSource)primKeyBinding).Control;
		}
		BindingSource secKeyBinding = SRInput.GetSecKeyBinding(action);
		if (secKeyBinding is KeyBindingSource)
		{
			bindingV.secKey = (int)((KeyBindingSource)secKeyBinding).Control.GetInclude(0);
		}
		else if (secKeyBinding is MouseBindingSource)
		{
			bindingV.secMouse = (int)((MouseBindingSource)secKeyBinding).Control;
		}
		BindingSource primGamepadBinding = SRInput.GetPrimGamepadBinding(action);
		if (primGamepadBinding is DeviceBindingSource)
		{
			bindingV.gamepad = (int)((DeviceBindingSource)primGamepadBinding).Control;
		}
		return bindingV;
	}

	// Token: 0x06001237 RID: 4663 RVA: 0x000483CE File Offset: 0x000465CE
	public static bool AddOrReplaceBinding(PlayerAction action, PlayerAction source)
	{
		return SRInput.AddOrReplaceBinding(action, SRInput.ToBinding(source));
	}

	// Token: 0x06001238 RID: 4664 RVA: 0x000483DC File Offset: 0x000465DC
	public static bool AddOrReplaceBinding(PlayerAction action, BindingV01 binding)
	{
		if (SRInput.IsProtected(action))
		{
			Log.Warning("Ignoring key override for protected binding, using defaults.", new object[]
			{
				"binding",
				binding.action
			});
			return false;
		}
		if (SRInput.IsProtected(new Key[]
		{
			(Key)binding.primKey,
			(Key)binding.secKey
		}))
		{
			Log.Warning("Ignoring key override for protected key, using defaults.", new object[]
			{
				"binding",
				binding.action,
				"binding.primKey",
				binding.primKey,
				"binding.secKey",
				binding.secKey
			});
			return false;
		}
		if (binding.primKey != 0)
		{
			action.AddOrReplaceBinding(SRInput.GetPrimKeyBinding(action), new KeyBindingSource(new Key[]
			{
				(Key)binding.primKey
			}));
		}
		else if (binding.primMouse != 0)
		{
			action.AddOrReplaceBinding(SRInput.GetPrimKeyBinding(action), new MouseBindingSource((Mouse)binding.primMouse));
		}
		else
		{
			action.AddOrReplaceBinding(SRInput.GetPrimKeyBinding(action), null);
		}
		if (binding.secKey != 0)
		{
			action.AddOrReplaceBinding(SRInput.GetSecKeyBinding(action), new KeyBindingSource(new Key[]
			{
				(Key)binding.secKey
			}));
		}
		else if (binding.secMouse != 0)
		{
			action.AddOrReplaceBinding(SRInput.GetSecKeyBinding(action), new MouseBindingSource((Mouse)binding.secMouse));
		}
		else
		{
			action.AddOrReplaceBinding(SRInput.GetSecKeyBinding(action), null);
		}
		if (binding.gamepad != 0)
		{
			action.AddOrReplaceBinding(SRInput.GetPrimGamepadBinding(action), new DeviceBindingSource((InputControlType)binding.gamepad));
		}
		else
		{
			action.AddOrReplaceBinding(SRInput.GetPrimGamepadBinding(action), null);
		}
		return true;
	}

	// Token: 0x04001128 RID: 4392
	public SRInput.PlayerActions actions;

	// Token: 0x04001129 RID: 4393
	public SRInput.PlayerPauseActions pauseActions;

	// Token: 0x0400112A RID: 4394
	public SRInput.PlayerEngageActions engageActions;

	// Token: 0x0400112B RID: 4395
	public SRInput.PlayerLookActions lookActions;

	// Token: 0x0400112C RID: 4396
	public readonly Key[] PROTECTED_KEYS;

	// Token: 0x0400112D RID: 4397
	public readonly InputControlType[] PROTECTED_CONTROLS;

	// Token: 0x0400112E RID: 4398
	public readonly PlayerAction[] PROTECTED_ACTIONS;

	// Token: 0x0400112F RID: 4399
	protected static SRInput instance;

	// Token: 0x04001130 RID: 4400
	private InputModeStack inputModeStack = new InputModeStack();

	// Token: 0x0200036B RID: 875
	public enum ButtonType
	{
		// Token: 0x04001132 RID: 4402
		PRIMARY,
		// Token: 0x04001133 RID: 4403
		SECONDARY,
		// Token: 0x04001134 RID: 4404
		GAMEPAD,
		// Token: 0x04001135 RID: 4405
		GAMEPAD_SEC
	}

	// Token: 0x0200036C RID: 876
	public class PlayerLookActions : PlayerActionSet
	{
		// Token: 0x06001239 RID: 4665 RVA: 0x00048564 File Offset: 0x00046764
		public PlayerLookActions()
		{
			this.lookXNeg = base.CreatePlayerAction("LookXNeg");
			this.lookXPos = base.CreatePlayerAction("LookXPos");
			this.lookX = base.CreateOneAxisPlayerAction(this.lookXNeg, this.lookXPos);
			this.lookYNeg = base.CreatePlayerAction("LookYNeg");
			this.lookYPos = base.CreatePlayerAction("LookYPos");
			this.lookY = base.CreateOneAxisPlayerAction(this.lookYNeg, this.lookYPos);
		}

		// Token: 0x0600123A RID: 4666 RVA: 0x000485EB File Offset: 0x000467EB
		public Vector2 GetMouseLook()
		{
			return new Vector2(this.lookX, this.lookY);
		}

		// Token: 0x0600123B RID: 4667 RVA: 0x00048608 File Offset: 0x00046808
		public Vector2 GetMouseLookRaw()
		{
			return new Vector2(this.lookX.RawValue, this.lookY.RawValue);
		}

		// Token: 0x04001136 RID: 4406
		public PlayerAction lookXPos;

		// Token: 0x04001137 RID: 4407
		public PlayerAction lookXNeg;

		// Token: 0x04001138 RID: 4408
		public PlayerOneAxisAction lookX;

		// Token: 0x04001139 RID: 4409
		public PlayerAction lookYPos;

		// Token: 0x0400113A RID: 4410
		public PlayerAction lookYNeg;

		// Token: 0x0400113B RID: 4411
		public PlayerOneAxisAction lookY;
	}

	// Token: 0x0200036D RID: 877
	public class PlayerActions : SRInput.PlayerLookActions
	{
		// Token: 0x0600123C RID: 4668 RVA: 0x00048628 File Offset: 0x00046828
		public PlayerActions(SRInput srInput)
		{
			this.attack = base.CreatePlayerAction("Attack");
			this.vac = base.CreatePlayerAction("Vac");
			this.slimeFilter = base.CreatePlayerAction("SlimeFilter");
			this.jump = base.CreatePlayerAction("Jump");
			this.run = base.CreatePlayerAction("Run");
			this.interact = base.CreatePlayerAction("Interact");
			this.accept = base.CreatePlayerAction("Accept1");
			this.menu = base.CreatePlayerAction("Menu");
			this.radarToggle = base.CreatePlayerAction("RadarToggle");
			this.openMap = base.CreatePlayerAction("OpenMap");
			this.pedia = base.CreatePlayerAction("Pedia");
			this.reportIssue = base.CreatePlayerAction("ReportIssue");
			this.screenshot = base.CreatePlayerAction("Screenshot");
			this.recordGif = base.CreatePlayerAction("RecordGif");
			this.verticalNeg = base.CreatePlayerAction("VerticalNeg");
			this.verticalPos = base.CreatePlayerAction("VerticalPos");
			this.vertical = base.CreateOneAxisPlayerAction(this.verticalNeg, this.verticalPos);
			this.horizontalNeg = base.CreatePlayerAction("HorizontalNeg");
			this.horizontalPos = base.CreatePlayerAction("HorizontalPos");
			this.horizontal = base.CreateOneAxisPlayerAction(this.horizontalNeg, this.horizontalPos);
			this.slot1 = base.CreatePlayerAction("Slot1");
			this.slot2 = base.CreatePlayerAction("Slot2");
			this.slot3 = base.CreatePlayerAction("Slot3");
			this.slot4 = base.CreatePlayerAction("Slot4");
			this.slot5 = base.CreatePlayerAction("Slot5");
			this.prevSlot = base.CreatePlayerAction("PrevSlot");
			this.nextSlot = base.CreatePlayerAction("NextSlot");
			this.light = base.CreatePlayerAction("Light");
			this.burst = base.CreatePlayerAction("Burst");
			this.toggleGadgetMode = base.CreatePlayerAction("ToggleGadgetMode");
			base.ListenOptions.IncludeMouseButtons = true;
			base.ListenOptions.IncludeModifiersAsFirstClassKeys = true;
			base.ListenOptions.UnsetDuplicateBindingsOnSet = true;
			base.ListenOptions.DisallowBindingKeys = srInput.PROTECTED_KEYS;
			base.ListenOptions.DisallowBindingControls = srInput.PROTECTED_CONTROLS;
		}

		// Token: 0x0400113C RID: 4412
		public PlayerAction attack;

		// Token: 0x0400113D RID: 4413
		public PlayerAction vac;

		// Token: 0x0400113E RID: 4414
		public PlayerAction slimeFilter;

		// Token: 0x0400113F RID: 4415
		public PlayerAction jump;

		// Token: 0x04001140 RID: 4416
		public PlayerAction run;

		// Token: 0x04001141 RID: 4417
		public PlayerAction interact;

		// Token: 0x04001142 RID: 4418
		public PlayerAction accept;

		// Token: 0x04001143 RID: 4419
		public PlayerAction menu;

		// Token: 0x04001144 RID: 4420
		public PlayerAction radarToggle;

		// Token: 0x04001145 RID: 4421
		public PlayerAction openMap;

		// Token: 0x04001146 RID: 4422
		public PlayerAction pedia;

		// Token: 0x04001147 RID: 4423
		public PlayerAction reportIssue;

		// Token: 0x04001148 RID: 4424
		public PlayerAction screenshot;

		// Token: 0x04001149 RID: 4425
		public PlayerAction recordGif;

		// Token: 0x0400114A RID: 4426
		public PlayerAction verticalNeg;

		// Token: 0x0400114B RID: 4427
		public PlayerAction verticalPos;

		// Token: 0x0400114C RID: 4428
		public PlayerOneAxisAction vertical;

		// Token: 0x0400114D RID: 4429
		public PlayerAction horizontalNeg;

		// Token: 0x0400114E RID: 4430
		public PlayerAction horizontalPos;

		// Token: 0x0400114F RID: 4431
		public PlayerOneAxisAction horizontal;

		// Token: 0x04001150 RID: 4432
		public PlayerAction slot1;

		// Token: 0x04001151 RID: 4433
		public PlayerAction slot2;

		// Token: 0x04001152 RID: 4434
		public PlayerAction slot3;

		// Token: 0x04001153 RID: 4435
		public PlayerAction slot4;

		// Token: 0x04001154 RID: 4436
		public PlayerAction slot5;

		// Token: 0x04001155 RID: 4437
		public PlayerAction prevSlot;

		// Token: 0x04001156 RID: 4438
		public PlayerAction nextSlot;

		// Token: 0x04001157 RID: 4439
		public PlayerAction light;

		// Token: 0x04001158 RID: 4440
		public PlayerAction burst;

		// Token: 0x04001159 RID: 4441
		public PlayerAction toggleGadgetMode;
	}

	// Token: 0x0200036E RID: 878
	public class PlayerPauseActions : PlayerActionSet
	{
		// Token: 0x0600123D RID: 4669 RVA: 0x00048890 File Offset: 0x00046A90
		public PlayerPauseActions()
		{
			this.submit = base.CreatePlayerAction("Submit");
			this.altSubmit = base.CreatePlayerAction("AltSubmit");
			this.cancel = base.CreatePlayerAction("Cancel");
			this.unmenu = base.CreatePlayerAction("Unmenu");
			this.menuUp = base.CreatePlayerAction("MenuUp");
			this.menuDown = base.CreatePlayerAction("MenuDown");
			this.menuLeft = base.CreatePlayerAction("MenuLeft");
			this.menuRight = base.CreatePlayerAction("MenuRight");
			this.menuTabLeft = base.CreatePlayerAction("MenuTabLeft");
			this.menuTabRight = base.CreatePlayerAction("MenuTabRight");
			this.menuScrollUp = base.CreatePlayerAction("MenuScrollUp");
			this.menuScrollDown = base.CreatePlayerAction("MenuScrollDown");
			this.closeMap = base.CreatePlayerAction("CloseMap");
			this.switchUser = base.CreatePlayerAction("SwitchUser");
		}

		// Token: 0x0400115A RID: 4442
		public PlayerAction submit;

		// Token: 0x0400115B RID: 4443
		public PlayerAction altSubmit;

		// Token: 0x0400115C RID: 4444
		public PlayerAction cancel;

		// Token: 0x0400115D RID: 4445
		public PlayerAction menuUp;

		// Token: 0x0400115E RID: 4446
		public PlayerAction menuDown;

		// Token: 0x0400115F RID: 4447
		public PlayerAction menuLeft;

		// Token: 0x04001160 RID: 4448
		public PlayerAction menuRight;

		// Token: 0x04001161 RID: 4449
		public PlayerAction menuTabLeft;

		// Token: 0x04001162 RID: 4450
		public PlayerAction menuTabRight;

		// Token: 0x04001163 RID: 4451
		public PlayerAction menuScrollUp;

		// Token: 0x04001164 RID: 4452
		public PlayerAction menuScrollDown;

		// Token: 0x04001165 RID: 4453
		public PlayerAction unmenu;

		// Token: 0x04001166 RID: 4454
		public PlayerAction closeMap;

		// Token: 0x04001167 RID: 4455
		public PlayerAction switchUser;
	}

	// Token: 0x0200036F RID: 879
	public class PlayerEngageActions : PlayerActionSet
	{
		// Token: 0x0600123E RID: 4670 RVA: 0x00048991 File Offset: 0x00046B91
		public PlayerEngageActions()
		{
			this.engage = base.CreatePlayerAction("Engage");
		}

		// Token: 0x04001168 RID: 4456
		public PlayerAction engage;
	}

	// Token: 0x02000370 RID: 880
	public enum InputMode
	{
		// Token: 0x0400116A RID: 4458
		NONE,
		// Token: 0x0400116B RID: 4459
		DEFAULT,
		// Token: 0x0400116C RID: 4460
		PAUSE,
		// Token: 0x0400116D RID: 4461
		ENGAGEMENT,
		// Token: 0x0400116E RID: 4462
		LOOK_ONLY
	}
}
