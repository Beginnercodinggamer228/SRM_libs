using System;
using UnityEngine;

// Token: 0x020002B0 RID: 688
public class PlayerDamageable : SRBehaviour, Damageable
{
	// Token: 0x06000E9E RID: 3742 RVA: 0x0003B0C4 File Offset: 0x000392C4
	private void Start()
	{
		this.playerState = SRSingleton<SceneContext>.Instance.PlayerState;
		this.playerAudio = base.GetComponent<SECTR_AudioSource>();
		this.screenShaker = base.GetComponent<ScreenShaker>();
	}

	// Token: 0x06000E9F RID: 3743 RVA: 0x0003B0F0 File Offset: 0x000392F0
	public bool Damage(int healthLoss, GameObject source)
	{
		healthLoss = Mathf.RoundToInt((float)healthLoss * SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().playerDamageMultiplier);
		if (this.playerState.CanBeDamaged())
		{
			SRSingleton<Overlay>.Instance.PlayDamage();
			this.playerAudio.Cue = this.GetDamageCue(source);
			this.playerAudio.Play();
			this.screenShaker.ShakeDamage(0.2f * (float)healthLoss);
			return this.playerState.Damage(healthLoss, source);
		}
		return false;
	}

	// Token: 0x06000EA0 RID: 3744 RVA: 0x0003B171 File Offset: 0x00039371
	private SECTR_AudioCue GetDamageCue(GameObject source)
	{
		if (source != null && Identifiable.GetId(source) == Identifiable.Id.GLITCH_TARR_SLIME)
		{
			return SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch.damageLossExposure.onExposedSFX;
		}
		return this.damagedCue;
	}

	// Token: 0x04000DB6 RID: 3510
	private PlayerState playerState;

	// Token: 0x04000DB7 RID: 3511
	private SECTR_AudioSource playerAudio;

	// Token: 0x04000DB8 RID: 3512
	private ScreenShaker screenShaker;

	// Token: 0x04000DB9 RID: 3513
	public SECTR_AudioCue damagedCue;

	// Token: 0x04000DBA RID: 3514
	private const float PER_DAMAGE_SCREEN_SHAKE = 0.2f;
}
