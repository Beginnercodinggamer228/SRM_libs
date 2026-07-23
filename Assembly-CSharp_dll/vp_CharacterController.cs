using System;
using UnityEngine;

// Token: 0x0200088B RID: 2187
[RequireComponent(typeof(CharacterController))]
public class vp_CharacterController : vp_Controller
{
	// Token: 0x17000330 RID: 816
	// (get) Token: 0x06002F6B RID: 12139 RVA: 0x000BA700 File Offset: 0x000B8900
	public CharacterController CharacterController
	{
		get
		{
			if (this.m_CharacterController == null)
			{
				this.m_CharacterController = base.gameObject.GetComponent<CharacterController>();
			}
			return this.m_CharacterController;
		}
	}

	// Token: 0x06002F6C RID: 12140 RVA: 0x000BA728 File Offset: 0x000B8928
	protected override void InitCollider()
	{
		this.m_NormalHeight = this.CharacterController.height;
		this.CharacterController.center = (this.m_NormalCenter = this.m_NormalHeight * (Vector3.up * 0.5f));
		this.CharacterController.radius = this.m_NormalHeight * 0.25f;
		this.m_CrouchHeight = this.m_NormalHeight * this.PhysicsCrouchHeightModifier;
		this.m_CrouchCenter = this.m_NormalCenter * this.PhysicsCrouchHeightModifier;
	}

	// Token: 0x06002F6D RID: 12141 RVA: 0x000BA7B8 File Offset: 0x000B89B8
	protected override void RefreshCollider()
	{
		if (base.Player.Crouch.Active && (!this.MotorFreeFly || base.Grounded))
		{
			this.CharacterController.height = this.m_NormalHeight * this.PhysicsCrouchHeightModifier;
			this.CharacterController.center = this.m_NormalCenter * this.PhysicsCrouchHeightModifier;
			return;
		}
		this.CharacterController.height = this.m_NormalHeight;
		this.CharacterController.center = this.m_NormalCenter;
	}

	// Token: 0x06002F6E RID: 12142 RVA: 0x000BA83E File Offset: 0x000B8A3E
	protected virtual float Get_StepOffset()
	{
		return this.CharacterController.stepOffset;
	}

	// Token: 0x17000331 RID: 817
	// (get) Token: 0x06002F6F RID: 12143 RVA: 0x000BA83E File Offset: 0x000B8A3E
	protected virtual float OnValue_StepOffset
	{
		get
		{
			return this.CharacterController.stepOffset;
		}
	}

	// Token: 0x06002F70 RID: 12144 RVA: 0x000BA84B File Offset: 0x000B8A4B
	protected virtual float Get_SlopeLimit()
	{
		return this.CharacterController.slopeLimit;
	}

	// Token: 0x17000332 RID: 818
	// (get) Token: 0x06002F71 RID: 12145 RVA: 0x000BA84B File Offset: 0x000B8A4B
	protected virtual float OnValue_SlopeLimit
	{
		get
		{
			return this.CharacterController.slopeLimit;
		}
	}

	// Token: 0x06002F72 RID: 12146 RVA: 0x000BA858 File Offset: 0x000B8A58
	protected virtual void OnMessage_Move(Vector3 direction)
	{
		if (this.CharacterController.enabled)
		{
			this.CharacterController.Move(direction);
		}
	}

	// Token: 0x06002F73 RID: 12147 RVA: 0x000BA874 File Offset: 0x000B8A74
	protected override float Get_Radius()
	{
		return this.CharacterController.radius;
	}

	// Token: 0x17000333 RID: 819
	// (get) Token: 0x06002F74 RID: 12148 RVA: 0x000BA874 File Offset: 0x000B8A74
	protected override float OnValue_Radius
	{
		get
		{
			return this.CharacterController.radius;
		}
	}

	// Token: 0x06002F75 RID: 12149 RVA: 0x000BA881 File Offset: 0x000B8A81
	protected override float Get_Height()
	{
		return this.CharacterController.height;
	}

	// Token: 0x17000334 RID: 820
	// (get) Token: 0x06002F76 RID: 12150 RVA: 0x000BA881 File Offset: 0x000B8A81
	protected override float OnValue_Height
	{
		get
		{
			return this.CharacterController.height;
		}
	}

	// Token: 0x06002F77 RID: 12151 RVA: 0x000BA890 File Offset: 0x000B8A90
	public override void Register(vp_EventHandler eventHandler)
	{
		base.Register(eventHandler);
		eventHandler.RegisterMessage<Vector3>("Move", new vp_Message<Vector3>.Sender<Vector3>(this.OnMessage_Move));
		eventHandler.RegisterValue<float>("SlopeLimit", new vp_Value<float>.Getter<float>(this.Get_SlopeLimit), null);
		eventHandler.RegisterValue<float>("StepOffset", new vp_Value<float>.Getter<float>(this.Get_StepOffset), null);
	}

	// Token: 0x06002F78 RID: 12152 RVA: 0x000BA8F0 File Offset: 0x000B8AF0
	public override void Unregister(vp_EventHandler eventHandler)
	{
		base.Unregister(eventHandler);
		eventHandler.UnregisterMessage<Vector3>("Move", new vp_Message<Vector3>.Sender<Vector3>(this.OnMessage_Move));
		eventHandler.UnregisterValue<float>("SlopeLimit", new vp_Value<float>.Getter<float>(this.Get_SlopeLimit), null);
		eventHandler.UnregisterValue<float>("StepOffset", new vp_Value<float>.Getter<float>(this.Get_StepOffset), null);
	}

	// Token: 0x04002D76 RID: 11638
	private CharacterController m_CharacterController;
}
