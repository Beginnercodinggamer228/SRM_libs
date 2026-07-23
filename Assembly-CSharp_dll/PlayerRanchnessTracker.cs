using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020002BF RID: 703
public class PlayerRanchnessTracker : MonoBehaviour
{
	// Token: 0x06000ED8 RID: 3800 RVA: 0x0003BBCC File Offset: 0x00039DCC
	public void Awake()
	{
		this.achieveDir = SRSingleton<SceneContext>.Instance.AchievementsDirector;
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.tutDir = SRSingleton<SceneContext>.Instance.TutorialDirector;
		this.member = base.GetComponent<RegionMember>();
		this.member.regionsChanged += this.InitSectorsChanged;
	}

	// Token: 0x06000ED9 RID: 3801 RVA: 0x0003BC2C File Offset: 0x00039E2C
	public void OnDestroy()
	{
		this.member.regionsChanged -= this.InitSectorsChanged;
	}

	// Token: 0x06000EDA RID: 3802 RVA: 0x0003BC48 File Offset: 0x00039E48
	private void InitSectorsChanged(List<Region> left, List<Region> joined)
	{
		this.lastOnHomeRanch = CellDirector.IsOnHomeRanch(this.member);
		this.member.regionsChanged += delegate(List<Region> left2, List<Region> joined2)
		{
			bool flag = CellDirector.IsOnHomeRanch(this.member);
			if (!flag && this.lastOnHomeRanch)
			{
				this.achieveDir.SetStat(AchievementsDirector.GameDoubleStat.LAST_LEFT_RANCH, this.timeDir.WorldTime());
				this.tutDir.OnLeftRanch();
			}
			else if (flag && !this.lastOnHomeRanch)
			{
				this.achieveDir.SetStat(AchievementsDirector.GameDoubleStat.LAST_ENTERED_RANCH, this.timeDir.WorldTime());
				this.tutDir.OnEnteredRanch();
			}
			this.lastOnHomeRanch = flag;
		};
		this.member.regionsChanged -= this.InitSectorsChanged;
	}

	// Token: 0x04000DEB RID: 3563
	private bool lastOnHomeRanch;

	// Token: 0x04000DEC RID: 3564
	private RegionMember member;

	// Token: 0x04000DED RID: 3565
	private AchievementsDirector achieveDir;

	// Token: 0x04000DEE RID: 3566
	private TimeDirector timeDir;

	// Token: 0x04000DEF RID: 3567
	private TutorialDirector tutDir;
}
