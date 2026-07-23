using System;
using System.Collections.Generic;

// Token: 0x02000259 RID: 601
[Serializable]
public class GameAchieveData : DataModule<GameAchieveData>
{
	// Token: 0x06000CD4 RID: 3284 RVA: 0x00034CD8 File Offset: 0x00032ED8
	public static void AssertEquals(GameAchieveData dataA, GameAchieveData dataB)
	{
		foreach (AchievementsDirector.GameIdDictStat gameIdDictStat in dataA.gameIdDictStatDict.Keys)
		{
		}
	}

	// Token: 0x04000BAC RID: 2988
	public const int CURR_FORMAT_ID = 1;

	// Token: 0x04000BAD RID: 2989
	public Dictionary<AchievementsDirector.GameFloatStat, float> gameFloatStatDict = new Dictionary<AchievementsDirector.GameFloatStat, float>();

	// Token: 0x04000BAE RID: 2990
	public Dictionary<AchievementsDirector.GameIntStat, int> gameIntStatDict = new Dictionary<AchievementsDirector.GameIntStat, int>();

	// Token: 0x04000BAF RID: 2991
	public Dictionary<AchievementsDirector.GameIdDictStat, Dictionary<Identifiable.Id, int>> gameIdDictStatDict = new Dictionary<AchievementsDirector.GameIdDictStat, Dictionary<Identifiable.Id, int>>();
}
