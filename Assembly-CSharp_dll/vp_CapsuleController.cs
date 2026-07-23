using System;
using UnityEngine;

// Token: 0x0200088A RID: 2186
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class vp_CapsuleController : vp_Controller
{
	// Token: 0x1700032D RID: 813
	// (get) Token: 0x06002F62 RID: 12130 RVA: 0x000BA544 File Offset: 0x000B8744
	protected CapsuleCollider CapsuleCollider
	{
		get
		{
			if (this.m_CapsuleCollider == null)
			{
				this.m_CapsuleCollider = (base.Collider as CapsuleCollider);
				if (this.m_CapsuleCollider != null && this.m_CapsuleCollider.isTrigger)
				{
					this.m_CapsuleCollider = null;
				}
			}
			return this.m_CapsuleCollider;
		}
	}

	// Token: 0x06002F63 RID: 12131 RVA: 0x000BA598 File Offset: 0x000B8798
	protected override void InitCollider()
	{
		this.m_NormalHeight = this.CapsuleCollider.height;
		this.CapsuleCollider.center = (this.m_NormalCenter = this.m_NormalHeight * (Vector3.up * 0.5f));
		this.CapsuleCollider.radius = this.m_NormalHeight * 0.25f;
		this.m_CrouchHeight = this.m_NormalHeight * this.PhysicsCrouchHeightModifier;
		this.m_CrouchCenter = this.m_NormalCenter * this.PhysicsCrouchHeightModifier;
		base.Collider.transform.localPosition = Vector3.zero;
	}

	// Token: 0x06002F64 RID: 12132 RVA: 0x000BA63C File Offset: 0x000B883C
	protected override void RefreshCollider()
	{
		if (base.Player.Crouch.Active && (!this.MotorFreeFly || base.Grounded))
		{
			this.CapsuleCollider.height = this.m_NormalHeight * this.PhysicsCrouchHeightModifier;
			this.CapsuleCollider.center = this.m_NormalCenter * this.PhysicsCrouchHeightModifier;
			return;
		}
		this.CapsuleCollider.height = this.m_NormalHeight;
		this.CapsuleCollider.center = this.m_NormalCenter;
	}

	// Token: 0x06002F65 RID: 12133 RVA: 0x000BA6C2 File Offset: 0x000B88C2
	public override void EnableCollider(bool isEnabled = true)
	{
		if (this.CapsuleCollider != null)
		{
			this.CapsuleCollider.enabled = isEnabled;
		}
	}

	// Token: 0x06002F66 RID: 12134 RVA: 0x000BA6DE File Offset: 0x000B88DE
	protected override float Get_Radius()
	{
		return this.CapsuleCollider.radius;
	}

	// Token: 0x1700032E RID: 814
	// (get) Token: 0x06002F67 RID: 12135 RVA: 0x000BA6DE File Offset: 0x000B88DE
	protected override float OnValue_Radius
	{
		get
		{
			return this.CapsuleCollider.radius;
		}
	}

	// Token: 0x06002F68 RID: 12136 RVA: 0x000BA6EB File Offset: 0x000B88EB
	protected override float Get_Height()
	{
		return this.CapsuleCollider.height;
	}

	// Token: 0x1700032F RID: 815
	// (get) Token: 0x06002F69 RID: 12137 RVA: 0x000BA6EB File Offset: 0x000B88EB
	protected override float OnValue_Height
	{
		get
		{
			return this.CapsuleCollider.height;
		}
	}

	// Token: 0x04002D75 RID: 11637
	protected CapsuleCollider m_CapsuleCollider;
}
