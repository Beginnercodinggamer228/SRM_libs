using System;
using UnityEngine;

// Token: 0x02000873 RID: 2163
public class vp_SlomoPickup : vp_Pickup
{
	// Token: 0x06002DBF RID: 11711 RVA: 0x000AF9BC File Offset: 0x000ADBBC
	protected override void Update()
	{
		this.UpdateMotion();
		if (this.m_Depleted)
		{
			if (this.m_Player != null && this.m_Player.Dead.Active && !this.m_RespawnTimer.Active)
			{
				this.Respawn();
				return;
			}
			if (Time.timeScale > 0.2f && !vp_TimeUtility.Paused)
			{
				vp_TimeUtility.FadeTimeScale(0.2f, 0.1f);
				return;
			}
			if (!this.m_Audio.isPlaying)
			{
				this.Remove();
				return;
			}
		}
		else if (Time.timeScale < 1f && !vp_TimeUtility.Paused)
		{
			vp_TimeUtility.FadeTimeScale(1f, 0.05f);
		}
	}

	// Token: 0x06002DC0 RID: 11712 RVA: 0x000AFA64 File Offset: 0x000ADC64
	protected override bool TryGive(vp_FPPlayerEventHandler player)
	{
		this.m_Player = player;
		return !this.m_Depleted && Time.timeScale == 1f;
	}

	// Token: 0x04002C00 RID: 11264
	private vp_FPPlayerEventHandler m_Player;
}
