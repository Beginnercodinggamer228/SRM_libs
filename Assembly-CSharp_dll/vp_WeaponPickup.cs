using System;

// Token: 0x02000876 RID: 2166
public class vp_WeaponPickup : vp_Pickup
{
	// Token: 0x06002DC7 RID: 11719 RVA: 0x000AFB34 File Offset: 0x000ADD34
	protected override bool TryGive(vp_FPPlayerEventHandler player)
	{
		if (player.Dead.Active)
		{
			return false;
		}
		if (!base.TryGive(player))
		{
			return false;
		}
		player.SetWeaponByName.Try(this.InventoryName);
		if (this.AmmoIncluded > 0)
		{
			player.AddAmmo.Try(new object[]
			{
				this.InventoryName,
				this.AmmoIncluded
			});
		}
		return true;
	}

	// Token: 0x04002C03 RID: 11267
	public int AmmoIncluded;
}
