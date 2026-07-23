using System;
using UnityEngine;

// Token: 0x02000897 RID: 2199
public class vp_WeaponReloader : MonoBehaviour, EventHandlerRegistrable
{
	// Token: 0x06003030 RID: 12336 RVA: 0x000BDF54 File Offset: 0x000BC154
	protected virtual void Awake()
	{
		this.m_Audio = base.GetComponent<AudioSource>();
		this.m_Player = (vp_PlayerEventHandler)base.transform.root.GetComponentInChildren(typeof(vp_PlayerEventHandler));
	}

	// Token: 0x06003031 RID: 12337 RVA: 0x000BDF87 File Offset: 0x000BC187
	protected virtual void Start()
	{
		this.m_Weapon = base.transform.GetComponent<vp_Weapon>();
	}

	// Token: 0x06003032 RID: 12338 RVA: 0x000BDF9A File Offset: 0x000BC19A
	protected virtual void OnEnable()
	{
		if (this.m_Player != null)
		{
			this.Register(this.m_Player);
		}
	}

	// Token: 0x06003033 RID: 12339 RVA: 0x000BDFB6 File Offset: 0x000BC1B6
	protected virtual void OnDisable()
	{
		if (this.m_Player != null)
		{
			this.Unregister(this.m_Player);
		}
	}

	// Token: 0x06003034 RID: 12340 RVA: 0x000BDFD4 File Offset: 0x000BC1D4
	protected virtual bool CanStart_Reload()
	{
		return this.m_Player.CurrentWeaponWielded.Get() && (this.m_Player.CurrentWeaponMaxAmmoCount.Get() == 0 || this.m_Player.CurrentWeaponAmmoCount.Get() != this.m_Player.CurrentWeaponMaxAmmoCount.Get()) && this.m_Player.CurrentWeaponClipCount.Get() >= 1;
	}

	// Token: 0x06003035 RID: 12341 RVA: 0x000BE05C File Offset: 0x000BC25C
	protected virtual void OnStart_Reload()
	{
		this.m_Player.Reload.AutoDuration = this.m_Player.CurrentWeaponReloadDuration.Get();
		if (this.m_Audio != null)
		{
			this.m_Audio.pitch = Time.timeScale;
			this.m_Audio.PlayOneShot(this.SoundReload);
		}
	}

	// Token: 0x06003036 RID: 12342 RVA: 0x000BE0BD File Offset: 0x000BC2BD
	protected virtual void OnStop_Reload()
	{
		this.m_Player.RefillCurrentWeapon.Try();
	}

	// Token: 0x06003037 RID: 12343 RVA: 0x000BE0D5 File Offset: 0x000BC2D5
	protected virtual float Get_CurrentWeaponReloadDuration()
	{
		return this.ReloadDuration;
	}

	// Token: 0x17000359 RID: 857
	// (get) Token: 0x06003038 RID: 12344 RVA: 0x000BE0D5 File Offset: 0x000BC2D5
	protected virtual float OnValue_CurrentWeaponReloadDuration
	{
		get
		{
			return this.ReloadDuration;
		}
	}

	// Token: 0x06003039 RID: 12345 RVA: 0x000BE0E0 File Offset: 0x000BC2E0
	public void Register(vp_EventHandler eventHandler)
	{
		eventHandler.RegisterValue<float>("CurrentWeaponReloadDuration", new vp_Value<float>.Getter<float>(this.Get_CurrentWeaponReloadDuration), null);
		eventHandler.RegisterActivity("Reload", new vp_Activity.Callback(this.OnStart_Reload), new vp_Activity.Callback(this.OnStop_Reload), new vp_Activity.Condition(this.CanStart_Reload), null, null, null);
	}

	// Token: 0x0600303A RID: 12346 RVA: 0x000BE13C File Offset: 0x000BC33C
	public void Unregister(vp_EventHandler eventHandler)
	{
		eventHandler.UnregisterValue<float>("CurrentWeaponReloadDuration", new vp_Value<float>.Getter<float>(this.Get_CurrentWeaponReloadDuration), null);
		eventHandler.UnregisterActivity("Reload", new vp_Activity.Callback(this.OnStart_Reload), new vp_Activity.Callback(this.OnStop_Reload), new vp_Activity.Condition(this.CanStart_Reload), null, null, null);
	}

	// Token: 0x04002E25 RID: 11813
	protected vp_Weapon m_Weapon;

	// Token: 0x04002E26 RID: 11814
	protected vp_PlayerEventHandler m_Player;

	// Token: 0x04002E27 RID: 11815
	protected AudioSource m_Audio;

	// Token: 0x04002E28 RID: 11816
	public AudioClip SoundReload;

	// Token: 0x04002E29 RID: 11817
	public float ReloadDuration = 1f;
}
