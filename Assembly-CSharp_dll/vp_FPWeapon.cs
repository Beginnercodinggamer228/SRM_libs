using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000885 RID: 2181
[RequireComponent(typeof(AudioSource))]
public class vp_FPWeapon : vp_Weapon
{
	// Token: 0x17000312 RID: 786
	// (get) Token: 0x06002EE0 RID: 12000 RVA: 0x000B5C7F File Offset: 0x000B3E7F
	public GameObject WeaponCamera
	{
		get
		{
			return this.m_WeaponCamera;
		}
	}

	// Token: 0x17000313 RID: 787
	// (get) Token: 0x06002EE1 RID: 12001 RVA: 0x000B5C87 File Offset: 0x000B3E87
	public GameObject WeaponModel
	{
		get
		{
			return this.m_WeaponModel;
		}
	}

	// Token: 0x17000314 RID: 788
	// (get) Token: 0x06002EE2 RID: 12002 RVA: 0x000B5C8F File Offset: 0x000B3E8F
	public Vector3 DefaultPosition
	{
		get
		{
			return (Vector3)base.DefaultState.Preset.GetFieldValue("PositionOffset");
		}
	}

	// Token: 0x17000315 RID: 789
	// (get) Token: 0x06002EE3 RID: 12003 RVA: 0x000B5CAB File Offset: 0x000B3EAB
	public Vector3 DefaultRotation
	{
		get
		{
			return (Vector3)base.DefaultState.Preset.GetFieldValue("RotationOffset");
		}
	}

	// Token: 0x17000316 RID: 790
	// (get) Token: 0x06002EE4 RID: 12004 RVA: 0x000B5CC7 File Offset: 0x000B3EC7
	// (set) Token: 0x06002EE5 RID: 12005 RVA: 0x000B5CCF File Offset: 0x000B3ECF
	public bool DrawRetractionDebugLine
	{
		get
		{
			return this.m_DrawRetractionDebugLine;
		}
		set
		{
			this.m_DrawRetractionDebugLine = value;
		}
	}

	// Token: 0x17000317 RID: 791
	// (get) Token: 0x06002EE6 RID: 12006 RVA: 0x000B5CD8 File Offset: 0x000B3ED8
	private vp_FPPlayerEventHandler FPPlayer
	{
		get
		{
			if (this.m_FPPlayer == null && base.EventHandler != null)
			{
				this.m_FPPlayer = (base.EventHandler as vp_FPPlayerEventHandler);
			}
			return this.m_FPPlayer;
		}
	}

	// Token: 0x06002EE7 RID: 12007 RVA: 0x000B5D10 File Offset: 0x000B3F10
	protected override void Awake()
	{
		base.Awake();
		if (base.transform.parent == null)
		{
			Debug.LogError("Error (" + this + ") Must not be placed in scene root. Disabling self.");
			vp_Utility.Activate(base.gameObject, false);
			return;
		}
		this.Controller = base.Transform.root.GetComponent<CharacterController>();
		if (this.Controller == null)
		{
			Debug.LogError("Error (" + this + ") Could not find CharacterController. Disabling self.");
			vp_Utility.Activate(base.gameObject, false);
			return;
		}
		base.Transform.eulerAngles = Vector3.zero;
		foreach (object obj in base.Transform.parent)
		{
			Camera camera = (Camera)((Transform)obj).GetComponent(typeof(Camera));
			if (camera != null)
			{
				this.m_WeaponCamera = camera.gameObject;
				break;
			}
		}
		if (base.GetComponent<Collider>() != null)
		{
			base.GetComponent<Collider>().enabled = false;
		}
	}

	// Token: 0x06002EE8 RID: 12008 RVA: 0x000B5E40 File Offset: 0x000B4040
	protected override void Start()
	{
		this.InstantiateWeaponModel();
		base.Start();
		this.m_WeaponGroup = new GameObject(base.name + "Transform");
		this.m_WeaponGroupTransform = this.m_WeaponGroup.transform;
		this.m_WeaponGroupTransform.parent = base.Transform.parent;
		this.m_WeaponGroupTransform.localPosition = this.PositionOffset;
		vp_Layer.Set(this.m_WeaponGroup, 31, false);
		base.Transform.parent = this.m_WeaponGroupTransform;
		base.Transform.localPosition = Vector3.zero;
		this.m_WeaponGroupTransform.localEulerAngles = this.RotationOffset;
		if (this.m_WeaponCamera != null && vp_Utility.IsActive(this.m_WeaponCamera.gameObject))
		{
			vp_Layer.Set(base.gameObject, 31, true);
		}
		this.m_Pivot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		this.m_Pivot.name = "Pivot";
		this.m_Pivot.GetComponent<Collider>().enabled = false;
		this.m_Pivot.gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		this.m_Pivot.transform.parent = this.m_WeaponGroupTransform;
		this.m_Pivot.transform.localPosition = Vector3.zero;
		this.m_Pivot.layer = 31;
		vp_Utility.Activate(this.m_Pivot.gameObject, false);
		Material material = new Material(Shader.Find("Transparent/Diffuse"));
		material.color = new Color(0f, 0f, 1f, 0.5f);
		this.m_Pivot.GetComponent<Renderer>().material = material;
		this.m_PositionSpring = new vp_Spring(this.m_WeaponGroup.gameObject.transform, vp_Spring.UpdateMode.Position, true);
		this.m_PositionSpring.RestState = this.PositionOffset;
		this.m_PositionPivotSpring = new vp_Spring(base.Transform, vp_Spring.UpdateMode.Position, true);
		this.m_PositionPivotSpring.RestState = this.PositionPivot;
		this.m_PositionSpring2 = new vp_Spring(base.Transform, vp_Spring.UpdateMode.PositionAdditiveLocal, true);
		this.m_PositionSpring2.MinVelocity = 1E-05f;
		this.m_RotationSpring = new vp_Spring(this.m_WeaponGroup.gameObject.transform, vp_Spring.UpdateMode.Rotation, true);
		this.m_RotationSpring.RestState = this.RotationOffset;
		this.m_RotationPivotSpring = new vp_Spring(base.Transform, vp_Spring.UpdateMode.Rotation, true);
		this.m_RotationPivotSpring.RestState = this.RotationPivot;
		this.m_RotationSpring2 = new vp_Spring(this.m_WeaponGroup.gameObject.transform, vp_Spring.UpdateMode.RotationAdditiveLocal, true);
		this.m_RotationSpring2.MinVelocity = 1E-05f;
		this.SnapSprings();
		this.Refresh();
	}

	// Token: 0x06002EE9 RID: 12009 RVA: 0x000B60FC File Offset: 0x000B42FC
	public virtual void InstantiateWeaponModel()
	{
		if (this.WeaponPrefab != null)
		{
			if (this.m_WeaponModel != null && this.m_WeaponModel != base.gameObject)
			{
				Destroyer.Destroy(this.m_WeaponModel, "vp_FPWeapon.InstantiateWeaponModel");
			}
			this.m_WeaponModel = UnityEngine.Object.Instantiate<GameObject>(this.WeaponPrefab);
			this.m_WeaponModel.transform.parent = base.transform;
			this.m_WeaponModel.transform.localPosition = Vector3.zero;
			this.m_WeaponModel.transform.localScale = new Vector3(1f, 1f, this.RenderingZScale);
			this.m_WeaponModel.transform.localEulerAngles = Vector3.zero;
			if (this.m_WeaponCamera != null && vp_Utility.IsActive(this.m_WeaponCamera.gameObject))
			{
				vp_Layer.Set(this.m_WeaponModel, 31, true);
			}
		}
		else
		{
			this.m_WeaponModel = base.gameObject;
		}
		base.CacheRenderers();
	}

	// Token: 0x06002EEA RID: 12010 RVA: 0x000B6203 File Offset: 0x000B4403
	protected override void Init()
	{
		base.Init();
		this.ScheduleAmbientAnimation();
	}

	// Token: 0x06002EEB RID: 12011 RVA: 0x000B6211 File Offset: 0x000B4411
	protected override void Update()
	{
		base.Update();
		if (Time.timeScale != 0f)
		{
			this.UpdateInput();
		}
	}

	// Token: 0x06002EEC RID: 12012 RVA: 0x000B622C File Offset: 0x000B442C
	protected override void FixedUpdate()
	{
		if (Time.timeScale != 0f)
		{
			this.UpdateZoom();
			this.UpdateSwaying();
			this.UpdateBob();
			this.UpdateEarthQuake();
			this.UpdateStep();
			this.UpdateShakes();
			this.UpdateRetraction(true);
			this.UpdateSprings();
			this.UpdateLookDown();
		}
	}

	// Token: 0x06002EED RID: 12013 RVA: 0x00003296 File Offset: 0x00001496
	protected override void LateUpdate()
	{
	}

	// Token: 0x06002EEE RID: 12014 RVA: 0x000B627C File Offset: 0x000B447C
	public virtual void AddForce(Vector3 force)
	{
		this.m_PositionSpring.AddForce(force);
	}

	// Token: 0x06002EEF RID: 12015 RVA: 0x000B628A File Offset: 0x000B448A
	public virtual void AddForce(float x, float y, float z)
	{
		this.AddForce(new Vector3(x, y, z));
	}

	// Token: 0x06002EF0 RID: 12016 RVA: 0x000B629A File Offset: 0x000B449A
	public virtual void AddForce(Vector3 positional, Vector3 angular)
	{
		this.m_PositionSpring.AddForce(positional);
		this.m_RotationSpring.AddForce(angular);
	}

	// Token: 0x06002EF1 RID: 12017 RVA: 0x000B62B4 File Offset: 0x000B44B4
	public virtual void AddForce(float xPos, float yPos, float zPos, float xRot, float yRot, float zRot)
	{
		this.AddForce(new Vector3(xPos, yPos, zPos), new Vector3(xRot, yRot, zRot));
	}

	// Token: 0x06002EF2 RID: 12018 RVA: 0x000B62CF File Offset: 0x000B44CF
	public virtual void AddSoftForce(Vector3 force, int frames)
	{
		this.m_PositionSpring.AddSoftForce(force, (float)frames);
	}

	// Token: 0x06002EF3 RID: 12019 RVA: 0x000B62DF File Offset: 0x000B44DF
	public virtual void AddSoftForce(float x, float y, float z, int frames)
	{
		this.AddSoftForce(new Vector3(x, y, z), frames);
	}

	// Token: 0x06002EF4 RID: 12020 RVA: 0x000B62F1 File Offset: 0x000B44F1
	public virtual void AddSoftForce(Vector3 positional, Vector3 angular, int frames)
	{
		this.m_PositionSpring.AddSoftForce(positional, (float)frames);
		this.m_RotationSpring.AddSoftForce(angular, (float)frames);
	}

	// Token: 0x06002EF5 RID: 12021 RVA: 0x000B630F File Offset: 0x000B450F
	public virtual void AddSoftForce(float xPos, float yPos, float zPos, float xRot, float yRot, float zRot, int frames)
	{
		this.AddSoftForce(new Vector3(xPos, yPos, zPos), new Vector3(xRot, yRot, zRot), frames);
	}

	// Token: 0x06002EF6 RID: 12022 RVA: 0x000B632C File Offset: 0x000B452C
	protected virtual void UpdateInput()
	{
		if (base.Player.Dead.Active)
		{
			return;
		}
		this.m_LookInput = this.FPPlayer.InputRawLook.Get() / base.Delta * Time.timeScale * Time.timeScale;
		this.m_LookInput *= this.RotationInputVelocityScale;
		this.m_LookInput = Vector3.Min(this.m_LookInput, Vector3.one * this.RotationMaxInputVelocity);
		this.m_LookInput = Vector3.Max(this.m_LookInput, Vector3.one * -this.RotationMaxInputVelocity);
	}

	// Token: 0x06002EF7 RID: 12023 RVA: 0x000B63F4 File Offset: 0x000B45F4
	protected virtual void UpdateZoom()
	{
		if (this.m_FinalZoomTime <= Time.time)
		{
			return;
		}
		if (!this.m_Wielded)
		{
			return;
		}
		this.RenderingZoomDamping = Mathf.Max(this.RenderingZoomDamping, 0.01f);
	}

	// Token: 0x06002EF8 RID: 12024 RVA: 0x000B6423 File Offset: 0x000B4623
	public virtual void Zoom()
	{
		this.m_FinalZoomTime = Time.time + this.RenderingZoomDamping;
	}

	// Token: 0x06002EF9 RID: 12025 RVA: 0x00003296 File Offset: 0x00001496
	public virtual void SnapZoom()
	{
	}

	// Token: 0x06002EFA RID: 12026 RVA: 0x000B6438 File Offset: 0x000B4638
	protected virtual void UpdateShakes()
	{
		if (this.ShakeSpeed != 0f)
		{
			this.m_Shake = Vector3.Scale(vp_SmoothRandom.GetVector3Centered(this.ShakeSpeed), this.ShakeAmplitude);
			this.m_RotationSpring.AddForce(this.m_Shake * Time.timeScale);
		}
	}

	// Token: 0x06002EFB RID: 12027 RVA: 0x000B648C File Offset: 0x000B468C
	protected virtual void UpdateRetraction(bool firstIteration = true)
	{
		if (this.RetractionDistance == 0f)
		{
			return;
		}
		Vector3 vector = this.WeaponModel.transform.TransformPoint(this.RetractionOffset);
		Vector3 end = vector + this.WeaponModel.transform.forward * this.RetractionDistance;
		RaycastHit raycastHit;
		if (Physics.Linecast(vector, end, out raycastHit, -675375893) && !raycastHit.collider.isTrigger)
		{
			this.WeaponModel.transform.position = raycastHit.point - (raycastHit.point - vector).normalized * (this.RetractionDistance * 0.99f);
			this.WeaponModel.transform.localPosition = Vector3.forward * Mathf.Min(this.WeaponModel.transform.localPosition.z, 0f);
			return;
		}
		if (firstIteration && this.WeaponModel.transform.localPosition != Vector3.zero && this.WeaponModel != base.gameObject)
		{
			this.WeaponModel.transform.localPosition = Vector3.forward * Mathf.SmoothStep(this.WeaponModel.transform.localPosition.z, 0f, this.RetractionRelaxSpeed * Time.timeScale);
			this.UpdateRetraction(false);
		}
	}

	// Token: 0x06002EFC RID: 12028 RVA: 0x000B6600 File Offset: 0x000B4800
	protected virtual void UpdateBob()
	{
		if (this.BobAmplitude == Vector4.zero || this.BobRate == Vector4.zero)
		{
			return;
		}
		this.m_BobSpeed = ((this.BobRequireGroundContact && !this.Controller.isGrounded) ? 0f : this.Controller.velocity.sqrMagnitude);
		this.m_BobSpeed = Mathf.Min(this.m_BobSpeed * this.BobInputVelocityScale, this.BobMaxInputVelocity);
		this.m_BobSpeed = Mathf.Round(this.m_BobSpeed * 1000f) / 1000f;
		if (this.m_BobSpeed == 0f)
		{
			this.m_BobSpeed = Mathf.Min(this.m_LastBobSpeed * 0.93f, this.BobMaxInputVelocity);
		}
		this.m_CurrentBobAmp.x = this.m_BobSpeed * (this.BobAmplitude.x * -0.0001f);
		this.m_CurrentBobVal.x = Mathf.Cos(Time.time * (this.BobRate.x * 10f)) * this.m_CurrentBobAmp.x;
		this.m_CurrentBobAmp.y = this.m_BobSpeed * (this.BobAmplitude.y * 0.0001f);
		this.m_CurrentBobVal.y = Mathf.Cos(Time.time * (this.BobRate.y * 10f)) * this.m_CurrentBobAmp.y;
		this.m_CurrentBobAmp.z = this.m_BobSpeed * (this.BobAmplitude.z * 0.0001f);
		this.m_CurrentBobVal.z = Mathf.Cos(Time.time * (this.BobRate.z * 10f)) * this.m_CurrentBobAmp.z;
		this.m_CurrentBobAmp.w = this.m_BobSpeed * (this.BobAmplitude.w * 0.0001f);
		this.m_CurrentBobVal.w = Mathf.Cos(Time.time * (this.BobRate.w * 10f)) * this.m_CurrentBobAmp.w;
		this.m_RotationSpring.AddForce(this.m_CurrentBobVal * Time.timeScale);
		this.m_PositionSpring.AddForce(Vector3.forward * this.m_CurrentBobVal.w * Time.timeScale);
		this.m_LastBobSpeed = this.m_BobSpeed;
	}

	// Token: 0x06002EFD RID: 12029 RVA: 0x000B687C File Offset: 0x000B4A7C
	protected virtual void UpdateEarthQuake()
	{
		if (this.FPPlayer == null)
		{
			return;
		}
		if (!this.FPPlayer.CameraEarthQuake.Active)
		{
			return;
		}
		if (!this.Controller.isGrounded)
		{
			return;
		}
		Vector3 vector = this.FPPlayer.CameraEarthQuakeForce.Get();
		this.AddForce(new Vector3(0f, 0f, -vector.z * 0.015f), new Vector3(vector.y * 2f, -vector.x, vector.x * 2f));
	}

	// Token: 0x06002EFE RID: 12030 RVA: 0x000B6918 File Offset: 0x000B4B18
	protected override void UpdateSprings()
	{
		this.m_PositionSpring.FixedUpdate();
		this.m_PositionPivotSpring.FixedUpdate();
		this.m_RotationPivotSpring.FixedUpdate();
		this.m_RotationSpring.FixedUpdate();
		this.m_PositionSpring2.FixedUpdate();
		this.m_RotationSpring2.FixedUpdate();
	}

	// Token: 0x06002EFF RID: 12031 RVA: 0x000B6968 File Offset: 0x000B4B68
	private void UpdateLookDown()
	{
		if (!this.LookDownActive)
		{
			return;
		}
		if (this.FPPlayer.Rotation.Get().x < 0f && this.m_LookDownPitch == 0f && this.m_LookDownYaw == 0f)
		{
			return;
		}
		if (this.FPPlayer.Rotation.Get().x > 0f)
		{
			this.m_LookDownPitch = Mathf.Lerp(this.m_LookDownPitch, vp_MathUtility.SnapToZero(Mathf.Max(0f, this.FPPlayer.Rotation.Get().x / 90f), 0.0001f), Time.deltaTime * 2f);
			this.m_LookDownYaw = Mathf.Lerp(this.m_LookDownYaw, vp_MathUtility.SnapToZero(Mathf.DeltaAngle(this.FPPlayer.Rotation.Get().y, this.FPPlayer.BodyYaw.Get()), 0.0001f) / 90f * vp_MathUtility.SnapToZero(Mathf.Max(0f, (this.FPPlayer.Rotation.Get().x - this.LookDownYawLimit) / (90f - this.LookDownYawLimit)), 0.0001f), Time.deltaTime * 2f);
		}
		else
		{
			this.m_LookDownPitch *= 0.9f;
			this.m_LookDownYaw *= 0.9f;
			if (this.m_LookDownPitch < 0.01f)
			{
				this.m_LookDownPitch = 0f;
			}
			if (this.m_LookDownYaw < 0.01f)
			{
				this.m_LookDownYaw = 0f;
			}
		}
		this.m_WeaponGroupTransform.localPosition = vp_MathUtility.NaNSafeVector3(Vector3.Lerp(this.m_WeaponGroupTransform.localPosition, this.LookDownPositionOffsetMiddle, this.m_LookDownCurve.Evaluate(this.m_LookDownPitch)), default(Vector3));
		this.m_WeaponGroupTransform.localRotation = vp_MathUtility.NaNSafeQuaternion(Quaternion.Slerp(this.m_WeaponGroupTransform.localRotation, Quaternion.Euler(this.LookDownRotationOffsetMiddle), this.m_LookDownPitch), default(Quaternion));
		if (this.m_LookDownYaw > 0f)
		{
			this.m_WeaponGroupTransform.localPosition = vp_MathUtility.NaNSafeVector3(Vector3.Lerp(this.m_WeaponGroupTransform.localPosition, this.LookDownPositionOffsetLeft, Mathf.SmoothStep(0f, 1f, this.m_LookDownYaw)), default(Vector3));
			this.m_WeaponGroupTransform.localRotation = vp_MathUtility.NaNSafeQuaternion(Quaternion.Slerp(this.m_WeaponGroupTransform.localRotation, Quaternion.Euler(this.LookDownRotationOffsetLeft), this.m_LookDownYaw), default(Quaternion));
		}
		else
		{
			this.m_WeaponGroupTransform.localPosition = vp_MathUtility.NaNSafeVector3(Vector3.Lerp(this.m_WeaponGroupTransform.localPosition, this.LookDownPositionOffsetRight, Mathf.SmoothStep(0f, 1f, -this.m_LookDownYaw)), default(Vector3));
			this.m_WeaponGroupTransform.localRotation = vp_MathUtility.NaNSafeQuaternion(Quaternion.Slerp(this.m_WeaponGroupTransform.localRotation, Quaternion.Euler(this.LookDownRotationOffsetRight), -this.m_LookDownYaw), default(Quaternion));
		}
		this.m_CurrentPosRestState = Vector3.Lerp(this.m_CurrentPosRestState, this.m_PositionSpring.RestState, Time.fixedDeltaTime);
		this.m_CurrentRotRestState = Vector3.Lerp(this.m_CurrentRotRestState, this.m_RotationSpring.RestState, Time.fixedDeltaTime);
		this.m_WeaponGroupTransform.localPosition += vp_MathUtility.NaNSafeVector3((this.m_PositionSpring.State - this.m_CurrentPosRestState) * (this.m_LookDownPitch * this.LookDownPositionSpringPower), default(Vector3));
		this.m_WeaponGroupTransform.localEulerAngles -= vp_MathUtility.NaNSafeVector3(new Vector3(Mathf.DeltaAngle(this.m_RotationSpring.State.x, this.m_CurrentRotRestState.x), Mathf.DeltaAngle(this.m_RotationSpring.State.y, this.m_CurrentRotRestState.y), Mathf.DeltaAngle(this.m_RotationSpring.State.z, this.m_CurrentRotRestState.z)) * (this.m_LookDownPitch * this.LookDownRotationSpringPower), default(Vector3));
	}

	// Token: 0x06002F00 RID: 12032 RVA: 0x000B6DE0 File Offset: 0x000B4FE0
	protected virtual void UpdateStep()
	{
		if (this.StepMinVelocity <= 0f || (this.BobRequireGroundContact && !this.Controller.isGrounded) || this.Controller.velocity.sqrMagnitude < this.StepMinVelocity)
		{
			return;
		}
		bool flag = this.m_LastUpBob < this.m_CurrentBobVal.x;
		this.m_LastUpBob = this.m_CurrentBobVal.x;
		if (flag && !this.m_BobWasElevating)
		{
			if (Mathf.Cos(Time.time * (this.BobRate.x * 5f)) > 0f)
			{
				this.m_PosStep = this.StepPositionForce - this.StepPositionForce * this.StepPositionBalance;
				this.m_RotStep = this.StepRotationForce - this.StepPositionForce * this.StepRotationBalance;
			}
			else
			{
				this.m_PosStep = this.StepPositionForce + this.StepPositionForce * this.StepPositionBalance;
				this.m_RotStep = Vector3.Scale(this.StepRotationForce - this.StepPositionForce * this.StepRotationBalance, -Vector3.one + Vector3.right * 2f);
			}
			this.AddSoftForce(this.m_PosStep * this.StepForceScale, this.m_RotStep * this.StepForceScale, this.StepSoftness);
		}
		this.m_BobWasElevating = flag;
	}

	// Token: 0x06002F01 RID: 12033 RVA: 0x000B6F6C File Offset: 0x000B516C
	protected virtual void UpdateSwaying()
	{
		this.m_SwayVel = this.Controller.velocity * this.PositionInputVelocityScale;
		this.m_SwayVel = Vector3.Min(this.m_SwayVel, Vector3.one * this.PositionMaxInputVelocity);
		this.m_SwayVel = Vector3.Max(this.m_SwayVel, Vector3.one * -this.PositionMaxInputVelocity);
		this.m_SwayVel *= Time.timeScale;
		Vector3 vector = base.Transform.InverseTransformDirection(this.m_SwayVel / 60f);
		this.m_RotationSpring.AddForce(new Vector3(this.m_LookInput.y * (this.RotationLookSway.x * 0.025f), this.m_LookInput.x * (this.RotationLookSway.y * -0.025f), this.m_LookInput.x * (this.RotationLookSway.z * -0.025f)));
		this.m_FallSway = this.RotationFallSway * (this.m_SwayVel.y * 0.005f);
		if (this.Controller.isGrounded)
		{
			this.m_FallSway *= this.RotationSlopeSway;
		}
		this.m_FallSway.z = Mathf.Max(0f, this.m_FallSway.z);
		this.m_RotationSpring.AddForce(this.m_FallSway);
		this.m_PositionSpring.AddForce(Vector3.forward * -Mathf.Abs(this.m_SwayVel.y * (this.PositionFallRetract * 2.5E-05f)));
		this.m_PositionSpring.AddForce(new Vector3(vector.x * (this.PositionWalkSlide.x * 0.0016f), -Mathf.Abs(vector.x * (this.PositionWalkSlide.y * 0.0016f)), -vector.z * (this.PositionWalkSlide.z * 0.0016f)));
		this.m_RotationSpring.AddForce(new Vector3(-Mathf.Abs(vector.x * (this.RotationStrafeSway.x * 0.16f)), -(vector.x * (this.RotationStrafeSway.y * 0.16f)), vector.x * (this.RotationStrafeSway.z * 0.16f)));
	}

	// Token: 0x06002F02 RID: 12034 RVA: 0x000B71DC File Offset: 0x000B53DC
	public virtual void ResetSprings(float positionReset, float rotationReset, float positionPauseTime = 0f, float rotationPauseTime = 0f)
	{
		this.m_PositionSpring.State = Vector3.Lerp(this.m_PositionSpring.State, this.m_PositionSpring.RestState, positionReset);
		this.m_RotationSpring.State = Vector3.Lerp(this.m_RotationSpring.State, this.m_RotationSpring.RestState, rotationReset);
		this.m_PositionPivotSpring.State = Vector3.Lerp(this.m_PositionPivotSpring.State, this.m_PositionPivotSpring.RestState, positionReset);
		this.m_RotationPivotSpring.State = Vector3.Lerp(this.m_RotationPivotSpring.State, this.m_RotationPivotSpring.RestState, rotationReset);
		if (positionPauseTime != 0f)
		{
			this.m_PositionSpring.ForceVelocityFadeIn(positionPauseTime);
		}
		if (rotationPauseTime != 0f)
		{
			this.m_RotationSpring.ForceVelocityFadeIn(rotationPauseTime);
		}
		if (positionPauseTime != 0f)
		{
			this.m_PositionPivotSpring.ForceVelocityFadeIn(positionPauseTime);
		}
		if (rotationPauseTime != 0f)
		{
			this.m_RotationPivotSpring.ForceVelocityFadeIn(rotationPauseTime);
		}
	}

	// Token: 0x06002F03 RID: 12035 RVA: 0x000B72DC File Offset: 0x000B54DC
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
			this.m_PositionSpring.RestState = this.PositionOffset - this.PositionPivot;
		}
		if (this.m_PositionPivotSpring != null)
		{
			this.m_PositionPivotSpring.Stiffness = new Vector3(this.PositionPivotSpringStiffness, this.PositionPivotSpringStiffness, this.PositionPivotSpringStiffness);
			this.m_PositionPivotSpring.Damping = Vector3.one - new Vector3(this.PositionPivotSpringDamping, this.PositionPivotSpringDamping, this.PositionPivotSpringDamping);
			this.m_PositionPivotSpring.RestState = this.PositionPivot;
		}
		if (this.m_RotationPivotSpring != null)
		{
			this.m_RotationPivotSpring.Stiffness = new Vector3(this.RotationPivotSpringStiffness, this.RotationPivotSpringStiffness, this.RotationPivotSpringStiffness);
			this.m_RotationPivotSpring.Damping = Vector3.one - new Vector3(this.RotationPivotSpringDamping, this.RotationPivotSpringDamping, this.RotationPivotSpringDamping);
			this.m_RotationPivotSpring.RestState = this.RotationPivot;
		}
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.Stiffness = new Vector3(this.PositionSpring2Stiffness, this.PositionSpring2Stiffness, this.PositionSpring2Stiffness);
			this.m_PositionSpring2.Damping = Vector3.one - new Vector3(this.PositionSpring2Damping, this.PositionSpring2Damping, this.PositionSpring2Damping);
			this.m_PositionSpring2.RestState = Vector3.zero;
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.Stiffness = new Vector3(this.RotationSpringStiffness, this.RotationSpringStiffness, this.RotationSpringStiffness);
			this.m_RotationSpring.Damping = Vector3.one - new Vector3(this.RotationSpringDamping, this.RotationSpringDamping, this.RotationSpringDamping);
			this.m_RotationSpring.RestState = this.RotationOffset;
		}
		if (this.m_RotationSpring2 != null)
		{
			this.m_RotationSpring2.Stiffness = new Vector3(this.RotationSpring2Stiffness, this.RotationSpring2Stiffness, this.RotationSpring2Stiffness);
			this.m_RotationSpring2.Damping = Vector3.one - new Vector3(this.RotationSpring2Damping, this.RotationSpring2Damping, this.RotationSpring2Damping);
			this.m_RotationSpring2.RestState = Vector3.zero;
		}
		if (base.Rendering)
		{
			this.Zoom();
		}
	}

	// Token: 0x06002F04 RID: 12036 RVA: 0x000B7572 File Offset: 0x000B5772
	public override void Activate()
	{
		base.Activate();
		this.SnapZoom();
		if (this.m_WeaponGroup != null && !vp_Utility.IsActive(this.m_WeaponGroup))
		{
			vp_Utility.Activate(this.m_WeaponGroup, true);
		}
		this.SetPivotVisible(false);
	}

	// Token: 0x06002F05 RID: 12037 RVA: 0x000B75AE File Offset: 0x000B57AE
	public override void Deactivate()
	{
		this.m_Wielded = false;
		if (this.m_WeaponGroup != null && vp_Utility.IsActive(this.m_WeaponGroup))
		{
			vp_Utility.Activate(this.m_WeaponGroup, false);
		}
	}

	// Token: 0x06002F06 RID: 12038 RVA: 0x000B75E0 File Offset: 0x000B57E0
	public virtual void SnapPivot()
	{
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.RestState = this.PositionOffset - this.PositionPivot;
			this.m_PositionSpring.State = this.PositionOffset - this.PositionPivot;
		}
		if (this.m_WeaponGroup != null)
		{
			this.m_WeaponGroupTransform.localPosition = this.PositionOffset - this.PositionPivot;
		}
		if (this.m_PositionPivotSpring != null)
		{
			this.m_PositionPivotSpring.RestState = this.PositionPivot;
			this.m_PositionPivotSpring.State = this.PositionPivot;
		}
		if (this.m_RotationPivotSpring != null)
		{
			this.m_RotationPivotSpring.RestState = this.RotationPivot;
			this.m_RotationPivotSpring.State = this.RotationPivot;
		}
		base.Transform.localPosition = this.PositionPivot;
		base.Transform.localEulerAngles = this.RotationPivot;
	}

	// Token: 0x06002F07 RID: 12039 RVA: 0x000B76CD File Offset: 0x000B58CD
	public virtual void SetPivotVisible(bool visible)
	{
		if (this.m_Pivot == null)
		{
			return;
		}
		vp_Utility.Activate(this.m_Pivot.gameObject, visible);
	}

	// Token: 0x06002F08 RID: 12040 RVA: 0x000B76EF File Offset: 0x000B58EF
	public virtual void SnapToExit()
	{
		this.RotationOffset = this.RotationExitOffset;
		this.PositionOffset = this.PositionExitOffset;
		this.SnapSprings();
		this.SnapPivot();
	}

	// Token: 0x06002F09 RID: 12041 RVA: 0x000B7718 File Offset: 0x000B5918
	public override void SnapSprings()
	{
		base.SnapSprings();
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.RestState = this.PositionOffset - this.PositionPivot;
			this.m_PositionSpring.State = this.PositionOffset - this.PositionPivot;
			this.m_PositionSpring.Stop(true);
		}
		if (this.m_WeaponGroup != null)
		{
			this.m_WeaponGroupTransform.localPosition = this.PositionOffset - this.PositionPivot;
		}
		if (this.m_PositionPivotSpring != null)
		{
			this.m_PositionPivotSpring.RestState = this.PositionPivot;
			this.m_PositionPivotSpring.State = this.PositionPivot;
			this.m_PositionPivotSpring.Stop(true);
		}
		base.Transform.localPosition = this.PositionPivot;
		if (this.m_RotationPivotSpring != null)
		{
			this.m_RotationPivotSpring.RestState = this.RotationPivot;
			this.m_RotationPivotSpring.State = this.RotationPivot;
			this.m_RotationPivotSpring.Stop(true);
		}
		base.Transform.localEulerAngles = this.RotationPivot;
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.RestState = this.RotationOffset;
			this.m_RotationSpring.State = this.RotationOffset;
			this.m_RotationSpring.Stop(true);
		}
	}

	// Token: 0x06002F0A RID: 12042 RVA: 0x000B7868 File Offset: 0x000B5A68
	public override void StopSprings()
	{
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.Stop(true);
		}
		if (this.m_PositionPivotSpring != null)
		{
			this.m_PositionPivotSpring.Stop(true);
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.Stop(true);
		}
		if (this.m_RotationPivotSpring != null)
		{
			this.m_RotationPivotSpring.Stop(true);
		}
	}

	// Token: 0x06002F0B RID: 12043 RVA: 0x000B78C8 File Offset: 0x000B5AC8
	public override void Wield(bool isWielding = true)
	{
		if (isWielding)
		{
			this.SnapToExit();
		}
		this.PositionOffset = (isWielding ? this.DefaultPosition : this.PositionExitOffset);
		this.RotationOffset = (isWielding ? this.DefaultRotation : this.RotationExitOffset);
		this.m_Wielded = isWielding;
		this.Refresh();
		base.StateManager.CombineStates();
		if (base.Audio != null && (isWielding ? this.SoundWield : this.SoundUnWield) != null && vp_Utility.IsActive(base.gameObject))
		{
			base.Audio.pitch = Time.timeScale;
			base.Audio.PlayOneShot(isWielding ? this.SoundWield : this.SoundUnWield);
		}
		if ((isWielding ? this.AnimationWield : this.AnimationUnWield) != null && vp_Utility.IsActive(base.gameObject))
		{
			if (isWielding)
			{
				this.m_WeaponModel.GetComponent<Animation>().CrossFade(this.AnimationWield.name);
				return;
			}
			this.m_WeaponModel.GetComponent<Animation>().CrossFade(this.AnimationUnWield.name);
		}
	}

	// Token: 0x06002F0C RID: 12044 RVA: 0x000B79E4 File Offset: 0x000B5BE4
	public virtual void ScheduleAmbientAnimation()
	{
		if (this.AnimationAmbient.Count == 0 || !vp_Utility.IsActive(base.gameObject))
		{
			return;
		}
		vp_Timer.In(UnityEngine.Random.Range(this.AmbientInterval.x, this.AmbientInterval.y), delegate()
		{
			if (vp_Utility.IsActive(base.gameObject))
			{
				this.m_CurrentAmbientAnimation = UnityEngine.Random.Range(0, this.AnimationAmbient.Count);
				if (this.AnimationAmbient[this.m_CurrentAmbientAnimation] != null)
				{
					this.m_WeaponModel.GetComponent<Animation>().CrossFadeQueued(this.AnimationAmbient[this.m_CurrentAmbientAnimation].name);
					this.ScheduleAmbientAnimation();
				}
			}
		}, this.m_AnimationAmbientTimer);
	}

	// Token: 0x06002F0D RID: 12045 RVA: 0x000B7A40 File Offset: 0x000B5C40
	protected virtual void OnMessage_FallImpact(float impact)
	{
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.AddSoftForce(Vector3.down * impact * this.PositionKneeling, (float)this.PositionKneelingSoftness);
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.AddSoftForce(Vector3.right * impact * this.RotationKneeling, (float)this.RotationKneelingSoftness);
		}
	}

	// Token: 0x06002F0E RID: 12046 RVA: 0x000B7AAD File Offset: 0x000B5CAD
	protected virtual void OnMessage_HeadImpact(float impact)
	{
		this.AddForce(Vector3.zero, Vector3.forward * (impact * 20f) * Time.timeScale);
	}

	// Token: 0x06002F0F RID: 12047 RVA: 0x000B7AD5 File Offset: 0x000B5CD5
	protected virtual void OnMessage_CameraGroundStomp(float impact)
	{
		this.AddForce(Vector3.zero, new Vector3(-0.25f, 0f, 0f) * impact);
	}

	// Token: 0x06002F10 RID: 12048 RVA: 0x000B7AFC File Offset: 0x000B5CFC
	protected virtual void OnMessage_CameraBombShake(float impact)
	{
		this.AddForce(Vector3.zero, new Vector3(-0.3f, 0.1f, 0.5f) * impact);
	}

	// Token: 0x06002F11 RID: 12049 RVA: 0x000B7B23 File Offset: 0x000B5D23
	protected virtual void OnMessage_CameraToggle3rdPerson()
	{
		this.RefreshWeaponModel();
	}

	// Token: 0x06002F12 RID: 12050 RVA: 0x000B7B2C File Offset: 0x000B5D2C
	protected override Vector3 Get_AimDirection()
	{
		if (this.FPPlayer.IsFirstPerson.Get())
		{
			return this.FPPlayer.HeadLookDirection.Get();
		}
		if (this.Weapon3rdPersonModel == null)
		{
			return this.FPPlayer.HeadLookDirection.Get();
		}
		return (this.Weapon3rdPersonModel.transform.position - this.FPPlayer.LookPoint.Get()).normalized;
	}

	// Token: 0x17000318 RID: 792
	// (get) Token: 0x06002F13 RID: 12051 RVA: 0x000B7BBC File Offset: 0x000B5DBC
	protected override Vector3 OnValue_AimDirection
	{
		get
		{
			if (this.FPPlayer.IsFirstPerson.Get())
			{
				return this.FPPlayer.HeadLookDirection.Get();
			}
			if (this.Weapon3rdPersonModel == null)
			{
				return this.FPPlayer.HeadLookDirection.Get();
			}
			return (this.Weapon3rdPersonModel.transform.position - this.FPPlayer.LookPoint.Get()).normalized;
		}
	}

	// Token: 0x06002F14 RID: 12052 RVA: 0x000B7C4C File Offset: 0x000B5E4C
	public override void Register(vp_EventHandler eventHandler)
	{
		base.Register(eventHandler);
		eventHandler.RegisterMessage<float>("CameraBombShake", new vp_Message<float>.Sender<float>(this.OnMessage_CameraBombShake));
		eventHandler.RegisterMessage<float>("CameraGroundStomp", new vp_Message<float>.Sender<float>(this.OnMessage_CameraGroundStomp));
		eventHandler.RegisterMessage("CameraToggle3rdPerson", new vp_Message.Sender(this.OnMessage_CameraToggle3rdPerson));
		eventHandler.RegisterMessage<float>("FallImpact", new vp_Message<float>.Sender<float>(this.OnMessage_FallImpact));
		eventHandler.RegisterMessage<float>("HeadImpact", new vp_Message<float>.Sender<float>(this.OnMessage_HeadImpact));
	}

	// Token: 0x06002F15 RID: 12053 RVA: 0x000B7CD8 File Offset: 0x000B5ED8
	public override void Unregister(vp_EventHandler eventHandler)
	{
		base.Unregister(eventHandler);
		eventHandler.UnregisterMessage<float>("CameraBombShake", new vp_Message<float>.Sender<float>(this.OnMessage_CameraBombShake));
		eventHandler.UnregisterMessage<float>("CameraGroundStomp", new vp_Message<float>.Sender<float>(this.OnMessage_CameraGroundStomp));
		eventHandler.UnregisterMessage("CameraToggle3rdPerson", new vp_Message.Sender(this.OnMessage_CameraToggle3rdPerson));
		eventHandler.UnregisterMessage<float>("FallImpact", new vp_Message<float>.Sender<float>(this.OnMessage_FallImpact));
		eventHandler.UnregisterMessage<float>("HeadImpact", new vp_Message<float>.Sender<float>(this.OnMessage_HeadImpact));
	}

	// Token: 0x06002F16 RID: 12054 RVA: 0x000B7D64 File Offset: 0x000B5F64
	protected override StateManager GetStateManager()
	{
		return new FPWeaponStateManager(this);
	}

	// Token: 0x04002CB6 RID: 11446
	public GameObject WeaponPrefab;

	// Token: 0x04002CB7 RID: 11447
	protected CharacterController Controller;

	// Token: 0x04002CB8 RID: 11448
	public float RenderingZoomDamping = 0.5f;

	// Token: 0x04002CB9 RID: 11449
	protected float m_FinalZoomTime;

	// Token: 0x04002CBA RID: 11450
	public float RenderingZScale = 1f;

	// Token: 0x04002CBB RID: 11451
	public float PositionSpringStiffness = 0.01f;

	// Token: 0x04002CBC RID: 11452
	public float PositionSpringDamping = 0.25f;

	// Token: 0x04002CBD RID: 11453
	public float PositionFallRetract = 1f;

	// Token: 0x04002CBE RID: 11454
	public float PositionPivotSpringStiffness = 0.01f;

	// Token: 0x04002CBF RID: 11455
	public float PositionPivotSpringDamping = 0.25f;

	// Token: 0x04002CC0 RID: 11456
	public float PositionKneeling = 0.06f;

	// Token: 0x04002CC1 RID: 11457
	public int PositionKneelingSoftness = 1;

	// Token: 0x04002CC2 RID: 11458
	public Vector3 PositionWalkSlide = new Vector3(0.5f, 0.75f, 0.5f);

	// Token: 0x04002CC3 RID: 11459
	public Vector3 PositionPivot = Vector3.zero;

	// Token: 0x04002CC4 RID: 11460
	public Vector3 RotationPivot = Vector3.zero;

	// Token: 0x04002CC5 RID: 11461
	public float PositionInputVelocityScale = 1f;

	// Token: 0x04002CC6 RID: 11462
	public float PositionMaxInputVelocity = 25f;

	// Token: 0x04002CC7 RID: 11463
	protected vp_Spring m_PositionSpring;

	// Token: 0x04002CC8 RID: 11464
	protected vp_Spring m_PositionPivotSpring;

	// Token: 0x04002CC9 RID: 11465
	protected vp_Spring m_RotationPivotSpring;

	// Token: 0x04002CCA RID: 11466
	protected GameObject m_WeaponCamera;

	// Token: 0x04002CCB RID: 11467
	protected GameObject m_WeaponGroup;

	// Token: 0x04002CCC RID: 11468
	protected GameObject m_Pivot;

	// Token: 0x04002CCD RID: 11469
	protected Transform m_WeaponGroupTransform;

	// Token: 0x04002CCE RID: 11470
	public float RotationSpringStiffness = 0.01f;

	// Token: 0x04002CCF RID: 11471
	public float RotationSpringDamping = 0.25f;

	// Token: 0x04002CD0 RID: 11472
	public float RotationPivotSpringStiffness = 0.01f;

	// Token: 0x04002CD1 RID: 11473
	public float RotationPivotSpringDamping = 0.25f;

	// Token: 0x04002CD2 RID: 11474
	public float RotationKneeling;

	// Token: 0x04002CD3 RID: 11475
	public int RotationKneelingSoftness = 1;

	// Token: 0x04002CD4 RID: 11476
	public Vector3 RotationLookSway = new Vector3(1f, 0.7f, 0f);

	// Token: 0x04002CD5 RID: 11477
	public Vector3 RotationStrafeSway = new Vector3(0.3f, 1f, 1.5f);

	// Token: 0x04002CD6 RID: 11478
	public Vector3 RotationFallSway = new Vector3(1f, -0.5f, -3f);

	// Token: 0x04002CD7 RID: 11479
	public float RotationSlopeSway = 0.5f;

	// Token: 0x04002CD8 RID: 11480
	public float RotationInputVelocityScale = 1f;

	// Token: 0x04002CD9 RID: 11481
	public float RotationMaxInputVelocity = 15f;

	// Token: 0x04002CDA RID: 11482
	protected vp_Spring m_RotationSpring;

	// Token: 0x04002CDB RID: 11483
	protected Vector3 m_SwayVel = Vector3.zero;

	// Token: 0x04002CDC RID: 11484
	protected Vector3 m_FallSway = Vector3.zero;

	// Token: 0x04002CDD RID: 11485
	public float RetractionDistance;

	// Token: 0x04002CDE RID: 11486
	public Vector2 RetractionOffset = new Vector2(0f, 0f);

	// Token: 0x04002CDF RID: 11487
	public float RetractionRelaxSpeed = 0.25f;

	// Token: 0x04002CE0 RID: 11488
	protected bool m_DrawRetractionDebugLine;

	// Token: 0x04002CE1 RID: 11489
	public float ShakeSpeed = 0.05f;

	// Token: 0x04002CE2 RID: 11490
	public Vector3 ShakeAmplitude = new Vector3(0.25f, 0f, 2f);

	// Token: 0x04002CE3 RID: 11491
	protected Vector3 m_Shake = Vector3.zero;

	// Token: 0x04002CE4 RID: 11492
	public Vector4 BobRate = new Vector4(0.9f, 0.45f, 0f, 0f);

	// Token: 0x04002CE5 RID: 11493
	public Vector4 BobAmplitude = new Vector4(0.35f, 0.5f, 0f, 0f);

	// Token: 0x04002CE6 RID: 11494
	public float BobInputVelocityScale = 1f;

	// Token: 0x04002CE7 RID: 11495
	public float BobMaxInputVelocity = 100f;

	// Token: 0x04002CE8 RID: 11496
	public bool BobRequireGroundContact = true;

	// Token: 0x04002CE9 RID: 11497
	protected float m_LastBobSpeed;

	// Token: 0x04002CEA RID: 11498
	protected Vector4 m_CurrentBobAmp = Vector4.zero;

	// Token: 0x04002CEB RID: 11499
	protected Vector4 m_CurrentBobVal = Vector4.zero;

	// Token: 0x04002CEC RID: 11500
	protected float m_BobSpeed;

	// Token: 0x04002CED RID: 11501
	public Vector3 StepPositionForce = new Vector3(0f, -0.0012f, -0.0012f);

	// Token: 0x04002CEE RID: 11502
	public Vector3 StepRotationForce = new Vector3(0f, 0f, 0f);

	// Token: 0x04002CEF RID: 11503
	public int StepSoftness = 4;

	// Token: 0x04002CF0 RID: 11504
	public float StepMinVelocity;

	// Token: 0x04002CF1 RID: 11505
	public float StepPositionBalance;

	// Token: 0x04002CF2 RID: 11506
	public float StepRotationBalance;

	// Token: 0x04002CF3 RID: 11507
	public float StepForceScale = 1f;

	// Token: 0x04002CF4 RID: 11508
	protected float m_LastUpBob;

	// Token: 0x04002CF5 RID: 11509
	protected bool m_BobWasElevating;

	// Token: 0x04002CF6 RID: 11510
	protected Vector3 m_PosStep = Vector3.zero;

	// Token: 0x04002CF7 RID: 11511
	protected Vector3 m_RotStep = Vector3.zero;

	// Token: 0x04002CF8 RID: 11512
	public bool LookDownActive;

	// Token: 0x04002CF9 RID: 11513
	public float LookDownYawLimit = 60f;

	// Token: 0x04002CFA RID: 11514
	public Vector3 LookDownPositionOffsetMiddle = new Vector3(0.32f, -0.37f, 0.78f);

	// Token: 0x04002CFB RID: 11515
	public Vector3 LookDownPositionOffsetLeft = new Vector3(0.27f, -0.31f, 0.7f);

	// Token: 0x04002CFC RID: 11516
	public Vector3 LookDownPositionOffsetRight = new Vector3(0.6f, -0.41f, 0.86f);

	// Token: 0x04002CFD RID: 11517
	public float LookDownPositionSpringPower = 1f;

	// Token: 0x04002CFE RID: 11518
	public Vector3 LookDownRotationOffsetMiddle = new Vector3(-3.9f, 2.24f, 4.69f);

	// Token: 0x04002CFF RID: 11519
	public Vector3 LookDownRotationOffsetLeft = new Vector3(-7f, -10.5f, 15.6f);

	// Token: 0x04002D00 RID: 11520
	public Vector3 LookDownRotationOffsetRight = new Vector3(-9.2f, -9.8f, 48.84f);

	// Token: 0x04002D01 RID: 11521
	public float LookDownRotationSpringPower = 1f;

	// Token: 0x04002D02 RID: 11522
	protected Vector3 m_CurrentPosRestState = Vector3.zero;

	// Token: 0x04002D03 RID: 11523
	protected Vector3 m_CurrentRotRestState = Vector3.zero;

	// Token: 0x04002D04 RID: 11524
	protected AnimationCurve m_LookDownCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.8f, 0.2f, 0.9f, 1.5f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04002D05 RID: 11525
	protected float m_LookDownPitch;

	// Token: 0x04002D06 RID: 11526
	protected float m_LookDownYaw;

	// Token: 0x04002D07 RID: 11527
	public AudioClip SoundWield;

	// Token: 0x04002D08 RID: 11528
	public AudioClip SoundUnWield;

	// Token: 0x04002D09 RID: 11529
	public AnimationClip AnimationWield;

	// Token: 0x04002D0A RID: 11530
	public AnimationClip AnimationUnWield;

	// Token: 0x04002D0B RID: 11531
	public List<UnityEngine.Object> AnimationAmbient = new List<UnityEngine.Object>();

	// Token: 0x04002D0C RID: 11532
	protected List<bool> m_AmbAnimPlayed = new List<bool>();

	// Token: 0x04002D0D RID: 11533
	public Vector2 AmbientInterval = new Vector2(2.5f, 7.5f);

	// Token: 0x04002D0E RID: 11534
	protected int m_CurrentAmbientAnimation;

	// Token: 0x04002D0F RID: 11535
	protected vp_Timer.Handle m_AnimationAmbientTimer = new vp_Timer.Handle();

	// Token: 0x04002D10 RID: 11536
	public Vector3 PositionExitOffset = new Vector3(0f, -1f, 0f);

	// Token: 0x04002D11 RID: 11537
	public Vector3 RotationExitOffset = new Vector3(40f, 0f, 0f);

	// Token: 0x04002D12 RID: 11538
	protected Vector2 m_LookInput = Vector2.zero;

	// Token: 0x04002D13 RID: 11539
	protected const float LOOKDOWNSPEED = 2f;

	// Token: 0x04002D14 RID: 11540
	private vp_FPPlayerEventHandler m_FPPlayer;
}
