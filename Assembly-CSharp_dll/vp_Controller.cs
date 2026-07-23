using System;
using UnityEngine;

// Token: 0x0200088C RID: 2188
[RequireComponent(typeof(vp_PlayerEventHandler))]
public abstract class vp_Controller : vp_Component
{
	// Token: 0x17000335 RID: 821
	// (get) Token: 0x06002F7A RID: 12154 RVA: 0x000BA94E File Offset: 0x000B8B4E
	public bool Grounded
	{
		get
		{
			return this.m_Grounded;
		}
	}

	// Token: 0x17000336 RID: 822
	// (get) Token: 0x06002F7B RID: 12155 RVA: 0x000BA956 File Offset: 0x000B8B56
	public float SkinWidth
	{
		get
		{
			return 0.08f;
		}
	}

	// Token: 0x17000337 RID: 823
	// (get) Token: 0x06002F7C RID: 12156 RVA: 0x000BA95D File Offset: 0x000B8B5D
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

	// Token: 0x06002F7D RID: 12157 RVA: 0x000BA992 File Offset: 0x000B8B92
	protected override void Awake()
	{
		base.Awake();
		this.InitCollider();
	}

	// Token: 0x06002F7E RID: 12158 RVA: 0x000BA9A0 File Offset: 0x000B8BA0
	protected override void Start()
	{
		base.Start();
		this.RefreshCollider();
	}

	// Token: 0x06002F7F RID: 12159 RVA: 0x000BA9AE File Offset: 0x000B8BAE
	protected override void Update()
	{
		base.Update();
		this.UpdatePlatformRotation();
	}

	// Token: 0x06002F80 RID: 12160 RVA: 0x000BA9BC File Offset: 0x000B8BBC
	protected override void FixedUpdate()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		this.UpdateForces();
		this.FixedMove();
		this.UpdateCollisions();
		this.UpdatePlatformMove();
		this.UpdateVelocity();
	}

	// Token: 0x06002F81 RID: 12161 RVA: 0x000BA9E9 File Offset: 0x000B8BE9
	protected virtual void UpdatePlatformMove()
	{
		if (this.m_Platform == null)
		{
			return;
		}
		this.m_PositionOnPlatform = this.m_Platform.InverseTransformPoint(this.m_Transform.position);
		this.m_LastPlatformPos = this.m_Platform.position;
	}

	// Token: 0x06002F82 RID: 12162 RVA: 0x000BAA28 File Offset: 0x000B8C28
	protected virtual void UpdatePlatformRotation()
	{
		if (this.m_Platform == null)
		{
			return;
		}
		if (this.Player.IsLocal.Get())
		{
			this.m_MovingPlatformBodyYawDif = Mathf.Lerp(this.m_MovingPlatformBodyYawDif, Mathf.DeltaAngle(this.Player.Rotation.Get().y, this.Player.BodyYaw.Get()), Time.deltaTime * 1f);
		}
		this.Player.Rotation.Set(new Vector2(this.Player.Rotation.Get().x, this.Player.Rotation.Get().y - Mathf.DeltaAngle(this.m_Platform.eulerAngles.y, this.m_LastPlatformAngle)));
		this.m_LastPlatformAngle = this.m_Platform.eulerAngles.y;
		if (this.Player.IsLocal.Get())
		{
			this.Player.BodyYaw.Set(this.Player.BodyYaw.Get() - this.m_MovingPlatformBodyYawDif);
		}
	}

	// Token: 0x06002F83 RID: 12163 RVA: 0x000BAB74 File Offset: 0x000B8D74
	protected virtual void UpdateVelocity()
	{
		this.m_PrevVelocity = this.m_Velocity;
		this.m_Velocity = (base.transform.position - this.m_PrevPosition) / Time.deltaTime;
		this.m_PrevPosition = base.Transform.position;
	}

	// Token: 0x06002F84 RID: 12164 RVA: 0x000BABC4 File Offset: 0x000B8DC4
	public virtual void Stop()
	{
		this.Player.Move.Send(Vector3.zero);
		this.Player.InputMoveVector.Set(Vector2.zero);
		this.m_FallSpeed = 0f;
		this.m_FallStartHeight = -99999f;
	}

	// Token: 0x06002F85 RID: 12165 RVA: 0x00003296 File Offset: 0x00001496
	protected virtual void InitCollider()
	{
	}

	// Token: 0x06002F86 RID: 12166 RVA: 0x00003296 File Offset: 0x00001496
	protected virtual void RefreshCollider()
	{
	}

	// Token: 0x06002F87 RID: 12167 RVA: 0x00003296 File Offset: 0x00001496
	public virtual void EnableCollider(bool enabled)
	{
	}

	// Token: 0x06002F88 RID: 12168 RVA: 0x000BAC1C File Offset: 0x000B8E1C
	protected virtual void StoreGroundInfo()
	{
		this.m_LastGroundHitTransform = this.m_GroundHitTransform;
		this.m_Grounded = false;
		this.m_GroundedNonMountain = false;
		this.m_GroundHitTransform = null;
		if (Physics.SphereCast(new Ray(base.Transform.position + Vector3.up * this.Player.Radius.Get(), Vector3.down), this.Player.Radius.Get(), out this.m_GroundHit, 0.18f, -675375893))
		{
			this.m_GroundHitTransform = this.m_GroundHit.transform;
			Collider collider = this.m_GroundHit.collider;
			if (collider != null)
			{
				Vacuumable vacuumable = null;
				if (vp_Layer.IsInMask(collider.gameObject.layer, -2206209))
				{
					vacuumable = collider.GetComponent<Vacuumable>();
				}
				this.m_Grounded = (vacuumable == null || !vacuumable.isCaptive());
				this.m_GroundedNonMountain = (this.m_Grounded && collider.gameObject.layer != 12);
			}
			else
			{
				this.m_Grounded = false;
			}
		}
		if (this.m_Velocity.y < 0f && this.m_GroundHitTransform == null && this.m_LastGroundHitTransform != null && !this.Player.Jump.Active)
		{
			this.SetFallHeight(base.Transform.position.y);
		}
	}

	// Token: 0x06002F89 RID: 12169 RVA: 0x000BAD9F File Offset: 0x000B8F9F
	private void SetFallHeight(float height)
	{
		if (this.m_FallStartHeight != -99999f)
		{
			return;
		}
		if (this.m_Grounded || this.m_GroundHitTransform != null)
		{
			return;
		}
		this.m_FallStartHeight = height;
	}

	// Token: 0x06002F8A RID: 12170 RVA: 0x000BADD0 File Offset: 0x000B8FD0
	protected virtual void UpdateForces()
	{
		this.m_LastGroundHitTransform = this.m_GroundHitTransform;
		if (this.m_Grounded && this.m_FallSpeed <= 0f)
		{
			this.m_FallSpeed = Physics.gravity.y * ((this.PhysicsGravityModifier + this.m_TempGravityModifier) * 0.002f) * vp_TimeUtility.AdjustedTimeScale;
			return;
		}
		this.m_FallSpeed = Mathf.Min(0.09f, this.m_FallSpeed + Physics.gravity.y * ((this.PhysicsGravityModifier + this.m_TempGravityModifier) * 0.002f) * vp_TimeUtility.AdjustedTimeScale);
		if (this.m_Velocity.y < 0f && this.m_PrevVelocity.y >= 0f)
		{
			this.SetFallHeight(base.Transform.position.y);
		}
	}

	// Token: 0x06002F8B RID: 12171 RVA: 0x000BAE9E File Offset: 0x000B909E
	protected virtual void FixedMove()
	{
		this.StoreGroundInfo();
	}

	// Token: 0x17000338 RID: 824
	// (get) Token: 0x06002F8C RID: 12172 RVA: 0x000BAEA6 File Offset: 0x000B90A6
	private float FallDistance
	{
		get
		{
			if (this.m_FallStartHeight == -99999f)
			{
				return 0f;
			}
			return Mathf.Max(0f, this.m_FallStartHeight - base.Transform.position.y);
		}
	}

	// Token: 0x06002F8D RID: 12173 RVA: 0x000BAEDC File Offset: 0x000B90DC
	protected virtual void UpdateCollisions()
	{
		if (this.Player.Climb.Active)
		{
			this.m_FallStartHeight = -99999f;
		}
		this.m_FallImpact = 0f;
		this.m_OnNewGround = false;
		this.m_WasFalling = false;
		if (this.m_GroundHitTransform != null && this.m_GroundHitTransform != this.m_LastGroundHitTransform)
		{
			this.m_OnNewGround = true;
			if (this.m_LastGroundHitTransform == null)
			{
				this.m_WasFalling = true;
				if (this.m_FallStartHeight > base.Transform.position.y && this.m_Grounded)
				{
					this.m_FallImpact = Mathf.Max(0f, -this.m_Velocity.y) * 0.01f;
					this.Player.FallImpact.Send(this.m_FallImpact);
				}
			}
			this.m_FallStartHeight = -99999f;
		}
	}

	// Token: 0x06002F8E RID: 12174 RVA: 0x000BAFCA File Offset: 0x000B91CA
	protected override void LateUpdate()
	{
		base.LateUpdate();
	}

	// Token: 0x06002F8F RID: 12175 RVA: 0x000BAFD2 File Offset: 0x000B91D2
	public virtual void SetPosition(Vector3 position)
	{
		base.Transform.position = position;
		this.m_PrevPosition = position;
		vp_Timer.In(0f, delegate()
		{
			this.m_PrevVelocity = vp_3DUtility.HorizontalVector(this.m_PrevVelocity);
		}, null);
	}

	// Token: 0x06002F90 RID: 12176 RVA: 0x000BB000 File Offset: 0x000B9200
	public void PushRigidbody(Rigidbody rigidbody, Vector3 moveDirection, vp_Controller.PushForceMode pushForcemode, Vector3 point)
	{
		if (pushForcemode == vp_Controller.PushForceMode.Simplified)
		{
			rigidbody.velocity += vp_3DUtility.HorizontalVector(new Vector3(moveDirection.x, 0f, moveDirection.z).normalized) * (this.PhysicsPushForce / rigidbody.mass);
			return;
		}
		if (pushForcemode != vp_Controller.PushForceMode.Kinetic)
		{
			return;
		}
		if (Vector3.Distance(vp_3DUtility.HorizontalVector(base.Transform.position), vp_3DUtility.HorizontalVector(point)) > this.Player.Radius.Get())
		{
			rigidbody.AddForceAtPosition(vp_3DUtility.HorizontalVector(moveDirection) * (this.PhysicsPushForce * 15f), point);
			return;
		}
		rigidbody.AddForceAtPosition(moveDirection * (this.PhysicsPushForce * 15f), point);
	}

	// Token: 0x06002F91 RID: 12177 RVA: 0x000BB0CC File Offset: 0x000B92CC
	public void PushRigidbody(Rigidbody rigidbody, Vector3 moveDirection, vp_Controller.PushForceMode pushForceMode)
	{
		this.PushRigidbody(rigidbody, moveDirection, pushForceMode, (pushForceMode == vp_Controller.PushForceMode.Simplified) ? Vector3.zero : rigidbody.ClosestPointOnBounds(base.Collider.bounds.center));
	}

	// Token: 0x06002F92 RID: 12178 RVA: 0x000BB108 File Offset: 0x000B9308
	public void PushRigidbody(Rigidbody rigidbody, Vector3 moveDirection)
	{
		this.PushRigidbody(rigidbody, moveDirection, this.PhysicsPushMode, (this.PhysicsPushMode == vp_Controller.PushForceMode.Simplified) ? Vector3.zero : rigidbody.ClosestPointOnBounds(base.Collider.bounds.center));
	}

	// Token: 0x06002F93 RID: 12179 RVA: 0x000BB14B File Offset: 0x000B934B
	public void PushRigidbody(Rigidbody rigidbody, Vector3 moveDirection, Vector3 point)
	{
		this.PushRigidbody(rigidbody, moveDirection, this.PhysicsPushMode, point);
	}

	// Token: 0x06002F94 RID: 12180 RVA: 0x000BB15C File Offset: 0x000B935C
	protected virtual void OnMessage_Stop()
	{
		this.Stop();
	}

	// Token: 0x06002F95 RID: 12181 RVA: 0x000BB164 File Offset: 0x000B9364
	protected virtual void OnStart_Crouch()
	{
		this.Player.Run.Stop(0f);
		this.RefreshCollider();
	}

	// Token: 0x06002F96 RID: 12182 RVA: 0x000BB181 File Offset: 0x000B9381
	protected virtual void OnStop_Crouch()
	{
		this.RefreshCollider();
	}

	// Token: 0x06002F97 RID: 12183 RVA: 0x000BB189 File Offset: 0x000B9389
	protected virtual Transform Get_Platform()
	{
		return this.m_Platform;
	}

	// Token: 0x06002F98 RID: 12184 RVA: 0x000BB191 File Offset: 0x000B9391
	protected virtual void Set_Platform(Transform value)
	{
		this.m_Platform = value;
	}

	// Token: 0x17000339 RID: 825
	// (get) Token: 0x06002F99 RID: 12185 RVA: 0x000BB189 File Offset: 0x000B9389
	// (set) Token: 0x06002F9A RID: 12186 RVA: 0x000BB191 File Offset: 0x000B9391
	protected virtual Transform OnValue_Platform
	{
		get
		{
			return this.m_Platform;
		}
		set
		{
			this.m_Platform = value;
		}
	}

	// Token: 0x06002F9B RID: 12187 RVA: 0x000BB19A File Offset: 0x000B939A
	protected virtual Vector3 Get_Position()
	{
		return base.Transform.position;
	}

	// Token: 0x06002F9C RID: 12188 RVA: 0x000BB1A7 File Offset: 0x000B93A7
	protected virtual void Set_Position(Vector3 value)
	{
		this.SetPosition(value);
	}

	// Token: 0x1700033A RID: 826
	// (get) Token: 0x06002F9D RID: 12189 RVA: 0x000BB19A File Offset: 0x000B939A
	// (set) Token: 0x06002F9E RID: 12190 RVA: 0x000BB1A7 File Offset: 0x000B93A7
	protected virtual Vector3 OnValue_Position
	{
		get
		{
			return base.Transform.position;
		}
		set
		{
			this.SetPosition(value);
		}
	}

	// Token: 0x06002F9F RID: 12191 RVA: 0x000BB1B0 File Offset: 0x000B93B0
	protected virtual float Get_FallSpeed()
	{
		return this.m_FallSpeed;
	}

	// Token: 0x06002FA0 RID: 12192 RVA: 0x000BB1B8 File Offset: 0x000B93B8
	protected virtual void Set_FallSpeed(float value)
	{
		this.m_FallSpeed = value;
	}

	// Token: 0x1700033B RID: 827
	// (get) Token: 0x06002FA1 RID: 12193 RVA: 0x000BB1B0 File Offset: 0x000B93B0
	// (set) Token: 0x06002FA2 RID: 12194 RVA: 0x000BB1B8 File Offset: 0x000B93B8
	protected virtual float OnValue_FallSpeed
	{
		get
		{
			return this.m_FallSpeed;
		}
		set
		{
			this.m_FallSpeed = value;
		}
	}

	// Token: 0x06002FA3 RID: 12195 RVA: 0x000BB1C1 File Offset: 0x000B93C1
	protected virtual Vector3 Get_Velocity()
	{
		return this.m_Velocity;
	}

	// Token: 0x06002FA4 RID: 12196 RVA: 0x000BB1C9 File Offset: 0x000B93C9
	protected virtual void Set_Velocity(Vector3 value)
	{
		this.m_Velocity = value;
	}

	// Token: 0x1700033C RID: 828
	// (get) Token: 0x06002FA5 RID: 12197 RVA: 0x000BB1C1 File Offset: 0x000B93C1
	// (set) Token: 0x06002FA6 RID: 12198 RVA: 0x000BB1C9 File Offset: 0x000B93C9
	protected virtual Vector3 OnValue_Velocity
	{
		get
		{
			return this.m_Velocity;
		}
		set
		{
			this.m_Velocity = value;
		}
	}

	// Token: 0x06002FA7 RID: 12199
	protected abstract float Get_Radius();

	// Token: 0x1700033D RID: 829
	// (get) Token: 0x06002FA8 RID: 12200
	protected abstract float OnValue_Radius { get; }

	// Token: 0x06002FA9 RID: 12201
	protected abstract float Get_Height();

	// Token: 0x1700033E RID: 830
	// (get) Token: 0x06002FAA RID: 12202
	protected abstract float OnValue_Height { get; }

	// Token: 0x06002FAB RID: 12203 RVA: 0x000BB1D2 File Offset: 0x000B93D2
	public void AdjustFallSpeed(float fallAdjust)
	{
		this.m_FallSpeed += fallAdjust;
	}

	// Token: 0x06002FAC RID: 12204 RVA: 0x000BB1E2 File Offset: 0x000B93E2
	public void SetTempGravityModifier(float tempGravityModifier)
	{
		this.m_TempGravityModifier = tempGravityModifier;
	}

	// Token: 0x06002FAD RID: 12205 RVA: 0x000BB1EB File Offset: 0x000B93EB
	protected string Vector3ToString(Vector3 s)
	{
		return string.Format("x={0}|y={1}|z={2}", s.x, s.y, s.z);
	}

	// Token: 0x06002FAE RID: 12206 RVA: 0x000BB218 File Offset: 0x000B9418
	public override void Register(vp_EventHandler eventHandler)
	{
		base.Register(eventHandler);
		eventHandler.RegisterMessage("Stop", new vp_Message.Sender(this.OnMessage_Stop));
		eventHandler.RegisterActivity("Crouch", new vp_Activity.Callback(this.OnStart_Crouch), new vp_Activity.Callback(this.OnStop_Crouch), null, null, null, null);
		eventHandler.RegisterValue<float>("FallSpeed", new vp_Value<float>.Getter<float>(this.Get_FallSpeed), new vp_Value<float>.Setter<float>(this.Set_FallSpeed));
		eventHandler.RegisterValue<float>("Height", new vp_Value<float>.Getter<float>(this.Get_Height), null);
		eventHandler.RegisterValue<Transform>("Platform", new vp_Value<Transform>.Getter<Transform>(this.Get_Platform), new vp_Value<Transform>.Setter<Transform>(this.Set_Platform));
		eventHandler.RegisterValue<float>("Radius", new vp_Value<float>.Getter<float>(this.Get_Radius), null);
		eventHandler.RegisterValue<Vector3>("Velocity", new vp_Value<Vector3>.Getter<Vector3>(this.Get_Velocity), new vp_Value<Vector3>.Setter<Vector3>(this.Set_Velocity));
	}

	// Token: 0x06002FAF RID: 12207 RVA: 0x000BB310 File Offset: 0x000B9510
	public override void Unregister(vp_EventHandler eventHandler)
	{
		base.Unregister(eventHandler);
		eventHandler.UnregisterMessage("Stop", new vp_Message.Sender(this.OnMessage_Stop));
		eventHandler.UnregisterActivity("Crouch", new vp_Activity.Callback(this.OnStart_Crouch), new vp_Activity.Callback(this.OnStop_Crouch), null, null, null, null);
		eventHandler.UnregisterValue<float>("FallSpeed", new vp_Value<float>.Getter<float>(this.Get_FallSpeed), new vp_Value<float>.Setter<float>(this.Set_FallSpeed));
		eventHandler.UnregisterValue<float>("Height", new vp_Value<float>.Getter<float>(this.Get_Height), null);
		eventHandler.UnregisterValue<Transform>("Platform", new vp_Value<Transform>.Getter<Transform>(this.Get_Platform), new vp_Value<Transform>.Setter<Transform>(this.Set_Platform));
		eventHandler.UnregisterValue<float>("Radius", new vp_Value<float>.Getter<float>(this.Get_Radius), null);
		eventHandler.UnregisterValue<Vector3>("Velocity", new vp_Value<Vector3>.Getter<Vector3>(this.Get_Velocity), new vp_Value<Vector3>.Setter<Vector3>(this.Set_Velocity));
	}

	// Token: 0x04002D77 RID: 11639
	protected bool m_Grounded;

	// Token: 0x04002D78 RID: 11640
	protected bool m_GroundedNonMountain;

	// Token: 0x04002D79 RID: 11641
	protected RaycastHit m_GroundHit;

	// Token: 0x04002D7A RID: 11642
	protected Transform m_LastGroundHitTransform;

	// Token: 0x04002D7B RID: 11643
	protected Transform m_GroundHitTransform;

	// Token: 0x04002D7C RID: 11644
	protected float m_FallStartHeight = -99999f;

	// Token: 0x04002D7D RID: 11645
	protected float m_FallImpact;

	// Token: 0x04002D7E RID: 11646
	protected bool m_OnNewGround;

	// Token: 0x04002D7F RID: 11647
	protected bool m_WasFalling;

	// Token: 0x04002D80 RID: 11648
	public float PhysicsGravityModifier = 0.2f;

	// Token: 0x04002D81 RID: 11649
	protected float m_FallSpeed;

	// Token: 0x04002D82 RID: 11650
	public bool MotorFreeFly;

	// Token: 0x04002D83 RID: 11651
	public float PhysicsPushForce = 5f;

	// Token: 0x04002D84 RID: 11652
	public vp_Controller.PushForceMode PhysicsPushMode;

	// Token: 0x04002D85 RID: 11653
	public float PhysicsPushInterval = 0.1f;

	// Token: 0x04002D86 RID: 11654
	public float PhysicsCrouchHeightModifier = 0.5f;

	// Token: 0x04002D87 RID: 11655
	public Vector3 m_Velocity = Vector3.zero;

	// Token: 0x04002D88 RID: 11656
	protected Vector3 m_PrevPosition = Vector3.zero;

	// Token: 0x04002D89 RID: 11657
	protected Vector3 m_PrevVelocity = Vector3.zero;

	// Token: 0x04002D8A RID: 11658
	protected float m_NextAllowedPushTime;

	// Token: 0x04002D8B RID: 11659
	protected Transform m_Platform;

	// Token: 0x04002D8C RID: 11660
	public Vector3 m_PositionOnPlatform = Vector3.zero;

	// Token: 0x04002D8D RID: 11661
	protected float m_LastPlatformAngle;

	// Token: 0x04002D8E RID: 11662
	public Vector3 m_LastPlatformPos = Vector3.zero;

	// Token: 0x04002D8F RID: 11663
	protected float m_MovingPlatformBodyYawDif;

	// Token: 0x04002D90 RID: 11664
	protected float m_NormalHeight;

	// Token: 0x04002D91 RID: 11665
	protected Vector3 m_NormalCenter = Vector3.zero;

	// Token: 0x04002D92 RID: 11666
	protected float m_CrouchHeight;

	// Token: 0x04002D93 RID: 11667
	protected Vector3 m_CrouchCenter = Vector3.zero;

	// Token: 0x04002D94 RID: 11668
	protected float m_TempGravityModifier;

	// Token: 0x04002D95 RID: 11669
	protected const float KINETIC_PUSHFORCE_MULTIPLIER = 15f;

	// Token: 0x04002D96 RID: 11670
	protected const float CHARACTER_CONTROLLER_SKINWIDTH = 0.08f;

	// Token: 0x04002D97 RID: 11671
	protected const float DEFAULT_RADIUS_MULTIPLIER = 0.25f;

	// Token: 0x04002D98 RID: 11672
	protected const float FALL_IMPACT_MULTIPLIER = 0.075f;

	// Token: 0x04002D99 RID: 11673
	protected const float NOFALL = -99999f;

	// Token: 0x04002D9A RID: 11674
	protected const float VEL_FALL_IMPACT_MULTIPLIER = 0.01f;

	// Token: 0x04002D9B RID: 11675
	protected const float MAX_FALL_SPEED_VALUE = 0.09f;

	// Token: 0x04002D9C RID: 11676
	private vp_PlayerEventHandler m_Player;

	// Token: 0x0200088D RID: 2189
	public enum PushForceMode
	{
		// Token: 0x04002D9E RID: 11678
		Simplified,
		// Token: 0x04002D9F RID: 11679
		Kinetic
	}
}
