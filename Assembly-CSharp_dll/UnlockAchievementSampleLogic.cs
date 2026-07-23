using System;
using Microsoft.Xbox;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000036 RID: 54
public class UnlockAchievementSampleLogic : MonoBehaviour
{
	// Token: 0x060000DD RID: 221 RVA: 0x000098D8 File Offset: 0x00007AD8
	public void UnlockAchievement()
	{
		Gdk.Helpers.UnlockAchievement("1");
		this.output.text = "Unlocking achievement...";
	}

	// Token: 0x04000149 RID: 329
	public Text output;
}
