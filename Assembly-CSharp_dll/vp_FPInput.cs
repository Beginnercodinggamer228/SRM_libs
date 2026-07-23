using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000880 RID: 2176
public class vp_FPInput : vp_Component
{
	// Token: 0x17000306 RID: 774
	// (get) Token: 0x06002E91 RID: 11921 RVA: 0x000B46C3 File Offset: 0x000B28C3
	public Vector2 MousePos
	{
		get
		{
			return this.m_MousePos;
		}
	}

	// Token: 0x17000307 RID: 775
	// (get) Token: 0x06002E92 RID: 11922 RVA: 0x000B46CB File Offset: 0x000B28CB
	// (set) Token: 0x06002E93 RID: 11923 RVA: 0x000B46D3 File Offset: 0x000B28D3
	public bool AllowGameplayInput
	{
		get
		{
			return this.m_AllowGameplayInput;
		}
		set
		{
			this.m_AllowGameplayInput = value;
		}
	}

	// Token: 0x17000308 RID: 776
	// (get) Token: 0x06002E94 RID: 11924 RVA: 0x000B46DC File Offset: 0x000B28DC
	public vp_FPPlayerEventHandler FPPlayer
	{
		get
		{
			if (this.m_FPPlayer == null)
			{
				this.m_FPPlayer = base.transform.root.GetComponentInChildren<vp_FPPlayerEventHandler>();
			}
			return this.m_FPPlayer;
		}
	}

	// Token: 0x06002E95 RID: 11925 RVA: 0x000B4708 File Offset: 0x000B2908
	protected override void Awake()
	{
		base.Awake();
		this.optionsDir = SRSingleton<GameContext>.Instance.OptionsDirector;
		this.inputDir = SRSingleton<GameContext>.Instance.InputDirector;
	}

	// Token: 0x06002E96 RID: 11926 RVA: 0x000B4730 File Offset: 0x000B2930
	protected override void OnEnable()
	{
		if (this.FPPlayer != null)
		{
			this.Register(this.FPPlayer);
		}
	}

	// Token: 0x06002E97 RID: 11927 RVA: 0x000B474C File Offset: 0x000B294C
	protected override void OnDisable()
	{
		if (this.FPPlayer != null)
		{
			this.Unregister(this.FPPlayer);
		}
	}

	// Token: 0x06002E98 RID: 11928 RVA: 0x000B4768 File Offset: 0x000B2968
	protected override void Update()
	{
		this.UpdateCursorLock();
		this.UpdatePause();
		if (this.FPPlayer.Pause.Get())
		{
			return;
		}
		if (!this.m_AllowGameplayInput)
		{
			return;
		}
		this.InputInteract();
		this.InputMove();
		this.InputRun();
		this.InputJump();
		this.InputCrouch();
		this.InputAttack();
		this.InputReload();
		this.InputSetWeapon();
		this.InputCamera();
	}

	// Token: 0x06002E99 RID: 11929 RVA: 0x000B47D8 File Offset: 0x000B29D8
	protected virtual void InputInteract()
	{
		if (SRInput.Actions.interact.WasReleased)
		{
			this.FPPlayer.Interact.TryStart(true);
			return;
		}
		this.FPPlayer.Interact.TryStop(true);
	}

	// Token: 0x06002E9A RID: 11930 RVA: 0x000B4810 File Offset: 0x000B2A10
	protected virtual void InputMove()
	{
		Vector2 vector = new Vector2(SRInput.Actions.horizontal, SRInput.Actions.vertical);
		Vector2 o = InputDirector.UsingGamepad() ? this.ApplyRadialDeadZone(vector, this.inputDir.ControllerStickDeadZone) : vector;
		this.FPPlayer.InputMoveVector.Set(o);
	}

	// Token: 0x06002E9B RID: 11931 RVA: 0x000B4878 File Offset: 0x000B2A78
	private Vector2 ApplyRadialDeadZone(Vector2 v, float deadZone)
	{
		float magnitude = v.magnitude;
		Vector2 result;
		if (magnitude < deadZone)
		{
			result = Vector2.zero;
		}
		else
		{
			result = v.normalized * ((magnitude - deadZone) / (1f - deadZone));
		}
		return result;
	}

	// Token: 0x06002E9C RID: 11932 RVA: 0x000B48B4 File Offset: 0x000B2AB4
	protected virtual void InputRun()
	{
		if (this.optionsDir.sprintHold ? SRInput.Actions.run.IsPressed : (this.FPPlayer.Run.Active ^ SRInput.Actions.run.WasPressed))
		{
			this.FPPlayer.Run.TryStart(true);
			return;
		}
		this.FPPlayer.Run.TryStop(true);
	}

	// Token: 0x06002E9D RID: 11933 RVA: 0x000B4928 File Offset: 0x000B2B28
	protected virtual void InputJump()
	{
		if (SRInput.Actions.jump.IsPressed)
		{
			if (!this.FPPlayer.Jump.TryStart(true) && !this.FPPlayer.Jump.Active)
			{
				this.FPPlayer.Jetpack.TryStart(true);
				return;
			}
		}
		else
		{
			this.FPPlayer.Jump.Stop(0f);
			this.FPPlayer.Jetpack.Stop(0f);
		}
	}

	// Token: 0x06002E9E RID: 11934 RVA: 0x00003296 File Offset: 0x00001496
	protected virtual void InputCrouch()
	{
	}

	// Token: 0x06002E9F RID: 11935 RVA: 0x00003296 File Offset: 0x00001496
	protected virtual void InputCamera()
	{
	}

	// Token: 0x06002EA0 RID: 11936 RVA: 0x000B49A8 File Offset: 0x000B2BA8
	protected virtual void InputAttack()
	{
		if (!vp_Utility.LockCursor)
		{
			return;
		}
		if (SRInput.Actions.attack.IsPressed)
		{
			this.FPPlayer.Attack.TryStart(true);
			return;
		}
		this.FPPlayer.Attack.TryStop(true);
	}

	// Token: 0x06002EA1 RID: 11937 RVA: 0x00003296 File Offset: 0x00001496
	protected virtual void InputReload()
	{
	}

	// Token: 0x06002EA2 RID: 11938 RVA: 0x00003296 File Offset: 0x00001496
	protected virtual void InputSetWeapon()
	{
	}

	// Token: 0x06002EA3 RID: 11939 RVA: 0x00003296 File Offset: 0x00001496
	protected virtual void UpdatePause()
	{
	}

	// Token: 0x06002EA4 RID: 11940 RVA: 0x000B49E8 File Offset: 0x000B2BE8
	protected virtual void UpdateCursorLock()
	{
		this.m_MousePos.x = Input.mousePosition.x;
		this.m_MousePos.y = (float)Screen.height - Input.mousePosition.y;
		if (this.MouseCursorForced)
		{
			if (vp_Utility.LockCursor)
			{
				vp_Utility.LockCursor = false;
			}
			return;
		}
		if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
		{
			if (this.MouseCursorZones.Length != 0)
			{
				foreach (Rect rect in this.MouseCursorZones)
				{
					if (rect.Contains(this.m_MousePos))
					{
						if (vp_Utility.LockCursor)
						{
							vp_Utility.LockCursor = false;
						}
						return;
					}
				}
			}
			if (!vp_Utility.LockCursor)
			{
				vp_Utility.LockCursor = true;
			}
		}
	}

	// Token: 0x06002EA5 RID: 11941 RVA: 0x000B4AA4 File Offset: 0x000B2CA4
	protected virtual Vector2 GetMouseLook()
	{
		if (this.MouseCursorBlocksMouseLook && !vp_Utility.LockCursor)
		{
			return Vector2.zero;
		}
		if (this.m_LastMouseLookFrame == Time.frameCount)
		{
			return this.m_CurrentMouseLook;
		}
		this.m_LastMouseLookFrame = Time.frameCount;
		Vector2 mouseLook = SRInput.Instance.GetMouseLook();
		Vector2 vector = mouseLook;
		if (InputDirector.UsingGamepad())
		{
			vector = this.ApplyRadialDeadZone(mouseLook, this.inputDir.ControllerStickDeadZone);
			vector.x *= this.inputDir.GetGamepadLookSensitivityXFactor();
			vector.y *= this.inputDir.GetGamepadLookSensitivityYFactor();
		}
		this.m_MouseLookSmoothMove.x = vector.x * Time.timeScale;
		this.m_MouseLookSmoothMove.y = vector.y * Time.timeScale;
		this.MouseLookSmoothSteps = Mathf.Clamp(this.MouseLookSmoothSteps, 1, 20);
		float num = this.inputDir.GetDisableMouseLookSmooth() ? 0f : (Mathf.Clamp01(this.MouseLookSmoothWeight) / base.Delta);
		while (this.m_MouseLookSmoothBuffer.Count > this.MouseLookSmoothSteps)
		{
			this.m_MouseLookSmoothBuffer.RemoveAt(0);
		}
		this.m_MouseLookSmoothBuffer.Add(this.m_MouseLookSmoothMove);
		float num2 = 1f;
		Vector2 a = Vector2.zero;
		float num3 = 0f;
		for (int i = this.m_MouseLookSmoothBuffer.Count - 1; i > 0; i--)
		{
			a += this.m_MouseLookSmoothBuffer[i] * num2;
			num3 += 1f * num2;
			num2 *= num;
		}
		num3 = Mathf.Max(1f, num3);
		this.m_CurrentMouseLook = vp_MathUtility.NaNSafeVector2(a / num3, default(Vector2));
		float num4 = 0f;
		float num5 = Mathf.Abs(this.m_CurrentMouseLook.x);
		float num6 = Mathf.Abs(this.m_CurrentMouseLook.y);
		if (this.MouseLookAcceleration)
		{
			num4 = Mathf.Sqrt(num5 * num5 + num6 * num6) / base.Delta;
			num4 = ((num4 <= this.MouseLookAccelerationThreshold) ? 0f : num4);
		}
		this.m_CurrentMouseLook.x = this.m_CurrentMouseLook.x * (this.MouseLookSensitivity.x + num4);
		this.m_CurrentMouseLook.y = this.m_CurrentMouseLook.y * (this.MouseLookSensitivity.y + num4);
		this.m_CurrentMouseLook.y = (this.MouseLookInvert ? this.m_CurrentMouseLook.y : (-this.m_CurrentMouseLook.y));
		return this.m_CurrentMouseLook;
	}

	// Token: 0x06002EA6 RID: 11942 RVA: 0x000B4D30 File Offset: 0x000B2F30
	protected virtual Vector2 GetMouseLookRaw()
	{
		if (this.MouseCursorBlocksMouseLook && !vp_Utility.LockCursor)
		{
			return Vector2.zero;
		}
		Vector2 mouseLookRaw = SRInput.Instance.GetMouseLookRaw();
		Vector2 vector = this.ApplyRadialDeadZone(mouseLookRaw, this.inputDir.ControllerStickDeadZone);
		this.m_MouseLookRawMove.x = vector.x;
		this.m_MouseLookRawMove.y = vector.y;
		return this.m_MouseLookRawMove;
	}

	// Token: 0x06002EA7 RID: 11943 RVA: 0x000B4D98 File Offset: 0x000B2F98
	protected virtual Vector2 Get_InputMoveVector()
	{
		return this.m_MoveVector;
	}

	// Token: 0x06002EA8 RID: 11944 RVA: 0x000B4DA0 File Offset: 0x000B2FA0
	protected virtual void Set_InputMoveVector(Vector2 value)
	{
		this.m_MoveVector = ((value.sqrMagnitude > 1f) ? value.normalized : value);
	}

	// Token: 0x17000309 RID: 777
	// (get) Token: 0x06002EA9 RID: 11945 RVA: 0x000B4D98 File Offset: 0x000B2F98
	// (set) Token: 0x06002EAA RID: 11946 RVA: 0x000B4DA0 File Offset: 0x000B2FA0
	protected virtual Vector2 OnValue_InputMoveVector
	{
		get
		{
			return this.m_MoveVector;
		}
		set
		{
			this.m_MoveVector = ((value.sqrMagnitude > 1f) ? value.normalized : value);
		}
	}

	// Token: 0x06002EAB RID: 11947 RVA: 0x000B4DC0 File Offset: 0x000B2FC0
	protected virtual float Get_InputClimbVector()
	{
		return SRInput.Actions.vertical.RawValue;
	}

	// Token: 0x1700030A RID: 778
	// (get) Token: 0x06002EAC RID: 11948 RVA: 0x000B4DC0 File Offset: 0x000B2FC0
	protected virtual float OnValue_InputClimbVector
	{
		get
		{
			return SRInput.Actions.vertical.RawValue;
		}
	}

	// Token: 0x06002EAD RID: 11949 RVA: 0x000B46CB File Offset: 0x000B28CB
	protected virtual bool Get_InputAllowGameplay()
	{
		return this.m_AllowGameplayInput;
	}

	// Token: 0x06002EAE RID: 11950 RVA: 0x000B46D3 File Offset: 0x000B28D3
	protected virtual void Set_InputAllowGameplay(bool value)
	{
		this.m_AllowGameplayInput = value;
	}

	// Token: 0x1700030B RID: 779
	// (get) Token: 0x06002EAF RID: 11951 RVA: 0x000B46CB File Offset: 0x000B28CB
	// (set) Token: 0x06002EB0 RID: 11952 RVA: 0x000B46D3 File Offset: 0x000B28D3
	protected virtual bool OnValue_InputAllowGameplay
	{
		get
		{
			return this.m_AllowGameplayInput;
		}
		set
		{
			this.m_AllowGameplayInput = value;
		}
	}

	// Token: 0x06002EB1 RID: 11953 RVA: 0x000B4DD1 File Offset: 0x000B2FD1
	protected virtual bool Get_Pause()
	{
		return vp_TimeUtility.Paused;
	}

	// Token: 0x06002EB2 RID: 11954 RVA: 0x000B4DD8 File Offset: 0x000B2FD8
	protected virtual void Set_Pause(bool value)
	{
		vp_TimeUtility.Paused = (!vp_Gameplay.isMultiplayer && value);
	}

	// Token: 0x1700030C RID: 780
	// (get) Token: 0x06002EB3 RID: 11955 RVA: 0x000B4DD1 File Offset: 0x000B2FD1
	// (set) Token: 0x06002EB4 RID: 11956 RVA: 0x000B4DD8 File Offset: 0x000B2FD8
	protected virtual bool OnValue_Pause
	{
		get
		{
			return vp_TimeUtility.Paused;
		}
		set
		{
			vp_TimeUtility.Paused = (!vp_Gameplay.isMultiplayer && value);
		}
	}

	// Token: 0x06002EB5 RID: 11957 RVA: 0x000350A2 File Offset: 0x000332A2
	protected virtual bool OnMessage_InputGetButton(string button)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06002EB6 RID: 11958 RVA: 0x000350A2 File Offset: 0x000332A2
	protected virtual bool OnMessage_InputGetButtonUp(string button)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06002EB7 RID: 11959 RVA: 0x000350A2 File Offset: 0x000332A2
	protected virtual bool OnMessage_InputGetButtonDown(string button)
	{
		throw new NotImplementedException();
	}

	// Token: 0x1700030D RID: 781
	// (get) Token: 0x06002EB8 RID: 11960 RVA: 0x000B4DEA File Offset: 0x000B2FEA
	protected virtual Vector2 OnValue_InputSmoothLook
	{
		get
		{
			return this.GetMouseLook();
		}
	}

	// Token: 0x1700030E RID: 782
	// (get) Token: 0x06002EB9 RID: 11961 RVA: 0x000B4DF2 File Offset: 0x000B2FF2
	protected virtual Vector2 OnValue_InputRawLook
	{
		get
		{
			return this.GetMouseLookRaw();
		}
	}

	// Token: 0x06002EBA RID: 11962 RVA: 0x000B4DFC File Offset: 0x000B2FFC
	public override void Register(vp_EventHandler eventHandler)
	{
		base.Register(eventHandler);
		eventHandler.RegisterMessage<string, bool>("InputGetButton", new vp_Message<string, bool>.Sender<string, bool>(this.OnMessage_InputGetButton));
		eventHandler.RegisterMessage<string, bool>("InputGetButtonDown", new vp_Message<string, bool>.Sender<string, bool>(this.OnMessage_InputGetButtonDown));
		eventHandler.RegisterMessage<string, bool>("InputGetButtonUp", new vp_Message<string, bool>.Sender<string, bool>(this.OnMessage_InputGetButtonUp));
		eventHandler.RegisterValue<Vector2>("InputSmoothLook", new vp_Value<Vector2>.Getter<Vector2>(this.GetMouseLook), null);
		eventHandler.RegisterValue<Vector2>("InputRawLook", new vp_Value<Vector2>.Getter<Vector2>(this.GetMouseLookRaw), null);
		eventHandler.RegisterValue<bool>("InputAllowGameplay", new vp_Value<bool>.Getter<bool>(this.Get_InputAllowGameplay), new vp_Value<bool>.Setter<bool>(this.Set_InputAllowGameplay));
		eventHandler.RegisterValue<float>("InputClimbVector", new vp_Value<float>.Getter<float>(this.Get_InputClimbVector), null);
		eventHandler.RegisterValue<Vector2>("InputMoveVector", new vp_Value<Vector2>.Getter<Vector2>(this.Get_InputMoveVector), new vp_Value<Vector2>.Setter<Vector2>(this.Set_InputMoveVector));
		eventHandler.RegisterValue<bool>("Pause", new vp_Value<bool>.Getter<bool>(this.Get_Pause), new vp_Value<bool>.Setter<bool>(this.Set_Pause));
	}

	// Token: 0x06002EBB RID: 11963 RVA: 0x000B4F14 File Offset: 0x000B3114
	public override void Unregister(vp_EventHandler eventHandler)
	{
		base.Unregister(eventHandler);
		eventHandler.UnregisterMessage<string, bool>("InputGetButton", new vp_Message<string, bool>.Sender<string, bool>(this.OnMessage_InputGetButton));
		eventHandler.UnregisterMessage<string, bool>("InputGetButtonDown", new vp_Message<string, bool>.Sender<string, bool>(this.OnMessage_InputGetButtonDown));
		eventHandler.UnregisterMessage<string, bool>("InputGetButtonUp", new vp_Message<string, bool>.Sender<string, bool>(this.OnMessage_InputGetButtonUp));
		eventHandler.UnregisterValue<Vector2>("InputSmoothLook", new vp_Value<Vector2>.Getter<Vector2>(this.GetMouseLook), null);
		eventHandler.UnregisterValue<Vector2>("InputRawLook", new vp_Value<Vector2>.Getter<Vector2>(this.GetMouseLookRaw), null);
		eventHandler.UnregisterValue<bool>("InputAllowGameplay", new vp_Value<bool>.Getter<bool>(this.Get_InputAllowGameplay), new vp_Value<bool>.Setter<bool>(this.Set_InputAllowGameplay));
		eventHandler.UnregisterValue<float>("InputClimbVector", new vp_Value<float>.Getter<float>(this.Get_InputClimbVector), null);
		eventHandler.UnregisterValue<Vector2>("InputMoveVector", new vp_Value<Vector2>.Getter<Vector2>(this.Get_InputMoveVector), new vp_Value<Vector2>.Setter<Vector2>(this.Set_InputMoveVector));
		eventHandler.UnregisterValue<bool>("Pause", new vp_Value<bool>.Getter<bool>(this.Get_Pause), new vp_Value<bool>.Setter<bool>(this.Set_Pause));
	}

	// Token: 0x04002C80 RID: 11392
	public Vector2 MouseLookSensitivity = new Vector2(5f, 5f);

	// Token: 0x04002C81 RID: 11393
	public int MouseLookSmoothSteps = 10;

	// Token: 0x04002C82 RID: 11394
	public float MouseLookSmoothWeight = 0.5f;

	// Token: 0x04002C83 RID: 11395
	public bool MouseLookAcceleration;

	// Token: 0x04002C84 RID: 11396
	public float MouseLookAccelerationThreshold = 0.4f;

	// Token: 0x04002C85 RID: 11397
	public bool MouseLookInvert;

	// Token: 0x04002C86 RID: 11398
	protected Vector2 m_MouseLookSmoothMove = Vector2.zero;

	// Token: 0x04002C87 RID: 11399
	protected Vector2 m_MouseLookRawMove = Vector2.zero;

	// Token: 0x04002C88 RID: 11400
	protected List<Vector2> m_MouseLookSmoothBuffer = new List<Vector2>();

	// Token: 0x04002C89 RID: 11401
	protected int m_LastMouseLookFrame = -1;

	// Token: 0x04002C8A RID: 11402
	protected Vector2 m_CurrentMouseLook = Vector2.zero;

	// Token: 0x04002C8B RID: 11403
	public Rect[] MouseCursorZones;

	// Token: 0x04002C8C RID: 11404
	public bool MouseCursorForced;

	// Token: 0x04002C8D RID: 11405
	public bool MouseCursorBlocksMouseLook = true;

	// Token: 0x04002C8E RID: 11406
	protected Vector2 m_MousePos = Vector2.zero;

	// Token: 0x04002C8F RID: 11407
	protected Vector2 m_MoveVector = Vector2.zero;

	// Token: 0x04002C90 RID: 11408
	protected bool m_AllowGameplayInput = true;

	// Token: 0x04002C91 RID: 11409
	protected vp_FPPlayerEventHandler m_FPPlayer;

	// Token: 0x04002C92 RID: 11410
	private OptionsDirector optionsDir;

	// Token: 0x04002C93 RID: 11411
	private InputDirector inputDir;
}
