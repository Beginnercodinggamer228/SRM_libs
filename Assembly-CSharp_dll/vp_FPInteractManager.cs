using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000881 RID: 2177
public class vp_FPInteractManager : MonoBehaviour
{
	// Token: 0x1700030F RID: 783
	// (get) Token: 0x06002EBD RID: 11965 RVA: 0x000B50C9 File Offset: 0x000B32C9
	// (set) Token: 0x06002EBE RID: 11966 RVA: 0x000B50D1 File Offset: 0x000B32D1
	public float CrosshairTimeoutTimer { get; set; }

	// Token: 0x06002EBF RID: 11967 RVA: 0x000B50DA File Offset: 0x000B32DA
	protected virtual void Awake()
	{
		this.m_Player = base.GetComponent<vp_FPPlayerEventHandler>();
		this.m_Camera = base.GetComponentInChildren<vp_FPCamera>();
	}

	// Token: 0x06002EC0 RID: 11968 RVA: 0x000B50F4 File Offset: 0x000B32F4
	protected virtual void OnEnable()
	{
		if (this.m_Player != null)
		{
			this.Register(this.m_Player);
		}
	}

	// Token: 0x06002EC1 RID: 11969 RVA: 0x000B5110 File Offset: 0x000B3310
	protected virtual void OnDisable()
	{
		if (this.m_Player != null)
		{
			this.Unregister(this.m_Player);
		}
	}

	// Token: 0x06002EC2 RID: 11970 RVA: 0x000B512C File Offset: 0x000B332C
	public virtual void OnStart_Dead()
	{
		this.ShouldFinishInteraction();
	}

	// Token: 0x06002EC3 RID: 11971 RVA: 0x000B5138 File Offset: 0x000B3338
	public virtual void LateUpdate()
	{
		if (this.m_Player.Dead.Active)
		{
			return;
		}
		if (this.m_OriginalCrosshair == null && this.m_Player.Crosshair.Get() != null)
		{
			this.m_OriginalCrosshair = this.m_Player.Crosshair.Get();
		}
		this.InteractCrosshair();
	}

	// Token: 0x06002EC4 RID: 11972 RVA: 0x000B51A4 File Offset: 0x000B33A4
	protected virtual bool CanStart_Interact()
	{
		if (this.ShouldFinishInteraction())
		{
			return false;
		}
		if (this.m_Player.SetWeapon.Active)
		{
			return false;
		}
		if (!(this.m_LastInteractable != null))
		{
			return false;
		}
		if (this.m_LastInteractable.InteractType != vp_Interactable.vp_InteractType.Normal)
		{
			return false;
		}
		if (!this.m_LastInteractable.TryInteract(this.m_Player))
		{
			return false;
		}
		this.ResetCrosshair(false);
		return true;
	}

	// Token: 0x06002EC5 RID: 11973 RVA: 0x000B520C File Offset: 0x000B340C
	protected virtual bool ShouldFinishInteraction()
	{
		if (this.m_Player.Interactable.Get() != null)
		{
			this.m_LastInteractable = null;
			this.ResetCrosshair(true);
			this.m_Player.Interactable.Get().FinishInteraction();
			this.m_Player.Interactable.Set(null);
			return true;
		}
		return false;
	}

	// Token: 0x06002EC6 RID: 11974 RVA: 0x000B5278 File Offset: 0x000B3478
	protected virtual void InteractCrosshair()
	{
		if (this.m_Player.Crosshair.Get() == null)
		{
			return;
		}
		if (this.m_Player.Interactable.Get() != null)
		{
			return;
		}
		vp_Interactable interactable = null;
		if (this.FindInteractable(out interactable))
		{
			if (interactable != this.m_LastInteractable)
			{
				if (this.CrosshairTimeoutTimer > Time.time && this.m_LastInteractable != null && interactable.GetType() == this.m_LastInteractable.GetType())
				{
					return;
				}
				this.m_CanInteract = true;
				this.m_LastInteractable = interactable;
				if (interactable.InteractText != "" && !this.m_ShowTextTimer.Active)
				{
					vp_Timer.In(interactable.DelayShowingText, delegate()
					{
						this.m_Player.HUDText.Send(interactable.InteractText);
					}, this.m_ShowTextTimer);
				}
				if (interactable.m_InteractCrosshair == null)
				{
					return;
				}
				this.m_Player.Crosshair.Set(interactable.m_InteractCrosshair);
				return;
			}
		}
		else
		{
			this.m_CanInteract = false;
			this.ResetCrosshair(true);
		}
	}

	// Token: 0x06002EC7 RID: 11975 RVA: 0x000B53D4 File Offset: 0x000B35D4
	protected virtual bool FindInteractable(out vp_Interactable interactable)
	{
		interactable = null;
		RaycastHit raycastHit;
		if (Physics.Raycast(this.m_Camera.Transform.position, this.m_Camera.Transform.forward, out raycastHit, this.MaxInteractDistance, -754974997))
		{
			if (!this.m_Interactables.TryGetValue(raycastHit.collider, out interactable))
			{
				Dictionary<Collider, vp_Interactable> interactables = this.m_Interactables;
				Collider collider = raycastHit.collider;
				vp_Interactable component;
				interactable = (component = raycastHit.collider.GetComponent<vp_Interactable>());
				interactables.Add(collider, component);
			}
			return !(interactable == null) && (interactable.InteractDistance != 0f || raycastHit.distance < (this.m_Player.IsFirstPerson.Get() ? this.InteractDistance : this.InteractDistance3rdPerson)) && (interactable.InteractDistance <= 0f || raycastHit.distance < interactable.InteractDistance);
		}
		return false;
	}

	// Token: 0x06002EC8 RID: 11976 RVA: 0x000B54C0 File Offset: 0x000B36C0
	protected virtual void ResetCrosshair(bool reset = true)
	{
		if (this.m_OriginalCrosshair == null || this.m_Player.Crosshair.Get() == this.m_OriginalCrosshair)
		{
			return;
		}
		this.m_ShowTextTimer.Cancel();
		if (reset)
		{
			this.m_Player.Crosshair.Set(this.m_OriginalCrosshair);
		}
		this.m_LastInteractable = null;
	}

	// Token: 0x06002EC9 RID: 11977 RVA: 0x000B5530 File Offset: 0x000B3730
	protected virtual void OnControllerColliderHit(ControllerColliderHit hit)
	{
		Rigidbody attachedRigidbody = hit.collider.attachedRigidbody;
		if (attachedRigidbody == null || attachedRigidbody.isKinematic)
		{
			return;
		}
		vp_Interactable vp_Interactable = null;
		if (!this.m_Interactables.TryGetValue(hit.collider, out vp_Interactable))
		{
			this.m_Interactables.Add(hit.collider, vp_Interactable = hit.collider.GetComponent<vp_Interactable>());
		}
		if (vp_Interactable == null)
		{
			return;
		}
		if (vp_Interactable.InteractType != vp_Interactable.vp_InteractType.CollisionTrigger)
		{
			return;
		}
		hit.gameObject.SendMessage("TryInteract", this.m_Player, SendMessageOptions.DontRequireReceiver);
	}

	// Token: 0x06002ECA RID: 11978 RVA: 0x000B55BB File Offset: 0x000B37BB
	protected virtual vp_Interactable Get_Interactable()
	{
		return this.m_CurrentInteractable;
	}

	// Token: 0x06002ECB RID: 11979 RVA: 0x000B55C3 File Offset: 0x000B37C3
	protected virtual void Set_Interactable(vp_Interactable value)
	{
		this.m_CurrentInteractable = value;
	}

	// Token: 0x17000310 RID: 784
	// (get) Token: 0x06002ECC RID: 11980 RVA: 0x000B55BB File Offset: 0x000B37BB
	// (set) Token: 0x06002ECD RID: 11981 RVA: 0x000B55C3 File Offset: 0x000B37C3
	protected virtual vp_Interactable OnValue_Interactable
	{
		get
		{
			return this.m_CurrentInteractable;
		}
		set
		{
			this.m_CurrentInteractable = value;
		}
	}

	// Token: 0x06002ECE RID: 11982 RVA: 0x000B55CC File Offset: 0x000B37CC
	protected virtual bool Get_CanInteract()
	{
		return this.m_CanInteract;
	}

	// Token: 0x06002ECF RID: 11983 RVA: 0x000B55D4 File Offset: 0x000B37D4
	protected virtual void Set_CanInteract(bool value)
	{
		this.m_CanInteract = value;
	}

	// Token: 0x17000311 RID: 785
	// (get) Token: 0x06002ED0 RID: 11984 RVA: 0x000B55CC File Offset: 0x000B37CC
	// (set) Token: 0x06002ED1 RID: 11985 RVA: 0x000B55D4 File Offset: 0x000B37D4
	protected virtual bool OnValue_CanInteract
	{
		get
		{
			return this.m_CanInteract;
		}
		set
		{
			this.m_CanInteract = value;
		}
	}

	// Token: 0x06002ED2 RID: 11986 RVA: 0x000B55E0 File Offset: 0x000B37E0
	public void Register(vp_EventHandler eventHandler)
	{
		eventHandler.RegisterActivity("Interact", null, null, new vp_Activity.Condition(this.CanStart_Interact), null, null, null);
		eventHandler.RegisterActivity("Dead", new vp_Activity.Callback(this.OnStart_Dead), null, null, null, null, null);
		eventHandler.RegisterValue<vp_Interactable>("Interactable", new vp_Value<vp_Interactable>.Getter<vp_Interactable>(this.Get_Interactable), new vp_Value<vp_Interactable>.Setter<vp_Interactable>(this.Set_Interactable));
		eventHandler.RegisterValue<bool>("CanInteract", new vp_Value<bool>.Getter<bool>(this.Get_CanInteract), new vp_Value<bool>.Setter<bool>(this.Set_CanInteract));
	}

	// Token: 0x06002ED3 RID: 11987 RVA: 0x000B5674 File Offset: 0x000B3874
	public void Unregister(vp_EventHandler eventHandler)
	{
		eventHandler.UnregisterActivity("Interact", null, null, new vp_Activity.Condition(this.CanStart_Interact), null, null, null);
		eventHandler.UnregisterActivity("Dead", new vp_Activity.Callback(this.OnStart_Dead), null, null, null, null, null);
		eventHandler.UnregisterValue<vp_Interactable>("Interactable", new vp_Value<vp_Interactable>.Getter<vp_Interactable>(this.Get_Interactable), new vp_Value<vp_Interactable>.Setter<vp_Interactable>(this.Set_Interactable));
		eventHandler.UnregisterValue<bool>("CanInteract", new vp_Value<bool>.Getter<bool>(this.Get_CanInteract), new vp_Value<bool>.Setter<bool>(this.Set_CanInteract));
	}

	// Token: 0x04002C94 RID: 11412
	public float InteractDistance = 2f;

	// Token: 0x04002C95 RID: 11413
	public float InteractDistance3rdPerson = 3f;

	// Token: 0x04002C96 RID: 11414
	public float MaxInteractDistance = 25f;

	// Token: 0x04002C98 RID: 11416
	protected vp_FPPlayerEventHandler m_Player;

	// Token: 0x04002C99 RID: 11417
	protected vp_FPCamera m_Camera;

	// Token: 0x04002C9A RID: 11418
	protected vp_Interactable m_CurrentInteractable;

	// Token: 0x04002C9B RID: 11419
	protected Texture m_OriginalCrosshair;

	// Token: 0x04002C9C RID: 11420
	protected Dictionary<Collider, vp_Interactable> m_Interactables = new Dictionary<Collider, vp_Interactable>();

	// Token: 0x04002C9D RID: 11421
	protected vp_Interactable m_LastInteractable;

	// Token: 0x04002C9E RID: 11422
	protected vp_Timer.Handle m_ShowTextTimer = new vp_Timer.Handle();

	// Token: 0x04002C9F RID: 11423
	protected bool m_CanInteract;
}
