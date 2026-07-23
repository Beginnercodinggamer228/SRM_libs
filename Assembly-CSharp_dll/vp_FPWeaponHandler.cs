using System;

// Token: 0x02000886 RID: 2182
public class vp_FPWeaponHandler : vp_WeaponHandler
{
	// Token: 0x06002F19 RID: 12057 RVA: 0x000B8217 File Offset: 0x000B6417
	protected virtual bool OnAttempt_AutoReload()
	{
		return this.ReloadAutomatically && this.m_Player.Reload.TryStart(true);
	}
}
