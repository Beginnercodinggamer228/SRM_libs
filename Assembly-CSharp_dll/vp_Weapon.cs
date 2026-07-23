using System;
using UnityEngine;

// Token: 0x02000892 RID: 2194
public class vp_Weapon : vp_Component
{
	// Token: 0x17000348 RID: 840
	// (get) Token: 0x06002FDE RID: 12254 RVA: 0x000BC856 File Offset: 0x000BAA56
	// (set) Token: 0x06002FDF RID: 12255 RVA: 0x000BC868 File Offset: 0x000BAA68
	public bool Wielded
	{
		get
		{
			return this.m_Wielded && base.Rendering;
		}
		set
		{
			this.m_Wielded = value;
		}
	}

	// Token: 0x17000349 RID: 841
	// (get) Token: 0x06002FE0 RID: 12256 RVA: 0x000BC871 File Offset: 0x000BAA71
	protected vp_PlayerEventHandler Player
	{
		get
		{
			if (this.m_Player == null && base.EventHandler != null)
			{
				this.m_Player = (vp_PlayerEventHandler)base.EventHandler;
			}
			return this.m_Player;
		}
	}

	// Token: 0x1700034A RID: 842
	// (get) Token: 0x06002FE1 RID: 12257 RVA: 0x000BC8A6 File Offset: 0x000BAAA6
	public Renderer Weapon3rdPersonModelRenderer
	{
		get
		{
			if (this.m_Weapon3rdPersonModelRenderer == null && this.Weapon3rdPersonModel != null)
			{
				this.m_Weapon3rdPersonModelRenderer = this.Weapon3rdPersonModel.GetComponent<Renderer>();
			}
			return this.m_Weapon3rdPersonModelRenderer;
		}
	}

	// Token: 0x06002FE2 RID: 12258 RVA: 0x000BC8DC File Offset: 0x000BAADC
	protected override void Awake()
	{
		base.Awake();
		this.RotationOffset = base.transform.localEulerAngles;
		this.PositionOffset = base.transform.position;
		base.Transform.localEulerAngles = this.RotationOffset;
		if (base.transform.parent == null)
		{
			Debug.LogError("Error (" + this + ") Must not be placed in scene root. Disabling self.");
			vp_Utility.Activate(base.gameObject, false);
			return;
		}
		if (base.GetComponent<Collider>() != null)
		{
			base.GetComponent<Collider>().enabled = false;
		}
	}

	// Token: 0x06002FE3 RID: 12259 RVA: 0x000BC971 File Offset: 0x000BAB71
	protected override void OnEnable()
	{
		base.OnEnable();
		this.RefreshWeaponModel();
	}

	// Token: 0x06002FE4 RID: 12260 RVA: 0x000BC97F File Offset: 0x000BAB7F
	protected override void OnDisable()
	{
		this.RefreshWeaponModel();
		this.Activate3rdPersonModel(false);
		base.OnDisable();
	}

	// Token: 0x1700034B RID: 843
	// (get) Token: 0x06002FE5 RID: 12261 RVA: 0x000BC994 File Offset: 0x000BAB94
	// (set) Token: 0x06002FE6 RID: 12262 RVA: 0x000BC99C File Offset: 0x000BAB9C
	public Vector3 RotationSpring2DefaultRotation
	{
		get
		{
			return this.m_RotationSpring2DefaultRotation;
		}
		set
		{
			this.m_RotationSpring2DefaultRotation = value;
		}
	}

	// Token: 0x06002FE7 RID: 12263 RVA: 0x000BC9A8 File Offset: 0x000BABA8
	protected override void Start()
	{
		base.Start();
		this.m_PositionSpring2 = new vp_Spring(base.transform, vp_Spring.UpdateMode.PositionAdditiveSelf, true);
		this.m_PositionSpring2.MinVelocity = 1E-05f;
		this.m_RotationSpring2 = new vp_Spring(base.transform, vp_Spring.UpdateMode.RotationAdditiveGlobal, true);
		this.m_RotationSpring2.MinVelocity = 1E-05f;
		this.SnapSprings();
		this.Refresh();
		base.CacheRenderers();
	}

	// Token: 0x1700034C RID: 844
	// (get) Token: 0x06002FE8 RID: 12264 RVA: 0x000BCA13 File Offset: 0x000BAC13
	public Vector3 Recoil
	{
		get
		{
			return this.m_RotationSpring2.State;
		}
	}

	// Token: 0x06002FE9 RID: 12265 RVA: 0x000BCA20 File Offset: 0x000BAC20
	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (Time.timeScale == 0f)
		{
			return;
		}
		this.UpdateSprings();
	}

	// Token: 0x06002FEA RID: 12266 RVA: 0x000BCA3B File Offset: 0x000BAC3B
	public virtual void AddForce2(Vector3 positional, Vector3 angular)
	{
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.AddForce(positional);
		}
		if (this.m_RotationSpring2 != null)
		{
			this.m_RotationSpring2.AddForce(angular);
		}
	}

	// Token: 0x06002FEB RID: 12267 RVA: 0x000BCA65 File Offset: 0x000BAC65
	public virtual void AddForce2(float xPos, float yPos, float zPos, float xRot, float yRot, float zRot)
	{
		this.AddForce2(new Vector3(xPos, yPos, zPos), new Vector3(xRot, yRot, zRot));
	}

	// Token: 0x06002FEC RID: 12268 RVA: 0x000BCA80 File Offset: 0x000BAC80
	protected virtual void UpdateSprings()
	{
		base.Transform.localPosition = Vector3.up;
		base.Transform.localRotation = Quaternion.identity;
		this.m_PositionSpring2.FixedUpdate();
		this.m_RotationSpring2.FixedUpdate();
	}

	// Token: 0x06002FED RID: 12269 RVA: 0x000BCAB8 File Offset: 0x000BACB8
	public override void Refresh()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.Stiffness = new Vector3(this.PositionSpring2Stiffness, this.PositionSpring2Stiffness, this.PositionSpring2Stiffness);
			this.m_PositionSpring2.Damping = Vector3.one - new Vector3(this.PositionSpring2Damping, this.PositionSpring2Damping, this.PositionSpring2Damping);
			this.m_PositionSpring2.RestState = Vector3.zero;
		}
		if (this.m_RotationSpring2 != null)
		{
			this.m_RotationSpring2.Stiffness = new Vector3(this.RotationSpring2Stiffness, this.RotationSpring2Stiffness, this.RotationSpring2Stiffness);
			this.m_RotationSpring2.Damping = Vector3.one - new Vector3(this.RotationSpring2Damping, this.RotationSpring2Damping, this.RotationSpring2Damping);
			this.m_RotationSpring2.RestState = this.m_RotationSpring2DefaultRotation;
		}
	}

	// Token: 0x06002FEE RID: 12270 RVA: 0x000BCB9A File Offset: 0x000BAD9A
	public override void Activate()
	{
		base.Activate();
		this.m_Wielded = true;
		base.Rendering = true;
	}

	// Token: 0x06002FEF RID: 12271 RVA: 0x000BCBB0 File Offset: 0x000BADB0
	public virtual void SnapSprings()
	{
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.RestState = Vector3.zero;
			this.m_PositionSpring2.State = Vector3.zero;
			this.m_PositionSpring2.Stop(true);
		}
		if (this.m_RotationSpring2 != null)
		{
			this.m_RotationSpring2.RestState = this.m_RotationSpring2DefaultRotation;
			this.m_RotationSpring2.State = this.m_RotationSpring2DefaultRotation;
			this.m_RotationSpring2.Stop(true);
		}
	}

	// Token: 0x06002FF0 RID: 12272 RVA: 0x000BCC27 File Offset: 0x000BAE27
	public virtual void StopSprings()
	{
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.Stop(true);
		}
		if (this.m_RotationSpring2 != null)
		{
			this.m_RotationSpring2.Stop(true);
		}
	}

	// Token: 0x06002FF1 RID: 12273 RVA: 0x000BCC51 File Offset: 0x000BAE51
	public virtual void Wield(bool isWielding = true)
	{
		this.m_Wielded = isWielding;
		this.Refresh();
		base.StateManager.CombineStates();
	}

	// Token: 0x06002FF2 RID: 12274 RVA: 0x000BCC6C File Offset: 0x000BAE6C
	public virtual void RefreshWeaponModel()
	{
		if (this.Player == null)
		{
			return;
		}
		if (this.Player.IsFirstPerson == null)
		{
			return;
		}
		base.Transform.localScale = (this.Player.IsFirstPerson.Get() ? Vector3.one : Vector3.zero);
		this.Activate3rdPersonModel(!this.Player.IsFirstPerson.Get());
		if (this.Player != null && this.Player.CurrentWeaponName != null && this.Player.CurrentWeaponName.Get != null && this.Player.CurrentWeaponName.Get() != base.name)
		{
			this.Activate3rdPersonModel(false);
		}
	}

	// Token: 0x06002FF3 RID: 12275 RVA: 0x000BCD38 File Offset: 0x000BAF38
	protected virtual void Activate3rdPersonModel(bool active = true)
	{
		if (this.Weapon3rdPersonModel == null)
		{
			return;
		}
		if (active)
		{
			if (this.Weapon3rdPersonModelRenderer != null)
			{
				this.Weapon3rdPersonModelRenderer.enabled = true;
			}
			vp_Utility.Activate(this.Weapon3rdPersonModel, true);
			return;
		}
		if (this.Weapon3rdPersonModelRenderer != null)
		{
			this.Weapon3rdPersonModelRenderer.enabled = false;
		}
		vp_Timer.In(0.1f, delegate()
		{
			if (this.Weapon3rdPersonModel != null)
			{
				vp_Utility.Activate(this.Weapon3rdPersonModel, false);
			}
		}, this.m_Weapon3rdPersonModelWakeUpTimer);
	}

	// Token: 0x06002FF4 RID: 12276 RVA: 0x000BCDB4 File Offset: 0x000BAFB4
	protected virtual void OnStart_Dead()
	{
		if (this.Player.IsFirstPerson.Get())
		{
			return;
		}
		base.Rendering = false;
	}

	// Token: 0x06002FF5 RID: 12277 RVA: 0x000BCDD5 File Offset: 0x000BAFD5
	protected virtual void OnStop_Dead()
	{
		if (this.Player.IsFirstPerson.Get())
		{
			return;
		}
		base.Rendering = true;
	}

	// Token: 0x06002FF6 RID: 12278 RVA: 0x000BCDF8 File Offset: 0x000BAFF8
	protected virtual Vector3 Get_AimDirection()
	{
		return (this.Weapon3rdPersonModel.transform.position - this.Player.LookPoint.Get()).normalized;
	}

	// Token: 0x1700034D RID: 845
	// (get) Token: 0x06002FF7 RID: 12279 RVA: 0x000BCE38 File Offset: 0x000BB038
	protected virtual Vector3 OnValue_AimDirection
	{
		get
		{
			return (this.Weapon3rdPersonModel.transform.position - this.Player.LookPoint.Get()).normalized;
		}
	}

	// Token: 0x06002FF8 RID: 12280 RVA: 0x000BCE77 File Offset: 0x000BB077
	protected virtual bool CanStart_Zoom()
	{
		return this.Player.CurrentWeaponType.Get() != 2;
	}

	// Token: 0x06002FF9 RID: 12281 RVA: 0x000BCE94 File Offset: 0x000BB094
	public override void Register(vp_EventHandler eventHandler)
	{
		base.Register(eventHandler);
		eventHandler.RegisterActivity("Zoom", null, null, new vp_Activity.Condition(this.CanStart_Zoom), null, null, null);
		eventHandler.RegisterActivity("Dead", new vp_Activity.Callback(this.OnStart_Dead), new vp_Activity.Callback(this.OnStop_Dead), null, null, null, null);
		eventHandler.RegisterValue<Vector3>("AimDirection", new vp_Value<Vector3>.Getter<Vector3>(this.Get_AimDirection), null);
	}

	// Token: 0x06002FFA RID: 12282 RVA: 0x000BCF08 File Offset: 0x000BB108
	public override void Unregister(vp_EventHandler eventHandler)
	{
		base.Unregister(eventHandler);
		eventHandler.UnregisterActivity("Zoom", null, null, new vp_Activity.Condition(this.CanStart_Zoom), null, null, null);
		eventHandler.UnregisterActivity("Dead", new vp_Activity.Callback(this.OnStart_Dead), new vp_Activity.Callback(this.OnStop_Dead), null, null, null, null);
		eventHandler.UnregisterValue<Vector3>("AimDirection", new vp_Value<Vector3>.Getter<Vector3>(this.Get_AimDirection), null);
	}

	// Token: 0x04002DF8 RID: 11768
	public GameObject Weapon3rdPersonModel;

	// Token: 0x04002DF9 RID: 11769
	protected GameObject m_WeaponModel;

	// Token: 0x04002DFA RID: 11770
	public Vector3 PositionOffset = new Vector3(0.15f, -0.15f, -0.15f);

	// Token: 0x04002DFB RID: 11771
	public float PositionSpring2Stiffness = 0.95f;

	// Token: 0x04002DFC RID: 11772
	public float PositionSpring2Damping = 0.25f;

	// Token: 0x04002DFD RID: 11773
	protected vp_Spring m_PositionSpring2;

	// Token: 0x04002DFE RID: 11774
	public Vector3 RotationOffset = Vector3.zero;

	// Token: 0x04002DFF RID: 11775
	public float RotationSpring2Stiffness = 0.95f;

	// Token: 0x04002E00 RID: 11776
	public float RotationSpring2Damping = 0.25f;

	// Token: 0x04002E01 RID: 11777
	protected vp_Spring m_RotationSpring2;

	// Token: 0x04002E02 RID: 11778
	protected bool m_Wielded = true;

	// Token: 0x04002E03 RID: 11779
	protected vp_Timer.Handle m_Weapon3rdPersonModelWakeUpTimer = new vp_Timer.Handle();

	// Token: 0x04002E04 RID: 11780
	public int AnimationType = 1;

	// Token: 0x04002E05 RID: 11781
	public int AnimationGrip = 1;

	// Token: 0x04002E06 RID: 11782
	protected vp_PlayerEventHandler m_Player;

	// Token: 0x04002E07 RID: 11783
	protected Renderer m_Weapon3rdPersonModelRenderer;

	// Token: 0x04002E08 RID: 11784
	protected Vector3 m_RotationSpring2DefaultRotation = Vector3.zero;

	// Token: 0x02000893 RID: 2195
	public new enum Type
	{
		// Token: 0x04002E0A RID: 11786
		Custom,
		// Token: 0x04002E0B RID: 11787
		Firearm,
		// Token: 0x04002E0C RID: 11788
		Melee,
		// Token: 0x04002E0D RID: 11789
		Thrown
	}

	// Token: 0x02000894 RID: 2196
	public enum Grip
	{
		// Token: 0x04002E0F RID: 11791
		Custom,
		// Token: 0x04002E10 RID: 11792
		OneHanded,
		// Token: 0x04002E11 RID: 11793
		TwoHanded,
		// Token: 0x04002E12 RID: 11794
		TwoHandedHeavy
	}
}
