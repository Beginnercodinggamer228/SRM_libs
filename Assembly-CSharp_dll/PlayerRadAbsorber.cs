using System;
using UnityEngine;

// Token: 0x020002BE RID: 702
public class PlayerRadAbsorber : SRBehaviour
{
	// Token: 0x06000ED4 RID: 3796 RVA: 0x0003BAFE File Offset: 0x00039CFE
	public void Awake()
	{
		this.playerState = SRSingleton<SceneContext>.Instance.PlayerState;
		this.damageable = base.GetComponent<PlayerDamageable>();
	}

	// Token: 0x06000ED5 RID: 3797 RVA: 0x0003BB1C File Offset: 0x00039D1C
	public void FixedUpdate()
	{
		SRSingleton<Overlay>.Instance.SetEnableRad(this.absorbingThisFrame);
		if (this.absorbingThisFrame && !this.radAudio.IsPlaying)
		{
			this.radAudio.Play();
		}
		else if (!this.absorbingThisFrame && this.radAudio.IsPlaying)
		{
			this.radAudio.Stop(false);
		}
		this.absorbingThisFrame = false;
	}

	// Token: 0x06000ED6 RID: 3798 RVA: 0x0003BB84 File Offset: 0x00039D84
	public void Absorb(GameObject source, float rads)
	{
		int num = this.playerState.AddRads(rads);
		if (num > 0 && this.damageable.Damage(num, null))
		{
			DeathHandler.Kill(base.gameObject, DeathHandler.Source.SLIME_RAD, source, "PlayerRadAbsorber.Absorb");
		}
		this.absorbingThisFrame = true;
	}

	// Token: 0x04000DE7 RID: 3559
	public SECTR_PointSource radAudio;

	// Token: 0x04000DE8 RID: 3560
	private PlayerState playerState;

	// Token: 0x04000DE9 RID: 3561
	private PlayerDamageable damageable;

	// Token: 0x04000DEA RID: 3562
	private bool absorbingThisFrame;
}
