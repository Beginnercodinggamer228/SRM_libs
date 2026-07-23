using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000889 RID: 2185
[RequireComponent(typeof(Animator))]
public class vp_BodyAnimator : MonoBehaviour, EventHandlerRegistrable
{
	// Token: 0x17000320 RID: 800
	// (get) Token: 0x06002F2C RID: 12076 RVA: 0x000B87B4 File Offset: 0x000B69B4
	protected vp_WeaponHandler WeaponHandler
	{
		get
		{
			if (this.m_WeaponHandler == null)
			{
				this.m_WeaponHandler = (vp_WeaponHandler)base.transform.root.GetComponentInChildren(typeof(vp_WeaponHandler));
			}
			return this.m_WeaponHandler;
		}
	}

	// Token: 0x17000321 RID: 801
	// (get) Token: 0x06002F2D RID: 12077 RVA: 0x000B87EF File Offset: 0x000B69EF
	protected Transform Transform
	{
		get
		{
			if (this.m_Transform == null)
			{
				this.m_Transform = base.transform;
			}
			return this.m_Transform;
		}
	}

	// Token: 0x17000322 RID: 802
	// (get) Token: 0x06002F2E RID: 12078 RVA: 0x000B8811 File Offset: 0x000B6A11
	protected vp_PlayerEventHandler Player
	{
		get
		{
			if (this.m_Player == null)
			{
				this.m_Player = (vp_PlayerEventHandler)base.transform.root.GetComponentInChildren(typeof(vp_PlayerEventHandler));
			}
			return this.m_Player;
		}
	}

	// Token: 0x17000323 RID: 803
	// (get) Token: 0x06002F2F RID: 12079 RVA: 0x000B884C File Offset: 0x000B6A4C
	protected SkinnedMeshRenderer Renderer
	{
		get
		{
			if (this.m_Renderer == null)
			{
				this.m_Renderer = base.transform.root.GetComponentInChildren<SkinnedMeshRenderer>();
			}
			return this.m_Renderer;
		}
	}

	// Token: 0x17000324 RID: 804
	// (get) Token: 0x06002F30 RID: 12080 RVA: 0x000B8878 File Offset: 0x000B6A78
	protected Animator Animator
	{
		get
		{
			if (this.m_Animator == null)
			{
				this.m_Animator = base.GetComponent<Animator>();
			}
			return this.m_Animator;
		}
	}

	// Token: 0x17000325 RID: 805
	// (get) Token: 0x06002F31 RID: 12081 RVA: 0x000B889A File Offset: 0x000B6A9A
	protected Vector3 m_LocalVelocity
	{
		get
		{
			return vp_MathUtility.SnapToZero(this.Transform.root.InverseTransformDirection(this.Player.Velocity.Get()) / this.m_MaxSpeed, 0.0001f);
		}
	}

	// Token: 0x17000326 RID: 806
	// (get) Token: 0x06002F32 RID: 12082 RVA: 0x000B88D6 File Offset: 0x000B6AD6
	protected float m_MaxSpeed
	{
		get
		{
			if (this.Player.Run.Active)
			{
				return this.m_MaxRunSpeed;
			}
			if (this.Player.Crouch.Active)
			{
				return this.m_MaxCrouchSpeed;
			}
			return this.m_MaxWalkSpeed;
		}
	}

	// Token: 0x17000327 RID: 807
	// (get) Token: 0x06002F33 RID: 12083 RVA: 0x000B8910 File Offset: 0x000B6B10
	protected GameObject HeadPoint
	{
		get
		{
			if (this.m_HeadPoint == null)
			{
				this.m_HeadPoint = new GameObject("HeadPoint");
				this.m_HeadPoint.transform.parent = this.m_HeadLookBones[0].transform;
				this.m_HeadPoint.transform.localPosition = Vector3.zero;
				this.HeadPoint.transform.eulerAngles = this.Player.Rotation.Get();
			}
			return this.m_HeadPoint;
		}
	}

	// Token: 0x17000328 RID: 808
	// (get) Token: 0x06002F34 RID: 12084 RVA: 0x000B89A1 File Offset: 0x000B6BA1
	protected GameObject DebugLookTarget
	{
		get
		{
			if (this.m_DebugLookTarget == null)
			{
				this.m_DebugLookTarget = vp_3DUtility.DebugBall(null);
			}
			return this.m_DebugLookTarget;
		}
	}

	// Token: 0x17000329 RID: 809
	// (get) Token: 0x06002F35 RID: 12085 RVA: 0x000B89C4 File Offset: 0x000B6BC4
	protected GameObject DebugLookArrow
	{
		get
		{
			if (this.m_DebugLookArrow == null)
			{
				this.m_DebugLookArrow = vp_3DUtility.DebugPointer(null);
				this.m_DebugLookArrow.transform.parent = this.HeadPoint.transform;
				this.m_DebugLookArrow.transform.localPosition = Vector3.zero;
				this.m_DebugLookArrow.transform.localRotation = Quaternion.identity;
				return this.m_DebugLookArrow;
			}
			return this.m_DebugLookArrow;
		}
	}

	// Token: 0x06002F36 RID: 12086 RVA: 0x000B8A3D File Offset: 0x000B6C3D
	protected virtual void OnEnable()
	{
		if (this.Player != null)
		{
			this.Register(this.Player);
		}
	}

	// Token: 0x06002F37 RID: 12087 RVA: 0x000B8A59 File Offset: 0x000B6C59
	protected virtual void OnDisable()
	{
		if (this.Player != null)
		{
			this.Unregister(this.Player);
		}
	}

	// Token: 0x06002F38 RID: 12088 RVA: 0x000B8A75 File Offset: 0x000B6C75
	protected virtual void Awake()
	{
		if (!this.IsValidSetup())
		{
			return;
		}
		this.InitHashIDs();
		this.InitHeadLook();
		this.InitMaxSpeeds();
	}

	// Token: 0x06002F39 RID: 12089 RVA: 0x000B8A94 File Offset: 0x000B6C94
	protected virtual void LateUpdate()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		if (!this.m_IsValid)
		{
			base.enabled = false;
			return;
		}
		this.UpdatePosition();
		this.UpdateGrounding();
		this.UpdateBody();
		this.UpdateSpine();
		this.UpdateAnimationSpeeds();
		this.UpdateAnimator();
		this.UpdateDebugInfo();
		this.UpdateHeadPoint();
	}

	// Token: 0x06002F3A RID: 12090 RVA: 0x000B8AF0 File Offset: 0x000B6CF0
	protected virtual void UpdateAnimationSpeeds()
	{
		if (Time.time > this.m_NextAllowedUpdateTurnTargetTime)
		{
			this.m_CurrentTurnTarget = Mathf.DeltaAngle(this.m_PrevBodyYaw, this.m_BodyYaw) * (this.Player.Crouch.Active ? 100f : 0.2f);
			this.m_NextAllowedUpdateTurnTargetTime = Time.time + 0.1f;
		}
		if (this.Player.Platform.Get() == null || !this.Player.IsLocal.Get())
		{
			this.m_CurrentTurn = Mathf.Lerp(this.m_CurrentTurn, this.m_CurrentTurnTarget, Time.deltaTime);
			if (Mathf.Round(this.Transform.root.eulerAngles.y) == Mathf.Round(this.m_LastYaw))
			{
				this.m_CurrentTurn *= 0.6f;
			}
			this.m_LastYaw = this.Transform.root.eulerAngles.y;
			this.m_CurrentTurn = vp_MathUtility.SnapToZero(this.m_CurrentTurn, 0.0001f);
		}
		else
		{
			this.m_CurrentTurn = 0f;
		}
		this.m_CurrentForward = Mathf.Lerp(this.m_CurrentForward, this.m_LocalVelocity.z, Time.deltaTime * 100f);
		this.m_CurrentForward = ((Mathf.Abs(this.m_CurrentForward) > 0.03f) ? this.m_CurrentForward : 0f);
		if (this.Player.Crouch.Active)
		{
			if (Mathf.Abs(this.GetStrafeDirection()) < Mathf.Abs(this.m_CurrentTurn))
			{
				this.m_CurrentStrafe = Mathf.Lerp(this.m_CurrentStrafe, this.m_CurrentTurn, Time.deltaTime * 5f);
			}
			else
			{
				this.m_CurrentStrafe = Mathf.Lerp(this.m_CurrentStrafe, this.GetStrafeDirection(), Time.deltaTime * 5f);
			}
		}
		else
		{
			this.m_CurrentStrafe = Mathf.Lerp(this.m_CurrentStrafe, this.GetStrafeDirection(), Time.deltaTime * 5f);
		}
		this.m_CurrentStrafe = ((Mathf.Abs(this.m_CurrentStrafe) > 0.03f) ? this.m_CurrentStrafe : 0f);
	}

	// Token: 0x06002F3B RID: 12091 RVA: 0x000B8D20 File Offset: 0x000B6F20
	protected virtual float GetStrafeDirection()
	{
		if (this.Player.InputMoveVector.Get().x < 0f)
		{
			return -1f;
		}
		if (this.Player.InputMoveVector.Get().x > 0f)
		{
			return 1f;
		}
		return 0f;
	}

	// Token: 0x06002F3C RID: 12092 RVA: 0x000B8D80 File Offset: 0x000B6F80
	protected virtual void UpdateAnimator()
	{
		this.Animator.SetBool(this.IsRunning, this.Player.Run.Active && this.GetIsMoving());
		this.Animator.SetBool(this.IsCrouching, this.Player.Crouch.Active);
		this.Animator.SetInteger(this.WeaponTypeIndex, this.Player.CurrentWeaponType.Get());
		this.Animator.SetInteger(this.WeaponGripIndex, this.Player.CurrentWeaponGrip.Get());
		this.Animator.SetBool(this.IsSettingWeapon, this.Player.SetWeapon.Active);
		this.Animator.SetBool(this.IsReloading, this.Player.Reload.Active);
		this.Animator.SetBool(this.IsOutOfControl, this.Player.OutOfControl.Active);
		this.Animator.SetBool(this.IsClimbing, this.Player.Climb.Active);
		this.Animator.SetBool(this.IsZooming, this.Player.Zoom.Active);
		this.Animator.SetBool(this.IsGrounded, this.m_Grounded);
		this.Animator.SetBool(this.IsMoving, this.GetIsMoving());
		this.Animator.SetBool(this.IsFirstPerson, this.Player.IsFirstPerson.Get());
		this.Animator.SetFloat(this.TurnAmount, this.m_CurrentTurn);
		this.Animator.SetFloat(this.ForwardAmount, this.m_CurrentForward);
		this.Animator.SetFloat(this.StrafeAmount, this.m_CurrentStrafe);
		this.Animator.SetFloat(this.PitchAmount, -this.Player.Rotation.Get().x / 90f);
		if (this.m_Grounded)
		{
			this.Animator.SetFloat(this.VerticalMoveAmount, 0f);
			return;
		}
		if (this.Player.Velocity.Get().y < 0f)
		{
			this.Animator.SetFloat(this.VerticalMoveAmount, Mathf.Lerp(this.Animator.GetFloat(this.VerticalMoveAmount), -1f, Time.deltaTime * 3f));
			return;
		}
		this.Animator.SetFloat(this.VerticalMoveAmount, this.Player.MotorThrottle.Get().y * 10f);
	}

	// Token: 0x06002F3D RID: 12093 RVA: 0x000B9040 File Offset: 0x000B7240
	protected virtual void UpdateDebugInfo()
	{
		if (this.ShowDebugObjects)
		{
			this.DebugLookTarget.transform.position = this.m_HeadLookBones[0].transform.position + this.HeadPoint.transform.forward * 1000f;
			this.DebugLookArrow.transform.LookAt(this.DebugLookTarget.transform.position);
			if (!vp_Utility.IsActive(this.m_DebugLookTarget))
			{
				vp_Utility.Activate(this.m_DebugLookTarget, true);
			}
			if (!vp_Utility.IsActive(this.m_DebugLookArrow))
			{
				vp_Utility.Activate(this.m_DebugLookArrow, true);
				return;
			}
		}
		else
		{
			if (this.m_DebugLookTarget != null)
			{
				vp_Utility.Activate(this.m_DebugLookTarget, false);
			}
			if (this.m_DebugLookArrow != null)
			{
				vp_Utility.Activate(this.m_DebugLookArrow, false);
			}
		}
	}

	// Token: 0x06002F3E RID: 12094 RVA: 0x000B9124 File Offset: 0x000B7324
	protected virtual void UpdateHeadPoint()
	{
		if (!this.HeadPointDirty)
		{
			return;
		}
		this.HeadPoint.transform.eulerAngles = this.Player.Rotation.Get();
		this.HeadPointDirty = false;
	}

	// Token: 0x06002F3F RID: 12095 RVA: 0x000B9160 File Offset: 0x000B7360
	protected virtual void UpdatePosition()
	{
		if (this.Player.IsFirstPerson.Get())
		{
			return;
		}
		if (this.Player.Climb.Active)
		{
			this.Transform.localPosition += this.ClimbOffset;
		}
	}

	// Token: 0x06002F40 RID: 12096 RVA: 0x000B91B4 File Offset: 0x000B73B4
	protected virtual void UpdateBody()
	{
		this.m_PrevBodyYaw = this.m_BodyYaw;
		this.m_BodyYaw = Mathf.LerpAngle(this.m_BodyYaw, this.m_CurrentBodyYawTarget, Time.deltaTime * ((this.Player.Velocity.Get().magnitude > 0.1f) ? this.FeetAdjustSpeedMoving : this.FeetAdjustSpeedStanding));
		this.m_BodyYaw = ((this.m_BodyYaw < -360f) ? (this.m_BodyYaw += 360f) : this.m_BodyYaw);
		this.m_BodyYaw = ((this.m_BodyYaw > 360f) ? (this.m_BodyYaw -= 360f) : this.m_BodyYaw);
		this.Transform.eulerAngles = this.m_BodyYaw * Vector3.up;
		this.m_CurrentHeadLookYaw = Mathf.DeltaAngle(this.Player.Rotation.Get().y, this.Transform.eulerAngles.y);
		if (Mathf.Max(0f, this.m_CurrentHeadLookYaw - 90f) > 0f)
		{
			this.Transform.eulerAngles = Vector3.up * (this.Transform.root.eulerAngles.y + 90f);
			this.m_BodyYaw = (this.m_CurrentBodyYawTarget = this.Transform.eulerAngles.y);
		}
		else if (Mathf.Min(0f, this.m_CurrentHeadLookYaw - -90f) < 0f)
		{
			this.Transform.eulerAngles = Vector3.up * (this.Transform.root.eulerAngles.y - 90f);
			this.m_BodyYaw = (this.m_CurrentBodyYawTarget = this.Transform.eulerAngles.y);
		}
		if (Mathf.Abs(this.Player.Rotation.Get().y - this.m_BodyYaw) > 180f)
		{
			if (this.m_BodyYaw > 0f)
			{
				this.m_BodyYaw -= 360f;
				this.m_PrevBodyYaw -= 360f;
			}
			else if (this.m_BodyYaw < 0f)
			{
				this.m_BodyYaw += 360f;
				this.m_PrevBodyYaw += 360f;
			}
		}
		if (this.m_CurrentHeadLookYaw > this.FeetAdjustAngle || this.m_CurrentHeadLookYaw < -this.FeetAdjustAngle || this.Player.Velocity.Get().magnitude > 0.1f || (this.Player.Crouch.Active && (this.Player.Attack.Active || this.Player.Zoom.Active)))
		{
			this.m_CurrentBodyYawTarget = Mathf.LerpAngle(this.m_CurrentBodyYawTarget, this.Transform.root.eulerAngles.y, 0.1f);
		}
	}

	// Token: 0x06002F41 RID: 12097 RVA: 0x000B94DC File Offset: 0x000B76DC
	protected virtual void UpdateSpine()
	{
		if (this.Player.Climb.Active)
		{
			return;
		}
		for (int i = 0; i < this.m_HeadLookBones.Count; i++)
		{
			if ((this.Player.IsFirstPerson.Get() || this.Animator.GetBool(this.IsAttacking) || this.Animator.GetBool(this.IsZooming)) && !this.Animator.GetBool(this.IsCrouching))
			{
				this.m_HeadLookTargetFalloffs[i] = this.m_HeadLookFalloffs[this.m_HeadLookFalloffs.Count - 1 - i];
			}
			else
			{
				this.m_HeadLookTargetFalloffs[i] = this.m_HeadLookFalloffs[i];
			}
			if (this.m_WasMoving && !this.Animator.GetBool(this.IsMoving))
			{
				this.m_HeadLookCurrentFalloffs[i] = this.m_HeadLookTargetFalloffs[i];
			}
			this.m_HeadLookCurrentFalloffs[i] = Mathf.SmoothStep(this.m_HeadLookCurrentFalloffs[i], Mathf.LerpAngle(this.m_HeadLookCurrentFalloffs[i], this.m_HeadLookTargetFalloffs[i], Time.deltaTime * 10f), Time.deltaTime * 20f);
			if (this.Player.IsFirstPerson.Get())
			{
				this.m_HeadLookTargetWorldDir = this.GetLookPoint() - this.m_HeadLookBones[0].transform.position;
				this.m_HeadLookCurrentWorldDir = Vector3.Slerp(this.m_HeadLookTargetWorldDir, vp_3DUtility.HorizontalVector(this.m_HeadLookTargetWorldDir), this.m_HeadLookCurrentFalloffs[i] / this.m_HeadLookFalloffs[0]);
			}
			else
			{
				this.m_ValidLookPoint = this.GetLookPoint();
				this.m_ValidLookPointForward = this.Transform.InverseTransformDirection(this.m_ValidLookPoint - this.m_HeadLookBones[0].transform.position).z;
				if (this.m_ValidLookPointForward < 0f)
				{
					this.m_ValidLookPoint += this.Transform.forward * -this.m_ValidLookPointForward;
				}
				this.m_HeadLookTargetWorldDir = Vector3.Slerp(this.m_HeadLookTargetWorldDir, this.m_ValidLookPoint - this.m_HeadLookBones[0].transform.position, Time.deltaTime * this.HeadYawSpeed);
				this.m_HeadLookCurrentWorldDir = Vector3.Slerp(this.m_HeadLookCurrentWorldDir, vp_3DUtility.HorizontalVector(this.m_HeadLookTargetWorldDir), this.m_HeadLookCurrentFalloffs[i] / this.m_HeadLookFalloffs[0]);
			}
			this.m_HeadLookBones[i].transform.rotation = vp_3DUtility.GetBoneLookRotationInWorldSpace(this.m_HeadLookBones[i].transform.rotation, this.m_HeadLookBones[this.m_HeadLookBones.Count - 1].transform.parent.rotation, this.m_HeadLookCurrentWorldDir, this.m_HeadLookCurrentFalloffs[i], this.m_ReferenceLookDirs[i], this.m_ReferenceUpDirs[i], Quaternion.identity);
			if (!this.Player.IsFirstPerson.Get())
			{
				this.m_CurrentHeadLookPitch = Mathf.SmoothStep(this.m_CurrentHeadLookPitch, Mathf.Clamp(this.Player.Rotation.Get().x, -this.HeadPitchCap, this.HeadPitchCap), Time.deltaTime * this.HeadPitchSpeed);
				this.m_HeadLookBones[i].transform.Rotate(this.HeadPoint.transform.right, this.m_CurrentHeadLookPitch * Mathf.Lerp(this.m_HeadLookFalloffs[i], this.m_HeadLookCurrentFalloffs[i], this.LeaningFactor), Space.World);
			}
		}
		this.m_WasMoving = this.Animator.GetBool(this.IsMoving);
	}

	// Token: 0x06002F42 RID: 12098 RVA: 0x000B98E0 File Offset: 0x000B7AE0
	protected virtual bool GetIsMoving()
	{
		return Vector3.Scale(this.Player.Velocity.Get(), Vector3.right + Vector3.forward).magnitude > 0.01f;
	}

	// Token: 0x06002F43 RID: 12099 RVA: 0x000B9928 File Offset: 0x000B7B28
	protected virtual Vector3 GetLookPoint()
	{
		this.m_HeadLookBackup = this.HeadPoint.transform.eulerAngles;
		this.HeadPoint.transform.eulerAngles = vp_MathUtility.NaNSafeVector3(this.Player.Rotation.Get(), default(Vector3));
		this.m_LookPoint = this.HeadPoint.transform.position + this.HeadPoint.transform.forward * 1000f;
		this.HeadPoint.transform.eulerAngles = vp_MathUtility.NaNSafeVector3(this.m_HeadLookBackup, default(Vector3));
		return this.m_LookPoint;
	}

	// Token: 0x06002F44 RID: 12100 RVA: 0x000B99E4 File Offset: 0x000B7BE4
	protected virtual List<float> CalculateBoneFalloffs(List<GameObject> boneList)
	{
		List<float> list = new List<float>();
		float num = 0f;
		for (int i = boneList.Count - 1; i > -1; i--)
		{
			if (boneList[i] == null)
			{
				boneList.RemoveAt(i);
			}
			else
			{
				float num2 = Mathf.Lerp(0f, 1f, (float)(i + 1) / (float)boneList.Count);
				list.Add(num2 * num2 * num2);
				num += num2 * num2 * num2;
			}
		}
		if (boneList.Count == 0)
		{
			return list;
		}
		for (int j = 0; j < list.Count; j++)
		{
			List<float> list2 = list;
			int index = j;
			list2[index] *= 1f / num;
		}
		return list;
	}

	// Token: 0x06002F45 RID: 12101 RVA: 0x000B9A98 File Offset: 0x000B7C98
	protected virtual void StoreReferenceDirections()
	{
		for (int i = 0; i < this.m_HeadLookBones.Count; i++)
		{
			Quaternion lhs = Quaternion.Inverse(this.m_HeadLookBones[this.m_HeadLookBones.Count - 1].transform.parent.rotation);
			this.m_ReferenceLookDirs.Add(lhs * this.Transform.rotation * Vector3.forward);
			this.m_ReferenceUpDirs.Add(lhs * this.Transform.rotation * Vector3.up);
		}
	}

	// Token: 0x06002F46 RID: 12102 RVA: 0x000B9B38 File Offset: 0x000B7D38
	protected virtual void UpdateGrounding()
	{
		Physics.SphereCast(new Ray(this.Transform.position + Vector3.up * 0.5f, Vector3.down), 0.4f, out this.m_GroundHit, 1f, -675375893);
		this.m_Grounded = (this.m_GroundHit.collider != null);
	}

	// Token: 0x06002F47 RID: 12103 RVA: 0x000B9BA0 File Offset: 0x000B7DA0
	protected virtual void RefreshWeaponStates()
	{
		if (this.WeaponHandler == null)
		{
			return;
		}
		if (this.WeaponHandler.CurrentWeapon == null)
		{
			return;
		}
		this.WeaponHandler.CurrentWeapon.SetState("Attack", this.Player.Attack.Active, false, false);
		this.WeaponHandler.CurrentWeapon.SetState("Zoom", this.Player.Zoom.Active, false, false);
	}

	// Token: 0x06002F48 RID: 12104 RVA: 0x000B9C20 File Offset: 0x000B7E20
	protected virtual void InitMaxSpeeds()
	{
		if (this.Player.IsLocal.Get())
		{
			vp_FPController componentInChildren = this.Transform.root.GetComponentInChildren<vp_FPController>();
			this.m_MaxWalkSpeed = componentInChildren.CalculateMaxSpeed("Default", 5f);
			this.m_MaxRunSpeed = componentInChildren.CalculateMaxSpeed("Run", 5f);
			this.m_MaxCrouchSpeed = componentInChildren.CalculateMaxSpeed("Crouch", 5f);
			return;
		}
		this.m_MaxWalkSpeed = 3.999999f;
		this.m_MaxRunSpeed = 10.08f;
		this.m_MaxCrouchSpeed = 1.44f;
	}

	// Token: 0x06002F49 RID: 12105 RVA: 0x000B9CBC File Offset: 0x000B7EBC
	protected virtual void InitHashIDs()
	{
		this.ForwardAmount = Animator.StringToHash("Forward");
		this.PitchAmount = Animator.StringToHash("Pitch");
		this.StrafeAmount = Animator.StringToHash("Strafe");
		this.TurnAmount = Animator.StringToHash("Turn");
		this.VerticalMoveAmount = Animator.StringToHash("VerticalMove");
		this.IsAttacking = Animator.StringToHash("IsAttacking");
		this.IsClimbing = Animator.StringToHash("IsClimbing");
		this.IsCrouching = Animator.StringToHash("IsCrouching");
		this.IsGrounded = Animator.StringToHash("IsGrounded");
		this.IsMoving = Animator.StringToHash("IsMoving");
		this.IsOutOfControl = Animator.StringToHash("IsOutOfControl");
		this.IsReloading = Animator.StringToHash("IsReloading");
		this.IsRunning = Animator.StringToHash("IsRunning");
		this.IsSettingWeapon = Animator.StringToHash("IsSettingWeapon");
		this.IsZooming = Animator.StringToHash("IsZooming");
		this.IsFirstPerson = Animator.StringToHash("IsFirstPerson");
		this.StartClimb = Animator.StringToHash("StartClimb");
		this.StartOutOfControl = Animator.StringToHash("StartOutOfControl");
		this.StartReload = Animator.StringToHash("StartReload");
		this.WeaponGripIndex = Animator.StringToHash("WeaponGrip");
		this.WeaponTypeIndex = Animator.StringToHash("WeaponType");
	}

	// Token: 0x06002F4A RID: 12106 RVA: 0x000B9E1C File Offset: 0x000B801C
	protected virtual void InitHeadLook()
	{
		if (!this.m_IsValid)
		{
			return;
		}
		this.m_HeadLookBones.Clear();
		GameObject gameObject = this.HeadBone;
		while (gameObject != this.LowestSpineBone.transform.parent.gameObject)
		{
			this.m_HeadLookBones.Add(gameObject);
			gameObject = gameObject.transform.parent.gameObject;
		}
		this.m_ReferenceUpDirs = new List<Vector3>();
		this.m_ReferenceLookDirs = new List<Vector3>();
		this.m_HeadLookFalloffs = this.CalculateBoneFalloffs(this.m_HeadLookBones);
		this.m_HeadLookCurrentFalloffs = new List<float>(this.m_HeadLookFalloffs);
		this.m_HeadLookTargetFalloffs = new List<float>(this.m_HeadLookFalloffs);
		this.StoreReferenceDirections();
	}

	// Token: 0x06002F4B RID: 12107 RVA: 0x000B9ED0 File Offset: 0x000B80D0
	protected virtual bool IsValidSetup()
	{
		if (this.HeadBone == null)
		{
			Debug.LogError("Error (" + this + ") No gameobject has been assigned for 'HeadBone'.");
		}
		else if (this.LowestSpineBone == null)
		{
			Debug.LogError("Error (" + this + ") No gameobject has been assigned for 'LowestSpineBone'.");
		}
		else if (!vp_Utility.IsDescendant(this.HeadBone.transform, base.transform.root))
		{
			this.NotInSameHierarchyError(this.HeadBone);
		}
		else if (!vp_Utility.IsDescendant(this.LowestSpineBone.transform, base.transform.root))
		{
			this.NotInSameHierarchyError(this.LowestSpineBone);
		}
		else
		{
			if (vp_Utility.IsDescendant(this.HeadBone.transform, this.LowestSpineBone.transform))
			{
				return true;
			}
			Debug.LogError("Error (" + this + ") 'HeadBone' must be a child or descendant of 'LowestSpineBone'.");
		}
		this.m_IsValid = false;
		base.enabled = false;
		return false;
	}

	// Token: 0x06002F4C RID: 12108 RVA: 0x000B9FC8 File Offset: 0x000B81C8
	protected virtual void NotInSameHierarchyError(GameObject o)
	{
		Debug.LogError(string.Concat(new object[]
		{
			"Error '",
			o,
			"' can not be used as a bone for  ",
			this,
			" because it is not part of the same hierarchy."
		}));
	}

	// Token: 0x06002F4D RID: 12109 RVA: 0x000B9FFA File Offset: 0x000B81FA
	protected virtual float Get_BodyYaw()
	{
		return this.Transform.eulerAngles.y;
	}

	// Token: 0x06002F4E RID: 12110 RVA: 0x000BA00C File Offset: 0x000B820C
	protected virtual void Set_BodyYaw(float value)
	{
		this.m_BodyYaw = value;
	}

	// Token: 0x1700032A RID: 810
	// (get) Token: 0x06002F4F RID: 12111 RVA: 0x000B9FFA File Offset: 0x000B81FA
	// (set) Token: 0x06002F50 RID: 12112 RVA: 0x000BA00C File Offset: 0x000B820C
	protected virtual float OnValue_BodyYaw
	{
		get
		{
			return this.Transform.eulerAngles.y;
		}
		set
		{
			this.m_BodyYaw = value;
		}
	}

	// Token: 0x06002F51 RID: 12113 RVA: 0x000BA015 File Offset: 0x000B8215
	protected virtual void OnStart_Attack()
	{
		if (this.Player.CurrentWeaponType.Get() != 3)
		{
			this.Animator.SetBool(this.IsAttacking, true);
		}
	}

	// Token: 0x06002F52 RID: 12114 RVA: 0x000BA041 File Offset: 0x000B8241
	protected virtual void OnStop_Attack()
	{
		vp_Timer.In(0.5f, delegate()
		{
			this.Animator.SetBool(this.IsAttacking, false);
			this.RefreshWeaponStates();
		}, this.m_AttackDoneTimer);
	}

	// Token: 0x06002F53 RID: 12115 RVA: 0x000BA05F File Offset: 0x000B825F
	protected virtual void OnStop_Zoom()
	{
		vp_Timer.In(0.5f, delegate()
		{
			if (!this.Player.Attack.Active)
			{
				this.Animator.SetBool(this.IsAttacking, false);
			}
			this.RefreshWeaponStates();
		}, this.m_AttackDoneTimer);
	}

	// Token: 0x06002F54 RID: 12116 RVA: 0x000BA07D File Offset: 0x000B827D
	protected virtual void OnStart_Reload()
	{
		this.Animator.SetTrigger(this.StartReload);
	}

	// Token: 0x06002F55 RID: 12117 RVA: 0x000BA090 File Offset: 0x000B8290
	protected virtual void OnStart_OutOfControl()
	{
		this.Animator.SetTrigger(this.StartOutOfControl);
	}

	// Token: 0x06002F56 RID: 12118 RVA: 0x000BA0A3 File Offset: 0x000B82A3
	protected virtual void OnStart_Climb()
	{
		this.Animator.SetTrigger(this.StartClimb);
	}

	// Token: 0x06002F57 RID: 12119 RVA: 0x000BA0B6 File Offset: 0x000B82B6
	protected virtual void OnStart_Dead()
	{
		if (this.m_AttackDoneTimer.Active)
		{
			this.m_AttackDoneTimer.Execute();
		}
	}

	// Token: 0x06002F58 RID: 12120 RVA: 0x000BA0D0 File Offset: 0x000B82D0
	protected virtual void OnStop_Dead()
	{
		this.HeadPointDirty = true;
	}

	// Token: 0x06002F59 RID: 12121 RVA: 0x000BA0D9 File Offset: 0x000B82D9
	protected virtual void OnMessage_CameraToggle3rdPerson()
	{
		this.m_WasMoving = !this.m_WasMoving;
		this.HeadPointDirty = true;
	}

	// Token: 0x06002F5A RID: 12122 RVA: 0x000BA0F4 File Offset: 0x000B82F4
	protected virtual Vector3 Get_HeadLookDirection()
	{
		return (this.Player.LookPoint.Get() - this.HeadPoint.transform.position).normalized;
	}

	// Token: 0x1700032B RID: 811
	// (get) Token: 0x06002F5B RID: 12123 RVA: 0x000BA134 File Offset: 0x000B8334
	protected virtual Vector3 OnValue_HeadLookDirection
	{
		get
		{
			return (this.Player.LookPoint.Get() - this.HeadPoint.transform.position).normalized;
		}
	}

	// Token: 0x1700032C RID: 812
	// (get) Token: 0x06002F5C RID: 12124 RVA: 0x000BA173 File Offset: 0x000B8373
	protected virtual Vector3 OnValue_LookPoint
	{
		get
		{
			return this.GetLookPoint();
		}
	}

	// Token: 0x06002F5D RID: 12125 RVA: 0x000BA17C File Offset: 0x000B837C
	public void Register(vp_EventHandler eventHandler)
	{
		eventHandler.RegisterMessage("CameraToggle3rdPerson", new vp_Message.Sender(this.OnMessage_CameraToggle3rdPerson));
		eventHandler.RegisterActivity("Attack", new vp_Activity.Callback(this.OnStart_Attack), new vp_Activity.Callback(this.OnStop_Attack), null, null, null, null);
		eventHandler.RegisterActivity("Climb", new vp_Activity.Callback(this.OnStart_Climb), null, null, null, null, null);
		eventHandler.RegisterActivity("Dead", new vp_Activity.Callback(this.OnStart_Dead), new vp_Activity.Callback(this.OnStop_Dead), null, null, null, null);
		eventHandler.RegisterActivity("OutOfControl", new vp_Activity.Callback(this.OnStart_OutOfControl), null, null, null, null, null);
		eventHandler.RegisterActivity("Reload", new vp_Activity.Callback(this.OnStart_Reload), null, null, null, null, null);
		eventHandler.RegisterActivity("Zoom", null, new vp_Activity.Callback(this.OnStop_Zoom), null, null, null, null);
		eventHandler.RegisterValue<Vector3>("HeadLookLocation", new vp_Value<Vector3>.Getter<Vector3>(this.Get_HeadLookDirection), null);
		eventHandler.RegisterValue<Vector3>("LookPoint", new vp_Value<Vector3>.Getter<Vector3>(this.GetLookPoint), null);
		eventHandler.RegisterValue<float>("BodyYaw", new vp_Value<float>.Getter<float>(this.Get_BodyYaw), new vp_Value<float>.Setter<float>(this.Set_BodyYaw));
	}

	// Token: 0x06002F5E RID: 12126 RVA: 0x000BA2C0 File Offset: 0x000B84C0
	public void Unregister(vp_EventHandler eventHandler)
	{
		eventHandler.UnregisterMessage("CameraToggle3rdPerson", new vp_Message.Sender(this.OnMessage_CameraToggle3rdPerson));
		eventHandler.UnregisterActivity("Attack", new vp_Activity.Callback(this.OnStart_Attack), new vp_Activity.Callback(this.OnStop_Attack), null, null, null, null);
		eventHandler.UnregisterActivity("Climb", new vp_Activity.Callback(this.OnStart_Climb), null, null, null, null, null);
		eventHandler.UnregisterActivity("Dead", new vp_Activity.Callback(this.OnStart_Dead), new vp_Activity.Callback(this.OnStop_Dead), null, null, null, null);
		eventHandler.UnregisterActivity("OutOfControl", new vp_Activity.Callback(this.OnStart_OutOfControl), null, null, null, null, null);
		eventHandler.UnregisterActivity("Reload", new vp_Activity.Callback(this.OnStart_Reload), null, null, null, null, null);
		eventHandler.UnregisterActivity("Zoom", null, new vp_Activity.Callback(this.OnStop_Zoom), null, null, null, null);
		eventHandler.UnregisterValue<Vector3>("HeadLookLocation", new vp_Value<Vector3>.Getter<Vector3>(this.Get_HeadLookDirection), null);
		eventHandler.UnregisterValue<Vector3>("LookPoint", new vp_Value<Vector3>.Getter<Vector3>(this.GetLookPoint), null);
		eventHandler.UnregisterValue<float>("BodyYaw", new vp_Value<float>.Getter<float>(this.Get_BodyYaw), new vp_Value<float>.Setter<float>(this.Set_BodyYaw));
	}

	// Token: 0x04002D29 RID: 11561
	protected bool m_IsValid = true;

	// Token: 0x04002D2A RID: 11562
	protected Vector3 m_ValidLookPoint = Vector3.zero;

	// Token: 0x04002D2B RID: 11563
	protected float m_ValidLookPointForward;

	// Token: 0x04002D2C RID: 11564
	protected bool HeadPointDirty = true;

	// Token: 0x04002D2D RID: 11565
	public GameObject HeadBone;

	// Token: 0x04002D2E RID: 11566
	public GameObject LowestSpineBone;

	// Token: 0x04002D2F RID: 11567
	[Range(0f, 90f)]
	public float HeadPitchCap = 45f;

	// Token: 0x04002D30 RID: 11568
	[Range(2f, 20f)]
	public float HeadPitchSpeed = 7f;

	// Token: 0x04002D31 RID: 11569
	[Range(0.2f, 20f)]
	public float HeadYawSpeed = 2f;

	// Token: 0x04002D32 RID: 11570
	[Range(0f, 1f)]
	public float LeaningFactor = 0.25f;

	// Token: 0x04002D33 RID: 11571
	protected List<GameObject> m_HeadLookBones = new List<GameObject>();

	// Token: 0x04002D34 RID: 11572
	protected List<Vector3> m_ReferenceUpDirs;

	// Token: 0x04002D35 RID: 11573
	protected List<Vector3> m_ReferenceLookDirs;

	// Token: 0x04002D36 RID: 11574
	protected float m_CurrentHeadLookYaw;

	// Token: 0x04002D37 RID: 11575
	protected float m_CurrentHeadLookPitch;

	// Token: 0x04002D38 RID: 11576
	protected List<float> m_HeadLookFalloffs = new List<float>();

	// Token: 0x04002D39 RID: 11577
	protected List<float> m_HeadLookCurrentFalloffs;

	// Token: 0x04002D3A RID: 11578
	protected List<float> m_HeadLookTargetFalloffs;

	// Token: 0x04002D3B RID: 11579
	protected Vector3 m_HeadLookTargetWorldDir;

	// Token: 0x04002D3C RID: 11580
	protected Vector3 m_HeadLookCurrentWorldDir;

	// Token: 0x04002D3D RID: 11581
	protected Vector3 m_HeadLookBackup = Vector3.zero;

	// Token: 0x04002D3E RID: 11582
	protected Vector3 m_LookPoint = Vector3.zero;

	// Token: 0x04002D3F RID: 11583
	public float FeetAdjustAngle = 80f;

	// Token: 0x04002D40 RID: 11584
	public float FeetAdjustSpeedStanding = 10f;

	// Token: 0x04002D41 RID: 11585
	public float FeetAdjustSpeedMoving = 12f;

	// Token: 0x04002D42 RID: 11586
	protected float m_PrevBodyYaw;

	// Token: 0x04002D43 RID: 11587
	protected float m_BodyYaw;

	// Token: 0x04002D44 RID: 11588
	protected float m_CurrentBodyYawTarget;

	// Token: 0x04002D45 RID: 11589
	protected float m_LastYaw;

	// Token: 0x04002D46 RID: 11590
	public Vector3 ClimbOffset = Vector3.forward * 0.6f;

	// Token: 0x04002D47 RID: 11591
	public Vector3 ClimbRotationOffset = Vector3.zero;

	// Token: 0x04002D48 RID: 11592
	protected float m_CurrentForward;

	// Token: 0x04002D49 RID: 11593
	protected float m_CurrentStrafe;

	// Token: 0x04002D4A RID: 11594
	protected float m_CurrentTurn;

	// Token: 0x04002D4B RID: 11595
	protected float m_CurrentTurnTarget;

	// Token: 0x04002D4C RID: 11596
	protected float m_MaxWalkSpeed = 1f;

	// Token: 0x04002D4D RID: 11597
	protected float m_MaxRunSpeed = 1f;

	// Token: 0x04002D4E RID: 11598
	protected float m_MaxCrouchSpeed = 1f;

	// Token: 0x04002D4F RID: 11599
	protected bool m_WasMoving;

	// Token: 0x04002D50 RID: 11600
	protected RaycastHit m_GroundHit;

	// Token: 0x04002D51 RID: 11601
	protected bool m_Grounded = true;

	// Token: 0x04002D52 RID: 11602
	protected vp_Timer.Handle m_AttackDoneTimer = new vp_Timer.Handle();

	// Token: 0x04002D53 RID: 11603
	protected float m_NextAllowedUpdateTurnTargetTime;

	// Token: 0x04002D54 RID: 11604
	protected const float TURNMODIFIER = 0.2f;

	// Token: 0x04002D55 RID: 11605
	protected const float CROUCHTURNMODIFIER = 100f;

	// Token: 0x04002D56 RID: 11606
	protected const float MOVEMODIFIER = 100f;

	// Token: 0x04002D57 RID: 11607
	public bool ShowDebugObjects;

	// Token: 0x04002D58 RID: 11608
	protected int ForwardAmount;

	// Token: 0x04002D59 RID: 11609
	protected int PitchAmount;

	// Token: 0x04002D5A RID: 11610
	protected int StrafeAmount;

	// Token: 0x04002D5B RID: 11611
	protected int TurnAmount;

	// Token: 0x04002D5C RID: 11612
	protected int VerticalMoveAmount;

	// Token: 0x04002D5D RID: 11613
	protected int IsAttacking;

	// Token: 0x04002D5E RID: 11614
	protected int IsClimbing;

	// Token: 0x04002D5F RID: 11615
	protected int IsCrouching;

	// Token: 0x04002D60 RID: 11616
	protected int IsGrounded;

	// Token: 0x04002D61 RID: 11617
	protected int IsMoving;

	// Token: 0x04002D62 RID: 11618
	protected int IsOutOfControl;

	// Token: 0x04002D63 RID: 11619
	protected int IsReloading;

	// Token: 0x04002D64 RID: 11620
	protected int IsRunning;

	// Token: 0x04002D65 RID: 11621
	protected int IsSettingWeapon;

	// Token: 0x04002D66 RID: 11622
	protected int IsZooming;

	// Token: 0x04002D67 RID: 11623
	protected int IsFirstPerson;

	// Token: 0x04002D68 RID: 11624
	protected int StartClimb;

	// Token: 0x04002D69 RID: 11625
	protected int StartOutOfControl;

	// Token: 0x04002D6A RID: 11626
	protected int StartReload;

	// Token: 0x04002D6B RID: 11627
	protected int WeaponGripIndex;

	// Token: 0x04002D6C RID: 11628
	protected int WeaponTypeIndex;

	// Token: 0x04002D6D RID: 11629
	protected vp_WeaponHandler m_WeaponHandler;

	// Token: 0x04002D6E RID: 11630
	protected Transform m_Transform;

	// Token: 0x04002D6F RID: 11631
	protected vp_PlayerEventHandler m_Player;

	// Token: 0x04002D70 RID: 11632
	protected SkinnedMeshRenderer m_Renderer;

	// Token: 0x04002D71 RID: 11633
	protected Animator m_Animator;

	// Token: 0x04002D72 RID: 11634
	protected GameObject m_HeadPoint;

	// Token: 0x04002D73 RID: 11635
	protected GameObject m_DebugLookTarget;

	// Token: 0x04002D74 RID: 11636
	protected GameObject m_DebugLookArrow;
}
