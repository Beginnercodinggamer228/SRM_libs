using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002A7 RID: 679
[CreateAssetMenu(menuName = "Game Mode")]
public class GameModeSettings : ScriptableObject
{
	// Token: 0x06000E5D RID: 3677 RVA: 0x0003A379 File Offset: 0x00038579
	public bool AllowMail()
	{
		return !this.assumeExperiencedUser || !this.suppressStory;
	}

	// Token: 0x06000E5E RID: 3678 RVA: 0x0003A390 File Offset: 0x00038590
	public double? EndTime()
	{
		if (this.endAtNoonDay > 0.0)
		{
			return new double?(86400.0 * (this.endAtNoonDay - 0.5));
		}
		return null;
	}

	// Token: 0x06000E5F RID: 3679 RVA: 0x0003A3D7 File Offset: 0x000385D7
	public static float GetScoreMultiplier(Identifiable.Id id)
	{
		if (!GameModeSettings.scoreMultiplierMap.ContainsKey(id))
		{
			return 0.05f;
		}
		return GameModeSettings.scoreMultiplierMap.Get(id);
	}

	// Token: 0x06000E60 RID: 3680 RVA: 0x0003A3F7 File Offset: 0x000385F7
	public static bool PlortBonusReached(int plortsCollected)
	{
		return plortsCollected >= 25;
	}

	// Token: 0x04000D60 RID: 3424
	[Tooltip("Starting money")]
	public int initCurrency;

	// Token: 0x04000D61 RID: 3425
	[Tooltip("Currency penalty on death as portion of total")]
	public float pctCurrencyLostOnDeath;

	// Token: 0x04000D62 RID: 3426
	[Tooltip("Time penalty on death in hours")]
	public float hoursLostOnDeath;

	// Token: 0x04000D63 RID: 3427
	[Tooltip("Whether to have a til-dawn sleep on death")]
	public bool hoursTilDawnOnDeath;

	// Token: 0x04000D64 RID: 3428
	[Tooltip("The day on which to begin our exchange offers")]
	public int exchangeStartDay;

	// Token: 0x04000D65 RID: 3429
	[Tooltip("Whether all exchange rewards are gold plorts")]
	public bool exchangeRewardsGoldPlorts;

	// Token: 0x04000D66 RID: 3430
	[Tooltip("Whether the plort market prices incorporate noise and saturation")]
	public bool plortMarketDynamic;

	// Token: 0x04000D67 RID: 3431
	[Tooltip("The day on which we should end the game, or never if 0.")]
	public double endAtNoonDay;

	// Token: 0x04000D68 RID: 3432
	[Tooltip("The UI to use at the end of the game.")]
	public GameObject endGameUIPrefab;

	// Token: 0x04000D69 RID: 3433
	[Tooltip("Whether the game mode assumes the player has experience, suppress tutorials, etc")]
	public bool assumeExperiencedUser;

	// Token: 0x04000D6A RID: 3434
	[Tooltip("Whether our upgrades bypass their normal staggering time delays.")]
	public bool immediateUpgrades;

	// Token: 0x04000D6B RID: 3435
	[Tooltip("Whether we enable 7Z Partner rewards.")]
	public bool enablePartnerRewards = true;

	// Token: 0x04000D6C RID: 3436
	[Tooltip("Disables Hobson journal entries and Casey emails when true.")]
	public bool suppressStory;

	// Token: 0x04000D6D RID: 3437
	[Tooltip("All damage will be multiplied by this factor.")]
	public float playerDamageMultiplier = 1f;

	// Token: 0x04000D6E RID: 3438
	[Tooltip("Prevents Tarr from forming and direct attacking from ferals.")]
	public bool preventHostiles;

	// Token: 0x04000D6F RID: 3439
	[Tooltip("Enables blueprints to be discovered.")]
	public bool blueprintsEnabled = true;

	// Token: 0x04000D70 RID: 3440
	[Tooltip("Enables Ogden missions to be unlockable.")]
	public bool enableOgdenMissions = true;

	// Token: 0x04000D71 RID: 3441
	[Tooltip("Enables Mochi missions to be unlockable.")]
	public bool enableMochiMissions = true;

	// Token: 0x04000D72 RID: 3442
	[Tooltip("Enables Viktor missions to be unlockable.")]
	public bool enableViktorMissions = true;

	// Token: 0x04000D73 RID: 3443
	[Tooltip("Prefab to instantiate when a new game is loaded.")]
	public GameObject newGamePrefab;

	// Token: 0x04000D74 RID: 3444
	[Tooltip("Enables event/party gordos.")]
	public bool enableEventGordos = true;

	// Token: 0x04000D75 RID: 3445
	[Tooltip("Enables Wiggly Wonderland echo note cluster nodes.")]
	public bool enableEchoNoteGordos = true;

	// Token: 0x04000D76 RID: 3446
	[Tooltip("Enables DLC.")]
	public bool enableDLC = true;

	// Token: 0x04000D77 RID: 3447
	[Tooltip("Minimum plorts required to add a score multiplier.")]
	public const int scoreMultiplierPlortsRequired = 25;

	// Token: 0x04000D78 RID: 3448
	[Tooltip("Default score multiplier applied for each plort type deposited.")]
	public const float scoreMultiplierDefault = 0.05f;

	// Token: 0x04000D79 RID: 3449
	[Tooltip("Custom score multipliers applied for each plort type deposited.")]
	public static Dictionary<Identifiable.Id, float> scoreMultiplierMap = new Dictionary<Identifiable.Id, float>(Identifiable.idComparer)
	{
		{
			Identifiable.Id.DERVISH_PLORT,
			0.08f
		},
		{
			Identifiable.Id.FIRE_PLORT,
			0.08f
		},
		{
			Identifiable.Id.MOSAIC_PLORT,
			0.08f
		},
		{
			Identifiable.Id.QUANTUM_PLORT,
			0.08f
		},
		{
			Identifiable.Id.TANGLE_PLORT,
			0.08f
		}
	};
}
