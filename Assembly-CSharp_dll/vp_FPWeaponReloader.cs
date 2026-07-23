using System;
using UnityEngine;

// Token: 0x02000887 RID: 2183
[RequireComponent(typeof(vp_FPWeapon))]
public class vp_FPWeaponReloader : vp_WeaponReloader
{
	// Token: 0x06002F1B RID: 12059 RVA: 0x000B823C File Offset: 0x000B643C
	protected override void OnStart_Reload()
	{
		base.OnStart_Reload();
		if (this.AnimationReload == null)
		{
			return;
		}
		if (this.m_Player.Reload.AutoDuration == 0f)
		{
			this.m_Player.Reload.AutoDuration = this.AnimationReload.length;
		}
		((vp_FPWeapon)this.m_Weapon).WeaponModel.GetComponent<Animation>().CrossFade(this.AnimationReload.name);
	}

	// Token: 0x04002D15 RID: 11541
	public AnimationClip AnimationReload;
}
