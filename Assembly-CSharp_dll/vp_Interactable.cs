using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200086C RID: 2156
[RequireComponent(typeof(Collider))]
public abstract class vp_Interactable : MonoBehaviour
{
	// Token: 0x06002DA0 RID: 11680 RVA: 0x000AEB80 File Offset: 0x000ACD80
	protected virtual void Start()
	{
		this.m_Transform = base.transform;
		if (this.RecipientTags.Count == 0)
		{
			this.RecipientTags.Add("Player");
		}
		if (this.InteractType == vp_Interactable.vp_InteractType.Trigger && base.GetComponent<Collider>() != null)
		{
			base.GetComponent<Collider>().isTrigger = true;
		}
	}

	// Token: 0x06002DA1 RID: 11681 RVA: 0x00003296 File Offset: 0x00001496
	protected virtual void OnEnable()
	{
	}

	// Token: 0x06002DA2 RID: 11682 RVA: 0x00003296 File Offset: 0x00001496
	protected virtual void OnDisable()
	{
	}

	// Token: 0x06002DA3 RID: 11683 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	public virtual bool TryInteract(vp_PlayerEventHandler player)
	{
		return false;
	}

	// Token: 0x06002DA4 RID: 11684 RVA: 0x000AEBDC File Offset: 0x000ACDDC
	protected virtual void OnTriggerEnter(Collider col)
	{
		if (this.InteractType != vp_Interactable.vp_InteractType.Trigger)
		{
			return;
		}
		foreach (string b in this.RecipientTags)
		{
			if (col.gameObject.tag == b)
			{
				goto IL_4F;
			}
		}
		return;
		IL_4F:
		this.m_Player = col.gameObject.GetComponent<vp_PlayerEventHandler>();
		if (this.m_Player == null)
		{
			return;
		}
		this.TryInteract(this.m_Player);
	}

	// Token: 0x06002DA5 RID: 11685 RVA: 0x00003296 File Offset: 0x00001496
	public virtual void FinishInteraction()
	{
	}

	// Token: 0x04002BC4 RID: 11204
	public vp_Interactable.vp_InteractType InteractType;

	// Token: 0x04002BC5 RID: 11205
	public List<string> RecipientTags = new List<string>();

	// Token: 0x04002BC6 RID: 11206
	public float InteractDistance;

	// Token: 0x04002BC7 RID: 11207
	public Texture m_InteractCrosshair;

	// Token: 0x04002BC8 RID: 11208
	public string InteractText = "";

	// Token: 0x04002BC9 RID: 11209
	public float DelayShowingText = 2f;

	// Token: 0x04002BCA RID: 11210
	protected Transform m_Transform;

	// Token: 0x04002BCB RID: 11211
	protected vp_FPController m_Controller;

	// Token: 0x04002BCC RID: 11212
	protected vp_FPCamera m_Camera;

	// Token: 0x04002BCD RID: 11213
	protected vp_WeaponHandler m_WeaponHandler;

	// Token: 0x04002BCE RID: 11214
	protected vp_PlayerEventHandler m_Player;

	// Token: 0x0200086D RID: 2157
	public enum vp_InteractType
	{
		// Token: 0x04002BD0 RID: 11216
		Normal,
		// Token: 0x04002BD1 RID: 11217
		Trigger,
		// Token: 0x04002BD2 RID: 11218
		CollisionTrigger
	}
}
