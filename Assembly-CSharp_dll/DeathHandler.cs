using System;
using UnityEngine;

// Token: 0x0200010F RID: 271
public static class DeathHandler
{
	// Token: 0x060005F5 RID: 1525 RVA: 0x00021EF4 File Offset: 0x000200F4
	public static void Kill(GameObject gameObject, DeathHandler.Source source, GameObject sourceGameObject, string stackTrace)
	{
		DeathHandler.Interface interfaceComponent = gameObject.GetInterfaceComponent<DeathHandler.Interface>();
		if (interfaceComponent != null)
		{
			interfaceComponent.OnDeath(source, sourceGameObject, stackTrace);
			return;
		}
		Destroyer.DestroyActor(gameObject, stackTrace, true);
	}

	// Token: 0x02000110 RID: 272
	public enum Source
	{
		// Token: 0x040005BB RID: 1467
		UNDEFINED,
		// Token: 0x040005BC RID: 1468
		SLIME_ATTACK,
		// Token: 0x040005BD RID: 1469
		SLIME_ATTACK_PLAYER,
		// Token: 0x040005BE RID: 1470
		SLIME_CRYSTAL_SPIKES,
		// Token: 0x040005BF RID: 1471
		SLIME_DAMAGE_PLAYER_ON_TOUCH,
		// Token: 0x040005C0 RID: 1472
		SLIME_EXPLODE,
		// Token: 0x040005C1 RID: 1473
		SLIME_IGNITE,
		// Token: 0x040005C2 RID: 1474
		SLIME_RAD,
		// Token: 0x040005C3 RID: 1475
		CHICKEN_VAMPIRISM,
		// Token: 0x040005C4 RID: 1476
		KILL_ON_TRIGGER,
		// Token: 0x040005C5 RID: 1477
		EMERGENCY_RETURN,
		// Token: 0x040005C6 RID: 1478
		FALL_DAMAGE
	}

	// Token: 0x02000111 RID: 273
	public interface Interface
	{
		// Token: 0x060005F6 RID: 1526
		void OnDeath(DeathHandler.Source source, GameObject sourceGameObject, string stackTrace);
	}
}
