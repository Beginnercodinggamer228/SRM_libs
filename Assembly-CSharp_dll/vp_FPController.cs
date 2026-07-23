using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200087D RID: 2173
public class vp_FPController : vp_CharacterController
{
	// Token: 0x170002F8 RID: 760
	// (get) Token: 0x06002E3A RID: 11834 RVA: 0x000B218C File Offset: 0x000B038C
	public Vector3 SmoothPosition
	{
		get
		{
			return this.m_SmoothPosition;
		}
	}

	// Token: 0x170002F9 RID: 761
	// (get) Token: 0x06002E3B RID: 11835 RVA: 0x000B2194 File Offset: 0x000B0394
	public Vector3 Velocity
	{
		get
		{
			return base.CharacterController.velocity;
		}
	}

	// Token: 0x170002FA RID: 762
	// (get) Token: 0x06002E3C RID: 11836 RVA: 0x000B21A1 File Offset: 0x000B03A1
	public bool HeadContact
	{
		get
		{
			return this.m_HeadContact;
		}
	}

	// Token: 0x170002FB RID: 763
	// (get) Token: 0x06002E3D RID: 11837 RVA: 0x000B21A9 File Offset: 0x000B03A9
	public Vector3 GroundNormal
	{
		get
		{
			return this.m_GroundHit.normal;
		}
	}

	// Token: 0x170002FC RID: 764
	// (get) Token: 0x06002E3E RID: 11838 RVA: 0x000B21B6 File Offset: 0x000B03B6
	public float GroundAngle
	{
		get
		{
			return Vector3.Angle(this.m_GroundHit.normal, Vector3.up);
		}
	}

	// Token: 0x170002FD RID: 765
	// (get) Token: 0x06002E3F RID: 11839 RVA: 0x000B21CD File Offset: 0x000B03CD
	public Transform GroundTransform
	{
		get
		{
			return this.m_GroundHitTransform.transform;
		}
	}

	// Token: 0x170002FE RID: 766
	// (get) Token: 0x06002E40 RID: 11840 RVA: 0x000B21DA File Offset: 0x000B03DA
	public bool GroundedNonMountain
	{
		get
		{
			return this.m_GroundedNonMountain && this.GroundAngle <= base.Player.SlopeLimit.Get();
		}
	}

	// Token: 0x06002E41 RID: 11841 RVA: 0x000B2206 File Offset: 0x000B0406
	public void AddDepenetrationForce(Vector3 force)
	{
		this.m_DepenetrationForce += force;
	}

	// Token: 0x06002E42 RID: 11842 RVA: 0x000B221A File Offset: 0x000B041A
	private void ResetDepenetrationForce()
	{
		this.m_DepenetrationForce = Vector3.zero;
	}

	// Token: 0x06002E43 RID: 11843 RVA: 0x000B2227 File Offset: 0x000B0427
	protected override void OnEnable()
	{
		base.OnEnable();
	}

	// Token: 0x06002E44 RID: 11844 RVA: 0x000B222F File Offset: 0x000B042F
	protected override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x06002E45 RID: 11845 RVA: 0x000B2238 File Offset: 0x000B0438
	protected override void Start()
	{
		base.Start();
		this.SetPosition(base.Transform.position);
		if (this.PhysicsHasCollisionTrigger)
		{
			this.m_Trigger = new GameObject("Trigger");
			this.m_Trigger.transform.parent = this.m_Transform;
			this.m_Trigger.layer = 8;
			this.m_Trigger.transform.localPosition = Vector3.zero;
			this.m_TriggerCollider = this.m_Trigger.AddComponent<CapsuleCollider>();
			this.m_TriggerCollider.isTrigger = true;
			this.m_TriggerCollider.radius = base.CharacterController.radius + base.SkinWidth;
			this.m_TriggerCollider.height = base.CharacterController.height + base.SkinWidth * 2f;
			this.m_TriggerCollider.center = base.CharacterController.center;
		}
		if (vp_FPController.onStartDelegate != null)
		{
			vp_FPController.onStartDelegate(this);
		}
	}

	// Token: 0x06002E46 RID: 11846 RVA: 0x000B2334 File Offset: 0x000B0534
	protected override void RefreshCollider()
	{
		base.RefreshCollider();
		if (this.m_TriggerCollider != null)
		{
			this.m_TriggerCollider.radius = base.CharacterController.radius + base.SkinWidth;
			this.m_TriggerCollider.height = base.CharacterController.height + base.SkinWidth * 2f;
			this.m_TriggerCollider.center = base.CharacterController.center;
		}
	}

	// Token: 0x06002E47 RID: 11847 RVA: 0x000B23AB File Offset: 0x000B05AB
	public override void EnableCollider(bool isEnabled = true)
	{
		if (base.CharacterController != null)
		{
			base.CharacterController.enabled = isEnabled;
		}
	}

	// Token: 0x06002E48 RID: 11848 RVA: 0x000B23C7 File Offset: 0x000B05C7
	protected override void Update()
	{
		base.Update();
		this.SmoothMove();
	}

	// Token: 0x06002E49 RID: 11849 RVA: 0x000B23D8 File Offset: 0x000B05D8
	protected override void FixedUpdate()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		this.UpdateMotor();
		this.UpdateJump();
		this.UpdateForces();
		this.UpdateSliding();
		this.UpdateOutOfControl();
		if (this.MotorFreeFly)
		{
			this.m_FallSpeed = 0f;
		}
		this.FixedMove();
		this.UpdateCollisions();
		this.UpdatePlatformMove();
		this.UpdateVelocity();
	}

	// Token: 0x06002E4A RID: 11850 RVA: 0x000B243B File Offset: 0x000B063B
	protected virtual void UpdateMotor()
	{
		if (!this.MotorFreeFly)
		{
			this.UpdateThrottleWalk();
		}
		else
		{
			this.UpdateThrottleFree();
		}
		this.m_MotorThrottle = vp_MathUtility.SnapToZero(this.m_MotorThrottle, 0.0001f);
	}

	// Token: 0x06002E4B RID: 11851 RVA: 0x000B246C File Offset: 0x000B066C
	protected virtual void UpdateThrottleWalk()
	{
		this.m_MotorAirSpeedModifier = (this.m_Grounded ? 1f : this.MotorAirSpeed);
		Vector3 vector = ((base.Player.InputMoveVector.Get().y > 0f) ? base.Player.InputMoveVector.Get().y : (base.Player.InputMoveVector.Get().y * this.MotorBackwardsSpeed)) * base.Transform.TransformDirection(Vector3.forward * (this.MotorAcceleration * 0.1f) * this.m_MotorAirSpeedModifier);
		Vector3 vector2 = base.Player.InputMoveVector.Get().x * base.Transform.TransformDirection(Vector3.right * (this.MotorAcceleration * 0.1f) * this.m_MotorAirSpeedModifier);
		this.m_MotorThrottle += vector * this.CalculateSlopeFactor(vector) + vector2 * this.CalculateSlopeFactor(vector2);
		this.m_MotorThrottle.x = this.m_MotorThrottle.x / (1f + this.MotorDamping * this.m_MotorAirSpeedModifier * Time.timeScale);
		this.m_MotorThrottle.z = this.m_MotorThrottle.z / (1f + this.MotorDamping * this.m_MotorAirSpeedModifier * Time.timeScale);
	}

	// Token: 0x06002E4C RID: 11852 RVA: 0x000B25F0 File Offset: 0x000B07F0
	protected virtual void UpdateThrottleFree()
	{
		bool isPressed = SRInput.Actions.run.IsPressed;
		this.m_MotorThrottle += base.Player.InputMoveVector.Get().y * base.Transform.TransformDirection(base.Transform.InverseTransformDirection(((vp_FPPlayerEventHandler)base.Player).CameraLookDirection.Get()) * (this.MotorAcceleration * 0.1f * (isPressed ? 3f : 1f)));
		this.m_MotorThrottle += base.Player.InputMoveVector.Get().x * base.Transform.TransformDirection(Vector3.right * (this.MotorAcceleration * 0.1f * (isPressed ? 3f : 1f)));
		this.m_MotorThrottle.x = this.m_MotorThrottle.x / (1f + this.MotorDamping * Time.timeScale);
		this.m_MotorThrottle.z = this.m_MotorThrottle.z / (1f + this.MotorDamping * Time.timeScale);
	}

	// Token: 0x06002E4D RID: 11853 RVA: 0x000B2734 File Offset: 0x000B0934
	protected virtual void UpdateJump()
	{
		if (this.m_HeadContact)
		{
			base.Player.Jump.Stop(1f);
		}
		if (!this.MotorFreeFly)
		{
			this.UpdateJumpForceWalk();
		}
		else
		{
			this.UpdateJumpForceFree();
		}
		this.m_MotorThrottle.y = this.m_MotorThrottle.y + this.m_MotorJumpForceAcc * Time.timeScale;
		this.m_MotorJumpForceAcc /= 1f + this.MotorJumpForceHoldDamping * Time.timeScale;
		this.m_MotorThrottle.y = this.m_MotorThrottle.y / (1f + this.MotorJumpForceDamping * Time.timeScale);
	}

	// Token: 0x06002E4E RID: 11854 RVA: 0x000B27D0 File Offset: 0x000B09D0
	protected virtual void UpdateJumpForceWalk()
	{
		if (base.Player.Jump.Active && !base.Player.Jetpack.Active && !this.m_Grounded)
		{
			if (this.m_MotorJumpForceHoldSkipFrames > 2)
			{
				if (base.Player.Velocity.Get().y >= 0f)
				{
					this.m_MotorJumpForceAcc += this.MotorJumpForceHold;
					return;
				}
			}
			else
			{
				this.m_MotorJumpForceHoldSkipFrames++;
			}
		}
	}

	// Token: 0x06002E4F RID: 11855 RVA: 0x000B2858 File Offset: 0x000B0A58
	protected virtual void UpdateJumpForceFree()
	{
		if (base.Player.Jump.Active && base.Player.Crouch.Active)
		{
			return;
		}
		if (base.Player.Jump.Active)
		{
			this.m_MotorJumpForceAcc += this.MotorJumpForceHold;
			return;
		}
		if (base.Player.Crouch.Active)
		{
			this.m_MotorJumpForceAcc -= this.MotorJumpForceHold;
			if (base.Grounded && base.CharacterController.height == this.m_NormalHeight)
			{
				base.CharacterController.height = this.m_CrouchHeight;
				base.CharacterController.center = this.m_CrouchCenter;
			}
		}
	}

	// Token: 0x06002E50 RID: 11856 RVA: 0x000B2914 File Offset: 0x000B0B14
	protected override void UpdateForces()
	{
		base.UpdateForces();
		if (this.m_SmoothForceFrame[0] != Vector3.zero)
		{
			this.AddForceInternal(this.m_SmoothForceFrame[0]);
			for (int i = 0; i < 120; i++)
			{
				this.m_SmoothForceFrame[i] = ((i < 119) ? this.m_SmoothForceFrame[i + 1] : Vector3.zero);
				if (this.m_SmoothForceFrame[i] == Vector3.zero)
				{
					break;
				}
			}
		}
		this.m_ExternalForce /= 1f + this.PhysicsForceDamping * vp_TimeUtility.AdjustedTimeScale;
	}

	// Token: 0x06002E51 RID: 11857 RVA: 0x000B29C0 File Offset: 0x000B0BC0
	protected virtual void UpdateSliding()
	{
		bool slideFast = this.m_SlideFast;
		bool slide = this.m_Slide;
		this.m_Slide = false;
		if (!this.m_Grounded)
		{
			this.m_OnSteepGroundSince = 0f;
			this.m_SlideFast = false;
		}
		else if (this.GroundAngle > this.PhysicsSlopeSlideLimit || !this.m_GroundedNonMountain)
		{
			this.m_Slide = true;
			if (this.GroundAngle <= base.Player.SlopeLimit.Get())
			{
				this.m_SlopeSlideSpeed = Mathf.Max(this.m_SlopeSlideSpeed, this.PhysicsSlopeSlidiness * 0.01f);
				this.m_OnSteepGroundSince = 0f;
				this.m_SlideFast = false;
				this.m_SlopeSlideSpeed = ((Mathf.Abs(this.m_SlopeSlideSpeed) < 0.0001f) ? 0f : (this.m_SlopeSlideSpeed / (1f + 0.05f * vp_TimeUtility.AdjustedTimeScale)));
			}
			else
			{
				if (this.m_SlopeSlideSpeed > 0.01f)
				{
					this.m_SlideFast = true;
				}
				if (this.m_OnSteepGroundSince == 0f)
				{
					this.m_OnSteepGroundSince = Time.time;
				}
				this.m_SlopeSlideSpeed += this.PhysicsSlopeSlidiness * 0.01f * ((Time.time - this.m_OnSteepGroundSince) * 0.125f) * vp_TimeUtility.AdjustedTimeScale;
				this.m_SlopeSlideSpeed = Mathf.Max(this.PhysicsSlopeSlidiness * 0.01f, this.m_SlopeSlideSpeed);
			}
			this.AddForce(Vector3.Cross(Vector3.Cross(this.GroundNormal, Vector3.down), this.GroundNormal) * this.m_SlopeSlideSpeed * vp_TimeUtility.AdjustedTimeScale);
		}
		else
		{
			this.m_OnSteepGroundSince = 0f;
			this.m_SlideFast = false;
			this.m_SlopeSlideSpeed = 0f;
		}
		if (this.m_MotorThrottle != Vector3.zero)
		{
			this.m_Slide = false;
		}
		if (this.m_SlideFast)
		{
			this.m_SlideFallSpeed = base.Transform.position.y;
		}
		else if (slideFast && !base.Grounded)
		{
			this.m_FallSpeed = Mathf.Min(0f, base.Transform.position.y - this.m_SlideFallSpeed);
		}
		if (slide != this.m_Slide)
		{
			base.Player.SetState("Slide", this.m_Slide, true, false);
		}
	}

	// Token: 0x06002E52 RID: 11858 RVA: 0x000B2C00 File Offset: 0x000B0E00
	private void UpdateOutOfControl()
	{
		if (this.m_ExternalForce.magnitude > 0.2f || this.m_FallSpeed < -0.2f || this.m_SlideFast)
		{
			base.Player.OutOfControl.Start(0f);
			return;
		}
		if (base.Player.OutOfControl.Active)
		{
			base.Player.OutOfControl.Stop(0f);
		}
	}

	// Token: 0x06002E53 RID: 11859 RVA: 0x000B2C74 File Offset: 0x000B0E74
	protected override void FixedMove()
	{
		this.m_MoveDirection = Vector3.zero;
		this.m_MoveDirection += this.m_ExternalForce;
		this.m_MoveDirection += this.m_DepenetrationForce;
		this.m_MoveDirection += this.m_MotorThrottle;
		this.m_MoveDirection.y = this.m_MoveDirection.y + this.m_FallSpeed;
		this.ResetDepenetrationForce();
		this.m_CurrentAntiBumpOffset = 0f;
		if (this.m_Grounded && this.m_MotorThrottle.y <= 0.001f && !base.Player.Jetpack.Active)
		{
			this.m_CurrentAntiBumpOffset = Mathf.Max(base.Player.StepOffset.Get(), Vector3.Scale(this.m_MoveDirection, Vector3.one - Vector3.up).magnitude);
			this.m_MoveDirection += this.m_CurrentAntiBumpOffset * Vector3.down;
		}
		this.m_PredictedPos = base.Transform.position + vp_MathUtility.NaNSafeVector3(this.m_MoveDirection * base.Delta * Time.timeScale, default(Vector3));
		if (this.m_Platform != null && this.m_PositionOnPlatform != Vector3.zero)
		{
			base.Player.Move.Send(vp_MathUtility.NaNSafeVector3(this.m_Platform.TransformPoint(this.m_PositionOnPlatform) - this.m_Transform.position, default(Vector3)));
		}
		base.Player.Move.Send(vp_MathUtility.NaNSafeVector3(this.m_MoveDirection * base.Delta * Time.timeScale, default(Vector3)));
		if (base.Player.Dead.Active)
		{
			base.Player.InputMoveVector.Set(Vector2.zero);
			return;
		}
		this.StoreGroundInfo();
		if (!this.m_Grounded && base.Player.Velocity.Get().y > 0f)
		{
			Physics.SphereCast(new Ray(base.Transform.position, Vector3.up), base.Player.Radius.Get(), out this.m_CeilingHit, base.Player.Height.Get() - (base.Player.Radius.Get() - base.SkinWidth) + 0.01f, -675375893);
			this.m_HeadContact = (this.m_CeilingHit.collider != null);
		}
		else
		{
			this.m_HeadContact = false;
		}
		if (this.m_GroundHitTransform == null && this.m_LastGroundHitTransform != null)
		{
			if (this.m_Platform != null && this.m_PositionOnPlatform != Vector3.zero)
			{
				this.AddForce(this.m_Platform.position - this.m_LastPlatformPos);
				this.m_Platform = null;
			}
			if (this.m_CurrentAntiBumpOffset != 0f)
			{
				base.Player.Move.Send(vp_MathUtility.NaNSafeVector3(this.m_CurrentAntiBumpOffset * Vector3.up, default(Vector3)) * base.Delta * Time.timeScale);
				this.m_PredictedPos += vp_MathUtility.NaNSafeVector3(this.m_CurrentAntiBumpOffset * Vector3.up, default(Vector3)) * base.Delta * Time.timeScale;
				this.m_MoveDirection += this.m_CurrentAntiBumpOffset * Vector3.up;
			}
		}
	}

	// Token: 0x06002E54 RID: 11860 RVA: 0x000B3084 File Offset: 0x000B1284
	protected virtual void SmoothMove()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		this.m_FixedPosition = base.Transform.position;
		base.Transform.position = this.m_SmoothPosition;
		base.Player.Move.Send(vp_MathUtility.NaNSafeVector3(this.m_MoveDirection * base.Delta * Time.timeScale, default(Vector3)));
		this.m_SmoothPosition = base.Transform.position;
		base.Transform.position = this.m_FixedPosition;
		if (Vector3.Distance(base.Transform.position, this.m_SmoothPosition) > base.Player.Radius.Get() || (this.m_Platform != null && this.m_LastPlatformPos != this.m_Platform.position))
		{
			this.m_SmoothPosition = base.Transform.position;
		}
		this.m_SmoothPosition = Vector3.Lerp(this.m_SmoothPosition, base.Transform.position, Time.deltaTime);
	}

	// Token: 0x06002E55 RID: 11861 RVA: 0x000B31A8 File Offset: 0x000B13A8
	protected override void UpdateCollisions()
	{
		base.UpdateCollisions();
		if (this.m_OnNewGround)
		{
			if (this.m_WasFalling && this.Velocity.y <= 0f)
			{
				this.DeflectDownForce();
				this.m_SmoothPosition.y = base.Transform.position.y;
				this.m_MotorThrottle.y = 0f;
				this.m_MotorJumpForceAcc = 0f;
				this.m_MotorJumpForceHoldSkipFrames = 0;
			}
			if (this.m_GroundHit.collider.gameObject.layer == 28)
			{
				this.m_Platform = this.m_GroundHit.transform;
				this.m_LastPlatformAngle = this.m_Platform.eulerAngles.y;
			}
			else
			{
				this.m_Platform = null;
			}
			Terrain component = this.m_GroundHitTransform.GetComponent<Terrain>();
			if (component != null)
			{
				this.m_CurrentTerrain = component;
			}
			else
			{
				this.m_CurrentTerrain = null;
			}
			vp_SurfaceIdentifier component2 = this.m_GroundHitTransform.GetComponent<vp_SurfaceIdentifier>();
			if (component2 != null)
			{
				this.m_CurrentSurface = component2;
			}
			else
			{
				this.m_CurrentSurface = null;
			}
		}
		if (this.m_PredictedPos.y > base.Transform.position.y && (this.m_ExternalForce.y > 0f || this.m_MotorThrottle.y > 0f))
		{
			this.DeflectUpForce();
		}
	}

	// Token: 0x06002E56 RID: 11862 RVA: 0x000B3300 File Offset: 0x000B1500
	protected virtual float CalculateSlopeFactor(Vector3 diff)
	{
		if (!this.m_Grounded)
		{
			return 1f;
		}
		float num = Vector3.Angle(this.m_GroundHit.normal, diff);
		float num2 = 1f + (1f - num / 90f);
		if (Mathf.Abs(1f - num2) < 0.01f)
		{
			num2 = 1f;
		}
		else if (num2 > 1f)
		{
			num2 *= this.MotorSlopeSpeedDown;
		}
		else
		{
			num2 *= this.MotorSlopeSpeedUp;
			if (num > base.Player.SlopeLimit.Get() + 90f)
			{
				num2 = 0f;
			}
		}
		return num2;
	}

	// Token: 0x06002E57 RID: 11863 RVA: 0x000B339D File Offset: 0x000B159D
	protected override void UpdatePlatformMove()
	{
		base.UpdatePlatformMove();
		if (this.m_Platform != null)
		{
			this.m_SmoothPosition = base.Transform.position;
		}
	}

	// Token: 0x06002E58 RID: 11864 RVA: 0x000B33C4 File Offset: 0x000B15C4
	protected override void UpdatePlatformRotation()
	{
		if (this.m_Platform == null)
		{
			return;
		}
		base.UpdatePlatformRotation();
	}

	// Token: 0x06002E59 RID: 11865 RVA: 0x000B33DB File Offset: 0x000B15DB
	public override void SetPosition(Vector3 position)
	{
		base.SetPosition(position);
		this.m_SmoothPosition = position;
	}

	// Token: 0x06002E5A RID: 11866 RVA: 0x000B33EB File Offset: 0x000B15EB
	protected virtual void AddForceInternal(Vector3 force)
	{
		this.m_ExternalForce += force;
	}

	// Token: 0x06002E5B RID: 11867 RVA: 0x000B33FF File Offset: 0x000B15FF
	public virtual void AddForce(float x, float y, float z)
	{
		this.AddForce(new Vector3(x, y, z));
	}

	// Token: 0x06002E5C RID: 11868 RVA: 0x000B340F File Offset: 0x000B160F
	public virtual void AddForce(Vector3 force)
	{
		if (Time.timeScale >= 1f)
		{
			this.AddForceInternal(force);
			return;
		}
		this.AddSoftForce(force, 1f);
	}

	// Token: 0x06002E5D RID: 11869 RVA: 0x000B3434 File Offset: 0x000B1634
	public virtual void AddSoftForce(Vector3 force, float frames)
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		force /= Time.timeScale;
		frames = Mathf.Clamp(frames, 1f, 120f);
		this.AddForceInternal(force / frames);
		for (int i = 0; i < Mathf.RoundToInt(frames) - 1; i++)
		{
			this.m_SmoothForceFrame[i] += force / frames;
		}
	}

	// Token: 0x06002E5E RID: 11870 RVA: 0x000B34B0 File Offset: 0x000B16B0
	public virtual void StopSoftForce()
	{
		int num = 0;
		while (num < 120 && !(this.m_SmoothForceFrame[num] == Vector3.zero))
		{
			this.m_SmoothForceFrame[num] = Vector3.zero;
			num++;
		}
	}

	// Token: 0x06002E5F RID: 11871 RVA: 0x000B34F4 File Offset: 0x000B16F4
	public override void Stop()
	{
		base.Stop();
		this.m_MotorThrottle = Vector3.zero;
		this.m_MotorJumpDone = true;
		this.m_MotorJumpForceAcc = 0f;
		this.m_ExternalForce = Vector3.zero;
		this.StopSoftForce();
		this.m_SmoothPosition = base.Transform.position;
		this.m_SlideFast = false;
		this.m_Slide = false;
		this.ResetDepenetrationForce();
		this.StoreGroundInfo();
	}

	// Token: 0x06002E60 RID: 11872 RVA: 0x000B3560 File Offset: 0x000B1760
	public virtual void DeflectDownForce()
	{
		if (this.GroundAngle > this.PhysicsSlopeSlideLimit)
		{
			this.m_SlopeSlideSpeed = this.m_FallImpact * (0.25f * Time.timeScale);
		}
		if (this.GroundAngle > 85f)
		{
			this.m_MotorThrottle += vp_3DUtility.HorizontalVector(this.GroundNormal * this.m_FallImpact);
			this.m_Grounded = false;
		}
	}

	// Token: 0x06002E61 RID: 11873 RVA: 0x000B35D0 File Offset: 0x000B17D0
	protected virtual void DeflectUpForce()
	{
		if (!this.m_HeadContact)
		{
			return;
		}
		this.m_NewDir = Vector3.Cross(Vector3.Cross(this.m_CeilingHit.normal, Vector3.up), this.m_CeilingHit.normal);
		this.m_ForceImpact = this.m_MotorThrottle.y + this.m_ExternalForce.y;
		Vector3 a = this.m_NewDir * (this.m_MotorThrottle.y + this.m_ExternalForce.y) * (1f - this.PhysicsWallFriction);
		this.m_ForceImpact -= a.magnitude;
		this.AddForce(a * Time.timeScale);
		this.m_MotorThrottle.y = 0f;
		this.m_ExternalForce.y = 0f;
		this.m_FallSpeed = 0f;
		this.m_NewDir.x = base.Transform.InverseTransformDirection(this.m_NewDir).x;
		base.Player.HeadImpact.Send((this.m_NewDir.x < 0f || (this.m_NewDir.x == 0f && UnityEngine.Random.value < 0.5f)) ? (-this.m_ForceImpact) : this.m_ForceImpact);
	}

	// Token: 0x06002E62 RID: 11874 RVA: 0x000B3728 File Offset: 0x000B1928
	protected virtual void DeflectHorizontalForce()
	{
		this.m_PredictedPos.y = base.Transform.position.y;
		this.m_PrevPosition.y = base.Transform.position.y;
		this.m_PrevDir = (this.m_PredictedPos - this.m_PrevPosition).normalized;
		this.CapsuleBottom = this.m_PrevPosition + Vector3.up * base.Player.Radius.Get();
		this.CapsuleTop = this.CapsuleBottom + Vector3.up * (base.Player.Height.Get() - base.Player.Radius.Get() * 2f);
		if (!Physics.CapsuleCast(this.CapsuleBottom, this.CapsuleTop, base.Player.Radius.Get(), this.m_PrevDir, out this.m_WallHit, Vector3.Distance(this.m_PrevPosition, this.m_PredictedPos), -675375893))
		{
			return;
		}
		this.m_NewDir = Vector3.Cross(this.m_WallHit.normal, Vector3.up).normalized;
		if (Vector3.Dot(Vector3.Cross(this.m_WallHit.point - base.Transform.position, this.m_PrevPosition - base.Transform.position), Vector3.up) > 0f)
		{
			this.m_NewDir = -this.m_NewDir;
		}
		this.m_ForceMultiplier = Mathf.Abs(Vector3.Dot(this.m_PrevDir, this.m_NewDir)) * (1f - this.PhysicsWallFriction);
		if (this.PhysicsWallBounce > 0f)
		{
			this.m_NewDir = Vector3.Lerp(this.m_NewDir, Vector3.Reflect(this.m_PrevDir, this.m_WallHit.normal), this.PhysicsWallBounce);
			this.m_ForceMultiplier = Mathf.Lerp(this.m_ForceMultiplier, 1f, this.PhysicsWallBounce * (1f - this.PhysicsWallFriction));
		}
		this.m_ForceImpact = 0f;
		float y = this.m_ExternalForce.y;
		this.m_ExternalForce.y = 0f;
		this.m_ForceImpact = this.m_ExternalForce.magnitude;
		this.m_ExternalForce = this.m_NewDir * this.m_ExternalForce.magnitude * this.m_ForceMultiplier;
		this.m_ForceImpact -= this.m_ExternalForce.magnitude;
		int num = 0;
		while (num < 120 && !(this.m_SmoothForceFrame[num] == Vector3.zero))
		{
			this.m_SmoothForceFrame[num] = this.m_SmoothForceFrame[num].magnitude * this.m_NewDir * this.m_ForceMultiplier;
			num++;
		}
		this.m_ExternalForce.y = y;
	}

	// Token: 0x06002E63 RID: 11875 RVA: 0x000B3A34 File Offset: 0x000B1C34
	public float CalculateMaxSpeed(string stateName = "Default", float accelDuration = 5f)
	{
		if (stateName != "Default")
		{
			bool flag = false;
			using (List<vp_State>.Enumerator enumerator = this.States.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Name == stateName)
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Error (",
					this,
					") Controller has no such state: '",
					stateName,
					"'."
				}));
				return 0f;
			}
		}
		Dictionary<vp_State, bool> dictionary = new Dictionary<vp_State, bool>();
		foreach (vp_State vp_State in this.States)
		{
			dictionary.Add(vp_State, vp_State.Enabled);
			vp_State.Enabled = false;
		}
		base.StateManager.Reset();
		if (stateName != "Default")
		{
			base.SetState(stateName, true, false, false);
		}
		float num = 0f;
		float num2 = 5f;
		int num3 = 0;
		while ((float)num3 < 60f * num2)
		{
			num += this.MotorAcceleration * 0.1f * 60f;
			num /= 1f + this.MotorDamping;
			num3++;
		}
		foreach (vp_State vp_State2 in this.States)
		{
			bool enabled;
			dictionary.TryGetValue(vp_State2, out enabled);
			vp_State2.Enabled = enabled;
		}
		return num;
	}

	// Token: 0x06002E64 RID: 11876 RVA: 0x000B3BEC File Offset: 0x000B1DEC
	protected virtual void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.gameObject.isStatic)
		{
			return;
		}
		if (hit.gameObject.layer == 29)
		{
			return;
		}
		Rigidbody attachedRigidbody = hit.collider.attachedRigidbody;
		if (attachedRigidbody == null)
		{
			return;
		}
		if (vp_Gameplay.isMaster && attachedRigidbody.isKinematic)
		{
			return;
		}
		if (Time.time < this.m_NextAllowedPushTime)
		{
			return;
		}
		this.m_NextAllowedPushTime = Time.time + this.PhysicsPushInterval;
		if (!vp_Gameplay.isMultiplayer)
		{
			base.PushRigidbody(attachedRigidbody, hit.moveDirection, hit.point);
		}
	}

	// Token: 0x06002E65 RID: 11877 RVA: 0x000B3C78 File Offset: 0x000B1E78
	protected virtual bool CanStart_Jump()
	{
		return Time.timeScale != 0f && (this.MotorFreeFly || (this.m_GroundedNonMountain && this.m_MotorJumpDone && this.GroundAngle <= base.Player.SlopeLimit.Get()));
	}

	// Token: 0x06002E66 RID: 11878 RVA: 0x000B3CD1 File Offset: 0x000B1ED1
	protected virtual bool CanStart_Run()
	{
		return !base.Player.Crouch.Active;
	}

	// Token: 0x06002E67 RID: 11879 RVA: 0x000B3CE8 File Offset: 0x000B1EE8
	protected virtual void OnStart_Jump()
	{
		this.m_MotorJumpDone = false;
		if (this.MotorFreeFly && !base.Grounded)
		{
			return;
		}
		this.m_MotorThrottle.y = this.MotorJumpForce / Time.timeScale;
		this.m_SmoothPosition.y = base.Transform.position.y;
	}

	// Token: 0x06002E68 RID: 11880 RVA: 0x000B3D3F File Offset: 0x000B1F3F
	protected virtual void OnStop_Jump()
	{
		this.m_MotorJumpDone = true;
	}

	// Token: 0x06002E69 RID: 11881 RVA: 0x000B3D48 File Offset: 0x000B1F48
	protected virtual bool CanStop_Crouch()
	{
		if (Physics.SphereCast(new Ray(base.Transform.position, Vector3.up), base.Player.Radius.Get(), this.m_NormalHeight - base.Player.Radius.Get() + 0.01f, -675375893))
		{
			base.Player.Crouch.NextAllowedStopTime = Time.time + 1f;
			return false;
		}
		return true;
	}

	// Token: 0x06002E6A RID: 11882 RVA: 0x000B3DCB File Offset: 0x000B1FCB
	protected virtual void OnMessage_ForceImpact(Vector3 force)
	{
		this.AddForce(force);
	}

	// Token: 0x06002E6B RID: 11883 RVA: 0x000B3DD4 File Offset: 0x000B1FD4
	protected virtual Vector3 Get_MotorThrottle()
	{
		return this.m_MotorThrottle;
	}

	// Token: 0x06002E6C RID: 11884 RVA: 0x000B3DDC File Offset: 0x000B1FDC
	protected virtual void Set_MotorThrottle(Vector3 value)
	{
		this.m_MotorThrottle = value;
	}

	// Token: 0x170002FF RID: 767
	// (get) Token: 0x06002E6D RID: 11885 RVA: 0x000B3DD4 File Offset: 0x000B1FD4
	// (set) Token: 0x06002E6E RID: 11886 RVA: 0x000B3DDC File Offset: 0x000B1FDC
	protected virtual Vector3 OnValue_MotorThrottle
	{
		get
		{
			return this.m_MotorThrottle;
		}
		set
		{
			this.m_MotorThrottle = value;
		}
	}

	// Token: 0x06002E6F RID: 11887 RVA: 0x000B3DE5 File Offset: 0x000B1FE5
	protected virtual bool Get_MotorJumpDone()
	{
		return this.m_MotorJumpDone;
	}

	// Token: 0x17000300 RID: 768
	// (get) Token: 0x06002E70 RID: 11888 RVA: 0x000B3DE5 File Offset: 0x000B1FE5
	protected virtual bool OnValue_MotorJumpDone
	{
		get
		{
			return this.m_MotorJumpDone;
		}
	}

	// Token: 0x06002E71 RID: 11889 RVA: 0x000B3DF0 File Offset: 0x000B1FF0
	protected virtual Texture Get_GroundTexture()
	{
		if (this.GroundTransform == null)
		{
			return null;
		}
		if (this.GroundTransform.GetComponent<Renderer>() == null && this.m_CurrentTerrain == null)
		{
			return null;
		}
		int num = -1;
		if (this.m_CurrentTerrain != null)
		{
			num = vp_FootstepManager.GetMainTerrainTexture(base.Player.Position.Get(), this.m_CurrentTerrain);
			if (num > this.m_CurrentTerrain.terrainData.terrainLayers.Length - 1)
			{
				return null;
			}
		}
		if (!(this.m_CurrentTerrain == null))
		{
			return this.m_CurrentTerrain.terrainData.terrainLayers[num].diffuseTexture;
		}
		return this.GroundTransform.GetComponent<Renderer>().material.mainTexture;
	}

	// Token: 0x17000301 RID: 769
	// (get) Token: 0x06002E72 RID: 11890 RVA: 0x000B3EB8 File Offset: 0x000B20B8
	protected virtual Texture OnValue_GroundTexture
	{
		get
		{
			if (this.GroundTransform == null)
			{
				return null;
			}
			if (this.GroundTransform.GetComponent<Renderer>() == null && this.m_CurrentTerrain == null)
			{
				return null;
			}
			int num = -1;
			if (this.m_CurrentTerrain != null)
			{
				num = vp_FootstepManager.GetMainTerrainTexture(base.Player.Position.Get(), this.m_CurrentTerrain);
				if (num > this.m_CurrentTerrain.terrainData.terrainLayers.Length - 1)
				{
					return null;
				}
			}
			if (!(this.m_CurrentTerrain == null))
			{
				return this.m_CurrentTerrain.terrainData.terrainLayers[num].diffuseTexture;
			}
			return this.GroundTransform.GetComponent<Renderer>().material.mainTexture;
		}
	}

	// Token: 0x06002E73 RID: 11891 RVA: 0x000B3F7D File Offset: 0x000B217D
	protected virtual vp_SurfaceIdentifier Get_SurfaceType()
	{
		return this.m_CurrentSurface;
	}

	// Token: 0x17000302 RID: 770
	// (get) Token: 0x06002E74 RID: 11892 RVA: 0x000B3F7D File Offset: 0x000B217D
	protected virtual vp_SurfaceIdentifier OnValue_SurfaceType
	{
		get
		{
			return this.m_CurrentSurface;
		}
	}

	// Token: 0x06002E75 RID: 11893 RVA: 0x000B3F85 File Offset: 0x000B2185
	protected virtual bool Get_IsFirstPerson()
	{
		return this.m_IsFirstPerson;
	}

	// Token: 0x06002E76 RID: 11894 RVA: 0x000B3F8D File Offset: 0x000B218D
	protected virtual void Set_IsFirstPerson(bool value)
	{
		this.m_IsFirstPerson = value;
	}

	// Token: 0x17000303 RID: 771
	// (get) Token: 0x06002E77 RID: 11895 RVA: 0x000B3F85 File Offset: 0x000B2185
	// (set) Token: 0x06002E78 RID: 11896 RVA: 0x000B3F8D File Offset: 0x000B218D
	protected virtual bool OnValue_IsFirstPerson
	{
		get
		{
			return this.m_IsFirstPerson;
		}
		set
		{
			this.m_IsFirstPerson = value;
		}
	}

	// Token: 0x06002E79 RID: 11897 RVA: 0x000B3F96 File Offset: 0x000B2196
	protected virtual void OnStart_Dead()
	{
		this.m_Platform = null;
	}

	// Token: 0x06002E7A RID: 11898 RVA: 0x000B3F9F File Offset: 0x000B219F
	protected virtual void OnStop_Dead()
	{
		base.Player.OutOfControl.Stop(0f);
	}

	// Token: 0x06002E7B RID: 11899 RVA: 0x000B3FB8 File Offset: 0x000B21B8
	public override void Register(vp_EventHandler eventHandler)
	{
		base.Register(eventHandler);
		eventHandler.RegisterActivity("Jump", new vp_Activity.Callback(this.OnStart_Jump), new vp_Activity.Callback(this.OnStop_Jump), new vp_Activity.Condition(this.CanStart_Jump), null, null, null);
		eventHandler.RegisterActivity("Run", null, null, new vp_Activity.Condition(this.CanStart_Run), null, null, null);
		eventHandler.RegisterActivity("Crouch", null, null, null, new vp_Activity.Condition(this.CanStop_Crouch), null, null);
		eventHandler.RegisterActivity("Dead", new vp_Activity.Callback(this.OnStart_Dead), new vp_Activity.Callback(this.OnStop_Dead), null, null, null, null);
		eventHandler.RegisterValue<Texture>("GroundTexture", new vp_Value<Texture>.Getter<Texture>(this.Get_GroundTexture), null);
		eventHandler.RegisterValue<bool>("IsFirstPerson", new vp_Value<bool>.Getter<bool>(this.Get_IsFirstPerson), new vp_Value<bool>.Setter<bool>(this.Set_IsFirstPerson));
		eventHandler.RegisterValue<bool>("MotorJumpDone", new vp_Value<bool>.Getter<bool>(this.Get_MotorJumpDone), null);
		eventHandler.RegisterValue<Vector3>("MotorThrottle", new vp_Value<Vector3>.Getter<Vector3>(this.Get_MotorThrottle), new vp_Value<Vector3>.Setter<Vector3>(this.Set_MotorThrottle));
		eventHandler.RegisterValue<Vector3>("Position", new vp_Value<Vector3>.Getter<Vector3>(this.Get_Position), new vp_Value<Vector3>.Setter<Vector3>(this.Set_Position));
		eventHandler.RegisterValue<vp_SurfaceIdentifier>("SurfaceType", new vp_Value<vp_SurfaceIdentifier>.Getter<vp_SurfaceIdentifier>(this.Get_SurfaceType), null);
	}

	// Token: 0x06002E7C RID: 11900 RVA: 0x000B4120 File Offset: 0x000B2320
	public override void Unregister(vp_EventHandler eventHandler)
	{
		base.Unregister(eventHandler);
		eventHandler.UnregisterActivity("Jump", new vp_Activity.Callback(this.OnStart_Jump), new vp_Activity.Callback(this.OnStop_Jump), new vp_Activity.Condition(this.CanStart_Jump), null, null, null);
		eventHandler.UnregisterActivity("Run", null, null, new vp_Activity.Condition(this.CanStart_Run), null, null, null);
		eventHandler.UnregisterActivity("Crouch", null, null, null, new vp_Activity.Condition(this.CanStop_Crouch), null, null);
		eventHandler.UnregisterActivity("Dead", new vp_Activity.Callback(this.OnStart_Dead), new vp_Activity.Callback(this.OnStop_Dead), null, null, null, null);
		eventHandler.UnregisterValue<Texture>("GroundTexture", new vp_Value<Texture>.Getter<Texture>(this.Get_GroundTexture), null);
		eventHandler.UnregisterValue<bool>("IsFirstPerson", new vp_Value<bool>.Getter<bool>(this.Get_IsFirstPerson), new vp_Value<bool>.Setter<bool>(this.Set_IsFirstPerson));
		eventHandler.UnregisterValue<bool>("MotorJumpDone", new vp_Value<bool>.Getter<bool>(this.Get_MotorJumpDone), null);
		eventHandler.UnregisterValue<Vector3>("MotorThrottle", new vp_Value<Vector3>.Getter<Vector3>(this.Get_MotorThrottle), new vp_Value<Vector3>.Setter<Vector3>(this.Set_MotorThrottle));
		eventHandler.UnregisterValue<Vector3>("Position", new vp_Value<Vector3>.Getter<Vector3>(this.Get_Position), new vp_Value<Vector3>.Setter<Vector3>(this.Set_Position));
		eventHandler.UnregisterValue<vp_SurfaceIdentifier>("SurfaceType", new vp_Value<vp_SurfaceIdentifier>.Getter<vp_SurfaceIdentifier>(this.Get_SurfaceType), null);
	}

	// Token: 0x06002E7D RID: 11901 RVA: 0x000B4286 File Offset: 0x000B2486
	protected override StateManager GetStateManager()
	{
		return new FPControllerStateManager(this);
	}

	// Token: 0x04002C4A RID: 11338
	public static vp_FPController.OnStartDelegate onStartDelegate;

	// Token: 0x04002C4B RID: 11339
	protected Vector3 m_FixedPosition = Vector3.zero;

	// Token: 0x04002C4C RID: 11340
	protected Vector3 m_SmoothPosition = Vector3.zero;

	// Token: 0x04002C4D RID: 11341
	protected bool m_IsFirstPerson = true;

	// Token: 0x04002C4E RID: 11342
	protected bool m_HeadContact;

	// Token: 0x04002C4F RID: 11343
	protected RaycastHit m_CeilingHit;

	// Token: 0x04002C50 RID: 11344
	protected RaycastHit m_WallHit;

	// Token: 0x04002C51 RID: 11345
	protected Terrain m_CurrentTerrain;

	// Token: 0x04002C52 RID: 11346
	protected vp_SurfaceIdentifier m_CurrentSurface;

	// Token: 0x04002C53 RID: 11347
	protected CapsuleCollider m_TriggerCollider;

	// Token: 0x04002C54 RID: 11348
	public bool PhysicsHasCollisionTrigger = true;

	// Token: 0x04002C55 RID: 11349
	protected GameObject m_Trigger;

	// Token: 0x04002C56 RID: 11350
	public float MotorAcceleration = 0.18f;

	// Token: 0x04002C57 RID: 11351
	public float MotorDamping = 0.17f;

	// Token: 0x04002C58 RID: 11352
	public float MotorBackwardsSpeed = 0.65f;

	// Token: 0x04002C59 RID: 11353
	public float MotorAirSpeed = 0.35f;

	// Token: 0x04002C5A RID: 11354
	public float MotorSlopeSpeedUp = 1f;

	// Token: 0x04002C5B RID: 11355
	public float MotorSlopeSpeedDown = 1f;

	// Token: 0x04002C5C RID: 11356
	protected Vector3 m_MoveDirection = Vector3.zero;

	// Token: 0x04002C5D RID: 11357
	protected Vector3 m_MotorThrottle = Vector3.zero;

	// Token: 0x04002C5E RID: 11358
	protected float m_MotorAirSpeedModifier = 1f;

	// Token: 0x04002C5F RID: 11359
	protected float m_CurrentAntiBumpOffset;

	// Token: 0x04002C60 RID: 11360
	public float MotorJumpForce = 0.18f;

	// Token: 0x04002C61 RID: 11361
	public float MotorJumpForceDamping = 0.08f;

	// Token: 0x04002C62 RID: 11362
	public float MotorJumpForceHold = 0.003f;

	// Token: 0x04002C63 RID: 11363
	public float MotorJumpForceHoldDamping = 0.5f;

	// Token: 0x04002C64 RID: 11364
	protected int m_MotorJumpForceHoldSkipFrames;

	// Token: 0x04002C65 RID: 11365
	protected float m_MotorJumpForceAcc;

	// Token: 0x04002C66 RID: 11366
	protected bool m_MotorJumpDone = true;

	// Token: 0x04002C67 RID: 11367
	public float PhysicsForceDamping = 0.05f;

	// Token: 0x04002C68 RID: 11368
	public float PhysicsSlopeSlideLimit = 30f;

	// Token: 0x04002C69 RID: 11369
	public float PhysicsSlopeSlidiness = 0.15f;

	// Token: 0x04002C6A RID: 11370
	public float PhysicsWallBounce;

	// Token: 0x04002C6B RID: 11371
	public float PhysicsWallFriction;

	// Token: 0x04002C6C RID: 11372
	protected Vector3 m_ExternalForce = Vector3.zero;

	// Token: 0x04002C6D RID: 11373
	protected Vector3[] m_SmoothForceFrame = new Vector3[120];

	// Token: 0x04002C6E RID: 11374
	protected bool m_Slide;

	// Token: 0x04002C6F RID: 11375
	protected bool m_SlideFast;

	// Token: 0x04002C70 RID: 11376
	protected float m_SlideFallSpeed;

	// Token: 0x04002C71 RID: 11377
	protected float m_OnSteepGroundSince;

	// Token: 0x04002C72 RID: 11378
	protected float m_SlopeSlideSpeed;

	// Token: 0x04002C73 RID: 11379
	protected Vector3 m_PredictedPos = Vector3.zero;

	// Token: 0x04002C74 RID: 11380
	protected Vector3 m_PrevDir = Vector3.zero;

	// Token: 0x04002C75 RID: 11381
	protected Vector3 m_NewDir = Vector3.zero;

	// Token: 0x04002C76 RID: 11382
	protected float m_ForceImpact;

	// Token: 0x04002C77 RID: 11383
	protected float m_ForceMultiplier;

	// Token: 0x04002C78 RID: 11384
	protected Vector3 CapsuleBottom = Vector3.zero;

	// Token: 0x04002C79 RID: 11385
	protected Vector3 CapsuleTop = Vector3.zero;

	// Token: 0x04002C7A RID: 11386
	private Vector3 m_DepenetrationForce = Vector3.zero;

	// Token: 0x04002C7B RID: 11387
	private const float FREE_SPRINT_FACTOR = 3f;

	// Token: 0x0200087E RID: 2174
	// (Invoke) Token: 0x06002E80 RID: 11904
	public delegate void OnStartDelegate(vp_FPController controller);
}
