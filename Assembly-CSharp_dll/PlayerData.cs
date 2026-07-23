using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000262 RID: 610
[Serializable]
public class PlayerData : DataModule<PlayerData>
{
	// Token: 0x06000CE8 RID: 3304 RVA: 0x00035364 File Offset: 0x00033564
	public static void AssertEquals(PlayerData dataA, PlayerData dataB)
	{
		TestUtil.AreApproximatelyEqual(dataA.playerPos, dataB.playerPos, 0.01f, string.Concat(new object[]
		{
			"Player position: ",
			dataA.playerPos,
			" vs ",
			dataB.playerPos
		}));
		TestUtil.AreApproximatelyEqual(dataA.playerRotEuler, dataB.playerRotEuler, 0.01f, string.Concat(new object[]
		{
			"Player rotation: ",
			dataA.playerRotEuler,
			" vs ",
			dataB.playerRotEuler
		}));
	}

	// Token: 0x06000CE9 RID: 3305 RVA: 0x0003540C File Offset: 0x0003360C
	private static string PrintAmmo(Ammo.AmmoData[] allAmmo)
	{
		string text = "Ammo: ";
		foreach (Ammo.AmmoData ammoData in allAmmo)
		{
			text += ((ammoData == null) ? "null," : string.Concat(new object[]
			{
				ammoData.id,
				":",
				ammoData.count,
				","
			}));
		}
		return text;
	}

	// Token: 0x06000CEA RID: 3306 RVA: 0x0003547C File Offset: 0x0003367C
	private static string PrintDelayedProgress(Dictionary<ProgressDirector.ProgressType, List<float>> delayedProg)
	{
		string text = "DelayedProg: ";
		foreach (KeyValuePair<ProgressDirector.ProgressType, List<float>> keyValuePair in delayedProg)
		{
			text = text + keyValuePair.Key + ":";
			foreach (float num in keyValuePair.Value)
			{
				text = text + num + ",";
			}
			text += ";";
		}
		return text;
	}

	// Token: 0x04000BF7 RID: 3063
	public const int CURR_FORMAT_ID = 3;

	// Token: 0x04000BF8 RID: 3064
	public Vector3 playerPos;

	// Token: 0x04000BF9 RID: 3065
	public Vector3 playerRotEuler;

	// Token: 0x04000BFA RID: 3066
	public int health;

	// Token: 0x04000BFB RID: 3067
	public int energy;

	// Token: 0x04000BFC RID: 3068
	public int rad;

	// Token: 0x04000BFD RID: 3069
	public int currency;

	// Token: 0x04000BFE RID: 3070
	public Ammo.AmmoData[] ammo;

	// Token: 0x04000BFF RID: 3071
	public List<PlayerState.Upgrade> upgrades;

	// Token: 0x04000C00 RID: 3072
	public Dictionary<PlayerState.Upgrade, float> upgradeLocks;

	// Token: 0x04000C01 RID: 3073
	public List<MailDirector.Mail> mail;

	// Token: 0x04000C02 RID: 3074
	public int keys;

	// Token: 0x04000C03 RID: 3075
	public Dictionary<ProgressDirector.ProgressType, int> progress;

	// Token: 0x04000C04 RID: 3076
	public Dictionary<ProgressDirector.ProgressType, List<float>> delayedProgress;

	// Token: 0x04000C05 RID: 3077
	public int currencyEverCollected;

	// Token: 0x04000C06 RID: 3078
	public PlayerState.GameMode gameMode;

	// Token: 0x04000C07 RID: 3079
	public Identifiable.Id gameIconId = Identifiable.Id.CARROT_VEGGIE;

	// Token: 0x04000C08 RID: 3080
	public string version = "0.3.0";
}
