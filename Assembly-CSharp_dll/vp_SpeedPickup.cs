using System;

// Token: 0x02000874 RID: 2164
public class vp_SpeedPickup : vp_Pickup
{
	// Token: 0x06002DC2 RID: 11714 RVA: 0x000AFA8C File Offset: 0x000ADC8C
	protected override void Update()
	{
		this.UpdateMotion();
		if (this.m_Depleted && !this.m_Audio.isPlaying)
		{
			this.Remove();
		}
	}

	// Token: 0x06002DC3 RID: 11715 RVA: 0x000AFAB0 File Offset: 0x000ADCB0
	protected override bool TryGive(vp_FPPlayerEventHandler player)
	{
		if (this.m_Timer.Active)
		{
			return false;
		}
		player.SetState("MegaSpeed", true, true, false);
		vp_Timer.In(this.RespawnDuration, delegate()
		{
			player.SetState("MegaSpeed", false, true, false);
		}, this.m_Timer);
		return true;
	}

	// Token: 0x04002C01 RID: 11265
	protected vp_Timer.Handle m_Timer = new vp_Timer.Handle();
}
