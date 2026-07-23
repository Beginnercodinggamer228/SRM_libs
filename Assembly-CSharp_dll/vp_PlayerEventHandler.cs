using System;
using UnityEngine;

// Token: 0x0200088E RID: 2190
public class vp_PlayerEventHandler : vp_StateEventHandler
{
	// Token: 0x06002FB2 RID: 12210 RVA: 0x000BB4B4 File Offset: 0x000B96B4
	public vp_PlayerEventHandler()
	{
		this.AddHandledEvents();
	}

	// Token: 0x06002FB3 RID: 12211 RVA: 0x00013CC5 File Offset: 0x00011EC5
	private bool GetTrue()
	{
		return true;
	}

	// Token: 0x06002FB4 RID: 12212 RVA: 0x000BB8FC File Offset: 0x000B9AFC
	protected override void Awake()
	{
		base.Awake();
		base.BindStateToActivity(this.Run);
		base.BindStateToActivity(this.Jump);
		base.BindStateToActivity(this.Crouch);
		base.BindStateToActivity(this.Zoom);
		base.BindStateToActivity(this.Reload);
		base.BindStateToActivity(this.Dead);
		base.BindStateToActivity(this.Climb);
		base.BindStateToActivity(this.OutOfControl);
		base.BindStateToActivityOnStart(this.Attack);
		base.BindStateToActivity(this.Underwater);
		this.SetWeapon.AutoDuration = 1f;
		this.Reload.AutoDuration = 1f;
		this.Zoom.MinDuration = 0.2f;
		this.Crouch.MinDuration = 0.5f;
		this.Jump.MinPause = 0f;
		this.SetWeapon.MinPause = 0.2f;
	}

	// Token: 0x06002FB5 RID: 12213 RVA: 0x000BB9E8 File Offset: 0x000B9BE8
	private void AddHandledEvents()
	{
		this.m_HandlerEvents.Add("IsFirstPerson", this.IsFirstPerson);
		this.m_HandlerEvents.Add("IsLocal", this.IsLocal);
		this.m_HandlerEvents.Add("IsAI", this.IsAI);
		this.m_HandlerEvents.Add("Health", this.Health);
		this.m_HandlerEvents.Add("MaxHealth", this.MaxHealth);
		this.m_HandlerEvents.Add("Position", this.Position);
		this.m_HandlerEvents.Add("Rotation", this.Rotation);
		this.m_HandlerEvents.Add("BodyYaw", this.BodyYaw);
		this.m_HandlerEvents.Add("LookPoint", this.LookPoint);
		this.m_HandlerEvents.Add("HeadLookDirection", this.HeadLookDirection);
		this.m_HandlerEvents.Add("AimDirection", this.AimDirection);
		this.m_HandlerEvents.Add("MotorThrottle", this.MotorThrottle);
		this.m_HandlerEvents.Add("MotorJumpDone", this.MotorJumpDone);
		this.m_HandlerEvents.Add("InputMoveVector", this.InputMoveVector);
		this.m_HandlerEvents.Add("InputClimbVector", this.InputClimbVector);
		this.m_HandlerEvents.Add("Dead", this.Dead);
		this.m_HandlerEvents.Add("Run", this.Run);
		this.m_HandlerEvents.Add("Jump", this.Jump);
		this.m_HandlerEvents.Add("Jetpack", this.Jetpack);
		this.m_HandlerEvents.Add("Crouch", this.Crouch);
		this.m_HandlerEvents.Add("Zoom", this.Zoom);
		this.m_HandlerEvents.Add("Attack", this.Attack);
		this.m_HandlerEvents.Add("Reload", this.Reload);
		this.m_HandlerEvents.Add("Climb", this.Climb);
		this.m_HandlerEvents.Add("Interact", this.Interact);
		this.m_HandlerEvents.Add("SetWeapon", this.SetWeapon);
		this.m_HandlerEvents.Add("OutOfControl", this.OutOfControl);
		this.m_HandlerEvents.Add("Underwater", this.Underwater);
		this.m_HandlerEvents.Add("Wield", this.Wield);
		this.m_HandlerEvents.Add("Unwield", this.Unwield);
		this.m_HandlerEvents.Add("Fire", this.Fire);
		this.m_HandlerEvents.Add("DryFire", this.DryFire);
		this.m_HandlerEvents.Add("SetPrevWeapon", this.SetPrevWeapon);
		this.m_HandlerEvents.Add("SetNextWeapon", this.SetNextWeapon);
		this.m_HandlerEvents.Add("SetWeaponByName", this.SetWeaponByName);
		this.m_HandlerEvents.Add("CurrentWeaponID", this.CurrentWeaponID);
		this.m_HandlerEvents.Add("CurrentWeaponIndex", this.CurrentWeaponIndex);
		this.m_HandlerEvents.Add("CurrentWeaponName", this.CurrentWeaponName);
		this.m_HandlerEvents.Add("CurrentWeaponWielded", this.CurrentWeaponWielded);
		this.m_HandlerEvents.Add("AutoReload", this.AutoReload);
		this.m_HandlerEvents.Add("CurrentWeaponReloadDuration", this.CurrentWeaponReloadDuration);
		this.m_HandlerEvents.Add("GetItemCount", this.GetItemCount);
		this.m_HandlerEvents.Add("RefillCurrentWeapon", this.RefillCurrentWeapon);
		this.m_HandlerEvents.Add("CurrentWeaponAmmoCount", this.CurrentWeaponAmmoCount);
		this.m_HandlerEvents.Add("CurrentWeaponMaxAmmoCount", this.CurrentWeaponMaxAmmoCount);
		this.m_HandlerEvents.Add("CurrentWeaponClipCount", this.CurrentWeaponClipCount);
		this.m_HandlerEvents.Add("CurrentWeaponType", this.CurrentWeaponType);
		this.m_HandlerEvents.Add("CurrentWeaponGrip", this.CurrentWeaponGrip);
		this.m_HandlerEvents.Add("AddItem", this.AddItem);
		this.m_HandlerEvents.Add("RemoveItem", this.RemoveItem);
		this.m_HandlerEvents.Add("DepleteAmmo", this.DepleteAmmo);
		this.m_HandlerEvents.Add("Move", this.Move);
		this.m_HandlerEvents.Add("Velocity", this.Velocity);
		this.m_HandlerEvents.Add("SlopeLimit", this.SlopeLimit);
		this.m_HandlerEvents.Add("StepOffset", this.StepOffset);
		this.m_HandlerEvents.Add("Radius", this.Radius);
		this.m_HandlerEvents.Add("Height", this.Height);
		this.m_HandlerEvents.Add("FallSpeed", this.FallSpeed);
		this.m_HandlerEvents.Add("FallImpact", this.FallImpact);
		this.m_HandlerEvents.Add("HeadImpact", this.HeadImpact);
		this.m_HandlerEvents.Add("Stop", this.Stop);
		this.m_HandlerEvents.Add("Platform", this.Platform);
		this.m_HandlerEvents.Add("Interactable", this.Interactable);
		this.m_HandlerEvents.Add("CanInteract", this.CanInteract);
		this.m_HandlerEvents.Add("GroundTexture", this.GroundTexture);
		this.m_HandlerEvents.Add("SurfaceType", this.SurfaceType);
		this.IsFirstPerson.Get = new vp_Value<bool>.Getter<bool>(this.GetTrue);
	}

	// Token: 0x04002DA0 RID: 11680
	public vp_Value<bool> IsFirstPerson = new vp_Value<bool>("IsFirstPerson");

	// Token: 0x04002DA1 RID: 11681
	public vp_Value<bool> IsLocal = new vp_Value<bool>("IsLocal");

	// Token: 0x04002DA2 RID: 11682
	public vp_Value<bool> IsAI = new vp_Value<bool>("IsAI");

	// Token: 0x04002DA3 RID: 11683
	public vp_Value<float> Health = new vp_Value<float>("Health");

	// Token: 0x04002DA4 RID: 11684
	public vp_Value<float> MaxHealth = new vp_Value<float>("MaxHealth");

	// Token: 0x04002DA5 RID: 11685
	public vp_Value<Vector3> Position = new vp_Value<Vector3>("Position");

	// Token: 0x04002DA6 RID: 11686
	public vp_Value<Vector2> Rotation = new vp_Value<Vector2>("Rotation");

	// Token: 0x04002DA7 RID: 11687
	public vp_Value<float> BodyYaw = new vp_Value<float>("BodyYaw");

	// Token: 0x04002DA8 RID: 11688
	public vp_Value<Vector3> LookPoint = new vp_Value<Vector3>("LookPoint");

	// Token: 0x04002DA9 RID: 11689
	public vp_Value<Vector3> HeadLookDirection = new vp_Value<Vector3>("HeadLookDirection");

	// Token: 0x04002DAA RID: 11690
	public vp_Value<Vector3> AimDirection = new vp_Value<Vector3>("AimDirection");

	// Token: 0x04002DAB RID: 11691
	public vp_Value<Vector3> MotorThrottle = new vp_Value<Vector3>("MotorThrottle");

	// Token: 0x04002DAC RID: 11692
	public vp_Value<bool> MotorJumpDone = new vp_Value<bool>("MotorJumpDone");

	// Token: 0x04002DAD RID: 11693
	public vp_Value<Vector2> InputMoveVector = new vp_Value<Vector2>("InputMoveVector");

	// Token: 0x04002DAE RID: 11694
	public vp_Value<float> InputClimbVector = new vp_Value<float>("InputClimbVector");

	// Token: 0x04002DAF RID: 11695
	public vp_Activity Dead = new vp_Activity("Dead");

	// Token: 0x04002DB0 RID: 11696
	public vp_Activity Run = new vp_Activity("Run");

	// Token: 0x04002DB1 RID: 11697
	public vp_Activity Jump = new vp_Activity("Jump");

	// Token: 0x04002DB2 RID: 11698
	public vp_Activity Jetpack = new vp_Activity("Jetpack");

	// Token: 0x04002DB3 RID: 11699
	public vp_Activity Crouch = new vp_Activity("Crouch");

	// Token: 0x04002DB4 RID: 11700
	public vp_Activity Zoom = new vp_Activity("Zoom");

	// Token: 0x04002DB5 RID: 11701
	public vp_Activity Attack = new vp_Activity("Attack");

	// Token: 0x04002DB6 RID: 11702
	public vp_Activity Reload = new vp_Activity("Reload");

	// Token: 0x04002DB7 RID: 11703
	public vp_Activity Climb = new vp_Activity("Climb");

	// Token: 0x04002DB8 RID: 11704
	public vp_Activity Interact = new vp_Activity("Interact");

	// Token: 0x04002DB9 RID: 11705
	public vp_Activity<int> SetWeapon = new vp_Activity<int>("SetWeapon");

	// Token: 0x04002DBA RID: 11706
	public vp_Activity OutOfControl = new vp_Activity("OutOfControl");

	// Token: 0x04002DBB RID: 11707
	public vp_Activity Underwater = new vp_Activity("Underwater");

	// Token: 0x04002DBC RID: 11708
	public vp_Message<int> Wield = new vp_Message<int>("Wield");

	// Token: 0x04002DBD RID: 11709
	public vp_Message Unwield = new vp_Message("Unwield", null, null);

	// Token: 0x04002DBE RID: 11710
	public vp_Attempt Fire = new vp_Attempt("Fire", null);

	// Token: 0x04002DBF RID: 11711
	public vp_Message DryFire = new vp_Message("DryFire", null, null);

	// Token: 0x04002DC0 RID: 11712
	public vp_Attempt SetPrevWeapon = new vp_Attempt("SetPrevWeapon", null);

	// Token: 0x04002DC1 RID: 11713
	public vp_Attempt SetNextWeapon = new vp_Attempt("SetNextWeapon", null);

	// Token: 0x04002DC2 RID: 11714
	public vp_Attempt<string> SetWeaponByName = new vp_Attempt<string>("SetWeaponByName");

	// Token: 0x04002DC3 RID: 11715
	[Obsolete("Please use the 'CurrentWeaponIndex' vp_Value instead.")]
	public vp_Value<int> CurrentWeaponID = new vp_Value<int>("CurrentWeaponID");

	// Token: 0x04002DC4 RID: 11716
	public vp_Value<int> CurrentWeaponIndex = new vp_Value<int>("CurrentWeaponIndex");

	// Token: 0x04002DC5 RID: 11717
	public vp_Value<string> CurrentWeaponName = new vp_Value<string>("CurrentWeaponName");

	// Token: 0x04002DC6 RID: 11718
	public vp_Value<bool> CurrentWeaponWielded = new vp_Value<bool>("CurrentWeaponWielded");

	// Token: 0x04002DC7 RID: 11719
	public vp_Attempt AutoReload = new vp_Attempt("AutoReload", null);

	// Token: 0x04002DC8 RID: 11720
	public vp_Value<float> CurrentWeaponReloadDuration = new vp_Value<float>("CurrentWeaponReloadDuration");

	// Token: 0x04002DC9 RID: 11721
	public vp_Message<string, int> GetItemCount = new vp_Message<string, int>("GetItemCount");

	// Token: 0x04002DCA RID: 11722
	public vp_Attempt RefillCurrentWeapon = new vp_Attempt("RefillCurrentWeapon", null);

	// Token: 0x04002DCB RID: 11723
	public vp_Value<int> CurrentWeaponAmmoCount = new vp_Value<int>("CurrentWeaponAmmoCount");

	// Token: 0x04002DCC RID: 11724
	public vp_Value<int> CurrentWeaponMaxAmmoCount = new vp_Value<int>("CurrentWeaponMaxAmmoCount");

	// Token: 0x04002DCD RID: 11725
	public vp_Value<int> CurrentWeaponClipCount = new vp_Value<int>("CurrentWeaponClipCount");

	// Token: 0x04002DCE RID: 11726
	public vp_Value<int> CurrentWeaponType = new vp_Value<int>("CurrentWeaponType");

	// Token: 0x04002DCF RID: 11727
	public vp_Value<int> CurrentWeaponGrip = new vp_Value<int>("CurrentWeaponGrip");

	// Token: 0x04002DD0 RID: 11728
	public vp_Attempt<object> AddItem = new vp_Attempt<object>("AddItem");

	// Token: 0x04002DD1 RID: 11729
	public vp_Attempt<object> RemoveItem = new vp_Attempt<object>("RemoveItem");

	// Token: 0x04002DD2 RID: 11730
	public vp_Attempt DepleteAmmo = new vp_Attempt("DepleteAmmo", null);

	// Token: 0x04002DD3 RID: 11731
	public vp_Message<Vector3> Move = new vp_Message<Vector3>("Move");

	// Token: 0x04002DD4 RID: 11732
	public vp_Value<Vector3> Velocity = new vp_Value<Vector3>("Velocity");

	// Token: 0x04002DD5 RID: 11733
	public vp_Value<float> SlopeLimit = new vp_Value<float>("SlopeLimit");

	// Token: 0x04002DD6 RID: 11734
	public vp_Value<float> StepOffset = new vp_Value<float>("StepOffset");

	// Token: 0x04002DD7 RID: 11735
	public vp_Value<float> Radius = new vp_Value<float>("Radius");

	// Token: 0x04002DD8 RID: 11736
	public vp_Value<float> Height = new vp_Value<float>("Height");

	// Token: 0x04002DD9 RID: 11737
	public vp_Value<float> FallSpeed = new vp_Value<float>("FallSpeed");

	// Token: 0x04002DDA RID: 11738
	public vp_Message<float> FallImpact = new vp_Message<float>("FallImpact");

	// Token: 0x04002DDB RID: 11739
	public vp_Message<float> HeadImpact = new vp_Message<float>("HeadImpact");

	// Token: 0x04002DDC RID: 11740
	public vp_Message Stop = new vp_Message("Stop", null, null);

	// Token: 0x04002DDD RID: 11741
	public vp_Value<Transform> Platform = new vp_Value<Transform>("Platform");

	// Token: 0x04002DDE RID: 11742
	public vp_Value<vp_Interactable> Interactable = new vp_Value<vp_Interactable>("Interactable");

	// Token: 0x04002DDF RID: 11743
	public vp_Value<bool> CanInteract = new vp_Value<bool>("CanInteract");

	// Token: 0x04002DE0 RID: 11744
	public vp_Value<Texture> GroundTexture = new vp_Value<Texture>("GroundTexture");

	// Token: 0x04002DE1 RID: 11745
	public vp_Value<vp_SurfaceIdentifier> SurfaceType = new vp_Value<vp_SurfaceIdentifier>("SurfaceType");
}
