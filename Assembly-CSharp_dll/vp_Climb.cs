using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000865 RID: 2149
public class vp_Climb : vp_Interactable, EventHandlerRegistrable
{
	// Token: 0x170002DC RID: 732
	// (get) Token: 0x06002D5A RID: 11610 RVA: 0x000AC7F6 File Offset: 0x000AA9F6
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

	// Token: 0x06002D5B RID: 11611 RVA: 0x000AC81D File Offset: 0x000AAA1D
	protected override void Start()
	{
		base.Start();
		this.m_CanClimbAgain = Time.time;
	}

	// Token: 0x06002D5C RID: 11612 RVA: 0x000AC830 File Offset: 0x000AAA30
	public override bool TryInteract(vp_PlayerEventHandler player)
	{
		if (!base.enabled)
		{
			return false;
		}
		if (!(player is vp_FPPlayerEventHandler))
		{
			return false;
		}
		if (Time.time < this.m_CanClimbAgain)
		{
			return false;
		}
		if (this.m_IsClimbing)
		{
			this.m_Player.Climb.TryStop(true);
			return false;
		}
		if (this.m_Player == null)
		{
			this.m_Player = player;
		}
		if (this.m_Player.Interactable.Get() != null)
		{
			return false;
		}
		if (this.m_Controller == null)
		{
			this.m_Controller = this.m_Player.GetComponent<vp_FPController>();
		}
		if (this.m_Player.Velocity.Get().magnitude > this.MinVelocityToClimb)
		{
			return false;
		}
		if (this.m_Camera == null)
		{
			this.m_Camera = this.m_Player.GetComponentInChildren<vp_FPCamera>();
		}
		if (this.Sounds.AudioSource == null)
		{
			this.Sounds.AudioSource = this.m_Player.GetComponent<AudioSource>();
		}
		if (this.m_Player != null)
		{
			this.Register(this.m_Player);
		}
		this.m_Player.Interactable.Set(this);
		return this.m_Player.Climb.TryStart(true);
	}

	// Token: 0x06002D5D RID: 11613 RVA: 0x000AC980 File Offset: 0x000AAB80
	protected virtual void OnStart_Climb()
	{
		this.m_Controller.PhysicsGravityModifier = 0f;
		this.m_Camera.SetRotation(this.m_Camera.Transform.eulerAngles, false);
		this.m_Player.Jump.Stop(0f);
		this.FPPlayer.InputAllowGameplay.Set(false);
		this.m_Player.Stop.Send();
		this.m_LastWeaponEquipped = this.m_Player.CurrentWeaponIndex.Get();
		this.m_Player.SetWeapon.TryStart<int>(0);
		this.m_Player.Interactable.Set(null);
		this.PlaySound(this.Sounds.MountSounds);
		if (this.m_Controller.Transform.GetComponent<Collider>().enabled && this.m_Transform.GetComponent<Collider>().enabled)
		{
			Log.Error("UFPS Ignoring Collider 1", Array.Empty<object>());
			Physics.IgnoreCollision(this.m_Controller.Transform.GetComponent<Collider>(), this.m_Transform.GetComponent<Collider>(), true);
		}
		base.StartCoroutine("LineUp");
	}

	// Token: 0x06002D5E RID: 11614 RVA: 0x000ACAB8 File Offset: 0x000AACB8
	protected virtual void PlaySound(List<AudioClip> sounds)
	{
		if (this.Sounds.AudioSource == null)
		{
			return;
		}
		if (sounds == null || sounds.Count == 0)
		{
			return;
		}
		for (;;)
		{
			this.m_SoundToPlay = sounds[UnityEngine.Random.Range(0, sounds.Count)];
			if (this.m_SoundToPlay == null)
			{
				break;
			}
			if (!(this.m_SoundToPlay == this.m_LastPlayedSound) || sounds.Count <= 1)
			{
				goto IL_63;
			}
		}
		return;
		IL_63:
		if (sounds == this.Sounds.ClimbingSounds)
		{
			this.Sounds.AudioSource.pitch = UnityEngine.Random.Range(this.Sounds.ClimbingPitch.x, this.Sounds.ClimbingPitch.y) * Time.timeScale;
		}
		else
		{
			this.Sounds.AudioSource.pitch = 1f;
		}
		this.Sounds.AudioSource.PlayOneShot(this.m_SoundToPlay);
		this.m_LastPlayedSound = this.m_SoundToPlay;
	}

	// Token: 0x06002D5F RID: 11615 RVA: 0x000ACBAA File Offset: 0x000AADAA
	protected virtual IEnumerator LineUp()
	{
		Vector3 startPosition = this.m_Player.Position.Get();
		Vector3 endPosition = this.GetNewPosition();
		Quaternion startingRotation = this.m_Camera.transform.rotation;
		Quaternion endRotation = Quaternion.LookRotation(-this.m_Transform.forward);
		bool flag = this.m_Controller.Transform.position.y > this.m_Transform.GetComponent<Collider>().bounds.center.y;
		if (flag)
		{
			endPosition += Vector3.down * this.m_Controller.CharacterController.height;
		}
		else
		{
			endPosition += this.m_Controller.Transform.up * (this.m_Controller.CharacterController.height / 2f);
		}
		if (flag && this.m_Transform.InverseTransformDirection(-this.FPPlayer.CameraLookDirection.Get()).z > 0f)
		{
			endRotation = Quaternion.Euler(new Vector3(45f, endRotation.eulerAngles.y, endRotation.eulerAngles.z));
		}
		else
		{
			endRotation = Quaternion.Euler(new Vector3(-45f, endRotation.eulerAngles.y, endRotation.eulerAngles.z));
		}
		endPosition = new Vector3(this.m_Transform.GetComponent<Collider>().bounds.center.x, endPosition.y, this.m_Transform.GetComponent<Collider>().bounds.center.z);
		endPosition += this.m_Transform.forward;
		float t = 0f;
		float duration = Vector3.Distance(this.m_Controller.Transform.position, endPosition) / ((!flag) ? (this.MountSpeed / 1.25f) : this.MountSpeed);
		while (t < 1f)
		{
			t += Time.deltaTime / duration;
			Vector3 o = Vector3.Lerp(startPosition, endPosition, t);
			this.m_Player.Position.Set(o);
			Quaternion quaternion = Quaternion.Slerp(startingRotation, endRotation, t);
			this.m_Player.Rotation.Set(new Vector2(this.MountAutoRotatePitch ? quaternion.eulerAngles.x : this.m_Player.Rotation.Get().x, quaternion.eulerAngles.y));
			yield return new WaitForEndOfFrame();
		}
		this.m_CachedDirection = this.m_Camera.Transform.forward;
		this.m_CachedRotation = this.m_Player.Rotation.Get();
		this.m_IsClimbing = true;
		yield break;
	}

	// Token: 0x06002D60 RID: 11616 RVA: 0x000ACBBC File Offset: 0x000AADBC
	protected virtual void OnStop_Climb()
	{
		this.m_Player.Interactable.Set(null);
		this.FPPlayer.InputAllowGameplay.Set(true);
		this.m_Player.SetWeapon.TryStart<int>(this.m_LastWeaponEquipped);
		this.Unregister(this.m_Player);
		this.m_CanClimbAgain = Time.time + this.ClimbAgainTimeout;
		if (this.m_Controller.Transform.GetComponent<Collider>().enabled && this.m_Transform.GetComponent<Collider>().enabled)
		{
			Log.Error("UFPS Ignoring Collider 2", Array.Empty<object>());
			Physics.IgnoreCollision(this.m_Controller.Transform.GetComponent<Collider>(), this.m_Transform.GetComponent<Collider>(), false);
		}
		this.PlaySound(this.Sounds.DismountSounds);
		Vector3 vector = this.m_Controller.Transform.forward * this.DismountForce;
		if (this.m_Transform.GetComponent<Collider>().bounds.center.y < this.m_Player.Position.Get().y)
		{
			vector *= 2f;
			vector.y = this.DismountForce * 0.5f;
		}
		else
		{
			vector = -vector * 0.5f;
		}
		this.m_Player.Stop.Send();
		this.m_Controller.AddForce(vector);
		this.m_IsClimbing = false;
		this.m_Player.SetState("Default", true, true, false);
		base.StartCoroutine("RestorePitch");
	}

	// Token: 0x06002D61 RID: 11617 RVA: 0x000ACD62 File Offset: 0x000AAF62
	protected virtual IEnumerator RestorePitch()
	{
		float t = 0f;
		while (t < 1f && this.FPPlayer.InputRawLook.Get().y == 0f)
		{
			t += Time.deltaTime;
			this.m_Player.Rotation.Set(Vector2.Lerp(this.m_Player.Rotation.Get(), new Vector2(0f, this.m_Player.Rotation.Get().y), t));
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	// Token: 0x06002D62 RID: 11618 RVA: 0x000ACD71 File Offset: 0x000AAF71
	protected virtual bool CanStart_Interact()
	{
		if (this.m_IsClimbing)
		{
			this.m_Player.Climb.TryStop(true);
		}
		return true;
	}

	// Token: 0x06002D63 RID: 11619 RVA: 0x000ACD8E File Offset: 0x000AAF8E
	protected virtual void FixedUpdate()
	{
		this.Climbing();
	}

	// Token: 0x06002D64 RID: 11620 RVA: 0x000ACD96 File Offset: 0x000AAF96
	protected virtual void Update()
	{
		this.InputJump();
	}

	// Token: 0x06002D65 RID: 11621 RVA: 0x000ACD9E File Offset: 0x000AAF9E
	protected virtual void OnStart_Dead()
	{
		this.FinishInteraction();
	}

	// Token: 0x06002D66 RID: 11622 RVA: 0x000ACDA6 File Offset: 0x000AAFA6
	public override void FinishInteraction()
	{
		if (this.m_IsClimbing)
		{
			this.m_Player.Climb.TryStop(true);
		}
	}

	// Token: 0x06002D67 RID: 11623 RVA: 0x000ACDC4 File Offset: 0x000AAFC4
	protected virtual void Climbing()
	{
		if (this.m_Player == null || !this.m_IsClimbing)
		{
			return;
		}
		this.m_Controller.PhysicsGravityModifier = 0f;
		this.m_Camera.RotationYawLimit = new Vector2(this.m_CachedRotation.y - 90f, this.m_CachedRotation.y + 90f);
		this.m_Camera.RotationPitchLimit = new Vector2(90f, -90f);
		Vector3 vector = this.GetNewPosition();
		Vector3 vector2 = Vector3.zero;
		float num = this.m_Player.Rotation.Get().x / 90f;
		float num2 = this.MinimumClimbSpeed / this.ClimbSpeed;
		if (Mathf.Abs(num) < num2)
		{
			num = ((num > 0f) ? num2 : (num2 * -1f));
		}
		if (num < 0f)
		{
			vector2 = Vector3.up * -num;
		}
		else if (num > 0f)
		{
			vector2 = Vector3.down * num;
		}
		float num3 = this.ClimbSpeed;
		float num4 = (vector2 * this.m_Player.InputClimbVector.Get()).y;
		if (this.SimpleClimb)
		{
			vector2 = Vector3.up;
			num3 *= 0.75f;
			num4 = this.m_Player.InputClimbVector.Get();
		}
		if ((num4 > 0f && vector.y > vp_Climb.GetTopOfCollider(this.m_Transform) - this.m_Controller.CharacterController.height * 0.25f) || (num4 < 0f && this.m_Controller.Grounded && this.m_Controller.GroundTransform.GetInstanceID() != this.m_Transform.GetInstanceID()))
		{
			this.m_Player.Climb.TryStop(true);
			return;
		}
		if (this.m_Player.InputClimbVector.Get() == 0f)
		{
			this.m_ClimbingSoundTimer.Cancel();
		}
		if (this.m_Player.InputClimbVector.Get() != 0f && !this.m_ClimbingSoundTimer.Active && this.Sounds.ClimbingSounds.Count > 0)
		{
			float num5 = Mathf.Abs(5f / vector2.y * (Time.deltaTime * 5f) / this.Sounds.ClimbingSoundSpeed);
			vp_Timer.In(this.SimpleClimb ? (num5 * 3f) : num5, delegate()
			{
				this.PlaySound(this.Sounds.ClimbingSounds);
			}, this.m_ClimbingSoundTimer);
		}
		vector += vector2 * num3 * Time.deltaTime * this.m_Player.InputClimbVector.Get();
		this.m_Player.Position.Set(Vector3.Slerp(this.m_Controller.Transform.position, vector, Time.deltaTime * num3));
	}

	// Token: 0x06002D68 RID: 11624 RVA: 0x000AD0B8 File Offset: 0x000AB2B8
	protected virtual Vector3 GetNewPosition()
	{
		Vector3 vector = this.m_Controller.Transform.position;
		RaycastHit raycastHit;
		Physics.Raycast(new Ray(this.m_Controller.Transform.position, this.m_CachedDirection), out raycastHit, this.DistanceToClimbable * 4f);
		if (raycastHit.collider != null && raycastHit.transform.GetInstanceID() == this.m_Transform.GetInstanceID() && (raycastHit.distance > this.DistanceToClimbable || raycastHit.distance < this.DistanceToClimbable))
		{
			vector = (vector - raycastHit.point).normalized * this.DistanceToClimbable + raycastHit.point;
		}
		return vector;
	}

	// Token: 0x06002D69 RID: 11625 RVA: 0x000AD17C File Offset: 0x000AB37C
	protected virtual void InputJump()
	{
		if (!this.m_IsClimbing)
		{
			return;
		}
		if (this.m_Player == null)
		{
			return;
		}
		if (this.FPPlayer.InputGetButton.Send("Jump") || this.FPPlayer.InputGetButtonDown.Send("Interact"))
		{
			this.m_Player.Climb.TryStop(true);
			if (this.FPPlayer.InputGetButton.Send("Jump"))
			{
				this.m_Controller.AddForce(-this.m_Controller.Transform.forward * this.m_Controller.MotorJumpForce);
			}
		}
	}

	// Token: 0x06002D6A RID: 11626 RVA: 0x000AD238 File Offset: 0x000AB438
	public static float GetTopOfCollider(Transform t)
	{
		return t.position.y + t.GetComponent<Collider>().bounds.size.y / 2f;
	}

	// Token: 0x06002D6B RID: 11627 RVA: 0x000AD270 File Offset: 0x000AB470
	public void Register(vp_EventHandler eventHandler)
	{
		eventHandler.RegisterActivity("Interact", null, null, new vp_Activity.Condition(this.CanStart_Interact), null, null, null);
		eventHandler.RegisterActivity("Climb", new vp_Activity.Callback(this.OnStart_Climb), new vp_Activity.Callback(this.OnStop_Climb), null, null, null, null);
		eventHandler.RegisterActivity("Dead", new vp_Activity.Callback(this.OnStart_Dead), null, null, null, null, null);
	}

	// Token: 0x06002D6C RID: 11628 RVA: 0x000AD2E0 File Offset: 0x000AB4E0
	public void Unregister(vp_EventHandler eventHandler)
	{
		eventHandler.UnregisterActivity("Interact", null, null, new vp_Activity.Condition(this.CanStart_Interact), null, null, null);
		eventHandler.UnregisterActivity("Climb", new vp_Activity.Callback(this.OnStart_Climb), new vp_Activity.Callback(this.OnStop_Climb), null, null, null, null);
		eventHandler.UnregisterActivity("Dead", new vp_Activity.Callback(this.OnStart_Dead), null, null, null, null, null);
	}

	// Token: 0x04002B70 RID: 11120
	public float MinimumClimbSpeed = 3f;

	// Token: 0x04002B71 RID: 11121
	public float ClimbSpeed = 16f;

	// Token: 0x04002B72 RID: 11122
	public float MountSpeed = 5f;

	// Token: 0x04002B73 RID: 11123
	public float DistanceToClimbable = 1f;

	// Token: 0x04002B74 RID: 11124
	public float MinVelocityToClimb = 7f;

	// Token: 0x04002B75 RID: 11125
	public float ClimbAgainTimeout = 1f;

	// Token: 0x04002B76 RID: 11126
	public bool MountAutoRotatePitch;

	// Token: 0x04002B77 RID: 11127
	public bool SimpleClimb = true;

	// Token: 0x04002B78 RID: 11128
	public float DismountForce = 0.2f;

	// Token: 0x04002B79 RID: 11129
	public vp_Climb.vp_ClimbingSounds Sounds;

	// Token: 0x04002B7A RID: 11130
	protected int m_LastWeaponEquipped;

	// Token: 0x04002B7B RID: 11131
	protected bool m_IsClimbing;

	// Token: 0x04002B7C RID: 11132
	protected float m_CanClimbAgain;

	// Token: 0x04002B7D RID: 11133
	protected Vector3 m_CachedDirection = Vector3.zero;

	// Token: 0x04002B7E RID: 11134
	protected Vector2 m_CachedRotation = Vector2.zero;

	// Token: 0x04002B7F RID: 11135
	protected vp_Timer.Handle m_ClimbingSoundTimer = new vp_Timer.Handle();

	// Token: 0x04002B80 RID: 11136
	protected AudioClip m_SoundToPlay;

	// Token: 0x04002B81 RID: 11137
	protected AudioClip m_LastPlayedSound;

	// Token: 0x04002B82 RID: 11138
	private vp_FPPlayerEventHandler m_FPPlayer;

	// Token: 0x02000866 RID: 2150
	[Serializable]
	public class vp_ClimbingSounds
	{
		// Token: 0x04002B83 RID: 11139
		public AudioSource AudioSource;

		// Token: 0x04002B84 RID: 11140
		public List<AudioClip> MountSounds = new List<AudioClip>();

		// Token: 0x04002B85 RID: 11141
		public List<AudioClip> DismountSounds = new List<AudioClip>();

		// Token: 0x04002B86 RID: 11142
		public float ClimbingSoundSpeed = 4f;

		// Token: 0x04002B87 RID: 11143
		public Vector2 ClimbingPitch = new Vector2(1f, 1.5f);

		// Token: 0x04002B88 RID: 11144
		public List<AudioClip> ClimbingSounds = new List<AudioClip>();
	}
}
