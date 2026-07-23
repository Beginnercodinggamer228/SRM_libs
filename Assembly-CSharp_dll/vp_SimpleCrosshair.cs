using System;
using UnityEngine;

// Token: 0x02000853 RID: 2131
public class vp_SimpleCrosshair : MonoBehaviour, EventHandlerRegistrable
{
	// Token: 0x06002CE9 RID: 11497 RVA: 0x000A98A6 File Offset: 0x000A7AA6
	protected virtual void Awake()
	{
		this.m_Player = (UnityEngine.Object.FindObjectOfType(typeof(vp_FPPlayerEventHandler)) as vp_FPPlayerEventHandler);
	}

	// Token: 0x06002CEA RID: 11498 RVA: 0x000A98C2 File Offset: 0x000A7AC2
	protected virtual void OnEnable()
	{
		if (this.m_Player != null)
		{
			this.Register(this.m_Player);
		}
	}

	// Token: 0x06002CEB RID: 11499 RVA: 0x000A98DE File Offset: 0x000A7ADE
	protected virtual void OnDisable()
	{
		if (this.m_Player != null)
		{
			this.Unregister(this.m_Player);
		}
	}

	// Token: 0x06002CEC RID: 11500 RVA: 0x000A98FC File Offset: 0x000A7AFC
	private void OnGUI()
	{
		if (this.m_ImageCrosshair == null)
		{
			return;
		}
		if (this.HideOnFirstPersonZoom && this.m_Player.Zoom.Active && this.m_Player.IsFirstPerson.Get())
		{
			return;
		}
		if (this.HideOnDeath && this.m_Player.Dead.Active)
		{
			return;
		}
		GUI.color = new Color(1f, 1f, 1f, 0.8f);
		GUI.DrawTexture(new Rect((float)Screen.width * 0.5f - (float)this.m_ImageCrosshair.width * 0.5f, (float)Screen.height * 0.5f - (float)this.m_ImageCrosshair.height * 0.5f, (float)this.m_ImageCrosshair.width, (float)this.m_ImageCrosshair.height), this.m_ImageCrosshair);
		GUI.color = Color.white;
	}

	// Token: 0x06002CED RID: 11501 RVA: 0x000A99F3 File Offset: 0x000A7BF3
	protected virtual Texture Get_Crosshair()
	{
		return this.m_ImageCrosshair;
	}

	// Token: 0x06002CEE RID: 11502 RVA: 0x000A99FB File Offset: 0x000A7BFB
	protected virtual void Set_Crosshair(Texture value)
	{
		this.m_ImageCrosshair = value;
	}

	// Token: 0x170002CF RID: 719
	// (get) Token: 0x06002CEF RID: 11503 RVA: 0x000A99F3 File Offset: 0x000A7BF3
	// (set) Token: 0x06002CF0 RID: 11504 RVA: 0x000A99FB File Offset: 0x000A7BFB
	protected virtual Texture OnValue_Crosshair
	{
		get
		{
			return this.m_ImageCrosshair;
		}
		set
		{
			this.m_ImageCrosshair = value;
		}
	}

	// Token: 0x06002CF1 RID: 11505 RVA: 0x000A9A04 File Offset: 0x000A7C04
	public void Register(vp_EventHandler eventHandler)
	{
		eventHandler.RegisterValue<Texture>("Crosshair", new vp_Value<Texture>.Getter<Texture>(this.Get_Crosshair), new vp_Value<Texture>.Setter<Texture>(this.Set_Crosshair));
	}

	// Token: 0x06002CF2 RID: 11506 RVA: 0x000A9A2B File Offset: 0x000A7C2B
	public void Unregister(vp_EventHandler eventHandler)
	{
		eventHandler.UnregisterValue<Texture>("Crosshair", new vp_Value<Texture>.Getter<Texture>(this.Get_Crosshair), new vp_Value<Texture>.Setter<Texture>(this.Set_Crosshair));
	}

	// Token: 0x04002ACE RID: 10958
	public Texture m_ImageCrosshair;

	// Token: 0x04002ACF RID: 10959
	public bool HideOnFirstPersonZoom = true;

	// Token: 0x04002AD0 RID: 10960
	public bool HideOnDeath = true;

	// Token: 0x04002AD1 RID: 10961
	protected vp_FPPlayerEventHandler m_Player;
}
