using System;
using UnityEngine;

// Token: 0x02000888 RID: 2184
public class vp_3rdPersonWeaponAim : MonoBehaviour, EventHandlerRegistrable
{
	// Token: 0x17000319 RID: 793
	// (get) Token: 0x06002F1D RID: 12061 RVA: 0x000B82BD File Offset: 0x000B64BD
	public Transform Transform
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

	// Token: 0x1700031A RID: 794
	// (get) Token: 0x06002F1E RID: 12062 RVA: 0x000B82DF File Offset: 0x000B64DF
	public vp_PlayerEventHandler Player
	{
		get
		{
			if (this.m_Player == null)
			{
				this.m_Player = (vp_PlayerEventHandler)this.Root.GetComponentInChildren(typeof(vp_PlayerEventHandler));
			}
			return this.m_Player;
		}
	}

	// Token: 0x1700031B RID: 795
	// (get) Token: 0x06002F1F RID: 12063 RVA: 0x000B8315 File Offset: 0x000B6515
	public vp_WeaponHandler WeaponHandler
	{
		get
		{
			if (this.m_WeaponHandler == null)
			{
				this.m_WeaponHandler = (vp_WeaponHandler)this.Root.GetComponentInChildren(typeof(vp_WeaponHandler));
			}
			return this.m_WeaponHandler;
		}
	}

	// Token: 0x1700031C RID: 796
	// (get) Token: 0x06002F20 RID: 12064 RVA: 0x000B834B File Offset: 0x000B654B
	protected Animator Animator
	{
		get
		{
			if (this.m_Animator == null)
			{
				this.m_Animator = this.Root.GetComponentInChildren<Animator>();
			}
			return this.m_Animator;
		}
	}

	// Token: 0x1700031D RID: 797
	// (get) Token: 0x06002F21 RID: 12065 RVA: 0x000B8372 File Offset: 0x000B6572
	private Transform Root
	{
		get
		{
			if (this.m_Root == null)
			{
				this.m_Root = this.Transform.root;
			}
			return this.m_Root;
		}
	}

	// Token: 0x1700031E RID: 798
	// (get) Token: 0x06002F22 RID: 12066 RVA: 0x000B8399 File Offset: 0x000B6599
	private Transform LowerArmObj
	{
		get
		{
			if (this.m_LowerArmObj == null)
			{
				this.m_LowerArmObj = this.HandObj.parent;
			}
			return this.m_LowerArmObj;
		}
	}

	// Token: 0x1700031F RID: 799
	// (get) Token: 0x06002F23 RID: 12067 RVA: 0x000B83C0 File Offset: 0x000B65C0
	private Transform HandObj
	{
		get
		{
			if (this.m_HandObj == null)
			{
				if (this.Hand != null)
				{
					this.m_HandObj = this.Hand.transform;
				}
				else
				{
					this.m_HandObj = vp_Utility.GetTransformByNameInAncestors(this.Transform, "hand", true, true);
					if (this.m_HandObj == null && this.Transform.parent != null)
					{
						this.m_HandObj = this.Transform.parent;
					}
					if (this.m_HandObj != null)
					{
						this.Hand = this.m_HandObj.gameObject;
					}
				}
			}
			return this.m_HandObj;
		}
	}

	// Token: 0x06002F24 RID: 12068 RVA: 0x000B846E File Offset: 0x000B666E
	protected virtual void OnEnable()
	{
		if (this.Player != null)
		{
			this.Register(this.Player);
		}
	}

	// Token: 0x06002F25 RID: 12069 RVA: 0x000B848A File Offset: 0x000B668A
	protected virtual void OnDisable()
	{
		if (this.Player != null)
		{
			this.Unregister(this.Player);
		}
	}

	// Token: 0x06002F26 RID: 12070 RVA: 0x000B84A8 File Offset: 0x000B66A8
	protected virtual void Awake()
	{
		this.m_DefaultRotation = this.Transform.localRotation;
		if (this.LowerArmObj == null || this.HandObj == null)
		{
			Debug.LogError("Hierarchy Error (" + this + ") This script should be placed on a 3rd person weapon gameobject childed to a hand bone in a rigged character.");
			base.enabled = false;
			return;
		}
		Quaternion lhs = Quaternion.Inverse(this.LowerArmObj.rotation);
		this.m_ReferenceLookDir = lhs * this.Root.rotation * Vector3.forward;
		this.m_ReferenceUpDir = lhs * this.Root.rotation * Vector3.up;
		Quaternion rotation = this.HandObj.rotation;
		this.HandObj.rotation = this.Root.rotation;
		Quaternion rotation2 = this.HandObj.rotation;
		this.HandObj.rotation = rotation;
		this.m_HandBoneRotDif = Quaternion.Inverse(rotation2) * rotation;
	}

	// Token: 0x06002F27 RID: 12071 RVA: 0x000B859E File Offset: 0x000B679E
	protected virtual void LateUpdate()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		this.UpdateAiming();
	}

	// Token: 0x06002F28 RID: 12072 RVA: 0x000B85B4 File Offset: 0x000B67B4
	protected virtual void UpdateAiming()
	{
		if (this.Animator == null)
		{
			return;
		}
		if ((!this.Animator.GetBool("IsAttacking") && !this.Animator.GetBool("IsZooming")) || this.Animator.GetBool("IsReloading") || this.Animator.GetBool("IsOutOfControl") || this.Player.CurrentWeaponIndex.Get() == 0)
		{
			this.Transform.localRotation = this.m_DefaultRotation;
			return;
		}
		Quaternion rotation = this.Transform.rotation;
		this.Transform.rotation = Quaternion.LookRotation(this.Player.AimDirection.Get());
		this.m_WorldDir = this.Transform.forward;
		this.Transform.rotation = rotation;
		this.HandObj.rotation = vp_3DUtility.GetBoneLookRotationInWorldSpace(this.HandObj.rotation, this.LowerArmObj.rotation, this.m_WorldDir, 1f, this.m_ReferenceUpDir, this.m_ReferenceLookDir, this.m_HandBoneRotDif);
		this.HandObj.Rotate(this.Transform.forward, this.AngleAdjustZ + this.WeaponHandler.CurrentWeapon.Recoil.z * this.RecoilFactorZ, Space.World);
		this.HandObj.Rotate(this.Transform.up, this.AngleAdjustY + this.WeaponHandler.CurrentWeapon.Recoil.y * this.RecoilFactorY, Space.World);
		this.HandObj.Rotate(this.Transform.right, this.AngleAdjustX + this.WeaponHandler.CurrentWeapon.Recoil.x * this.RecoilFactorX, Space.World);
	}

	// Token: 0x06002F29 RID: 12073 RVA: 0x00003296 File Offset: 0x00001496
	public void Register(vp_EventHandler eventHandler)
	{
	}

	// Token: 0x06002F2A RID: 12074 RVA: 0x00003296 File Offset: 0x00001496
	public void Unregister(vp_EventHandler eventHandler)
	{
	}

	// Token: 0x04002D16 RID: 11542
	public GameObject Hand;

	// Token: 0x04002D17 RID: 11543
	[Range(0f, 360f)]
	public float AngleAdjustX;

	// Token: 0x04002D18 RID: 11544
	[Range(0f, 360f)]
	public float AngleAdjustY;

	// Token: 0x04002D19 RID: 11545
	[Range(0f, 360f)]
	public float AngleAdjustZ;

	// Token: 0x04002D1A RID: 11546
	[Range(0f, 5f)]
	public float RecoilFactorX = 1f;

	// Token: 0x04002D1B RID: 11547
	[Range(0f, 5f)]
	public float RecoilFactorY = 1f;

	// Token: 0x04002D1C RID: 11548
	[Range(0f, 5f)]
	public float RecoilFactorZ = 1f;

	// Token: 0x04002D1D RID: 11549
	protected Quaternion m_DefaultRotation;

	// Token: 0x04002D1E RID: 11550
	protected Vector3 m_ReferenceUpDir;

	// Token: 0x04002D1F RID: 11551
	protected Vector3 m_ReferenceLookDir;

	// Token: 0x04002D20 RID: 11552
	protected Quaternion m_HandBoneRotDif;

	// Token: 0x04002D21 RID: 11553
	protected Vector3 m_WorldDir = Vector3.zero;

	// Token: 0x04002D22 RID: 11554
	protected Transform m_Transform;

	// Token: 0x04002D23 RID: 11555
	protected vp_PlayerEventHandler m_Player;

	// Token: 0x04002D24 RID: 11556
	private vp_WeaponHandler m_WeaponHandler;

	// Token: 0x04002D25 RID: 11557
	protected Animator m_Animator;

	// Token: 0x04002D26 RID: 11558
	private Transform m_Root;

	// Token: 0x04002D27 RID: 11559
	private Transform m_LowerArmObj;

	// Token: 0x04002D28 RID: 11560
	private Transform m_HandObj;
}
