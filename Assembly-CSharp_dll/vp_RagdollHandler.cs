using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000890 RID: 2192
public class vp_RagdollHandler : MonoBehaviour, EventHandlerRegistrable
{
	// Token: 0x17000340 RID: 832
	// (get) Token: 0x06002FBE RID: 12222 RVA: 0x000BC0B3 File Offset: 0x000BA2B3
	protected vp_PlayerEventHandler Player
	{
		get
		{
			if (this.m_Player == null && !this.m_TriedToFetchPlayer)
			{
				this.m_Player = base.transform.root.GetComponentInChildren<vp_PlayerEventHandler>();
				this.m_TriedToFetchPlayer = true;
			}
			return this.m_Player;
		}
	}

	// Token: 0x17000341 RID: 833
	// (get) Token: 0x06002FBF RID: 12223 RVA: 0x000BC0EE File Offset: 0x000BA2EE
	public vp_FPCamera FPCamera
	{
		get
		{
			if (this.m_FPCamera == null)
			{
				this.m_FPCamera = base.transform.root.GetComponentInChildren<vp_FPCamera>();
			}
			return this.m_FPCamera;
		}
	}

	// Token: 0x17000342 RID: 834
	// (get) Token: 0x06002FC0 RID: 12224 RVA: 0x000BC11A File Offset: 0x000BA31A
	protected vp_Controller Controller
	{
		get
		{
			if (this.m_Controller == null)
			{
				this.m_Controller = base.transform.root.GetComponentInChildren<vp_Controller>();
			}
			return this.m_Controller;
		}
	}

	// Token: 0x17000343 RID: 835
	// (get) Token: 0x06002FC1 RID: 12225 RVA: 0x000BC148 File Offset: 0x000BA348
	protected List<Collider> Colliders
	{
		get
		{
			if (this.m_Colliders == null)
			{
				this.m_Colliders = new List<Collider>();
				foreach (Collider item in base.GetComponentsInChildren<Collider>())
				{
					this.m_Colliders.Add(item);
				}
			}
			return this.m_Colliders;
		}
	}

	// Token: 0x17000344 RID: 836
	// (get) Token: 0x06002FC2 RID: 12226 RVA: 0x000BC193 File Offset: 0x000BA393
	protected List<Rigidbody> Rigidbodies
	{
		get
		{
			if (this.m_Rigidbodies == null)
			{
				this.m_Rigidbodies = new List<Rigidbody>(base.GetComponentsInChildren<Rigidbody>());
			}
			return this.m_Rigidbodies;
		}
	}

	// Token: 0x17000345 RID: 837
	// (get) Token: 0x06002FC3 RID: 12227 RVA: 0x000BC1B4 File Offset: 0x000BA3B4
	protected List<Transform> Transforms
	{
		get
		{
			if (this.m_Transforms == null)
			{
				this.m_Transforms = new List<Transform>();
				foreach (Rigidbody rigidbody in this.Rigidbodies)
				{
					this.m_Transforms.Add(rigidbody.transform);
				}
			}
			return this.m_Transforms;
		}
	}

	// Token: 0x17000346 RID: 838
	// (get) Token: 0x06002FC4 RID: 12228 RVA: 0x000BC22C File Offset: 0x000BA42C
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

	// Token: 0x17000347 RID: 839
	// (get) Token: 0x06002FC5 RID: 12229 RVA: 0x000BC24E File Offset: 0x000BA44E
	protected vp_BodyAnimator BodyAnimator
	{
		get
		{
			if (this.m_BodyAnimator == null)
			{
				this.m_BodyAnimator = base.GetComponent<vp_BodyAnimator>();
			}
			return this.m_BodyAnimator;
		}
	}

	// Token: 0x06002FC6 RID: 12230 RVA: 0x000BC270 File Offset: 0x000BA470
	protected virtual void Awake()
	{
		if (this.Colliders == null || this.Colliders.Count == 0 || this.Rigidbodies == null || this.Rigidbodies.Count == 0 || this.Transforms == null || this.Transforms.Count == 0 || this.Animator == null || this.BodyAnimator == null)
		{
			Debug.LogError("Error (" + this + ") Could not be initialized. Please make sure hierarchy has ragdoll colliders, Animator and vp_BodyAnimator.");
			base.enabled = false;
			return;
		}
	}

	// Token: 0x06002FC7 RID: 12231 RVA: 0x000BC2F5 File Offset: 0x000BA4F5
	protected virtual void Start()
	{
		this.SetRagdoll(false);
	}

	// Token: 0x06002FC8 RID: 12232 RVA: 0x000BC300 File Offset: 0x000BA500
	protected virtual void SaveStartPose()
	{
		foreach (Transform transform in this.Transforms)
		{
			if (!this.TransformRotations.ContainsKey(transform))
			{
				this.TransformRotations.Add(transform.transform, transform.localRotation);
			}
			if (!this.TransformPositions.ContainsKey(transform))
			{
				this.TransformPositions.Add(transform.transform, transform.localPosition);
			}
		}
	}

	// Token: 0x06002FC9 RID: 12233 RVA: 0x000BC398 File Offset: 0x000BA598
	protected virtual void RestoreStartPose()
	{
		foreach (Transform transform in this.Transforms)
		{
			if (this.TransformRotations.TryGetValue(transform, out this.m_Rot))
			{
				transform.localRotation = this.m_Rot;
			}
			if (this.TransformPositions.TryGetValue(transform, out this.m_Pos))
			{
				transform.localPosition = this.m_Pos;
			}
		}
	}

	// Token: 0x06002FCA RID: 12234 RVA: 0x000BC424 File Offset: 0x000BA624
	protected virtual void OnEnable()
	{
		if (this.Player != null)
		{
			this.Register(this.Player);
		}
	}

	// Token: 0x06002FCB RID: 12235 RVA: 0x000BC440 File Offset: 0x000BA640
	protected virtual void OnDisable()
	{
		if (this.Player != null)
		{
			this.Unregister(this.Player);
		}
	}

	// Token: 0x06002FCC RID: 12236 RVA: 0x00003296 File Offset: 0x00001496
	private void Update()
	{
	}

	// Token: 0x06002FCD RID: 12237 RVA: 0x000BC45C File Offset: 0x000BA65C
	private void LateUpdate()
	{
		this.UpdateDeathCamera();
	}

	// Token: 0x06002FCE RID: 12238 RVA: 0x000BC464 File Offset: 0x000BA664
	protected virtual void UpdateDeathCamera()
	{
		if (this.Player == null)
		{
			return;
		}
		if (!this.Player.Dead.Active)
		{
			return;
		}
		if (this.HeadBone == null)
		{
			return;
		}
		if (!this.Player.IsFirstPerson.Get())
		{
			return;
		}
		this.FPCamera.Transform.position = this.HeadBone.transform.position;
		this.m_HeadRotationCorrection = this.HeadBone.transform.localEulerAngles;
		if (Time.time - this.m_TimeOfDeath < this.CameraFreezeDelay)
		{
			this.FPCamera.Transform.localEulerAngles = (this.m_CameraFreezeAngle = new Vector3(-this.m_HeadRotationCorrection.z, -this.m_HeadRotationCorrection.x, this.m_HeadRotationCorrection.y));
			return;
		}
		this.FPCamera.Transform.localEulerAngles = this.m_CameraFreezeAngle;
	}

	// Token: 0x06002FCF RID: 12239 RVA: 0x000BC55C File Offset: 0x000BA75C
	public virtual void SetRagdoll(bool enabled = true)
	{
		if (enabled)
		{
			if (!this.Player.Dead.Active)
			{
				return;
			}
			this.PostponeTimer.Cancel();
			if (!this.Animator.GetBool("IsGrounded"))
			{
				vp_Timer.In(0.1f, delegate()
				{
					this.SetRagdoll(true);
				}, this.PostponeTimer);
				return;
			}
		}
		if (this.Animator != null)
		{
			this.Animator.enabled = !enabled;
		}
		if (this.BodyAnimator != null)
		{
			this.BodyAnimator.enabled = !enabled;
		}
		if (this.Controller != null)
		{
			this.Controller.EnableCollider(!enabled);
		}
		foreach (Rigidbody rigidbody in this.Rigidbodies)
		{
			rigidbody.isKinematic = !enabled;
			if (enabled)
			{
				rigidbody.AddForce(this.Player.Velocity.Get() * this.VelocityMultiplier);
			}
		}
		foreach (Collider collider in this.Colliders)
		{
			collider.enabled = enabled;
		}
		if (!enabled)
		{
			this.RestoreStartPose();
		}
	}

	// Token: 0x06002FD0 RID: 12240 RVA: 0x000BC6CC File Offset: 0x000BA8CC
	protected virtual void OnStart_Dead()
	{
		this.m_TimeOfDeath = Time.time;
		vp_Timer.In(0f, delegate()
		{
			this.SetRagdoll(true);
		}, null);
	}

	// Token: 0x06002FD1 RID: 12241 RVA: 0x000BC6F0 File Offset: 0x000BA8F0
	protected virtual void OnStop_Dead()
	{
		this.SetRagdoll(false);
		this.Player.OutOfControl.Stop(0f);
	}

	// Token: 0x06002FD2 RID: 12242 RVA: 0x000BC70E File Offset: 0x000BA90E
	public void Register(vp_EventHandler eventHandler)
	{
		eventHandler.RegisterActivity("Dead", new vp_Activity.Callback(this.OnStart_Dead), new vp_Activity.Callback(this.OnStop_Dead), null, null, null, null);
	}

	// Token: 0x06002FD3 RID: 12243 RVA: 0x000BC739 File Offset: 0x000BA939
	public void Unregister(vp_EventHandler eventHandler)
	{
		eventHandler.UnregisterActivity("Dead", new vp_Activity.Callback(this.OnStart_Dead), new vp_Activity.Callback(this.OnStop_Dead), null, null, null, null);
	}

	// Token: 0x04002DE3 RID: 11747
	public float CameraFreezeDelay = 2.5f;

	// Token: 0x04002DE4 RID: 11748
	public float VelocityMultiplier = 30f;

	// Token: 0x04002DE5 RID: 11749
	public GameObject HeadBone;

	// Token: 0x04002DE6 RID: 11750
	protected float m_TimeOfDeath;

	// Token: 0x04002DE7 RID: 11751
	protected vp_Timer.Handle PostponeTimer = new vp_Timer.Handle();

	// Token: 0x04002DE8 RID: 11752
	protected Vector3 m_HeadRotationCorrection = Vector3.zero;

	// Token: 0x04002DE9 RID: 11753
	protected Vector3 m_CameraFreezeAngle = Vector3.zero;

	// Token: 0x04002DEA RID: 11754
	protected List<Collider> m_Colliders;

	// Token: 0x04002DEB RID: 11755
	protected List<Rigidbody> m_Rigidbodies;

	// Token: 0x04002DEC RID: 11756
	protected List<Transform> m_Transforms;

	// Token: 0x04002DED RID: 11757
	protected Animator m_Animator;

	// Token: 0x04002DEE RID: 11758
	protected vp_BodyAnimator m_BodyAnimator;

	// Token: 0x04002DEF RID: 11759
	protected vp_PlayerEventHandler m_Player;

	// Token: 0x04002DF0 RID: 11760
	protected vp_FPCamera m_FPCamera;

	// Token: 0x04002DF1 RID: 11761
	protected vp_Controller m_Controller;

	// Token: 0x04002DF2 RID: 11762
	protected Dictionary<Transform, Quaternion> TransformRotations = new Dictionary<Transform, Quaternion>();

	// Token: 0x04002DF3 RID: 11763
	protected Dictionary<Transform, Vector3> TransformPositions = new Dictionary<Transform, Vector3>();

	// Token: 0x04002DF4 RID: 11764
	protected Quaternion m_Rot;

	// Token: 0x04002DF5 RID: 11765
	protected Vector3 m_Pos;

	// Token: 0x04002DF6 RID: 11766
	private bool m_TriedToFetchPlayer;
}
