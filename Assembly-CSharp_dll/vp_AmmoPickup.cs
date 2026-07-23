using System;

// Token: 0x02000870 RID: 2160
public class vp_AmmoPickup : vp_Pickup
{
	// Token: 0x06002DB1 RID: 11697 RVA: 0x000AF13C File Offset: 0x000AD33C
	protected override bool TryGive(vp_FPPlayerEventHandler player)
	{
		if (player.Dead.Active)
		{
			return false;
		}
		int i = 0;
		while (i < this.GiveAmount)
		{
			if (!base.TryGive(player))
			{
				if (this.TryReloadIfEmpty(player))
				{
					base.TryGive(player);
					return true;
				}
				return false;
			}
			else
			{
				i++;
			}
		}
		this.TryReloadIfEmpty(player);
		return true;
	}

	// Token: 0x06002DB2 RID: 11698 RVA: 0x000AF190 File Offset: 0x000AD390
	private bool TryReloadIfEmpty(vp_FPPlayerEventHandler player)
	{
		return player.CurrentWeaponAmmoCount.Get() <= 0 && !(player.CurrentWeaponClipType.Get() != this.InventoryName) && player.Reload.TryStart(true);
	}

	// Token: 0x04002BDF RID: 11231
	public int GiveAmount = 1;
}
