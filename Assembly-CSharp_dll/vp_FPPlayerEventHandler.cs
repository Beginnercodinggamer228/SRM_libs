using System;
using UnityEngine;

// Token: 0x02000884 RID: 2180
public class vp_FPPlayerEventHandler : vp_PlayerEventHandler
{
	// Token: 0x06002EDD RID: 11997 RVA: 0x000B597A File Offset: 0x000B3B7A
	protected override void Awake()
	{
		base.Awake();
	}

	// Token: 0x06002EDE RID: 11998 RVA: 0x000B5984 File Offset: 0x000B3B84
	public vp_FPPlayerEventHandler()
	{
		this.AddEvents();
	}

	// Token: 0x06002EDF RID: 11999 RVA: 0x000B5AD0 File Offset: 0x000B3CD0
	private void AddEvents()
	{
		this.m_HandlerEvents.Add("HUDText", this.HUDText);
		this.m_HandlerEvents.Add("Crosshair", this.Crosshair);
		this.m_HandlerEvents.Add("CurrentAmmoIcon", this.CurrentAmmoIcon);
		this.m_HandlerEvents.Add("InputSmoothLook", this.InputSmoothLook);
		this.m_HandlerEvents.Add("InputRawLook", this.InputRawLook);
		this.m_HandlerEvents.Add("InputGetButton", this.InputGetButton);
		this.m_HandlerEvents.Add("InputGetButtonUp", this.InputGetButtonUp);
		this.m_HandlerEvents.Add("InputGetButtonDown", this.InputGetButtonDown);
		this.m_HandlerEvents.Add("InputAllowGameplay", this.InputAllowGameplay);
		this.m_HandlerEvents.Add("Pause", this.Pause);
		this.m_HandlerEvents.Add("CameraLookDirection", this.CameraLookDirection);
		this.m_HandlerEvents.Add("CameraToggle3rdPerson", this.CameraToggle3rdPerson);
		this.m_HandlerEvents.Add("CameraGroundStomp", this.CameraGroundStomp);
		this.m_HandlerEvents.Add("CameraBombShake", this.CameraBombShake);
		this.m_HandlerEvents.Add("CameraEarthQuakeForce", this.CameraEarthQuakeForce);
		this.m_HandlerEvents.Add("CameraEarthQuake", this.CameraEarthQuake);
		this.m_HandlerEvents.Add("CurrentWeaponClipType", this.CurrentWeaponClipType);
		this.m_HandlerEvents.Add("AddAmmo", this.AddAmmo);
		this.m_HandlerEvents.Add("RemoveClip", this.RemoveClip);
	}

	// Token: 0x04002CA3 RID: 11427
	public vp_Message<string> HUDText = new vp_Message<string>("HUDText");

	// Token: 0x04002CA4 RID: 11428
	public vp_Value<Texture> Crosshair = new vp_Value<Texture>("Crosshair");

	// Token: 0x04002CA5 RID: 11429
	public vp_Value<Texture2D> CurrentAmmoIcon = new vp_Value<Texture2D>("CurrentAmmoIcon");

	// Token: 0x04002CA6 RID: 11430
	public vp_Value<Vector2> InputSmoothLook = new vp_Value<Vector2>("InputSmootLook");

	// Token: 0x04002CA7 RID: 11431
	public vp_Value<Vector2> InputRawLook = new vp_Value<Vector2>("InputRawLook");

	// Token: 0x04002CA8 RID: 11432
	public vp_Message<string, bool> InputGetButton = new vp_Message<string, bool>("InputGetButton");

	// Token: 0x04002CA9 RID: 11433
	public vp_Message<string, bool> InputGetButtonUp = new vp_Message<string, bool>("InputGetButtonUp");

	// Token: 0x04002CAA RID: 11434
	public vp_Message<string, bool> InputGetButtonDown = new vp_Message<string, bool>("InputGetButtonDown");

	// Token: 0x04002CAB RID: 11435
	public vp_Value<bool> InputAllowGameplay = new vp_Value<bool>("InputAllowGameplay");

	// Token: 0x04002CAC RID: 11436
	public vp_Value<bool> Pause = new vp_Value<bool>("Pause");

	// Token: 0x04002CAD RID: 11437
	public vp_Value<Vector3> CameraLookDirection = new vp_Value<Vector3>("CameraLookDirection");

	// Token: 0x04002CAE RID: 11438
	public vp_Message CameraToggle3rdPerson = new vp_Message("CameraToggle3rdPerson", null, null);

	// Token: 0x04002CAF RID: 11439
	public vp_Message<float> CameraGroundStomp = new vp_Message<float>("CameraGroundStomp");

	// Token: 0x04002CB0 RID: 11440
	public vp_Message<float> CameraBombShake = new vp_Message<float>("CameraBombShake");

	// Token: 0x04002CB1 RID: 11441
	public vp_Value<Vector3> CameraEarthQuakeForce = new vp_Value<Vector3>("CameraEarthQuakeForce");

	// Token: 0x04002CB2 RID: 11442
	public vp_Activity<Vector3> CameraEarthQuake = new vp_Activity<Vector3>("CameraEarthQuake");

	// Token: 0x04002CB3 RID: 11443
	public vp_Value<string> CurrentWeaponClipType = new vp_Value<string>("CurrentWeaponClipType");

	// Token: 0x04002CB4 RID: 11444
	public vp_Attempt<object> AddAmmo = new vp_Attempt<object>("AddAmmo");

	// Token: 0x04002CB5 RID: 11445
	public vp_Attempt RemoveClip = new vp_Attempt("RemoveClip", null);
}
