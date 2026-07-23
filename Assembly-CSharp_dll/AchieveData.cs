using System;
using System.Collections.Generic;

// Token: 0x02000255 RID: 597
[Serializable]
public class AchieveData : DataModule<AchieveData>
{
	// Token: 0x04000B9D RID: 2973
	public const int CURR_FORMAT_ID = 2;

	// Token: 0x04000B9E RID: 2974
	public Dictionary<AchievementsDirector.BoolStat, bool> boolStatDict = new Dictionary<AchievementsDirector.BoolStat, bool>();

	// Token: 0x04000B9F RID: 2975
	public Dictionary<AchievementsDirector.IntStat, int> intStatDict = new Dictionary<AchievementsDirector.IntStat, int>();

	// Token: 0x04000BA0 RID: 2976
	public Dictionary<AchievementsDirector.EnumStat, List<Enum>> enumStatDict = new Dictionary<AchievementsDirector.EnumStat, List<Enum>>();

	// Token: 0x04000BA1 RID: 2977
	public List<AchievementsDirector.Achievement> earnedAchievements = new List<AchievementsDirector.Achievement>();
}
