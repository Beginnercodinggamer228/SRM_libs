using System;
using UnityEngine;

// Token: 0x0200087B RID: 2171
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(AudioListener))]
public class vp_FPCamera : vp_Component
{
	// Token: 0x170002EB RID: 747
	// (get) Token: 0x06002DED RID: 11757 RVA: 0x000B067C File Offset: 0x000AE87C
	// (set) Token: 0x06002DEE RID: 11758 RVA: 0x000B0684 File Offset: 0x000AE884
	public float ZoomOffset
	{
		get
		{
			return this.m_ZoomOffset;
		}
		set
		{
			this.m_ZoomOffset = value;
		}
	}

	// Token: 0x170002EC RID: 748
	// (get) Token: 0x06002DEF RID: 11759 RVA: 0x000B068D File Offset: 0x000AE88D
	// (set) Token: 0x06002DF0 RID: 11760 RVA: 0x000B0695 File Offset: 0x000AE895
	public bool DrawCameraCollisionDebugLine
	{
		get
		{
			return this.m_DrawCameraCollisionDebugLine;
		}
		set
		{
			this.m_DrawCameraCollisionDebugLine = value;
		}
	}

	// Token: 0x170002ED RID: 749
	// (get) Token: 0x06002DF1 RID: 11761 RVA: 0x000B069E File Offset: 0x000AE89E
	public Vector3 CollisionVector
	{
		get
		{
			return this.m_CollisionVector;
		}
	}

	// Token: 0x170002EE RID: 750
	// (get) Token: 0x06002DF2 RID: 11762 RVA: 0x000B06A6 File Offset: 0x000AE8A6
	private vp_FPPlayerEventHandler Player
	{
		get
		{
			if (this.m_Player == null && base.EventHandler != null)
			{
				this.m_Player = (vp_FPPlayerEventHandler)base.EventHandler;
			}
			return this.m_Player;
		}
	}

	// Token: 0x170002EF RID: 751
	// (get) Token: 0x06002DF3 RID: 11763 RVA: 0x000B06DB File Offset: 0x000AE8DB
	private Rigidbody FirstRigidBody
	{
		get
		{
			if (this.m_FirstRigidbody == null)
			{
				this.m_FirstRigidbody = base.Transform.root.GetComponentInChildren<Rigidbody>();
			}
			return this.m_FirstRigidbody;
		}
	}

	// Token: 0x170002F0 RID: 752
	// (get) Token: 0x06002DF4 RID: 11764 RVA: 0x000B0707 File Offset: 0x000AE907
	// (set) Token: 0x06002DF5 RID: 11765 RVA: 0x000B071A File Offset: 0x000AE91A
	public Vector2 Angle
	{
		get
		{
			return new Vector2(this.m_Pitch, this.m_Yaw);
		}
		set
		{
			this.Pitch = value.x;
			this.Yaw = value.y;
		}
	}

	// Token: 0x170002F1 RID: 753
	// (get) Token: 0x06002DF6 RID: 11766 RVA: 0x000B0734 File Offset: 0x000AE934
	public Vector3 Forward
	{
		get
		{
			return this.m_Transform.forward;
		}
	}

	// Token: 0x170002F2 RID: 754
	// (get) Token: 0x06002DF7 RID: 11767 RVA: 0x000B0741 File Offset: 0x000AE941
	// (set) Token: 0x06002DF8 RID: 11768 RVA: 0x000B0749 File Offset: 0x000AE949
	public float Pitch
	{
		get
		{
			return this.m_Pitch;
		}
		set
		{
			if (value > 90f)
			{
				value -= 360f;
			}
			this.m_Pitch = value;
		}
	}

	// Token: 0x170002F3 RID: 755
	// (get) Token: 0x06002DF9 RID: 11769 RVA: 0x000B0763 File Offset: 0x000AE963
	// (set) Token: 0x06002DFA RID: 11770 RVA: 0x000B076B File Offset: 0x000AE96B
	public float Yaw
	{
		get
		{
			return this.m_Yaw;
		}
		set
		{
			this.m_Yaw = value;
		}
	}

	// Token: 0x06002DFB RID: 11771 RVA: 0x000B0774 File Offset: 0x000AE974
	protected override void Awake()
	{
		base.Awake();
		this.FPController = base.Root.GetComponent<vp_FPController>();
		this.SetRotation(new Vector2(base.Transform.eulerAngles.x, base.Transform.eulerAngles.y));
		base.Parent.gameObject.layer = 8;
		base.GetComponent<Camera>().cullingMask &= 2147483391;
		base.GetComponent<Camera>().depth = 0f;
		this.m_PositionSpring = new vp_Spring(base.Transform, vp_Spring.UpdateMode.Position, false);
		this.m_PositionSpring.MinVelocity = 1E-05f;
		this.m_PositionSpring.RestState = this.PositionOffset;
		this.m_PositionSpring2 = new vp_Spring(base.Transform, vp_Spring.UpdateMode.PositionAdditiveLocal, false);
		this.m_PositionSpring2.MinVelocity = 1E-05f;
		this.m_RotationSpring = new vp_Spring(base.Transform, vp_Spring.UpdateMode.RotationAdditiveLocal, false);
		this.m_RotationSpring.MinVelocity = 1E-05f;
	}

	// Token: 0x06002DFC RID: 11772 RVA: 0x000B0878 File Offset: 0x000AEA78
	protected override void OnEnable()
	{
		base.OnEnable();
		vp_TargetEvent<float>.Register(this.m_Root, "CameraBombShake", new Action<float>(this.OnMessage_CameraBombShake));
		vp_TargetEvent<float>.Register(this.m_Root, "CameraGroundStomp", new Action<float>(this.OnMessage_CameraGroundStomp));
	}

	// Token: 0x06002DFD RID: 11773 RVA: 0x000B08C8 File Offset: 0x000AEAC8
	protected override void OnDisable()
	{
		base.OnDisable();
		vp_TargetEvent<float>.Unregister(this.m_Root, "CameraBombShake", new Action<float>(this.OnMessage_CameraBombShake));
		vp_TargetEvent<float>.Unregister(this.m_Root, "CameraGroundStomp", new Action<float>(this.OnMessage_CameraGroundStomp));
	}

	// Token: 0x06002DFE RID: 11774 RVA: 0x000B0915 File Offset: 0x000AEB15
	protected override void Start()
	{
		base.Start();
		this.Refresh();
		this.SnapSprings();
		this.SnapZoom();
	}

	// Token: 0x06002DFF RID: 11775 RVA: 0x000B092F File Offset: 0x000AEB2F
	protected override void Init()
	{
		base.Init();
	}

	// Token: 0x06002E00 RID: 11776 RVA: 0x000B0937 File Offset: 0x000AEB37
	protected override void Update()
	{
		base.Update();
		if (Time.timeScale == 0f)
		{
			return;
		}
		this.UpdateInput();
	}

	// Token: 0x06002E01 RID: 11777 RVA: 0x000B0952 File Offset: 0x000AEB52
	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (Time.timeScale == 0f)
		{
			return;
		}
		this.UpdateSwaying();
		this.UpdateBob();
		this.UpdateEarthQuake();
		this.UpdateShakes();
		this.UpdateSprings();
	}

	// Token: 0x06002E02 RID: 11778 RVA: 0x000B0988 File Offset: 0x000AEB88
	protected override void LateUpdate()
	{
		base.LateUpdate();
		if (Time.timeScale == 0f)
		{
			return;
		}
		this.m_Transform.position = this.FPController.SmoothPosition;
		if (this.Player.IsFirstPerson.Get())
		{
			this.m_Transform.localPosition += this.m_PositionSpring.State + this.m_PositionSpring2.State;
		}
		else
		{
			this.m_Transform.localPosition += this.m_PositionSpring.State + Vector3.Scale(this.m_PositionSpring2.State, Vector3.up);
		}
		if (this.HasCollision)
		{
			this.DoCameraCollision();
		}
		Quaternion lhs = Quaternion.AngleAxis(this.m_Yaw, Vector3.up);
		Quaternion rhs = Quaternion.AngleAxis(0f, Vector3.left);
		base.Parent.rotation = vp_MathUtility.NaNSafeQuaternion(lhs * rhs, base.Parent.rotation);
		rhs = Quaternion.AngleAxis(-this.m_Pitch, Vector3.left);
		base.Transform.rotation = vp_MathUtility.NaNSafeQuaternion(lhs * rhs, base.Transform.rotation);
		base.Transform.localEulerAngles += vp_MathUtility.NaNSafeVector3(Vector3.forward * this.m_RotationSpring.State.z, default(Vector3));
		this.Update3rdPerson();
	}

	// Token: 0x06002E03 RID: 11779 RVA: 0x000B0B0C File Offset: 0x000AED0C
	private void Update3rdPerson()
	{
		if (this.Position3rdPersonOffset == Vector3.zero)
		{
			return;
		}
		if (this.PositionOnDeath != Vector3.zero)
		{
			base.Transform.position = this.PositionOnDeath;
			if (this.FirstRigidBody != null)
			{
				base.Transform.LookAt(this.FirstRigidBody.transform.position + Vector3.up);
				return;
			}
			base.Transform.LookAt(base.Root.position + Vector3.up);
			return;
		}
		else
		{
			if (this.Player.IsFirstPerson.Get())
			{
				this.m_Final3rdPersonCameraOffset = Vector3.zero;
				this.m_Current3rdPersonBlend = 0f;
				this.LookPoint = this.GetLookPoint();
				return;
			}
			this.m_Current3rdPersonBlend = Mathf.Lerp(this.m_Current3rdPersonBlend, 1f, Time.deltaTime);
			this.m_Final3rdPersonCameraOffset = base.Transform.position;
			if (base.Transform.localPosition.z > -0.2f)
			{
				base.Transform.localPosition = new Vector3(base.Transform.localPosition.x, base.Transform.localPosition.y, -0.2f);
			}
			Vector3 vector = base.Transform.position;
			vector += this.m_Transform.right * this.Position3rdPersonOffset.x;
			vector += this.m_Transform.up * this.Position3rdPersonOffset.y;
			vector += this.m_Transform.forward * this.Position3rdPersonOffset.z;
			base.Transform.position = Vector3.Lerp(base.Transform.position, vector, this.m_Current3rdPersonBlend);
			this.m_Final3rdPersonCameraOffset -= base.Transform.position;
			this.DoCameraCollision();
			this.LookPoint = this.GetLookPoint();
			return;
		}
	}

	// Token: 0x06002E04 RID: 11780 RVA: 0x000B0D18 File Offset: 0x000AEF18
	public virtual void DoCameraCollision()
	{
		this.m_CameraCollisionStartPos = this.FPController.Transform.TransformPoint(0f, this.PositionOffset.y, 0f) - (this.m_Player.IsFirstPerson.Get() ? Vector3.zero : (this.FPController.Transform.position - this.FPController.SmoothPosition));
		this.m_CameraCollisionEndPos = base.Transform.position + (base.Transform.position - this.m_CameraCollisionStartPos).normalized * this.FPController.CharacterController.radius;
		this.m_CollisionVector = Vector3.zero;
		if (Physics.Linecast(this.m_CameraCollisionStartPos, this.m_CameraCollisionEndPos, out this.m_CameraHit, -675375893) && !this.m_CameraHit.collider.isTrigger)
		{
			base.Transform.position = this.m_CameraHit.point - (this.m_CameraHit.point - this.m_CameraCollisionStartPos).normalized * this.FPController.CharacterController.radius;
			this.m_CollisionVector = this.m_CameraHit.point - this.m_CameraCollisionEndPos;
		}
		if (base.Transform.localPosition.y < this.PositionGroundLimit)
		{
			base.Transform.localPosition = new Vector3(base.Transform.localPosition.x, this.PositionGroundLimit, base.Transform.localPosition.z);
		}
	}

	// Token: 0x06002E05 RID: 11781 RVA: 0x000B0ECF File Offset: 0x000AF0CF
	public virtual void AddForce(Vector3 force)
	{
		this.m_PositionSpring.AddForce(force);
	}

	// Token: 0x06002E06 RID: 11782 RVA: 0x000B0EDD File Offset: 0x000AF0DD
	public virtual void AddForce(float x, float y, float z)
	{
		this.AddForce(new Vector3(x, y, z));
	}

	// Token: 0x06002E07 RID: 11783 RVA: 0x000B0EED File Offset: 0x000AF0ED
	public virtual void AddForce2(Vector3 force)
	{
		this.m_PositionSpring2.AddForce(force);
	}

	// Token: 0x06002E08 RID: 11784 RVA: 0x000B0EFB File Offset: 0x000AF0FB
	public void AddForce2(float x, float y, float z)
	{
		this.AddForce2(new Vector3(x, y, z));
	}

	// Token: 0x06002E09 RID: 11785 RVA: 0x000B0F0B File Offset: 0x000AF10B
	public virtual void AddRollForce(float force)
	{
		this.m_RotationSpring.AddForce(Vector3.forward * force);
	}

	// Token: 0x06002E0A RID: 11786 RVA: 0x000B0F24 File Offset: 0x000AF124
	protected virtual void UpdateInput()
	{
		if (this.Player.Dead.Active)
		{
			return;
		}
		if (this.Player.InputSmoothLook.Get() == Vector2.zero)
		{
			return;
		}
		this.m_Yaw += this.Player.InputSmoothLook.Get().x;
		this.m_Pitch += this.Player.InputSmoothLook.Get().y;
		this.m_Yaw = ((this.m_Yaw < -360f) ? (this.m_Yaw += 360f) : this.m_Yaw);
		this.m_Yaw = ((this.m_Yaw > 360f) ? (this.m_Yaw -= 360f) : this.m_Yaw);
		this.m_Yaw = Mathf.Clamp(this.m_Yaw, this.RotationYawLimit.x, this.RotationYawLimit.y);
		this.m_Pitch = ((this.m_Pitch < -360f) ? (this.m_Pitch += 360f) : this.m_Pitch);
		this.m_Pitch = ((this.m_Pitch > 360f) ? (this.m_Pitch -= 360f) : this.m_Pitch);
		this.m_Pitch = Mathf.Clamp(this.m_Pitch, -this.RotationPitchLimit.x, -this.RotationPitchLimit.y);
	}

	// Token: 0x06002E0B RID: 11787 RVA: 0x00003296 File Offset: 0x00001496
	protected virtual void UpdateZoom()
	{
	}

	// Token: 0x06002E0C RID: 11788 RVA: 0x00003296 File Offset: 0x00001496
	public void RefreshZoom()
	{
	}

	// Token: 0x06002E0D RID: 11789 RVA: 0x000B10C4 File Offset: 0x000AF2C4
	public virtual void Zoom()
	{
		this.m_FinalZoomTime = Time.time + this.RenderingZoomDamping;
	}

	// Token: 0x06002E0E RID: 11790 RVA: 0x00003296 File Offset: 0x00001496
	public virtual void SnapZoom()
	{
	}

	// Token: 0x06002E0F RID: 11791 RVA: 0x000B10D8 File Offset: 0x000AF2D8
	protected virtual void UpdateShakes()
	{
		if (this.ShakeSpeed != 0f)
		{
			this.m_Yaw -= this.m_Shake.y;
			this.m_Pitch -= this.m_Shake.x;
			this.m_Shake = Vector3.Scale(vp_SmoothRandom.GetVector3Centered(this.ShakeSpeed), this.ShakeAmplitude);
			this.m_Yaw += this.m_Shake.y;
			this.m_Pitch += this.m_Shake.x;
			this.m_RotationSpring.AddForce(Vector3.forward * this.m_Shake.z * Time.timeScale);
		}
	}

	// Token: 0x06002E10 RID: 11792 RVA: 0x000B119C File Offset: 0x000AF39C
	protected virtual void UpdateBob()
	{
		if (this.BobAmplitude == Vector4.zero || this.BobRate == Vector4.zero)
		{
			return;
		}
		if (!this.Player.IsFirstPerson.Get())
		{
			return;
		}
		this.m_BobSpeed = ((this.BobRequireGroundContact && !this.FPController.Grounded) ? 0f : this.FPController.CharacterController.velocity.sqrMagnitude);
		this.m_BobSpeed = Mathf.Min(this.m_BobSpeed * this.BobInputVelocityScale, this.BobMaxInputVelocity);
		this.m_BobSpeed = Mathf.Round(this.m_BobSpeed * 1000f) / 1000f;
		if (this.m_BobSpeed == 0f)
		{
			this.m_BobSpeed = Mathf.Min(this.m_LastBobSpeed * 0.93f, this.BobMaxInputVelocity);
		}
		this.m_CurrentBobAmp.y = this.m_BobSpeed * (this.BobAmplitude.y * -0.0001f);
		this.m_CurrentBobVal.y = Mathf.Cos(Time.time * (this.BobRate.y * 10f)) * this.m_CurrentBobAmp.y;
		this.m_CurrentBobAmp.x = this.m_BobSpeed * (this.BobAmplitude.x * 0.0001f);
		this.m_CurrentBobVal.x = Mathf.Cos(Time.time * (this.BobRate.x * 10f)) * this.m_CurrentBobAmp.x;
		this.m_CurrentBobAmp.z = this.m_BobSpeed * (this.BobAmplitude.z * 0.0001f);
		this.m_CurrentBobVal.z = Mathf.Cos(Time.time * (this.BobRate.z * 10f)) * this.m_CurrentBobAmp.z;
		this.m_CurrentBobAmp.w = this.m_BobSpeed * (this.BobAmplitude.w * 0.0001f);
		this.m_CurrentBobVal.w = Mathf.Cos(Time.time * (this.BobRate.w * 10f)) * this.m_CurrentBobAmp.w;
		this.m_PositionSpring.AddForce(this.m_CurrentBobVal * Time.timeScale);
		this.AddRollForce(this.m_CurrentBobVal.w * Time.timeScale);
		this.m_LastBobSpeed = this.m_BobSpeed;
		this.DetectBobStep(this.m_BobSpeed, this.m_CurrentBobVal.y);
	}

	// Token: 0x06002E11 RID: 11793 RVA: 0x000B1438 File Offset: 0x000AF638
	protected virtual void DetectBobStep(float speed, float upBob)
	{
		if (this.BobStepCallback == null)
		{
			return;
		}
		if (speed < this.BobStepThreshold)
		{
			return;
		}
		bool flag = this.m_LastUpBob < upBob;
		this.m_LastUpBob = upBob;
		if (flag && !this.m_BobWasElevating)
		{
			this.BobStepCallback();
		}
		this.m_BobWasElevating = flag;
	}

	// Token: 0x06002E12 RID: 11794 RVA: 0x000B148C File Offset: 0x000AF68C
	protected virtual void UpdateSwaying()
	{
		Vector3 vector = base.Transform.InverseTransformDirection(this.FPController.CharacterController.velocity * 0.016f) * Time.timeScale;
		this.AddRollForce(vector.x * this.RotationStrafeRoll);
	}

	// Token: 0x06002E13 RID: 11795 RVA: 0x000B14DC File Offset: 0x000AF6DC
	protected virtual void UpdateEarthQuake()
	{
		if (this.Player == null)
		{
			return;
		}
		if (!this.Player.CameraEarthQuake.Active)
		{
			return;
		}
		if (this.m_PositionSpring.State.y >= this.m_PositionSpring.RestState.y)
		{
			Vector3 vector = this.Player.CameraEarthQuakeForce.Get();
			vector.y = -vector.y;
			this.Player.CameraEarthQuakeForce.Set(vector);
		}
		this.m_PositionSpring.AddForce(this.Player.CameraEarthQuakeForce.Get() * this.PositionEarthQuakeFactor);
		this.m_RotationSpring.AddForce(Vector3.forward * (-this.Player.CameraEarthQuakeForce.Get().x * 2f) * this.RotationEarthQuakeFactor);
	}

	// Token: 0x06002E14 RID: 11796 RVA: 0x000B15D3 File Offset: 0x000AF7D3
	protected virtual void UpdateSprings()
	{
		this.m_PositionSpring.FixedUpdate();
		this.m_PositionSpring2.FixedUpdate();
		this.m_RotationSpring.FixedUpdate();
	}

	// Token: 0x06002E15 RID: 11797 RVA: 0x000B15F8 File Offset: 0x000AF7F8
	public virtual void DoBomb(Vector3 positionForce, float minRollForce, float maxRollForce)
	{
		this.AddForce2(positionForce);
		float num = UnityEngine.Random.Range(minRollForce, maxRollForce);
		if (UnityEngine.Random.value > 0.5f)
		{
			num = -num;
		}
		this.AddRollForce(num);
	}

	// Token: 0x06002E16 RID: 11798 RVA: 0x000B162C File Offset: 0x000AF82C
	public override void Refresh()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.Stiffness = new Vector3(this.PositionSpringStiffness, this.PositionSpringStiffness, this.PositionSpringStiffness);
			this.m_PositionSpring.Damping = Vector3.one - new Vector3(this.PositionSpringDamping, this.PositionSpringDamping, this.PositionSpringDamping);
			this.m_PositionSpring.MinState.y = this.PositionGroundLimit;
			this.m_PositionSpring.RestState = this.PositionOffset;
		}
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.Stiffness = new Vector3(this.PositionSpring2Stiffness, this.PositionSpring2Stiffness, this.PositionSpring2Stiffness);
			this.m_PositionSpring2.Damping = Vector3.one - new Vector3(this.PositionSpring2Damping, this.PositionSpring2Damping, this.PositionSpring2Damping);
			this.m_PositionSpring2.MinState.y = -this.PositionOffset.y + this.PositionGroundLimit;
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.Stiffness = new Vector3(this.RotationSpringStiffness, this.RotationSpringStiffness, this.RotationSpringStiffness);
			this.m_RotationSpring.Damping = Vector3.one - new Vector3(this.RotationSpringDamping, this.RotationSpringDamping, this.RotationSpringDamping);
		}
		this.Zoom();
	}

	// Token: 0x06002E17 RID: 11799 RVA: 0x000B1794 File Offset: 0x000AF994
	public virtual void SnapSprings()
	{
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.RestState = this.PositionOffset;
			this.m_PositionSpring.State = this.PositionOffset;
			this.m_PositionSpring.Stop(true);
		}
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.RestState = Vector3.zero;
			this.m_PositionSpring2.State = Vector3.zero;
			this.m_PositionSpring2.Stop(true);
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.RestState = Vector3.zero;
			this.m_RotationSpring.State = Vector3.zero;
			this.m_RotationSpring.Stop(true);
		}
	}

	// Token: 0x06002E18 RID: 11800 RVA: 0x000B1840 File Offset: 0x000AFA40
	public virtual void StopSprings()
	{
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.Stop(true);
		}
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.Stop(true);
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.Stop(true);
		}
		this.m_BobSpeed = 0f;
		this.m_LastBobSpeed = 0f;
	}

	// Token: 0x06002E19 RID: 11801 RVA: 0x000B189F File Offset: 0x000AFA9F
	public virtual void Stop()
	{
		this.SnapSprings();
		this.SnapZoom();
		this.Refresh();
	}

	// Token: 0x06002E1A RID: 11802 RVA: 0x000B18B3 File Offset: 0x000AFAB3
	public virtual void SetRotation(Vector2 eulerAngles, bool stopZoomAndSprings)
	{
		this.Angle = eulerAngles;
		if (stopZoomAndSprings)
		{
			this.Stop();
		}
	}

	// Token: 0x06002E1B RID: 11803 RVA: 0x000B18C5 File Offset: 0x000AFAC5
	public virtual void SetRotation(Vector2 eulerAngles)
	{
		this.Angle = eulerAngles;
		this.Stop();
	}

	// Token: 0x06002E1C RID: 11804 RVA: 0x000B18D4 File Offset: 0x000AFAD4
	public virtual void SetRotation(Vector2 eulerAngles, bool stopZoomAndSprings, bool obsolete)
	{
		this.SetRotation(eulerAngles, stopZoomAndSprings);
	}

	// Token: 0x06002E1D RID: 11805 RVA: 0x000B18E0 File Offset: 0x000AFAE0
	public Vector3 GetLookPoint()
	{
		if (!this.Player.IsFirstPerson.Get() && Physics.Linecast(base.Transform.position, base.Transform.position + base.Transform.forward * 1000f, out this.m_LookPointHit, -675375893) && !this.m_LookPointHit.collider.isTrigger && base.Root.InverseTransformPoint(this.m_LookPointHit.point).z > 0f)
		{
			return this.m_LookPointHit.point;
		}
		return base.Transform.position + base.Transform.forward * 1000f;
	}

	// Token: 0x06002E1E RID: 11806 RVA: 0x000B19AE File Offset: 0x000AFBAE
	public virtual Vector3 Get_LookPoint()
	{
		return this.LookPoint;
	}

	// Token: 0x170002F4 RID: 756
	// (get) Token: 0x06002E1F RID: 11807 RVA: 0x000B19AE File Offset: 0x000AFBAE
	public virtual Vector3 OnValue_LookPoint
	{
		get
		{
			return this.LookPoint;
		}
	}

	// Token: 0x06002E20 RID: 11808 RVA: 0x000B19B8 File Offset: 0x000AFBB8
	protected virtual Vector3 Get_CameraLookDirection()
	{
		return (this.Player.LookPoint.Get() - base.Transform.position).normalized;
	}

	// Token: 0x170002F5 RID: 757
	// (get) Token: 0x06002E21 RID: 11809 RVA: 0x000B19F4 File Offset: 0x000AFBF4
	protected virtual Vector3 OnValue_CameraLookDirection
	{
		get
		{
			return (this.Player.LookPoint.Get() - base.Transform.position).normalized;
		}
	}

	// Token: 0x06002E22 RID: 11810 RVA: 0x000B1A30 File Offset: 0x000AFC30
	protected virtual void OnMessage_FallImpact(float impact)
	{
		impact = Mathf.Abs(impact * 55f);
		float num = impact * this.PositionKneeling;
		float num2 = impact * this.RotationKneeling;
		num = Mathf.SmoothStep(0f, 1f, num);
		num2 = Mathf.SmoothStep(0f, 1f, num2);
		num2 = Mathf.SmoothStep(0f, 1f, num2);
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.AddSoftForce(Vector3.down * num, (float)this.PositionKneelingSoftness);
		}
		if (this.m_RotationSpring != null)
		{
			float d = (UnityEngine.Random.value > 0.5f) ? (num2 * 2f) : (-(num2 * 2f));
			this.m_RotationSpring.AddSoftForce(Vector3.forward * d, (float)this.RotationKneelingSoftness);
		}
	}

	// Token: 0x06002E23 RID: 11811 RVA: 0x000B1AFC File Offset: 0x000AFCFC
	protected virtual void OnMessage_HeadImpact(float impact)
	{
		if (this.m_RotationSpring != null && Mathf.Abs(this.m_RotationSpring.State.z) < 30f)
		{
			this.m_RotationSpring.AddForce(Vector3.forward * (impact * 20f) * Time.timeScale);
		}
	}

	// Token: 0x06002E24 RID: 11812 RVA: 0x000B1B53 File Offset: 0x000AFD53
	protected virtual void OnMessage_CameraGroundStomp(float impact)
	{
		this.AddForce2(new Vector3(0f, -1f, 0f) * impact);
	}

	// Token: 0x06002E25 RID: 11813 RVA: 0x000B1B75 File Offset: 0x000AFD75
	protected virtual void OnMessage_CameraBombShake(float impact)
	{
		this.DoBomb(new Vector3(1f, -10f, 1f) * impact, 1f, 2f);
	}

	// Token: 0x06002E26 RID: 11814 RVA: 0x000B1BA1 File Offset: 0x000AFDA1
	protected virtual void OnStart_Zoom()
	{
		if (this.Player == null)
		{
			return;
		}
		this.Player.Run.Stop(0f);
	}

	// Token: 0x06002E27 RID: 11815 RVA: 0x000B1BC7 File Offset: 0x000AFDC7
	protected virtual bool CanStart_Run()
	{
		return this.Player == null || !this.Player.Zoom.Active;
	}

	// Token: 0x06002E28 RID: 11816 RVA: 0x000B1BEE File Offset: 0x000AFDEE
	protected virtual Vector2 Get_Rotation()
	{
		return this.Angle;
	}

	// Token: 0x06002E29 RID: 11817 RVA: 0x000B1BF6 File Offset: 0x000AFDF6
	protected virtual void Set_Rotation(Vector2 value)
	{
		this.Angle = value;
	}

	// Token: 0x170002F6 RID: 758
	// (get) Token: 0x06002E2A RID: 11818 RVA: 0x000B1BEE File Offset: 0x000AFDEE
	// (set) Token: 0x06002E2B RID: 11819 RVA: 0x000B1BF6 File Offset: 0x000AFDF6
	protected virtual Vector2 OnValue_Rotation
	{
		get
		{
			return this.Angle;
		}
		set
		{
			this.Angle = value;
		}
	}

	// Token: 0x06002E2C RID: 11820 RVA: 0x000B1BFF File Offset: 0x000AFDFF
	protected virtual void OnMessage_Stop()
	{
		this.Stop();
	}

	// Token: 0x06002E2D RID: 11821 RVA: 0x000B1C07 File Offset: 0x000AFE07
	protected virtual void OnStart_Dead()
	{
		if (this.Player.IsFirstPerson.Get())
		{
			return;
		}
		this.PositionOnDeath = base.Transform.position - this.m_Final3rdPersonCameraOffset;
	}

	// Token: 0x06002E2E RID: 11822 RVA: 0x000B1C3D File Offset: 0x000AFE3D
	protected virtual void OnStop_Dead()
	{
		if (this.Player.IsFirstPerson.Get())
		{
			return;
		}
		this.PositionOnDeath = Vector3.zero;
		this.m_Current3rdPersonBlend = 0f;
	}

	// Token: 0x06002E2F RID: 11823 RVA: 0x00013CC5 File Offset: 0x00011EC5
	protected virtual bool Get_IsLocal()
	{
		return true;
	}

	// Token: 0x170002F7 RID: 759
	// (get) Token: 0x06002E30 RID: 11824 RVA: 0x00013CC5 File Offset: 0x00011EC5
	protected virtual bool OnValue_IsLocal
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002E31 RID: 11825 RVA: 0x000B1C6D File Offset: 0x000AFE6D
	protected virtual void OnMessage_CameraToggle3rdPerson()
	{
		this.m_Player.IsFirstPerson.Set(!this.m_Player.IsFirstPerson.Get());
	}

	// Token: 0x06002E32 RID: 11826 RVA: 0x000B1C9C File Offset: 0x000AFE9C
	public override void Register(vp_EventHandler eventHandler)
	{
		base.Register(eventHandler);
		eventHandler.RegisterActivity("Run", null, null, new vp_Activity.Condition(this.CanStart_Run), null, null, null);
		eventHandler.RegisterMessage<float>("CameraBombShake", new vp_Message<float>.Sender<float>(this.OnMessage_CameraBombShake));
		eventHandler.RegisterMessage<float>("CameraGroundStomp", new vp_Message<float>.Sender<float>(this.OnMessage_CameraGroundStomp));
		eventHandler.RegisterMessage("CameraToggle3rdPerson", new vp_Message.Sender(this.OnMessage_CameraToggle3rdPerson));
		eventHandler.RegisterMessage<float>("FallImpact", new vp_Message<float>.Sender<float>(this.OnMessage_FallImpact));
		eventHandler.RegisterMessage<float>("HeadImpact", new vp_Message<float>.Sender<float>(this.OnMessage_HeadImpact));
		eventHandler.RegisterMessage("Stop", new vp_Message.Sender(this.OnMessage_Stop));
		eventHandler.RegisterActivity("Dead", new vp_Activity.Callback(this.OnStart_Dead), new vp_Activity.Callback(this.OnStop_Dead), null, null, null, null);
		eventHandler.RegisterActivity("Zoom", new vp_Activity.Callback(this.OnStart_Zoom), null, null, null, null, null);
		eventHandler.RegisterValue<bool>("IsLocal", new vp_Value<bool>.Getter<bool>(this.Get_IsLocal), null);
		eventHandler.RegisterValue<Vector2>("Rotation", new vp_Value<Vector2>.Getter<Vector2>(this.Get_Rotation), new vp_Value<Vector2>.Setter<Vector2>(this.Set_Rotation));
		eventHandler.RegisterValue<Vector3>("LookPoint", new vp_Value<Vector3>.Getter<Vector3>(this.Get_LookPoint), null);
		eventHandler.RegisterValue<Vector3>("CameraLookDirection", new vp_Value<Vector3>.Getter<Vector3>(this.Get_CameraLookDirection), null);
	}

	// Token: 0x06002E33 RID: 11827 RVA: 0x000B1E14 File Offset: 0x000B0014
	public override void Unregister(vp_EventHandler eventHandler)
	{
		base.Unregister(eventHandler);
		eventHandler.UnregisterActivity("Run", null, null, new vp_Activity.Condition(this.CanStart_Run), null, null, null);
		eventHandler.UnregisterMessage<float>("CameraBombShake", new vp_Message<float>.Sender<float>(this.OnMessage_CameraBombShake));
		eventHandler.UnregisterMessage<float>("CameraGroundStomp", new vp_Message<float>.Sender<float>(this.OnMessage_CameraGroundStomp));
		eventHandler.UnregisterMessage("CameraToggle3rdPerson", new vp_Message.Sender(this.OnMessage_CameraToggle3rdPerson));
		eventHandler.UnregisterMessage<float>("FallImpact", new vp_Message<float>.Sender<float>(this.OnMessage_FallImpact));
		eventHandler.UnregisterMessage<float>("HeadImpact", new vp_Message<float>.Sender<float>(this.OnMessage_HeadImpact));
		eventHandler.UnregisterMessage("Stop", new vp_Message.Sender(this.OnMessage_Stop));
		eventHandler.UnregisterActivity("Dead", new vp_Activity.Callback(this.OnStart_Dead), new vp_Activity.Callback(this.OnStop_Dead), null, null, null, null);
		eventHandler.UnregisterActivity("Zoom", new vp_Activity.Callback(this.OnStart_Zoom), null, null, null, null, null);
		eventHandler.UnregisterValue<bool>("IsLocal", new vp_Value<bool>.Getter<bool>(this.Get_IsLocal), null);
		eventHandler.UnregisterValue<Vector2>("Rotation", new vp_Value<Vector2>.Getter<Vector2>(this.Get_Rotation), new vp_Value<Vector2>.Setter<Vector2>(this.Set_Rotation));
		eventHandler.UnregisterValue<Vector3>("LookPoint", new vp_Value<Vector3>.Getter<Vector3>(this.Get_LookPoint), null);
		eventHandler.UnregisterValue<Vector3>("CameraLookDirection", new vp_Value<Vector3>.Getter<Vector3>(this.Get_CameraLookDirection), null);
	}

	// Token: 0x06002E34 RID: 11828 RVA: 0x000B1F8B File Offset: 0x000B018B
	protected override StateManager GetStateManager()
	{
		return new FPCameraStateManager(this);
	}

	// Token: 0x04002C11 RID: 11281
	public vp_FPController FPController;

	// Token: 0x04002C12 RID: 11282
	public float RenderingFieldOfView = 60f;

	// Token: 0x04002C13 RID: 11283
	public float RenderingZoomDamping = 0.2f;

	// Token: 0x04002C14 RID: 11284
	protected float m_FinalZoomTime;

	// Token: 0x04002C15 RID: 11285
	protected float m_ZoomOffset;

	// Token: 0x04002C16 RID: 11286
	public Vector3 PositionOffset = new Vector3(0f, 1.75f, 0.1f);

	// Token: 0x04002C17 RID: 11287
	public float PositionGroundLimit = 0.1f;

	// Token: 0x04002C18 RID: 11288
	public float PositionSpringStiffness = 0.01f;

	// Token: 0x04002C19 RID: 11289
	public float PositionSpringDamping = 0.25f;

	// Token: 0x04002C1A RID: 11290
	public float PositionSpring2Stiffness = 0.95f;

	// Token: 0x04002C1B RID: 11291
	public float PositionSpring2Damping = 0.25f;

	// Token: 0x04002C1C RID: 11292
	public float PositionKneeling = 0.025f;

	// Token: 0x04002C1D RID: 11293
	public int PositionKneelingSoftness = 1;

	// Token: 0x04002C1E RID: 11294
	public float PositionEarthQuakeFactor = 1f;

	// Token: 0x04002C1F RID: 11295
	protected vp_Spring m_PositionSpring;

	// Token: 0x04002C20 RID: 11296
	protected vp_Spring m_PositionSpring2;

	// Token: 0x04002C21 RID: 11297
	protected bool m_DrawCameraCollisionDebugLine;

	// Token: 0x04002C22 RID: 11298
	protected Vector3 PositionOnDeath = Vector3.zero;

	// Token: 0x04002C23 RID: 11299
	public Vector2 RotationPitchLimit = new Vector2(90f, -90f);

	// Token: 0x04002C24 RID: 11300
	public Vector2 RotationYawLimit = new Vector2(-360f, 360f);

	// Token: 0x04002C25 RID: 11301
	public float RotationSpringStiffness = 0.01f;

	// Token: 0x04002C26 RID: 11302
	public float RotationSpringDamping = 0.25f;

	// Token: 0x04002C27 RID: 11303
	public float RotationKneeling = 0.025f;

	// Token: 0x04002C28 RID: 11304
	public int RotationKneelingSoftness = 1;

	// Token: 0x04002C29 RID: 11305
	public float RotationStrafeRoll = 0.01f;

	// Token: 0x04002C2A RID: 11306
	public float RotationEarthQuakeFactor;

	// Token: 0x04002C2B RID: 11307
	public Vector3 LookPoint = Vector3.zero;

	// Token: 0x04002C2C RID: 11308
	protected float m_Pitch;

	// Token: 0x04002C2D RID: 11309
	protected float m_Yaw;

	// Token: 0x04002C2E RID: 11310
	protected vp_Spring m_RotationSpring;

	// Token: 0x04002C2F RID: 11311
	protected RaycastHit m_LookPointHit;

	// Token: 0x04002C30 RID: 11312
	public Vector3 Position3rdPersonOffset = new Vector3(0.5f, 0.1f, 0.75f);

	// Token: 0x04002C31 RID: 11313
	protected float m_Current3rdPersonBlend;

	// Token: 0x04002C32 RID: 11314
	protected Vector3 m_Final3rdPersonCameraOffset = Vector3.zero;

	// Token: 0x04002C33 RID: 11315
	public float ShakeSpeed;

	// Token: 0x04002C34 RID: 11316
	public Vector3 ShakeAmplitude = new Vector3(10f, 10f, 0f);

	// Token: 0x04002C35 RID: 11317
	protected Vector3 m_Shake = Vector3.zero;

	// Token: 0x04002C36 RID: 11318
	public Vector4 BobRate = new Vector4(0f, 1.4f, 0f, 0.7f);

	// Token: 0x04002C37 RID: 11319
	public Vector4 BobAmplitude = new Vector4(0f, 0.25f, 0f, 0.5f);

	// Token: 0x04002C38 RID: 11320
	public float BobInputVelocityScale = 1f;

	// Token: 0x04002C39 RID: 11321
	public float BobMaxInputVelocity = 100f;

	// Token: 0x04002C3A RID: 11322
	public bool BobRequireGroundContact = true;

	// Token: 0x04002C3B RID: 11323
	protected float m_LastBobSpeed;

	// Token: 0x04002C3C RID: 11324
	protected Vector4 m_CurrentBobAmp = Vector4.zero;

	// Token: 0x04002C3D RID: 11325
	protected Vector4 m_CurrentBobVal = Vector4.zero;

	// Token: 0x04002C3E RID: 11326
	protected float m_BobSpeed;

	// Token: 0x04002C3F RID: 11327
	public vp_FPCamera.BobStepDelegate BobStepCallback;

	// Token: 0x04002C40 RID: 11328
	public float BobStepThreshold = 10f;

	// Token: 0x04002C41 RID: 11329
	protected float m_LastUpBob;

	// Token: 0x04002C42 RID: 11330
	protected bool m_BobWasElevating;

	// Token: 0x04002C43 RID: 11331
	public bool HasCollision = true;

	// Token: 0x04002C44 RID: 11332
	protected Vector3 m_CollisionVector = Vector3.zero;

	// Token: 0x04002C45 RID: 11333
	protected Vector3 m_CameraCollisionStartPos = Vector3.zero;

	// Token: 0x04002C46 RID: 11334
	protected Vector3 m_CameraCollisionEndPos = Vector3.zero;

	// Token: 0x04002C47 RID: 11335
	protected RaycastHit m_CameraHit;

	// Token: 0x04002C48 RID: 11336
	private vp_FPPlayerEventHandler m_Player;

	// Token: 0x04002C49 RID: 11337
	private Rigidbody m_FirstRigidbody;

	// Token: 0x0200087C RID: 2172
	// (Invoke) Token: 0x06002E37 RID: 11831
	public delegate void BobStepDelegate();
}
