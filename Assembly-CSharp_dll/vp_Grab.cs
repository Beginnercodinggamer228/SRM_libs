using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000869 RID: 2153
[RequireComponent(typeof(Rigidbody))]
public class vp_Grab : vp_Interactable, EventHandlerRegistrable
{
	// Token: 0x170002E1 RID: 737
	// (get) Token: 0x06002D7C RID: 11644 RVA: 0x000AD8FE File Offset: 0x000ABAFE
	private vp_FPPlayerEventHandler FPPlayer
	{
		get
		{
			if (this.m_FPPlayer == null)
			{
				this.m_FPPlayer = (this.m_Player as vp_FPPlayerEventHandler);
			}
			return this.m_FPPlayer;
		}
	}

	// Token: 0x06002D7D RID: 11645 RVA: 0x000AD928 File Offset: 0x000ABB28
	protected override void Start()
	{
		base.Start();
		if (!base.GetComponent<Rigidbody>() || !base.GetComponent<Collider>())
		{
			base.enabled = false;
		}
		if (base.GetComponent<Rigidbody>() != null)
		{
			this.m_DefaultGravity = base.GetComponent<Rigidbody>().useGravity;
			this.m_DefaultDrag = this.m_Transform.GetComponent<Rigidbody>().drag;
			this.m_DefaultAngularDrag = this.m_Transform.GetComponent<Rigidbody>().angularDrag;
		}
		this.InteractType = vp_Interactable.vp_InteractType.Normal;
		this.m_InteractManager = (UnityEngine.Object.FindObjectOfType(typeof(vp_FPInteractManager)) as vp_FPInteractManager);
	}

	// Token: 0x06002D7E RID: 11646 RVA: 0x000AD9C8 File Offset: 0x000ABBC8
	protected virtual void FixedUpdate()
	{
		if (!this.m_IsGrabbed || this.m_Transform.parent == null)
		{
			return;
		}
		this.UpdateShake();
		this.UpdatePosition();
		this.UpdateRotation();
		this.UpdateBurden();
		this.DampenForces();
	}

	// Token: 0x06002D7F RID: 11647 RVA: 0x000ADA04 File Offset: 0x000ABC04
	protected virtual void Update()
	{
		if (!this.m_IsGrabbed || this.m_Transform.parent == null)
		{
			return;
		}
		this.UpdateInput();
	}

	// Token: 0x06002D80 RID: 11648 RVA: 0x000ADA28 File Offset: 0x000ABC28
	protected virtual void UpdateInput()
	{
		this.m_CurrentMouseMove.x = this.FPPlayer.InputRawLook.Get().x * Time.timeScale;
		this.m_CurrentMouseMove.y = this.FPPlayer.InputRawLook.Get().y * Time.timeScale;
		if (this.FPPlayer.InputGetButtonDown.Send("Attack"))
		{
			this.FPPlayer.Interact.TryStart(true);
			return;
		}
		if (this.m_Player.CurrentWeaponIndex.Get() != 0)
		{
			this.m_Player.SetWeapon.TryStart<int>(0);
			return;
		}
	}

	// Token: 0x06002D81 RID: 11649 RVA: 0x000ADAE4 File Offset: 0x000ABCE4
	protected virtual void UpdateShake()
	{
		this.m_CurrentShake = Vector3.Scale(vp_SmoothRandom.GetVector3Centered(this.ShakeSpeed), this.ShakeAmplitude);
		this.m_Transform.localEulerAngles += this.m_CurrentShake;
	}

	// Token: 0x06002D82 RID: 11650 RVA: 0x000ADB20 File Offset: 0x000ABD20
	protected virtual void UpdatePosition()
	{
		this.m_CurrentSwayForce += this.m_Player.Velocity.Get() * 0.005f;
		this.m_CurrentSwayForce.y = this.m_CurrentSwayForce.y + this.m_CurrentFootstepForce;
		this.m_CurrentSwayForce += this.m_Camera.Transform.TransformDirection(new Vector3(this.m_CurrentMouseMove.x * 0.05f, (this.m_Player.Rotation.Get().x > this.m_Camera.RotationPitchLimit.y) ? (this.m_CurrentMouseMove.y * 0.015f) : (this.m_CurrentMouseMove.y * 0.05f), 0f));
		this.m_TempCarryingOffset = (this.m_Player.IsFirstPerson.Get() ? this.CarryingOffset : (this.CarryingOffset - this.m_Camera.Position3rdPersonOffset));
		this.m_Transform.position = Vector3.Lerp(this.m_Transform.position, this.m_Camera.Transform.position - this.m_CurrentSwayForce + this.m_Camera.Transform.right * this.m_TempCarryingOffset.x + this.m_Camera.Transform.up * this.m_Transform.localScale.y * (this.m_TempCarryingOffset.y + this.m_CurrentShake.y * 0.5f) + this.m_Camera.Transform.forward * this.m_TempCarryingOffset.z, (this.m_FetchProgress < 1f) ? this.m_FetchProgress : (Time.deltaTime * (this.Stiffness * 60f)));
	}

	// Token: 0x06002D83 RID: 11651 RVA: 0x000ADD2C File Offset: 0x000ABF2C
	protected virtual void UpdateRotation()
	{
		this.m_Camera.RotationPitchLimit = Vector2.Lerp(this.m_Camera.RotationPitchLimit, new Vector2(this.m_Camera.RotationPitchLimit.x, this.CameraPitchDownLimit), this.m_FetchProgress);
		if (this.m_DisableAngleSwayTimer.Active)
		{
			return;
		}
		this.m_CurrentSwayTorque += this.m_Player.Velocity.Get() * 0.005f;
		this.m_CurrentRotationSway = this.m_Camera.Transform.InverseTransformDirection(this.m_CurrentSwayTorque * 1.5f);
		this.m_CurrentRotationSway.y = this.m_CurrentRotationSway.z;
		this.m_CurrentRotationSway.z = this.m_CurrentRotationSway.x;
		this.m_CurrentRotationSway.x = -this.m_CurrentRotationSway.y * 0.5f;
		this.m_CurrentRotationSway.y = 0f;
		Quaternion localRotation = this.m_Transform.localRotation;
		this.m_Transform.Rotate(this.m_Camera.transform.forward, this.m_CurrentRotationSway.z * -0.5f * Time.timeScale);
		this.m_Transform.Rotate(this.m_Camera.transform.right, this.m_CurrentRotationSway.x * -0.5f * Time.timeScale);
		Quaternion localRotation2 = this.m_Transform.localRotation;
		this.m_Transform.localRotation = localRotation;
		this.m_Transform.localRotation = Quaternion.Slerp(localRotation2, Quaternion.Euler(this.m_CurrentHoldAngle + this.m_CurrentShake * 50f), Time.deltaTime * (this.Stiffness * 60f));
	}

	// Token: 0x06002D84 RID: 11652 RVA: 0x000ADF00 File Offset: 0x000AC100
	protected virtual void UpdateBurden()
	{
		if (this.Burden <= 0f)
		{
			return;
		}
		this.m_Player.MotorThrottle.Set(this.m_Player.MotorThrottle.Get() * (1f - Mathf.Clamp01(this.Burden)));
	}

	// Token: 0x06002D85 RID: 11653 RVA: 0x000ADF5B File Offset: 0x000AC15B
	protected virtual void DampenForces()
	{
		this.m_CurrentSwayForce *= 0.9f;
		this.m_CurrentSwayTorque *= 0.9f;
		this.m_CurrentFootstepForce *= 0.9f;
	}

	// Token: 0x06002D86 RID: 11654 RVA: 0x000ADF9C File Offset: 0x000AC19C
	public override bool TryInteract(vp_PlayerEventHandler player)
	{
		if (!(player is vp_FPPlayerEventHandler))
		{
			return false;
		}
		if (this.m_Player == null)
		{
			this.m_Player = player;
		}
		if (player == null)
		{
			return false;
		}
		if (this.m_Controller == null)
		{
			this.m_Controller = this.m_Player.GetComponent<vp_FPController>();
		}
		if (this.m_Controller == null)
		{
			return false;
		}
		if (this.m_Camera == null)
		{
			this.m_Camera = this.m_Player.GetComponentInChildren<vp_FPCamera>();
		}
		if (this.m_Camera == null)
		{
			return false;
		}
		if (this.m_WeaponHandler == null)
		{
			this.m_WeaponHandler = this.m_Player.GetComponentInChildren<vp_WeaponHandler>();
		}
		if (this.m_Audio == null)
		{
			this.m_Audio = this.m_Player.GetComponent<AudioSource>();
		}
		this.Register(this.m_Player);
		if (!this.m_IsGrabbed)
		{
			this.StartGrab();
		}
		else
		{
			this.StopGrab();
		}
		this.m_Player.Interactable.Set(this);
		if (this.GrabStateCrosshair != null)
		{
			this.FPPlayer.Crosshair.Set(this.GrabStateCrosshair);
		}
		else
		{
			this.FPPlayer.Crosshair.Set(new Texture2D(0, 0));
		}
		return true;
	}

	// Token: 0x06002D87 RID: 11655 RVA: 0x000AE0F0 File Offset: 0x000AC2F0
	protected virtual void StartGrab()
	{
		vp_AudioUtility.PlayRandomSound(this.m_Audio, this.GrabSounds, this.SoundsPitch);
		if (!string.IsNullOrEmpty(this.OnGrabText))
		{
			this.FPPlayer.HUDText.Send(this.OnGrabText);
		}
		vp_FPCamera camera = this.m_Camera;
		camera.BobStepCallback = (vp_FPCamera.BobStepDelegate)Delegate.Combine(camera.BobStepCallback, new vp_FPCamera.BobStepDelegate(this.Footstep));
		this.m_LastWeaponEquipped = this.m_Player.CurrentWeaponIndex.Get();
		this.m_OriginalPitchDownLimit = this.m_Camera.RotationPitchLimit.y;
		this.m_FetchProgress = 0f;
		if (this.m_LastWeaponEquipped != 0)
		{
			this.m_Player.SetWeapon.TryStart<int>(0);
		}
		else if (!this.m_IsFetching)
		{
			base.StartCoroutine("Fetch");
		}
		if (this.m_Transform.GetComponent<Rigidbody>() != null)
		{
			this.m_Transform.GetComponent<Rigidbody>().useGravity = false;
			this.m_Transform.GetComponent<Rigidbody>().drag = this.Stiffness * 60f;
			this.m_Transform.GetComponent<Rigidbody>().angularDrag = this.Stiffness * 60f;
		}
		if (this.m_Controller.Transform.GetComponent<Collider>().enabled && this.m_Transform.GetComponent<Collider>().enabled)
		{
			Log.Error("UFPS Ignoring Collider 3", Array.Empty<object>());
			Physics.IgnoreCollision(this.m_Controller.Transform.GetComponent<Collider>(), this.m_Transform.GetComponent<Collider>(), true);
		}
		this.m_Transform.parent = this.m_Camera.Transform;
		this.m_CurrentHoldAngle = this.m_Transform.localEulerAngles;
		this.m_IsGrabbed = true;
	}

	// Token: 0x06002D88 RID: 11656 RVA: 0x000AE2B4 File Offset: 0x000AC4B4
	protected virtual void StopGrab()
	{
		this.m_IsGrabbed = false;
		this.m_FetchProgress = 1f;
		vp_FPCamera camera = this.m_Camera;
		camera.BobStepCallback = (vp_FPCamera.BobStepDelegate)Delegate.Remove(camera.BobStepCallback, new vp_FPCamera.BobStepDelegate(this.Footstep));
		this.m_Player.SetWeapon.TryStart<int>(this.m_LastWeaponEquipped);
		if (this.m_Transform.GetComponent<Rigidbody>() != null)
		{
			this.m_Transform.GetComponent<Rigidbody>().useGravity = this.m_DefaultGravity;
			this.m_Transform.GetComponent<Rigidbody>().drag = this.m_DefaultDrag;
			this.m_Transform.GetComponent<Rigidbody>().angularDrag = this.m_DefaultAngularDrag;
		}
		if (!this.m_Player.Dead.Active && vp_Utility.IsActive(this.m_Transform.gameObject) && this.m_Controller.Transform.GetComponent<Collider>().enabled && this.m_Transform.GetComponent<Collider>().enabled)
		{
			Log.Error("UFPS Ignoring Collider 4", Array.Empty<object>());
			Physics.IgnoreCollision(this.m_Controller.Transform.GetComponent<Collider>(), this.m_Transform.GetComponent<Collider>(), false);
		}
		Vector3 eulerAngles = this.m_Transform.eulerAngles;
		this.m_Transform.parent = null;
		this.m_Transform.eulerAngles = eulerAngles;
		if (this.m_Transform.GetComponent<Rigidbody>() != null)
		{
			this.m_Transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
			this.m_Transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		}
		if (this.FPPlayer.InputGetButtonDown.Send("Attack"))
		{
			vp_AudioUtility.PlayRandomSound(this.m_Audio, this.ThrowSounds, this.SoundsPitch);
			if (this.m_Transform.GetComponent<Rigidbody>() != null)
			{
				this.m_Transform.GetComponent<Rigidbody>().AddForce(this.m_Player.Velocity.Get() + this.FPPlayer.CameraLookDirection.Get() * this.ThrowStrength, ForceMode.Impulse);
				if (this.AllowThrowRotation)
				{
					this.m_Transform.GetComponent<Rigidbody>().AddTorque(this.m_Camera.Transform.forward * ((UnityEngine.Random.value > 0.5f) ? 0.5f : -0.5f) + this.m_Camera.Transform.right * ((UnityEngine.Random.value > 0.5f) ? 0.5f : -0.5f), ForceMode.Impulse);
				}
			}
		}
		else
		{
			vp_AudioUtility.PlayRandomSound(this.m_Audio, this.DropSounds, this.SoundsPitch);
			if (this.m_Transform.GetComponent<Rigidbody>() != null)
			{
				this.m_Transform.GetComponent<Rigidbody>().AddForce(this.m_Player.Velocity.Get() + this.FPPlayer.CameraLookDirection.Get(), ForceMode.Impulse);
			}
		}
		if (this.m_InteractManager == null)
		{
			this.m_InteractManager = (UnityEngine.Object.FindObjectOfType(typeof(vp_FPInteractManager)) as vp_FPInteractManager);
		}
		this.m_InteractManager.CrosshairTimeoutTimer = Time.time + 0.5f;
		vp_Timer.In(0.1f, delegate()
		{
			this.m_Camera.RotationPitchLimit.y = this.m_OriginalPitchDownLimit;
		}, null);
	}

	// Token: 0x06002D89 RID: 11657 RVA: 0x000AE618 File Offset: 0x000AC818
	public override void FinishInteraction()
	{
		if (this.m_IsGrabbed)
		{
			this.StopGrab();
		}
	}

	// Token: 0x06002D8A RID: 11658 RVA: 0x000AE628 File Offset: 0x000AC828
	protected virtual IEnumerator Fetch()
	{
		this.m_IsFetching = true;
		this.m_CurrentSwayForce = Vector3.zero;
		this.m_CurrentSwayTorque = Vector3.zero;
		this.m_CurrentFootstepForce = 0f;
		this.m_FetchProgress = 0f;
		this.duration = Vector3.Distance(this.m_Camera.Transform.position, this.m_Transform.position) * 0.5f;
		vp_Timer.In(this.duration + 1f, delegate()
		{
		}, this.m_DisableAngleSwayTimer);
		while (this.m_FetchProgress < 1f)
		{
			this.m_FetchProgress += Time.deltaTime / this.duration;
			yield return new WaitForEndOfFrame();
		}
		this.m_IsFetching = false;
		yield break;
	}

	// Token: 0x06002D8B RID: 11659 RVA: 0x000AE638 File Offset: 0x000AC838
	protected virtual void OnCollisionEnter(Collision col)
	{
		if (!this.m_IsGrabbed)
		{
			return;
		}
		if (this.m_FetchProgress < 1f)
		{
			this.m_FetchProgress *= 1.2f;
		}
		vp_Timer.In(2f, delegate()
		{
		}, this.m_DisableAngleSwayTimer);
	}

	// Token: 0x06002D8C RID: 11660 RVA: 0x000AE69C File Offset: 0x000AC89C
	protected virtual void OnCollisionStay(Collision col)
	{
		if (!this.m_IsGrabbed || this.MaxCollisionCount == 0)
		{
			return;
		}
		if (col.collider != this.m_LastExternalCollider)
		{
			if (!col.collider.GetComponent<Rigidbody>() || col.collider.GetComponent<Rigidbody>().isKinematic)
			{
				this.m_LastExternalCollider = col.collider;
				this.m_CollisionCount = 1;
				return;
			}
		}
		else
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(this.m_Transform.position, this.m_Camera.Transform.forward, out raycastHit, 1f) && raycastHit.collider == this.m_LastExternalCollider && this.m_FetchProgress >= 1f)
			{
				this.m_CollisionCount = this.MaxCollisionCount;
			}
			this.m_CollisionCount++;
			if (this.m_CollisionCount > this.MaxCollisionCount && (!Physics.Raycast(col.contacts[0].point + Vector3.up * 0.1f, -Vector3.up, out raycastHit, 0.2f) || !(raycastHit.collider == this.m_LastExternalCollider)))
			{
				this.m_CollisionCount = 0;
				this.m_FetchProgress = 1f;
				this.m_LastExternalCollider = null;
				if (this.m_Player != null)
				{
					this.m_Player.Interact.TryStart(true);
				}
			}
		}
	}

	// Token: 0x06002D8D RID: 11661 RVA: 0x000AE807 File Offset: 0x000ACA07
	protected virtual void OnCollisionExit()
	{
		this.m_CurrentHoldAngle = this.m_Transform.localEulerAngles;
	}

	// Token: 0x06002D8E RID: 11662 RVA: 0x000AE81A File Offset: 0x000ACA1A
	protected virtual void Footstep()
	{
		this.m_CurrentFootstepForce += this.FootstepForce;
	}

	// Token: 0x06002D8F RID: 11663 RVA: 0x000AE82F File Offset: 0x000ACA2F
	protected virtual void OnMessage_FallImpact(float impact)
	{
		this.m_CurrentSwayForce.y = this.m_CurrentSwayForce.y + impact * this.Kneeling;
	}

	// Token: 0x06002D90 RID: 11664 RVA: 0x000AE848 File Offset: 0x000ACA48
	protected virtual void OnStop_SetWeapon()
	{
		if (this.m_IsGrabbed && !this.m_IsFetching)
		{
			base.StartCoroutine("Fetch");
		}
	}

	// Token: 0x06002D91 RID: 11665 RVA: 0x000AE868 File Offset: 0x000ACA68
	protected virtual bool CanStart_SetWeapon()
	{
		int num = (int)this.m_Player.SetWeapon.Argument;
		return !this.m_IsGrabbed || num == 0;
	}

	// Token: 0x06002D92 RID: 11666 RVA: 0x000AE89C File Offset: 0x000ACA9C
	public void Register(vp_EventHandler eventHandler)
	{
		eventHandler.RegisterActivity("SetWeapon", null, new vp_Activity.Callback(this.OnStop_SetWeapon), new vp_Activity.Condition(this.CanStart_SetWeapon), null, null, null);
		eventHandler.RegisterMessage<float>("FallImpact", new vp_Message<float>.Sender<float>(this.OnMessage_FallImpact));
	}

	// Token: 0x06002D93 RID: 11667 RVA: 0x000AE8EC File Offset: 0x000ACAEC
	public void Unregister(vp_EventHandler eventHandler)
	{
		eventHandler.UnregisterActivity("SetWeapon", null, new vp_Activity.Callback(this.OnStop_SetWeapon), new vp_Activity.Condition(this.CanStart_SetWeapon), null, null, null);
		eventHandler.UnregisterMessage<float>("FallImpact", new vp_Message<float>.Sender<float>(this.OnMessage_FallImpact));
	}

	// Token: 0x04002B96 RID: 11158
	public string OnGrabText = "";

	// Token: 0x04002B97 RID: 11159
	public Texture GrabStateCrosshair;

	// Token: 0x04002B98 RID: 11160
	public float FootstepForce = 0.015f;

	// Token: 0x04002B99 RID: 11161
	public float Kneeling = 5f;

	// Token: 0x04002B9A RID: 11162
	public float Stiffness = 0.5f;

	// Token: 0x04002B9B RID: 11163
	public float ShakeSpeed = 0.1f;

	// Token: 0x04002B9C RID: 11164
	public Vector3 ShakeAmplitude = Vector3.one;

	// Token: 0x04002B9D RID: 11165
	public float ThrowStrength = 6f;

	// Token: 0x04002B9E RID: 11166
	public bool AllowThrowRotation = true;

	// Token: 0x04002B9F RID: 11167
	public float Burden;

	// Token: 0x04002BA0 RID: 11168
	public int MaxCollisionCount = 20;

	// Token: 0x04002BA1 RID: 11169
	protected Vector3 m_CurrentShake = Vector3.zero;

	// Token: 0x04002BA2 RID: 11170
	protected Vector3 m_CurrentRotationSway = Vector3.zero;

	// Token: 0x04002BA3 RID: 11171
	protected Vector2 m_CurrentMouseMove;

	// Token: 0x04002BA4 RID: 11172
	protected Vector3 m_CurrentSwayForce;

	// Token: 0x04002BA5 RID: 11173
	protected Vector3 m_CurrentSwayTorque;

	// Token: 0x04002BA6 RID: 11174
	protected float m_CurrentFootstepForce;

	// Token: 0x04002BA7 RID: 11175
	protected Vector3 m_CurrentHoldAngle = Vector3.one;

	// Token: 0x04002BA8 RID: 11176
	protected Collider m_LastExternalCollider;

	// Token: 0x04002BA9 RID: 11177
	protected int m_CollisionCount;

	// Token: 0x04002BAA RID: 11178
	public Vector3 CarryingOffset = new Vector3(0f, -0.5f, 1.5f);

	// Token: 0x04002BAB RID: 11179
	public float CameraPitchDownLimit;

	// Token: 0x04002BAC RID: 11180
	protected Vector3 m_TempCarryingOffset = Vector3.zero;

	// Token: 0x04002BAD RID: 11181
	protected bool m_IsFetching;

	// Token: 0x04002BAE RID: 11182
	protected float duration;

	// Token: 0x04002BAF RID: 11183
	protected float m_FetchProgress;

	// Token: 0x04002BB0 RID: 11184
	protected vp_FPInteractManager m_InteractManager;

	// Token: 0x04002BB1 RID: 11185
	protected AudioSource m_Audio;

	// Token: 0x04002BB2 RID: 11186
	protected int m_LastWeaponEquipped;

	// Token: 0x04002BB3 RID: 11187
	protected bool m_IsGrabbed;

	// Token: 0x04002BB4 RID: 11188
	protected float m_OriginalPitchDownLimit;

	// Token: 0x04002BB5 RID: 11189
	protected bool m_DefaultGravity;

	// Token: 0x04002BB6 RID: 11190
	protected float m_DefaultDrag;

	// Token: 0x04002BB7 RID: 11191
	protected float m_DefaultAngularDrag;

	// Token: 0x04002BB8 RID: 11192
	public Vector2 SoundsPitch = new Vector2(1f, 1.5f);

	// Token: 0x04002BB9 RID: 11193
	public List<AudioClip> GrabSounds = new List<AudioClip>();

	// Token: 0x04002BBA RID: 11194
	public List<AudioClip> DropSounds = new List<AudioClip>();

	// Token: 0x04002BBB RID: 11195
	public List<AudioClip> ThrowSounds = new List<AudioClip>();

	// Token: 0x04002BBC RID: 11196
	protected vp_Timer.Handle m_DisableAngleSwayTimer = new vp_Timer.Handle();

	// Token: 0x04002BBD RID: 11197
	private vp_FPPlayerEventHandler m_FPPlayer;
}
